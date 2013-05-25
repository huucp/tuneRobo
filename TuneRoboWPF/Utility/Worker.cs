using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TuneRoboWPF.Utility
{
    public class ImageDownload
    {
        public delegate void SuccessfullyEventHandler(BitmapImage sender);

        public event SuccessfullyEventHandler DownloadCompleted;

        public void OnDownloadCompleted(BitmapImage sender)
        {
            SuccessfullyEventHandler handler = DownloadCompleted;
            if (handler != null) handler(sender);
        }

        public delegate void ErrorEventHandler(object sender, string msg);

        public event ErrorEventHandler DownloadFailed;

        public void OnDownloadFailed(object sender, string msg)
        {
            ErrorEventHandler handler = DownloadFailed;
            if (handler != null) handler(sender, msg);
        }

        private ulong ImageID { get; set; }
        private string ImageUrl { get; set; }

        public ImageDownload(ulong id, string url)
        {
            ImageID = id;
            ImageUrl = url;
        }
        public void Process()
        {
            DownloadRemoteImageFile(ImageUrl, ImageID, ImageID + ".jpg");
        }

        private string FindImageInStorage(ulong id)
        {
            string filename = Path.Combine(GlobalVariables.AppDataFolder, id.ToString() + ".jpg");
            return File.Exists(filename) ? filename : null;
        }

        private void DownloadRemoteImageFile(string url, ulong id, string filename)
        {
            if(GlobalVariables.ImageDictionary.ContainsKey(url))
            {
                OnDownloadCompleted(GlobalVariables.ImageDictionary[url]);
                return;                
            }
            var cachedImagePath = FindImageInStorage(id);
            if(cachedImagePath!=null)
            {
                using (var memory = new MemoryStream(File.ReadAllBytes(cachedImagePath)))
                {
                    memory.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    bitmapImage.Freeze();

                    GlobalVariables.ImageDictionary.Add(url, bitmapImage);

                    OnDownloadCompleted(bitmapImage);
                    return;
                }
            }

            var urlUri = new Uri(url);
            var request = WebRequest.CreateDefault(urlUri);

            var buffer = new byte[4096];

            string savedPath = Path.Combine(GlobalVariables.AppDataFolder, filename);
            using (var target = new FileStream(savedPath, FileMode.Create, FileAccess.Write))
            {
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var mStream = new MemoryStream())
                        {
                            int read;
                            Debug.Assert(stream != null, "stream is null");
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                target.Write(buffer, 0, read);
                                mStream.Write(buffer,0,read);
                            }

                            mStream.Position = 0;
                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = mStream;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();

                            bitmapImage.Freeze();

                            GlobalVariables.ImageDictionary.Add(url, bitmapImage);

                            OnDownloadCompleted(bitmapImage);
                        }
                    }
                }
            }            
        }
    }

    public class ImageDownloadWorker
    {
        public static BlockingQueue<ImageDownload> ListsJobs = new BlockingQueue<ImageDownload>(false);

        private static readonly Lazy<ImageDownloadWorker> Lazy = new Lazy<ImageDownloadWorker>(() => new ImageDownloadWorker());

        public static ImageDownloadWorker Instance { get { return Lazy.Value; } }

        private Thread backgroundWorker;

        private ImageDownloadWorker()
        {
            backgroundWorker = new Thread(MainProcess)
            {
                IsBackground = true,
                Name = "Worker thread"
            };

            backgroundWorker.Start();
        }

        public void AddJob(ImageDownload request)
        {
            ListsJobs.Enqueue(request);
        }

        private void MainProcess()
        {
            while (true)
            {
                var currentJob = ListsJobs.GetFirst();
                currentJob.Process();
            }
        }
    }

    public class StoreWorker
    {
        public static BlockingQueue<IRequest> ListsJobs = new BlockingQueue<IRequest>(true);

        private static readonly Lazy<StoreWorker> Lazy = new Lazy<StoreWorker>(() => new StoreWorker());

        public static StoreWorker Instance { get { return Lazy.Value; } }

        private Thread backgroundWorker;

        private bool _isClearWorker;
        public bool IsClearWorker
        {
            get { return _isClearWorker; }
            set
            {
                _isClearWorker = value;
                ListsJobs.ClearQueue = value;
            }
        }

        private StoreWorker()
        {
            backgroundWorker = new Thread(MainProcess)
            {
                IsBackground = true,
                Name = "Worker thread"
            };

            backgroundWorker.Start();
        }

        public void AddJob(IRequest request)
        {
            ListsJobs.Enqueue(request);
        }

        private static void MainProcess()
        {
            while (true)
            {
                var currentJob = ListsJobs.GetFirst();
                currentJob.Process();
            }
        }
    }

    public class RoboWorker
    {
        public static BlockingQueue<IRequest> ListsJobs = new BlockingQueue<IRequest>(true);

        private static readonly Lazy<RoboWorker> Lazy = new Lazy<RoboWorker>(() => new RoboWorker());

        public static RoboWorker Instance { get { return Lazy.Value; } }

        private Thread backgroundWorker;

        private RoboWorker()
        {
            backgroundWorker = new Thread(MainProcess)
            {
                IsBackground = true,
                Name = "Worker thread"
            };

            backgroundWorker.Start();
        }

        public void AddJob(IRequest request)
        {
            ListsJobs.Enqueue(request);
        }

        private static void MainProcess()
        {
            while (true)
            {
                var currentJob = ListsJobs.GetFirst();
                currentJob.Process();
            }
        }
    }

    public interface IRequest
    {
        object Process();
    }

    public class BlockingQueue<T>
    {
        private const int MaxWaitingCount = 2;
        private Queue<T> q = new Queue<T>();
        public bool ClearQueue { get; set; }

        public BlockingQueue(bool clearQueue = false)
        {
            ClearQueue = clearQueue;
        }

        public void Enqueue(T element)
        {
            lock (q)
            {
                if (q.Count >= MaxWaitingCount && ClearQueue)
                {
                    Console.WriteLine("clear queue");
                    q.Clear();
                }
                q.Enqueue(element);
                Monitor.PulseAll(q);
            }
        }

        public T GetFirst()
        {
            lock (q)
            {
                while (IsEmpty())
                {
                    Monitor.Wait(q);
                }
                return q.Dequeue();
            }
        }

        public bool IsEmpty()
        {
            lock (q)
            {
                return q.Count == 0;
            }
        }
    }
}

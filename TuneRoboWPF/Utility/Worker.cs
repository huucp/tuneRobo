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

        private string ImageUrl { get; set; }

        public ImageDownload(string url)
        {
            ImageUrl = url;
        }
        public void Process()
        {
            string filename = GlobalFunction.CalculateMD5Hash(ImageUrl);
            DownloadRemoteImageFile(ImageUrl, filename + ".jpg");
        }

        private string FindImageInStorage(string filename)
        {
            string filePath = Path.Combine(GlobalVariables.AppDataFolder, filename);
            var fileInfo = new FileInfo(filePath);
            return (File.Exists(filePath) && fileInfo.Length != 0) ? filePath : null;
        }

        private bool LoadImageInStorage(string cachedImagePath,string url)
        {
            bool loadImage = true;
            if (cachedImagePath != null)
            {
                using (var memory = new MemoryStream(File.ReadAllBytes(cachedImagePath)))
                {
                    memory.Position = 0;
                    
                    var bitmapImage = new BitmapImage();
                    try
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        bitmapImage.Freeze();

                    }
                    catch (Exception e)
                    {
                        loadImage = false;
                        File.Delete(cachedImagePath);
                        Debug.Fail(e.Message);
                    }

                    if (loadImage)
                    {
                        GlobalVariables.ImageDictionary.Add(url, bitmapImage);

                        OnDownloadCompleted(bitmapImage);                        
                    }
                }
            }
            else
            {
                loadImage = false;
            }
            return loadImage;
        }

        private void DownloadImageFromUrl(string url,string filename)
        {
            Uri urlUri = null;
            try
            {
                urlUri = new Uri(url);
            }
            catch (Exception e)
            {
                OnDownloadFailed(null, e.Message);
                return;
            }
            var request = WebRequest.CreateDefault(urlUri);
            request.Timeout = 5000;

            var buffer = new byte[4096];

            string savedPath = Path.Combine(GlobalVariables.AppDataFolder, filename);
            try
            {
                //using (var target = new FileStream(savedPath, FileMode.Create, FileAccess.Write))
                //{
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
                                //target.Write(buffer, 0, read);
                                mStream.Write(buffer, 0, read);
                            }

                            File.WriteAllBytes(savedPath, mStream.ToArray());

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
                //}
            }
            catch (Exception e)
            {

                OnDownloadFailed(null, e.Message);
            }
        }

        private void DownloadRemoteImageFile(string url, string filename)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                OnDownloadFailed(null, "URL invalid");
                return;
            }
            if (GlobalVariables.ImageDictionary.ContainsKey(url))
            {
                OnDownloadCompleted(GlobalVariables.ImageDictionary[url]);
                return;
            }
            var cachedImagePath = FindImageInStorage(filename);
            if (LoadImageInStorage(cachedImagePath,url)) return;
            DownloadImageFromUrl(url,filename);
            
        }
    }

    public class ImageDownloadWorker
    {
        public static BlockingQueue<ImageDownload> ListsJobs = new BlockingQueue<ImageDownload>();

        private static readonly Lazy<ImageDownloadWorker> Lazy = new Lazy<ImageDownloadWorker>(() => new ImageDownloadWorker());

        public static ImageDownloadWorker Instance { get { return Lazy.Value; } }

        private Thread backgroundWorker;

        private ImageDownloadWorker()
        {
            backgroundWorker = new Thread(MainProcess)
            {
                IsBackground = true,
                Name = "Image Download Worker",
                Priority = ThreadPriority.BelowNormal
            };

            backgroundWorker.Start();
        }

        public void AddDownload(ImageDownload request)
        {
            ListsJobs.Add(request);
        }

        public void ClearAll()
        {
            ListsJobs.ClearAll();
        }

        private void MainProcess()
        {
            while (true)
            {
                var currentJob = ListsJobs.Get();
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

        private bool IsClearWorker
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
                Name = "Store Worker"
            };
            IsClearWorker = true;
            backgroundWorker.Start();
        }

        public void AddRequest(IRequest request)
        {
            ListsJobs.Add(request);
        }

        public void ForceAddRequest(IRequest request)
        {
            IsClearWorker = false;
            ListsJobs.Add(request);
            IsClearWorker = true;
        }

        private static void MainProcess()
        {
            while (true)
            {
                var currentJob = ListsJobs.Get();
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
                Name = "Robot Worker"
            };

            backgroundWorker.Start();
        }

        public void AddJob(IRequest request)
        {
            ListsJobs.Add(request);
        }

        private static void MainProcess()
        {
            while (true)
            {
                var currentJob = ListsJobs.Get();
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
        private const int MaxWaitingCount = 1;
        private Queue<T> q = new Queue<T>();
        public bool ClearQueue { get; set; }

        public BlockingQueue(bool clearQueue = false)
        {
            ClearQueue = clearQueue;
        }

        public void Add(T element)
        {
            lock (q)
            {
                if (q.Count > MaxWaitingCount && ClearQueue)
                {
                    Console.WriteLine("clear queue");
                    q.Clear();
                }
                q.Enqueue(element);
                Monitor.PulseAll(q);
            }
        }

        public T Get()
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

        private bool IsEmpty()
        {
            lock (q)
            {
                return q.Count == 0;
            }
        }

        public void ClearAll()
        {
            lock(q)
            {
                q.Clear();
            }
        }
    }

    public class BlockingStack<T>
    {
        private const int MaxWaitingCount = 1;
        private Stack<T> stack = new Stack<T>();
        public bool ClearStack { get; set; }

        public BlockingStack(bool clearStack = false)
        {
            ClearStack = clearStack;
        }

        public void Add(T element)
        {
            lock (stack)
            {
                if (stack.Count > MaxWaitingCount && ClearStack)
                {
                    Console.WriteLine("clear stack");
                    stack.Clear();
                }
                stack.Push(element);                
                Monitor.PulseAll(stack);
            }
        }

        public T Get()
        {
            lock (stack)
            {
                while (IsEmpty())
                {
                    Monitor.Wait(stack);
                }
                return stack.Pop();
            }
        }

        public bool IsEmpty()
        {
            lock (stack)
            {
                return stack.Count == 0;
            }
        }
    }
}

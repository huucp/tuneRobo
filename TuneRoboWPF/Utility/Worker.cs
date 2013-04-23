using System;
using System.Collections.Generic;
using System.Threading;

namespace TuneRoboWPF.Utility
{

    public class StoreWorker
    {
        public static BlockingQueue<IRequest> ListsJobs = new BlockingQueue<IRequest>();

        private static readonly Lazy<StoreWorker> Lazy = new Lazy<StoreWorker>(() => new StoreWorker());

        public static StoreWorker Instance { get { return Lazy.Value; } }

        private Thread backgroundWorker;

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
        public static BlockingQueue<IRequest> ListsJobs = new BlockingQueue<IRequest>();

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

        public void Enqueue(T element)
        {
            lock (q)
            {
                if (q.Count >= MaxWaitingCount) q.Clear();
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

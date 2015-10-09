using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SharpMap.Rendering
{
    public class SingleThreadedWorkQueue
    {
        private readonly BlockingCollection<Action> renderQueue;
        private readonly Thread thread;

        public SingleThreadedWorkQueue(string threadName)
        {
            renderQueue = new BlockingCollection<Action>();
            thread = new Thread(WorkLoop)
            {
                Name = threadName, IsBackground = true
            };
            thread.Start();
        }

        public void Enqueue(Action action)
        {
            renderQueue.Add(action);
        }

        public int GetThreadId()
        {
            return thread.ManagedThreadId;
        }

        private void WorkLoop()
        {
            while (true)
            {
                try
                {
                    renderQueue.Take()();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
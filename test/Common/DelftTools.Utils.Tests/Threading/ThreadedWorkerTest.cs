using System;
using System.Diagnostics;
using System.Threading;
using DelftTools.Utils.Threading;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Threading
{
    [TestFixture]
    public class ThreadedWorkerTest
    {
        [Test]
        public void ThreadedWorkerShouldBeFast()
        {
            const int timePerWorkItem = 50;
            const int numWorkItems = 20;
            const int sequentialTime = numWorkItems*timePerWorkItem;
            const double maxTime = 0.8 * sequentialTime;

            var threadedWorker = new ThreadedWorker();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < numWorkItems; i++)
                threadedWorker.ProcessWorkItemAsync(()=>Thread.Sleep(timePerWorkItem));

            threadedWorker.WaitTillAllWorkItemsDone();

            stopwatch.Stop();

            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine("NumThreadsMin: " + workerThreads + ", " + completionPortThreads);

            Console.WriteLine("Elapsed: " + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Max: " + maxTime);
            Console.WriteLine(String.Format("Speedup: {0:N2}x", (double) sequentialTime/stopwatch.ElapsedMilliseconds));

            if (workerThreads <= 2)
            {
                Console.WriteLine("ThreadPool does not provie enough threads to make this test feasible. What is wrong with this machine!?");
                return; //too few
            }

            Assert.Less(stopwatch.ElapsedMilliseconds, maxTime);
        }

        [Test]
        public void TestWorkerCanBeReUsed()
        {
            const int timePerWorkItem = 50;
            const int numWorkItems = 4;

            var threadedWorker = new ThreadedWorker();

            for (int i = 0; i < numWorkItems; i++)
                threadedWorker.ProcessWorkItemAsync(() => Thread.Sleep(timePerWorkItem));
            threadedWorker.WaitTillAllWorkItemsDone();

            for (int i = 0; i < numWorkItems; i++)
                threadedWorker.ProcessWorkItemAsync(() => Thread.Sleep(timePerWorkItem));
            threadedWorker.WaitTillAllWorkItemsDone();
        }
    }
}
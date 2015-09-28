using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class ChunkedListTest
    {
        [Test]
        public void CheckConsistentBehavior()
        {
            var nativeList = new List<double>();
            var list = new ChunkedList<double>(8); //small chunk size

            // generate random (fixed seed) operations (but same for both lists)
            ListTestUtils.DoFixedRandomOperations(nativeList);
            ListTestUtils.DoFixedRandomOperations(list);

            // verify both lists are the same; this helps verifying the chunked implementation is correct
            Assert.AreEqual(nativeList.Count, list.Count);
            for (int i = 0; i < nativeList.Count; i++)
                Assert.AreEqual(nativeList[i], list[i], "" + i);
        }

        [Test]
        public void SingleChunkShouldPerformNearNativeList()
        {
            var nativeList = new List<double>();
            var list = new ChunkedList<double>(100000); 

            // generate random (fixed seed) operations (but same for both lists)
            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            ListTestUtils.DoFixedRandomOperations(nativeList, 5); 
            stopwatch.Stop();
            var nativeTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            ListTestUtils.DoFixedRandomOperations(list, 5);
            stopwatch.Stop();
            var chunkTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("Native time: {0}\nChunked time: {1}", nativeTime, chunkTime);
            Assert.Less(chunkTime, nativeTime * 1.3); //allow 30% overhead
        }

        [Test]
        public void VerifyValues()
        {
            var size = 10000000;

            var chunkedList = new ChunkedList<int>();
            for (int i = 0; i < size; i++)
                chunkedList.Add(i);

            for (int i = size-1; i > 0; i--)
                chunkedList[i] = chunkedList[i - 1] + 5;

            var indexOf1 = chunkedList.IndexOf(348974);
            var indexOf2 = chunkedList.IndexOf(5498322);

            Assert.AreEqual(348974 - 4, indexOf1);
            Assert.AreEqual(5498322 - 4, indexOf2);
            Assert.AreEqual(3214232 + 4, chunkedList[3214232]);

            chunkedList.Remove(348974);

            var indexOf3 = chunkedList.IndexOf(348974);
            var indexOf4 = chunkedList.IndexOf(348975);
            Assert.AreEqual(-1, indexOf3);
            Assert.AreEqual(348970, indexOf4);
        }

        [Test]
        public void IterateThroughList()
        {
            var size = 1000000;

            var nativeList = new List<int>();
            var chunkedList = new ChunkedList<int>();

            for (int i = 0; i < size; i++)
                nativeList.Add(i);
            for (int i = 0; i < size; i++)
                chunkedList.Add(i);

            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            var copy = nativeList.Select(i => i).ToList();
            stopwatch.Stop();
            var nativeEnumerateTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            var copy2 = chunkedList.Select(i => i).ToList();
            stopwatch.Stop();
            var chunkedEnumerateTime = stopwatch.ElapsedMilliseconds;

            // verify both lists are the same; this helps verifying the chunked implementation is correct
            Assert.AreEqual(copy.Count, copy2.Count);
            for (int i = 0; i < copy.Count; i++)
                Assert.AreEqual(copy[i], copy2[i], "" + i);

            // display performance info:
            Console.WriteLine("Native:");
            Console.WriteLine("Enumerate = " + nativeEnumerateTime);

            Console.WriteLine("Chunked:");
            Console.WriteLine("Enumerate = " + chunkedEnumerateTime); 
        }

        [Test]
        public void ComparePerformanceWithNativeList()
        {
            var size = 1000000;

            var nativeList = new List<int>();
            var chunkedList = new ChunkedList<int>();
            var stopwatch = new Stopwatch();

            stopwatch.Restart();
            for (int i = 0; i < size; i++)
                nativeList.Add(i);
            stopwatch.Stop();
            var nativeCreateTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            for (int i = 0; i < size; i++)
                chunkedList.Add(i);
            stopwatch.Stop();
            var chunkedCreateTime = stopwatch.ElapsedMilliseconds;

            int q = 0;
            stopwatch.Restart();
            for (int i = 0; i < size; i++)
                q = nativeList[i];
            stopwatch.Stop();
            var nativeGetTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            for (int i = 0; i < size; i++)
                q = chunkedList[i];
            stopwatch.Stop();
            var chunkedGetTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            for (int i = 0; i < size; i++)
                nativeList[i] = i + 2;
            stopwatch.Stop();
            var nativeSetTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            for (int i = 0; i < size; i++)
                chunkedList[i] = i + 2;
            stopwatch.Stop();
            var chunkedSetTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("Native:");
            Console.WriteLine("Add = " + nativeCreateTime);
            Console.WriteLine("Get = " + nativeGetTime);
            Console.WriteLine("Set = " + nativeSetTime);

            Console.WriteLine("Chunked:");
            Console.WriteLine("Add = " + chunkedCreateTime); 
            Console.WriteLine("Get = " + chunkedGetTime);
            Console.WriteLine("Set = " + chunkedSetTime);

            Assert.Less(chunkedSetTime, nativeSetTime*20); //20x slower..
        }
    }
}
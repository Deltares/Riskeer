using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections.Extensions;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class ChunkedArrayTest
    {
        [Test]
        public void AllocateAroundBlockSize()
        {
            var array1 = new ChunkedArray<int>(ChunkedArray<int>.BLOCK_SIZE + 1);
            Assert.IsNotNull(array1);
            var array2 = new ChunkedArray<int>(ChunkedArray<int>.BLOCK_SIZE - 1);
            Assert.IsNotNull(array2);
            var array3 = new ChunkedArray<int>(ChunkedArray<int>.BLOCK_SIZE);
            Assert.IsNotNull(array3);
        }

        [Test]
        [Category(TestCategory.WorkInProgress)]
        public void ComparePerformanceWithNativeArray()
        {
            var size = 10000000;

            var nativeArray = new int[size];
            var chunkedArray = new ChunkedArray<int>(size);
            
            var nativeList = new List<int>();
            nativeList.AddRange(Enumerable.Range(0, size));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 1; i < size; i++)
                nativeArray[i] = nativeArray[i - 1] + 1;
            stopwatch.Stop();

            var nativeTime = stopwatch.ElapsedMilliseconds;
            
            stopwatch.Restart();
            for (int i = 1; i < size; i++)
                nativeList[i] = nativeList[i - 1] + 1;
            stopwatch.Stop();
            var nativeListTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            for (int i = 1; i < size; i++)
                chunkedArray[i] = chunkedArray[i - 1] + 1;
            stopwatch.Stop();

            var chunkedTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("array: " + nativeTime);
            Console.WriteLine("list: " + nativeListTime);
            Console.WriteLine("chunked: " + chunkedTime);
            Assert.Less(chunkedTime, nativeTime*5); //max 5x slower
        }
    }
}
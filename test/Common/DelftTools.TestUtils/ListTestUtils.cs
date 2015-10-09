using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DelftTools.TestUtils
{
    public static class ListTestUtils
    {
        /// <summary>
        /// Used to verify an implementation of IList<T>, both for correctness and performance.
        /// By supplying the same seed & numOperations to this method and perform it on both a 
        /// native List<T> and your own implementation of IList<T>, you can verify the resulting
        /// lists are exactly equal and thus your implementation is correct (with very high 
        /// probability)
        /// 
        /// Performs a number of random operations on a (empty or filled) list given a seed. The 
        /// operations do bounds checking to make sure they are always valid.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="seed"></param>
        /// <param name="numOperations"></param>
        public static void DoFixedRandomOperations(IList<double> list, int seed = 0, int numOperations = 100000)
        {
            var rnd = new Random(seed);
            for (int i = 0; i < numOperations; i++)
            {
                var operation = rnd.Next(0, 6);
                switch (operation)
                {
                    case 0:
                    case 1:
                        list.Add(rnd.NextDouble());
                        break;
                    case 2:
                        list.Insert(rnd.Next(0, list.Count), rnd.NextDouble());
                        break;
                    case 3:
                        if (list.Count > 0)
                        {
                            list.RemoveAt(rnd.Next(0, list.Count));
                        }
                        break;
                    case 4:
                        if (list.Count > 0)
                        {
                            list.Remove(list[rnd.Next(0, list.Count)]);
                        }
                        break;
                    case 5:
                        if (list.Count > 0)
                        {
                            list[rnd.Next(0, list.Count)] = list[rnd.Next(0, list.Count)];
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void AssertAreEqual(IList<double> expected, IList<double> tested, double epsilonAbs)
        {
            Assert.AreEqual(expected.Count, tested.Count);
            for (var i = 0; i < expected.Count; ++i)
            {
                Assert.AreEqual(expected[i], tested[i], epsilonAbs);
            }
        }
    }
}
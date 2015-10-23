using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Wti.Data.TestUtil;

namespace Wti.Data.Test
{
    public class PointCollectionHelperTest
    {
        [Test]
        public void CreateFromString_OnePoint_ReturnsExpectedPoints()
        {
            // Call
            var result = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "3",
                "..1..",
                ".....",
                "....."
                )).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(2, result[0].X);
            Assert.AreEqual(2, result[0].Z);
        }

        [Test]
        public void CreateFromString_TwoPoint_ReturnsExpectedPoints()
        {
            // Call
            var result = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "3",
                "..1..",
                ".....",
                "....2"
                )).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0].X);
            Assert.AreEqual(2, result[0].Z);
            Assert.AreEqual(4, result[1].X);
            Assert.AreEqual(0, result[1].Z);
        }

        [Test]
        public void CreateFromString_TwoPointReversed_ReturnsExpectedPoints()
        {
            // Call
            var result = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "3",
                "..2..",
                ".....",
                "....1"
                )).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[1].X);
            Assert.AreEqual(2, result[1].Z);
            Assert.AreEqual(4, result[0].X);
            Assert.AreEqual(0, result[0].Z);
        }


        [Test]
        public void CreateFromFile_TwoPointReversed_ReturnsExpectedPoints()
        {
            // Setup
            var text = String.Join(Environment.NewLine,
                                   "3",
                                   "..2..",
                                   ".....",
                                   "....1"
                );
            var url = "temp";
            File.WriteAllText(url, text);

            // Call
            var result = PointCollectionHelper.CreateFromFile(url).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[1].X);
            Assert.AreEqual(2, result[1].Z);
            Assert.AreEqual(4, result[0].X);
            Assert.AreEqual(0, result[0].Z);
        }
    }
}
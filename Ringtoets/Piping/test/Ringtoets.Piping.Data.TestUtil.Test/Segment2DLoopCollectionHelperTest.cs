// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class Segment2DLoopCollectionHelperTest
    {
        [Test]
        public void CreateFromString_OnePoint_ReturnsExpectedPoints()
        {
            // Call
            Segment2D[] result = Segment2DLoopCollectionHelper.CreateFromString(string.Join(Environment.NewLine,
                                                                                            "3",
                                                                                            "..1..",
                                                                                            ".....",
                                                                                            "....."
                                                                                )).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(2, result[0].FirstPoint.X);
            Assert.AreEqual(2, result[0].FirstPoint.Y);
        }

        [Test]
        public void CreateFromString_TwoPoint_ReturnsExpectedPoints()
        {
            // Call
            Segment2D[] result = Segment2DLoopCollectionHelper.CreateFromString(string.Join(Environment.NewLine,
                                                                                            "3",
                                                                                            "..1..",
                                                                                            ".....",
                                                                                            "....2"
                                                                                )).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0].FirstPoint.X);
            Assert.AreEqual(2, result[0].FirstPoint.Y);
            Assert.AreEqual(4, result[0].SecondPoint.X);
            Assert.AreEqual(0, result[0].SecondPoint.Y);
        }

        [Test]
        public void CreateFromString_TwoPointReversed_ReturnsExpectedPoints()
        {
            // Call
            Segment2D[] result = Segment2DLoopCollectionHelper.CreateFromString(string.Join(Environment.NewLine,
                                                                                            "3",
                                                                                            "..2..",
                                                                                            ".....",
                                                                                            "....1"
                                                                                )).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0].SecondPoint.X);
            Assert.AreEqual(2, result[0].SecondPoint.Y);
            Assert.AreEqual(4, result[0].FirstPoint.X);
            Assert.AreEqual(0, result[0].FirstPoint.Y);
        }
    }
}
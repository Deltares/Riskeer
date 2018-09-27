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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsGridTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, double.NaN, double.NaN);

            // Assert
            Assert.IsInstanceOf<ICloneable>(grid);

            Assert.IsNaN(grid.XLeft);
            Assert.AreEqual(2, grid.XLeft.NumberOfDecimalPlaces);

            Assert.IsNaN(grid.XRight);
            Assert.AreEqual(2, grid.XRight.NumberOfDecimalPlaces);

            Assert.IsNaN(grid.ZTop);
            Assert.AreEqual(2, grid.ZTop.NumberOfDecimalPlaces);

            Assert.IsNaN(grid.ZBottom);
            Assert.AreEqual(2, grid.ZBottom.NumberOfDecimalPlaces);

            Assert.AreEqual(5, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(5, grid.NumberOfVerticalPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidPointCombinations))]
        public void Constructor_InvalidX_ThrowsArgumentException(double xLeft, double xRight)
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsGrid(xLeft, xRight, double.NaN, double.NaN);

            // Assert
            const string message = "X links moet kleiner zijn dan of gelijk zijn aan X rechts, of NaN.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        [TestCaseSource(nameof(GetValidPointCombinations))]
        public void Constructor_ValidX_ExpectedValues(double xLeft, double xRight)
        {
            // Call
            var grid = new MacroStabilityInwardsGrid(xLeft, xRight, double.NaN, double.NaN);

            // Assert
            Assert.AreEqual(xLeft, grid.XLeft, grid.XLeft.GetAccuracy());
            Assert.AreEqual(xRight, grid.XRight, grid.XRight.GetAccuracy());
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidPointCombinations))]
        public void Constructor_InvalidZ_ThrowsArgumentException(double zBottom, double zTop)
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsGrid(double.NaN, double.NaN, zTop, zBottom);

            // Assert
            const string message = "Z boven moet groter zijn dan of gelijk zijn aan Z onder, of NaN.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        [TestCaseSource(nameof(GetValidPointCombinations))]
        public void Constructor_ValidZPoints_ExpectedValues(double zBottom, double zTop)
        {
            // Call
            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, zTop, zBottom);

            // Assert
            Assert.AreEqual(zTop, grid.ZTop, grid.ZTop.GetAccuracy());
            Assert.AreEqual(zBottom, grid.ZBottom, grid.ZBottom.GetAccuracy());
        }

        [Test]
        public void Properties_ValidValues_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double xLeft = random.NextDouble(0.0, 1.0);
            double xRight = random.NextDouble(1.0, 2.0);
            double zTop = random.NextDouble(1.0, 2.0);
            double zBottom = random.NextDouble(0.0, 1.0);
            int numberOfHorizontalPoints = random.Next(1, 100);
            int numberOfVerticalPoints = random.Next(1, 100);

            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            grid.XLeft = (RoundedDouble) xLeft;
            grid.XRight = (RoundedDouble) xRight;
            grid.ZTop = (RoundedDouble) zTop;
            grid.ZBottom = (RoundedDouble) zBottom;
            grid.NumberOfHorizontalPoints = numberOfHorizontalPoints;
            grid.NumberOfVerticalPoints = numberOfVerticalPoints;

            // Assert
            Assert.AreEqual(xLeft, grid.XLeft, grid.XLeft.GetAccuracy());
            Assert.AreEqual(xRight, grid.XRight, grid.XRight.GetAccuracy());
            Assert.AreEqual(zTop, grid.ZTop, grid.ZTop.GetAccuracy());
            Assert.AreEqual(zBottom, grid.ZBottom, grid.ZBottom.GetAccuracy());
            Assert.AreEqual(numberOfHorizontalPoints, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(numberOfVerticalPoints, grid.NumberOfVerticalPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidPointCombinations))]
        public void XLeft_InvalidXLeft_ThrowsArgumentException(double xLeft, double xRight)
        {
            // Setup
            var grid = new MacroStabilityInwardsGrid(double.NaN, xRight, double.NaN, double.NaN);

            // Call
            TestDelegate test = () => grid.XLeft = (RoundedDouble) xLeft;

            // Assert
            const string message = "X links moet kleiner zijn dan of gelijk zijn aan X rechts, of NaN.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidPointCombinations))]
        public void XRight_InvalidXRight_ThrowsArgumentException(double xLeft, double xRight)
        {
            // Setup
            var grid = new MacroStabilityInwardsGrid(xLeft, double.NaN, double.NaN, double.NaN);

            // Call
            TestDelegate test = () => grid.XRight = (RoundedDouble) xRight;

            // Assert
            const string message = "X rechts moet groter zijn dan of gelijk zijn aan X links, of NaN.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidPointCombinations))]
        public void ZTop_InvalidZTop_ThrowsArgumentException(double zBottom, double zTop)
        {
            // Setup
            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, double.NaN, zBottom);

            // Call
            TestDelegate test = () => grid.ZTop = (RoundedDouble) zTop;

            // Assert
            const string message = "Z boven moet groter zijn dan of gelijk zijn aan Z onder, of NaN.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidPointCombinations))]
        public void ZBottom_InvalidZBottom_ThrowsArgumentException(double zBottom, double zTop)
        {
            // Setup
            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, zTop, double.NaN);

            // Call
            TestDelegate test = () => grid.ZBottom = (RoundedDouble) zBottom;

            // Assert
            const string message = "Z onder moet kleiner zijn dan of gelijk zijn aan Z boven, of NaN.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(101)]
        public void NumberOfHorizontalPoints_NumberOfPointsNotInRange_ThrowsArgumentOutOfRangeException(int numberOfPoints)
        {
            // Setup
            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            TestDelegate call = () => grid.NumberOfHorizontalPoints = numberOfPoints;

            // Assert
            const string expectedMessage = "De waarde voor het aantal horizontale punten moet in het bereik [1, 100] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(101)]
        public void NumberOfVerticalPoints_NumberOfPointsNotInRange_ThrowsArgumentOutOfRangeException(int numberOfPoints)
        {
            // Setup
            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            TestDelegate call = () => grid.NumberOfVerticalPoints = numberOfPoints;

            // Assert
            const string expectedMessage = "De waarde voor het aantal verticale punten moet in het bereik [1, 100] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new MacroStabilityInwardsGrid(random.NextDouble(0.0, 1.0),
                                                         random.NextDouble(1.0, 2.0),
                                                         random.NextDouble(1.0, 2.0),
                                                         random.NextDouble(0.0, 1.0))
            {
                NumberOfHorizontalPoints = random.Next(1, 100),
                NumberOfVerticalPoints = random.Next(1, 100)
            };

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        private static IEnumerable<TestCaseData> GetInvalidPointCombinations()
        {
            yield return new TestCaseData(1e-2, 0.0);
            yield return new TestCaseData(1.0, 0.0);
            yield return new TestCaseData(0.0, -1.0);
        }

        private static IEnumerable<TestCaseData> GetValidPointCombinations()
        {
            yield return new TestCaseData(double.NaN, 0.0);
            yield return new TestCaseData(0.0, double.NaN);
            yield return new TestCaseData(double.NaN, double.NaN);
            yield return new TestCaseData(0.0, 0.0);
            yield return new TestCaseData(0.0, 1e-3);
            yield return new TestCaseData(1e-3, 0.0);
            yield return new TestCaseData(0.0, 1.0);
            yield return new TestCaseData(-1.0, 1.0);
        }
    }
}
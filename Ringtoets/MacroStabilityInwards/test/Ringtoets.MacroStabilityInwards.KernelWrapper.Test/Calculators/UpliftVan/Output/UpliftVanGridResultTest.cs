// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Output
{
    [TestFixture]
    public class UpliftVanGridResultTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double xLeft = random.Next();
            double xRight = random.Next();
            double zTop = random.Next();
            double zBottom = random.Next();
            int nrOfHorizontalPoints = random.Next();
            int nrOfVerticalPoints = random.Next();

            // Call
            var gridResult = new UpliftVanGridResult(xLeft, xRight, zTop, zBottom, nrOfHorizontalPoints, nrOfVerticalPoints);

            // Assert
            Assert.AreEqual(xLeft, gridResult.XLeft);
            Assert.AreEqual(xRight, gridResult.XRight);
            Assert.AreEqual(zTop, gridResult.ZTop);
            Assert.AreEqual(zBottom, gridResult.ZBottom);
            Assert.AreEqual(nrOfHorizontalPoints, gridResult.NumberOfHorizontalPoints);
            Assert.AreEqual(nrOfVerticalPoints, gridResult.NumberOfVerticalPoints);
        }
    }
}
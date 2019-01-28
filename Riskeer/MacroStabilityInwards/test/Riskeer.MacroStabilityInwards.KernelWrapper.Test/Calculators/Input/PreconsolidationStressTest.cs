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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.Input
{
    [TestFixture]
    public class PreconsolidationStressTest
    {
        [Test]
        public void Constructor_CoordinateNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PreconsolidationStress(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("coordinate", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(11);
            var coordinate = new Point2D(random.NextDouble(), random.NextDouble());
            double stress = random.NextDouble();

            // Call
            var preconsolidationStress = new PreconsolidationStress(coordinate, stress);

            // Assert
            Assert.AreSame(coordinate, preconsolidationStress.Coordinate);
            Assert.AreEqual(stress, preconsolidationStress.Stress);
        }
    }
}
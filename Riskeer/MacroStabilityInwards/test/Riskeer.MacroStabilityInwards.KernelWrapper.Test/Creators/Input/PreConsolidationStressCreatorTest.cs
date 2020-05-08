// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Deltares.MacroStability.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class PreConsolidationStressCreatorTest
    {
        [Test]
        public void Create_PreconsolidationStressesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PreConsolidationStressCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("preconsolidationStresses", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsPreConsolidationStresses()
        {
            // Setup
            var preconsolidationStresses = new[]
            {
                new PreconsolidationStress(new Point2D(3.2, 4.8), 1.5),
                new PreconsolidationStress(new Point2D(1.2, 0.5), 17.9),
                new PreconsolidationStress(new Point2D(50.8, 9.9), 3.8)
            };

            // Call
            IEnumerable<PreConsolidationStress> preConsolidationStresses = PreConsolidationStressCreator.Create(preconsolidationStresses).ToArray();

            // Assert
            Assert.AreEqual(preConsolidationStresses.Count(), 3);

            Assert.AreEqual(preConsolidationStresses.First().X, 3.2);
            Assert.AreEqual(preConsolidationStresses.First().Z, 4.8);
            Assert.AreEqual(preConsolidationStresses.First().StressValue, 1.5);

            Assert.AreEqual(preConsolidationStresses.ElementAt(1).X, 1.2);
            Assert.AreEqual(preConsolidationStresses.ElementAt(1).Z, 0.5);
            Assert.AreEqual(preConsolidationStresses.ElementAt(1).StressValue, 17.9);

            Assert.AreEqual(preConsolidationStresses.Last().X, 50.8);
            Assert.AreEqual(preConsolidationStresses.Last().Z, 9.9);
            Assert.AreEqual(preConsolidationStresses.Last().StressValue, 3.8);
        }
    }
}
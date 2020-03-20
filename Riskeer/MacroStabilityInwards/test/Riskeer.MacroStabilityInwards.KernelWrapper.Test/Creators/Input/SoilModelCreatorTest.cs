﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Deltares.MacroStability.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class SoilModelCreatorTest
    {
        [Test]
        public void Create_SoilsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilModelCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soils", exception.ParamName);
        }

        [Test]
        public void Create_WithSoils_ReturnSoilModelWithSoils()
        {
            // Setup
            var soils = new[]
            {
                new Soil("soil 1"),
                new Soil("soil 2")
            };

            // Call
            SoilModel soilModel = SoilModelCreator.Create(soils);

            // Assert
            CollectionAssert.AreEqual(soils, soilModel.Soils);
        }
    }
}
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
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressCreateExtensionsTest
    {
        [Test]
        public void Create_PreconsolidationStressNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsPreconsolidationStress) null).Create();

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("preconsolidationStress", parameterName);
        }

        [Test]
        public void Create_WithValidProperties_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            var stress = new MacroStabilityInwardsPreconsolidationStress(random.NextDouble(),
                                                                         random.NextDouble(),
                                                                         random.NextDouble(),
                                                                         random.NextDouble());

            // Call
            MacroStabilityInwardsPreconsolidationStressEntity entity = stress.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(stress.XCoordinate, entity.CoordinateX);
            Assert.AreEqual(stress.ZCoordinate, entity.CoordinateZ);
            Assert.AreEqual(stress.PreconsolidationStressMean, entity.PreconsolidationStressMean);
            Assert.AreEqual(stress.PreconsolidationStressCoefficientOfVariation, entity.PreconsolidationStressCoefficientOfVariation);
        }
    }
}
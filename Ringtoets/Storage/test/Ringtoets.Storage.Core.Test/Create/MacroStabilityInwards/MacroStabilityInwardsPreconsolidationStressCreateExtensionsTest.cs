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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressCreateExtensionsTest
    {
        [Test]
        public void Create_PreconsolidationStressNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsPreconsolidationStress) null).Create(0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("preconsolidationStress", parameterName);
        }

        [Test]
        public void Create_WithValidProperties_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);

            var stress = new MacroStabilityInwardsPreconsolidationStress(new Point2D(random.NextDouble(), random.NextDouble()),
                                                                         new VariationCoefficientLogNormalDistribution
                                                                         {
                                                                             Mean = (RoundedDouble) 0.005,
                                                                             CoefficientOfVariation = random.NextRoundedDouble()
                                                                         });
            int order = random.Next();

            // Call
            MacroStabilityInwardsPreconsolidationStressEntity entity = stress.Create(order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(stress.Location.X, entity.CoordinateX);
            Assert.AreEqual(stress.Location.Y, entity.CoordinateZ);

            VariationCoefficientLogNormalDistribution preconsolidationStressDistribution = stress.Stress;
            Assert.AreEqual(preconsolidationStressDistribution.Mean, entity.PreconsolidationStressMean,
                            preconsolidationStressDistribution.GetAccuracy());
            Assert.AreEqual(preconsolidationStressDistribution.CoefficientOfVariation, entity.PreconsolidationStressCoefficientOfVariation,
                            preconsolidationStressDistribution.GetAccuracy());
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_WithNaNValues_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);

            var stress = new MacroStabilityInwardsPreconsolidationStress(new Point2D(random.NextDouble(), random.NextDouble()),
                                                                         new VariationCoefficientLogNormalDistribution
                                                                         {
                                                                             Mean = RoundedDouble.NaN,
                                                                             CoefficientOfVariation = RoundedDouble.NaN
                                                                         });
            int order = random.Next();

            // Call
            MacroStabilityInwardsPreconsolidationStressEntity entity = stress.Create(order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.PreconsolidationStressMean);
            Assert.IsNull(entity.PreconsolidationStressCoefficientOfVariation);
            Assert.AreEqual(order, entity.Order);
        }
    }
}
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;
using Ringtoets.Storage.Core.TestUtil.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.MacroStabilityInwards;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileTwoDEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => ((MacroStabilityInwardsSoilProfileTwoDEntity) null).Read(collector);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsSoilProfileWithPropertiesSet()
        {
            // Setup
            Ring outerRingA = RingTestFactory.CreateRandomRing(32);
            Ring outerRingB = RingTestFactory.CreateRandomRing(33);

            var random = new Random(31);
            var preconsolidationStressEntity = new MacroStabilityInwardsPreconsolidationStressEntity
            {
                CoordinateX = random.NextDouble(),
                CoordinateZ = random.NextDouble(),
                PreconsolidationStressMean = random.NextDouble(),
                PreconsolidationStressCoefficientOfVariation = random.NextDouble(),
                Order = 1
            };

            var point2DXmlSerializer = new Point2DCollectionXmlSerializer();
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = nameof(MacroStabilityInwardsSoilProfileTwoDEntity),
                MacroStabilityInwardsSoilLayerTwoDEntities =
                {
                    new MacroStabilityInwardsSoilLayerTwoDEntity
                    {
                        MaterialName = "A",
                        OuterRingXml = point2DXmlSerializer.ToXml(outerRingA.Points),
                        Order = 1
                    },
                    new MacroStabilityInwardsSoilLayerTwoDEntity
                    {
                        MaterialName = "B",
                        OuterRingXml = point2DXmlSerializer.ToXml(outerRingB.Points),
                        Order = 0
                    }
                },
                MacroStabilityInwardsPreconsolidationStressEntities =
                {
                    preconsolidationStressEntity,
                    new MacroStabilityInwardsPreconsolidationStressEntity
                    {
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsSoilProfile2D profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entity.Name, profile.Name);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, profile.Layers.Select(l => l.Data.MaterialName));

            CollectionAssert.AreEqual(new[]
            {
                outerRingB,
                outerRingA
            }, profile.Layers.Select(l => l.OuterRing));

            profile.Layers.Select(l => l.NestedLayers).ForEachElementDo(CollectionAssert.IsEmpty);

            CollectionAssert.AreEqual(new[]
            {
                new MacroStabilityInwardsPreconsolidationStress(new Point2D(0, 0),
                                                                new VariationCoefficientLogNormalDistribution
                                                                {
                                                                    Mean = RoundedDouble.NaN,
                                                                    CoefficientOfVariation = RoundedDouble.NaN
                                                                }),
                new MacroStabilityInwardsPreconsolidationStress(new Point2D(preconsolidationStressEntity.CoordinateX,
                                                                            preconsolidationStressEntity.CoordinateZ),
                                                                new VariationCoefficientLogNormalDistribution
                                                                {
                                                                    Mean = (RoundedDouble) preconsolidationStressEntity.PreconsolidationStressMean.ToNullAsNaN(),
                                                                    CoefficientOfVariation = (RoundedDouble) preconsolidationStressEntity.PreconsolidationStressCoefficientOfVariation.ToNullAsNaN()
                                                                })
            }, profile.PreconsolidationStresses);
        }

        [Test]
        public void GivenReadObject_WhenReadCalledOnSameEntity_ThenSameObjectInstanceReturned()
        {
            // Given
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = "testName",
                MacroStabilityInwardsSoilLayerTwoDEntities =
                {
                    MacroStabilityInwardsSoilLayerTwoDEntityTestFactory.CreateMacroStabilityInwardsSoilLayerTwoDEntity()
                }
            };
            var collector = new ReadConversionCollector();

            MacroStabilityInwardsSoilProfile2D profile = entity.Read(collector);

            // When
            MacroStabilityInwardsSoilProfile2D secondProfile = entity.Read(collector);

            // Then
            Assert.AreSame(profile, secondProfile);
        }
    }
}
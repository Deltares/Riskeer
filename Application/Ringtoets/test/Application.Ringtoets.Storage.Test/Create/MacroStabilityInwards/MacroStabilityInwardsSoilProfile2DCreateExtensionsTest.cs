﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile2DCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D soilProfile = CreateMacroStabilityInwardsSoilProfile2D();

            // Call
            TestDelegate test = () => soilProfile.Create(null);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => ((MacroStabilityInwardsSoilProfile2D) null).Create(registry);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("soilProfile", parameterName);
        }

        [Test]
        public void Create_WithValidProperties_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsSoilProfile2D("some name", new[]
            {
                MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D(),
                MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D()
            }, new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress()
            });
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsSoilProfileTwoDEntity entity = soilProfile.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(soilProfile.Layers.Count(), entity.MacroStabilityInwardsSoilLayerTwoDEntities.Count);
            Assert.AreEqual(soilProfile.PreconsolidationStresses.Count(), entity.MacroStabilityInwardsPreconsolidationStressEntities.Count);

            AssertPreconsolidationStress(soilProfile.PreconsolidationStresses.First(),
                                         entity.MacroStabilityInwardsPreconsolidationStressEntities.First());
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D soilProfile = CreateMacroStabilityInwardsSoilProfile2D("some name");
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsSoilProfileTwoDEntity entity = soilProfile.Create(registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(soilProfile.Name, entity.Name);
        }

        [Test]
        public void GivenCreatedEntity_WhenCreateCalledOnSameObject_ThenSameEntityInstanceReturned()
        {
            // Given
            MacroStabilityInwardsSoilProfile2D soilProfile = CreateMacroStabilityInwardsSoilProfile2D();
            var registry = new PersistenceRegistry();

            MacroStabilityInwardsSoilProfileTwoDEntity firstEntity = soilProfile.Create(registry);

            // When
            MacroStabilityInwardsSoilProfileTwoDEntity secondEntity = soilProfile.Create(registry);

            // Then
            Assert.AreSame(firstEntity, secondEntity);
        }

        private static void AssertPreconsolidationStress(MacroStabilityInwardsPreconsolidationStress preconsolidationStress,
                                                         MacroStabilityInwardsPreconsolidationStressEntity entity)
        {
            Assert.AreEqual(preconsolidationStress.Location.X, entity.CoordinateX);
            Assert.AreEqual(preconsolidationStress.Location.Y, entity.CoordinateZ);

            VariationCoefficientLogNormalDistribution preconsolidationDistribution = preconsolidationStress.Stress;
            Assert.AreEqual(preconsolidationDistribution.Mean, entity.PreconsolidationStressMean,
                            preconsolidationDistribution.GetAccuracy());
            Assert.AreEqual(preconsolidationDistribution.CoefficientOfVariation, entity.PreconsolidationStressCoefficientOfVariation,
                            preconsolidationDistribution.GetAccuracy());
        }

        private static MacroStabilityInwardsSoilProfile2D CreateMacroStabilityInwardsSoilProfile2D()
        {
            return CreateMacroStabilityInwardsSoilProfile2D(string.Empty);
        }

        private static MacroStabilityInwardsSoilProfile2D CreateMacroStabilityInwardsSoilProfile2D(string name)
        {
            var random = new Random(31);
            var layers = new Collection<MacroStabilityInwardsSoilLayer2D>
            {
                new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                {
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble())
                }), Enumerable.Empty<Ring>())
                {
                    Data = new MacroStabilityInwardsSoilLayerData()
                }
            };

            return new MacroStabilityInwardsSoilProfile2D(name, layers, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());
        }
    }
}
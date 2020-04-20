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
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableSoilLayerCollectionFactoryTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableSoilLayerCollectionFactory.Create(null, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }
        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfileUnderSurfaceLine>();
            mocks.ReplayAll();


            // Call
            void Call() => PersistableSoilLayerCollectionFactory.Create(soilProfile, null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfileUnderSurfaceLine>();
            mocks.ReplayAll();

            // Call
            void Call() => PersistableSoilLayerCollectionFactory.Create(soilProfile, new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableSoilLayerCollectionCollection()
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(
                new[]
                {
                    MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D(new[]
                    {
                        MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D()
                    })
                },
                Enumerable.Empty<IMacroStabilityInwardsPreconsolidationStress>());
            
            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            PersistableSoilCollection soils = PersistableSoilCollectionFactory.Create(soilProfile, idFactory, registry);
            IEnumerable<PersistableGeometry> geometries = PersistableGeometryFactory.Create(soilProfile, idFactory, registry);

            // Call
            IEnumerable<PersistableSoilLayerCollection> soilLayerCollections = PersistableSoilLayerCollectionFactory.Create(soilProfile, idFactory, registry);

            // Assert
            Assert.AreEqual(2, soilLayerCollections.Count());

            IEnumerable<MacroStabilityInwardsSoilLayer2D> originalLayers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfile.Layers);

            for (var i = 0; i < soilLayerCollections.Count(); i++)
            {
                PersistableSoilLayerCollection soilLayerCollection = soilLayerCollections.ElementAt(i);

                Assert.IsNotNull(soilLayerCollection.Id);
                Assert.AreEqual(originalLayers.Count(), soilLayerCollection.SoilLayers.Count());

                for (var j = 0; j < originalLayers.Count(); j++)
                {
                    PersistableSoilLayer persistableSoilLayer = soilLayerCollection.SoilLayers.ElementAt(j);

                    Assert.AreEqual(soils.Soils.ElementAt(j).Id, persistableSoilLayer.SoilId);
                    Assert.AreEqual(geometries.ElementAt(i).Layers.ElementAt(j).Id, persistableSoilLayer.LayerId);
                }
            }

            var stages = new[]
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            };

            Assert.AreEqual(2, registry.SoilLayers.Count);

            for (var i = 0; i < stages.Length; i++)
            {
                Assert.AreEqual(registry.SoilLayers[stages[i]], soilLayerCollections.ElementAt(i).Id);
            }
        }
    }
}
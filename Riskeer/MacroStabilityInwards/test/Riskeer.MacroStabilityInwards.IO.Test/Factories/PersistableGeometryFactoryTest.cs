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
using Riskeer.MacroStabilityInwards.IO.TestUtil;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableGeometryFactoryTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableGeometryFactory.Create(null, new IdFactory(), new MacroStabilityInwardsExportRegistry());

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
            void Call() => PersistableGeometryFactory.Create(soilProfile, null, new MacroStabilityInwardsExportRegistry());

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
            void Call() => PersistableGeometryFactory.Create(soilProfile, new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_WithValidData_ReturnsNull()
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D(new[]
                {
                    MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D()
                })
            }, Enumerable.Empty<IMacroStabilityInwardsPreconsolidationStress>());

            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            IEnumerable<PersistableGeometry> geometries = PersistableGeometryFactory.Create(soilProfile, new IdFactory(), registry);

            // Assert
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layersRecursively = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfile.Layers);

            PersistableDataModelTestHelper.AssertPersistableGeometry(layersRecursively, geometries);
            AssertRegistry(registry, geometries, layersRecursively);
        }

        private static void AssertRegistry(MacroStabilityInwardsExportRegistry registry, IEnumerable<PersistableGeometry> geometries,
                                           IEnumerable<MacroStabilityInwardsSoilLayer2D> layers)
        {
            var stageTypes = new[]
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            };

            Assert.AreEqual(stageTypes.Length, registry.Geometries.Count);
            Assert.AreEqual(stageTypes.Length, registry.GeometryLayers.Count);

            for (var i = 0; i < stageTypes.Length; i++)
            {
                KeyValuePair<MacroStabilityInwardsExportStageType, string> storedGeometry = registry.Geometries.ElementAt(i);
                Assert.AreEqual(stageTypes[i], storedGeometry.Key);
                Assert.AreEqual(geometries.ElementAt(i).Id, storedGeometry.Value);

                KeyValuePair<MacroStabilityInwardsExportStageType, Dictionary<MacroStabilityInwardsSoilLayer2D, string>> storedGeometryLayers = registry.GeometryLayers.ElementAt(i);

                Assert.AreEqual(stageTypes[i], storedGeometryLayers.Key);

                IEnumerable<PersistableLayer> persistableGeometryLayers = geometries.ElementAt(i).Layers;
                Assert.AreEqual(persistableGeometryLayers.Count(), storedGeometryLayers.Value.Count);

                for (var j = 0; j < persistableGeometryLayers.Count(); j++)
                {
                    KeyValuePair<MacroStabilityInwardsSoilLayer2D, string> storedGeometryLayer = storedGeometryLayers.Value.ElementAt(j);

                    Assert.AreSame(layers.ElementAt(j), storedGeometryLayer.Key);
                    Assert.AreEqual(persistableGeometryLayers.ElementAt(j).Id, storedGeometryLayer.Value);
                }
            }
        }
    }
}
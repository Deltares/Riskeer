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
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsExportRegistryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var registry = new MacroStabilityInwardsExportRegistry();

            // Assert
            CollectionAssert.IsEmpty(registry.Settings);
            CollectionAssert.IsEmpty(registry.Soils);
            CollectionAssert.IsEmpty(registry.Geometries);
        }

        [Test]
        public void AddSettings_InvalidStageType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();
            const MacroStabilityInwardsExportStageType stageType = (MacroStabilityInwardsExportStageType)99;

            // Call
            void Call() => registry.Add(stageType, "1");

            // Assert
            string expectedMessage = $"The value of argument '{nameof(stageType)}' ({stageType}) is invalid for Enum type '{nameof(MacroStabilityInwardsExportStageType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void AddSettings_WithSettings_AddsSettings()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();
            const MacroStabilityInwardsExportStageType stageType = MacroStabilityInwardsExportStageType.Daily;
            const string id = "1";

            // Call
            registry.Add(stageType, id);

            // Assert
            Assert.AreEqual(1, registry.Settings.Count);
            KeyValuePair<MacroStabilityInwardsExportStageType, string> storedSettings = registry.Settings.Single();
            Assert.AreEqual(stageType, storedSettings.Key);
            Assert.AreEqual(id, storedSettings.Value);
        }

        [Test]
        public void AddSoil_SoilLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            void Call() => registry.AddSoil(null, "1");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void AddSoil_WithSoilLayer_AddsSoilLayer()
        {
            // Setup
            var mocks = new MockRepository();
            var soilLayer = mocks.Stub<IMacroStabilityInwardsSoilLayer>();
            mocks.ReplayAll();

            var registry = new MacroStabilityInwardsExportRegistry();
            const string id = "1";

            // Call
            registry.AddSoil(soilLayer, id);

            // Assert
            Assert.AreEqual(1, registry.Soils.Count);
            KeyValuePair<IMacroStabilityInwardsSoilLayer, string> storedSoil = registry.Soils.Single();
            Assert.AreSame(soilLayer, storedSoil.Key);
            Assert.AreEqual(id, storedSoil.Value);
        }

        [Test]
        public void AddGeometry_GeometryLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            void Call() => registry.AddGeometry(null, "1");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("geometryLayer", exception.ParamName);
        }

        [Test]
        public void AddGeometry_WithGeometryLayer_AddsGeometryLayer()
        {
            // Setup
            var mocks = new MockRepository();
            var geometryLayer = mocks.Stub<IMacroStabilityInwardsSoilLayer>();
            mocks.ReplayAll();

            var registry = new MacroStabilityInwardsExportRegistry();
            const string id = "1";

            // Call
            registry.AddGeometry(geometryLayer, id);

            // Assert
            Assert.AreEqual(1, registry.Geometries.Count);
            KeyValuePair<IMacroStabilityInwardsSoilLayer, string> storedGeometry = registry.Geometries.Single();
            Assert.AreSame(geometryLayer, storedGeometry.Key);
            Assert.AreEqual(id, storedGeometry.Value);
        }
    }
}
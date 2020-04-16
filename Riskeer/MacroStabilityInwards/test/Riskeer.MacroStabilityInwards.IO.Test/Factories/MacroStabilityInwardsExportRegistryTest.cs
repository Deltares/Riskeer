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
            CollectionAssert.IsEmpty(registry.GeometryLayers);
        }

        [Test]
        public void AddSettings_InvalidStageType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();
            const MacroStabilityInwardsExportStageType stageType = (MacroStabilityInwardsExportStageType) 99;

            // Call
            void Call() => registry.AddSettings(stageType, "1");

            // Assert
            string expectedMessage = $"The value of argument '{nameof(stageType)}' ({stageType}) is invalid for Enum type '{nameof(MacroStabilityInwardsExportStageType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void AddSettings_WithSettings_AddsSettings()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();
            var stageType = new Random(21).NextEnumValue<MacroStabilityInwardsExportStageType>();
            const string id = "1";

            // Call
            registry.AddSettings(stageType, id);

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
            KeyValuePair<IMacroStabilityInwardsSoilLayer, string> registeredSoil = registry.Soils.Single();
            Assert.AreSame(soilLayer, registeredSoil.Key);
            Assert.AreEqual(id, registeredSoil.Value);
        }

        [Test]
        public void AddGeometry_InvalidStageType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();
            const MacroStabilityInwardsExportStageType stageType = (MacroStabilityInwardsExportStageType) 99;

            // Call
            void Call() => registry.AddGeometry(stageType, "1");

            // Assert
            string expectedMessage = $"The value of argument '{nameof(stageType)}' ({stageType}) is invalid for Enum type '{nameof(MacroStabilityInwardsExportStageType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void AddGeometry_WithGeometry_AddsSettings()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();
            var stageType = new Random(21).NextEnumValue<MacroStabilityInwardsExportStageType>();
            const string id = "1";

            // Call
            registry.AddGeometry(stageType, id);

            // Assert
            Assert.AreEqual(1, registry.Geometries.Count);
            KeyValuePair<MacroStabilityInwardsExportStageType, string> storedGeometry = registry.Geometries.Single();
            Assert.AreEqual(stageType, storedGeometry.Key);
            Assert.AreEqual(id, storedGeometry.Value);
        }

        [Test]
        public void AddGeometryLayer_InvalidStageType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var geometryLayer = mocks.Stub<IMacroStabilityInwardsSoilLayer>();
            mocks.ReplayAll();

            var registry = new MacroStabilityInwardsExportRegistry();
            const MacroStabilityInwardsExportStageType stageType = (MacroStabilityInwardsExportStageType) 99;

            // Call
            void Call() => registry.AddGeometryLayer(stageType, geometryLayer, "1");

            // Assert
            string expectedMessage = $"The value of argument '{nameof(stageType)}' ({stageType}) is invalid for Enum type '{nameof(MacroStabilityInwardsExportStageType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void AddGeometryLayer_GeometryLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            void Call() => registry.AddGeometryLayer(new Random(21).NextEnumValue<MacroStabilityInwardsExportStageType>(), null, "1");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("geometryLayer", exception.ParamName);
        }

        [Test]
        public void AddGeometryLayer_NewStageType_AddsStageTypeAndGeometryLayer()
        {
            // Setup
            var mocks = new MockRepository();
            var geometryLayer = mocks.Stub<IMacroStabilityInwardsSoilLayer>();
            mocks.ReplayAll();

            var registry = new MacroStabilityInwardsExportRegistry();
            var stageType = new Random(21).NextEnumValue<MacroStabilityInwardsExportStageType>();
            const string id = "1";

            // Call
            registry.AddGeometryLayer(stageType, geometryLayer, id);

            // Assert
            Assert.AreEqual(1, registry.GeometryLayers.Count);
            KeyValuePair<MacroStabilityInwardsExportStageType, Dictionary<IMacroStabilityInwardsSoilLayer, string>> registeredStorageTypeGeometry = registry.GeometryLayers.Single();
            Assert.AreEqual(stageType, registeredStorageTypeGeometry.Key);

            Assert.AreEqual(1, registeredStorageTypeGeometry.Value.Count);
            KeyValuePair<IMacroStabilityInwardsSoilLayer, string> registeredGeometry = registeredStorageTypeGeometry.Value.Single();

            Assert.AreSame(geometryLayer, registeredGeometry.Key);
            Assert.AreEqual(id, registeredGeometry.Value);
        }

        [Test]
        public void AddGeometryLayer_StageTypeAlreadyRegistered_AddsGeometryLayer()
        {
            // Setup
            var mocks = new MockRepository();
            var geometryLayer1 = mocks.Stub<IMacroStabilityInwardsSoilLayer>();
            var geometryLayer2 = mocks.Stub<IMacroStabilityInwardsSoilLayer>();
            mocks.ReplayAll();

            var registry = new MacroStabilityInwardsExportRegistry();
            var stageType = new Random(21).NextEnumValue<MacroStabilityInwardsExportStageType>();
            const string id1 = "1";
            const string id2 = "2";

            registry.AddGeometryLayer(stageType, geometryLayer1, id1);

            // Precondition
            Assert.AreEqual(1, registry.GeometryLayers.Count);

            // Call
            registry.AddGeometryLayer(stageType, geometryLayer2, id2);

            // Assert
            Assert.AreEqual(1, registry.GeometryLayers.Count);
            KeyValuePair<MacroStabilityInwardsExportStageType, Dictionary<IMacroStabilityInwardsSoilLayer, string>> registeredStageTypeGeometry = registry.GeometryLayers.Single();
            Assert.AreEqual(stageType, registeredStageTypeGeometry.Key);

            Assert.AreEqual(2, registeredStageTypeGeometry.Value.Count);

            Dictionary<IMacroStabilityInwardsSoilLayer, string> registeredGeometries = registeredStageTypeGeometry.Value;

            KeyValuePair<IMacroStabilityInwardsSoilLayer, string> registeredGeometry1 = registeredGeometries.First();
            Assert.AreSame(geometryLayer1, registeredGeometry1.Key);
            Assert.AreEqual(id1, registeredGeometry1.Value);
            KeyValuePair<IMacroStabilityInwardsSoilLayer, string> registeredGeometry2 = registeredGeometries.Last();
            Assert.AreSame(geometryLayer2, registeredGeometry2.Key);
            Assert.AreEqual(id2, registeredGeometry2.Value);
        }
    }
}
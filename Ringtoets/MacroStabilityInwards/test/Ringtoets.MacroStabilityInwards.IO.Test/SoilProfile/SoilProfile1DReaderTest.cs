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
using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.SoilProfile.Schema;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfile1DReaderTest
    {
        private MockRepository mocks;
        private IRowBasedDatabaseReader reader;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            reader = mocks.DynamicMock<IRowBasedDatabaseReader>();
        }

        [Test]
        public void ReadFrom_InvalidCriticalProperty_ThrowsCriticalFileReadException()
        {
            // Setup
            const string profileName = "<profile name>";
            const string path = "A";

            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(profileName);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Throw(new InvalidCastException());
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject($"ondergrondschematisatie '{profileName}'")
                .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidRequiredProperty_ThrowsMacroStabilityInwardsSoilProfileReadException()
        {
            // Setup
            const string profileName = "<profile name>";
            const string path = "A";

            reader.Expect(r => r.Path).Return(path);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.Bottom)).Throw(new InvalidCastException());

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<MacroStabilityInwardsSoilProfileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject($"ondergrondschematisatie '{profileName}'")
                .Build("Ondergrondschematisatie bevat geen geldige waarde in kolom 'Bottom'.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_ZeroLayerCount_ThrowsMacroStabilityInwardsSoilProfileReadException()
        {
            // Setup
            const string profileName = "<very cool name>";
            const string path = "A";

            SetExpectations(0, profileName, 0.0, 0.0, null, null, null, null, null, null, null, null, null);
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<MacroStabilityInwardsSoilProfileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject($"ondergrondschematisatie '{profileName}'")
                .Build("Geen lagen gevonden voor de ondergrondschematisatie.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidIsAquifer_ReturnsProfileWithNullValuesOnLayer()
        {
            // Setup
            const string path = "A";
            const string profileName = "<name>";

            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(profileName);
            reader.Expect(r => r.Path).Return(path);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.IsAquifer)).Throw(new InvalidCastException());
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<MacroStabilityInwardsSoilProfileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject($"ondergrondschematisatie '{profileName}'")
                .Build("Ondergrondschematisatie bevat geen geldige waarde in kolom 'IsAquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_NullValuesForLayer_ReturnsProfileWithNaNValuesAndDefaultsOnLayer()
        {
            // Setup
            const double bottom = 1.1;
            const double top = 1.1;
            SetExpectations(1, "", bottom, top, null, null, null, null, null, null, null, null, null);

            mocks.ReplayAll();

            // Call
            MacroStabilityInwardsSoilProfile1D profile = SoilProfile1DReader.ReadFrom(reader);

            // Assert
            Assert.AreEqual(1, profile.Layers.Count());
            Assert.AreEqual(bottom, profile.Bottom);

            MacroStabilityInwardsSoilLayer1D macroStabilityInwardsSoilLayer = profile.Layers.First();

            Assert.AreEqual(top, macroStabilityInwardsSoilLayer.Top);
            Assert.IsEmpty(macroStabilityInwardsSoilLayer.MaterialName);
            Assert.AreEqual(Color.Empty, macroStabilityInwardsSoilLayer.Color);
            Assert.IsFalse(macroStabilityInwardsSoilLayer.IsAquifer);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ReadFrom_ProperValuesForNumberOfLayers_ReturnsProfileWithNumberOfLayers(int layerCount)
        {
            // Setup
            var random = new Random(22);
            double bottom = random.NextDouble();
            double top = bottom + random.NextDouble();
            const string materialName = "material";
            Color color = Color.FromArgb(Color.DarkKhaki.ToArgb());

            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelDeviation = random.NextDouble();
            double diameterD70Mean = random.NextDouble();
            double diameterD70CoefficientOfVariation = random.NextDouble();
            double permeabilityMean = random.NextDouble();
            double permeabilityCoefficientOfVariation = random.NextDouble();

            SetExpectations(
                layerCount,
                "",
                bottom,
                top,
                1.0,
                materialName,
                color.ToArgb(),
                belowPhreaticLevelMean,
                belowPhreaticLevelDeviation,
                diameterD70Mean,
                diameterD70CoefficientOfVariation,
                permeabilityMean,
                permeabilityCoefficientOfVariation);

            mocks.ReplayAll();

            // Call
            MacroStabilityInwardsSoilProfile1D profile = SoilProfile1DReader.ReadFrom(reader);

            Assert.AreEqual(bottom, profile.Bottom);

            // Assert
            Assert.AreEqual(layerCount, profile.Layers.Count());

            MacroStabilityInwardsSoilLayer1D macroStabilityInwardsSoilLayer = profile.Layers.First();
            Assert.AreEqual(top, macroStabilityInwardsSoilLayer.Top);
            Assert.IsTrue(macroStabilityInwardsSoilLayer.IsAquifer);
            Assert.AreEqual(materialName, macroStabilityInwardsSoilLayer.MaterialName);
            Assert.AreEqual(color, macroStabilityInwardsSoilLayer.Color);

            mocks.VerifyAll();
        }

        private void SetExpectations(int layerCount, string profileName, double bottom, double top, double? isAquifer,
                                     string materialName, double? color, double? belowPhreaticLevelMean, double? belowPhreaticLevelDeviation,
                                     double? diameterD70Mean, double? diameterD70CoefficientOfVariation, double? permeabilityMean,
                                     double? permeabilityCoefficientOfVariation)
        {
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.Bottom)).Return(bottom).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.Top)).Return(top).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.IsAquifer)).Return(isAquifer).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<string>(SoilProfileTableColumns.MaterialName)).Return(materialName).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.Color)).Return(color).Repeat.Any();

            const int logNormalDistribution = 3;
            const int logNormalShift = 0;
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelDistribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelShift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelMean)).Return(belowPhreaticLevelMean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelDeviation)).Return(belowPhreaticLevelDeviation).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Distribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Shift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Mean)).Return(diameterD70Mean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70CoefficientOfVariation)).Return(diameterD70CoefficientOfVariation).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityDistribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityShift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityMean)).Return(permeabilityMean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityCoefficientOfVariation)).Return(permeabilityCoefficientOfVariation).Repeat.Any();
        }
    }
}
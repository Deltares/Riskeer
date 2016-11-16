// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.SoilProfile
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

            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(profileName);
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Throw(new InvalidCastException());
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisatie '{0}'", profileName))
                .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidRequiredProperty_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            const string profileName = "<profile name>";
            const string path = "A";

            reader.Expect(r => r.Path).Return(path);
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.Bottom)).Throw(new InvalidCastException());

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisatie '{0}'", profileName))
                .Build("Ondergrondschematisatie bevat geen geldige waarde in kolom 'Bottom'.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_ZeroLayerCount_ThrowsPipingSoilProfileReadException()
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
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisatie '{0}'", profileName))
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

            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(profileName);
            reader.Expect(r => r.Path).Return(path);
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.IsAquifer)).Throw(new InvalidCastException());
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisatie '{0}'", profileName))
                .Build("Ondergrondschematisatie bevat geen geldige waarde in kolom 'IsAquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_NullValuesForLayer_ReturnsProfileWithNaNValuesAndDefaultsOnLayer()
        {
            // Setup
            var bottom = 1.1;
            var top = 1.1;
            SetExpectations(1, "", bottom, top, null, null, null, null, null, null, null, null, null);

            mocks.ReplayAll();

            // Call
            PipingSoilProfile profile = SoilProfile1DReader.ReadFrom(reader);

            // Assert
            Assert.AreEqual(1, profile.Layers.Count());
            Assert.AreEqual(bottom, profile.Bottom);

            PipingSoilLayer pipingSoilLayer = profile.Layers.First();

            Assert.AreEqual(top, pipingSoilLayer.Top);
            Assert.IsEmpty(pipingSoilLayer.MaterialName);
            Assert.AreEqual(Color.Empty, pipingSoilLayer.Color);
            Assert.IsFalse(pipingSoilLayer.IsAquifer);

            Assert.IsNaN(pipingSoilLayer.BelowPhreaticLevelMean);
            Assert.IsNaN(pipingSoilLayer.BelowPhreaticLevelDeviation);
            Assert.IsNaN(pipingSoilLayer.DiameterD70Mean);
            Assert.IsNaN(pipingSoilLayer.DiameterD70Deviation);
            Assert.IsNaN(pipingSoilLayer.PermeabilityMean);
            Assert.IsNaN(pipingSoilLayer.PermeabilityDeviation);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidBelowPhreaticLevelDistributionValue_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return("");
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileDatabaseColumns.BelowPhreaticLevelDistribution)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            string message = Assert.Throws<PipingSoilProfileReadException>(test).Message;
            string expected = string.Format(
                "Fout bij het lezen van bestand '' (ondergrondschematisatie ''): parameter '{0}' is niet verschoven lognormaal verdeeld.",
                "Verzadigd gewicht");
            Assert.AreEqual(expected, message);
            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidDiameterD70DistributionValue_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return("");
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileDatabaseColumns.DiameterD70Distribution)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            string message = Assert.Throws<PipingSoilProfileReadException>(test).Message;
            string expected = string.Format(
                "Fout bij het lezen van bestand '' (ondergrondschematisatie ''): parameter '{0}' is niet lognormaal verdeeld.",
                "Korrelgrootte");
            Assert.AreEqual(expected, message);
            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidDiameterD70ShiftValue_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return("");
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileDatabaseColumns.DiameterD70Distribution)).Return(SoilLayerConstants.LogNormalDistributionValue);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.DiameterD70Shift)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            string message = Assert.Throws<PipingSoilProfileReadException>(test).Message;
            string expected = string.Format(
                "Fout bij het lezen van bestand '' (ondergrondschematisatie ''): parameter '{0}' is niet lognormaal verdeeld.",
                "Korrelgrootte");
            Assert.AreEqual(expected, message);
            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidPermeabilityDistributionValue_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return("");
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileDatabaseColumns.PermeabilityDistribution)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            string message = Assert.Throws<PipingSoilProfileReadException>(test).Message;
            string expected = string.Format(
                "Fout bij het lezen van bestand '' (ondergrondschematisatie ''): parameter '{0}' is niet lognormaal verdeeld.",
                "Doorlatendheid");
            Assert.AreEqual(expected, message);
            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidPermeabilityShiftValue_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return("");
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileDatabaseColumns.PermeabilityDistribution)).Return(SoilLayerConstants.LogNormalDistributionValue);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.PermeabilityShift)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            string message = Assert.Throws<PipingSoilProfileReadException>(test).Message;
            string expected = string.Format(
                "Fout bij het lezen van bestand '' (ondergrondschematisatie ''): parameter '{0}' is niet lognormaal verdeeld.",
                "Doorlatendheid");
            Assert.AreEqual(expected, message);
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
            var bottom = random.NextDouble();
            var top = bottom + random.NextDouble();
            string materialName = "material";
            Color color = Color.FromArgb(Color.DarkKhaki.ToArgb());

            var belowPhreaticLevelMean = random.NextDouble();
            var belowPhreaticLevelDeviation = random.NextDouble();
            var diameterD70Mean = random.NextDouble();
            var diameterD70Deviation = random.NextDouble();
            var permeabilityMean = random.NextDouble();
            var permeabilityDeviation = random.NextDouble();

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
                diameterD70Deviation,
                permeabilityMean,
                permeabilityDeviation);

            mocks.ReplayAll();

            // Call
            PipingSoilProfile profile = SoilProfile1DReader.ReadFrom(reader);

            Assert.AreEqual(bottom, profile.Bottom);

            // Assert
            Assert.AreEqual(layerCount, profile.Layers.Count());

            PipingSoilLayer pipingSoilLayer = profile.Layers.First();
            Assert.AreEqual(top, pipingSoilLayer.Top);
            Assert.IsTrue(pipingSoilLayer.IsAquifer);
            Assert.AreEqual(materialName, pipingSoilLayer.MaterialName);
            Assert.AreEqual(color, pipingSoilLayer.Color);

            Assert.AreEqual(belowPhreaticLevelMean, pipingSoilLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, pipingSoilLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(diameterD70Mean, pipingSoilLayer.DiameterD70Mean);
            Assert.AreEqual(diameterD70Deviation, pipingSoilLayer.DiameterD70Deviation);
            Assert.AreEqual(permeabilityMean, pipingSoilLayer.PermeabilityMean);
            Assert.AreEqual(permeabilityDeviation, pipingSoilLayer.PermeabilityDeviation);

            mocks.VerifyAll();
        }

        private void SetExpectations(int layerCount, string profileName, double bottom, double top, double? isAquifer, string materialName, double? color, double? belowPhreaticLevelMean, double? belowPhreaticLevelDeviation, double? diameterD70Mean, double? diameterD70Deviation, double? permeabilityMean, double? permeabilityDeviation)
        {
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.Bottom)).Return(bottom).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.Top)).Return(top).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.IsAquifer)).Return(isAquifer).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<string>(SoilProfileDatabaseColumns.MaterialName)).Return(materialName).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.Color)).Return(color).Repeat.Any();

            var logNormalDistribution = 3;
            var logNormalShift = 0;
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.BelowPhreaticLevelDistribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.BelowPhreaticLevelShift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.BelowPhreaticLevelMean)).Return(belowPhreaticLevelMean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.BelowPhreaticLevelDeviation)).Return(belowPhreaticLevelDeviation).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.DiameterD70Distribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.DiameterD70Shift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.DiameterD70Mean)).Return(diameterD70Mean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.DiameterD70Deviation)).Return(diameterD70Deviation).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.PermeabilityDistribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.PermeabilityShift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.PermeabilityMean)).Return(permeabilityMean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.PermeabilityDeviation)).Return(permeabilityDeviation).Repeat.Any();
        }
    }
}
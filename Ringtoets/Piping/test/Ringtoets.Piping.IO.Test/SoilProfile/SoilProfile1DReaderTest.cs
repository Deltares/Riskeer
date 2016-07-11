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
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;

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
            var exception = Assert.Throws<CriticalFileReadException>(test);
            var expectedMessage = new FileReaderErrorMessageBuilder(path)
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
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = new FileReaderErrorMessageBuilder(path)
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

            SetExpectations(0, profileName, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, string.Empty, 0);
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = new FileReaderErrorMessageBuilder(path)
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
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisatie '{0}'", profileName))
                .Build("Ondergrondschematisatie bevat geen geldige waarde in kolom 'IsAquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_NullValuesForLayer_ReturnsProfileWithNullValuesAndDefaultsOnLayer()
        {
            // Setup
            var bottom = 1.1;
            var top = 1.1;
            SetExpectations(1, "", bottom, top, null, null, null, null, null, null);

            mocks.ReplayAll();

            // Call
            var profile = SoilProfile1DReader.ReadFrom(reader);

            // Assert
            Assert.AreEqual(1, profile.Layers.Count());
            Assert.AreEqual(bottom, profile.Bottom);

            var pipingSoilLayer = profile.Layers.First();

            Assert.AreEqual(top, pipingSoilLayer.Top);
            Assert.IsNull(pipingSoilLayer.BelowPhreaticLevel);
            Assert.IsNull(pipingSoilLayer.AbovePhreaticLevel);
            Assert.IsNull(pipingSoilLayer.DryUnitWeight);
            Assert.IsEmpty(pipingSoilLayer.MaterialName);
            Assert.AreEqual(Color.Empty, pipingSoilLayer.Color);
            Assert.IsFalse(pipingSoilLayer.IsAquifer);

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
            var belowPhreaticLevel = random.NextDouble();
            var abovePhreaticLevel = random.NextDouble();
            var dryUnitWeight = random.NextDouble();
            var top = random.NextDouble();
            var bottom = random.NextDouble();
            string materialName = "material";
            var color = Color.FromArgb(Color.DarkKhaki.ToArgb());

            SetExpectations(layerCount, "", bottom, top, 1.0, belowPhreaticLevel, abovePhreaticLevel, dryUnitWeight, materialName, color.ToArgb());

            mocks.ReplayAll();

            // Call
            var profile = SoilProfile1DReader.ReadFrom(reader);

            Assert.AreEqual(bottom, profile.Bottom);

            // Assert
            Assert.AreEqual(layerCount, profile.Layers.Count());

            var pipingSoilLayer = profile.Layers.First();
            Assert.AreEqual(top, pipingSoilLayer.Top);
            Assert.AreEqual(belowPhreaticLevel, pipingSoilLayer.BelowPhreaticLevel);
            Assert.AreEqual(abovePhreaticLevel, pipingSoilLayer.AbovePhreaticLevel);
            Assert.AreEqual(dryUnitWeight, pipingSoilLayer.DryUnitWeight);
            Assert.IsTrue(pipingSoilLayer.IsAquifer);
            Assert.AreEqual(materialName, pipingSoilLayer.MaterialName);
            Assert.AreEqual(color, pipingSoilLayer.Color);
            mocks.VerifyAll();
        }

        private void SetExpectations(int layerCount, string profileName, double bottom, double top, double? isAquifer, double? belowPhreaticLevel, double? abovePhreaticLevel, double? dryUnitWeight, string materialName, double? color)
        {
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.Bottom)).Return(bottom).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.Top)).Return(top).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.IsAquifer)).Return(isAquifer).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.AbovePhreaticLevel)).Return(abovePhreaticLevel).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.DryUnitWeight)).Return(dryUnitWeight).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.BelowPhreaticLevelMean)).Return(belowPhreaticLevel).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.DiameterD70Mean)).Return(abovePhreaticLevel).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.PermeabilityMean)).Return(dryUnitWeight).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<string>(SoilProfileDatabaseColumns.MaterialName)).Return(materialName).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileDatabaseColumns.Color)).Return(color).Repeat.Any();
        }
    }
}
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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.IO.SoilProfile.Schema;
using Ringtoets.Piping.IO.Test.TestHelpers;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfile2DReaderTest
    {
        private MockRepository mocks;
        private IRowBasedDatabaseReader reader;

        private readonly byte[] someGeometry = StringGeometryHelper.GetByteArray("<GeometrySurface><OuterLoop><CurveList>" +
                                                                                 "<GeometryCurve>" +
                                                                                 "<HeadPoint><X>0</X><Y>0</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0</Y><Z>1.1</Z></EndPoint>" +
                                                                                 "</GeometryCurve>" +
                                                                                 "<GeometryCurve>" +
                                                                                 "<HeadPoint><X>0</X><Y>0</Y><Z>1.1</Z></HeadPoint><EndPoint><X>1</X><Y>0</Y><Z>1.1</Z></EndPoint>" +
                                                                                 "</GeometryCurve>" +
                                                                                 "</CurveList></OuterLoop>" +
                                                                                 "<InnerLoops/></GeometrySurface>");

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
            const string path = "A";
            const string name = "B";

            reader.Expect(r => r.Path).Return(path);
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(name);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Throw(new InvalidCastException());

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidRequiredProperty_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            const string name = "A";
            const string path = "B";

            reader.Expect(r => r.Path).Return(path);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(name).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.IntersectionX)).Throw(new InvalidCastException());

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Ondergrondschematisatie bevat geen geldige waarde in kolom 'IntersectionX'.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_DoubleNaNIntersectionX_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            const string name = "profile";
            const string path = "A";

            reader.Expect(r => r.Path).Return(path);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(name).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.IntersectionX)).Return(double.NaN);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, string.Format("Geen geldige X waarde gevonden om intersectie te maken uit 2D profiel '{0}'.", name));
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_ZeroLayerCount_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            const string name = "profile";
            const string path = "A";

            reader.Expect(r => r.Path).Return(path);
            SetExpectations(0, name, 0.0, 1.0, string.Empty, 0, new byte[0], null, null, null, null, null, null);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Geen lagen gevonden voor de ondergrondschematisatie.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_NullGeometry_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            const string name = "profile";
            const string path = "A";

            reader.Expect(r => r.Path).Return(path);
            SetExpectations(1, name, 0.0, 1.0, string.Empty, 0, null, null, null, null, null, null, null);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "De geometrie is leeg.");
            StringAssert.StartsWith(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_EmptyGeometry_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            const string name = "cool name";
            const string path = "A";

            SetExpectations(1, name, 0.0, 1.0, string.Empty, 0, new byte[0], null, null, null, null, null, null);
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidIsAquifer_ReturnsProfileWithNullValuesOnLayer()
        {
            // Setup
            const string name = "cool name";
            const string path = "A";

            reader.Expect(r => r.Path).Return(path);
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(name);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.IsAquifer)).Throw(new InvalidCastException());
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            string expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Ondergrondschematisatie bevat geen geldige waarde in kolom 'IsAquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_NullValuesForLayer_ReturnsProfileWithNullValuesAndDefaultsOnLayer()
        {
            // Setup
            SetExpectations(1, "", 0.0, null, null, null, someGeometry, null, null, null, null, null, null);

            mocks.ReplayAll();

            // Call
            PipingSoilProfile profile = SoilProfile2DReader.ReadFrom(reader);

            // Assert
            Assert.AreEqual(1, profile.Layers.Count());
            Assert.AreEqual(1.1, profile.Bottom);

            PipingSoilLayer pipingSoilLayer = profile.Layers.First();

            Assert.AreEqual(1.1, pipingSoilLayer.Top);
            Assert.IsFalse(pipingSoilLayer.IsAquifer);
            Assert.IsEmpty(pipingSoilLayer.MaterialName);
            Assert.AreEqual(Color.Empty, pipingSoilLayer.Color);

            Assert.IsNaN(pipingSoilLayer.BelowPhreaticLevelMean);
            Assert.IsNaN(pipingSoilLayer.BelowPhreaticLevelDeviation);
            Assert.IsNaN(pipingSoilLayer.DiameterD70Mean);
            Assert.IsNaN(pipingSoilLayer.DiameterD70Deviation);
            Assert.IsNaN(pipingSoilLayer.PermeabilityMean);
            Assert.IsNaN(pipingSoilLayer.PermeabilityDeviation);

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
            var intersectionX = 0.5;
            var materialName = "material";
            Color color = Color.FromArgb(Color.AliceBlue.ToArgb());

            var belowPhreaticLevelMean = random.NextDouble();
            var belowPhreaticLevelDeviation = random.NextDouble();
            var diameterD70Mean = random.NextDouble();
            var diameterD70Deviation = random.NextDouble();
            var permeabilityMean = random.NextDouble();
            var permeabilityDeviation = random.NextDouble();

            SetExpectations(
                layerCount,
                "",
                intersectionX,
                1.0,
                materialName,
                color.ToArgb(),
                someGeometry,
                belowPhreaticLevelMean,
                belowPhreaticLevelDeviation,
                diameterD70Mean,
                diameterD70Deviation,
                permeabilityMean,
                permeabilityDeviation);

            mocks.ReplayAll();

            // Call
            PipingSoilProfile profile = SoilProfile2DReader.ReadFrom(reader);

            // Assert
            Assert.AreEqual(layerCount, profile.Layers.Count());

            Assert.AreEqual(1.1, profile.Bottom);

            PipingSoilLayer pipingSoilLayer = profile.Layers.First();
            Assert.AreEqual(1.1, pipingSoilLayer.Top);
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

        [Test]
        public void ReadFrom_InvalidBelowPhreaticLevelDistributionValue_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return("");
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.IntersectionX)).Return(0.0).Repeat.Any();
            reader.Expect(r => r.Read<byte[]>(SoilProfileTableColumns.LayerGeometry)).Return(someGeometry).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableColumns.BelowPhreaticLevelDistribution)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

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
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return("");
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.IntersectionX)).Return(0.0).Repeat.Any();
            reader.Expect(r => r.Read<byte[]>(SoilProfileTableColumns.LayerGeometry)).Return(someGeometry).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableColumns.DiameterD70Distribution)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

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
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return("");
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.IntersectionX)).Return(0.0).Repeat.Any();
            reader.Expect(r => r.Read<byte[]>(SoilProfileTableColumns.LayerGeometry)).Return(someGeometry).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableColumns.DiameterD70Distribution)).Return(SoilLayerConstants.LogNormalDistributionValue);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Shift)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

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
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return("");
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.IntersectionX)).Return(0.0).Repeat.Any();
            reader.Expect(r => r.Read<byte[]>(SoilProfileTableColumns.LayerGeometry)).Return(someGeometry).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableColumns.PermeabilityDistribution)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

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
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(1);
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return("");
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.IntersectionX)).Return(0.0).Repeat.Any();
            reader.Expect(r => r.Read<byte[]>(SoilProfileTableColumns.LayerGeometry)).Return(someGeometry).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableColumns.PermeabilityDistribution)).Return(SoilLayerConstants.LogNormalDistributionValue);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityShift)).Return(1);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            string message = Assert.Throws<PipingSoilProfileReadException>(test).Message;
            string expected = string.Format(
                "Fout bij het lezen van bestand '' (ondergrondschematisatie ''): parameter '{0}' is niet lognormaal verdeeld.",
                "Doorlatendheid");
            Assert.AreEqual(expected, message);
            mocks.VerifyAll();
        }

        private void SetExpectations(int layerCount, string profileName, double intersectionX, double? isAquifer, string materialName, double? color, byte[] geometry, double? belowPhreaticLevelMean, double? belowPhreaticLevelDeviation, double? diameterD70Mean, double? diameterD70Deviation, double? permeabilityMean, double? permeabilityDeviation)
        {
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileTableColumns.IntersectionX)).Return(intersectionX).Repeat.Any();
            reader.Expect(r => r.Read<byte[]>(SoilProfileTableColumns.LayerGeometry)).Return(geometry).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.IsAquifer)).Return(isAquifer).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<string>(SoilProfileTableColumns.MaterialName)).Return(materialName).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.Color)).Return(color).Repeat.Any();

            var logNormalDistribution = 3;
            var logNormalShift = 0;
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelDistribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelShift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelMean)).Return(belowPhreaticLevelMean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelDeviation)).Return(belowPhreaticLevelDeviation).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Distribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Shift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Mean)).Return(diameterD70Mean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Deviation)).Return(diameterD70Deviation).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityDistribution)).Return(logNormalDistribution).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityShift)).Return(logNormalShift).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityMean)).Return(permeabilityMean).Repeat.Any();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityDeviation)).Return(permeabilityDeviation).Repeat.Any();
        }

        private static string GetExpectedSoilProfileReaderErrorMessage(string path, string name, string errorMessage)
        {
            return new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisatie '{0}'", name))
                .Build(errorMessage);
        }
    }
}
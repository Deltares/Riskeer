using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.IO.Test.TestHelpers;

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
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(name);
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Throw(new InvalidCastException());

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            var expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
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
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(name).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.IntersectionX)).Throw(new InvalidCastException());

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Ondergrondschematisering bevat geen geldige waarde in kolom 'IntersectionX'.");
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
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(name).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.IntersectionX)).Return(double.NaN);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, string.Format("Geen geldige X waarde gevonden om intersectie te maken uit 2D profiel '{0}'.", name));
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
            SetExpectations(0, name, 0.0, 1.0, 0.0, 0.0, 0.0, new byte[0]);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Geen lagen gevonden voor het profiel.");
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
            SetExpectations(1, name, 0.0, 1.0, 0.0, 0.0, 0.0, null);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "De geometrie is leeg.");
            StringAssert.StartsWith(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_EmptyGeometry_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            const string name = "cool name";
            const string path = "A";

            SetExpectations(1, name, 0.0, 1.0, 0.0, 0.0, 0.0, new byte[0]);
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.");
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
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(name);
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.IsAquifer)).Throw(new InvalidCastException());
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile2DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            var expectedMessage = GetExpectedSoilProfileReaderErrorMessage(path, name, "Ondergrondschematisering bevat geen geldige waarde in kolom 'IsAquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_NullValuesForLayer_ReturnsProfileWithNullValuesOnLayer()
        {
            // Setup
            SetExpectations(1, "", 0.0, null, null, null, null, someGeometry);

            mocks.ReplayAll();

            // Call
            var profile = SoilProfile2DReader.ReadFrom(reader);

            // Assert
            Assert.AreEqual(1, profile.Layers.Count());
            Assert.AreEqual(1.1, profile.Bottom);

            var pipingSoilLayer = profile.Layers.First();

            Assert.AreEqual(1.1, pipingSoilLayer.Top);
            Assert.IsNull(pipingSoilLayer.BelowPhreaticLevel);
            Assert.IsNull(pipingSoilLayer.AbovePhreaticLevel);
            Assert.IsNull(pipingSoilLayer.DryUnitWeight);
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
            var intersectionX = 0.5;

            SetExpectations(layerCount, "", intersectionX, 1.0, belowPhreaticLevel, abovePhreaticLevel, dryUnitWeight, someGeometry);

            mocks.ReplayAll();

            // Call
            var profile = SoilProfile2DReader.ReadFrom(reader);

            // Assert
            Assert.AreEqual(layerCount, profile.Layers.Count());

            Assert.AreEqual(1.1, profile.Bottom);

            var pipingSoilLayer = profile.Layers.First();
            Assert.AreEqual(1.1, pipingSoilLayer.Top);
            Assert.AreEqual(belowPhreaticLevel, pipingSoilLayer.BelowPhreaticLevel);
            Assert.AreEqual(abovePhreaticLevel, pipingSoilLayer.AbovePhreaticLevel);
            Assert.AreEqual(dryUnitWeight, pipingSoilLayer.DryUnitWeight);
            Assert.IsTrue(pipingSoilLayer.IsAquifer);
            mocks.VerifyAll();
        }

        private void SetExpectations(int layerCount, string profileName, double intersectionX, double? isAquifer, double? belowPhreaticLevel, double? abovePhreaticLevel, double? dryUnitWeight, byte[] geometry)
        {
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.IntersectionX)).Return(intersectionX).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.IsAquifer)).Return(isAquifer).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.BelowPhreaticLevel)).Return(belowPhreaticLevel).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.AbovePhreaticLevel)).Return(abovePhreaticLevel).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.DryUnitWeight)).Return(dryUnitWeight).Repeat.Any();
            reader.Expect(r => r.Read<byte[]>(SoilProfileDatabaseColumns.LayerGeometry)).Return(geometry).Repeat.Any();
        }

        private static string GetExpectedSoilProfileReaderErrorMessage(string path, string name, string errorMessage)
        {
            return new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisering '{0}'", name))
                .Build(errorMessage);
        }
    }
}
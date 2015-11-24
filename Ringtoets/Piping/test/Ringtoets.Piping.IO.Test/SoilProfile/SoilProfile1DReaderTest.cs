﻿using System;
using System.Linq;
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
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Throw(new InvalidCastException());

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidRequiredProperty_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return("").Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.Bottom)).Throw(new InvalidCastException());

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            Assert.AreEqual("Ondergrondschematisering '' in database bevat geen geldige waarde in kolom 'Bottom'.", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_ZeroLayerCount_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            SetExpectations(0, "", 0.0, 0.0, 1.0, 0.0, 0.0, 0.0);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            Assert.AreEqual("Geen lagen gevonden voor het profiel.", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_InvalidIsAquifer_ReturnsProfileWithNullValuesOnLayer()
        {
            // Setup
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(1).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.IsAquifer)).Throw(new InvalidCastException());
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => SoilProfile1DReader.ReadFrom(reader);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            Assert.AreEqual("Ondergrondschematisering '' in database bevat geen geldige waarde in kolom 'IsAquifer'.", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void ReadFrom_NullValuesForLayer_ReturnsProfileWithNullValuesOnLayer()
        {
            // Setup
            var bottom = 1.1;
            var top = 1.1;
            SetExpectations(1, "", bottom, top, null, null, null, null);

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

            SetExpectations(layerCount, "", bottom, top, 1.0, belowPhreaticLevel, abovePhreaticLevel, dryUnitWeight);

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
            mocks.VerifyAll();
        }

        private void SetExpectations(int layerCount, string profileName, double bottom, double top, double? isAquifer, double? belowPhreaticLevel, double? abovePhreaticLevel, double? dryUnitWeight)
        {
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.Bottom)).Return(bottom).Repeat.Any();
            reader.Expect(r => r.Read<double>(SoilProfileDatabaseColumns.Top)).Return(top).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.IsAquifer)).Return(isAquifer).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.BelowPhreaticLevel)).Return(belowPhreaticLevel).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.AbovePhreaticLevel)).Return(abovePhreaticLevel).Repeat.Any();
            reader.Expect(r => r.ReadOrNull<double>(SoilProfileDatabaseColumns.DryUnitWeight)).Return(dryUnitWeight).Repeat.Any();
        }
    }
}
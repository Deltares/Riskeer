// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfile1DReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(SoilProfile1DReader));

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            void Call()
            {
                using (new SoilProfile1DReader(testFile)) {}
            }

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_InvalidPath_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            void Call() => new SoilProfile1DReader(fileName);

            // Assert
            Assert.Throws<CriticalFileReadException>(Call);
        }

        [Test]
        public void Constructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            // Call
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Initialize_IncorrectFormatFileOrInvalidSchema_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "text.txt");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                // Call
                void Call() => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(Call);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kon geen ondergrondschematisaties verkrijgen uit de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_IncorrectCriticalProperty_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileInvalidId.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                void Call() => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(Call);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile)
                                         .WithSubject("ondergrondschematisatie 'Profile'")
                                         .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenReadSoilProfileThrowsException_WhenReadingNextProfile_ThenReturnsNextProfile()
        {
            // Given
            string dbFile = Path.Combine(testDataPath, "1dprofileWithInvalidLayerProperty.soil");

            SoilProfileReadException exception = null;
            var readSoilProfiles = new List<SoilProfileWrapper<SoilProfile1D>>();
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // When
                try
                {
                    reader.ReadSoilProfile();
                }
                catch (SoilProfileReadException e)
                {
                    exception = e;
                }

                // Then
                readSoilProfiles.Add(reader.ReadSoilProfile());
            }

            Assert.IsInstanceOf<SoilProfileReadException>(exception);
            Assert.AreEqual("Profile", exception.ProfileName);
            Assert.AreEqual(1, readSoilProfiles.Count);
            Assert.AreEqual("Profile2", readSoilProfiles[0].SoilProfile.Name);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        [TestCase("1dprofile", FailureMechanismType.Piping)]
        [TestCase("1dprofileStability", FailureMechanismType.Stability)]
        public void ReadSoilProfile_DatabaseWith1DSoilProfileWithValidFailureMechanismType_ReturnsExpectedFailureMechanismType(
            string fileName, FailureMechanismType expectedFailureMechanismType)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, $"{fileName}.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfileWrapper<SoilProfile1D> readProfile = reader.ReadSoilProfile();

                // Assert
                Assert.AreEqual(expectedFailureMechanismType, readProfile.FailureMechanismType);
            }
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith1DSoilProfileWithOtherFailureMechanism_ThrowsSoilProfileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileInvalidFailureMechanismType.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                void Call() => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<SoilProfileReadException>(Call);

                const string expectedMessage = "Het faalmechanisme 'UNKNOWN' wordt niet ondersteund.";
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith1DAnd1DSoilProfileWithoutSoilLayers_ReturnOneProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithEmpty1d.soil");

            var result = new Collection<SoilProfileWrapper<SoilProfile1D>>();
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                while (reader.HasNext)
                {
                    result.Add(reader.ReadSoilProfile());
                }
            }

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Profile", result[0].SoilProfile.Name);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_LayerTopInvalidValue_ThrowsSoilProfileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithInvalidTopLayer.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                void Call() => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<SoilProfileReadException>(Call);
                const string expectedMessage = "Het lezen van de ondergrondschematisatie 'INCORRECT' is mislukt. " +
                                               "Geen geldige waarde in kolom 'Top'.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.AreEqual("INCORRECT", exception.ProfileName);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_BottomInvalidValue_ThrowsSoilProfileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithInvalidBottom.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                void Call() => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<SoilProfileReadException>(Call);
                const string expectedMessage = "Het lezen van de ondergrondschematisatie 'Profile' is mislukt. " +
                                               "Geen geldige waarde in kolom 'Bottom'.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.AreEqual("Profile", exception.ProfileName);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_BottomAboveLayers_ReturnsProfileWithExpectedBottomAndTopValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithIncorrectBottom.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfileWrapper<SoilProfile1D> profile = reader.ReadSoilProfile();

                // Assert
                SoilProfile1D soilProfile = profile.SoilProfile;
                Assert.AreEqual(9999999, soilProfile.Bottom);

                CollectionAssert.AreEqual(new[]
                {
                    1.1,
                    2.2,
                    3.3
                }, soilProfile.Layers.Select(layer => layer.Top));
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_Empty1DProfileWithoutLayers_ReturnsProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileNoLayers.soil");
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfileWrapper<SoilProfile1D> readSoilProfile = reader.ReadSoilProfile();

                // Assert
                SoilProfile1D soilProfile = readSoilProfile.SoilProfile;
                Assert.AreEqual("Schematisering1", soilProfile.Name);
                Assert.AreEqual(1, soilProfile.Id);
                Assert.IsNaN(soilProfile.Bottom);
                CollectionAssert.IsEmpty(soilProfile.Layers);
            }
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith1DProfile4Layers_ReturnsProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofile.soil");
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfileWrapper<SoilProfile1D> readSoilProfile = reader.ReadSoilProfile();

                // Assert
                SoilProfile1D soilProfile = readSoilProfile.SoilProfile;
                Assert.AreEqual(-40, soilProfile.Bottom);
                Assert.AreEqual("Schematisering1", soilProfile.Name);

                Assert.AreEqual(4, soilProfile.Layers.Count());
                CollectionAssert.AreEqual(new[]
                {
                    30,
                    10,
                    -20,
                    -30
                }, soilProfile.Layers.Select(l => l.Top));

                CollectionAssert.AreEqual(new[]
                {
                    "Klei1",
                    "Zand",
                    "dummy",
                    "Klei 3"
                }, soilProfile.Layers.Select(l => l.MaterialName));

                CollectionAssert.AreEqual(new[]
                {
                    0.0,
                    1.0,
                    1.0,
                    1.0
                }, soilProfile.Layers.Select(l => l.IsAquifer));

                CollectionAssert.AreEqual(new[]
                {
                    -12156236,
                    -65536,
                    -65536,
                    -4144897
                }, soilProfile.Layers.Select(l => l.Color));

                CollectionAssert.AreEqual(new[]
                {
                    2,
                    2,
                    2,
                    2
                }, soilProfile.Layers.Select(l => l.BelowPhreaticLevelDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    24,
                    20,
                    30,
                    28
                }, soilProfile.Layers.Select(l => l.BelowPhreaticLevelMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.24,
                    0.2,
                    0.3,
                    0.28
                }, soilProfile.Layers.Select(l => l.BelowPhreaticLevelDeviation));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.01,
                    0.01,
                    0.01
                }, soilProfile.Layers.Select(l => l.BelowPhreaticLevelCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0,
                    0,
                    0
                }, soilProfile.Layers.Select(l => l.BelowPhreaticLevelShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, soilProfile.Layers.Select(l => l.DiameterD70DistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    0.00017,
                    0.00016,
                    0.000155,
                    0.00017
                }, soilProfile.Layers.Select(l => l.DiameterD70Mean));
                CollectionAssert.AreEqual(new[]
                {
                    0.11764705882352941,
                    0.125,
                    0.12903225806451615,
                    0.11764705882352941
                }, soilProfile.Layers.Select(l => l.DiameterD70CoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0,
                    0,
                    0
                }, soilProfile.Layers.Select(l => l.DiameterD70Shift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, soilProfile.Layers.Select(l => l.PermeabilityDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0.000185,
                    0.001,
                    0
                }, soilProfile.Layers.Select(l => l.PermeabilityMean));
                CollectionAssert.AreEqual(new[]
                {
                    double.NaN,
                    0,
                    0,
                    double.NaN
                }, soilProfile.Layers.Select(l => l.PermeabilityCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0,
                    0,
                    0
                }, soilProfile.Layers.Select(l => l.PermeabilityShift));

                CollectionAssert.AreEqual(new double?[]
                {
                    null,
                    null,
                    0,
                    0
                }, soilProfile.Layers.Select(l => l.UsePop));

                CollectionAssert.AreEqual(new double?[]
                {
                    null,
                    9,
                    1,
                    6
                }, soilProfile.Layers.Select(l => l.ShearStrengthModel));

                CollectionAssert.AreEqual(new[]
                {
                    2,
                    2,
                    2,
                    2
                }, soilProfile.Layers.Select(l => l.AbovePhreaticLevelDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    14,
                    10,
                    20,
                    18
                }, soilProfile.Layers.Select(l => l.AbovePhreaticLevelMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.01,
                    0.01,
                    0.01
                }, soilProfile.Layers.Select(l => l.AbovePhreaticLevelCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0,
                    0,
                    0
                }, soilProfile.Layers.Select(l => l.AbovePhreaticLevelShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, soilProfile.Layers.Select(l => l.CohesionDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    1,
                    7,
                    4,
                    3
                }, soilProfile.Layers.Select(l => l.CohesionMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.1,
                    0.09999999999999999,
                    0.1,
                    0.09999999999999999
                }, soilProfile.Layers.Select(l => l.CohesionCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.07,
                    0.04,
                    0.03
                }, soilProfile.Layers.Select(l => l.CohesionShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, soilProfile.Layers.Select(l => l.FrictionAngleDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    11,
                    77,
                    44,
                    33
                }, soilProfile.Layers.Select(l => l.FrictionAngleMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.01,
                    0.01,
                    0.01
                }, soilProfile.Layers.Select(l => l.FrictionAngleCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.07,
                    0.04,
                    0.03
                }, soilProfile.Layers.Select(l => l.FrictionAngleShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, soilProfile.Layers.Select(l => l.ShearStrengthRatioDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    24,
                    78,
                    31,
                    28
                }, soilProfile.Layers.Select(l => l.ShearStrengthRatioMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.017499999999999998,
                    0.011153846153846153,
                    0.004193548387096774,
                    0.029285714285714283
                }, soilProfile.Layers.Select(l => l.ShearStrengthRatioCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0.04,
                    0.07,
                    0.03,
                    0.08
                }, soilProfile.Layers.Select(l => l.ShearStrengthRatioShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, soilProfile.Layers.Select(l => l.StrengthIncreaseExponentDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    1,
                    3,
                    4,
                    2
                }, soilProfile.Layers.Select(l => l.StrengthIncreaseExponentMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.1,
                    0.09999999999999999,
                    0.1,
                    0.1
                }, soilProfile.Layers.Select(l => l.StrengthIncreaseExponentCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.03,
                    0.04,
                    0.02
                }, soilProfile.Layers.Select(l => l.StrengthIncreaseExponentShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, soilProfile.Layers.Select(l => l.PopDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    111,
                    333,
                    444,
                    222
                }, soilProfile.Layers.Select(l => l.PopMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.000990990990990991,
                    0.000990990990990991,
                    0.000990990990990991,
                    0.000990990990990991
                }, soilProfile.Layers.Select(l => l.PopCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.03,
                    0.04,
                    0.02
                }, soilProfile.Layers.Select(l => l.PopShift));
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith1DProfile1LayerWithAllNullValues_ReturnsProfileWithDefaultValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithLayerWithNullValuesOnly.soil");
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfileWrapper<SoilProfile1D> readSoilProfile = reader.ReadSoilProfile();

                // Assert
                SoilProfile1D soilProfile = readSoilProfile.SoilProfile;
                Assert.AreEqual(1, soilProfile.Bottom);
                Assert.AreEqual("Schematisering1", soilProfile.Name);

                SoilLayer1D soilLayer = soilProfile.Layers.Single();
                Assert.AreEqual("dummy", soilLayer.MaterialName);
                Assert.AreEqual(1, soilLayer.Top);
                Assert.IsNull(soilLayer.IsAquifer);
                Assert.IsNull(soilLayer.Color);

                Assert.IsNull(soilLayer.BelowPhreaticLevelDistributionType);
                Assert.IsNaN(soilLayer.BelowPhreaticLevelMean);
                Assert.IsNaN(soilLayer.BelowPhreaticLevelDeviation);
                Assert.IsNaN(soilLayer.BelowPhreaticLevelCoefficientOfVariation);
                Assert.IsNaN(soilLayer.BelowPhreaticLevelShift);

                Assert.IsNull(soilLayer.DiameterD70DistributionType);
                Assert.IsNaN(soilLayer.DiameterD70Mean);
                Assert.IsNaN(soilLayer.DiameterD70CoefficientOfVariation);
                Assert.IsNaN(soilLayer.DiameterD70Shift);

                Assert.IsNull(soilLayer.PermeabilityDistributionType);
                Assert.IsNaN(soilLayer.PermeabilityMean);
                Assert.IsNaN(soilLayer.PermeabilityCoefficientOfVariation);
                Assert.IsNaN(soilLayer.PermeabilityShift);

                Assert.IsNull(soilLayer.UsePop);
                Assert.IsNull(soilLayer.ShearStrengthModel);

                Assert.IsNull(soilLayer.AbovePhreaticLevelDistributionType);
                Assert.IsNaN(soilLayer.AbovePhreaticLevelMean);
                Assert.IsNaN(soilLayer.AbovePhreaticLevelCoefficientOfVariation);
                Assert.IsNaN(soilLayer.AbovePhreaticLevelShift);

                Assert.IsNull(soilLayer.CohesionDistributionType);
                Assert.IsNaN(soilLayer.CohesionMean);
                Assert.IsNaN(soilLayer.CohesionCoefficientOfVariation);
                Assert.IsNaN(soilLayer.CohesionShift);

                Assert.IsNull(soilLayer.FrictionAngleDistributionType);
                Assert.IsNaN(soilLayer.FrictionAngleMean);
                Assert.IsNaN(soilLayer.FrictionAngleCoefficientOfVariation);
                Assert.IsNaN(soilLayer.FrictionAngleShift);

                Assert.IsNull(soilLayer.ShearStrengthRatioDistributionType);
                Assert.IsNaN(soilLayer.ShearStrengthRatioMean);
                Assert.IsNaN(soilLayer.ShearStrengthRatioCoefficientOfVariation);
                Assert.IsNaN(soilLayer.ShearStrengthRatioShift);

                Assert.IsNull(soilLayer.StrengthIncreaseExponentDistributionType);
                Assert.IsNaN(soilLayer.StrengthIncreaseExponentMean);
                Assert.IsNaN(soilLayer.StrengthIncreaseExponentCoefficientOfVariation);
                Assert.IsNaN(soilLayer.StrengthIncreaseExponentShift);

                Assert.IsNull(soilLayer.PopDistributionType);
                Assert.IsNaN(soilLayer.PopMean);
                Assert.IsNaN(soilLayer.PopCoefficientOfVariation);
                Assert.IsNaN(soilLayer.PopShift);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_SoilProfilePartOfMultipleFailureMechanismTypes_ReturnsProfilesWithExpectedFailureMechanismTypes()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithSameProfileInMultipleSegmentsAndDifferentFailureMechanismTypes.soil");

            var readProfiles = new List<SoilProfileWrapper<SoilProfile1D>>();
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                while (reader.HasNext)
                {
                    // Call
                    readProfiles.Add(reader.ReadSoilProfile());
                }

                // Assert
                Assert.IsFalse(reader.HasNext);
                Assert.AreEqual(2, readProfiles.Count);

                CollectionAssert.AreEqual(new[]
                {
                    FailureMechanismType.Stability,
                    FailureMechanismType.Piping
                }, readProfiles.Select(p => p.FailureMechanismType));

                Assert.True(readProfiles.All(p => p.SoilProfile.Name == "Segment_41009_1D1"));
                Assert.True(readProfiles.All(p => p.SoilProfile.Layers.Count() == 3));
                Assert.True(readProfiles.All(p => Math.Abs(p.SoilProfile.Bottom + 21) < 1e-6));
            }
        }
    }
}
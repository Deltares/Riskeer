// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfile2DReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(SoilProfile2DReader));

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            TestDelegate test = () =>
            {
                using (new SoilProfile2DReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_InvalidPath_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () => new SoilProfile2DReader(fileName);

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            // Call
            using (var reader = new SoilProfile2DReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Initialize_IncorrectFormatFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "text.txt");

            using (var reader = new SoilProfile2DReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

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
            string dbFile = Path.Combine(testDataPath, "2dprofileWithInvalidId.soil");

            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile)
                                         .WithSubject("ondergrondschematisatie 'Profile'")
                                         .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_Empty2DProfileWithoutLayers_ReturnsSoilProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileNoLayers.soil");

            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfile2D profile = reader.ReadSoilProfile();

                // Assert
                Assert.AreEqual("Profile", profile.Name);
                Assert.AreEqual(1, profile.Id);
                Assert.IsNaN(profile.IntersectionX);
                CollectionAssert.IsEmpty(profile.Layers);
                CollectionAssert.IsEmpty(profile.PreconsolidationStresses);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith2DSoilProfile3Layers_ReturnOneProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofile.soil");

            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfile2D soilProfile2D = reader.ReadSoilProfile();

                // Assert
                Assert.AreEqual(1, soilProfile2D.Id);
                Assert.AreEqual("Profile", soilProfile2D.Name);
                CollectionAssert.IsEmpty(soilProfile2D.PreconsolidationStresses);
                Assert.AreEqual(85.2, soilProfile2D.IntersectionX);
                Assert.AreEqual(3, soilProfile2D.Layers.Count());

                CollectionAssert.AreEqual(new[]
                {
                    1.0,
                    0.0,
                    0.0
                }, soilProfile2D.Layers.Select(l => l.IsAquifer));
                CollectionAssert.AreEqual(new[]
                {
                    -12156236,
                    -65536,
                    -8323200
                }, soilProfile2D.Layers.Select(l => l.Color));
                CollectionAssert.AreEqual(new[]
                {
                    "Material1",
                    "Material2",
                    "Material3"
                }, soilProfile2D.Layers.Select(l => l.MaterialName));

                CollectionAssert.AreEqual(new[]
                {
                    2,
                    2,
                    2
                }, soilProfile2D.Layers.Select(l => l.BelowPhreaticLevelDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    24,
                    28,
                    20
                }, soilProfile2D.Layers.Select(l => l.BelowPhreaticLevelMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.24,
                    0.28,
                    0.2
                }, soilProfile2D.Layers.Select(l => l.BelowPhreaticLevelDeviation));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.01,
                    0.01
                }, soilProfile2D.Layers.Select(l => l.BelowPhreaticLevelCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0.3,
                    0.32,
                    0.4
                }, soilProfile2D.Layers.Select(l => l.BelowPhreaticLevelShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3
                }, soilProfile2D.Layers.Select(l => l.DiameterD70DistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    0.00017,
                    0.00017,
                    0.00016
                }, soilProfile2D.Layers.Select(l => l.DiameterD70Mean));
                CollectionAssert.AreEqual(new[]
                {
                    0.11764705882352941,
                    0.11764705882352941,
                    0.125
                }, soilProfile2D.Layers.Select(l => l.DiameterD70CoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0,
                    0
                }, soilProfile2D.Layers.Select(l => l.DiameterD70Shift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3
                }, soilProfile2D.Layers.Select(l => l.PermeabilityDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0,
                    0.000185
                }, soilProfile2D.Layers.Select(l => l.PermeabilityMean));
                CollectionAssert.AreEqual(new[]
                {
                    double.NaN,
                    double.NaN,
                    0
                }, soilProfile2D.Layers.Select(l => l.PermeabilityCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0,
                    0
                }, soilProfile2D.Layers.Select(l => l.PermeabilityShift));

                CollectionAssert.AreEqual(new double?[]
                {
                    null,
                    0,
                    null
                }, soilProfile2D.Layers.Select(l => l.UsePop));

                CollectionAssert.AreEqual(new double?[]
                {
                    null,
                    6,
                    9
                }, soilProfile2D.Layers.Select(l => l.ShearStrengthModel));

                CollectionAssert.AreEqual(new[]
                {
                    2,
                    2,
                    2
                }, soilProfile2D.Layers.Select(l => l.AbovePhreaticLevelDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    14,
                    18,
                    10
                }, soilProfile2D.Layers.Select(l => l.AbovePhreaticLevelMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0,
                    0.01
                }, soilProfile2D.Layers.Select(l => l.AbovePhreaticLevelCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    10,
                    0,
                    0
                }, soilProfile2D.Layers.Select(l => l.AbovePhreaticLevelShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3
                }, soilProfile2D.Layers.Select(l => l.CohesionDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    1,
                    3,
                    7
                }, soilProfile2D.Layers.Select(l => l.CohesionMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.1,
                    0.09999999999999999,
                    0.09999999999999999
                }, soilProfile2D.Layers.Select(l => l.CohesionCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.03,
                    0.07
                }, soilProfile2D.Layers.Select(l => l.CohesionShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3
                }, soilProfile2D.Layers.Select(l => l.FrictionAngleDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    0.1,
                    33,
                    77
                }, soilProfile2D.Layers.Select(l => l.FrictionAngleMean));
                CollectionAssert.AreEqual(new[]
                {
                    10,
                    0.01,
                    0.01
                }, soilProfile2D.Layers.Select(l => l.FrictionAngleCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0.03,
                    0.07
                }, soilProfile2D.Layers.Select(l => l.FrictionAngleShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3
                }, soilProfile2D.Layers.Select(l => l.ShearStrengthRatioDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    1,
                    28,
                    78
                }, soilProfile2D.Layers.Select(l => l.ShearStrengthRatioMean));
                CollectionAssert.AreEqual(new[]
                {
                    1,
                    0.029285714285714283,
                    0.011153846153846153
                }, soilProfile2D.Layers.Select(l => l.ShearStrengthRatioCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    1,
                    0.08,
                    0.07
                }, soilProfile2D.Layers.Select(l => l.ShearStrengthRatioShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3
                }, soilProfile2D.Layers.Select(l => l.StrengthIncreaseExponentDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    1,
                    2,
                    3
                }, soilProfile2D.Layers.Select(l => l.StrengthIncreaseExponentMean));
                CollectionAssert.AreEqual(new[]
                {
                    1,
                    0.1,
                    0.09999999999999999
                }, soilProfile2D.Layers.Select(l => l.StrengthIncreaseExponentCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0,
                    0.02,
                    0.03
                }, soilProfile2D.Layers.Select(l => l.StrengthIncreaseExponentShift));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3
                }, soilProfile2D.Layers.Select(l => l.PopDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    111,
                    222,
                    333
                }, soilProfile2D.Layers.Select(l => l.PopMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.000990990990990991,
                    0.000990990990990991,
                    0.000990990990990991
                }, soilProfile2D.Layers.Select(l => l.PopCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    0.01,
                    0.02,
                    0.03
                }, soilProfile2D.Layers.Select(l => l.PopShift));
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith2DSoilProfileContainingNestedLayers_ReturnOneProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileNestedLayers.soil");

            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfile2D soilProfile2D = reader.ReadSoilProfile();

                // Assert
                Assert.AreEqual(1, soilProfile2D.Id);
                Assert.AreEqual("Profile", soilProfile2D.Name);
                CollectionAssert.IsEmpty(soilProfile2D.PreconsolidationStresses);
                Assert.AreEqual(90.0, soilProfile2D.IntersectionX, 1e-3);
                Assert.AreEqual(1, soilProfile2D.Layers.Count());

                SoilLayer2D layer = soilProfile2D.Layers.First();
                Assert.AreEqual("Material1", layer.MaterialName);
                Assert.AreEqual(2, layer.NestedLayers.Count());

                SoilLayer2D firstNestedLayer = layer.NestedLayers.First();
                Assert.AreEqual("Material4", firstNestedLayer.MaterialName);
                Assert.AreEqual(1, firstNestedLayer.NestedLayers.Count());
                Assert.AreEqual("Material3", firstNestedLayer.NestedLayers.First().MaterialName);
                Assert.AreEqual(0, firstNestedLayer.NestedLayers.First().NestedLayers.Count());

                SoilLayer2D secondNestedLayer = layer.NestedLayers.ElementAt(1);
                Assert.AreEqual("Material2", secondNestedLayer.MaterialName);
                Assert.AreEqual(0, secondNestedLayer.NestedLayers.Count());
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith2DProfile1LayerWithAllNullValues_ReturnsProfileWithDefaultValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileWithLayerWithNullValuesOnly.soil");
            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfile2D profile = reader.ReadSoilProfile();

                // Assert
                Assert.AreEqual(1, profile.Id);
                Assert.AreEqual("Profile", profile.Name);
                Assert.AreEqual(85.2, profile.IntersectionX);
                CollectionAssert.IsEmpty(profile.PreconsolidationStresses);

                SoilLayer2D soilLayer = profile.Layers.Single();
                Assert.AreEqual("Material1", soilLayer.MaterialName);
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
        public void ReadSoilProfile_DatabaseWith2DProfileWithPreconsolidationStresses_ReturnOneProfileWithStresses()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileWithPreconsolidationStresses.soil");
            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call 
                SoilProfile2D soilProfile2D = reader.ReadSoilProfile();

                // Assert
                Assert.AreEqual(4, soilProfile2D.PreconsolidationStresses.Count());

                CollectionAssert.AreEqual(new[]
                {
                    1,
                    2,
                    3,
                    4
                }, soilProfile2D.PreconsolidationStresses.Select(stress => stress.XCoordinate));
                CollectionAssert.AreEqual(new[]
                {
                    5,
                    6,
                    7,
                    8
                }, soilProfile2D.PreconsolidationStresses.Select(stress => stress.ZCoordinate));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, soilProfile2D.PreconsolidationStresses.Select(stress => stress.StressDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    1337,
                    3371,
                    8.5,
                    9.3
                }, soilProfile2D.PreconsolidationStresses.Select(stress => stress.StressMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.0074794315632011965,
                    0.0029664787896766538,
                    1.8823529411764706,
                    0.8064516129032258
                }, soilProfile2D.PreconsolidationStresses.Select(stress => stress.StressCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    11,
                    12,
                    0,
                    0
                }, soilProfile2D.PreconsolidationStresses.Select(stress => stress.StressShift));
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith2DProfileWithPreconsolidationStressesNullValues_ReturnsOneProfileWithDefaultStressValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileWithPreconsolidationStressesNullValues.soil");
            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call 
                SoilProfile2D soilProfile2D = reader.ReadSoilProfile();

                // Assert
                PreconsolidationStress[] preconsolidationStresses = soilProfile2D.PreconsolidationStresses.ToArray();
                Assert.AreEqual(1, preconsolidationStresses.Length);
                PreconsolidationStress actualPreconsolidationStress = preconsolidationStresses[0];

                Assert.IsNaN(actualPreconsolidationStress.XCoordinate);
                Assert.IsNaN(actualPreconsolidationStress.ZCoordinate);
                Assert.IsNull(actualPreconsolidationStress.StressDistributionType);
                Assert.IsNaN(actualPreconsolidationStress.StressMean);
                Assert.IsNaN(actualPreconsolidationStress.StressCoefficientOfVariation);
                Assert.IsNaN(actualPreconsolidationStress.StressShift);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_IntersectionXFor2DSoilProfileInvalid_ThrowsSoilProfileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dProfileWithXInvalid.soil");

            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<SoilProfileReadException>(test);

                const string expectedMessage = "Het lezen van de ondergrondschematisatie 'Profile' is mislukt. " +
                                               "Geen geldige waarde in kolom 'IntersectionX'.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_GeometryFor2DSoilProfileInvalid_ThrowsSoilProfileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileWithInvalidGeometry.soil");

            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<SoilProfileReadException>(test);

                const string expectedMessage = "Het lezen van de ondergrondschematisatie 'Profile' is mislukt. " +
                                               "Geen geldige waarde in kolom 'LayerGeometry'.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenReadSoilProfileThrowsException_WhenReadingNextProfile_ReturnsNextProfile()
        {
            // Given
            string dbFile = Path.Combine(testDataPath, "2dprofileWithInvalidLayerProperty.soil");

            SoilProfileReadException expectedException = null;
            var readSoilProfiles = new List<SoilProfile2D>();
            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                // When
                try
                {
                    reader.ReadSoilProfile();
                }
                catch (SoilProfileReadException e)
                {
                    expectedException = e;
                }

                // Then
                readSoilProfiles.Add(reader.ReadSoilProfile());
            }

            Assert.IsNotNull(expectedException);
            Assert.AreEqual("Profile", expectedException.ProfileName);

            Assert.AreEqual(1, readSoilProfiles.Count);
            Assert.AreEqual("Profile2", readSoilProfiles[0].Name);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_IntersectionXFor2DSoilProfileNull_ReturnOneProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dProfileWithXNull.soil");

            var result = new Collection<SoilProfile2D>();
            using (var reader = new SoilProfile2DReader(dbFile))
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

            SoilProfile2D soilProfile2D = result[0];
            Assert.AreEqual(1, soilProfile2D.Id);
            Assert.AreEqual("Profile", soilProfile2D.Name);
            Assert.IsNaN(soilProfile2D.IntersectionX);
            Assert.AreEqual(3, soilProfile2D.Layers.Count());

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenDatabaseWithSoilProfileAndSoilLayers_WhenReadingSoilLayerFails_ThenThrowsSoilProfileReadExceptionAndCanContinueReading()
        {
            // Given
            string dbFile = Path.Combine(testDataPath, "2dProfilesWithSoilLayersUnparsableValues.soil");

            var readProfiles = new List<SoilProfile2D>();
            SoilProfileReadException actualException = null;
            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                while (reader.HasNext)
                {
                    // When
                    try
                    {
                        readProfiles.Add(reader.ReadSoilProfile());
                    }
                    catch (SoilProfileReadException e)
                    {
                        actualException = e;
                    }
                }

                // Then
                Assert.IsFalse(reader.HasNext);
                Assert.AreEqual(2, readProfiles.Count);
            }

            Assert.IsNotNull(actualException);
            Assert.AreEqual("InvalidProfile", actualException.ProfileName);

            CollectionAssert.AreEqual(new[]
            {
                "Profile_1",
                "Profile_2"
            }, readProfiles.Select(profile => profile.Name));
            CollectionAssert.AreEqual(new[]
            {
                3,
                3
            }, readProfiles.Select(profile => profile.Layers.Count()));
            CollectionAssert.AreEqual(new[]
            {
                0,
                0
            }, readProfiles.Select(profile => profile.PreconsolidationStresses.Count()));
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_SoilProfilesWithAndWithoutPreconsolidationStresses_ReturnsProfilesWithExpectedNrOfPreconsolidationStresses()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dProfilesWithAndWithoutPreconsolidationStresses.soil");

            var readProfiles = new List<SoilProfile2D>();
            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                while (reader.HasNext)
                {
                    // Call
                    readProfiles.Add(reader.ReadSoilProfile());
                }

                // Assert
                Assert.IsFalse(reader.HasNext);
                Assert.AreEqual(3, readProfiles.Count);
            }

            CollectionAssert.AreEqual(new[]
            {
                1,
                0,
                1
            }, readProfiles.Select(profile => profile.PreconsolidationStresses.Count()));

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenDatabaseWithSoilProfilesAndPreconsolidationStresses_WhenReadingPreconsolidationStressesFails_ThenThrowsSoilProfileReadExceptionAndCanContinueReading()
        {
            // Given
            string dbFile = Path.Combine(testDataPath, "2dProfilesWithAndWithoutPreconsolidationStressesUnparsableValues.soil");

            var readProfiles = new List<SoilProfile2D>();
            SoilProfileReadException expectedException = null;
            using (var reader = new SoilProfile2DReader(dbFile))
            {
                reader.Initialize();

                while (reader.HasNext)
                {
                    // When
                    try
                    {
                        readProfiles.Add(reader.ReadSoilProfile());
                    }
                    catch (SoilProfileReadException e)
                    {
                        expectedException = e;
                    }
                }

                // Then
                Assert.IsFalse(reader.HasNext);
                Assert.AreEqual(2, readProfiles.Count);
            }

            Assert.IsNotNull(expectedException);
            Assert.AreEqual("InvalidPreconsolidationStress1", expectedException.ProfileName);

            CollectionAssert.AreEqual(new[]
            {
                "Profile_1",
                "Profile_2"
            }, readProfiles.Select(profile => profile.Name));
            CollectionAssert.AreEqual(new[]
            {
                3,
                3
            }, readProfiles.Select(profile => profile.Layers.Count()));
            CollectionAssert.AreEqual(new[]
            {
                1,
                1
            }, readProfiles.Select(profile => profile.PreconsolidationStresses.Count()));

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}
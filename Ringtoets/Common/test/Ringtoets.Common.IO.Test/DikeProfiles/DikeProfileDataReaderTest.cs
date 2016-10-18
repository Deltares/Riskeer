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
using System.IO;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.DikeProfiles;

namespace Ringtoets.Common.IO.Test.DikeProfiles
{
    [TestFixture]
    public class DikeProfileDataReaderTest
    {
        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void ReadDikeProfileData_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Setup
            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLine_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "profiel001 - Ringtoets.prfl"));
            string invalidFilePath = validFilePath.Replace("-", invalidFileNameChars[3].ToString());

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, string.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLine_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet verwijzen naar een lege bestandsnaam.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLine_ShapefileDoesntExist_ThrowCriticalFileReadException()
        {
            string expectedMessage = "Het bestand bestaat niet.";
            ReadFileAndExpectCriticalFileReadException("I_do_not_exist.shp", expectedMessage);
        }

        [Test]
        [TestCase("profiel001 - Ringtoets.prfl")]
        [TestCase("profiel001 - Ringtoets_WithWhiteSpaceAfterValues.prfl")]
        public void ReadDikeProfileData_ValidFilePath1_ReturnDikeProfileData(
            string validFileName)
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", validFileName));

            var reader = new DikeProfileDataReader();

            // Call
            DikeProfileData result = reader.ReadDikeProfileData(validFilePath);

            // Assert
            Assert.AreEqual("profiel001", result.Id);
            Assert.AreEqual(330.0, result.Orientation);
            Assert.AreEqual(DamType.None, result.DamType);
            Assert.AreEqual(SheetPileType.Coordinates, result.SheetPileType);
            Assert.AreEqual(0.0, result.DamHeight);
            CollectionAssert.IsEmpty(result.ForeshoreGeometry);
            Assert.AreEqual(6.0, result.DikeHeight);
            Assert.AreEqual(2, result.DikeGeometry.Length);
            Assert.AreEqual(new Point2D(0.0, 0.0), result.DikeGeometry[0].Point);
            Assert.AreEqual(1.0, result.DikeGeometry[0].Roughness.Value);
            Assert.AreEqual(new Point2D(18.0, 6.0), result.DikeGeometry[1].Point);
            Assert.AreEqual(1.0, result.DikeGeometry[1].Roughness.Value);
            var expectedMemo =
                "Verkenning prfl format:" + Environment.NewLine +
                "Basis:" + Environment.NewLine +
                "geen dam" + Environment.NewLine +
                "geen voorland" + Environment.NewLine +
                "recht talud" + Environment.NewLine;
            Assert.AreEqual(expectedMemo, result.Memo);
        }

        [Test]
        public void ReadDikeProfileData_ValidFilePath2_ReturnDikeProfileData()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "profiel004 - Ringtoets.prfl"));

            var reader = new DikeProfileDataReader();

            // Call
            DikeProfileData result = reader.ReadDikeProfileData(validFilePath);

            // Assert
            Assert.AreEqual("profiel004", result.Id);
            Assert.AreEqual(330.0, result.Orientation);
            Assert.AreEqual(DamType.None, result.DamType);
            Assert.AreEqual(SheetPileType.Coordinates, result.SheetPileType);
            Assert.AreEqual(0.5, result.DamHeight);
            Assert.AreEqual(3, result.ForeshoreGeometry.Length);
            Assert.AreEqual(new Point2D(-150.0, -9.0), result.ForeshoreGeometry[0].Point);
            Assert.AreEqual(1.0, result.ForeshoreGeometry[0].Roughness.Value);
            Assert.AreEqual(new Point2D(-100.0, -6.0), result.ForeshoreGeometry[1].Point);
            Assert.AreEqual(1.0, result.ForeshoreGeometry[1].Roughness.Value);
            Assert.AreEqual(new Point2D(-18.0, -6.0), result.ForeshoreGeometry[2].Point);
            Assert.AreEqual(1.0, result.ForeshoreGeometry[2].Roughness.Value);

            Assert.AreEqual(6.0, result.DikeHeight);
            Assert.AreEqual(4, result.DikeGeometry.Length);
            Assert.AreEqual(new Point2D(-18.0, -6.0), result.DikeGeometry[0].Point);
            Assert.AreEqual(1.0, result.DikeGeometry[0].Roughness.Value);
            Assert.AreEqual(new Point2D(-2.0, -0.1), result.DikeGeometry[1].Point);
            Assert.AreEqual(0.5, result.DikeGeometry[1].Roughness.Value);
            Assert.AreEqual(new Point2D(2.0, 0.1), result.DikeGeometry[2].Point);
            Assert.AreEqual(1.0, result.DikeGeometry[2].Roughness.Value);
            Assert.AreEqual(new Point2D(18.0, 6.0), result.DikeGeometry[3].Point);
            Assert.AreEqual(1.0, result.DikeGeometry[3].Roughness.Value);
            var expectedMemo =
                "Verkenning prfl format:" + Environment.NewLine +
                "geen dam" + Environment.NewLine +
                "voorland" + Environment.NewLine +
                "talud met (ruwe) berm" + Environment.NewLine;
            Assert.AreEqual(expectedMemo, result.Memo);
        }

        [Test]
        public void ReadDikeProfileData_ValidFilePath3_ReturnDikeProfileData()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "profiel001_DifferentDamAndDamwand.prfl"));

            var reader = new DikeProfileDataReader();

            // Call
            DikeProfileData result = reader.ReadDikeProfileData(validFilePath);

            // Assert
            Assert.AreEqual("profiel001", result.Id);
            Assert.AreEqual(330.0, result.Orientation);
            Assert.AreEqual(DamType.HarborDam, result.DamType);
            Assert.AreEqual(SheetPileType.SheetPileWithNoseConstruction, result.SheetPileType);
            Assert.AreEqual(0.0, result.DamHeight);
            CollectionAssert.IsEmpty(result.ForeshoreGeometry);
            Assert.AreEqual(6.0, result.DikeHeight);
            Assert.AreEqual(2, result.DikeGeometry.Length);
            Assert.AreEqual(new Point2D(0.0, 0.0), result.DikeGeometry[0].Point);
            Assert.AreEqual(1.0, result.DikeGeometry[0].Roughness.Value);
            Assert.AreEqual(new Point2D(18.0, 6.0), result.DikeGeometry[1].Point);
            Assert.AreEqual(1.0, result.DikeGeometry[1].Roughness.Value);
            var expectedMemo =
                "Verkenning prfl format:" + Environment.NewLine +
                "Basis:" + Environment.NewLine +
                "geen dam" + Environment.NewLine +
                "geen voorland" + Environment.NewLine +
                "recht talud" + Environment.NewLine;
            Assert.AreEqual(expectedMemo, result.Memo);
        }

        [Test]
        public void ReadDikeProfileData_ValidFilePath4_ReturnDikeProfileData()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "fileWithEmptyDikeAndForeshore.prfl"));

            var reader = new DikeProfileDataReader();

            // Call
            DikeProfileData result = reader.ReadDikeProfileData(validFilePath);

            // Assert
            Assert.AreEqual("ikBenBestWaardeloos", result.Id);
            Assert.AreEqual(123.456, result.Orientation);
            Assert.AreEqual(DamType.None, result.DamType);
            Assert.AreEqual(SheetPileType.Coordinates, result.SheetPileType);
            Assert.AreEqual(0.0, result.DamHeight);
            CollectionAssert.IsEmpty(result.ForeshoreGeometry);
            Assert.AreEqual(6.0, result.DikeHeight);
            CollectionAssert.IsEmpty(result.DikeGeometry);
            var expectedMemo =
                "Verkenning prfl format:" + Environment.NewLine +
                "Basis:" + Environment.NewLine +
                "geen dam" + Environment.NewLine +
                "geen voorland" + Environment.NewLine +
                "geen dijk" + Environment.NewLine +
                "recht talud" + Environment.NewLine;
            Assert.AreEqual(expectedMemo, result.Memo);
        }

        [Test]
        [TestCase("faulty_noId.prfl", "ID")]
        [TestCase("faulty_emptyFile.prfl", "VERSIE, ID, RICHTING, DAM, DAMHOOGTE, VOORLAND, DAMWAND, KRUINHOOGTE, DIJK, MEMO")]
        [TestCase("faulty_noDam.prfl", "DAM")]
        [TestCase("faulty_noDamHoogte.prfl", "DAMHOOGTE")]
        [TestCase("faulty_noDamWand.prfl", "DAMWAND")]
        [TestCase("faulty_noDijk.prfl", "DIJK")]
        [TestCase("faulty_noKruinHoogte.prfl", "KRUINHOOGTE")]
        [TestCase("faulty_NoMemo.prfl", "MEMO")]
        [TestCase("faulty_noRichting.prfl", "RICHTING")]
        [TestCase("faulty_noVersie.prfl", "VERSIE")]
        [TestCase("faulty_noVoorland.prfl", "VOORLAND")]
        public void ReadDikeProfileData_FilesWithMissingParameters_ThrowCriticalFileReadException(
            string faultyFileName, string missingParameterNames)
        {
            string expectedMessage = string.Format("De volgende parameters zijn niet aanwezig in het bestand: {0}",
                                                   missingParameterNames);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, expectedMessage);
        }

        [Test]
        [TestCase("faulty_incorrectVersion1.prfl")]
        [TestCase("faulty_incorrectVersion2.prfl")]
        public void ReadDikeProfileData_FileWithUnsupportedVersion_ThrowCriticalFileReadException(
            string faultyFileName)
        {
            string expectedMessage = @"Enkel bestanden van versie '4.0' worden ondersteund.";
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 1, expectedMessage);
        }

        [Test]
        [TestCase("faulty_richtingTooBig.prfl", 360.5)]
        [TestCase("faulty_richtingTooSmall.prfl", -12.36)]
        public void ReadDikeProfileData_FileWithOrientationOutOfRange_ThrowCriticalFileReadException(
            string faultyFileName, double expectedOrientationInFile)
        {
            string expectedMessage = string.Format("De oriëntatie ('{0}') moet in het bereik [0, 360] liggen.",
                                                   expectedOrientationInFile);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 4, expectedMessage);
        }

        [Test]
        [TestCase("faulty_damTooBig.prfl", 4)]
        [TestCase("faulty_damTooSmall.prfl", -1)]
        public void ReadDikeProfileData_FileWithDamTypeOutOfRange_ThrowCriticalFileReadException(
            string faultyFileName, int expectedDamInFile)
        {
            string expectedMessage = string.Format("Het ingelezen damtype ('{0}') moet 0, 1, 2 of 3 zijn.",
                                                   expectedDamInFile);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 6, expectedMessage);
        }

        [Test]
        [TestCase("faulty_damwandTooBig.prfl", 3)]
        [TestCase("faulty_damwandTooSmall.prfl", -1)]
        public void ReadDikeProfileData_FileWithProfileTypeOutOfRange_ThrowCriticalFileReadException(
            string faultyFileName, int expectedDamInFile)
        {
            string expectedMessage = string.Format("Het ingelezen damwandtype ('{0}') moet '0', '1' of '2' zijn.",
                                                   expectedDamInFile);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 11, expectedMessage);
        }

        [Test]
        public void ReadDikeProfileData_FileWithNegativeForeshorePointCount_ThrowCriticalFileReadException()
        {
            string expectedMessage = "Het aantal punten van de voorlandgeometrie ('-1') mag niet negatief zijn.";
            ReadFileAndExpectCriticalFileReadException("faulty_voorlandCountNegative.prfl", 9, expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("faulty_voorlandHasRoughnessTooBig.prfl", 1.234, 11)]
        [TestCase("faulty_dijkHasRoughnessTooBig.prfl", 1.321, 15)]
        public void ReadDikeProfileData_FileWithRoughnessOutOfRange_ThrowCriticalFileReadException_nl_NL(
            string faultyFileName, double expectedFaultyRoughness, int expectedLineNumber)
        {
            ReadDikeProfileData_FileWithRoughnessOutOfRange_ThrowsCriticalFileReadException(
                faultyFileName, expectedFaultyRoughness, expectedLineNumber, "0,5");
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase("faulty_voorlandHasRoughnessTooSmall.prfl", -0.943, 10)]
        [TestCase("faulty_dijkHasRoughnessTooSmall.prfl", -0.123, 14)]
        public void ReadDikeProfileData_FileWithRoughnessOutOfRange_ThrowCriticalFileReadException_en_US(
            string faultyFileName, double expectedFaultyRoughness, int expectedLineNumber)
        {
            ReadDikeProfileData_FileWithRoughnessOutOfRange_ThrowsCriticalFileReadException(
                faultyFileName, expectedFaultyRoughness, expectedLineNumber, "0.5");
        }

        [Test]
        [TestCase("faulty_unparsableVersie.prfl", "syudrj    iowydlklk")]
        [TestCase("faulty_unparsableVersie_noValue1.prfl", "")]
        [TestCase("faulty_unparsableVersie_noValue2.prfl", "")]
        [TestCase("faulty_unparsableVersie_IncorrectCharacter.prfl", "4.q")]
        [TestCase("faulty_unparsableVersie_NegativeNumber.prfl", "-4.-0")]
        public void ReadDikeProfileData_FileWithUnparsableVersion_ThrowCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = @"Enkel bestanden van versie '4.0' worden ondersteund.";
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 1, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableId_noValue1.prfl", "")]
        [TestCase("faulty_unparsableId_noValue2.prfl", "")]
        public void ReadDikeProfileData_FileWithUnparsableId_ThrowCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("De ingelezen ID ('{0}') is ongeldig.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 2, expectedMessage);
        }

        [Test]
        public void ReadDikeProfileData_FileWithInvalidId_ThrowCriticalFileReadException()
        {
            string expectedMessage = @"De ingelezen ID ('Id's are not allowed to have any white spaces!') bevat spaties. Spaties zijn niet toegestaan.";
            ReadFileAndExpectCriticalFileReadException("faulty_invalidId.prfl", 2, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableVersie_Overflow.prfl", "44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.0")]
        public void ReadDikeProfileData_FileWithOverflowVersion_ThrowCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = @"Enkel bestanden van versie '4.0' worden ondersteund.";
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 1, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableRichting.prfl", "d;apwiorqu  ihk dfh")]
        [TestCase("faulty_unparsableRichting_noValue1.prfl", "")]
        [TestCase("faulty_unparsableRichting_noValue2.prfl", "")]
        public void ReadDikeProfileData_FileWithUnparsableOrientation_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("De ingelezen oriëntatie ('{0}') is geen getal.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 4, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableRichting_Overflow1.prfl", "-22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222.2")]
        [TestCase("faulty_unparsableRichting_Overflow2.prfl", "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222.2")]
        public void ReadDikeProfileData_FileWithOverflowOrientation_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("De oriëntatie ('{0}') is te groot of te klein om ingelezen te worden.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 4, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDam.prfl", "309845poevotiuwe985v le09b 38- 35thp9 -")]
        [TestCase("faulty_unparsableDam_noValue1.prfl", "")]
        [TestCase("faulty_unparsableDam_noValue2.prfl", "")]
        public void ReadDikeProfileData_FileWithUnparsableDamType_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("Het ingelezen damtype ('{0}') moet 0, 1, 2 of 3 zijn.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 6, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDam_Overflow1.prfl", "-55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555")]
        [TestCase("faulty_unparsableDam_Overflow2.prfl", "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222")]
        public void ReadDikeProfileData_FileWithOverflowDamType_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("Het ingelezen damtype ('{0}') moet 0, 1, 2 of 3 zijn.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 6, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDamwand.prfl", "0v9 5y8w o8p 38uy-9863")]
        [TestCase("faulty_unparsableDamwand_noValue1.prfl", "")]
        [TestCase("faulty_unparsableDamwand_noValue2.prfl", "")]
        public void ReadDikeProfileData_FileWithUnparsableProfileType_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("Het ingelezen damwandtype ('{0}') moet '0', '1' of '2' zijn.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 11, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDamwand_Overflow1.prfl", "-55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555")]
        [TestCase("faulty_unparsableDamwand_Overflow2.prfl", "33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        public void ReadDikeProfileData_FileWithOverflowProfileType_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("Het ingelezen damwandtype ('{0}') moet '0', '1' of '2' zijn.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 11, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDamhoogte.prfl", "-0 6u498y4")]
        [TestCase("faulty_unparsableDamhoogte_noValue1.prfl", "")]
        [TestCase("faulty_unparsableDamhoogte_noValue2.prfl", "")]
        public void ReadDikeProfileData_FileWithUnparsableDamHeight_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("De ingelezen damhoogte ('{0}') is geen getal.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 7, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDamhoogte_Overflow1.prfl", "-11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        [TestCase("faulty_unparsableDamhoogte_Overflow2.prfl", "33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        public void ReadDikeProfileData_FileWithOverflowDamHeight_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("De ingelezen damhoogte ('{0}') is te groot of te klein om ingelezen te worden.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 7, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableKruinhoogte.prfl", "- 8ykultow9yowl;i 3-9854")]
        [TestCase("faulty_unparsableKruinhoogte_noValue1.prfl", "")]
        [TestCase("faulty_unparsableKruinhoogte_noValue2.prfl", "")]
        public void ReadDikeProfileData_FileWithUnparsableDikeHeight_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("De ingelezen dijkhoogte ('{0}') is geen getal.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 12, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableKruinhoogte_Overflow1.prfl", "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        [TestCase("faulty_unparsableKruinhoogte_Overflow2.prfl", "-33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        public void ReadDikeProfileData_FileWithOverflowDikeHeight_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("De ingelezen dijkhoogte ('{0}') is te groot of te klein om ingelezen te worden.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 12, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDijk.prfl", "069xf837 uo uyhtwuht098y hb3loiu43597")]
        [TestCase("faulty_unparsableDijk_noValue1.prfl", "")]
        [TestCase("faulty_unparsableDijk_noValue2.prfl", "")]
        public void ReadDikeProfileData_FileWithUnparsableDikeCount_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("Het aantal punten van de dijkgeometrie ('{0}') moet worden gespecificeerd door middel van een geheel getal.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 16, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDijk_Overflow1.prfl", "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        [TestCase("faulty_unparsableDijk_Overflow2.prfl", "-22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222")]
        public void ReadDikeProfileData_FileWithOverflowDijkCount_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("Het gespecificeerde aantal punten van de dijkgeometrie ('{0}') is te groot of te klein om ingelezen te worden.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 16, expectedMessage);
        }

        [Test]
        public void ReadDikeFileName_FileWithNegativeDikeCount_ThrowsCriticalFileReadException()
        {
            string expectedMessage = "Het aantal punten van de dijkgeometrie ('-1') mag niet negatief zijn.";
            ReadFileAndExpectCriticalFileReadException("faulty_dijkCountNegative.prfl", 13, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDijk_X.prfl", 18, "X-coördinaat", "suhf")]
        [TestCase("faulty_unparsableDijk_Z.prfl", 20, "Z-coördinaat", "o;jfhe;lhtvwposiu")]
        [TestCase("faulty_unparsableDijk_Roughness.prfl", 17, "ruwheid", "dr;tjn")]
        [TestCase("faulty_unparsableVoorland_X.prfl", 10, "X-coördinaat", "glkjdhflgkjhsk")]
        [TestCase("faulty_unparsableVoorland_Z.prfl", 12, "Z-coördinaat", "lijfhsliufghkj")]
        [TestCase("faulty_unparsableVoorland_Roughness.prfl", 10, "ruwheid", ";lsduglkwab")]
        public void ReadDikeProfileData_UnparsableRoughnessPoints_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, string expectedParameterName, string expectedReadText)
        {
            string expectedMessage = string.Format("De ingelezen {0} ('{1}') is geen getal.",
                                                   expectedParameterName, expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, expectedLineNumber, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDijk_Z_noValue1.prfl", 20, "18.000\t\t\t")]
        [TestCase("faulty_unparsableDijk_Z_noValue2.prfl", 20, "18.000")]
        [TestCase("faulty_unparsableDijk_Roughness_noValue1.prfl", 17, "-18.000\t-6.000\t\t\t")]
        [TestCase("faulty_unparsableDijk_Roughness_noValue2.prfl", 17, "-18.000\t-6.000")]
        [TestCase("faulty_unparsableDijk_tooManyValues.prfl", 14, "0.000\t0.000\t1.000\t12.34")]
        [TestCase("faulty_unparsableVoorland_X_noValue1.prfl", 11, "\t\t\t\t\t")]
        [TestCase("faulty_unparsableVoorland_X_noValue2.prfl", 12, "")]
        [TestCase("faulty_unparsableVoorland_Z_noValue1.prfl", 10, "-150.000\t\t\t\t")]
        [TestCase("faulty_unparsableVoorland_Z_noValue2.prfl", 11, "-100.000")]
        [TestCase("faulty_unparsableVoorland_Roughness_noValue1.prfl", 11, "-100.000\t-6.000\t\t\t\t")]
        [TestCase("faulty_unparsableVoorland_Roughness_noValue2.prfl", 12, "-18.000\t-6.000")]
        [TestCase("faulty_unparsableVoorland_tooManyValues.prfl", 11, "18.000\t6.000\t1.000\t985.345")]
        public void ReadDikeProfileData_NoRoughnessPointDefinition_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, string expectedReadText)
        {
            string expectedMessage = string.Format("De ingelezen regel ('{0}') is geen 'X Y ruwheid' definitie.", expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, expectedLineNumber, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDijk_X_Overflow1.prfl", 18, "X-coördinaat", "99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999")]
        [TestCase("faulty_unparsableDijk_X_Overflow2.prfl", 18, "X-coördinaat", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444")]
        [TestCase("faulty_unparsableDijk_Z_Overflow1.prfl", 19, "Z-coördinaat", "88888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888.8")]
        [TestCase("faulty_unparsableDijk_Z_Overflow2.prfl", 20, "Z-coördinaat", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.4")]
        [TestCase("faulty_unparsableDijk_Roughness_Overflow1.prfl", 17, "ruwheid", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.4")]
        [TestCase("faulty_unparsableDijk_Roughness_Overflow2.prfl", 17, "ruwheid", "88888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888.8")]
        [TestCase("faulty_unparsableVoorland_X_Overflow1.prfl", 11, "X-coördinaat", "55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555")]
        [TestCase("faulty_unparsableVoorland_X_Overflow2.prfl", 12, "X-coördinaat", "-33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        [TestCase("faulty_unparsableVoorland_Z_Overflow1.prfl", 11, "Z-coördinaat", "77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777.7")]
        [TestCase("faulty_unparsableVoorland_Z_Overflow2.prfl", 10, "Z-coördinaat", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.4")]
        [TestCase("faulty_unparsableVoorland_Roughness_Overflow1.prfl", 10, "ruwheid", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.4")]
        [TestCase("faulty_unparsableVoorland_Roughness_Overflow2.prfl", 11, "ruwheid", "-33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        public void ReadDikeProfileData_OverflowRoughnessPoints_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, string expectedParameterName, string expectedReadText)
        {
            string expectedMessage = string.Format("De in te lezen {0} ('{1}') is te groot of te klein om ingelezen te worden.",
                                                   expectedParameterName, expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, expectedLineNumber, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableDijk_missingElements.prfl", 19, 2, 4)]
        [TestCase("faulty_unparsableDijk_X_noValue1.prfl", 18, 1, 4)]
        [TestCase("faulty_unparsableDijk_X_noValue2.prfl", 18, 1, 4)]
        public void ReadDikeProfileData_FileWithMissingDikePoints_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, int actualCount, int expectedCount)
        {
            string expectedMessage = string.Format("Het aantal punten van de dijkgeometrie gevonden in het bestand '{0}' komt niet overeen met de daarin aangegeven hoeveelheid ('{1}').",
                                                   actualCount, expectedCount);
            ReadFileAndExpectCriticalFileReadException(faultyFileName,
                                                       expectedLineNumber, expectedMessage);
        }

        [Test]
        [TestCase("faulty_dijkNotMonotonicallyIncreasingX_1.prfl", 20, "dijk")]
        [TestCase("faulty_dijkNotMonotonicallyIncreasingX_2.prfl", 18, "dijk")]
        [TestCase("faulty_voorlandNotMonotonicallyIncreasingX_1.prfl", 12, "voorland")]
        [TestCase("faulty_voorlandNotMonotonicallyIncreasingX_2.prfl", 11, "voorland")]
        public void ReadDikeProfileData_IncorrectOrderingX_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, string expectedTypePrefix)
        {
            string expectedMessage = string.Format("De X-coördinaten van de {0}geometriepunten moeten strikt toenemend zijn.",
                                                   expectedTypePrefix);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, expectedLineNumber, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableVoorland.prfl", "oidushp9t8w uyp394hp 983 94")]
        [TestCase("faulty_unparsableVoorland_noValue1.prfl", "")]
        [TestCase("faulty_unparsableVoorland_noValue2.prfl", "")]
        public void ReadDikeProfileData_FileWithUnparsableForeshoreCount_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("Het aantal punten van de voorlandgeometrie ('{0}') moet worden gespecificeerd door middel van een geheel getal.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 9, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unparsableVoorland_Overflow1.prfl", "33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        [TestCase("faulty_unparsableVoorland_Overflow2.prfl", "-55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555")]
        public void ReadDikeProfileData_FileWithOverflowForeshoreCount_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            string expectedMessage = string.Format("Het gespecificeerde aantal punten van de voorlandgeometrie ('{0}') is te groot of te klein om ingelezen te worden.",
                                                   expectedReadText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, 9, expectedMessage);
        }

        [Test]
        public void ReadDikeProfileData_FileWithNegativeForeshoreCount_ThrowsCriticalFileReadException()
        {
            string expectedMessage = "Het aantal punten van de voorlandgeometrie ('-1') mag niet negatief zijn.";
            ReadFileAndExpectCriticalFileReadException("faulty_voorlandCountNegative.prfl",
                                                       9, expectedMessage);
        }

        [Test]
        public void ReadDikeProfileData_FileWithMissingForeshorePoints_ThrowsCriticalFileReadException()
        {
            string expectedMessage = "Het aantal punten van de voorlandgeometrie gevonden in het bestand '1' komt niet overeen met de daarin aangegeven hoeveelheid ('3').";
            ReadFileAndExpectCriticalFileReadException("faulty_unparsableVoorland_missingElements.prfl",
                                                       11, expectedMessage);
        }

        [Test]
        [TestCase("faulty_doubleVersie.prfl", 2, "VERSIE")]
        [TestCase("faulty_doubleId.prfl", 4, "ID")]
        [TestCase("faulty_doubleRichting.prfl", 7, "RICHTING")]
        [TestCase("faulty_doubleDam.prfl", 7, "DAM")]
        [TestCase("faulty_doubleDamhoogte.prfl", 10, "DAMHOOGTE")]
        [TestCase("faulty_doubleVoorland.prfl", 10, "VOORLAND")]
        [TestCase("faulty_doubleDamwand.prfl", 12, "DAMWAND")]
        [TestCase("faulty_doubleKruinhoogte.prfl", 14, "KRUINHOOGTE")]
        [TestCase("faulty_doubleDijk.prfl", 18, "DIJK")]
        public void ReadDikeProfileData_FileWithDoubleParameter_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, string expectedKeyword)
        {
            string expectedMessage = string.Format("De parameter {0} is al eerder in het bestand gedefinieerd.",
                                                   expectedKeyword);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, expectedLineNumber, expectedMessage);
        }

        [Test]
        [TestCase("faulty_unexpectedText1.prfl", 5, "Nobody expects the Spanish inquisition!")]
        [TestCase("faulty_unexpectedText2.prfl", 10, "0.000\t0.000\t1.000")]
        [TestCase("faulty_unexpectedText3.prfl", 11, "18.000\t6.000\t1.000")]
        [TestCase("faulty_unexpectedText4.prfl", 17, "-18.000\t-6.000\t1.000")]
        [TestCase("faulty_unexpectedText5.prfl", 20, "18.000\t6.000\t1.000")]
        [TestCase("faulty_unexpectedText6.prfl", 16, "haha hihi hoho hehe")]
        [TestCase("faulty_unexpectedText7.prfl", 13, "Niemand weet, wat repelsteeltje eet!")]
        public void ReadDikeProfileData_FileWithUnexpectedText_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, string expectedText)
        {
            string expectedMessage = string.Format("De regel ('{0}') bevat ongeldige tekst.",
                                                   expectedText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, expectedLineNumber, expectedMessage);
        }

        private void ReadFileAndExpectCriticalFileReadException(string fileName, int lineNumber, string errorMessage)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                               Path.Combine("DikeProfiles", fileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel {1}: {2}",
                                                   faultyFilePath, lineNumber, errorMessage);
            Assert.AreEqual(expectedMessage, message);
        }

        private void ReadFileAndExpectCriticalFileReadException(string fileName, string errorMessage)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                               Path.Combine("DikeProfiles", fileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   faultyFilePath, errorMessage);
            Assert.AreEqual(expectedMessage, message);
        }

        private void ReadDikeProfileData_FileWithRoughnessOutOfRange_ThrowsCriticalFileReadException(
            string faultyFileName, double expectedFaultyRoughness, int expectedLineNumber, string expectedLowerLimitText)
        {
            string expectedMessage = string.Format("De ingelezen ruwheid ('{0}') moet in het bereik [{1}, 1] liggen.",
                                                   expectedFaultyRoughness, expectedLowerLimitText);
            ReadFileAndExpectCriticalFileReadException(faultyFileName, expectedLineNumber, expectedMessage);
        }
    }
}
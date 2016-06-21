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

using Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.DikeProfiles
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

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                              Path.Combine("DikeProfiles", "profiel001 - Ringtoets.prfl"));
            string invalidFilePath = validFilePath.Replace("-", invalidFileNameChars[3].ToString());

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, String.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLine_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadReferenceLine_ShapefileDoesntExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                "I_do_not_exist.shp");

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                                                invalidFilePath);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("profiel001 - Ringtoets.prfl")]
        [TestCase("profiel001 - Ringtoets_WithWhiteSpaceAfterValues.prfl")]
        public void ReadDikeProfileData_ValidFilePath1_ReturnDikeProfileData(
            string validFileName)
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                              Path.Combine("DikeProfiles", validFileName));

            var reader = new DikeProfileDataReader();

            // Call
            DikeProfileData result = reader.ReadDikeProfileData(validFilePath);

            // Assert
            Assert.AreEqual("profiel001", result.Id);
            Assert.AreEqual(330.0, result.Orientation);
            Assert.AreEqual(DamType.None, result.DamType);
            Assert.AreEqual(ProfileType.Coordinates, result.ProfileType);
            Assert.AreEqual(0.0, result.DamHeight);
            CollectionAssert.IsEmpty(result.ForeshoreGeometry);
            Assert.AreEqual(6.0, result.CrestLevel);
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
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                              Path.Combine("DikeProfiles", "profiel004 - Ringtoets.prfl"));

            var reader = new DikeProfileDataReader();

            // Call
            DikeProfileData result = reader.ReadDikeProfileData(validFilePath);

            // Assert
            Assert.AreEqual("profiel004", result.Id);
            Assert.AreEqual(330.0, result.Orientation);
            Assert.AreEqual(DamType.None, result.DamType);
            Assert.AreEqual(ProfileType.Coordinates, result.ProfileType);
            Assert.AreEqual(0.5, result.DamHeight);
            Assert.AreEqual(3, result.ForeshoreGeometry.Length);
            Assert.AreEqual(new Point2D(-150.0, -9.0), result.ForeshoreGeometry[0].Point);
            Assert.AreEqual(1.0, result.ForeshoreGeometry[0].Roughness.Value);
            Assert.AreEqual(new Point2D(-100.0, -6.0), result.ForeshoreGeometry[1].Point);
            Assert.AreEqual(1.0, result.ForeshoreGeometry[1].Roughness.Value);
            Assert.AreEqual(new Point2D(-18.0, -6.0), result.ForeshoreGeometry[2].Point);
            Assert.AreEqual(1.0, result.ForeshoreGeometry[2].Roughness.Value);

            Assert.AreEqual(6.0, result.CrestLevel);
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
        [TestCase("faulry_noId.prfl", "ID")]
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
        public void ReadDikeProfileData_FaultyFilesWithMissingParameters_ThrowCriticalFileReadException(
            string faultyFileName, string missingParameterNames)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': De volgende parameter(s) zijn niet aanwezig in het bestand: {1}",
                                                   faultyFilePath, missingParameterNames);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_incorrectVersion1.prfl")]
        [TestCase("faulty_incorrectVersion2.prfl")]
        public void ReadDikeProfileData_FaultyFileWithUnsupportedVersion_ThrowCriticalFileReadException(
            string faultyFileName)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: Enkel bestanden van versie '4.0' worden ondersteund.",
                                                   faultyFilePath);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_richtingTooBig.prfl", 360.5)]
        [TestCase("faulty_richtingTooSmall.prfl", -12.36)]
        public void ReadDikeProfileData_FaultyFileWithOrientationOutOfRange_ThrowCriticalFileReadException(
            string faultyFileName, double expectedOrientationInFile)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 4: De ingelezen orientatie waarde ({1}) moet binnen het bereik [0, 360] vallen.",
                                                   faultyFilePath, expectedOrientationInFile);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_damTooBig.prfl", 4)]
        [TestCase("faulty_damTooSmall.prfl", -1)]
        public void ReadDikeProfileData_FaultyFileWithDamTypeOutOfRange_ThrowCriticalFileReadException(
            string faultyFileName, int expectedDamInFile)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 6: De ingelezen dam-type waarde ({1}) moet binnen het bereik [0, 3] vallen.",
                                                   faultyFilePath, expectedDamInFile);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_damwandTooBig.prfl", 3)]
        [TestCase("faulty_damwandTooSmall.prfl", -1)]
        public void ReadDikeProfileData_FaultyFileWithProfileTypeOutOfRange_ThrowCriticalFileReadException(
            string faultyFileName, int expectedDamInFile)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 11: De ingelezen damwand-type waarde ({1}) moet binnen het bereik [0, 2] vallen.",
                                                   faultyFilePath, expectedDamInFile);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ReadDikeProfileData_FaultyFileWithNegativeForshorePointCount_ThrowCriticalFileReadException()
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", "faulty_voorlandCountNegative.prfl"));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 9: Het ingelezen aantal voorland punten (-1) mag niet negatief zijn.",
                                                   faultyFilePath);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("faulty_voorlandHasRoughnessTooBig.prfl", 1.234, 11)]
        [TestCase("faulty_dijkHasRoughnessTooBig.prfl", 1.321, 15)]
        public void ReadDikeProfileData_FaultyFileWithRoughnessOutOfRange_ThrowCriticalFileReadException_nl_NL(
            string faultyFileName, double expectedFaultyRoughness, double expectedLineNumber)
        {
            DoReadDikeProfileData_FaultyFileWithRoughnessOutOfRange_ThrowsCriticalFileReadException(
                faultyFileName, expectedFaultyRoughness, expectedLineNumber, "0,5");
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase("faulty_voorlandHasRoughnessTooSmall.prfl", -0.943, 10)]
        [TestCase("faulty_dijkHasRoughnessTooSmall.prfl", -0.123, 14)]
        public void ReadDikeProfileData_FaultyFileWithRoughnessOutOfRange_ThrowCriticalFileReadException_en_US(
            string faultyFileName, double expectedFaultyRoughness, double expectedLineNumber)
        {
            DoReadDikeProfileData_FaultyFileWithRoughnessOutOfRange_ThrowsCriticalFileReadException(
                faultyFileName, expectedFaultyRoughness, expectedLineNumber, "0.5");
        }

        [Test]
        [TestCase("faulty_unparsableVersie.prfl", "syudrj    iowydlklk")]
        [TestCase("faulty_unparsableVersie_noValue1.prfl", "")]
        [TestCase("faulty_unparsableVersie_noValue2.prfl", "")]
        [TestCase("faulty_unparsableVersie_IncorrectCharacter.prfl", "4.q")]
        [TestCase("faulty_unparsableVersie_NegativeNumber.prfl", "-4.-0")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableVersion_ThrowCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: De ingelezen versie ({1}) is geen geldige versie code.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableId_noValue1.prfl", "")]
        [TestCase("faulty_unparsableId_noValue2.prfl", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableId_ThrowCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: De ingelezen Id ({1}) is geen geldig id.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ReadDikeProfileData_FaultyFileWithInvalidId_ThrowCriticalFileReadException()
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", "faulty_invalidId.prfl"));
            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: De ingelezen Id (Id's are not allowed to have any white space!) bevat spaties, welke niet zijn toegestaan.",
                                                   faultyFilePath);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableVersie_Overflow.prfl", "44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.0")]
        public void ReadDikeProfileData_FaultyFileWithOverflowVersion_ThrowCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: De ingelezen versie ({1}) bevat een versienummer die te groot is.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableRichting.prfl", "d;apwiorqu  ihk dfh")]
        [TestCase("faulty_unparsableRichting_noValue1.prfl", "")]
        [TestCase("faulty_unparsableRichting_noValue2.prfl", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableOrientation_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 4: De ingelezen orientatie ({1}) is geen getal.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableRichting_Overflow1.prfl", "-22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222.2")]
        [TestCase("faulty_unparsableRichting_Overflow2.prfl", "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222.2")]
        public void ReadDikeProfileData_FaultyFileWithOverflowOrientation_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 4: De ingelezen orientatie ({1}) is te groot of te klein om ingelezen te worden.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDam.prfl", "309845poevotiuwe985v le09b 38- 35thp9 -")]
        [TestCase("faulty_unparsableDam_noValue1.prfl", "")]
        [TestCase("faulty_unparsableDam_noValue2.prfl", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableDamType_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 6: Het ingelezen dam-type ({1}) is geen geheel getal.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDam_Overflow1.prfl", "-55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555")]
        [TestCase("faulty_unparsableDam_Overflow2.prfl", "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222")]
        public void ReadDikeProfileData_FaultyFileWithOverflowDamType_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 6: Het ingelezen dam-type ({1}) is te groot of te klein om ingelezen te worden.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDamwand.prfl", "0v9 5y8w o8p 38uy-9863")]
        [TestCase("faulty_unparsableDamwand_noValue1.prfl", "")]
        [TestCase("faulty_unparsableDamwand_noValue2.prfl", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableProfileType_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 11: Het ingelezen profiel-type ({1}) is geen geheel getal.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDamwand_Overflow1.prfl", "-55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555")]
        [TestCase("faulty_unparsableDamwand_Overflow2.prfl", "33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        public void ReadDikeProfileData_FaultyFileWithOverflowProfileType_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 11: Het ingelezen profiel-type ({1}) is te groot of te klein om ingelezen te worden.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDamhoogte.prfl", "-0 6u498y4")]
        [TestCase("faulty_unparsableDamhoogte_noValue1.prfl", "")]
        [TestCase("faulty_unparsableDamhoogte_noValue2.prfl", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableDamHeight_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 7: De ingelezen damhoogte ({1}) is geen getal.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDamhoogte_Overflow1.prfl", "-11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        [TestCase("faulty_unparsableDamhoogte_Overflow2.prfl", "33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        public void ReadDikeProfileData_FaultyFileWithOverflowDamHeight_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 7: De ingelezen damhoogte ({1}) is te groot of te klein om ingelezen te worden.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableKruinhoogte.prfl", "- 8ykultow9yowl;i 3-9854")]
        [TestCase("faulty_unparsableKruinhoogte_noValue1.prfl", "")]
        [TestCase("faulty_unparsableKruinhoogte_noValue2.prfl", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableCrestLevel_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 12: De ingelezen kruinhoogte ({1}) is geen getal.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableKruinhoogte_Overflow1.prfl", "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        [TestCase("faulty_unparsableKruinhoogte_Overflow2.prfl", "-33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        public void ReadDikeProfileData_FaultyFileWithOverflowCrestLevel_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 12: De ingelezen kruinhoogte ({1}) is te groot of te klein om ingelezen te worden.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDijk.prfl", "069xf837 uo uyhtwuht098y hb3loiu43597")]
        [TestCase("faulty_unparsableDijk_noValue1.prfl", "")]
        [TestCase("faulty_unparsableDijk_noValue2.prfl", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableDikeCount_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 16: Het ingelezen aantal dijk punten ({1}) is geen geheel getal.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDijk_Overflow1.prfl", "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        [TestCase("faulty_unparsableDijk_Overflow2.prfl", "-22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222")]
        public void ReadDikeProfileData_FaultyFileWithOverflowDijkCount_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 16: Het ingelezen aantal dijk punten ({1}) is te groot of te klein om ingelezen te worden.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDijk_X.prfl", 18, "X coordinaat", "suhf")]
        [TestCase("faulty_unparsableDijk_X_noValue1.prfl", 18, "X coordinaat", "")]
        [TestCase("faulty_unparsableDijk_X_noValue2.prfl", 18, "X coordinaat", "")]
        [TestCase("faulty_unparsableDijk_Y.prfl", 20, "Y coordinaat", "o;jfhe;lhtvwposiu")]
        [TestCase("faulty_unparsableDijk_Y_noValue1.prfl", 20, "Y coordinaat", "")]
        [TestCase("faulty_unparsableDijk_Y_noValue2.prfl", 20, "Y coordinaat", "")]
        [TestCase("faulty_unparsableDijk_Roughness.prfl", 17, "ruwheid", "dr;tjn")]
        [TestCase("faulty_unparsableDijk_Roughness_noValue1.prfl", 17, "ruwheid", "")]
        [TestCase("faulty_unparsableDijk_Roughness_noValue2.prfl", 17, "ruwheid", "")]
        [TestCase("faulty_unparsableVoorland_X.prfl", 10, "X coordinaat", "glkjdhflgkjhsk")]
        [TestCase("faulty_unparsableVoorland_X_noValue1.prfl", 11, "X coordinaat", "")]
        [TestCase("faulty_unparsableVoorland_X_noValue2.prfl", 12, "X coordinaat", "")]
        [TestCase("faulty_unparsableVoorland_Y.prfl", 12, "Y coordinaat", "lijfhsliufghkj")]
        [TestCase("faulty_unparsableVoorland_Y_noValue1.prfl", 10, "Y coordinaat", "")]
        [TestCase("faulty_unparsableVoorland_Y_noValue2.prfl", 11, "Y coordinaat", "")]
        [TestCase("faulty_unparsableVoorland_Roughness.prfl", 10, "ruwheid", ";lsduglk wab")]
        [TestCase("faulty_unparsableVoorland_Roughness_noValue1.prfl", 11, "ruwheid", "")]
        [TestCase("faulty_unparsableVoorland_Roughness_noValue2.prfl", 12, "ruwheid", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableRoughnessPoints_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, string expectedParameterName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel {1}: De ingelezen {2} ({3}) is geen getal.",
                                                   faultyFilePath, expectedLineNumber, expectedParameterName, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableDijk_X_Overflow1.prfl", 18, "X coordinaat", "99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999")]
        [TestCase("faulty_unparsableDijk_X_Overflow2.prfl", 18, "X coordinaat", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444")]
        [TestCase("faulty_unparsableDijk_Y_Overflow1.prfl", 19, "Y coordinaat", "88888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888.8")]
        [TestCase("faulty_unparsableDijk_Y_Overflow2.prfl", 20, "Y coordinaat", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.4")]
        [TestCase("faulty_unparsableDijk_Roughness_Overflow1.prfl", 17, "ruwheid", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.4")]
        [TestCase("faulty_unparsableDijk_Roughness_Overflow2.prfl", 17, "ruwheid", "88888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888.8")]
        [TestCase("faulty_unparsableVoorland_X_Overflow1.prfl", 11, "X coordinaat", "55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555")]
        [TestCase("faulty_unparsableVoorland_X_Overflow2.prfl", 12, "X coordinaat", "-33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        [TestCase("faulty_unparsableVoorland_Y_Overflow1.prfl", 11, "Y coordinaat", "77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777.7")]
        [TestCase("faulty_unparsableVoorland_Y_Overflow2.prfl", 10, "Y coordinaat", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.4")]
        [TestCase("faulty_unparsableVoorland_Roughness_Overflow1.prfl", 10, "ruwheid", "-44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444.4")]
        [TestCase("faulty_unparsableVoorland_Roughness_Overflow2.prfl", 11, "ruwheid", "-33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        public void ReadDikeProfileData_FaultyFileWithOverflowRoughnessPoints_ThrowsCriticalFileReadException(
            string faultyFileName, int expectedLineNumber, string expectedParameterName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel {1}: De ingelezen {2} ({3}) is te groot of te klein om ingelezen te worden.",
                                                   faultyFilePath, expectedLineNumber, expectedParameterName, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ReadDikeProfileData_FaultyFileWithMissingDikePoints_ThrowsCriticalFileReadException()
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", "faulty_unparsableDijk_missingElements.prfl"));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 19: Het aantal dijk punten in het bestand (2) komt niet overeen met de aangegeven hoeveelheid dijk punten (4).",
                                                   faultyFilePath);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableVoorland.prfl", "oidushp9t8w uyp394hp 983 94")]
        [TestCase("faulty_unparsableVoorland_noValue1.prfl", "")]
        [TestCase("faulty_unparsableVoorland_noValue2.prfl", "")]
        public void ReadDikeProfileData_FaultyFileWithUnparsableForeshoreCount_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 9: Het ingelezen aantal voorland punten ({1}) is geen geheel getal.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("faulty_unparsableVoorland_Overflow1.prfl", "33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333")]
        [TestCase("faulty_unparsableVoorland_Overflow2.prfl", "-55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555")]
        public void ReadDikeProfileData_FaultyFileWithOverflowForeshoreCount_ThrowsCriticalFileReadException(
            string faultyFileName, string expectedReadText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 9: Het ingelezen aantal voorland punten ({1}) is te groot of te klein om ingelezen te worden.",
                                                   faultyFilePath, expectedReadText);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ReadDikeProfileData_FaultyFileWithMissingForeshorePoints_ThrowsCriticalFileReadException()
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", "faulty_unparsableVoorland_missingElements.prfl"));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 11: Het aantal voorland punten in het bestand (1) komt niet overeen met de aangegeven hoeveelheid voorland punten (3).",
                                                   faultyFilePath);
            Assert.AreEqual(expectedMessage, message);
        }

        // TODO: DAMWAND en DAMWAND Type coverage (beide files hebben dezelfde waardes)

        private static void DoReadDikeProfileData_FaultyFileWithRoughnessOutOfRange_ThrowsCriticalFileReadException(
            string faultyFileName, double expectedFaultyRoughness, double expectedLineNumber, string expectedLowerLimitText)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                               Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel {1}: De ingelezen ruwheid ({2}) moet in het bereik [{3}, 1] vallen.",
                                                   faultyFilePath, expectedLineNumber, expectedFaultyRoughness, expectedLowerLimitText);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}
﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        public void ReadDikeProfileData_ValidFilePath1_ReturnDikeProfileData()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                              Path.Combine("DikeProfiles", "profiel001 - Ringtoets.prfl"));

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
            Assert.AreEqual(1, result.DikeGeometry.Length);
            Assert.AreEqual(new Point2D(0.0, 0.0), result.DikeGeometry[0].StartingPoint);
            Assert.AreEqual(new Point2D(18.0, 6.0), result.DikeGeometry[0].EndingPoint);
            Assert.AreEqual(1.0, result.DikeGeometry[0].Roughness);
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
            Assert.AreEqual(2, result.ForeshoreGeometry.Length);
            Assert.AreEqual(new Point2D(-150.0, -9.0), result.ForeshoreGeometry[0].StartingPoint);
            Assert.AreEqual(new Point2D(-100.0, -6.0), result.ForeshoreGeometry[0].EndingPoint);
            Assert.AreEqual(1.0, result.ForeshoreGeometry[0].Roughness);
            Assert.AreEqual(new Point2D(-100.0, -6.0), result.ForeshoreGeometry[1].StartingPoint);
            Assert.AreEqual(new Point2D(-18.0, -6.0), result.ForeshoreGeometry[1].EndingPoint);
            Assert.AreEqual(1.0, result.ForeshoreGeometry[1].Roughness);

            Assert.AreEqual(6.0, result.CrestLevel);
            Assert.AreEqual(3, result.DikeGeometry.Length);
            Assert.AreEqual(new Point2D(-18.0, -6.0), result.DikeGeometry[0].StartingPoint);
            Assert.AreEqual(new Point2D(-2.0, -0.1), result.DikeGeometry[0].EndingPoint);
            Assert.AreEqual(1.0, result.DikeGeometry[0].Roughness);
            Assert.AreEqual(new Point2D(-2.0, -0.1), result.DikeGeometry[1].StartingPoint);
            Assert.AreEqual(new Point2D(2.0, 0.1), result.DikeGeometry[1].EndingPoint);
            Assert.AreEqual(0.5, result.DikeGeometry[1].Roughness);
            Assert.AreEqual(new Point2D(2.0, 0.1), result.DikeGeometry[2].StartingPoint);
            Assert.AreEqual(new Point2D(18.0, 6.0), result.DikeGeometry[2].EndingPoint);
            Assert.AreEqual(1.0, result.DikeGeometry[2].Roughness);
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
        [TestCase("faulty_voorlandHasRoughnessTooBig.prfl", 1.234, 11)]
        [TestCase("faulty_voorlandHasRoughnessTooSmall.prfl", -0.943, 10)]
        [TestCase("faulty_dijkHasRoughnessTooSmall.prfl", -0.123, 14)]
        [TestCase("faulty_dijkHasRoughnessTooBig.prfl", 1.321, 15)]
        public void ReadDikeProfileData_FaultyFileWithRoughnessOutOfRange_ThrowCriticalFileReadException(
            string faultyFileName, double expectedFaultyRoughness, double expectedLineNumber)
        {
            // Setup
            string faultyFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                                Path.Combine("DikeProfiles", faultyFileName));

            var reader = new DikeProfileDataReader();

            // Call
            TestDelegate call = () => reader.ReadDikeProfileData(faultyFilePath);

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel {1}: De ingelezen ruwheid ({2}) moet in het bereik [0, 1] vallen.",
                                                   faultyFilePath, expectedLineNumber, expectedFaultyRoughness);
            Assert.AreEqual(expectedMessage, message);
        }
        
        // TODO: DAMWAND en DAMWAND Type coverage (beide files hebben dezelfde waardes)
    }
}
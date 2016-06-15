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
using Core.Common.TestUtil;

using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.DikeProfiles
{
    [TestFixture]
    public class DikeProfileDataReaderTest
    {
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
    }
}
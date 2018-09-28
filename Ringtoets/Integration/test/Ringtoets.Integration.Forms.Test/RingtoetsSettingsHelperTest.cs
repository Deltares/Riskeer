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
using System.IO;
using Core.Common.Util.Settings;
using NUnit.Framework;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class RingtoetsSettingsHelperTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var settingsHelper = new RingtoetsSettingsHelper();

            // Assert
            Assert.IsInstanceOf<SettingsHelper>(settingsHelper);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_WithoutParams_ReturnsExpectedDirectory()
        {
            // Setup
            var settingsHelper = new RingtoetsSettingsHelper();

            // Call
            string ringtoetsLocalApplicationDataPath = settingsHelper.GetApplicationLocalUserSettingsDirectory();

            // Assert
            string localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string expectedPath = Path.Combine(localApplicationDataPath, "WTI", "Ringtoets");
            Assert.AreEqual(expectedPath, ringtoetsLocalApplicationDataPath);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_WithParams_ReturnsExpectedDirectory()
        {
            // Setup
            var settingsHelper = new RingtoetsSettingsHelper();
            string localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string rootPath = Path.Combine(localApplicationDataPath, "WTI", "Ringtoets");

            // Call
            string ringtoetsLocalApplicationDataPath = settingsHelper.GetApplicationLocalUserSettingsDirectory("subFolder", "subSubFolder");

            // Assert
            string expectedPath = Path.Combine(rootPath, "subFolder", "subSubFolder");
            Assert.AreEqual(expectedPath, ringtoetsLocalApplicationDataPath);
        }

        [Test]
        public void GetCommonDocumentsRingtoetsDirectory_WithoutParams_ReturnsExpectedDirectory()
        {
            // Setup
            var settingsHelper = new RingtoetsSettingsHelper();

            // Call
            string pathFromSettings = settingsHelper.GetCommonDocumentsDirectory();

            // Assert
            string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "WTI", "Ringtoets");
            Assert.AreEqual(expectedPath, pathFromSettings);
        }

        [Test]
        public void GetCommonDocumentsRingtoetsDirectory_WithParams_ReturnsExpectedDirectory()
        {
            // Setup
            var settingsHelper = new RingtoetsSettingsHelper();

            // Call
            string pathFromSettings = settingsHelper.GetCommonDocumentsDirectory("some folder");

            // Assert
            string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "WTI", "Ringtoets", "some folder");
            Assert.AreEqual(expectedPath, pathFromSettings);
        }

        [Test]
        public void GetCommonDocumentsRingtoetsShapeFileDirectory_ReturnsExpectedDirectory()
        {
            // Setup
            string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "WTI", "Ringtoets", "NBPW");

            // Call
            string pathFromSettings = RingtoetsSettingsHelper.GetCommonDocumentsRingtoetsShapeFileDirectory();

            // Assert
            Assert.AreEqual(expectedPath, pathFromSettings);
        }
    }
}
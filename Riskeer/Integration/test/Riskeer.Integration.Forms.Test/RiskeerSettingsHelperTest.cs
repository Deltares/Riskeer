// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.IO;
using Core.Common.Util.Settings;
using NUnit.Framework;

namespace Riskeer.Integration.Forms.Test
{
    [TestFixture]
    public class RiskeerSettingsHelperTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var settingsHelper = new RiskeerSettingsHelper();

            // Assert
            Assert.IsInstanceOf<SettingsHelper>(settingsHelper);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_WithoutParams_ReturnsExpectedDirectory()
        {
            // Setup
            var settingsHelper = new RiskeerSettingsHelper();

            // Call
            string localUserSettingsDirectory = settingsHelper.GetApplicationLocalUserSettingsDirectory();

            // Assert
            string localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string expectedPath = Path.Combine(localApplicationDataPath, "BOI", "Riskeer");
            Assert.AreEqual(expectedPath, localUserSettingsDirectory);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_WithParams_ReturnsExpectedDirectory()
        {
            // Setup
            var settingsHelper = new RiskeerSettingsHelper();
            string localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string rootPath = Path.Combine(localApplicationDataPath, "BOI", "Riskeer");

            // Call
            string localUserSettingsDirectory = settingsHelper.GetApplicationLocalUserSettingsDirectory("subFolder", "subSubFolder");

            // Assert
            string expectedPath = Path.Combine(rootPath, "subFolder", "subSubFolder");
            Assert.AreEqual(expectedPath, localUserSettingsDirectory);
        }

        [Test]
        public void GetCommonDocumentsRiskeerDirectory_WithoutParams_ReturnsExpectedDirectory()
        {
            // Setup
            var settingsHelper = new RiskeerSettingsHelper();

            // Call
            string pathFromSettings = settingsHelper.GetCommonDocumentsDirectory();

            // Assert
            string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "BOI", "Riskeer");
            Assert.AreEqual(expectedPath, pathFromSettings);
        }

        [Test]
        public void GetCommonDocumentsRiskeerDirectory_WithParams_ReturnsExpectedDirectory()
        {
            // Setup
            var settingsHelper = new RiskeerSettingsHelper();

            // Call
            string pathFromSettings = settingsHelper.GetCommonDocumentsDirectory("some folder");

            // Assert
            string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "BOI", "Riskeer", "some folder");
            Assert.AreEqual(expectedPath, pathFromSettings);
        }

        [Test]
        public void GetCommonDocumentsRiskeerShapeFileDirectory_ReturnsExpectedDirectory()
        {
            // Setup
            string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "BOI", "Riskeer", "NBPW");

            // Call
            string pathFromSettings = RiskeerSettingsHelper.GetCommonDocumentsRiskeerShapeFileDirectory();

            // Assert
            Assert.AreEqual(expectedPath, pathFromSettings);
        }
    }
}
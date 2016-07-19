// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using Core.Common.Gui.Settings;
using Core.Common.Utils.Reflection;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Settings
{
    [TestFixture]
    public class SettingsHelperTest
    {
        [Test]
        public void ApplicationName_ReturnsProductNameOfExecutingAssembly()
        {
            // Call
            var settings = SettingsHelper.ApplicationName;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Product, settings);
        }

        [Test]
        public void ApplicationVersion_RetursnVersionOfExecutingAssembly()
        {
            // Call
            var settings = SettingsHelper.ApplicationVersion;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Version, settings);
        }

        [Test]
        public void ApplicationCompany_ReturnsCompanyOfExecutingAssembly()
        {
            // Call
            var settings = SettingsHelper.ApplicationCompany;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Company, settings);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_ReturnsApplicationLocalUserSettingsDirectory()
        {
            // Setup
            var localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var companySettingsDirectoryPath = Path.Combine(localSettingsDirectoryPath, SettingsHelper.ApplicationCompany);
            var appSettingsDirectoryPath = Path.Combine(companySettingsDirectoryPath, SettingsHelper.ApplicationName + " " + SettingsHelper.ApplicationVersion);

            // Call
            var pathFromSettings = SettingsHelper.GetApplicationLocalUserSettingsDirectory();

            // Assert
            Assert.AreEqual(appSettingsDirectoryPath, pathFromSettings);
        }

        [Test]
        public void GetCommonDocumentsDirectory_ReturnsCommonDocumentsDirectory()
        {
            // Setup
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "WTI", "NBPW");

            // Call
            var pathFromSettings = SettingsHelper.GetCommonDocumentsDirectory();

            // Assert
            Assert.AreEqual(path, pathFromSettings);
        }
    }
}
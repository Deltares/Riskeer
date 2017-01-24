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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using Migration.Core.Storage;
using Migration.Scripts.Data;
using NUnit.Framework;
using Ringtoets.Common.Utils;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class StorageMigrationIntegrationTest
    {
        private const string tempExtension = ".temp";
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage);

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithMigratedFile_MigratedProjectSet()
        {
            string targetFilePath = GetRandomRingtoetsFile();
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, "FullTestProject164.rtd");
            MigrateFile(sourceFilePath, targetFilePath);

            // Given
            var projectStore = new StorageSqLite();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                // When
                gui.Run(targetFilePath);

                // Then
                Assert.AreEqual(targetFilePath, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                string expectedProjectName = Path.GetFileNameWithoutExtension(targetFilePath);
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual("description", gui.Project.Description);
                Assert.IsInstanceOf<RingtoetsProject>(gui.Project);
            }
        }

        [OneTimeTearDown]
        public void TearDownTempRingtoetsFile()
        {
            IEnumerable<string> filesToDelete = Directory.EnumerateFiles(testDataPath, string.Concat("*", tempExtension));

            foreach (string fileToDelete in filesToDelete)
            {
                TearDownTempRingtoetsFile(fileToDelete);
            }
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        private static void TearDownTempRingtoetsFile(string filePath)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static string GetRandomRingtoetsFile()
        {
            return Path.Combine(testDataPath, string.Concat(Path.GetRandomFileName(), tempExtension));
        }

        private static void MigrateFile(string sourceFilePath, string targetFilePath)
        {
            string newVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            var fromVersionedFile = new VersionedFile(sourceFilePath);
            var migrator = new VersionedFileMigrator();

            migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);
        }
    }
}
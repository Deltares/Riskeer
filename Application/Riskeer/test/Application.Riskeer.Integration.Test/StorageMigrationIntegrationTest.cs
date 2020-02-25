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
using System.Threading;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Util;
using Riskeer.Integration.Data;
using Riskeer.Migration;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;
using Riskeer.Storage.Core;

namespace Application.Riskeer.Integration.Test
{
    [TestFixture]
    public class StorageMigrationIntegrationTest
    {
        private readonly string workingDirectory = TestHelper.GetScratchPadPath(nameof(StorageMigrationIntegrationTest));
        private DirectoryDisposeHelper directoryDisposeHelper;

        [Test]
        [TestCaseSource(typeof(ProjectMigrationTestHelper), nameof(ProjectMigrationTestHelper.GetAllOutdatedSupportedProjectFileNames))]
        [Apartment(ApartmentState.STA)]
        public void GivenRiskeerGuiWithStorageSql_WhenRunWithMigratedFile_MigratedProjectSet(string version)
        {
            string sourceFilePath = GetTestProjectFilePath(version);
            string targetFilePath = Path.Combine(workingDirectory, nameof(GivenRiskeerGuiWithStorageSql_WhenRunWithMigratedFile_MigratedProjectSet));
            MigrateFile(sourceFilePath, targetFilePath);

            // Given
            var projectStore = new StorageSqLite();
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            var projectMigrator = new ProjectMigrator(inquiryHelper);

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(), new GuiCoreSettings()))
            {
                // When
                gui.Run(targetFilePath);

                // Then
                Assert.AreEqual(targetFilePath, gui.ProjectFilePath);
                Assert.NotNull(gui.Project);
                string expectedProjectName = Path.GetFileNameWithoutExtension(targetFilePath);
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual("description", gui.Project.Description);
                Assert.IsInstanceOf<RiskeerProject>(gui.Project);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(ProjectMigrationTestHelper), nameof(ProjectMigrationTestHelper.GetAllOutdatedSupportedProjectFileNames))]
        [Apartment(ApartmentState.STA)]
        public void GivenRiskeerGui_WhenRunWithUnmigratedFileAndInquireContinuation_MigratedProjectSet(string version)
        {
            // Given
            string sourceFilePath = GetTestProjectFilePath(version);
            string targetFilePath = Path.Combine(workingDirectory, nameof(GivenRiskeerGui_WhenRunWithUnmigratedFileAndInquireContinuation_MigratedProjectSet));

            var projectStore = new StorageSqLite();
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(true);
            inquiryHelper.Expect(helper => helper.GetTargetFileLocation(null, null))
                         .IgnoreArguments()
                         .Return(targetFilePath);
            mocks.ReplayAll();

            var projectMigrator = new ProjectMigrator(inquiryHelper);

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(), new GuiCoreSettings()))
            {
                // When
                gui.Run(sourceFilePath);

                // Then
                Assert.AreEqual(targetFilePath, gui.ProjectFilePath);
                string expectedProjectName = Path.GetFileNameWithoutExtension(targetFilePath);
                Assert.AreEqual(expectedProjectName, gui.Project.Name);
                Assert.AreEqual("description", gui.Project.Description);
                Assert.IsInstanceOf<RiskeerProject>(gui.Project);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(ProjectMigrationTestHelper), nameof(ProjectMigrationTestHelper.GetAllOutdatedSupportedProjectFileNames))]
        [Apartment(ApartmentState.STA)]
        public void GivenRiskeerGui_WhenRunWithUnmigratedFileAndNoInquireContinuation_MigratedProjectNotSet(string version)
        {
            // Given
            string sourceFilePath = GetTestProjectFilePath(version);
            var projectStore = new StorageSqLite();
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(false);
            mocks.ReplayAll();

            var projectMigrator = new ProjectMigrator(inquiryHelper);

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(), new GuiCoreSettings()))
            {
                // When
                gui.Run(sourceFilePath);

                // Then
                Assert.IsNull(gui.ProjectFilePath);
                Assert.AreEqual("Project", gui.Project.Name);
                Assert.IsEmpty(gui.Project.Description);
            }

            mocks.VerifyAll();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(StorageMigrationIntegrationTest));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            directoryDisposeHelper.Dispose();
        }

        private static void MigrateFile(string sourceFilePath, string targetFilePath)
        {
            string newVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);
            var migrator = new ProjectFileMigrator();

            migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);
        }

        private static string GetTestProjectFilePath(string fileName)
        {
            return TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core, fileName);
        }
    }
}
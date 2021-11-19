// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Helpers;
using Core.Gui.Settings;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Util;
using Riskeer.Integration.Data;
using Riskeer.Migration;
using Riskeer.Migration.Core;
using Riskeer.Storage.Core;

namespace Application.Riskeer.Integration.Test
{
    [TestFixture]
    public class StorageMigrationIntegrationTest
    {
        private readonly string workingDirectory = TestHelper.GetScratchPadPath(nameof(StorageMigrationIntegrationTest));
        private DirectoryDisposeHelper directoryDisposeHelper;

        [Test]
        [TestCaseSource(nameof(GetAllOutdatedSupportedProjects))]
        [Apartment(ApartmentState.STA)]
        public void GivenRiskeerGuiWithStorageSql_WhenRunWithMigratedFile_MigratedProjectSet(string sourceFilePath)
        {
            string targetFilePath = Path.Combine(workingDirectory, nameof(GivenRiskeerGuiWithStorageSql_WhenRunWithMigratedFile_MigratedProjectSet));
            MigrateFile(sourceFilePath, targetFilePath);

            // Given
            var projectStore = new StorageSqLite();
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            var projectMigrator = new ProjectMigrator(inquiryHelper);
            var guiCoreSettings = new GuiCoreSettings
            {
                ApplicationIcon = SystemIcons.Application
            };

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), guiCoreSettings))
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
        [TestCaseSource(nameof(GetAllOutdatedSupportedProjects))]
        [Apartment(ApartmentState.STA)]
        public void GivenRiskeerGui_WhenRunWithUnmigratedFileAndInquireContinuation_MigratedProjectSet(string sourceFilePath)
        {
            // Given
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
            var guiCoreSettings = new GuiCoreSettings
            {
                ApplicationIcon = SystemIcons.Application
            };

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), guiCoreSettings))
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
        [TestCaseSource(nameof(GetAllOutdatedSupportedProjects))]
        [Apartment(ApartmentState.STA)]
        public void GivenRiskeerGui_WhenRunWithUnmigratedFileAndNoInquireContinuation_MigratedProjectNotSet(string sourceFilePath)
        {
            // Given
            var projectStore = new StorageSqLite();
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(false);
            mocks.ReplayAll();

            var projectMigrator = new ProjectMigrator(inquiryHelper);

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                // When
                gui.Run(sourceFilePath);

                // Then
                Assert.IsNull(gui.ProjectFilePath);
                Assert.IsNull(gui.Project);
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

        private static IEnumerable<TestCaseData> GetAllOutdatedSupportedProjects()
        {
            var fileDirectory = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Application.Riskeer.Integration), "StorageMigrationIntegrationTest");
            return GetAllOutdatedSupportedProjectFileNames().Select(projectFileName => new TestCaseData(Path.Combine(fileDirectory, projectFileName))
                                                                        .SetName(projectFileName))
                                                            .ToArray();
        }

        private static IEnumerable<string> GetAllOutdatedSupportedProjectFileNames()
        {
            yield return "MigrationTestProjectSingleAssessmentSection164.rtd";
            yield return "MigrationTestProjectSingleAssessmentSection171.rtd";
            yield return "MigrationTestProjectSingleAssessmentSection172.rtd";
            yield return "MigrationTestProjectSingleAssessmentSection173.rtd";
            yield return "MigrationTestProjectSingleAssessmentSection181.rtd";
            yield return "MigrationTestProjectSingleAssessmentSection191.risk";
            yield return "MigrationTestProjectSingleAssessmentSection211.risk";
        }

        private static void MigrateFile(string sourceFilePath, string targetFilePath)
        {
            string newVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);
            var migrator = new ProjectFileMigrator();

            migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);
        }
    }
}
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
using System.Threading;
using Application.Ringtoets.Migration;
using Application.Ringtoets.Migration.Core;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Utils;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class StorageMigrationIntegrationTest
    {
        [Test]
        [TestCaseSource(typeof(RingtoetsProjectMigrationTestHelper), nameof(RingtoetsProjectMigrationTestHelper.GetAllOutdatedSupportedProjectFilePaths))]
        [Apartment(ApartmentState.STA)]
        public void GivenRingtoetsGuiWithStorageSql_WhenRunWithMigratedFile_MigratedProjectSet(string sourceFilePath)
        {
            string targetFilePath = TestHelper.GetScratchPadPath(nameof(GivenRingtoetsGuiWithStorageSql_WhenRunWithMigratedFile_MigratedProjectSet));
            using (new FileDisposeHelper(targetFilePath))
            {
                MigrateFile(sourceFilePath, targetFilePath);

                // Given
                var projectStore = new StorageSqLite();

                var mocks = new MockRepository();
                var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
                mocks.ReplayAll();

                var projectMigrator = new RingtoetsProjectMigrator(inquiryHelper);

                using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
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
                GC.WaitForPendingFinalizers();

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(typeof(RingtoetsProjectMigrationTestHelper), nameof(RingtoetsProjectMigrationTestHelper.GetAllOutdatedSupportedProjectFilePaths))]
        [Apartment(ApartmentState.STA)]
        public void GivenRingtoetsGui_WhenRunWithUnmigratedFileAndInquireContinuation_MigratedProjectSet(string sourceFilePath)
        {
            // Given
            string targetFilePath = TestHelper.GetScratchPadPath(nameof(GivenRingtoetsGui_WhenRunWithUnmigratedFileAndInquireContinuation_MigratedProjectSet));
            using (new FileDisposeHelper(targetFilePath))
            {
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

                var projectMigrator = new RingtoetsProjectMigrator(inquiryHelper);

                using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
                {
                    // When
                    gui.Run(sourceFilePath);

                    // Then
                    Assert.AreEqual(targetFilePath, gui.ProjectFilePath);
                    string expectedProjectName = Path.GetFileNameWithoutExtension(targetFilePath);
                    Assert.AreEqual(expectedProjectName, gui.Project.Name);
                    Assert.AreEqual("description", gui.Project.Description);
                    Assert.IsInstanceOf<RingtoetsProject>(gui.Project);
                }
                GC.WaitForPendingFinalizers();

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(typeof(RingtoetsProjectMigrationTestHelper), nameof(RingtoetsProjectMigrationTestHelper.GetAllOutdatedSupportedProjectFilePaths))]
        [Apartment(ApartmentState.STA)]
        public void GivenRingtoetsGui_WhenRunWithUnmigratedFileAndNoInquireContinuation_MigratedProjectSet(string sourceFilePath)
        {
            // Given
            var projectStore = new StorageSqLite();

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.InquireContinuation(null))
                         .IgnoreArguments()
                         .Return(false);
            mocks.ReplayAll();

            var projectMigrator = new RingtoetsProjectMigrator(inquiryHelper);

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                // When
                gui.Run(sourceFilePath);

                // Then
                Assert.IsNull(gui.ProjectFilePath);
                Assert.AreEqual("Project", gui.Project.Name);
                Assert.IsEmpty(gui.Project.Description);
            }
            GC.WaitForPendingFinalizers();

            mocks.VerifyAll();
        }

        private static void MigrateFile(string sourceFilePath, string targetFilePath)
        {
            string newVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);
        }
    }
}
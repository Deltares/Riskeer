// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using Core.Plugins.ProjectExplorer;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Plugin;

namespace Core.Common.Integration.Test.Ringtoets.Application.Ringtoets
{
    [TestFixture]
    public class GuiCoreIntegrationTest
    {
        [SetUp]
        public void SetUp()
        {
            LogHelper.ResetLogging();
        }

        [TearDown]
        public void TearDown()
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_GuiWithRingtoetsPlugin_DoesNotCrash()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new RingtoetsPlugin());
                gui.Run();
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Run_StartWithCommonPlugins_RunsFasterThanThreshold()
        {
            TestHelper.AssertIsFasterThan(7500, StartWithCommonPlugins);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void FormActionIsRunForMainWindow()
        {
            //testing testhelper + visible changed event of mainwindow.
            //could be tested separately but the combination is vital to many tests. That's why this test is here.
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();
                var callCount = 0;
                WpfTestHelper.ShowModal((Control) gui.MainWindow, () => callCount++);
                Assert.AreEqual(1, callCount);
            }

            mocks.VerifyAll();
        }

        private static void StartWithCommonPlugins()
        {
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new RingtoetsPlugin());
                gui.Plugins.Add(new ProjectExplorerPlugin());

                gui.Run();

                WpfTestHelper.ShowModal((Control) gui.MainWindow);
            }

            mocks.VerifyAll();
        }
    }
}
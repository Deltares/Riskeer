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
using System.Windows;
using System.Windows.Threading;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Settings;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.Chart.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Chart.Test
{
    [TestFixture]
    public class ChartPluginTest
    {
        [Test]
        public void DefaultConstructor_Always_NoRibbonCommandHandlerSet()
        {
            // Call
            using (var plugin = new ChartPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.IsNull(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        [RequiresSTA]
        public void Activate_WithoutGui_ThrowsArgumentNullException()
        {
            // Setup
            using (var plugin = new ChartPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                Assert.Throws<ArgumentNullException>(test);
            }
        }

        [Test]
        [RequiresSTA]
        public void Activate_WithGui_AddsChartLegendView()
        {
            // Setup
            var mocks = new MockRepository();

            using (var plugin = new ChartPlugin())
            {
                var gui = mocks.StrictMock<IGui>();
                var viewHost = mocks.Stub<IViewHost>();

                gui.Stub(g => g.ViewHost).Return(viewHost);
                viewHost.Expect(vm => vm.ToolViews).Return(new IView[0]);
                viewHost.Expect(vm => vm.AddToolView(Arg<ChartLegendView>.Is.NotNull, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
                viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
                viewHost.Expect(vm => vm.ActiveDocumentView).Return(null);
                viewHost.Expect(vm => vm.ActiveDocumentViewChanged += null).IgnoreArguments();
                viewHost.Expect(vm => vm.ActiveDocumentViewChanged -= null).IgnoreArguments();

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                plugin.Activate();

                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.NotNull(plugin.RibbonCommandHandler);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [RequiresSTA]
        public void GivenConfiguredGui_WhenActiveDocumentViewChangesToViewWithChart_ThenRibbonSetVisibility(bool visible)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                var plugin = new ChartPlugin();
                var testChartView = new TestChartView();
                var chart = new ChartControl();
                IView viewMock = visible ? (IView) testChartView : new TestView();

                testChartView.Data = chart;

                gui.Plugins.Add(plugin);
                plugin.Gui = gui;
                gui.Run();

                // When
                gui.ViewHost.AddDocumentView(viewMock);

                // Then
                Assert.AreEqual(visible ? Visibility.Visible : Visibility.Collapsed, plugin.RibbonCommandHandler.GetRibbonControl().ContextualGroups[0].Visibility);
                mocks.VerifyAll();
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}
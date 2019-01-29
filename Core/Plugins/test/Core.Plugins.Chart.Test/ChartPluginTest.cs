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

using System;
using System.Linq;
using System.Threading;
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
using Core.Common.Gui.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.Chart.Legend;
using Core.Plugins.Chart.PropertyClasses;
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
        public void Activate_WithGui_AddsChartLegendView()
        {
            // Setup
            var mocks = new MockRepository();

            using (var plugin = new ChartPlugin())
            {
                var gui = mocks.StrictMock<IGui>();
                var viewHost = mocks.Stub<IViewHost>();

                gui.Stub(g => g.ViewHost).Return(viewHost);
                viewHost.Expect(vm => vm.AddToolView(Arg<ChartLegendView>.Is.NotNull, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
                viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
                viewHost.Expect(vm => vm.ActiveDocumentView).Return(null);
                viewHost.Expect(vm => vm.ActiveDocumentViewChanged += null).IgnoreArguments();
                viewHost.Expect(vm => vm.ActiveDocumentViewChanged -= null).IgnoreArguments();
                viewHost.Expect(vm => vm.ViewOpened += null).IgnoreArguments();
                viewHost.Expect(vm => vm.ViewOpened -= null).IgnoreArguments();
                viewHost.Expect(vm => vm.ViewBroughtToFront += null).IgnoreArguments();
                viewHost.Expect(vm => vm.ViewBroughtToFront -= null).IgnoreArguments();
                viewHost.Expect(vm => vm.ViewClosed += null).IgnoreArguments();
                viewHost.Expect(vm => vm.ViewClosed -= null).IgnoreArguments();

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
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new ChartPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(6, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ChartDataCollection),
                    typeof(ChartDataCollectionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ChartLineData),
                    typeof(ChartLineDataProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ChartAreaData),
                    typeof(ChartAreaDataProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ChartMultipleAreaData),
                    typeof(ChartMultipleAreaDataProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ChartMultipleLineData),
                    typeof(ChartMultipleLineDataProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ChartPointData),
                    typeof(ChartPointDataProperties));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenActiveDocumentViewChangesToViewWithChart_ThenRibbonSetVisibility(bool visible)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var plugin = new ChartPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var testChartView = new TestChartView
                {
                    Data = new ChartControl()
                };
                IView view = visible ? (IView) testChartView : new TestView();

                // When
                gui.ViewHost.AddDocumentView(view);

                // Then
                Assert.AreEqual(visible ? Visibility.Visible : Visibility.Collapsed, plugin.RibbonCommandHandler.GetRibbonControl().ContextualGroups[0].Visibility);
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenChartViewAdded_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var plugin = new ChartPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var view = new TestChartView();
                IViewHost guiViewHost = gui.ViewHost;
                ChartLegendView chartLegendView = guiViewHost.ToolViews.OfType<ChartLegendView>().First();
                var chartingRibbon = (ChartingRibbon) plugin.RibbonCommandHandler;

                // Precondition
                Assert.IsNull(GetChartControl(chartLegendView));
                Assert.IsNull(GetChartControl(chartingRibbon));

                // When
                guiViewHost.AddDocumentView(view);

                // Then
                Assert.AreSame(view.Chart, GetChartControl(chartLegendView));
                Assert.AreSame(view.Chart, GetChartControl(chartingRibbon));
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenChartViewBroughtToFront_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var plugin = new ChartPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var view1 = new TestChartView();
                var view2 = new TestChartView();
                IViewHost guiViewHost = gui.ViewHost;
                ChartLegendView chartLegendView = guiViewHost.ToolViews.OfType<ChartLegendView>().First();
                var chartingRibbon = (ChartingRibbon) plugin.RibbonCommandHandler;

                guiViewHost.AddDocumentView(view1);
                guiViewHost.AddDocumentView(view2);

                // Precondition
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));
                Assert.AreSame(view2.Chart, GetChartControl(chartingRibbon));

                // When
                guiViewHost.BringToFront(view1);

                // Then
                Assert.AreSame(view1.Chart, GetChartControl(chartLegendView));
                Assert.AreSame(view1.Chart, GetChartControl(chartingRibbon));
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenChartViewRemoved_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var plugin = new ChartPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var view = new TestChartView();
                IViewHost guiViewHost = gui.ViewHost;
                ChartLegendView chartLegendView = guiViewHost.ToolViews.OfType<ChartLegendView>().First();
                var chartingRibbon = (ChartingRibbon) plugin.RibbonCommandHandler;

                guiViewHost.AddDocumentView(view);

                // Precondition
                Assert.AreSame(view.Chart, GetChartControl(chartLegendView));
                Assert.AreSame(view.Chart, GetChartControl(chartingRibbon));

                // When
                guiViewHost.Remove(view);

                // Then
                Assert.IsNull(GetChartControl(chartLegendView));
                Assert.IsNull(GetChartControl(chartingRibbon));
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenOtherChartViewRemoved_ThenComponentsNotUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                var plugin = new ChartPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var view1 = new TestChartView();
                var view2 = new TestChartView();
                IViewHost guiViewHost = gui.ViewHost;
                ChartLegendView chartLegendView = guiViewHost.ToolViews.OfType<ChartLegendView>().First();
                var chartingRibbon = (ChartingRibbon) plugin.RibbonCommandHandler;

                guiViewHost.AddDocumentView(view1);
                guiViewHost.AddDocumentView(view2);

                // Precondition
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));
                Assert.AreSame(view2.Chart, GetChartControl(chartingRibbon));

                // When
                guiViewHost.Remove(view1);

                // Then
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));
                Assert.AreSame(view2.Chart, GetChartControl(chartingRibbon));
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        private static IChartControl GetChartControl(ChartingRibbon chartRibbon)
        {
            return TypeUtils.GetProperty<IChartControl>(chartRibbon, "Chart");
        }

        private static IChartControl GetChartControl(ChartLegendView chartLegendView)
        {
            return TypeUtils.GetProperty<IChartControl>(chartLegendView, "ChartControl");
        }
    }
}
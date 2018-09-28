// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.PropertyClasses;
using DotSpatial.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test
{
    [TestFixture]
    public class MapPluginTest
    {
        [Test]
        public void DefaultConstructor_Always_NoRibbonCommandHandlerSet()
        {
            // Call
            using (var plugin = new MapPlugin())
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
            using (var plugin = new MapPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("viewController", exception.ParamName);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Activate_WithGui_AddsMapLegendView()
        {
            // Setup
            var mocks = new MockRepository();

            using (var plugin = new MapPlugin())
            {
                var gui = mocks.StrictMock<IGui>();
                var viewHost = mocks.Stub<IViewHost>();

                gui.Stub(g => g.ViewHost).Return(viewHost);
                viewHost.Expect(vm => vm.AddToolView(Arg<MapLegendView>.Is.NotNull, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
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
                viewHost.Expect(vm => vm.Remove(Arg<MapLegendView>.Is.NotNull));

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
            using (var plugin = new MapPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(4, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MapDataCollection),
                    typeof(MapDataCollectionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MapPointData),
                    typeof(MapPointDataProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MapLineData),
                    typeof(MapLineDataProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MapPolygonData),
                    typeof(MapPolygonDataProperties));
            }
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedImportInfos()
        {
            // Setup
            using (var plugin = new MapPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(1, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(MapDataCollection)));
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenConfiguredGui_WhenActiveDocumentViewChangesToViewWithMap_ThenRibbonSetVisibility(bool visible)
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
                var plugin = new MapPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                IView view = visible ? (IView) new TestMapView() : new TestView();

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
        public void GivenConfiguredGui_WhenMapViewOpened_ThenMapZoomedToExtents()
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
                gui.Plugins.Add(new MapPlugin
                {
                    Gui = gui
                });

                gui.Run();

                var testMapView = new TestMapView();
                var map = (DotSpatialMap) ((MapControl) testMapView.Map).Controls[0];

                // Precondition
                Extent initialExtents = map.ViewExtents;

                // When
                gui.ViewHost.AddDocumentView(testMapView);

                // Then
                Assert.AreNotEqual(initialExtents, map.ViewExtents);
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenMapViewAdded_ThenComponentsUpdated()
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
                var plugin = new MapPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var view = new TestMapView();
                IViewHost guiViewHost = gui.ViewHost;
                MapLegendView mapLegendView = guiViewHost.ToolViews.OfType<MapLegendView>().First();
                var mapRibbon = (MapRibbon) plugin.RibbonCommandHandler;

                // Precondition
                Assert.IsNull(GetMapControl(mapLegendView));
                Assert.IsNull(GetMapControl(mapRibbon));

                // When
                guiViewHost.AddDocumentView(view);

                // Then
                Assert.AreSame(view.Map, GetMapControl(mapLegendView));
                Assert.AreSame(view.Map, GetMapControl(mapRibbon));
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenMapViewBroughtToFront_ThenComponentsUpdated()
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
                var plugin = new MapPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var view1 = new TestMapView();
                var view2 = new TestMapView();
                IViewHost guiViewHost = gui.ViewHost;
                MapLegendView mapLegendView = guiViewHost.ToolViews.OfType<MapLegendView>().First();
                var mapRibbon = (MapRibbon) plugin.RibbonCommandHandler;

                guiViewHost.AddDocumentView(view1);
                guiViewHost.AddDocumentView(view2);

                // Precondition
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));
                Assert.AreSame(view2.Map, GetMapControl(mapRibbon));

                // When
                guiViewHost.BringToFront(view1);

                // Then
                Assert.AreSame(view1.Map, GetMapControl(mapLegendView));
                Assert.AreSame(view1.Map, GetMapControl(mapRibbon));
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenMapViewRemoved_ThenComponentsUpdated()
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
                var plugin = new MapPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var view = new TestMapView();
                IViewHost guiViewHost = gui.ViewHost;
                MapLegendView mapLegendView = guiViewHost.ToolViews.OfType<MapLegendView>().First();
                var mapRibbon = (MapRibbon) plugin.RibbonCommandHandler;

                guiViewHost.AddDocumentView(view);

                // Precondition
                Assert.AreSame(view.Map, GetMapControl(mapLegendView));
                Assert.AreSame(view.Map, GetMapControl(mapRibbon));

                // When
                guiViewHost.Remove(view);

                // Then
                Assert.IsNull(GetMapControl(mapLegendView));
                Assert.IsNull(GetMapControl(mapRibbon));
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenConfiguredGui_WhenOtherMapViewRemoved_ThenComponentsNotUpdated()
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
                var plugin = new MapPlugin
                {
                    Gui = gui
                };

                gui.Plugins.Add(plugin);
                gui.Run();

                var view1 = new TestMapView();
                var view2 = new TestMapView();
                IViewHost guiViewHost = gui.ViewHost;
                MapLegendView mapLegendView = guiViewHost.ToolViews.OfType<MapLegendView>().First();
                var mapRibbon = (MapRibbon) plugin.RibbonCommandHandler;

                guiViewHost.AddDocumentView(view1);
                guiViewHost.AddDocumentView(view2);

                // Precondition
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));
                Assert.AreSame(view2.Map, GetMapControl(mapRibbon));

                // When
                guiViewHost.Remove(view1);

                // Then
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));
                Assert.AreSame(view2.Map, GetMapControl(mapRibbon));
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            mocks.VerifyAll();
        }

        private static IMapControl GetMapControl(MapRibbon mapRibbon)
        {
            return TypeUtils.GetProperty<IMapControl>(mapRibbon, "Map");
        }

        private static IMapControl GetMapControl(MapLegendView mapLegendView)
        {
            return TypeUtils.GetProperty<IMapControl>(mapLegendView, "MapControl");
        }
    }
}
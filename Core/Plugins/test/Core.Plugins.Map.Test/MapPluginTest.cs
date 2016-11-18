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
using System.Linq;
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
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.PropertyClasses;
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
        [RequiresSTA]
        public void Activate_WithoutGui_ThrowsArgumentNullException()
        {
            // Setup
            using (var plugin = new MapPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("viewController", exception.ParamName);
            }
        }

        [Test]
        [RequiresSTA]
        public void Activate_WithGui_AddsMapLegendView()
        {
            // Setup
            var mocks = new MockRepository();

            using (var plugin = new MapPlugin())
            {
                var gui = mocks.StrictMock<IGui>();
                var mainWindow = mocks.StrictMock<IMainWindow>();
                var viewHost = mocks.Stub<IViewHost>();

                gui.Expect(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ViewHost).Return(viewHost);
                viewHost.Expect(vm => vm.ToolViews).Return(new IView[0]);
                viewHost.Expect(vm => vm.AddToolView(Arg<MapLegendView>.Matches(c => true), Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
                viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
                viewHost.Expect(vm => vm.ActiveDocumentView).Return(null);
                viewHost.Expect(vm => vm.ActiveDocumentViewChanged += null).IgnoreArguments();
                viewHost.Expect(vm => vm.ActiveDocumentViewChanged -= null).IgnoreArguments();
                viewHost.Expect(vm => vm.Remove(Arg<MapLegendView>.Matches(c => true)));

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
        public void GetPropertyInfos_ReturnsSupportedPropertyClassesWithExpectedValues()
        {
            // Setup
            using (var plugin = new MapPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(4, propertyInfos.Length);

                PropertyInfo mapDataCollectionPropertyInfo = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MapDataCollection),
                    typeof(MapDataCollectionProperties));
                Assert.IsNull(mapDataCollectionPropertyInfo.AdditionalDataCheck);
                Assert.IsNull(mapDataCollectionPropertyInfo.GetObjectPropertiesData);
                Assert.IsNull(mapDataCollectionPropertyInfo.AfterCreate);

                PropertyInfo mapPointDataPropertyInfo = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MapPointData),
                    typeof(MapPointDataProperties));
                Assert.IsNull(mapPointDataPropertyInfo.AdditionalDataCheck);
                Assert.IsNull(mapPointDataPropertyInfo.GetObjectPropertiesData);
                Assert.IsNull(mapPointDataPropertyInfo.AfterCreate);

                PropertyInfo mapLineDataPropertyInfo = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MapLineData),
                    typeof(MapLineDataProperties));
                Assert.IsNull(mapLineDataPropertyInfo.AdditionalDataCheck);
                Assert.IsNull(mapLineDataPropertyInfo.GetObjectPropertiesData);
                Assert.IsNull(mapLineDataPropertyInfo.AfterCreate);

                PropertyInfo mapPolygonDataPropertyInfo = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MapPolygonData),
                    typeof(MapPolygonDataProperties));
                Assert.IsNull(mapPolygonDataPropertyInfo.AdditionalDataCheck);
                Assert.IsNull(mapPolygonDataPropertyInfo.GetObjectPropertiesData);
                Assert.IsNull(mapPolygonDataPropertyInfo.AfterCreate);
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenConfiguredGui_WhenActiveDocumentViewChangesToViewWithMap_ThenRibbonSetVisibility(bool visible)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectFactory, new GuiCoreSettings()))
            {
                var plugin = new MapPlugin();
                var testMapView = new TestMapView();
                var map = new MapControl();
                IView viewMock = visible ? (IView) testMapView : new TestView();

                testMapView.Data = map;

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
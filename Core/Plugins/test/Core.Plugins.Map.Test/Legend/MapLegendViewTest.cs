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
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.Legend
{
    [TestFixture]
    public class MapLegendViewTest
    {
        private MockRepository mocks;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;
        private IWin32Window parentWindow;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            parentWindow = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ContextMenuBuilderProviderAndWindowNotNull_CreatesUserControlAndTreeViewControl()
        {
            // Call
            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow))
            {
                var treeView = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");

                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(Resources.General_Map, view.Text);
                Assert.IsNotNull(treeView);
                Assert.IsInstanceOf<TreeViewControl>(treeView);
            }
        }

        [Test]
        public void Constructor_ContextMenuBuilderProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLegendView(null, parentWindow);

            // Assert
            const string expectedMessage = "Cannot create a MapLegendView when the context menu builder provider is null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_ParentWindowNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLegendView(contextMenuBuilderProvider, null);

            // Assert
            const string expectedMessage = "Cannot create a MapLegendView when the parent window is null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Data_MapDataCollection_DataSet()
        {
            // Setup
            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow))
            {
                var mapData = new MapDataCollection("test data");

                // Call
                view.Data = mapData;

                // Assert
                Assert.AreSame(mapData, view.Data);
                Assert.IsInstanceOf<MapData>(view.Data);
            }
        }

        [Test]
        public void Data_MapPointData_DataSet()
        {
            // Setup
            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow))
            {
                var mapData = new MapPointData("test data")
                {
                    Features = CreateFeatures()
                };

                // Call
                view.Data = mapData;

                // Assert
                Assert.AreSame(mapData, view.Data);
                Assert.IsInstanceOf<MapData>(view.Data);
            }
        }

        [Test]
        public void Data_MapLineData_DataSet()
        {
            // Setup
            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow))
            {
                var mapData = new MapLineData("test data")
                {
                    Features = CreateFeatures()
                };

                // Call
                view.Data = mapData;

                // Assert
                Assert.AreSame(mapData, view.Data);
                Assert.IsInstanceOf<MapData>(view.Data);
            }
        }

        [Test]
        public void Data_MapPolygonData_DataSet()
        {
            // Setup
            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow))
            {
                var mapData = new MapPolygonData("test data")
                {
                    Features = CreateFeatures()
                };

                // Call
                view.Data = mapData;

                // Assert
                Assert.AreSame(mapData, view.Data);
                Assert.IsInstanceOf<MapData>(view.Data);
            }
        }

        [Test]
        public void Data_ForNull_NullSet()
        {
            // Setup
            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow))
            {
                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_OtherObject_ThrowsInvalidCastException()
        {
            // Setup 
            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow))
            {
                // Call
                TestDelegate test = () => view.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(test);
            }
        }

        [Test]
        [RequiresSTA]
        public void Selection_Always_ReturnsSelectedNodeData()
        {
            // Setup
            var mapData = new MapLineData("line data");
            var mapDataCollection = new MapDataCollection("collection");

            mapDataCollection.Add(mapData);

            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow)
            {
                Data = mapDataCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");

                WindowsFormsTestHelper.Show(treeViewControl);
                treeViewControl.TrySelectNodeForData(mapData);

                // Call
                var selection = view.Selection;

                // Assert
                Assert.AreSame(mapData, selection);
            }
            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [RequiresSTA]
        public void TreeViewSelectedNodeChanged_Always_SelectionChangedFired()
        {
            // Setup
            var mapData = new MapLineData("line data");
            var mapDataCollection = new MapDataCollection("collection");

            mapDataCollection.Add(mapData);

            using (var view = new MapLegendView(contextMenuBuilderProvider, parentWindow)
            {
                Data = mapDataCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");

                WindowsFormsTestHelper.Show(treeViewControl);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                // Call
                treeViewControl.TrySelectNodeForData(mapData);

                // Assert
                Assert.AreEqual(1, selectionChangedCount);
            }
            WindowsFormsTestHelper.CloseAll();
        }

        private static MapFeature[] CreateFeatures()
        {
            return new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };
        }
    }
}
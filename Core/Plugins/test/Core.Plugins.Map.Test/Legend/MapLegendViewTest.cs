// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
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

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
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
            using (var view = new MapLegendView(contextMenuBuilderProvider))
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");

                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(Resources.General_Map, view.Text);
                Assert.IsNotNull(treeViewControl);
                Assert.IsInstanceOf<TreeViewControl>(treeViewControl);
            }
        }

        [Test]
        public void Constructor_ContextMenuBuilderProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLegendView(null);

            // Assert
            const string expectedMessage = "Cannot create a MapLegendView when the context menu builder provider is null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Data_MapDataCollection_DataSet()
        {
            // Setup
            using (var view = new MapLegendView(contextMenuBuilderProvider))
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
            using (var view = new MapLegendView(contextMenuBuilderProvider))
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
            using (var view = new MapLegendView(contextMenuBuilderProvider))
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
            using (var view = new MapLegendView(contextMenuBuilderProvider))
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
        public void Data_Null_NullSet()
        {
            // Setup
            using (var view = new MapLegendView(contextMenuBuilderProvider))
            {
                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_OtherObjectType_ThrowsInvalidCastException()
        {
            // Setup 
            using (var view = new MapLegendView(contextMenuBuilderProvider))
            {
                // Call
                TestDelegate test = () => view.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(test);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void MapControl_MapControlHasMapWithData_DataReturnsMapDataOfMap()
        {
            // Setup
            var mapData = new MapDataCollection("A");
            var mockRepository = new MockRepository();
            var mapControl = mockRepository.Stub<IMapControl>();

            mapControl.Expect(mc => mc.Data).Return(mapData);
            mockRepository.ReplayAll();

            using (var view = new MapLegendView(contextMenuBuilderProvider)
            {
                Data = new MapDataCollection("A")
            })
            {
                // Call
                view.MapControl = mapControl;

                // Assert
                Assert.AreSame(mapData, view.Data);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void MapControl_DataSetAndThenMapControlSetToNull_DataSetToNull()
        {
            // Setup
            using (var view = new MapLegendView(contextMenuBuilderProvider)
            {
                Data = new MapDataCollection("A")
            })
            {
                // Call
                view.MapControl = null;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Selection_Always_ReturnsSelectedNodeData()
        {
            // Setup
            var mapData = new MapLineData("line data");
            var mapDataCollection = new MapDataCollection("collection");

            mapDataCollection.Add(mapData);

            using (var view = new MapLegendView(contextMenuBuilderProvider)
            {
                Data = mapDataCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");

                WindowsFormsTestHelper.Show(treeViewControl);
                treeViewControl.TrySelectNodeForData(mapData);

                // Call
                object selection = view.Selection;

                // Assert
                Assert.AreSame(mapData, selection);
            }

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenMapLegendView_WhenSelectedNodeChanged_SelectionChangedFired()
        {
            // Given
            var mapData = new MapLineData("line data");
            var mapDataCollection = new MapDataCollection("collection");

            mapDataCollection.Add(mapData);

            using (var view = new MapLegendView(contextMenuBuilderProvider)
            {
                Data = mapDataCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");
                WindowsFormsTestHelper.Show(treeViewControl);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                // When
                treeViewControl.TrySelectNodeForData(mapData);

                // Then
                Assert.AreEqual(1, selectionChangedCount);
            }

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenMapLegendView_WhenSettingData_SelectionChangedFired()
        {
            // Given
            using (var view = new MapLegendView(contextMenuBuilderProvider))
            {
                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");
                WindowsFormsTestHelper.Show(treeViewControl);

                // When
                view.Data = new MapDataCollection("collection");

                // Then
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
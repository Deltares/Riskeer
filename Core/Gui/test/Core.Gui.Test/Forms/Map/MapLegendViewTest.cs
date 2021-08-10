﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Core.Components.Gis.TestUtil;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Map;
using Core.Gui.PresentationObjects.Map;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Gui.Test.Forms.Map
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
                Assert.IsNotNull(treeViewControl);
                Assert.IsInstanceOf<TreeViewControl>(treeViewControl);
            }
        }

        [Test]
        public void Constructor_ContextMenuBuilderProviderNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MapLegendView(null);

            // Assert
            const string expectedMessage = "Cannot create a MapLegendView when the context menu builder provider is null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(Call, expectedMessage);
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
                Assert.IsInstanceOf<MapDataCollectionContext>(view.Data);
                var viewData = (MapDataCollectionContext) view.Data;
                Assert.AreSame(mapData, viewData.WrappedData);
                Assert.IsNull(viewData.ParentMapData);
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
                void Call() => view.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(Call);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void MapControl_MapControlHasMapWithData_DataReturnsWrappedMapDataOfMap()
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
                Assert.AreSame(mapData, ((MapDataCollectionContext) view.Data).WrappedData);
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
        public void Selection_Always_ReturnsDataContext()
        {
            // Setup
            var mapData = new TestFeatureBasedMapData();
            var mapDataCollection = new MapDataCollection("collection");

            mapDataCollection.Add(mapData);

            using (var view = new MapLegendView(contextMenuBuilderProvider)
            {
                Data = mapDataCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");
                WindowsFormsTestHelper.Show(treeViewControl);
                treeViewControl.TrySelectNodeForData(view.Data);

                // Call
                var selection = (MapDataCollectionContext) view.Selection;

                // Assert
                Assert.AreSame(mapDataCollection, selection.WrappedData);
            }

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenMapLegendView_WhenSelectedNodeChanged_SelectionChangedFired()
        {
            // Given
            var mapData = new MapPointData("test");
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
                var context = new MapPointDataContext(mapData, new MapDataCollectionContext(mapDataCollection, null));
                treeViewControl.TrySelectNodeForData(context);

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
    }
}
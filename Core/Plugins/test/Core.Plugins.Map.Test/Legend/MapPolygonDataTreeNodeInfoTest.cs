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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.Legend
{
    [TestFixture]
    public class MapPolygonDataTreeNodeInfoTest
    {
        private MockRepository mocks;
        private MapLegendView mapLegendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            var mockRepository = new MockRepository();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mockRepository.StrictMock<IWin32Window>();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider, parentWindow);

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapPolygonData)];

            mocks = mockRepository;
        }

        [TearDown]
        public void TearDown()
        {
            mapLegendView.Dispose();

            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(MapPolygonData), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsNameFromMapData()
        {
            // Setup
            mocks.ReplayAll();
            var mapPolygonData = new MapPolygonData("MapPolygonData");

            // Call
            var text = info.Text(mapPolygonData);

            // Assert
            Assert.AreEqual(mapPolygonData.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.AreaIcon, image);
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();
            var mapPolygonData = new MapPolygonData("test data");

            // Call
            var canCheck = info.CanCheck(mapPolygonData);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();
            var mapPolygonData = new MapPolygonData("test data");

            // Call
            var canDrag = info.CanDrag(mapPolygonData, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfPolygonData(bool isVisible)
        {
            // Setup
            mocks.ReplayAll();
            var mapPolygonData = new MapPolygonData("test data")
            {
                IsVisible = isVisible
            };

            // Call
            var canCheck = info.IsChecked(mapPolygonData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_PolygonDataNodeWithoutParent_SetsPolygonDataVisibility(bool initialVisibleState)
        {
            // Setup
            mocks.ReplayAll();
            var mapPolygonData = new MapPolygonData("test data")
            {
                IsVisible = initialVisibleState
            };

            // Call
            info.OnNodeChecked(mapPolygonData, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, mapPolygonData.IsVisible);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_PolygonDataNodeWithObservableParent_SetsPolygonDataVisibilityAndNotifiesParentObservers(bool initialVisibleState)
        {
            // Setup
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());

            mocks.ReplayAll();

            var mapPolygonData = new MapPolygonData("test data")
            {
                IsVisible = initialVisibleState
            };

            // Call
            info.OnNodeChecked(mapPolygonData, observable);

            // Assert
            Assert.AreEqual(!initialVisibleState, mapPolygonData.IsVisible);
        }
    }
}
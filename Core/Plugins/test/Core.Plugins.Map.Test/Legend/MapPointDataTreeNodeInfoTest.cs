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
    public class MapPointDataTreeNodeInfoTest
    {
        private TreeNodeInfo info;
        private MockRepository mocks;
        private MapLegendView mapLegendView;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider);

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapPointData)];
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
            Assert.AreEqual(typeof(MapPointData), info.TagType);
            Assert.IsNull(info.ForeColor);
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
            var mapPointData = new MapPointData("MapPointData");

            // Call
            var text = info.Text(mapPointData);

            // Assert
            Assert.AreEqual(mapPointData.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.PointsIcon, image);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(menuBuilderMock);

            mocks.ReplayAll();

            // Call
            info.ContextMenuStrip(null, null, null);

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();
            var mapPointData = new MapPointData("test data");

            // Call
            var canCheck = info.CanCheck(mapPointData);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();
            var mapPointData = new MapPointData("test data");

            // Call
            var canDrag = info.CanDrag(mapPointData, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfPointData(bool isVisible)
        {
            // Setup
            mocks.ReplayAll();
            var mapPointData = new MapPointData("test data")
            {
                IsVisible = isVisible
            };

            // Call
            var canCheck = info.IsChecked(mapPointData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_MapPointDataNode_SetsPointDataVisibilityAndNotifiesObservers(bool initialVisibleState)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var mapPointData = new MapPointData("test data")
            {
                IsVisible = initialVisibleState
            };

            mapPointData.Attach(observer);

            // Call
            info.OnNodeChecked(mapPointData, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, mapPointData.IsVisible);
        }
    }
}
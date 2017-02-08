﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class CategoryTreeFolderTreeNodeInfoTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            mocks.ReplayAll();

            var testname = "testName";
            var categoryTreeFolder = new CategoryTreeFolder(testname, new object[0]);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(categoryTreeFolder);

                // Assert
                Assert.AreEqual(testname, text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_TreeFolderOfCategoryGeneral_ReturnsGeneralFolderIcon()
        {
            // Setup
            mocks.ReplayAll();

            var categoryTreeFolder = new CategoryTreeFolder("", new object[0]);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_TreeFolderOfCategoryInput_ReturnsInputFolderIcon()
        {
            // Setup
            mocks.ReplayAll();

            var categoryTreeFolder = new CategoryTreeFolder("", new object[0], TreeFolderCategory.Input);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.InputFolderIcon, image);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_TreeFolderOfCategoryOutput_ReturnsOutputFolderIcon()
        {
            // Setup
            mocks.ReplayAll();

            var categoryTreeFolder = new CategoryTreeFolder("", new object[0], TreeFolderCategory.Output);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.OutputFolderIcon, image);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            mocks.ReplayAll();

            var object1 = new object();
            var object2 = new object();
            var categoryTreeFolder = new CategoryTreeFolder("", new[]
            {
                object1,
                object2
            });

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var objects = info.ChildNodeObjects(categoryTreeFolder);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    object1,
                    object2
                }, objects);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(null, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }
            // Assert
            mocks.VerifyAll();
        }

        private TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(CategoryTreeFolder));
        }
    }
}
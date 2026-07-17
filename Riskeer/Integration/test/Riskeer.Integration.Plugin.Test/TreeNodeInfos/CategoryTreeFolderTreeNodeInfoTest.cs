// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class CategoryTreeFolderTreeNodeInfoTest
    {
        [SetUp]
        public void SetUp() {}

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

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
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            const string testname = "testName";
            var categoryTreeFolder = new CategoryTreeFolder(testname, new object[0]);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(categoryTreeFolder);

                // Assert
                Assert.AreEqual(testname, text);
            }
        }

        [Test]
        public void Image_TreeFolderOfCategoryGeneral_ReturnsGeneralFolderIcon()
        {
            // Setup
            var categoryTreeFolder = new CategoryTreeFolder("", new object[0]);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void Image_TreeFolderOfCategoryInput_ReturnsInputFolderIcon()
        {
            // Setup
            var categoryTreeFolder = new CategoryTreeFolder("", new object[0], TreeFolderCategory.Input);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.InputFolderIcon, image);
            }
        }

        [Test]
        public void Image_TreeFolderOfCategoryOutput_ReturnsOutputFolderIcon()
        {
            // Setup
            var categoryTreeFolder = new CategoryTreeFolder("", new object[0], TreeFolderCategory.Output);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.OutputFolderIcon, image);
            }
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var object1 = new object();
            var object2 = new object();
            var categoryTreeFolder = new CategoryTreeFolder("", new[]
            {
                object1,
                object2
            });

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(categoryTreeFolder);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    object1,
                    object2
                }, objects);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
            menuBuilder.AddExpandAllItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(null, treeViewControl).Returns(menuBuilder);
                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            Received.InOrder(() =>
            {
                menuBuilder.AddCollapseAllItem();
                menuBuilder.AddExpandAllItem();
                menuBuilder.Build();
            });
        }

        private TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(CategoryTreeFolder));
        }
    }
}
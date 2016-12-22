// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Properties;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsProjectTreeNodeInfoTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(RingtoetsProject), info.TagType);
                Assert.IsNull(info.ForeColor);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
            }
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var testName = "ttt";
            var project = new RingtoetsProject(testName);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(project);

                // Assert
                Assert.AreEqual(testName, text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var project = new RingtoetsProject();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(project);

                // Assert
                TestHelper.AssertImagesAreEqual(GuiResources.ProjectIcon, image);
            }
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var project = new RingtoetsProject();
            project.AssessmentSections.Add(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);
                // Call

                var objects = info.ChildNodeObjects(project);

                // Assert
                Assert.AreEqual(1, objects.Length);
                var actualAssessmentSection = (AssessmentSection) objects[0];
                Assert.AreSame(assessmentSection, actualAssessmentSection);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                var guiMock = mockRepository.Stub<IGui>();
                guiMock.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);
                    plugin.Gui = guiMock;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }
            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_ContextMenuItemAddAssessmentSectionEnabled()
        {
            // Setup
            var guiMock = mockRepository.StrictMock<IGui>();
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            var project = new RingtoetsProject();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(project, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(project, null, treeViewControl))
                    {
                        const string expectedItemText = "T&raject toevoegen...";
                        const string expectedItemTooltip = "Voeg een nieuw traject toe aan het project.";
                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, expectedItemText, expectedItemTooltip, Resources.AddAssessmentSectionFolder);
                    }
                }
            }
            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(RingtoetsProject));
        }
    }
}
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

using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class FailureMechanismPlaceholderTreeNodeInfoTest
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
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(FailureMechanismPlaceholderContext), info.TagType);
                Assert.IsNull(info.EnsureVisibleOnCreate);
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
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var testName = "ttt";
            var placeholder = new FailureMechanismPlaceholder(testName);
            var placeholderContext = new FailureMechanismPlaceholderContext(placeholder, assessmentSection);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(placeholderContext);

                // Assert
                Assert.AreEqual(testName, text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismIcon, image);
            }
        }

        [Test]
        public void ForeColor_Always_ReturnsControlText()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                mocks.ReplayAll();

                var failureMechanism = new FailureMechanismPlaceholder("C");
                var context = new FailureMechanismPlaceholderContext(failureMechanism, assessmentSection);

                // Call
                var textColor = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), textColor);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnFoldersWithInputAndOutput()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                var failureMechanism = new FailureMechanismPlaceholder("test");
                failureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(1, 2),
                    new Point2D(5, 6)
                }));
                var failureMechanismContext = new FailureMechanismPlaceholderContext(failureMechanism, assessmentSection);

                // Call
                object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(2, children.Length);
                var inputFolder = (CategoryTreeFolder) children[0];
                Assert.AreEqual("Invoer", inputFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Input, inputFolder.Category);

                var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputFolder.Contents[0];
                CollectionAssert.AreEqual(failureMechanism.Sections, failureMechanismSectionsContext.WrappedData);
                Assert.AreSame(failureMechanism, failureMechanismSectionsContext.ParentFailureMechanism);
                Assert.AreSame(assessmentSection, failureMechanismSectionsContext.ParentAssessmentSection);

                Assert.AreSame(failureMechanism.Locations, inputFolder.Contents[1]);
                Assert.AreSame(failureMechanism.BoundaryConditions, inputFolder.Contents[2]);

                var commentContext = (CommentContext<ICommentable>) inputFolder.Contents[3];
                Assert.IsNotNull(commentContext);
                Assert.AreSame(failureMechanism, commentContext.CommentContainer);

                var outputFolder = (CategoryTreeFolder) children[1];
                Assert.AreEqual("Uitvoer", outputFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Output, outputFolder.Category);

                var failureMechanismResultsContext = (FailureMechanismSectionResultContext) outputFolder.Contents[0];
                Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
                Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.SectionResults);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var failureMechanism = new FailureMechanismPlaceholder("A");
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new FailureMechanismPlaceholderContext(failureMechanism, assessmentSection);

                var gui = mocks.StrictMock<IGui>();
                var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);

                gui.Expect(cmp => cmp.Get(context, treeView)).Return(menuBuilderMock);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocks.ReplayAll();

                using (var plugin = new RingtoetsGuiPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, assessmentSection, treeView);
                }
                // Assert
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CalculateAllAndClearAllItemDisabled()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = new FailureMechanismPlaceholder("A");
                var context = new FailureMechanismPlaceholderContext(failureMechanism, assessmentSection);

                var menuBuilderMock = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeView)).Return(menuBuilderMock);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocks.ReplayAll();

                using (var plugin = new RingtoetsGuiPlugin())
                {
                    plugin.Gui = gui;

                    var info = GetInfo(plugin);

                    // Call
                    var menu = info.ContextMenuStrip(context, assessmentSection, treeView);

                    TestHelper.AssertContextMenuStripContainsItem(menu, 0,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, 1,
                                                                  RingtoetsCommonFormsResources.Clear_all_output,
                                                                  RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }

                // Assert
                mocks.VerifyAll();
            }
        }

        private TreeNodeInfo GetInfo(RingtoetsGuiPlugin guiPlugin)
        {
            return guiPlugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(FailureMechanismPlaceholderContext));
        }
    }
}
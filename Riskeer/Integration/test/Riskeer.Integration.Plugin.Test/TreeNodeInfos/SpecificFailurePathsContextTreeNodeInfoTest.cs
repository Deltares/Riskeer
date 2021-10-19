// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Integration.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class SpecificFailurePathsContextTreeNodeInfoTest
    {
        private const int contextMenuCreateFailurePathIndex = 0;

        private MockRepository mocks;
        private TreeNodeInfo info;
        private RiskeerPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RiskeerPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(SpecificFailurePathsContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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

        [Test]
        public void Text_Always_ReturnsSetText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePaths = new ObservableList<IFailurePath>();
            var failureMechanismContext = new SpecificFailurePathsContext(failurePaths, assessmentSection);

            // Call
            string text = info.Text(failureMechanismContext);

            // Assert
            Assert.AreEqual("Specifieke faalpaden", text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void ContextMenuStrip_Always_ReturnsExpectedItem()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var failurePaths = new ObservableList<IFailurePath>();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new SpecificFailurePathsContext(failurePaths, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                // Assert expectancies are called in TearDown()
            }
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            var failurePaths = new ObservableList<IFailurePath>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var context = new SpecificFailurePathsContext(failurePaths, assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeView = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreEqual(6, menu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCreateFailurePathIndex,
                                                                  "Faalpad toevoegen",
                                                                  "Voeg faalpad toe",
                                                                  RiskeerCommonFormsResources.FailureMechanismIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddSpecificFailurePathItem_SpecificFailurePathAddedAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var failurePaths = new ObservableList<IFailurePath>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var context = new SpecificFailurePathsContext(failurePaths, assessmentSection);
            context.Attach(observer);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeView = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // Call
                    menu.Items[contextMenuCreateFailurePathIndex].PerformClick();

                    // Assert
                    Assert.AreEqual(1, failurePaths.Count);
                    IFailurePath addedItem = failurePaths.Single();
                    Assert.IsInstanceOf<SpecificFailurePath>(addedItem);
                    Assert.AreEqual("Nieuw faalpad", addedItem.Name);
                }
            }
        }

        [Test]
        public void GivenSpecificFailurePathsContainsItems_WhenAddSpecificFailurePath_ThenItemAddedAndNotifyObservers()
        {
            // Given
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var failurePaths = new ObservableList<IFailurePath>
            {
                new SpecificFailurePath
                {
                    Name = "Nieuw faalpad"
                }
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var context = new SpecificFailurePathsContext(failurePaths, assessmentSection);
            context.Attach(observer);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeView = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // When
                    menu.Items[contextMenuCreateFailurePathIndex].PerformClick();

                    // Then
                    Assert.AreEqual(2, failurePaths.Count);
                    IFailurePath addedItem = failurePaths.Last();
                    Assert.IsInstanceOf<SpecificFailurePath>(addedItem);
                    Assert.AreEqual("Nieuw faalpad (1)", addedItem.Name);
                }
            }
        }

        [Test]
        public void ChildNodeObjects_WithoutFailurePaths_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePaths = new ObservableList<IFailurePath>();
            var context = new SpecificFailurePathsContext(failurePaths, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_WithFailurePaths_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePath = new SpecificFailurePath();
            var failurePaths = new ObservableList<IFailurePath>
            {
                failurePath
            };
            var context = new SpecificFailurePathsContext(failurePaths, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var failurePathContext = (SpecificFailurePathContext) children[0];
            Assert.AreSame(failurePath, failurePathContext.WrappedData);
            Assert.AreSame(assessmentSection, failurePathContext.Parent);
        }
    }
}
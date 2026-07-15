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
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Integration.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class SpecificFailureMechanismsContextTreeNodeInfoTest
    {
        private const int contextMenuCreateFailureMechanismIndex = 0;

        private TreeNodeInfo info;
        private RiskeerPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(SpecificFailureMechanismsContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNotNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.CheckedState);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsSetText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
            var failureMechanismContext = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            // Call
            string text = info.Text(failureMechanismContext);

            // Assert
            Assert.AreEqual("Specifieke faalmechanismen", text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void ExpandOnCreate_Always_ReturnsTrue()
        {
            // Setup
            // Call
            bool expandOnCreate = info.ExpandOnCreate(null);

            // Assert
            Assert.IsTrue(expandOnCreate);
        }

        [Test]
        public void ContextMenuStrip_Always_ReturnsExpectedItem()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var context = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

                var menuBuilder = Substitute.For<IContextMenuBuilder>();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddDeleteChildrenItem().Returns(menuBuilder);
                menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
                menuBuilder.AddExpandAllItem().Returns(menuBuilder);

                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeView).Returns(menuBuilder);
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                Received.InOrder(() =>
                {
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                    menuBuilder.AddSeparator();
                    menuBuilder.AddDeleteChildrenItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCollapseAllItem();
                    menuBuilder.AddExpandAllItem();
                    menuBuilder.Build();
                });
            }
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var context = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeView = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeView).Returns(menuBuilder);
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreEqual(6, menu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCreateFailureMechanismIndex,
                                                                  "Faalmechanisme &toevoegen",
                                                                  "Voeg een nieuw faalmechanisme toe aan deze map.",
                                                                  RiskeerCommonFormsResources.FailureMechanismIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddSpecificFailureMechanismItem_SpecificFailureMechanismAddedAndNotifyObservers()
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var context = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);
            context.Attach(observer);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeView = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeView).Returns(menuBuilder);
                plugin.Gui = gui;

                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // Call
                    menu.Items[contextMenuCreateFailureMechanismIndex].PerformClick();

                    // Assert
                    Assert.AreEqual(1, failureMechanisms.Count);
                    IFailureMechanism addedItem = failureMechanisms.Single();
                    Assert.IsInstanceOf<SpecificFailureMechanism>(addedItem);
                    Assert.AreEqual("Nieuw faalmechanisme", addedItem.Name);
                }
            }

            observer.Received().UpdateObserver();
        }

        [Test]
        public void GivenSpecificFailureMechanismsContainsItems_WhenAddSpecificFailureMechanism_ThenItemAddedAndNotifyObservers()
        {
            // Given
            var observer = Substitute.For<IObserver>();

            var failureMechanisms = new ObservableList<SpecificFailureMechanism>
            {
                new SpecificFailureMechanism
                {
                    Name = "Nieuw faalmechanisme"
                }
            };
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var context = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);
            context.Attach(observer);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeView = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeView).Returns(menuBuilder);
                plugin.Gui = gui;

                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // When
                    menu.Items[contextMenuCreateFailureMechanismIndex].PerformClick();

                    // Then
                    Assert.AreEqual(2, failureMechanisms.Count);
                    IFailureMechanism addedItem = failureMechanisms.Last();
                    Assert.IsInstanceOf<SpecificFailureMechanism>(addedItem);
                    Assert.AreEqual("Nieuw faalmechanisme (1)", addedItem.Name);
                }
            }

            observer.Received().UpdateObserver();
        }

        [Test]
        public void CanInsert_DraggedDataOfUnsupportedDataType_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
            var targetData = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            // Call
            bool canInsert = info.CanInsert(new object(), targetData);

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        public void CanInsert_DraggedDataNotPartOfContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
            var targetData = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            var draggedData = new SpecificFailureMechanismContext(new SpecificFailureMechanism(), assessmentSection);

            // Call
            bool canInsert = info.CanInsert(draggedData, targetData);

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        public void CanInsert_DraggedDataPartOfContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new SpecificFailureMechanism();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>
            {
                failureMechanism
            };

            var targetData = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);
            var draggedData = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool canInsert = info.CanInsert(draggedData, targetData);

            // Assert
            Assert.IsTrue(canInsert);
        }

        [Test]
        public void CanDrop_DraggedDataOfUnsupportedDataType_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
            var targetData = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            // Call
            bool canDrop = info.CanDrop(new object(), targetData);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanDrop_DraggedDataNotPartOfContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
            var targetData = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            var draggedData = new SpecificFailureMechanismContext(new SpecificFailureMechanism(), assessmentSection);

            // Call
            bool canDrop = info.CanDrop(draggedData, targetData);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanDrop_DraggedDataPartOfContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new SpecificFailureMechanism();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>
            {
                failureMechanism
            };

            var targetData = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);
            var draggedData = new SpecificFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool canDrop = info.CanDrop(draggedData, targetData);

            // Assert
            Assert.IsTrue(canDrop);
        }

        [Test]
        public void OnDrop_DataDroppedToDifferentIndex_DroppedDataCorrectlyMovedAndObserversNotified()
        {
            // Setup
            var observer = Substitute.For<IObserver>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var unmovedFailureMechanism = new SpecificFailureMechanism();
            var movedFailureMechanism = new SpecificFailureMechanism();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>
            {
                unmovedFailureMechanism,
                movedFailureMechanism
            };
            failureMechanisms.Attach(observer);

            var parentData = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);
            var draggedData = new SpecificFailureMechanismContext(movedFailureMechanism, assessmentSection);

            // Call
            info.OnDrop(draggedData, parentData, parentData, 0, null);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                movedFailureMechanism,
                unmovedFailureMechanism
            }, failureMechanisms);
            observer.Received().UpdateObserver();
        }

        [Test]
        public void ChildNodeObjects_WithoutFailureMechanisms_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>();
            var context = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_WithFailureMechanisms_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new SpecificFailureMechanism();
            var failureMechanisms = new ObservableList<SpecificFailureMechanism>
            {
                failureMechanism
            };
            var context = new SpecificFailureMechanismsContext(failureMechanisms, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var failureMechanismContext = (SpecificFailureMechanismContext) children[0];
            Assert.AreSame(failureMechanism, failureMechanismContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismContext.Parent);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Base.Service;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Service;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingCalculationGroupContextNodePresenterTest
    {
        private MockRepository mockRepository;
        private IContextMenuBuilderProvider contextMenuBuilderProviderMock;

        private const int contextMenuAddCalculationGroupIndex = 0;
        private const int contextMenuAddCalculationIndex = 1;
        private const int contextMenuValidateAllIndex = 2;
        private const int contextMenuCalculateAllIndex = 3;
        private const int contextMenuClearOutputIndex = 4;
        
        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingCalculationContextNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Setup

            // Call
            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PipingCalculationGroupContext>>(nodePresenter);
            Assert.AreEqual(typeof(PipingCalculationGroupContext), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var node = mocks.Stub<ITreeNode>();
            node.ForegroundColor = Color.AliceBlue;
            mocks.ReplayAll();
            
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(parentNode, node, nodeData);

            // Assert
            Assert.AreEqual(group.Name, node.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), node.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.FolderIcon, node.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_ParentIsPipingFailureMechanismNode_ReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var parentNode = mocks.Stub<ITreeNode>();
            parentNode.Tag = failureMechanism;

            var node = mocks.StrictMock<ITreeNode>();
            node.Expect(n => n.Parent).Return(parentNode).Repeat.Twice();
            mocks.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(node);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_EverythingElse_ReturnTrue()
        {
            // Setup
            var parentNode = mockRepository.Stub<ITreeNode>();
            parentNode.Tag = new object();

            var node = mockRepository.StrictMock<ITreeNode>();
            node.Expect(n => n.Parent).Return(parentNode).Repeat.Twice();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(node);

            // Assert
            Assert.IsTrue(isRenamingAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRenamedNodeTo_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var node = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNodeTo(node, "newName");

            // Assert
            Assert.IsTrue(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRenamed_WithData_RenameGroupAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());
            nodeData.Attach(observer);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            const string newName = "new name";
            nodePresenter.OnNodeRenamed(nodeData, newName);

            // Assert
            Assert.AreEqual(newName, group.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsFailureMechanism_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var parentNodeData = new PipingFailureMechanism();
            parentNodeData.CalculationsGroup.Children.Add(group);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(parentNodeData, nodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupContainingGroup_ReturnTrue()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var parentGroup = new PipingCalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());


            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(parentNodeData, nodeData);

            // Assert
            Assert.IsTrue(isRemovalAllowed);
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupNotContainingGroup_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var parentGroup = new PipingCalculationGroup();
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Precondition
            CollectionAssert.DoesNotContain(parentGroup.Children, group);

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(parentNodeData, nodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var parentGroup = new PipingCalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());
            parentNodeData.Attach(observer);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Precondition
            Assert.IsTrue(nodePresenter.CanRemove(parentNodeData, nodeData));

            // Call
            bool removealSuccesful = nodePresenter.RemoveNodeData(parentNodeData, nodeData);

            // Assert
            Assert.IsTrue(removealSuccesful);
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ValidData_ReturnContextWithItems()
        {
            // Setup
            var mocks = new MockRepository();
            var node = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Assert
            Assert.AreEqual(5, contextMenu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(
                contextMenu, 
                contextMenuAddCalculationGroupIndex,
                "Map toevoegen", 
                "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                PipingFormsResources.AddFolderIcon);

            TestHelper.AssertContextMenuStripContainsItem(
                contextMenu,
                contextMenuAddCalculationIndex,
                "Berekening toevoegen",
                "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(
                contextMenu,
                contextMenuValidateAllIndex,
                "Valideren",
                "Valideer alle berekeningen binnen deze berekeningsmap.",
                PipingFormsResources.ValidationIcon);

            TestHelper.AssertContextMenuStripContainsItem(
                contextMenu,
                contextMenuCalculateAllIndex,
                "Alles be&rekenen",
                "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                RingtoetsFormsResources.CalculateAllIcon);

            TestHelper.AssertContextMenuStripContainsItem(
                contextMenu,
                contextMenuClearOutputIndex,
                "&Wis alle uitvoer",
                "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                RingtoetsFormsResources.ClearIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationItem = mocks.Stub<IPipingCalculationItem>();
            calculationItem.Expect(ci => ci.Name).Return("Nieuwe map");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var node = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            group.Children.Add(calculationItem);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());
            nodeData.Attach(observer);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationGroupIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<PipingCalculationGroup>(newlyAddedItem);
            Assert.AreEqual("Nieuwe map (1)", newlyAddedItem.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationItem = mocks.Stub<IPipingCalculationItem>();
            calculationItem.Expect(ci => ci.Name).Return("Nieuwe berekening");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var node = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            group.Children.Add(calculationItem);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());
            nodeData.Attach(observer);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<PipingCalculation>(newlyAddedItem);
            Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var node = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(validCalculation);
            
            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(invalidCalculation);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Call
            Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validCalculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validCalculation.Name), msgs[1]);

                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidCalculation.Name), msgs[2]);
                // Some validation error from validation service
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidCalculation.Name), msgs[5]);
            });
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var node = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(validCalculation);

            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(invalidCalculation);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var activitesToRun = new List<Activity>();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock)
            {
                RunActivityAction = activity => activitesToRun.Add(activity)
            };

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Call
            contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, activitesToRun.Count);
            foreach (var activity in activitesToRun)
            {
                Assert.IsInstanceOf<PipingCalculationActivity>(activity);
            }
            CollectionAssert.AreEquivalent(new[]{validCalculation.Name, invalidCalculation.Name}, activitesToRun.Select(a=>a.Name));
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var node = mocks.StrictMock<ITreeNode>();

            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());

            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculation1 = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation1.Name = "A";
            calculation1.Output = new TestPipingOutput();
            calculation1.Attach(calculation1Observer);
            var calculation2 = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation2.Name = "B";
            calculation2.Output = new TestPipingOutput();
            calculation1.Attach(calculation2Observer);

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(calculation1);
            
            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(calculation2);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            // Precondition
            Assert.IsTrue(group.HasOutput);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Call
            contextMenu.Items[contextMenuClearOutputIndex].PerformClick();

            // Assert
            Assert.IsFalse(group.HasOutput);
            Assert.IsFalse(calculation1.HasOutput);
            Assert.IsFalse(calculation2.HasOutput);
            mocks.VerifyAll();
        }

        [Test]
        public void GetChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var children = nodePresenter.GetChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void GetChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationItem = mocks.StrictMock<IPipingCalculationItem>();
            mocks.ReplayAll();

            var childCalculation = new PipingCalculation();
            
            var childGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());


            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var children = nodePresenter.GetChildNodeObjects(nodeData).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationContext = (PipingCalculationContext)children[1];
            Assert.AreSame(childCalculation, returnedCalculationContext.WrappedData);
            var returnedCalculationGroupContext = (PipingCalculationGroupContext)children[2];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            mocks.VerifyAll();
        }
    }
}
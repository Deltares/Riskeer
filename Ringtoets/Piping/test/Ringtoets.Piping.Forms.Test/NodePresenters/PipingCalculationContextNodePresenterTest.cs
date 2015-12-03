using System;
using System.ComponentModel;
using System.Linq;

using Core.Common.Controls;
using Core.Common.Utils.Collections;
using Core.Common.TestUtils;
using Core.Common.Base;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtils.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingCalculationContextNodePresenterTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
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
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            // Call
            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PipingCalculationContext>>(nodePresenter);
            Assert.AreEqual(typeof(PipingCalculationContext), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string nodeName = "<Cool name>";

            var pipingNode = mockRepository.Stub<ITreeNode>();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            var calculation = new PipingCalculation
            {
                Name = nodeName
            };
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>());

            // Call
            pipingCalculationContextNodePresenter.UpdateNode(null, pipingNode, pipingCalculationContext);

            // Assert
            Assert.AreEqual(nodeName, pipingNode.Text);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, pipingNode.Image);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {
            // Setup
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);
            
            var calculation = new PipingCalculation
            {
                Output = new PipingOutput(0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
            };
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        new[]
                                                                        {
                                                                            new RingtoetsPipingSurfaceLine()
                                                                        },
                                                                        new[]
                                                                        {
                                                                            new TestPipingSoilProfile()
                                                                        });

            // Call
            var children = pipingCalculationContextNodePresenter.GetChildNodeObjects(pipingCalculationContext).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(4, children.Length);
            Assert.AreSame(pipingCalculationContext.WrappedData.Comments, children[0]);
            var pipingInputContext = (PipingInputContext)children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedPipingInput);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSoilProfiles, pipingInputContext.AvailablePipingSoilProfiles);
            Assert.AreSame(pipingCalculationContext.WrappedData.Output, children[2]);
            Assert.AreSame(pipingCalculationContext.WrappedData.CalculationReport, children[3]);
        }

        [Test]
        public void GetChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            var pipingCalculationContext = new PipingCalculationContext(new PipingCalculation(),
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>());
            
            // Precondition
            Assert.IsFalse(pipingCalculationContext.WrappedData.HasOutput);

            // Call
            var children = pipingCalculationContextNodePresenter.GetChildNodeObjects(pipingCalculationContext).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(4, children.Length);
            Assert.AreSame(pipingCalculationContext.WrappedData.Comments, children[0]);
            var pipingInputContext = (PipingInputContext)children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedPipingInput);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSoilProfiles, pipingInputContext.AvailablePipingSoilProfiles);

            Assert.IsInstanceOf<EmptyPipingOutput>(children[2]);
            Assert.IsInstanceOf<EmptyPipingCalculationReport>(children[3]);
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            // Call
            var renameAllowed = pipingCalculationContextNodePresenter.CanRenameNode(nodeMock);

            // Assert
            Assert.IsTrue(renameAllowed);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnTrue()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            // Call
            var renameAllowed = pipingCalculationContextNodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsTrue(renameAllowed);
            mockRepository.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeRenamed_Always_SetNewNameToPipingCalculation()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            var calculation = new PipingCalculation
            {
                Name = "<Original name>"
            };
            var pipingCalculationsInputs = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>());
            pipingCalculationsInputs.Attach(observerMock);

            // Call
            const string newName = "<Insert New Name Here>";
            pipingCalculationContextNodePresenter.OnNodeRenamed(pipingCalculationsInputs, newName);

            // Assert
            Assert.AreEqual(newName, calculation.Name);
            mockRepository.VerifyAll();
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            // Call
            pipingCalculationContextNodePresenter.OnNodeChecked(nodeMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanDrag_Always_ReturnMove()
        {
            // Setup
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            var nodeData = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            // Call
            DragOperations dragAllowed = pipingCalculationContextNodePresenter.CanDrag(nodeData);

            // Assert
            Assert.AreEqual(DragOperations.Move, dragAllowed);
        }

        [Test]
        public void CanDrop_DefaultBehavior_ReturnNone()
        {
            // Setup
            var sourceMock = mockRepository.StrictMock<ITreeNode>();
            var targetMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationContext>(new PipingCalculation(),
                                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                               Enumerable.Empty<PipingSoilProfile>());
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            // Call
            DragOperations dropAllowed = pipingCalculationContextNodePresenter.CanDrop(dataMock, sourceMock, targetMock, DragOperations.Move);

            // Assert
            Assert.AreEqual(DragOperations.None, dropAllowed);
            mockRepository.VerifyAll(); // Expect no calls on mockRepository.
        }

        [Test]
        public void CanInsert_DefaultBehavior_ReturnFalse()
        {
            // Setup
            var sourceMock = mockRepository.StrictMock<ITreeNode>();
            var targetMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationContext>(new PipingCalculation(),
                                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                               Enumerable.Empty<PipingSoilProfile>());
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            // Call
            bool insertionAllowed = pipingCalculationContextNodePresenter.CanInsert(dataMock, sourceMock, targetMock);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnDragDrop_Always_DoNothing()
        {
            // Setup
            var dataMockOwner = mockRepository.StrictMock<object>();
            var target = mockRepository.StrictMock<PipingCalculationContext>(new PipingCalculation(),
                                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                               Enumerable.Empty<PipingSoilProfile>());
            var dataMock = mockRepository.StrictMock<object>();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            // Call
            pipingCalculationContextNodePresenter.OnDragDrop(dataMock, dataMockOwner, target, DragOperations.Move, 2);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnNodeSelected_Always_DoNothing()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<PipingCalculationContext>(new PipingCalculation(),
                                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                               Enumerable.Empty<PipingSoilProfile>());
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            // Call
            pipingCalculationContextNodePresenter.OnNodeSelected(dataMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_PipingCalculationWithoutOutput_ContextMenuItemClearOutputDisabled()
        {
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation();
            var nodeData = new PipingCalculationContext(calculation,
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, nodeData);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, PipingFormsResources.PipingCalculationItem_Validate, null, PipingFormsResources.ValidationIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, PipingFormsResources.Calculate, null, PipingFormsResources.Play);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, PipingFormsResources.Clear_output, PipingFormsResources.ClearOutput_No_output_to_clear, RingtoetsCommonFormsResources.ClearIcon, false);
        }

        [Test]
        public void GetContextMenu_PipingCalculationWithOutput_ContextMenuItemClearOutputEnabled()
        {
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation
            {
                Output = new TestPipingOutput()
            };
            var nodeData = new PipingCalculationContext(calculation,
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, nodeData);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, PipingFormsResources.PipingCalculationItem_Validate, null, PipingFormsResources.ValidationIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, PipingFormsResources.Calculate, null, PipingFormsResources.Play);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, PipingFormsResources.Clear_output, null, RingtoetsCommonFormsResources.ClearIcon);
        }
        
        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.Stub<IContextMenuBuilder>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mockRepository.ReplayAll();

            var nodeData = new PipingCalculationContext(new PipingCalculation(), 
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, nodeData);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var eventArgsMock = mockRepository.StrictMock<PropertyChangedEventArgs>("");
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            var dataMock = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = pipingCalculationContextNodePresenter;

            // Call
            nodePresenter.OnPropertyChanged(dataMock, nodeMock, eventArgsMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnCollectionChange_Always_DoNothing()
        {
            // Setup
            var eventArgsMock = mockRepository.StrictMock<NotifyCollectionChangingEventArgs>();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            var dataMock = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = pipingCalculationContextNodePresenter;

            // Call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRemove_ParentIsPipingCalculationGroupWithCalculation_ReturnTrue(bool groupNameEditable)
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            
            mockRepository.ReplayAll();

            var calculationToBeRemoved = new PipingCalculation();
            var group = new PipingCalculationGroup("", groupNameEditable);
            group.Children.Add(calculationToBeRemoved);

            var calculationContext = new PipingCalculationContext(calculationToBeRemoved,
                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                  Enumerable.Empty<PipingSoilProfile>());
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool removalAllowed = nodePresenter.CanRemove(groupContext, calculationContext);

            // Assert
            Assert.IsTrue(removalAllowed);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRemove_ParentIsPipingCalculationGroupWithoutCalculation_ReturnFalse(bool groupNameEditable)
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var calculationToBeRemoved = new PipingCalculation();
            var group = new PipingCalculationGroup("", groupNameEditable);

            var calculationContext = new PipingCalculationContext(calculationToBeRemoved,
                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                  Enumerable.Empty<PipingSoilProfile>());
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool removalAllowed = nodePresenter.CanRemove(groupContext, calculationContext);

            // Assert
            Assert.IsFalse(removalAllowed);
        }

        [Test]
        public void CanRemove_EverythingElse_ReturnFalse()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<object>();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            var nodeMock = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = pipingCalculationContextNodePresenter;

            // Call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void RemoveNodeData_ParentIsPipingCalculationGroupContext_RemoveCalculationFromGroup(bool groupNameEditable)
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            var elementToBeRemoved = new PipingCalculation();

            var group = new PipingCalculationGroup();
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new PipingCalculation());
            group.Attach(observer);

            var calculationContext = new PipingCalculationContext(elementToBeRemoved,
                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                  Enumerable.Empty<PipingSoilProfile>());
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = pipingCalculationContextNodePresenter;

            // Precondition
            Assert.IsTrue(nodePresenter.CanRemove(groupContext, calculationContext));
            Assert.AreEqual(2, group.Children.Count);

            // Call
            bool removalSuccesful = nodePresenter.RemoveNodeData(groupContext, calculationContext);

            // Assert
            Assert.IsTrue(removalSuccesful);
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveNodeData_EverythingElse_ThrowsInvalidOperationException()
        {
            // Setup
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var pipingCalculationContextNodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProvider);

            // Call
            TestDelegate removeAction = () => pipingCalculationContextNodePresenter.RemoveNodeData(null, null);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(removeAction);
            var expectedMessage = string.Format("Kan knoop uit boom van type {0} niet verwijderen.", pipingCalculationContextNodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenInvalidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObserversAndLogMessageAdded()
        {
            // Given
            var contextMenuBuilderProviderMock = mockRepository.DynamicMock<IContextMenuBuilderProvider>();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Once();
            var treeNodeMock = mockRepository.StrictMock<ITreeNode>();
            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(treeNodeMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            var expectedValidationMessageCount = 2; // No surfaceline or soil profile selected for calculation
            var calculateContextMenuItemIndex = 1;

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation();
            calculation.Attach(observer);

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock)
            {
                RunActivityAction = activity =>
                {
                    activity.Run();
                    activity.Finish();
                }
            };

            var contextMenuAdapter = nodePresenter.GetContextMenu(treeNodeMock, new PipingCalculationContext(calculation,
                                                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingSoilProfile>()));


            // When
            Action action = () =>
            {
                contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();
            };

            // Then
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.GetEnumerator();
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Nieuwe berekening' gestart om: ", msgs.Current);
                for (int i = 0; i < expectedValidationMessageCount; i++)
                {
                    Assert.IsTrue(msgs.MoveNext());
                    StringAssert.StartsWith("Validatie mislukt: ", msgs.Current);
                }
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Nieuwe berekening' beëindigd om: ", msgs.Current);
            });
            Assert.IsNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenInvalidPipingCalculation_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            var contextMenuBuilderProviderMock = mockRepository.DynamicMock<IContextMenuBuilderProvider>();
            var observer = mockRepository.StrictMock<IObserver>();
            var treeNodeMock = mockRepository.StrictMock<ITreeNode>();
            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(treeNodeMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            var expectedValidationMessageCount = 2; // No surfaceline or soil profile selected for calculation
            var expectedStatusMessageCount = 2;
            var expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;

            var validateContextMenuItemIndex = 0;
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation();
            calculation.Attach(observer);

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            var contextMenuAdapter = nodePresenter.GetContextMenu(treeNodeMock, new PipingCalculationContext(calculation,
                                                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingSoilProfile>()));

            // When
            Action action = () => contextMenuAdapter.Items[validateContextMenuItemIndex].PerformClick();

            // Then
            TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenValidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObservers()
        {
            // Given
            var contextMenuBuilderProviderMock = mockRepository.DynamicMock<IContextMenuBuilderProvider>();
            var observer = mockRepository.StrictMock<IObserver>();
            var treeNodeMock = mockRepository.StrictMock<ITreeNode>();
            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(treeNodeMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var calculateContextMenuItemIndex = 1;
            var calculation = new PipingCalculation();
            var validPipingInput = new TestPipingInput();
            calculation.InputParameters.AssessmentLevel = validPipingInput.AssessmentLevel;
            calculation.InputParameters.BeddingAngle = validPipingInput.BeddingAngle;
            calculation.InputParameters.DampingFactorExit.Mean = validPipingInput.DampingFactorExit;
            calculation.InputParameters.DarcyPermeability.Mean = validPipingInput.DarcyPermeability;
            calculation.InputParameters.Diameter70.Mean = validPipingInput.Diameter70;
            calculation.InputParameters.ExitPointXCoordinate = validPipingInput.ExitPointXCoordinate;
            calculation.InputParameters.Gravity = validPipingInput.Gravity;
            calculation.InputParameters.MeanDiameter70 = validPipingInput.MeanDiameter70;
            calculation.InputParameters.PhreaticLevelExit.Mean = validPipingInput.PhreaticLevelExit;
            calculation.InputParameters.PiezometricHeadExit = validPipingInput.PiezometricHeadExit;
            calculation.InputParameters.PiezometricHeadPolder = validPipingInput.PiezometricHeadPolder;
            calculation.InputParameters.SandParticlesVolumicWeight = validPipingInput.SandParticlesVolumicWeight;
            calculation.InputParameters.SeepageLength.Mean = validPipingInput.SeepageLength;
            calculation.InputParameters.SellmeijerModelFactor = validPipingInput.SellmeijerModelFactor;
            calculation.InputParameters.SellmeijerReductionFactor = validPipingInput.SellmeijerReductionFactor;
            calculation.InputParameters.ThicknessAquiferLayer.Mean = validPipingInput.ThicknessAquiferLayer;
            calculation.InputParameters.ThicknessCoverageLayer.Mean = validPipingInput.ThicknessCoverageLayer;
            calculation.InputParameters.UpliftModelFactor = validPipingInput.UpliftModelFactor;
            calculation.InputParameters.WaterVolumetricWeight = validPipingInput.WaterVolumetricWeight;
            calculation.InputParameters.WaterKinematicViscosity = validPipingInput.WaterKinematicViscosity;
            calculation.InputParameters.WhitesDragCoefficient = validPipingInput.WhitesDragCoefficient;
            calculation.InputParameters.SurfaceLine = validPipingInput.SurfaceLine;
            calculation.InputParameters.SoilProfile = validPipingInput.SoilProfile;


            calculation.Attach(observer);
            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);


            var contextMenuAdapter = nodePresenter.GetContextMenu(treeNodeMock, new PipingCalculationContext(calculation,
                                                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingSoilProfile>()));
            nodePresenter.RunActivityAction = activity =>
            {
                activity.Run();
                activity.Finish();
            };

            // When
            Action action = () =>
            {
                contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();
            };

            // Then
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.GetEnumerator();
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Nieuwe berekening' gestart om: ", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Nieuwe berekening' beëindigd om: ", msgs.Current);

                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Berekening van 'Nieuwe berekening' gestart om: ", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Berekening van 'Nieuwe berekening' beëindigd om: ", msgs.Current);
            });
            Assert.IsNotNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationWithOutput_WhenClearingOutputFromContextMenu_ThenPipingCalculationOutputClearedAndNotified()
        {
            // Given
            var contextMenuBuilderProviderMock = mockRepository.DynamicMock<IContextMenuBuilderProvider>();
            var observer = mockRepository.StrictMock<IObserver>();
            var treeNodeMock = mockRepository.StrictMock<ITreeNode>();
            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(treeNodeMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            int clearOutputItemPosition = 2;
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation();
            calculation.Output = new TestPipingOutput();
            calculation.Attach(observer);
            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            var contextMenuAdapter = nodePresenter.GetContextMenu(treeNodeMock, new PipingCalculationContext(calculation,
                                                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingSoilProfile>()));

            // When
            contextMenuAdapter.Items[clearOutputItemPosition].PerformClick();

            // Then
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }
    }
}
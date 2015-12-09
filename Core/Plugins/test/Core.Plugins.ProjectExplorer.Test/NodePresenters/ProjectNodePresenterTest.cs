using System;
using Core.Common.Base.Data;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Plugins.ProjectExplorer.NodePresenters;
using Core.Plugins.ProjectExplorer.Properties;
using NUnit.Framework;
using Rhino.Mocks;

using CommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.ProjectExplorer.Test.NodePresenters
{
    [TestFixture]
    public class ProjectNodePresenterTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ProjectNodePresenter(null, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_NoCommandHandlerProvider_ArgumentNullException()
        {
            // Setup
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ProjectNodePresenter(contextMenuBuilderProviderMock, null);


            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CommonGuiResources.NodePresenter_CommandHandler_required, message);
            StringAssert.EndsWith("commandHandler", message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithParamsSet_DoesNotThrow()
        {
            // Setup
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var guiCommandHandler = mocks.StrictMock<IGuiCommandHandler>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ProjectNodePresenter(contextMenuBuilderProviderMock, guiCommandHandler);
            
            // Assert
            Assert.DoesNotThrow(test);

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var guiCommandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
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

            mocks.ReplayAll();

            var nodePresenter = new ProjectNodePresenter(contextMenuBuilderProviderMock, guiCommandHandlerMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new Project());

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_Always_AddsAddItem()
        {
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var guiCommandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mocks.ReplayAll();

            var nodePresenter = new ProjectNodePresenter(contextMenuBuilderProviderMock, guiCommandHandlerMock);

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new Project());

            // Assert
            mocks.VerifyAll(); 
            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.AddItem, null, Resources.plus);
        }

        [Test]
        public void CanDrag_Always_ReturnNone()
        {
            // Setup
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var commandHandler = mocks.StrictMock<IGuiCommandHandler>();

            mocks.ReplayAll();

            var nodeData = new Project();

            var nodePresenter = new ProjectNodePresenter(contextMenuBuilderProvider, commandHandler);

            // Call
            var draggingOperations = nodePresenter.CanDrag(nodeData);

            // Assert
            Assert.AreEqual(DragOperations.None, draggingOperations);
            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_Always_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var commandHandler = mocks.StrictMock<IGuiCommandHandler>();

            var node = mocks.StrictMock<ITreeNode>();
            var targetNode = mocks.StrictMock<ITreeNode>();

            mocks.ReplayAll();

            var nodeData = new Project();

            var nodePresenter = new ProjectNodePresenter(contextMenuBuilderProvider, commandHandler);

            // Call
            var insertionAllowed = nodePresenter.CanInsert(nodeData, node, targetNode);

            // Assert
            Assert.IsFalse(insertionAllowed);
        }

        [Test]
        public void CanDrop_Always_ReturnNone()
        {
            // Setup
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var commandHandler = mocks.StrictMock<IGuiCommandHandler>();

            var node = mocks.StrictMock<ITreeNode>();
            var targetNode = mocks.StrictMock<ITreeNode>();

            mocks.ReplayAll();

            var nodeData = new Project();

            var nodePresenter = new ProjectNodePresenter(contextMenuBuilderProvider, commandHandler);

            // Call
            var droppingOperations = nodePresenter.CanDrop(nodeData, node, targetNode, DragOperations.Move);

            // Assert
            Assert.AreEqual(DragOperations.None, droppingOperations);
            mocks.VerifyAll();
        }
    }
}
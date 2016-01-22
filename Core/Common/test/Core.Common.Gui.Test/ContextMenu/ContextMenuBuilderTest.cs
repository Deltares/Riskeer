using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class ContextMenuBuilderTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_NoTreeNode_ThrowsContextMenuBuilderException()
        {
            // Call
            TestDelegate test = () => new ContextMenuBuilder(null, null, null);

            // Assert
            var message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
        }

        [Test]
        public void Constructor_NoGui_ThrowsContextMenuBuilderException()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var importExportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(null, importExportHandlerMock, treeNodeMock);

            // Assert
            var message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoExportImportHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var guiCommandHandler = mocks.StrictMock<IGuiCommandHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(guiCommandHandler, null, treeNodeMock);

            // Assert
            var message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ParamsSet_DoesNotThrow()
        {
            // Setup
            var guiCommandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(guiCommandHandlerMock, exportImportHandlerMock, treeNodeMock);
            
            // Assert
            Assert.DoesNotThrow(test);

            mocks.VerifyAll();
        }

        [Test]
        public void Build_NothingAdded_EmptyContextMenu()
        {
            // Setup
            var guiCommandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(guiCommandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            CollectionAssert.IsEmpty(result.Items);

            mocks.VerifyAll();
        }

        [Test]
        public void AddRenameItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();

            var treeNodePresenterMock = mocks.StrictMock<ITreeNodePresenter>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();

            treeNodeMock.Expect(tn => tn.Presenter).Return(treeNodePresenterMock);
            treeNodePresenterMock.Expect(tn => tn.CanRenameNode(treeNodeMock)).Return(true);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.AddRenameItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Rename, Resources.Rename_ToolTip, Resources.RenameIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void AddDeleteItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();

            var treeNodePresenterMock = mocks.StrictMock<ITreeNodePresenter>();
            var treeParentNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();

            treeNodeMock.Expect(tn => tn.Parent).Return(treeParentNodeMock);
            treeNodeMock.Expect(tn => tn.Presenter).Return(treeNodePresenterMock);
            var nodeData = new object();
            var parentData = new object();
            treeNodeMock.Expect(tn => tn.Tag).Return(nodeData);
            treeParentNodeMock.Expect(tn => tn.Tag).Return(parentData);
            treeNodePresenterMock.Expect(tn => tn.CanRemove(parentData, nodeData)).Return(true);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.AddDeleteItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Delete, Resources.Delete_ToolTip, Resources.DeleteIcon);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExpandAllItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            IList<TreeNode> children = new List<TreeNode>();
            if (hasChildren)
            {
                children.Add(treeNodeMock);
            }
            treeNodeMock.Expect(tn => tn.Nodes).Return(children);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.AddExpandAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Expand_all, Resources.Expand_all_ToolTip, Resources.ExpandAllIcon, hasChildren);

            mocks.VerifyAll();
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddCollapseAllItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            IList<TreeNode> children = new List<TreeNode>();
            if (hasChildren)
            {
                children.Add(treeNodeMock);
            }
            treeNodeMock.Expect(tn => tn.Nodes).Return(children);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.AddCollapseAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Collapse_all, Resources.Collapse_all_ToolTip, Resources.CollapseAllIcon, hasChildren);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddOpenItem_WhenBuild_ItemAddedToContextMenu(bool hasViewForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var treeNodeMock = mocks.Stub<TreeNode>();
            var nodeData = new object();

            commandHandlerMock.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(hasViewForNodeData);

            mocks.ReplayAll();

            treeNodeMock.Tag = nodeData;

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.AddOpenItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Open, Resources.Open_ToolTip, Resources.OpenIcon, hasViewForNodeData);

            mocks.VerifyAll();
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExportItem_WhenBuild_ItemAddedToContextMenu(bool hasExportersForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var nodeData = new object();
            var treeNodeMock = mocks.Stub<TreeNode>();
            exportImportHandlerMock.Expect(ch => ch.CanExportFrom(nodeData)).Return(hasExportersForNodeData);

            mocks.ReplayAll();

            treeNodeMock.Tag = nodeData;

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.AddExportItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Export, Resources.Export_ToolTip, Resources.ExportIcon, hasExportersForNodeData);

            mocks.VerifyAll();
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItem_WhenBuild_ItemAddedToContextMenu(bool hasImportersForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var nodeData = new object();
            var treeNodeMock = mocks.Stub<TreeNode>();
            exportImportHandlerMock.Expect(ch => ch.CanImportOn(nodeData)).Return(hasImportersForNodeData);

            mocks.ReplayAll();

            treeNodeMock.Tag = nodeData;

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.AddImportItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Import, Resources.Import_ToolTip, Resources.ImportIcon, hasImportersForNodeData);

            mocks.VerifyAll();
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddPropertiesItem_WhenBuild_ItemAddedToContextMenu(bool hasPropertiesForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var nodeData = new object();
            var treeNodeMock = mocks.Stub<TreeNode>();

            commandHandlerMock.Expect(ch => ch.CanShowPropertiesFor(nodeData)).Return(hasPropertiesForNodeData);

            mocks.ReplayAll();

            treeNodeMock.Tag = nodeData;

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);
            
            // Call
            var result = builder.AddPropertiesItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Properties, Resources.Properties_ToolTip, Resources.PropertiesIcon, hasPropertiesForNodeData);

            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);
            var item = new StrictContextMenuItem(null,null,null,null);

            // Call
            var result = builder.AddCustomItem(item).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);
            
            Assert.AreSame(item, result.Items[0]);

            mocks.VerifyAll();
        }

        [Test]
        public void AddSeparator_NoOtherItemsWhenBuild_EmptyContextMenu()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            // Call
            var result = builder.AddSeparator().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.IsEmpty(result.Items);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorAddedAtStart_SeparatorsNotAdded(int count)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            var someItem = new StrictContextMenuItem(null, null, null, null);

            // Call
            for (int i = 0; i < count; i++)
            {
                builder.AddSeparator();
            }
            var result = builder.AddCustomItem(someItem).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            Assert.IsInstanceOf<ToolStripMenuItem>(result.Items[0]);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeperatorsAddedInBetweenItems_OneSeparatorAdded(int count)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            var someItem = new StrictContextMenuItem(null, null, null, null);
            var someOtherItem = new StrictContextMenuItem(null, null, null, null);
            
            builder.AddCustomItem(someItem);
            
            // Call
            for (int i = 0; i < count; i++)
            {
                builder.AddSeparator();
            }
            var result = builder.AddCustomItem(someOtherItem).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(3, result.Items.Count);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[1]);
            Assert.IsInstanceOf<ToolStripMenuItem>(result.Items[2]);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorsAddedAtEnd_SeparatorsNotAdded(int count)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(commandHandlerMock, exportImportHandlerMock, treeNodeMock);

            builder.AddCustomItem(new StrictContextMenuItem(null, null, null, null));

            // Call
            for (int i = 0; i < count; i++)
            {
                builder.AddSeparator();
            }
            var result = builder.Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            mocks.VerifyAll();
        }
    }
}
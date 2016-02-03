using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

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
        public void Constructor_NoApplicationFeatureCommands_ThrowsContextMenuBuilderException()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var importExportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(null, importExportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMock, null, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

            // Assert
            var message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoViewCommands_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommandsMockMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMockMock, exportImportHandlerMock, null, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

            // Assert
            var message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoTreeNode_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var importExportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMock, importExportHandlerMock, viewCommandsMock, null, treeNodeInfoMock, treeViewControlMock);

            // Assert
            var message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
        }

        [Test]
        public void Constructor_NoTreeNodeInfo_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var importExportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMock, importExportHandlerMock, viewCommandsMock, treeNodeMock, null, treeViewControlMock);

            // Assert
            var message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
        }

        [Test]
        public void Constructor_NoTreeViewControl_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var importExportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMock, importExportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, null);

            // Assert
            var message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
        }

        [Test]
        public void Constructor_ParamsSet_DoesNotThrow()
        {
            // Setup
            var applicationFeatureCommandsMockMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMockMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

            // Assert
            Assert.DoesNotThrow(test);

            mocks.VerifyAll();
        }

        [Test]
        public void Build_NothingAdded_EmptyContextMenu()
        {
            // Setup
            var applicationFeatureCommandsMockMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMockMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            treeNodeInfoMock.CanRename = treeNode => treeNode == treeNodeMock;

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var treeParentNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();

            treeNodeMock.Stub(tn => tn.Parent).Return(treeParentNodeMock);
            var nodeData = new object();
            var parentData = new object();
            treeNodeMock.Expect(tn => tn.Tag).Return(nodeData);
            treeParentNodeMock.Expect(tn => tn.Tag).Return(parentData);

            treeNodeInfoMock.CanRemove = (nd, pnd) => nd == nodeData && pnd == parentData;

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var treeNode= new TreeNode();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();

            if (hasChildren)
            {
                treeNode.Nodes.Add(new TreeNode());
            }

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNode, treeNodeInfoMock, treeViewControlMock);

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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var treeNode = new TreeNode();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();

            if (hasChildren)
            {
                treeNode.Nodes.Add(new TreeNode());
            }

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNode, treeNodeInfoMock, treeViewControlMock);

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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var treeNodeMock = mocks.Stub<TreeNode>();
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();
            var nodeData = new object();

            viewCommandsMock.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(hasViewForNodeData);

            mocks.ReplayAll();

            treeNodeMock.Tag = nodeData;

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var nodeData = new object();
            var treeNodeMock = mocks.Stub<TreeNode>();
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();
            exportImportHandlerMock.Expect(ch => ch.CanExportFrom(nodeData)).Return(hasExportersForNodeData);

            mocks.ReplayAll();

            treeNodeMock.Tag = nodeData;

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var nodeData = new object();
            var treeNodeMock = mocks.Stub<TreeNode>();
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();
            exportImportHandlerMock.Expect(ch => ch.CanImportOn(nodeData)).Return(hasImportersForNodeData);

            mocks.ReplayAll();

            treeNodeMock.Tag = nodeData;

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var nodeData = new object();
            var treeNodeMock = mocks.Stub<TreeNode>();
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();

            applicationFeatureCommandsMock.Expect(ch => ch.CanShowPropertiesFor(nodeData)).Return(hasPropertiesForNodeData);

            mocks.ReplayAll();

            treeNodeMock.Tag = nodeData;

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);
            
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
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);
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
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
            var treeNodeInfoMock = mocks.Stub<TreeNodeInfo>();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, treeNodeMock, treeNodeInfoMock, treeViewControlMock);

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
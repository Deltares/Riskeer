using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtils;
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
        public void Constructor_NoTreeNode_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ContextMenuBuilder(null, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

        }

        [Test]
        public void Constructor_NoGui_NewInstance()
        {
            // Call
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Assert
            Assert.IsInstanceOf<ContextMenuBuilder>(builder);
        } 

        [Test]
        public void Constructor_Gui_NewInstance()
        {
            // Call
            var builder = new ContextMenuBuilder(MockRepository.GenerateMock<IGuiCommandHandler>(), MockRepository.GenerateMock<ITreeNode>());

            // Assert
            Assert.IsInstanceOf<ContextMenuBuilder>(builder);
        }

        [Test]
        public void Build_NothingAdded_EmptyContextMenu()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            CollectionAssert.IsEmpty(result.Items);
        } 

        [Test]
        public void AddDeleteItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddDeleteItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Delete, Resources.Delete_ToolTip, Resources.DeleteIcon);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExpandAllItem_WhenBuild_ItemAddedToContextMenu(bool enabled)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            IList<ITreeNode> childs = new List<ITreeNode>();
            if (enabled)
            {
                childs.Add(treeNodeMock);
            }
            treeNodeMock.Expect(tn => tn.Nodes).Return(childs);
            var builder = new ContextMenuBuilder(null, treeNodeMock);

            mocks.ReplayAll();

            // Call
            var result = builder.AddExpandAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Expand_all, Resources.Expand_all_ToolTip, Resources.ExpandAllIcon, enabled);
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddCollapseAllItem_WhenBuild_ItemAddedToContextMenu(bool enabled)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            IList<ITreeNode> childs = new List<ITreeNode>();
            if (enabled)
            {
                childs.Add(treeNodeMock);
            }
            treeNodeMock.Expect(tn => tn.Nodes).Return(childs);
            var builder = new ContextMenuBuilder(null, treeNodeMock);

            mocks.ReplayAll();

            // Call
            var result = builder.AddCollapseAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Collapse_all, Resources.Collapse_all_ToolTip, Resources.CollapseAllIcon, enabled);
        }

        [Test]
        public void AddOpenItem_WithoutGuiWhenBuild_ContextMenuEmpty()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddOpenItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.IsEmpty(result.Items);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddOpenItem_WithGuiWhenBuild_ItemAddedToContextMenu(bool enabled)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            commandHandlerMock.Expect(ch => ch.CanOpenDefaultViewForSelection()).Return(enabled);
            var builder = new ContextMenuBuilder(commandHandlerMock, MockRepository.GenerateMock<ITreeNode>());

            mocks.ReplayAll();

            // Call
            var result = builder.AddOpenItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Open, Resources.Open_ToolTip, Resources.OpenIcon, enabled);

            mocks.VerifyAll();
        } 

        [Test]
        public void AddExportItem_WithoutGuiWhenBuild_ContextMenuEmpty()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddExportItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.IsEmpty(result.Items);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExportItem_WithGuiWhenBuild_ItemAddedToContextMenu(bool enabled)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            commandHandlerMock.Expect(ch => ch.CanExportFromGuiSelection()).Return(enabled);
            var builder = new ContextMenuBuilder(commandHandlerMock, MockRepository.GenerateMock<ITreeNode>());

            mocks.ReplayAll();

            // Call
            var result = builder.AddExportItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Export, Resources.Export_ToolTip, Resources.ExportIcon, enabled);

            mocks.VerifyAll();
        } 

        [Test]
        public void AddImportItem_WithoutGuiWhenBuild_ContextMenuEmpty()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddImportItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.IsEmpty(result.Items);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItem_WithGuiWhenBuild_ItemAddedToContextMenu(bool enabled)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            commandHandlerMock.Expect(ch => ch.CanImportToGuiSelection()).Return(enabled);
            var builder = new ContextMenuBuilder(commandHandlerMock, MockRepository.GenerateMock<ITreeNode>());

            mocks.ReplayAll();

            // Call
            var result = builder.AddImportItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Import, Resources.Import_ToolTip, Resources.ImportIcon, enabled);

            mocks.VerifyAll();
        } 

        [Test]
        public void AddPropertiesItem_WithoutGuiWhenBuild_ContextMenuEmpty()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddPropertiesItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.IsEmpty(result.Items);
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddPropertiesItem_WithGuiWhenBuild_ItemAddedToContextMenu(bool enabled)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesForGuiSelection()).Return(enabled);
            var builder = new ContextMenuBuilder(commandHandlerMock, MockRepository.GenerateMock<ITreeNode>());

            mocks.ReplayAll();

            // Call
            var result = builder.AddPropertiesItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Properties, Resources.Properties_ToolTip, Resources.PropertiesIcon, enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomItem_NoItemWhenBuild_ContextMenuEmpty()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddCustomItem(null).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.IsEmpty(result.Items);
        }

        [Test]
        public void AddCustomItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());
            var item = new StrictContextMenuItem(null,null,null,null);
            // Call
            var result = builder.AddCustomItem(item).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);
            
            Assert.AreSame(item, result.Items[0]);
        }

        [Test]
        public void AddSeparator_NoItemsWhenBuild_EmptyContextMenu()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddSeparator().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.IsEmpty(result.Items);
        }

        [Test]
        public void AddSeparator_ItemAddedWhenBuild_SeparatorAdded()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            builder.AddCustomItem(new StrictContextMenuItem(null, null, null, null));

            // Call
            var result = builder.AddSeparator().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(2, result.Items.Count);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[1]);
        }

        [Test]
        public void AddSeparator_ItemAndSeparatorAddedWhenBuild_NoSeparatorAdded()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            builder.AddCustomItem(new StrictContextMenuItem(null, null, null, null)).AddSeparator();

            // Call
            var result = builder.AddSeparator().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(2, result.Items.Count);
        }
    }
}
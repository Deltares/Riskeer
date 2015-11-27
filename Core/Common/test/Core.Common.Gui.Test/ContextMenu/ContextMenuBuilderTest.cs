using System;
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
            var builder = new ContextMenuBuilder(MockRepository.GenerateMock<IGui>(), MockRepository.GenerateMock<ITreeNode>());

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
        public void AddExpandAllItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddExpandAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            var expandAllItem = result.Items[0];
            Assert.AreEqual(Resources.Expand_all, expandAllItem.Text);
            Assert.AreEqual(Resources.Expand_all_ToolTip, expandAllItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExpandAllIcon, expandAllItem.Image);
        } 

        [Test]
        public void AddCollapseAllItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var builder = new ContextMenuBuilder(null, MockRepository.GenerateMock<ITreeNode>());

            // Call
            var result = builder.AddCollapseAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            var collapseAll = result.Items[0];
            Assert.AreEqual(Resources.Collapse_all, collapseAll.Text);
            Assert.AreEqual(Resources.Collapse_all_ToolTip, collapseAll.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.CollapseAllIcon, collapseAll.Image);
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
        public void AddOpenItem_WithGuiWhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var guiStub = mocks.StrictMock<IGui>();
            guiStub.Expect(g => g.Plugins).Return(new GuiPlugin[0]);
            var builder = new ContextMenuBuilder(guiStub, MockRepository.GenerateMock<ITreeNode>());

            mocks.ReplayAll();

            // Call
            var result = builder.AddOpenItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            var export = result.Items[0];
            Assert.AreEqual(Resources.Open, export.Text);
            Assert.AreEqual(Resources.Open_ToolTip, export.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.OpenIcon, export.Image);

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
        public void AddExportItem_WithGuiWhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var guiStub = mocks.StrictMock<IGui>();
            guiStub.Expect(g => g.ApplicationCore).Return(new ApplicationCore());
            var builder = new ContextMenuBuilder(guiStub, MockRepository.GenerateMock<ITreeNode>());

            mocks.ReplayAll();

            // Call
            var result = builder.AddExportItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            var export = result.Items[0];
            Assert.AreEqual(Resources.Export, export.Text);
            Assert.AreEqual(Resources.Export_ToolTip, export.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExportIcon, export.Image);

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
        public void AddImportItem_WithGuiWhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var guiStub = mocks.StrictMock<IGui>();
            guiStub.Expect(g => g.ApplicationCore).Return(new ApplicationCore());
            var builder = new ContextMenuBuilder(guiStub, MockRepository.GenerateMock<ITreeNode>());

            mocks.ReplayAll();

            // Call
            var result = builder.AddImportItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            var import = result.Items[0];
            Assert.AreEqual(Resources.Import, import.Text);
            Assert.AreEqual(Resources.Import_ToolTip, import.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, import.Image);

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
        public void AddPropertiesItem_WithGuiWhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var guiStub = mocks.StrictMock<IGui>();
            guiStub.Expect(g => g.Plugins).Return(new GuiPlugin[0]);
            guiStub.Expect(g => g.CommandHandler).Return(null);
            var builder = new ContextMenuBuilder(guiStub, MockRepository.GenerateMock<ITreeNode>());

            mocks.ReplayAll();

            // Call
            var result = builder.AddPropertiesItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            var properties = result.Items[0];
            Assert.AreEqual(Resources.Properties, properties.Text);
            Assert.AreEqual(Resources.Properties_ToolTip, properties.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.PropertiesIcon, properties.Image);

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
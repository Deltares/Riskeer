using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;
using Core.Plugins.ProjectExplorer.NodePresenters;
using Core.Plugins.ProjectExplorer.Properties;
using NUnit.Framework;
using Rhino.Mocks;

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
        public void GetContextMenu_NoContextMenuProvider_ReturnsNull()
        {
            // Setup
            var presenter = new ProjectNodePresenter();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            // Call
            var result = presenter.GetContextMenu(nodeMock, new Project());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetContextMenu_MenuBuilderProvider_ReturnsFourItems()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var nodePresenter = new ProjectNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock)
            };

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new Project());

            // Assert
            Assert.AreEqual(9, result.Items.Count);

            Assert.AreEqual(Resources.AddItem, result.Items[0].Text);
            Assert.IsNull(result.Items[0].ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.plus, result.Items[0].Image);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[1]);

            ToolStripItem expandItem = result.Items[2];
            Assert.AreEqual(Common.Gui.Properties.Resources.Expand_all, expandItem.Text);
            Assert.AreEqual(Common.Gui.Properties.Resources.Expand_all_ToolTip, expandItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Common.Gui.Properties.Resources.ExpandAllIcon, expandItem.Image);

            ToolStripItem collapseItem = result.Items[3];
            Assert.AreEqual(Common.Gui.Properties.Resources.Collapse_all, collapseItem.Text);
            Assert.AreEqual(Common.Gui.Properties.Resources.Collapse_all_ToolTip, collapseItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Common.Gui.Properties.Resources.CollapseAllIcon, collapseItem.Image);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[4]);

            Assert.AreEqual(Common.Gui.Properties.Resources.Export, result.Items[5].Text);
            Assert.AreEqual(Common.Gui.Properties.Resources.Export_ToolTip, result.Items[5].ToolTipText);
            TestHelper.AssertImagesAreEqual(Common.Gui.Properties.Resources.ExportIcon, result.Items[5].Image);

            Assert.AreEqual(Common.Gui.Properties.Resources.Import, result.Items[6].Text);
            Assert.AreEqual(Common.Gui.Properties.Resources.Import_ToolTip, result.Items[6].ToolTipText);
            TestHelper.AssertImagesAreEqual(Common.Gui.Properties.Resources.ImportIcon, result.Items[6].Image);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[7]);

            Assert.AreEqual(Common.Gui.Properties.Resources.Properties, result.Items[8].Text);
            Assert.AreEqual(Common.Gui.Properties.Resources.Properties_ToolTip, result.Items[8].ToolTipText);
            TestHelper.AssertImagesAreEqual(Common.Gui.Properties.Resources.PropertiesIcon, result.Items[8].Image);
        }
    }
}
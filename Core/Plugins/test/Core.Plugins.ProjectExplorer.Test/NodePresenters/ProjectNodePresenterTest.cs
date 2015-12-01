using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;
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
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_MenuBuilderProvider_ReturnsFourItems(bool commonItemsEnabled)
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var nodePresenter = new ProjectNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock, commonItemsEnabled)
            };

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new Project());

            // Assert
            Assert.AreEqual(9, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.AddItem, null, Resources.plus);
            TestHelper.AssertContextMenuStripContainsItem(result, 2, CommonGuiResources.Expand_all, CommonGuiResources.Expand_all_ToolTip, CommonGuiResources.ExpandAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(result, 3, CommonGuiResources.Collapse_all, CommonGuiResources.Collapse_all_ToolTip, CommonGuiResources.CollapseAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(result, 5, CommonGuiResources.Import, CommonGuiResources.Import_ToolTip, CommonGuiResources.ImportIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(result, 6, CommonGuiResources.Export, CommonGuiResources.Export_ToolTip, CommonGuiResources.ExportIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(result, 8, CommonGuiResources.Properties, CommonGuiResources.Properties_ToolTip, CommonGuiResources.PropertiesIcon, commonItemsEnabled);
        }
    }
}
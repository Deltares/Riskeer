using Core.Common.Controls.TreeView;
using Core.GIS.SharpMap.Styles;
using Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.SharpMapGis.Test.Forms
{
    [TestFixture]
    public class VectorStyleNodePresenterTest
    {
        private static readonly MockRepository Mocks = new MockRepository();
        private static ITreeNodePresenter vectorStyleNodePresenter;

        [SetUp]
        public void Setup()
        {
            vectorStyleNodePresenter = new VectorStyleTreeViewNodePresenter();
            var tv = Mocks.Stub<TreeView>();
            vectorStyleNodePresenter.TreeView = tv;
        }

        [Test]
        public void CreateNode()
        {
            var node = Mocks.Stub<TreeNode>();

            var style = new VectorStyle();

            vectorStyleNodePresenter.UpdateNode(null, node, style);

            Assert.AreEqual(style, node.Tag);
            Assert.IsEmpty(node.Text);
            Assert.IsNotNull(node.Image);
        }
    }
}
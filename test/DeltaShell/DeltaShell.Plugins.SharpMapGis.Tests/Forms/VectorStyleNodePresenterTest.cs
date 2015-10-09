using DelftTools.Controls;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms
{
    [TestFixture]
    public class VectorStyleNodePresenterTest
    {
        private static readonly MockRepository mocks = new MockRepository();
        private static ITreeNodePresenter vectorStyleNodePresenter;

        [SetUp]
        public void Setup()
        {
            vectorStyleNodePresenter = new VectorStyleTreeViewNodePresenter();
            var tv = mocks.Stub<ITreeView>();
            vectorStyleNodePresenter.TreeView = tv;
        }

        [Test]
        public void CreateNode()
        {
            var node = mocks.Stub<ITreeNode>();

            var style = new VectorStyle();

            vectorStyleNodePresenter.UpdateNode(null, node, style);

            Assert.AreEqual(style, node.Tag);
            Assert.IsEmpty(node.Text);
            Assert.IsNotNull(node.Image);
        }
    }
}
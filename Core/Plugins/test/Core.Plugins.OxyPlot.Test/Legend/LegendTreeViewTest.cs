using System.Linq;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class LegendTreeViewTest
    {
        [Test]
        public void DefaultConstructor_SetsTwoNodePresenters()
        {
            // Call
            var view = new LegendTreeView();

            // Assert
            Assert.AreEqual(2, view.NodePresenters.Count());
            Assert.IsInstanceOf<ChartDataNodePresenter>(view.NodePresenters.ElementAt(0));
            Assert.IsInstanceOf<ChartNodePresenter>(view.NodePresenters.ElementAt(1));
            Assert.IsNull(view.ChartData);
        }
    }
}
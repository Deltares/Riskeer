using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using Core.Components.OxyPlot.Forms.Properties;
using OxyPlot.WindowsForms;
using TickStyle = OxyPlot.Axes.TickStyle;

namespace Core.Components.OxyPlot.Forms.Test
{
    [TestFixture]
    public class LinearPlotViewTest
    {
        [Test]
        public void DefaultConstructor_HasTwoLinearAxes()
        {
            // Call
            var view = new LinearPlotView();

            // Assert
            Assert.IsInstanceOf<PlotView>(view);
            Assert.AreEqual(DockStyle.Fill, view.Dock);

            var axes = view.Model.Axes;
            Assert.AreEqual(2, axes.Count);
            CollectionAssert.AllItemsAreInstancesOfType(axes, typeof(LinearAxis));
            CollectionAssert.AreEqual(new [] {Resources.BaseChart_XAxisTitle, Resources.BaseChart_YAxisTitle} , axes.Select(a => a.Title));
            CollectionAssert.AreEqual(new [] {AxisPosition.Bottom, AxisPosition.Left} , axes.Select(a => a.Position));
            CollectionAssert.AreEqual(new [] {TickStyle.None, TickStyle.None} , axes.Select(a => a.TickStyle));
            CollectionAssert.AreEqual(new [] {new[] { 0.0 }, new[] { 0.0 }} , axes.Select(a => a.ExtraGridlines));
            CollectionAssert.AreEqual(new [] {1, 1} , axes.Select(a => a.ExtraGridlineThickness));
            CollectionAssert.AreEqual(new [] {AxisLayer.AboveSeries, AxisLayer.AboveSeries} , axes.Select(a => a.Layer));
            CollectionAssert.AreEqual(new [] {LineStyle.Solid, LineStyle.Solid} , axes.Select(a => a.MajorGridlineStyle));
            CollectionAssert.AreEqual(new [] {LineStyle.Dot,LineStyle.Dot} , axes.Select(a => a.MinorGridlineStyle));
        }

        [Test]
        public void ZoomToAll_Always_InvalidatesView()
        {
            // Setup
            var form = new Form();
            var view = new LinearPlotView();
            form.Controls.Add(view);
            var invalidated = 0;
            view.Invalidated += (sender, args) => invalidated++;

            form.Show();

            // Call
            view.ZoomToAll();

            // Assert
            Assert.AreEqual(1, invalidated);
        }
    }
}
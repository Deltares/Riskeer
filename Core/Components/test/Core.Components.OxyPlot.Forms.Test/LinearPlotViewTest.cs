using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using Core.Components.OxyPlot.Forms.Properties;
using OxyPlot.Series;
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
        public void ZoomToAll_ViewInForm_AxesAreSetToOriginal()
        {
            // Setup
            var form = new Form();
            var view = new LinearPlotView
            {
                Dock = DockStyle.Fill
            };

            form.Controls.Add(view);

            form.Show();

            view.Model.Series.Add(new LineSeries
            {
                Points =
                {
                    new DataPoint(0,0),
                    new DataPoint(10,10)
                }
            });

            view.Refresh();

            var maxX = view.Model.Axes[0].ActualMaximum;
            var minX = view.Model.Axes[0].ActualMinimum;
            var maxY = view.Model.Axes[1].ActualMaximum;
            var minY = view.Model.Axes[1].ActualMinimum;

            view.Model.ZoomAllAxes(1.2);
            
            // Preconditions
            Assert.AreNotEqual(maxX, view.Model.Axes[0].ActualMaximum);
            Assert.AreNotEqual(minX, view.Model.Axes[0].ActualMinimum);
            Assert.AreNotEqual(maxY, view.Model.Axes[1].ActualMaximum);
            Assert.AreNotEqual(minY, view.Model.Axes[1].ActualMinimum);

            // Call
            view.ZoomToAll();

            // Assert
            Assert.AreEqual(maxX, view.Model.Axes[0].ActualMaximum);
            Assert.AreEqual(minX, view.Model.Axes[0].ActualMinimum);
            Assert.AreEqual(maxY, view.Model.Axes[1].ActualMaximum);
            Assert.AreEqual(minY, view.Model.Axes[1].ActualMinimum);
        }

        [Test]
        public void ZoomToAll_ViewInFormSerieVisibilityUpdated_AxesAreUpdated()
        {
            // Setup
            var form = new Form();
            var view = new LinearPlotView
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0)
            };

            form.Controls.Add(view);

            form.Show();

            var lineSeries = new LineSeries
            {
                Points =
                {
                    new DataPoint(0,0),
                    new DataPoint(5,5)
                }
            };
            var lineSeriesToBeUpdated = new LineSeries
            {
                Points =
                {
                    new DataPoint(5,5),
                    new DataPoint(10,10)
                }
            };
            view.Model.Series.Add(lineSeries);
            view.Model.Series.Add(lineSeriesToBeUpdated);

            view.Refresh();

            var maxX = view.Model.Axes[0].ActualMaximum;
            var minX = view.Model.Axes[0].ActualMinimum;
            var maxY = view.Model.Axes[1].ActualMaximum;
            var minY = view.Model.Axes[1].ActualMinimum;

            lineSeriesToBeUpdated.IsVisible = false;
            view.InvalidatePlot(true);
            view.Refresh();

            // Call
            view.ZoomToAll();

            // Assert
            
            Assert.Greater(maxX, view.Model.Axes[0].ActualMaximum);
            Assert.AreEqual(minX/2, view.Model.Axes[0].ActualMinimum, 1e-6);
            Assert.Greater(maxY, view.Model.Axes[1].ActualMaximum);
            Assert.AreEqual(minY/2, view.Model.Axes[1].ActualMinimum, 1e-6);
        }
    }
}
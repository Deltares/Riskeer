using System.Windows.Forms;
using Core.Components.OxyPlot.Properties;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;
using TickStyle = OxyPlot.Axes.TickStyle;

namespace Core.Components.OxyPlot.Forms
{
    public sealed class LinearPlotView : PlotView
    {
        internal LinearPlotView()
        {
            Dock = DockStyle.Fill;
            Model = new PlotModel
            {
                Axes =
                {
                    CreateAxis(Resources.BaseChart_XAxisTitle, AxisPosition.Bottom),
                    CreateAxis(Resources.BaseChart_YAxisTitle, AxisPosition.Left)
                }
            };
        }

        /// <summary>
        /// Creates an axis with default style set.
        /// </summary>
        /// <param name="title">The title of the <see cref="LinearAxis"/>.</param>
        /// <param name="position">The <see cref="AxisPosition"/> of the <see cref="LinearAxis"/>.</param>
        /// <returns>A new <see cref="LinearAxis"/> with given <paramref name="title"/> and <paramref name="position"/>.</returns>
        private static LinearAxis CreateAxis(string title, AxisPosition position)
        {
            return new LinearAxis
            {
                Title = title,
                Position = position,
                TickStyle = TickStyle.None,
                ExtraGridlines = new[]
                {
                    0.0
                },
                ExtraGridlineThickness = 1,
                Layer = AxisLayer.AboveSeries,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };
        }

        /// <summary>
        /// Zooms to a level so that everything is in view.
        /// </summary>
        public void ZoomToAll()
        {
            Model.ResetAllAxes();
            Model.ZoomAllAxes(1);
            InvalidatePlot(true);
        }
    }
}
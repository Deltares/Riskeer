using System;
using System.Drawing;

namespace DelftTools.Controls.Swf.Charting
{
    ///<summary>
    /// A TeeChart Legend wrapper class
    ///</summary>
    public class ChartLegend : IChartLegend
    {
        private readonly Steema.TeeChart.Legend legend;

        ///<summary>
        /// Creates a TeeChart Legend wrapper class
        ///</summary>
        ///<param name="legend"></param>
        public ChartLegend(Steema.TeeChart.Legend legend)
        {
            this.legend = legend;
            legend.LegendStyle = Steema.TeeChart.LegendStyles.Series;
        }

        public bool Visible
        {
            get { return legend.Visible; }
            set { legend.Visible = value; }
        }

        public LegendAlignment Alignment
        {
            get
            {
                string enumName = Enum.GetName(typeof(Steema.TeeChart.LegendAlignments), legend.Alignment);
                return (LegendAlignment)Enum.Parse(typeof(LegendAlignment), enumName);
            }
            set
            {
                string enumName = Enum.GetName(typeof(LegendAlignment), value);
                legend.Alignment = (Steema.TeeChart.LegendAlignments)Enum.Parse(typeof(Steema.TeeChart.LegendAlignments), enumName);
            }
        }

        ///<summary>
        /// Enables checkboxes in the legend
        ///</summary>
        public bool ShowCheckBoxes
        {
            get { return legend.CheckBoxes; }
            set { legend.CheckBoxes = value; }
        }

        public Font Font
        {
            get { return legend.Font.DrawingFont; }
            set
            {
                legend.Font.Bold = value.Bold;
                legend.Font.Italic = value.Italic;
                legend.Font.Name = value.Name;
                legend.Font.SizeFloat = value.SizeInPoints;
                legend.Font.Strikeout = value.Strikeout;
                legend.Font.Underline = value.Underline;
            }
        }

        ///<summary>
        /// Maximum width of the legend
        ///</summary>
        public int Width
        {
            get { return legend.Width; }
            set { legend.Width = value; }
        }

        ///<summary>
        /// Distance between the upper left corner of the legend and the top of the axes canvas (in pixels)
        ///</summary>
        public int Top
        {
            get { return legend.Top; }
            set { legend.Top = value; }
        }

        ///<summary>
        /// Distance between the upper left corner of the legend and the left side of the axes canvas (in pixels)
        ///</summary>
        public int Left
        {
            get { return legend.Left; }
            set { legend.Left = value; }
        }
    }
}
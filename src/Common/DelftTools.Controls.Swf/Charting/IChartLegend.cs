using System;
using System.Drawing;

namespace DelftTools.Controls.Swf.Charting
{
    ///<summary>
    ///</summary>
    public interface IChartLegend
    {
        ///<summary>
        /// sets the visibility of the chart legend
        ///</summary>
        bool Visible { get; set; }
       
        LegendAlignment Alignment { get; set; }
        
        int Width { get; set; }
        
        int Top { get; set; }
        
        int Left { get; set; }

        [Obsolete("This functionality is replaced by the ChartLegendView")]
        bool ShowCheckBoxes { get; set; }

        /// <summary>
        /// Sets the font properties of the chart legend
        /// </summary>
        Font Font { get; set; }
    }
}
using System.Drawing;

namespace DelftTools.Controls.Swf.Charting
{
    public class ChartAxis : IChartAxis
    {
        private readonly Steema.TeeChart.Axis teeChartAxis;

        public ChartAxis(Steema.TeeChart.Axis axis)
        {
            teeChartAxis = axis;
        }
               
        public string LabelsFormat
        {
            get { return teeChartAxis.Labels.ValueFormat; }
            set { teeChartAxis.Labels.ValueFormat = value; }
        }
        
        public bool Labels
        {
            get { return teeChartAxis.Labels.Visible; }
            set { teeChartAxis.Labels.Visible = value; }
        }
               
        public bool Visible
        {
            get { return teeChartAxis.Visible; }
            set { teeChartAxis.Visible = value; }
        }

        public bool Automatic
        {
            get { return teeChartAxis.Automatic; }
            set { teeChartAxis.Automatic = value; }
        }

        public double Minimum
        {
            get
            {
                // teechart tends to update the min and max of an axis when needed but this is to late 
                // if we want to respond to viewport changed events.
                teeChartAxis.AdjustMaxMin();
                return teeChartAxis.Minimum;
            }
            set
            {
                teeChartAxis.Minimum = value;
            }
        }

        public double Maximum
        {
            get
            {
                teeChartAxis.AdjustMaxMin();
                return teeChartAxis.Maximum;
            }
            set
            {
                teeChartAxis.Maximum = value;
            }
        }

        public int MinimumOffset
        {
            get { return teeChartAxis.MinimumOffset; }
            set { teeChartAxis.MinimumOffset = value; }
        }

        public int MaximumOffset
        {
            get { return teeChartAxis.MaximumOffset; }
            set { teeChartAxis.MaximumOffset = value; }
        }

        public string Title
        {
            get { return teeChartAxis.Title.Caption; }
            set { teeChartAxis.Title.Caption = value; }
        }

        public Font TitleFont
        { 
            get { return teeChartAxis.Title.Font.DrawingFont; }
            set
            {
                teeChartAxis.Title.Font.Bold = value.Bold;
                teeChartAxis.Title.Font.Italic = value.Italic;
                teeChartAxis.Title.Font.Name = value.Name;
                teeChartAxis.Title.Font.SizeFloat = value.SizeInPoints;
                teeChartAxis.Title.Font.Strikeout = value.Strikeout;
                teeChartAxis.Title.Font.Underline = value.Underline;
            }
        }

        public Font LabelsFont
        {
            get { return teeChartAxis.Labels.Font.DrawingFont; }
            set
            {
                teeChartAxis.Labels.Font.Bold = value.Bold;
                teeChartAxis.Labels.Font.Italic = value.Italic;
                teeChartAxis.Labels.Font.Name = value.Name;
                teeChartAxis.Labels.Font.SizeFloat = value.SizeInPoints;
                teeChartAxis.Labels.Font.Strikeout = value.Strikeout;
                teeChartAxis.Labels.Font.Underline = value.Underline;
            }
        }

        public bool Logaritmic
        {
            get { return teeChartAxis.Logarithmic; }
            set { teeChartAxis.Logarithmic = value; }
        }

        public bool IsDateTime
        {
            get { return teeChartAxis.IsDateTime; }
        }

        public double CalcPosPoint(int position)
        {
            return teeChartAxis.CalcPosPoint(position);
        }

        public int CalcPosValue(double value)
        {
            return teeChartAxis.CalcPosValue(value);
        }
    }
}
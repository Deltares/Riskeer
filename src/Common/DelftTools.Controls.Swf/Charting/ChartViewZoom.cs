namespace DelftTools.Controls.Swf.Charting
{
    internal class ChartViewZoom : IChartViewZoom
    {
        private Steema.TeeChart.Zoom teeChartZoom;

        public ChartViewZoom(Steema.TeeChart.Zoom zoom)
        {
            teeChartZoom = zoom;
        }

        public bool Animated
        {
            get { return teeChartZoom.Animated; }
            set { teeChartZoom.Animated = value; }
        }

        public bool Allow
        {
            get { return teeChartZoom.Allow; }
            set { teeChartZoom.Allow = value; }
        }

        public ZoomDirections Direction
        {
            get
            {
                switch (teeChartZoom.Direction)
                {
                    case Steema.TeeChart.ZoomDirections.Horizontal: return ZoomDirections.Horizontal;
                    case Steema.TeeChart.ZoomDirections.Vertical: return ZoomDirections.Vertical;
                    default : return ZoomDirections.Both;
                } 
            }
            set
            {
                switch (value)
                {
                    case ZoomDirections.Vertical:
                        teeChartZoom.Direction = Steema.TeeChart.ZoomDirections.Vertical;
                        break;
                    case ZoomDirections.Horizontal:
                        teeChartZoom.Direction = Steema.TeeChart.ZoomDirections.Horizontal;
                        break;
                    case ZoomDirections.Both:
                        teeChartZoom.Direction = Steema.TeeChart.ZoomDirections.Both;
                        break;
                }
            }
        }
    }
}
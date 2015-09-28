using System.Windows.Forms;

namespace SharpMap.UI.Tools.Zooming
{
    public class FixedZoomInTool : ZoomTool
    {
        public FixedZoomInTool()
        {
            Name = "FixedZoomIn";
        }


        public override bool AlwaysActive
        {
            get { return true; }
        }

        public override void Execute()
        {
            Map.Zoom *= 0.80; // zoom in
            MapControl.Refresh();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if((e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add) && IsCtrlPressed)
            {
                Execute();
            }
        }
    }
}
using System.Windows.Forms;

namespace SharpMap.UI.Tools.Zooming
{
    public class FixedZoomOutTool : ZoomTool
    {
        public FixedZoomOutTool()
        {
            Name = "FixedZoomOut";
        }


        public override bool AlwaysActive
        {
            get { return true; }
        }

        public override void Execute()
        {
            Map.Zoom *= 1.20; // zoom out
            MapControl.Refresh();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if((e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract) && IsCtrlPressed)
            {
                Execute();
            }
        }
    }
}
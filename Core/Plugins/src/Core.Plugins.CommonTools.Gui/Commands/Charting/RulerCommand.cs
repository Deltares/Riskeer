using Core.Common.Controls.Swf.Charting.Tools;

namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    public class RulerCommand : ChartViewCommandBase
    {
        public override bool Checked
        {
            get
            {
                return RulerTool != null && RulerTool.Active;
            }
        }

        public override bool Enabled
        {
            get
            {
                return View != null && RulerTool != null;
            }
        }

        public override void Execute(params object[] arguments)
        {
            var view = RulerTool;
            if (view == null || arguments.Length == 0)
            {
                return;
            }

            RulerTool.Active = (bool) arguments[0];
        }

        private RulerTool RulerTool
        {
            get
            {
                return View != null ? View.GetTool<RulerTool>() : null;
            }
        }
    }
}
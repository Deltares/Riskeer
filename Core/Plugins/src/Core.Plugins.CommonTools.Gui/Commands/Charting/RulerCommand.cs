using Core.Common.Controls.Charting.Tools;

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

        /// <summary>
        /// Shows or hides the <see cref="RulerTool"/>.
        /// </summary>
        /// <param name="arguments">
        /// <list type="bullet">
        /// <item>[0] <see cref="bool"/> - <c>true</c> to show the <see cref="RulerTool"/>, <c>false</c> to hide the <see cref="RulerTool"/>.</item>
        /// </list></param>
        public override void Execute(params object[] arguments)
        {
            var rulerTool = RulerTool;
            if (rulerTool == null || arguments.Length == 0)
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
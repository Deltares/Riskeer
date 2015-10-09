using DelftTools.Controls;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Tools;
using DelftTools.Shell.Gui;

namespace DeltaShell.Plugins.CommonTools.Gui.Commands.Charting
{
    public class RulerCommand : Command, IGuiCommand
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
                return Gui != null && Gui.DocumentViews.ActiveView != null && RulerTool != null;
            }
        }

        public IGui Gui { get; set; }

        protected override void OnExecute(params object[] arguments)
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
                var chartView = GetChartViewWithRulerRecursive(Gui.DocumentViews.ActiveView);
                return chartView == null ? null : chartView.GetTool<RulerTool>() as RulerTool;
            }
        }

        private ChartView GetChartViewWithRulerRecursive(IView view)
        {
            var chartView = view as ChartView;
            if (chartView == null)
            {
                var compositeView = view as ICompositeView;
                if (compositeView != null)
                {
                    foreach (var childView in compositeView.ChildViews)
                    {
                        var v = GetChartViewWithRulerRecursive(childView);
                        if (v != null)
                        {
                            return v;
                        }
                    }
                }
            }
            return chartView == null ? null :
                       chartView.GetTool<RulerTool>() == null ? null :
                           chartView;
        }
    }
}
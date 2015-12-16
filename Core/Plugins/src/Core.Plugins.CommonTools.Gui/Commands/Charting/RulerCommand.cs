using Core.Common.Controls;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Controls.Swf.Charting.Tools;
using Core.Common.Gui;

namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    public class RulerCommand : GuiCommand
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
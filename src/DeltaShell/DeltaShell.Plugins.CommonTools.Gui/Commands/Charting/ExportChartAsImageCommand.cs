using System.Drawing;
using DelftTools.Controls;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Shell.Gui;

namespace DeltaShell.Plugins.CommonTools.Gui.Commands.Charting
{
    public class IncreaseFontSizeCommand : ChartViewCommandBase
    {
        protected override void OnExecute(params object[] arguments)
        {
            var view = View;
            if (view != null)
            {
                view.Chart.Font = GetChangedFontSize(view.Chart.Font, 1);
                view.Chart.Legend.Font = GetChangedFontSize(view.Chart.Legend.Font, 1);
                view.Chart.LeftAxis.LabelsFont = GetChangedFontSize(view.Chart.LeftAxis.LabelsFont, 1);
                view.Chart.LeftAxis.TitleFont = GetChangedFontSize(view.Chart.LeftAxis.TitleFont, 1);
                view.Chart.BottomAxis.LabelsFont = GetChangedFontSize(view.Chart.BottomAxis.LabelsFont, 1);
                view.Chart.BottomAxis.TitleFont = GetChangedFontSize(view.Chart.BottomAxis.TitleFont, 1);
                view.Chart.RightAxis.LabelsFont = GetChangedFontSize(view.Chart.RightAxis.LabelsFont, 1);
                view.Chart.RightAxis.TitleFont = GetChangedFontSize(view.Chart.RightAxis.TitleFont, 1);
            }
        }
    }

    public class DecreaseFontSizeCommand : ChartViewCommandBase
    {
        protected override void OnExecute(params object[] arguments)
        {
            var view = View;
            if (view != null)
            {
                view.Chart.Font = GetChangedFontSize(view.Chart.Font, -1);
                view.Chart.Legend.Font = GetChangedFontSize(view.Chart.Legend.Font, -1);
                view.Chart.LeftAxis.LabelsFont = GetChangedFontSize(view.Chart.LeftAxis.LabelsFont, -1);
                view.Chart.LeftAxis.TitleFont = GetChangedFontSize(view.Chart.LeftAxis.TitleFont, -1);
                view.Chart.BottomAxis.LabelsFont = GetChangedFontSize(view.Chart.BottomAxis.LabelsFont, -1);
                view.Chart.BottomAxis.TitleFont = GetChangedFontSize(view.Chart.BottomAxis.TitleFont, -1);
                view.Chart.RightAxis.LabelsFont = GetChangedFontSize(view.Chart.RightAxis.LabelsFont, -1);
                view.Chart.RightAxis.TitleFont = GetChangedFontSize(view.Chart.RightAxis.TitleFont, -1);
            }
        }
    }

    public class ExportChartAsImageCommand : ChartViewCommandBase
    {
        protected override void OnExecute(params object[] arguments)
        {
            var view = View;
            if (view != null)
            {
                view.ExportAsImage();
            }
        }
    }

    public abstract class ChartViewCommandBase : Command, IGuiCommand
    {
        public override bool Enabled
        {
            get
            {
                return View != null;
            }
        }

        public IGui Gui { get; set; }

        protected ChartView View
        {
            get
            {
                return Gui != null
                           ? GetViewRecursive<ChartView>(Gui.DocumentViews.ActiveView)
                           : null;
            }
        }

        protected abstract override void OnExecute(params object[] arguments);

        protected Font GetChangedFontSize(Font font, int pixels)
        {
            return new Font(font.FontFamily, font.Size + pixels, font.Style);
        }

        private T GetViewRecursive<T>(IView view) where T : class, IView
        {
            if (view is T)
            {
                return (T) view;
            }
            var compositeView = view as ICompositeView;
            if (compositeView != null)
            {
                foreach (var childView in compositeView.ChildViews)
                {
                    var childOfTypeT = GetViewRecursive<T>(childView);
                    if (childOfTypeT != null)
                    {
                        return childOfTypeT;
                    }
                }
            }
            return null;
        }
    }
}
using System.Drawing;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Controls.Views;
using Core.Common.Gui;

namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    public abstract class ChartViewCommandBase : GuiCommand
    {
        public override bool Enabled
        {
            get
            {
                return View != null;
            }
        }

        public override bool Checked
        {
            get
            {
                return false;
            }
        }

        protected ChartView View
        {
            get
            {
                return Gui != null
                           ? GetViewRecursive<ChartView>(Gui.DocumentViews.ActiveView)
                           : null;
            }
        }

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
using System;
using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Gui;

namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    /// <summary>
    /// Classes derived from this class will have a possibility to obtain the active
    /// <see cref="ChartView"/> (if any) and the <see cref="IGui"/>.
    /// </summary>
    public abstract class ChartViewCommandBase : IGuiCommand
    {
        public virtual bool Enabled
        {
            get
            {
                return View != null;
            }
        }

        public virtual bool Checked
        {
            get
            {
                return false;
            }
        }

        public IGui Gui { get; set; }

        public abstract void Execute(params object[] arguments);

        /// <summary>
        /// Gets the currently active view if it is a <see cref="ChartView"/> and
        /// <c>null</c> otherwise.
        /// </summary>
        protected IChartView View
        {
            get
            {
                return Gui != null
                           ? Gui.DocumentViews.ActiveView as IChartView
                           : null;
            }
        }

        /// <summary>
        /// Changes the size of the <paramref name="font"/> with <paramref name="points"/> points.
        /// </summary>
        /// <param name="font">The <see cref="Font"/> to modify the size of.</param>
        /// <param name="points">The change in points to modify the size of <paramref name="font"/> by.</param>
        /// <returns>A new instance of <see cref="Font"/> with the new size.</returns>
        /// <exception cref="ArgumentException">Thrown when modification ends up in a font size &lt;= 0.</exception>
        protected Font GetChangedFontSize(Font font, int points)
        {
            return new Font(font.FontFamily, font.SizeInPoints + points, font.Style);
        }
    }
}
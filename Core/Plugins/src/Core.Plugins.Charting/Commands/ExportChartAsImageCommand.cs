namespace Core.Plugins.Charting.Commands
{
    public class ExportChartAsImageCommand : ChartViewCommandBase
    {
        /// <summary>
        /// Exports the active <see cref="Core.Common.Controls.Charting.IChartView"/> as an image.
        /// </summary>
        /// <param name="arguments">No arguments are needed when calling this method.</param>
        public override void Execute(params object[] arguments)
        {
            var view = View;
            if (view != null)
            {
                view.ExportAsImage();
            }
        }
    }
}
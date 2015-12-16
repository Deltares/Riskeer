namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    public class ExportChartAsImageCommand : ChartViewCommandBase
    {
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
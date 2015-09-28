using System.Drawing;

namespace DelftTools.Controls.Swf.Charting
{
    public delegate void NotifyChartSeriesValueChanged(object sender, NotifyChartSeriesValueChangedArgs args);

    public class NotifyChartSeriesValueChangedArgs
    {
        // tell what it is, index, value, new, old
    }

    /// <summary>
    /// Provides data in a suitable format to be used by chartseries of Steema.TeeChart.
    /// </summary>
    public interface IChartSeriesValueProvider
    {
        T GetX<T>(int index);
        T GetY<T>(int index);
        Color GetColor(int index);
        string GetLabel(int index);
        
        void SetX<T>(int index, T value);
        void SetY<T>(int index, T value);
        void SetColor(int index, Color color);
        void SetLabel(int index, string text);

        event NotifyChartSeriesValueChanged ValueChanged;
    }
}
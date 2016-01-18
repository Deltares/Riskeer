using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Core.Plugins.OxyPlot.Test
{
    /// <summary>
    /// Simple <see cref="IView"/> implementation which can be used in tests.
    /// </summary>
    public class TestView : Control, IView
    {
        public object Data { get; set; }
    }
}
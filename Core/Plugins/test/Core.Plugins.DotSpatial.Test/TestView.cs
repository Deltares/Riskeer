using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Core.Plugins.DotSpatial.Test
{
    public partial class TestView : UserControl, IView
    {
        public TestView()
        {
            InitializeComponent();
        }

        public object Data { get; set; }
    }
}
using System.Windows.Forms;

using Core.Common.Controls.Views;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    public partial class TestView : UserControl, IView
    {
        public TestView()
        {
            InitializeComponent();
        }

        public object Data { get; set; }
    }

    public class TestViewDerivative : TestView {}

    public class TestWrapper
    {
        public string RealData { get; set; }
    }
}
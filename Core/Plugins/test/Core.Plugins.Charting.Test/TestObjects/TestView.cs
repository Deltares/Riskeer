using System.Windows.Forms;
using Core.Common.Forms.Views;

namespace Core.Plugins.Charting.Test.TestObjects
{
    public partial class TestView : UserControl, IView
    {
        public TestView()
        {
            InitializeComponent();
        }

        public object Data { get; set; }
    }

    public class TestViewDerivative : TestView
    {

    }

    public class TestWrapper
    {
        public string RealData { get; set; }
    }
}
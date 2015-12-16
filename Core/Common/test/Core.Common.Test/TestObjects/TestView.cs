using System;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Views;

namespace Core.Common.Test.TestObjects
{
    public partial class TestView : UserControl, IView
    {
        public TestView()
        {
            InitializeComponent();
        }

        public object Data { get; set; }

        public ViewInfo ViewInfo { get; set; }

        public void EnsureVisible(object item) {}
    }

    public class ReusableTestView : TestView, IReusableView
    {
        public event EventHandler LockedChanged;
        private bool locked;

        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                if (!Equals(locked, value))
                {
                    locked = value;
                    if (LockedChanged != null)
                    {
                        LockedChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
    }

    public class AdditionalView : TestView, IAdditionalView {}

    public class TestWrapper
    {
        public string RealData { get; set; }
    }
}
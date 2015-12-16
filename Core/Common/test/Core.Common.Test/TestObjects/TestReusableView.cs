using System;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Views;

namespace Core.Common.Test.TestObjects
{
    public partial class TestReusableView : UserControl, IReusableView
    {
        public event EventHandler LockedChanged;

        private bool locked;

        public TestReusableView()
        {
            InitializeComponent();
        }

        public object Data { get; set; }

        public ViewInfo ViewInfo { get; set; }

        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
                if (LockedChanged != null)
                {
                    LockedChanged(this, new EventArgs());
                }
            }
        }
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;

namespace Core.Common.Tests.TestObjects
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
        public Image Image { get; set; }
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

        public void EnsureVisible(object item) {}
    }
}
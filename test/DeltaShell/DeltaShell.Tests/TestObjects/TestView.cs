﻿using System;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;

namespace DeltaShell.Tests.TestObjects
{
    public partial class TestView : UserControl,IView
    {
        public TestView()
        {
            InitializeComponent();
        }

        public object Data { get; set; }
        public Image Image { get; set; }
        public void EnsureVisible(object item) { }
        public ViewInfo ViewInfo { get; set; }
    }

    public class ReusableTestView : TestView, IReusableView
    {
        private bool locked;
        public bool Locked
        {
            get { return locked; }
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

        public event EventHandler LockedChanged;
    }

    public class AdditionalView : TestView, IAdditionalView
    {
    }

    public class TestWrapper
    {
        public string RealData { get; set; }
    }
}

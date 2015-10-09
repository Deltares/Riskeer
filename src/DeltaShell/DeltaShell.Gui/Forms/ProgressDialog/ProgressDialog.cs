using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DeltaShell.Gui.Properties;

namespace DeltaShell.Gui.Forms.ProgressDialog
{
    public partial class ProgressDialog : Form
    {
        public event EventHandler<EventArgs> CancelClicked;
        private Timer refreshTimer = new Timer();
        private BindingList<ActivityInfo> bindingList;

        private IEventedList<IActivity> data;
        //private IEnumerable<IActivity> activities;

        public ProgressDialog()
        {
            InitializeComponent();
            dgvActivities.AutoGenerateColumns = false;
            StartPosition = FormStartPosition.Manual;

            refreshTimer.Interval = 300;
            refreshTimer.Tick += RefreshTimerOnTick;
        }

        public IEventedList<IActivity> Data
        {
            get
            {
                return data;
            }
            set
            {
                /*if (data != null)
                {
                    UnWireBindingList();
                }*/
                //create a bindinglist<ActivityInfo>
                data = value;
                if (data != null)
                {
                    CreateBindingList();
                    WireBindingList();
                    dgvActivities.DataSource = bindingList;
                }
                //data = value;
            }
        }

        public void CenterToParent()
        {
            if (Owner == null)
            {
                CenterToScreen(); //temp hack for wpf shizzle
                return;
            }

            // parent is smaller
            if (Width > Owner.Width || Height > Owner.Height)
            {
                Location = Owner.Location;
                return;
            }

            var x = Owner.DesktopLocation.X + (Owner.Width - Width)/2;
            int y = Owner.DesktopLocation.Y + (Owner.Height - Height)/2;

            Location = new Point(x, y);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) //user shouldn't close this (with Alt-F4 or anything)
            {
                e.Cancel = true;
            }

            refreshTimer.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            CenterToParent();
            refreshTimer.Start();
        }

        private void RefreshTimerOnTick(object sender, EventArgs eventArgs)
        {
            dgvActivities.Refresh();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetButtonState(true);

            if (CancelClicked != null)
            {
                CancelClicked(this, EventArgs.Empty);
            }
        }

        private void SetButtonState(bool cancelling)
        {
            if (cancelling)
            {
                btnCancel.Text = Resources.ProgressDialog_SetButtonState_Cancelling___;
                btnCancel.Enabled = false;
            }
            else
            {
                btnCancel.Text = Resources.ProgressDialog_SetButtonState_Cancel_all_activities;
                btnCancel.Enabled = true;
            }
        }

        private void UnWireBindingList()
        {
            if (data is INotifyPropertyChanged)
            {
                data.CollectionChanged -= DataCollectionChanged;
            }
        }

        private void WireBindingList()
        {
            if (data is INotifyPropertyChanged)
            {
                data.CollectionChanged += DataCollectionChanged;
            }
        }

        [InvokeRequired]
        private void DataCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            //dont handle bubbled events
            if (sender != data)
            {
                return;
            }
            if (e.Action == NotifyCollectionChangeAction.Add)
            {
                bindingList.Add(new ActivityInfo((IActivity) e.Item));
                SetButtonState(false);
            }
            if (e.Action == NotifyCollectionChangeAction.Remove)
            {
                bindingList.RemoveAt(e.Index);
            }
            if (e.Action == NotifyCollectionChangeAction.Replace)
            {
                bindingList[e.Index] = new ActivityInfo((IActivity) e.Item);
            }
        }

        private void CreateBindingList()
        {
            bindingList = new BindingList<ActivityInfo>();
            foreach (var activity in data)
            {
                bindingList.Add(new ActivityInfo(activity));
            }
        }
    }
}
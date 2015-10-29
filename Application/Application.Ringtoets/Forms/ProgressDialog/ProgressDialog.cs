using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Application.Ringtoets.Properties;
using Core.Common.BaseDelftTools.Workflow;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;

namespace Application.Ringtoets.Forms.ProgressDialog
{
    public partial class ProgressDialog : Form
    {
        public event EventHandler<EventArgs> CancelClicked;
        private BindingList<ActivityInfo> bindingList;
        private IEventedList<IActivity> data;

        public ProgressDialog()
        {
            InitializeComponent();
            dgvActivities.AutoGenerateColumns = false;
            StartPosition = FormStartPosition.Manual;
        }

        public IEventedList<IActivity> Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                if (data != null)
                {
                    CreateBindingList();
                    WireBindingList();
                    dgvActivities.DataSource = bindingList;
                }
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
        }

        protected override void OnLoad(EventArgs e)
        {
            CenterToParent();
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
using System;
using System.ComponentModel;
using Application.Ringtoets.Properties;
using Core.Common.Base.Workflow;
using Core.Common.Utils.Reflection;

namespace Application.Ringtoets.Forms.ProgressDialog
{
    /// <summary>
    /// Class wrapper an IActivity to make it bindable and evented etc.
    /// Used in progressdialog only
    /// </summary>
    internal class ActivityInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IActivity activity;

        public ActivityInfo(IActivity activity)
        {
            this.activity = activity;

            activity.ProgressChanged += ActivityInfoProgressChanged;

            if (this.activity is INotifyPropertyChanged)
            {
                (this.activity as INotifyPropertyChanged).PropertyChanged += ActivityInfoPropertyChanged;
            }
        }

        public string ProgressText
        {
            get
            {
                string progressText;
                try
                {
                    progressText = activity.ProgressText;
                }
                catch
                {
                    progressText = Resources.ActivityInfo_ProgressText_undefined;
                }
                return progressText ?? "";
            }
        }

        /* Push down to model etc.
         * private static string GetProgressText(IProgressingActivity<DateTime, TimeSpan> progressingActivity)
        {
            var totalTime = progressingActivity.ProgressStop - progressingActivity.ProgressStart;
            var elapsedTime = progressingActivity.ProgressCurrent - progressingActivity.ProgressStart;
            var percentage = Math.Max(0.0, elapsedTime.TotalSeconds/totalTime.TotalSeconds);
            return (100*percentage).ToString("N2") + " %";
        }*/

        public string Name
        {
            get
            {
                return activity.Name;
            }
        }

        private void ActivityInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //a name change changes the wrapper name
            if (e.PropertyName == TypeUtils.GetMemberName(() => Name))
            {
                OnPropertyChanged(TypeUtils.GetMemberName(() => Name));
            }
        }

        private void ActivityInfoProgressChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(TypeUtils.GetMemberName(() => ProgressText));
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
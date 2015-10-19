using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Utils;

namespace DelftTools.Controls.Swf
{
    /// <summary>
    /// Control that shows html page defined by Web link.
    /// </summary>
    public class HtmlPageView : WebBrowser, IView
    {
        private Url url;

        /// <summary>
        /// Initializes a new instance of the <c>HtmlPageView</c> class. 
        /// </summary>
        public HtmlPageView()
        {
            ScriptErrorsSuppressed = true;
        }

        /// <summary>
        /// Loads the html page at the specified url.
        /// </summary>
        public override void Refresh()
        {
            Navigate(url.Path);
            Name = url.Name;
            Text = Name;
            try
            {
                base.Refresh();
            }
            catch (Exception)
            {
                // do nothing with this exception
            }
        }

        ~HtmlPageView()
        {
            DisableEventListeners();
        }

        #region IView Members

        /// <summary>
        /// Web link.
        /// </summary>
        public object Data
        {
            get
            {
                return url;
            }
            set
            {
                DisableEventListeners();
                url = (Url) value;

                if (url != null)
                {
                    var notifyPropertyChanged = url as INotifyPropertyChange;
                    if (notifyPropertyChanged != null)
                    {
                        notifyPropertyChanged.PropertyChanged += notifyPropertyChanged_PropertyChanged;
                    }
                    Refresh();
                }
            }
        }

        private void DisableEventListeners()
        {
            var notifyPropertyChanged = url as INotifyPropertyChange;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged -= notifyPropertyChanged_PropertyChanged;
            }
        }

        private void notifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Path")
            {
                Navigate(url.Path);
            }
            else if (e.PropertyName == "Name")
            {
                Text = url.Name;
            }
        }

        public override string Text { get; set; }

        public Image Image { get; set; }
        public void EnsureVisible(object item) {}
        public ViewInfo ViewInfo { get; set; }

        protected override void OnDragDrop(DragEventArgs e)
        {
            var url1 = e.Data.GetData(typeof(Url)) as Url;
            if (url1 != null)
            {
                Data = url1;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            var url1 = e.Data.GetData(typeof(Url)) as Url;
            if (url1 != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        #endregion
    }
}
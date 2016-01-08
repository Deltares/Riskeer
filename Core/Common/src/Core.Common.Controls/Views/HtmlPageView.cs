using System.Windows.Forms;
using Core.Common.Utils;

namespace Core.Common.Controls.Views
{
    /// <summary>
    /// Class for showing a html page based on an <see cref="Url"/>.
    /// </summary>
    public partial class HtmlPageView : UserControl, IView
    {
        private Url url;

        /// <summary>
        /// Constructs a new <see cref="HtmlPageView"/>.
        /// </summary>
        public HtmlPageView()
        {
            InitializeComponent();
        }

        public object Data
        {
            get
            {
                return url;
            }
            set
            {
                url = value as Url;

                if (url != null && url.Path != null)
                {
                    webBrowser.Navigate(url.Path);
                }
                else
                {
                    if (!IsDisposed)
                    {
                        webBrowser.Navigate("");
                    }
                }
            }
        }
    }
}

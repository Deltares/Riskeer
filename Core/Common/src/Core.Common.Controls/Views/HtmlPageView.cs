using System.Windows.Forms;
using Core.Common.Utils;

namespace Core.Common.Controls.Views
{
    /// <summary>
    /// Class for showing a html page based on an <see cref="WebLink"/>.
    /// </summary>
    public partial class HtmlPageView : UserControl, IView
    {
        private WebLink url;

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
                url = value as WebLink;

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

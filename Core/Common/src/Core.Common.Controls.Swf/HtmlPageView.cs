using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Utils;

namespace Core.Common.Controls.Swf
{
    /// <summary>
    /// Control that shows html page defined by <see cref="Url"/>.
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

        public object Data
        {
            get
            {
                return url;
            }
            set
            {
                url = (Url) value;

                if (url != null)
                {
                    Navigate(url.Path);
                }
            }
        }

        public override string Text { get; set; }

        public Image Image { get; set; }

        public ViewInfo ViewInfo { get; set; }

        public void EnsureVisible(object item) {}
    }
}
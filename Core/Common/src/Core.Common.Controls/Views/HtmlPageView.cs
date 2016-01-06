using System.Windows.Forms;
using Core.Common.Utils;

namespace Core.Common.Forms.Views
{
    /// <summary>
    /// Class for showing a html page based on an <see cref="Url"/>.
    /// </summary>
    public class HtmlPageView : WebBrowser, IView
    {
        private Url url;

        /// <summary>
        /// Constructs a new <see cref="HtmlPageView"/>.
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

                if (url != null && url.Path != null)
                {
                    Navigate(url.Path);
                }
            }
        }

        /// <summary>
        /// Get or sets the text of the <see cref="HtmlPageView"/>.
        /// </summary>
        /// <remarks>
        /// A base implementation was already provided by <see cref="WebBrowser"/> but this implementation turned out to only throw exceptions.
        /// </remarks>
        public override string Text { get; set; }
    }
}
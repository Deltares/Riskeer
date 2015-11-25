using System.Windows;
using System.Windows.Media;
using log4net;

namespace Core.Common.Gui.Forms.SplashScreen
{
    /// <summary>
    ///     Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SplashScreen));
        private string progressText;
        private int progressValuePercent;
        private string licenseText;
        private string companyText;
        private string copyrightText;
        private string versionText;
        private bool hasProgress;

        public SplashScreen()
        {
            InitializeComponent();

            progressBar.Maximum = 100; // classic percentage approach, there is no need for the splash screen to be more precise

            HasProgress = true;
            ProgressValuePercent = 0;
            ProgressText = "";
            CompanyText = "";
            CopyrightText = "";
            LicenseText = "";
            VersionText = "";
        }

        /// <summary>
        /// Defines if the progress bar and progress label should be visible in the window.
        /// </summary>
        public bool HasProgress
        {
            get
            {
                return hasProgress;
            }
            set
            {
                hasProgress = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Version to be shown
        /// </summary>
        public string VersionText
        {
            get
            {
                return versionText;
            }
            set
            {
                versionText = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Copyright owner to be shown
        /// </summary>
        public string CopyrightText
        {
            get
            {
                return copyrightText;
            }
            set
            {
                copyrightText = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Registred company to be shown
        /// </summary>
        public string CompanyText
        {
            get
            {
                return companyText;
            }
            set
            {
                companyText = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Type of the license, plain text
        /// </summary>
        public string LicenseText
        {
            get
            {
                return licenseText;
            }
            set
            {
                licenseText = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Percentage value to be set as progress indication. 
        /// </summary>
        public int ProgressValuePercent
        {
            get
            {
                return progressValuePercent;
            }
            set
            {
                progressValuePercent = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Text, as a current status of the progress
        /// </summary>
        public string ProgressText
        {
            get
            {
                return progressText;
            }
            set
            {
                progressText = value;
                InvalidateVisual();
            }
        }

        public void Shutdown()
        {
            Focusable = false;
            log.Info(Properties.Resources.SplashScreen_Shutdown_Hiding_splash_screen);
            Close();
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (labelLicense.Content.ToString() != LicenseText)
            {
                labelLicense.Content = LicenseText;
            }

            if (labelCompany.Content.ToString() != CompanyText)
            {
                labelCompany.Content = CompanyText;
            }

            if (labelCopyright.Content.ToString() != CopyrightText)
            {
                labelCopyright.Content = CopyrightText;
            }

            if (labelVersion.Content.ToString() != VersionText)
            {
                labelVersion.Content = VersionText;
            }

            var progressVisibility = HasProgress ? Visibility.Visible : Visibility.Hidden;
            
            progressBar.Visibility = progressVisibility;
            labelProgressBar.Visibility = progressVisibility;
            labelProgressMessage.Visibility = progressVisibility;

            if (!HasProgress)
            {
                return; // no need to update progress related labels below
            }

            if (progressBar.Value != ProgressValuePercent)
            {
                progressBar.Value = ProgressValuePercent;
                labelProgressBar.Content = string.Format("{0} %", ProgressValuePercent);
            }

            if (labelProgressMessage.Content.ToString() != ProgressText)
            {
                labelProgressMessage.Content = ProgressText;
            }
        }
    }
}
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Core.Common.Base;
using log4net;

namespace Core.Common.Gui.Forms.SplashScreen
{
    /// <summary>
    ///     Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DeltaShellApplication));

        private readonly DispatcherTimer updateTimer;

        private IApplication app;
        private string logoImageFilePath;
        private volatile ProgressInfo progressInfo;
        private bool scheduleUpdate;

        private int progressBarValue;

        private string lastProgressMessage; // sometimes we progress only percentage, in this case we keep the last message here

        private bool refreshing;

        private bool shutdown;

        public SplashScreen() : this(null) {}

        public SplashScreen(IApplication app)
        {
            InitializeComponent();

            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            this.app = app;

            progressBar.Maximum = 1;
            progressBar.Value = 0;
            labelProgressBar.Content = "0 %";

            IsVisibleChanged += SplashScreen_VisibleChanged;

            updateTimer = new DispatcherTimer(DispatcherPriority.Render, Dispatcher);
            updateTimer.Tick += OnTimer;
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            updateTimer.Start();
        }

        public string LabelVersionVersion
        {
            get
            {
                return (string) labelVersion.Content;
            }
            set
            {
                labelVersion.Content = value;
            }
        }

        public string SplashScreenCopyright
        {
            get
            {
                return (string) labelCopyright.Content;
            }
            set
            {
                labelCopyright.Content = value;
            }
        }

        public string LabelCompany
        {
            get
            {
                return (string) labelCompany.Content;
            }
            set
            {
                labelCompany.Content = value;
            }
        }

        public string LabelLicense
        {
            get
            {
                return (string) labelLicense.Content;
            }
            set
            {
                labelLicense.Content = value;
            }
        }

        public int ProgressBarValue
        {
            get
            {
                return progressBarValue;
            }
            set
            {
                progressBar.Value = value;
                progressBarValue = value;
            }
        }

        public string ProgressBarText
        {
            get
            {
                return (string) labelProgressBar.Content;
            }
            set
            {
                labelProgressBar.Content = value;
            }
        }

        public void Shutdown()
        {
            shutdown = true;

            updateTimer.Stop();
            while (refreshing)
            {
                Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
            }

            Focusable = false;
            app = null;

            log.Info(Properties.Resources.SplashScreen_Shutdown_Hiding_splash_screen____);

            Close();
        }

        private void OnTimer(object state, EventArgs eventArgs)
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (!scheduleUpdate)
            {
                return;
            }

            scheduleUpdate = false;

            ProgressInfo newInfo = progressInfo;

            ProgressBarValue = newInfo.ProgressValue;
            ProgressBarText = newInfo.ProgressText;

            labelProgressMessage.Content = newInfo.Message != "" ? newInfo.Message : lastProgressMessage;
        }

        private void SplashScreen_VisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (IsVisible)
            {
                log.Debug(Properties.Resources.SplashScreen_SplashScreen_VisibleChanged_Showing_splash_screen____);
                string splashScreenLogFilePath =
                    Path.Combine(app.GetUserSettingsDirectoryPath(), "splash-screen-log-history.xml");
            }
            else
            {
                updateTimer.Stop();
            }
        }

        private void ScheduleUpdate()
        {
            if (!shutdown)
            {
                scheduleUpdate = true;
                RefreshControls();
            }
        }

        private void RefreshControls()
        {
            refreshing = true;
            progressBar.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => { }));
            labelProgressMessage.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => { }));
            refreshing = false;
        }

        public class ProgressInfo
        {
            public string Message;
            public string ProgressText;
            public int ProgressValue;
        }
    }
}
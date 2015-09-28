using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DelftTools.Shell.Core;
using DeltaShell.Core;
using DeltaShell.Gui.Forms.MessageWindow;
using log4net;

namespace DeltaShell.Gui.Forms
{
    /// <summary>
    ///     Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (DeltaShellApplication));
        private IApplication app;

        private readonly DispatcherTimer updateTimer;
        private DateTime lastUpdateTime;
        private string logoImageFilePath;
        private MessageWindowData messageWindowData;
        private volatile ProgressInfo progressInfo;
        private bool scheduleUpdate;

        public class ProgressInfo
        {
            public string Message;
            public string ProgressText;
            public int ProgressValue;
        }

        public SplashScreen() : this(null)
        {
            
        }

        public SplashScreen(IApplication app)
        {
            InitializeComponent();

            lastUpdateTime = new DateTime(0);

            if (app == null) throw new ArgumentNullException("app");
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

        public string LogoImageFilePath
        {
            get { return logoImageFilePath; }
            set
            {
                logoImageFilePath = value;
                Background = new ImageBrush(new BitmapImage(new Uri(logoImageFilePath, UriKind.Relative)));
            }
        }

        public string LabelVersionVersion
        {
            get { return (string) labelVersion.Content; }
            set { labelVersion.Content = value; }
        }

        public string SplashScreenCopyright
        {
            get { return (string)labelCopyright.Content; }
            set { labelCopyright.Content = value; }
        }

        public string LabelCompany
        {
            get { return (string)labelCompany.Content; }
            set { labelCompany.Content = value; }
        }

        public string LabelLicense
        {
            get { return (string)labelLicense.Content; }
            set { labelLicense.Content = value; }
        }
        
        public int maxProgressBarValue;
        public int MaxProgressBarValue
        {
            get { return maxProgressBarValue; }
            set
            {
                progressBar.Maximum = value;
                maxProgressBarValue = value;
            }
        }

        private int progressBarValue;
        public int ProgressBarValue
        {
            get { return progressBarValue; }
            set
            {
                progressBar.Value = value;
                progressBarValue = value;
            }
        }

        public string ProgressBarText
        {
            get { return (string) labelProgressBar.Content; }
            set { labelProgressBar.Content = value; }
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

            if (newInfo.Message != "")
            {
                labelProgressMessage.Content = newInfo.Message;
            }
            else
            {
                labelProgressMessage.Content = lastProgressMessage;
            }
        }

        private void SplashScreen_VisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (IsVisible)
            {
                log.Debug(Properties.Resources.SplashScreen_SplashScreen_VisibleChanged_Showing_splash_screen____);
                string splashScreenLogFilePath =
                    System.IO.Path.Combine(app.GetUserSettingsDirectoryPath(), "splash-screen-log-history.xml");
                SplashScreenLogAppender.InitializeUsingLogging(splashScreenLogFilePath, this);
            }
            else
            {
                updateTimer.Stop();
            }
        }

        private string lastProgressMessage; // sometimes we progress only percentage, in this case we keep the last message here
        public void SetProgress(ProgressInfo info)
        {
            // multi-threaded app support, makes sure that the last message is really the last one (skip "")
            if (info.Message == "" && progressInfo != null && progressInfo.Message != "")
            {
                lastProgressMessage = progressInfo.Message;
            }

            progressInfo = info;
            ScheduleUpdate();
        }

        private void ScheduleUpdate()
        {
            if (!shutdown)
            {
                scheduleUpdate = true;
                RefreshControls();
            }
        }

        private bool refreshing;
        private void RefreshControls()
        {
            refreshing = true;
            progressBar.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => { }));
            labelProgressMessage.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => { }));
            refreshing = false;
        }

        private bool shutdown;
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
            SplashScreenLogAppender.Shutdown();

            Close();
        }
    }
}
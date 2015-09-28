using System;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using DelftTools.Shell.Core;
using DelftTools.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;
using Application = System.Windows.Forms.Application;
using SplashScreen = DeltaShell.Gui.Forms.SplashScreen;

namespace DeltaShell.Tests.Gui.Forms
{
    [TestFixture]
    public class SplashScreenTest
    {
        private static readonly MockRepository Mocks = new MockRepository();

        /// <summary>
        /// Shows SplashScreen, used to check if background image is scaled ok on non-100% DPI.
        /// </summary>
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void DpiTest()
        {
            var app = Mocks.Stub<IApplication>();
            app.Expect(a => a.GetUserSettingsDirectoryPath()).Repeat.Any().Return(TestHelper.GetCurrentMethodName());
            app.Expect(a => a.Version).Repeat.Once().Return("0.0.0.0");
            Mocks.ReplayAll(); // just replay, no verify required

            // action
            var splashScreen = new SplashScreen(app);

            WpfTestHelper.ShowModal(splashScreen);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowSplashScreenFunctionality()
        {
            var appSettings = new NameValueCollection
            {
                {"fullVersion", "1.0.0.0"},
                {"copyright", "© Deltares " + DateTime.Now.Year},
                {"company", "Deltares"},
                {"license", "Trial"},
                {"splashScreenLogoImageFilePath","SplashScreenBackground.png"}
            };

            var app = Mocks.Stub<IApplication>();
            app.Expect(a => a.GetUserSettingsDirectoryPath()).Repeat.Any().Return(TestHelper.GetCurrentMethodName());
            app.Settings = appSettings;
            Mocks.ReplayAll(); // just replay, no verify required

            var splashScreen = new SplashScreen(app)
            {
                LabelVersionVersion = app.Settings["fullVersion"],
                SplashScreenCopyright = app.Settings["copyright"],
                LabelLicense = app.Settings["license"],
                LabelCompany = app.Settings["company"],
                //LogoImageFilePath = app.Settings["splashScreenLogoImageFilePath"]
            };

            // action
            Action onShown = delegate
            {
                
                while (splashScreen.Visibility != Visibility.Visible)
                {
                    Application.DoEvents();
                }
                splashScreen.SetProgress(new SplashScreen.ProgressInfo()
                {
                    Message = "Initializing splashscreen",
                    ProgressText = "0 %, time left: 4.8 sec",
                    ProgressValue = 0
                });
                Application.DoEvents();
                Thread.Sleep(1000);
                splashScreen.SetProgress(new SplashScreen.ProgressInfo()
                {
                    Message = "Initializing Windows",
                    ProgressText = "38 %, time left: 3.8 sec",
                    ProgressValue = 38
                });
                Application.DoEvents();
                Thread.Sleep(1250);
                splashScreen.SetProgress(new SplashScreen.ProgressInfo()
                {
                    Message = "Loading data...",
                    ProgressText = "65 %, time left: 2.5 sec",
                    ProgressValue = 65
                });
                Application.DoEvents();
                Thread.Sleep(1250);
                splashScreen.SetProgress(new SplashScreen.ProgressInfo()
                {
                    Message = "Finalizing",
                    ProgressText = "85 %, time left: 1.1 sec",
                    ProgressValue = 85
                });
                Application.DoEvents();
                Thread.Sleep(1050);
                splashScreen.SetProgress(new SplashScreen.ProgressInfo()
                {
                    Message = "Cleaning",
                    ProgressText = "85 %, time left: 0.3 sec",
                    ProgressValue = 85
                });
                Application.DoEvents();
                Thread.Sleep(300);
                splashScreen.SetProgress(new SplashScreen.ProgressInfo()
                {
                    Message = "Done",
                    ProgressText = "100 %, time left:  N/A",
                    ProgressValue = 100
                });



            };
            WpfTestHelper.ShowModal(splashScreen, onShown);
        }
    }
}
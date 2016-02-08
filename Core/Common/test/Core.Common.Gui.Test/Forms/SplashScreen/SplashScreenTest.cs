using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using NUnit.Framework;

using Application = System.Windows.Forms.Application;

namespace Core.Common.Gui.Test.Forms.SplashScreen
{
    [TestFixture]
    public class SplashScreenTest
    {
        [Test]
        [RequiresSTA]
        public void ViewProperties_SetNewValues_ShouldSetLabelTextOfUserInterfaceElements()
        {
            // Setup
            var strCompany = "Cmp1";
            var strCopyright = "Copy1";
            var strLicense = "License1";
            var strVersion = "Version1";
            var strProgressText = "SomeProgress1";

            // Call
            var screen = new Gui.Forms.SplashScreen.SplashScreen
            {
                CompanyText = strCompany,
                CopyrightText = strCopyright,
                LicenseText = strLicense,
                VersionText = strVersion,
                ProgressText = strProgressText
            };
            screen.Show();

            // Assert
            Assert.AreEqual(strVersion, GetLabelText(screen, "labelVersion"));
            Assert.AreEqual(strCompany, GetLabelText(screen, "labelCompany"));
            Assert.AreEqual(strLicense, GetLabelText(screen, "labelLicense"));
            Assert.AreEqual(strCopyright, GetLabelText(screen, "labelCopyright"));
            Assert.AreEqual(strProgressText, GetLabelText(screen, "labelProgressMessage"));

            screen.Close();
        }

        [Test]
        [RequiresSTA]
        public void TestProgressBarVisible()
        {
            // Setup
            var screen = new Gui.Forms.SplashScreen.SplashScreen();
            screen.Show();
            Assert.IsTrue(screen.HasProgress, "Initially, the progress should be visible");
            Assert.IsTrue(GetIsControlVisible(screen, "progressBar"));
            Assert.IsTrue(GetIsControlVisible(screen, "labelProgressMessage"));
            Assert.IsTrue(GetIsControlVisible(screen, "labelProgressBar"));

            // Call
            screen.HasProgress = false;
            Application.DoEvents(); // creating space for lazy-updating to do its work

            // Assert
            Assert.IsFalse(screen.HasProgress, "HasProgress is changed to FALSE by now");

            Assert.IsFalse(GetIsControlVisible(screen, "progressBar"));
            Assert.IsFalse(GetIsControlVisible(screen, "labelProgressMessage"));
            Assert.IsFalse(GetIsControlVisible(screen, "labelProgressBar"));

            screen.Close();
        }

        private FrameworkElement FindControlRecursively(FrameworkElement parent, string name)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            FrameworkElement foundChild = null;
            for (var childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                var child = (FrameworkElement)VisualTreeHelper.GetChild(parent, childIndex);
                foundChild = child.Name == name ? child : FindControlRecursively(child, name);

                if (foundChild != null)
                {
                    break;
                }
            }

            return foundChild;
        }

        private string GetLabelText(FrameworkElement parent, string labelName)
        {
            var label = FindControlRecursively(parent, labelName) as Label;
            return (label != null) ? label.Content.ToString() : "";
        }

        private bool GetIsControlVisible(FrameworkElement parent, string ctrlName)
        {
            var ctrl = FindControlRecursively(parent, ctrlName) as Control;
            return ctrl != null && ctrl.IsVisible;
        }
    }
}
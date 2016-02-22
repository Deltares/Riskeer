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
        public void Shutdown_SplashScreenShown_ShouldBeClosed()
        {
            // Setup
            var screen = new Gui.Forms.SplashScreen.SplashScreen();
            var screenClosedCalled = false;
            screen.Closed += (sender, args) => { screenClosedCalled = true; };
            screen.Show();

            // Call
            screen.Shutdown();

            // Assert
            Assert.IsFalse(screen.IsVisible);
            Assert.IsTrue(screenClosedCalled);
        }

        [Test]
        [RequiresSTA]
        public void ViewProperties_SetNewValues_ShouldSetLabelTextOfUserInterfaceElements()
        {
            // Setup
            const string strCopyright = "Copy1";
            const string strLicense = "License1";
            const string strVersion = "Version1";
            const string strProgressText = "SomeProgress1";
            const string supportEmail = "<email>";
            const string supportPhone = "<phone>";

            // Call
            var screen = new Gui.Forms.SplashScreen.SplashScreen
            {
                CopyrightText = strCopyright,
                LicenseText = strLicense,
                VersionText = strVersion,
                ProgressText = strProgressText,
                SupportEmail = supportEmail,
                SupportPhoneNumber = supportPhone
            };
            screen.Show();

            // Assert
            Assert.AreEqual(strVersion, GetLabelText(screen, "LabelVersion"));
            Assert.AreEqual(strLicense, GetLabelText(screen, "LabelLicense"));
            Assert.AreEqual(strCopyright, GetLabelText(screen, "LabelCopyright"));
            Assert.AreEqual(strProgressText, GetLabelText(screen, "LabelProgressMessage"));
            Assert.AreEqual(supportEmail, GetLabelText(screen, "LabelSupportEmailAddress"));
            Assert.AreEqual(supportPhone, GetLabelText(screen, "LabelSupportPhoneNumber"));

            // Teardown
            screen.Close();
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        [RequiresSTA]
        public void ViewPropertiesVisibility_SupportValuesSet_SupportValuesShouldBeVisible(
            bool emailVisible, bool phoneVisible,
            bool supportUiElementsShouldBeVisible)
        {
            // Setup
            var supportEmail = emailVisible ? "<email>" : string.Empty;
            var supportPhone = phoneVisible ? "<phone>" : string.Empty;

            // Call
            var screen = new Gui.Forms.SplashScreen.SplashScreen
            {
                SupportPhoneNumber = supportPhone,
                SupportEmail = supportEmail
            };
            screen.Show();

            // Assert
            Assert.AreEqual(supportUiElementsShouldBeVisible, GetIsControlVisible(screen, "LabelSupportTitle"));
            Assert.AreEqual(supportUiElementsShouldBeVisible, GetIsControlVisible(screen, "LabelSupportPhoneNumberTitle"));
            Assert.AreEqual(supportUiElementsShouldBeVisible, GetIsControlVisible(screen, "LabelSupportPhoneNumber"));
            Assert.AreEqual(supportUiElementsShouldBeVisible, GetIsControlVisible(screen, "LabelSupportEmailAddressTitle"));
            Assert.AreEqual(supportUiElementsShouldBeVisible, GetIsControlVisible(screen, "LabelSupportEmailAddress"));

            // Teardown
            screen.Close();
        }

        [Test]
        [RequiresSTA]
        public void HasProgress_SetToFalse_HideProgressBarRelatedUserInterfaceElements()
        {
            // Setup
            var screen = new Gui.Forms.SplashScreen.SplashScreen();
            screen.Show();

            // Precondition:
            Assert.IsTrue(screen.HasProgress, "Initially, the progress should be visible");
            Assert.IsTrue(GetIsControlVisible(screen, "ProgressBar"));
            Assert.IsTrue(GetIsControlVisible(screen, "LabelProgressMessage"));
            Assert.IsTrue(GetIsControlVisible(screen, "LabelProgressBar"));

            // Call
            screen.HasProgress = false;
            Application.DoEvents(); // creating space for lazy-updating to do its work

            // Assert
            Assert.IsFalse(screen.HasProgress, "HasProgress is changed to FALSE by now");
            Assert.IsFalse(GetIsControlVisible(screen, "ProgressBar"));
            Assert.IsFalse(GetIsControlVisible(screen, "LabelProgressMessage"));
            Assert.IsFalse(GetIsControlVisible(screen, "LabelProgressBar"));

            // Teardown
            screen.Close();
        }

        private FrameworkElement FindControlRecursively(FrameworkElement parent, string name)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            FrameworkElement foundChild = null;
            for (var childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                var child = (FrameworkElement) VisualTreeHelper.GetChild(parent, childIndex);
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
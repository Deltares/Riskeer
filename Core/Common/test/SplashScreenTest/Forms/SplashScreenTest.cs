using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NUnit.Framework;

namespace Core.Common.Gui.Tests.Forms
{
    [TestFixture]
    public class SplashScreenTest
    {
        [Test]
        [RequiresSTA]
        public void TestFillsLabelsViaProperties()
        {
            var strCompany = "Cmp1";
            var strCopyright = "Copy1";
            var strLicense = "License1";
            var strVersion = "Version1";
            var strProgressText = "SomeProgress1";

            var screen = new Gui.Forms.SplashScreen.SplashScreen
            {
                CompanyText = strCompany,
                CopyrightText = strCopyright,
                LicenseText = strLicense,
                VersionText = strVersion,
                ProgressText = strProgressText
            };

            screen.Show();

            Assert.AreEqual(strVersion, GetLabelText(screen, "labelVersion"));
            Assert.AreEqual(strCompany, GetLabelText(screen, "labelCompany"));
            Assert.AreEqual(strLicense, GetLabelText(screen, "labelLicense"));
            Assert.AreEqual(strCopyright, GetLabelText(screen, "labelCopyright"));
            Assert.AreEqual(strProgressText, GetLabelText(screen, "labelProgressMessage"));
        }

        private FrameworkElement FindControlRecursively(FrameworkElement parent, string name)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            FrameworkElement foundChild = null;
            for (var childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                var child = VisualTreeHelper.GetChild(parent, childIndex) as FrameworkElement;
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
            var label = FindControlRecursively(parent, labelName);
            return (label as Label).Content.ToString();
        }
    }
}
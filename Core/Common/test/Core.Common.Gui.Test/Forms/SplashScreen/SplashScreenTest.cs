// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Forms.SplashScreen
{
    [TestFixture]
    public class SplashScreenTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Shutdown_SplashScreenShown_ShouldBeClosed()
        {
            // Setup
            var screen = new Gui.Forms.SplashScreen.SplashScreen();
            var screenClosedCalled = false;
            screen.Closed += (sender, args) => screenClosedCalled = true;
            screen.Show();

            // Call
            screen.Shutdown();

            // Assert
            Assert.IsFalse(screen.IsVisible);
            Assert.IsTrue(screenClosedCalled);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ViewProperties_SetNewValues_ShouldSetLabelTextOfUserInterfaceElements()
        {
            // Setup
            const string strVersion = "Version1";
            const string supportEmail = "<email>";
            const string supportPhone = "<phone>";

            // Call
            var screen = new Gui.Forms.SplashScreen.SplashScreen
            {
                VersionText = strVersion,
                SupportEmail = supportEmail,
                SupportPhoneNumber = supportPhone
            };
            screen.Show();

            // Assert
            Assert.AreEqual(strVersion, GetLabelText(screen, "LabelVersion"));
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
        [Apartment(ApartmentState.STA)]
        public void ViewPropertiesVisibility_SupportValuesSet_SupportValuesShouldBeVisible(
            bool emailVisible, bool phoneVisible,
            bool supportUiElementsShouldBeVisible)
        {
            // Setup
            string supportEmail = emailVisible ? "<email>" : string.Empty;
            string supportPhone = phoneVisible ? "<phone>" : string.Empty;

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

        private static FrameworkElement FindControlRecursively(FrameworkElement parent, string name)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
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

        private static string GetLabelText(FrameworkElement parent, string labelName)
        {
            var label = FindControlRecursively(parent, labelName) as Label;
            return label?.Content.ToString() ?? "";
        }

        private static bool GetIsControlVisible(FrameworkElement parent, string ctrlName)
        {
            var ctrl = FindControlRecursively(parent, ctrlName) as Control;
            return ctrl != null && ctrl.IsVisible;
        }
    }
}
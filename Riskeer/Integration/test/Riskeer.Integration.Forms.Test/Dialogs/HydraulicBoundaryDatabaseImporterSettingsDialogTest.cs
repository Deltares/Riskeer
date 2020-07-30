// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Helpers;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.Properties;
using Riskeer.Integration.Forms.Dialogs;

namespace Riskeer.Integration.Forms.Test.Dialogs
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseImporterSettingsDialogTest
    {
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("inquiryHelper", parameter);
            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ShowDialog_Always_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            using (var dialogParent = new Form())
            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.AreEqual(600, dialog.MinimumSize.Width);
                Assert.AreEqual(250, dialog.MinimumSize.Height);
                Assert.AreEqual("Koppel aan database", dialog.Text);

                Icon icon = BitmapToIcon(Resources.DatabaseIcon);
                Bitmap expectedImage = icon.ToBitmap();
                Bitmap actualImage = dialog.Icon.ToBitmap();
                TestHelper.AssertImagesAreEqual(expectedImage, actualImage);

                var buttonCancel = (Button) new ButtonTester("buttonCancel", dialog).TheObject;
                Assert.AreSame(buttonCancel, dialog.CancelButton);
                Assert.IsTrue(buttonCancel.Enabled);
            }
        }

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}

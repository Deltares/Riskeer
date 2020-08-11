﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Helpers;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.Properties;
using Riskeer.Integration.Forms.Dialogs;
using Riskeer.Integration.IO.Importers;

namespace Riskeer.Integration.Forms.Test.Dialogs
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseImporterSettingsDialogTest : NUnitFormTest
    {
        private static readonly string validHrdDirectory = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "HydraulicBoundaryDatabase");
        private static readonly string validHlcdFilePath = Path.Combine(validHrdDirectory, "HLCD.sqlite");
        private static readonly string validLocationsFilePath = Path.Combine(validHrdDirectory, "Locations.sqlite");

        private static IEnumerable<TestCaseData> TestCaseSettings
        {
            get
            {
                yield return new TestCaseData(null, false, "Kan niet koppelen aan database: er is geen HLCD bestand geselecteerd.");
                yield return new TestCaseData(new HydraulicBoundaryDatabaseImporterSettings(string.Empty, validHrdDirectory, validLocationsFilePath),
                                              false, "Kan niet koppelen aan database: er is geen HLCD bestand geselecteerd.");
                yield return new TestCaseData(new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, string.Empty, validLocationsFilePath),
                                              false, "Kan niet koppelen aan database: er is geen HRD bestandsmap geselecteerd.");
                yield return new TestCaseData(new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, validHrdDirectory, string.Empty),
                                              false, "Kan niet koppelen aan database: er is geen locatie bestand geselecteerd.");
                yield return new TestCaseData(new HydraulicBoundaryDatabaseImporterSettings("notExisting", validHrdDirectory, validLocationsFilePath),
                                              false, "Kan niet koppelen aan database: het geselecteerde HLCD bestand bestaat niet.");
                yield return new TestCaseData(new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, "notExisting", validLocationsFilePath),
                                              false, "Kan niet koppelen aan database: de geselecteerde HRD bestandsmap bestaat niet.");
                yield return new TestCaseData(new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, validHrdDirectory, "notExisting"),
                                              false, "Kan niet koppelen aan database: het geselecteerde locatie bestand bestaat niet.");
                yield return new TestCaseData(new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, validHrdDirectory, validLocationsFilePath),
                                              true, string.Empty);
            }
        }

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
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

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

                mockRepository.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ShowDialog_WithoutSettings_TextBoxesAsExpected()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper))
            {
                // Call
                dialog.Show();

                // Assert
                var textBoxHlcd = (TextBox) new ControlTester("textBoxHlcd", dialog).TheObject;
                Assert.AreEqual("<selecteer>", textBoxHlcd.Text);

                var textBoxHrd = (TextBox) new ControlTester("textBoxHrd", dialog).TheObject;
                Assert.AreEqual("<selecteer>", textBoxHrd.Text);

                var textBoxLocations = (TextBox) new ControlTester("textBoxLocations", dialog).TheObject;
                Assert.AreEqual("<selecteer>", textBoxLocations.Text);

                mockRepository.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ShowDialog_WithSettings_TextBoxesAsExpected()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            var settings = new HydraulicBoundaryDatabaseImporterSettings("path hlcd file", "path hrd directory", "path locations file");

            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper, settings))
            {
                // Call
                dialog.Show();

                // Assert
                var textBoxHlcd = (TextBox) new ControlTester("textBoxHlcd", dialog).TheObject;
                Assert.AreEqual(settings.HlcdFilePath, textBoxHlcd.Text);

                var textBoxHrd = (TextBox) new ControlTester("textBoxHrd", dialog).TheObject;
                Assert.AreEqual(settings.HrdDirectoryPath, textBoxHrd.Text);

                var textBoxLocations = (TextBox) new ControlTester("textBoxLocations", dialog).TheObject;
                Assert.AreEqual(settings.LocationsFilePath, textBoxLocations.Text);

                mockRepository.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCaseSource(nameof(TestCaseSettings))]
        public void ShowDialog_WithSettings_ButtonConnectStateAndErrorProviderAsExpected(HydraulicBoundaryDatabaseImporterSettings settings,
                                                                                         bool expectedEnabledState,
                                                                                         string expectedErrorMessage)
        {
            // Setup
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper, settings))
            {
                // Call
                dialog.Show();

                // Assert
                var buttonConnect = (Button) new ButtonTester("buttonConnect", dialog).TheObject;
                Assert.AreEqual(expectedEnabledState, buttonConnect.Enabled);

                var errorProvider = TypeUtils.GetField<ErrorProvider>(dialog, "errorProvider");
                Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(buttonConnect));

                mockRepository.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidDialog_WhenCancelButtonClicked_ThenSelectedSettingsNull()
        {
            // Given
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    new ButtonTester("buttonCancel", name).Click();
                }
            };

            var settings = new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, validHrdDirectory, validLocationsFilePath);

            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper, settings))
            {
                // When
                DialogResult dialogResult = dialog.ShowDialog();

                // Then
                Assert.AreEqual(DialogResult.Cancel, dialogResult);
                Assert.IsNull(dialog.SelectedSettings);

                mockRepository.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidDialog_WhenConnectButtonClicked_ThenSelectedSettingsAsExpected()
        {
            // Given
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    new ButtonTester("buttonConnect", name).Click();
                }
            };

            var settings = new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, validHrdDirectory, validLocationsFilePath);

            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper, settings))
            {
                // When
                DialogResult dialogResult = dialog.ShowDialog();

                // Then
                Assert.AreEqual(DialogResult.OK, dialogResult);
                Assert.IsNotNull(dialog.SelectedSettings);
                Assert.AreEqual(validHlcdFilePath, dialog.SelectedSettings.HlcdFilePath);
                Assert.AreEqual(validHrdDirectory, dialog.SelectedSettings.HrdDirectoryPath);
                Assert.AreEqual(validLocationsFilePath, dialog.SelectedSettings.LocationsFilePath);

                mockRepository.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenDialog_WhenHlcdButtonClicked_ThenSelectedDirectoryPathSetToTextBoxAndButtonConnectUpdated()
        {
            // Given
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation("HLCD bestand (*.sqlite)|*.sqlite")).Return(validHlcdFilePath);
            mockRepository.ReplayAll();

            var settings = new HydraulicBoundaryDatabaseImporterSettings(string.Empty, validHrdDirectory, validLocationsFilePath);

            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper, settings))
            {
                dialog.Show();

                // Precondition
                var buttonConnect = (Button) new ButtonTester("buttonConnect", dialog).TheObject;
                Assert.IsFalse(buttonConnect.Enabled);

                // When
                var button = new ButtonTester("buttonHlcd", dialog);
                button.Click();

                // Then
                var textBoxHlcd = (TextBox) new ControlTester("textBoxHlcd", dialog).TheObject;
                Assert.AreEqual(validHlcdFilePath, textBoxHlcd.Text);

                Assert.IsTrue(buttonConnect.Enabled);

                mockRepository.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenDialog_WhenHrdButtonClicked_ThenSelectedDirectoryPathSetToTextBoxAndButtonConnectUpdated()
        {
            // Given
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetTargetFolderLocation()).Return(validHrdDirectory);
            mockRepository.ReplayAll();

            var settings = new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, string.Empty, validLocationsFilePath);

            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper, settings))
            {
                dialog.Show();

                // Precondition
                var buttonConnect = (Button) new ButtonTester("buttonConnect", dialog).TheObject;
                Assert.IsFalse(buttonConnect.Enabled);

                // When
                var button = new ButtonTester("buttonHrd", dialog);
                button.Click();

                // Then
                var textBoxHrd = (TextBox) new ControlTester("textBoxHrd", dialog).TheObject;
                Assert.AreEqual(validHrdDirectory, textBoxHrd.Text);

                Assert.IsTrue(buttonConnect.Enabled);

                mockRepository.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenDialog_WhenLocationsButtonClicked_ThenSelectedDirectoryPathSetToTextBoxAndButtonConnectUpdated()
        {
            // Given
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation("Locatie bestand (*.sqlite)|*.sqlite")).Return(validLocationsFilePath);
            mockRepository.ReplayAll();

            var settings = new HydraulicBoundaryDatabaseImporterSettings(validHlcdFilePath, validHrdDirectory, string.Empty);

            using (var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper, settings))
            {
                dialog.Show();

                // Precondition
                var buttonConnect = (Button) new ButtonTester("buttonConnect", dialog).TheObject;
                Assert.IsFalse(buttonConnect.Enabled);

                // When
                var button = new ButtonTester("buttonLocations", dialog);
                button.Click();

                // Then
                var textBoxLocations = (TextBox) new ControlTester("textBoxLocations", dialog).TheObject;
                Assert.AreEqual(validLocationsFilePath, textBoxLocations.Text);

                Assert.IsTrue(buttonConnect.Enabled);

                mockRepository.VerifyAll();
            }
        }

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}
// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using Core.Common.Gui.Settings;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.MainWindow
{
    [TestFixture]
    public class MainWindowTest
    {
        private MessageWindowLogAppender originalValue;

        [SetUp]
        public void SetUp()
        {
            originalValue = MessageWindowLogAppender.Instance;
            MessageWindowLogAppender.Instance = new MessageWindowLogAppender();
        }

        [TearDown]
        public void TearDown()
        {
            MessageWindowLogAppender.Instance = originalValue;
        }

        [Test]
        [STAThread]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Assert
                Assert.IsInstanceOf<IMainWindow>(mainWindow);
                Assert.IsInstanceOf<IDisposable>(mainWindow);
                Assert.IsInstanceOf<ISynchronizeInvoke>(mainWindow);

                Assert.IsFalse(mainWindow.IsWindowDisposed);
                Assert.IsFalse(mainWindow.Visible);

                Assert.IsNull(mainWindow.MessageWindow);
                Assert.IsNull(mainWindow.PropertyGrid);

                Assert.AreEqual(string.Empty, mainWindow.StatusBarMessage);

                Assert.IsNotNull(mainWindow.Handle);
                Assert.IsFalse(mainWindow.InvokeRequired,
                               "'mainWindow' instance on same thread as test, therefore invocation not required.");

                Assert.AreEqual("MainWindow", mainWindow.Title);
                Assert.AreEqual(ResizeMode.CanResizeWithGrip, mainWindow.ResizeMode);
                Assert.AreEqual(FlowDirection.LeftToRight, mainWindow.FlowDirection);
            }
        }

        [Test]
        [STAThread]
        public void Dispose_SetIsWindowDisposedTrue()
        {
            // Setup
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                mainWindow.Dispose();

                // Assert
                Assert.IsTrue(mainWindow.IsWindowDisposed);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [STAThread]
        public void Visible_SettingValueWithoutHavingSetGui_ThrowInvalidOperationException(bool newVisibleValue)
        {
            // Setup
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                TestDelegate call = () => mainWindow.Visible = newVisibleValue;

                // Assert
                Assert.Throws<InvalidOperationException>(call);
            }
        }

        [Test]
        [STAThread]
        public void Visible_SetToTrue_ShowMainForm()
        {
            // Setup
            var mocks = new MockRepository();
            var settings = mocks.Stub<ApplicationSettingsBase>();

            var viewHost = mocks.Stub<IViewHost>();
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.UserSettings).Return(settings);
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            gui.Stub(g => g.Plugins).Return(Enumerable.Empty<PluginBase>().ToList());
            gui.Stub(g => g.ViewHost).Return(viewHost);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.Visible = true;

                // Assert
                Assert.IsTrue(mainWindow.Visible);
                Assert.IsTrue(mainWindow.IsVisible);
                Assert.AreEqual(Visibility.Visible, mainWindow.Visibility);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void Visible_SetToFalse_HideMainForm()
        {
            // Setup
            var mocks = new MockRepository();
            var settings = mocks.Stub<ApplicationSettingsBase>();

            var viewHost = mocks.Stub<IViewHost>();
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.UserSettings).Return(settings);
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            gui.Stub(g => g.Plugins).Return(Enumerable.Empty<PluginBase>().ToList());
            gui.Stub(g => g.ViewHost).Return(viewHost);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);
                mainWindow.Visible = true;

                // Call
                mainWindow.Visible = false;

                // Assert
                Assert.IsFalse(mainWindow.Visible);
                Assert.IsFalse(mainWindow.IsVisible);
                Assert.AreEqual(Visibility.Hidden, mainWindow.Visibility);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void StatusBarMessage_SetValue_ReturnNewlySetValue()
        {
            // Setup
            const string message = "<some message>";
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                mainWindow.StatusBarMessage = message;

                // Assert
                Assert.AreEqual(message, TypeUtils.GetField<TextBlock>(mainWindow, "StatusMessageTextBlock").Text);
                Assert.AreEqual(message, mainWindow.StatusBarMessage);
            }
        }

        [Test]
        [STAThread]
        public void SubscribeToGui_NoGuiSet_DoNothing()
        {
            // Setup
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                TestDelegate call = () => mainWindow.SubscribeToGui();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        [Test]
        [STAThread]
        public void SubscribeToGui_GuiSet_AttachEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var viewHost = mocks.Stub<IViewHost>();
            viewHost.Expect(vm => vm.ActiveDocumentViewChanged += null).IgnoreArguments();
            viewHost.Expect(vm => vm.ActiveDocumentViewChanging += null).IgnoreArguments(); // Should happen during dispose

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.SubscribeToGui();
            }
            // Assert
            mocks.VerifyAll(); // Expect event subscription
        }

        [Test]
        [STAThread]
        public void UnsubscribeFromGui_NoGuiSet_DoNothing()
        {
            // Setup
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                TestDelegate call = () => mainWindow.UnsubscribeFromGui();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        [Test]
        [STAThread]
        public void UnsubscribeFromGui_GuiSetAndSubscribed_DetachEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var viewHost = mocks.Stub<IViewHost>();
            viewHost.Expect(l => l.ActiveDocumentViewChanged += null).IgnoreArguments();
            viewHost.Expect(l => l.ActiveDocumentViewChanging += null).IgnoreArguments();
            viewHost.Expect(l => l.ActiveDocumentViewChanged -= null).IgnoreArguments();
            viewHost.Expect(l => l.ActiveDocumentViewChanging -= null).IgnoreArguments();

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);
                mainWindow.SubscribeToGui();

                // Call
                mainWindow.UnsubscribeFromGui();
            }
            // Assert
            mocks.VerifyAll(); // Expect event subscription and desubscription
        }

        [Test]
        [STAThread]
        public void InitPropertiesWindowAndActivate_GuiNotSet_ThrowInvalidOperationException()
        {
            // Setup
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                TestDelegate call = () => mainWindow.InitPropertiesWindowAndActivate();

                // Assert
                Assert.Throws<InvalidOperationException>(call);
            }
        }

        [Test]
        [STAThread]
        public void InitPropertiesWindowAndActivate_GuiSet_InitializePropertyGrid()
        {
            // Setup
            var selectedObject = new object();

            var mocks = new MockRepository();
            var selectedObjectProperties = mocks.Stub<IObjectProperties>();

            var viewHost = new AvalonDockViewHost();

            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(selectedObject))
                            .Return(selectedObjectProperties);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Selection = selectedObject;
            gui.Stub(g => g.PropertyResolver).Return(propertyResolver);
            gui.Stub(g => g.SelectionChanged += null).IgnoreArguments();
            gui.Stub(g => g.SelectionChanged -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.InitPropertiesWindowAndActivate();

                // Assert
                Assert.IsNull(viewHost.ActiveDocumentView);
                Assert.AreSame(viewHost.ToolViews.ElementAt(0), mainWindow.PropertyGrid, "PropertyGrid instance should remain the same.");
                Assert.AreEqual("Eigenschappen", mainWindow.PropertyGrid.Text);
                Assert.AreEqual(selectedObjectProperties, mainWindow.PropertyGrid.Data);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void InitPropertiesWindowAndActivate_GuiSetAndCalledTwice_UpdateAlreadyExistingPropertyGridViewInstance()
        {
            // Setup
            var selectedObject = new object();

            var mocks = new MockRepository();
            var selectedObjectProperties = mocks.Stub<IObjectProperties>();
            var viewHost = new AvalonDockViewHost();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(selectedObject))
                            .Return(selectedObjectProperties)
                            .Repeat.Twice();

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Selection = selectedObject;
            gui.Stub(g => g.PropertyResolver).Return(propertyResolver);
            gui.Stub(g => g.SelectionChanged += null).IgnoreArguments();
            gui.Stub(g => g.SelectionChanged -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);
                mainWindow.InitPropertiesWindowAndActivate();

                var originalPropertyGrid = mainWindow.PropertyGrid;

                // Call
                mainWindow.InitPropertiesWindowAndActivate();

                // Assert
                Assert.IsNull(viewHost.ActiveDocumentView);
                Assert.AreSame(originalPropertyGrid, mainWindow.PropertyGrid, "PropertyGrid instance should remain the same.");
                Assert.AreEqual("Eigenschappen", mainWindow.PropertyGrid.Text);
                Assert.AreEqual(selectedObjectProperties, mainWindow.PropertyGrid.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void InitializeToolWindows_GuiNotSet_ThrowInvalidOperationException()
        {
            // Setup
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                TestDelegate call = () => mainWindow.InitializeToolWindows();

                // Assert
                Assert.Throws<InvalidOperationException>(call);
            }
        }

        [Test]
        [STAThread]
        public void InitializeToolWindows_GuiSet_InitializePropertyGridAndMessageWindowAndMakeActive()
        {
            // Setup
            var selectedObject = new object();

            var mocks = new MockRepository();
            var selectedObjectProperties = mocks.Stub<IObjectProperties>();
            var viewHost = new AvalonDockViewHost();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(selectedObject))
                            .Return(selectedObjectProperties);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Selection = selectedObject;
            gui.Stub(g => g.PropertyResolver).Return(propertyResolver);
            gui.Stub(g => g.SelectionChanged += null).IgnoreArguments();
            gui.Stub(g => g.SelectionChanged -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.InitializeToolWindows();

                // Assert
                Assert.IsInstanceOf<Gui.Forms.PropertyGridView.PropertyGridView>(mainWindow.PropertyGrid);
                Assert.AreEqual("Eigenschappen", mainWindow.PropertyGrid.Text);
                Assert.AreEqual(selectedObjectProperties, mainWindow.PropertyGrid.Data);

                Assert.IsInstanceOf<Gui.Forms.MessageWindow.MessageWindow>(mainWindow.MessageWindow);
                Assert.AreEqual("Berichten", mainWindow.MessageWindow.Text);

                Assert.IsNull(viewHost.ActiveDocumentView);
            }
            mocks.VerifyAll();
        }

        [Test]
        [STAThread]
        public void ShowStartPage_IgnoreUserSettings_OpenViewForWebPage()
        {
            // Setup
            const string pageName = "<google page name>";
            var fixedSettings = new GuiCoreSettings
            {
                StartPageUrl = "http://www.google.nl"
            };

            var mocks = new MockRepository();

            var userSettings = mocks.Stub<ApplicationSettingsBase>();
            userSettings["startPageName"] = pageName;

            var viewCommands = mocks.Stub<IViewCommands>();
            viewCommands.Expect(c => c.OpenView(Arg<WebLink>.Matches(link => link.Name == pageName &&
                                                                             link.Path == new Uri(fixedSettings.StartPageUrl))));

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.FixedSettings).Return(fixedSettings);
            gui.Stub(g => g.UserSettings).Return(userSettings);
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.ShowStartPage(false);
            }
            // Assert
            mocks.VerifyAll(); // Assert that expectancies are met
        }

        [Test]
        [STAThread]
        [TestCase(null)]
        [TestCase("")]
        public void ShowStartPage_IgnoreUserSettingsAndNoStartPageUrl_OpenViewForAboutBlank(string startPageNameValue)
        {
            // Setup
            var fixedSettings = new GuiCoreSettings
            {
                StartPageUrl = "about:blank"
            };

            var mocks = new MockRepository();

            var userSettings = mocks.Stub<ApplicationSettingsBase>();
            userSettings["startPageName"] = startPageNameValue;

            var viewCommands = mocks.Stub<IViewCommands>();
            viewCommands.Expect(c => c.OpenView(Arg<WebLink>.Matches(link => link.Name == startPageNameValue &&
                                                                             link.Path == new Uri(fixedSettings.StartPageUrl))));

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.FixedSettings).Return(fixedSettings);
            gui.Stub(g => g.UserSettings).Return(userSettings);
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.ShowStartPage(false);
            }
            // Assert
            mocks.VerifyAll(); // Assert that expectancies are met
        }

        [Test]
        [STAThread]
        public void ShowStartPage_FromUserSettings_OpenViewForWebPage()
        {
            // Setup
            const string pageName = "<google page name>";
            var fixedSettings = new GuiCoreSettings
            {
                StartPageUrl = "http://www.google.nl"
            };

            var mocks = new MockRepository();

            var userSettings = mocks.Stub<ApplicationSettingsBase>();
            userSettings["startPageName"] = pageName;
            userSettings["showStartPage"] = true.ToString();

            var viewCommands = mocks.Stub<IViewCommands>();
            viewCommands.Expect(c => c.OpenView(Arg<WebLink>.Matches(link => link.Name == pageName &&
                                                                             link.Path == new Uri(fixedSettings.StartPageUrl))));

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.FixedSettings).Return(fixedSettings);
            gui.Stub(g => g.UserSettings).Return(userSettings);
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.ShowStartPage();
            }
            // Assert
            mocks.VerifyAll(); // Assert that expectancies are met
        }

        [Test]
        [STAThread]
        public void ShowStartPage_FromUserSettingsWithShowStartPageFalse_DoNothing()
        {
            // Setup
            var fixedSettings = new GuiCoreSettings();

            var mocks = new MockRepository();

            var userSettings = mocks.Stub<ApplicationSettingsBase>();
            userSettings["showStartPage"] = false.ToString();

            var viewCommands = mocks.StrictMock<IViewCommands>();

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.FixedSettings).Return(fixedSettings);
            gui.Stub(g => g.UserSettings).Return(userSettings);
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.ShowStartPage();
            }
            // Assert
            mocks.VerifyAll(); // Assert that expectancies are met
        }

        [Test]
        [STAThread]
        public void SetWaitCursorOn_SetMouseOverrideToWait()
        {
            // Setup
            Cursor originalValue = Mouse.OverrideCursor;

            try
            {
                using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
                {
                    // Call
                    mainWindow.SetWaitCursorOn();

                    // Assert
                    Assert.AreEqual(Cursors.Wait, Mouse.OverrideCursor);
                }
            }
            finally
            {
                Mouse.OverrideCursor = originalValue;
            }
        }

        [Test]
        [STAThread]
        public void SetWaitCursorOff_SetMouseOverrideToNull()
        {
            // Setup
            Cursor originalValue = Mouse.OverrideCursor;

            try
            {
                using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
                {
                    mainWindow.SetWaitCursorOn();

                    // Call
                    mainWindow.SetWaitCursorOff();

                    // Assert
                    Assert.IsNull(Mouse.OverrideCursor);
                }
            }
            finally
            {
                Mouse.OverrideCursor = originalValue;
            }
        }
    }
}
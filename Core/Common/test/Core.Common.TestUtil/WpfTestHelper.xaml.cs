// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Windows;
using System.Windows.Controls;
using log4net;
using Application = System.Windows.Forms.Application;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Interaction logic for WpfTestHelper.xaml
    /// </summary>
    public partial class WpfTestHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WindowsFormsTestHelper));

        private static Window window;
        private Action shownAction;
        private bool wasShown;

        public WpfTestHelper()
        {
            InitializeComponent();
        }

        public static void ShowModal(Control control, params object[] propertyObjects)
        {
            new WpfTestHelper().ShowTopLevel(control, propertyObjects, true, null);
        }

        public static void ShowModal(Control control, Action formVisibleChangedAction, params object[] propertyObjects)
        {
            try
            {
                new WpfTestHelper().ShowTopLevel(control, propertyObjects, true, formVisibleChangedAction);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                throw;
            }
        }

        private void ShowTopLevel(Control control, object[] propertyObjects, bool modal, Action onShownAction)
        {
            ThrowIfPropertyObjectsContainsActionDueToLikelyMisuse(propertyObjects);

            GuiTestHelper.Initialize();

            shownAction = onShownAction;

            if (control is Window)
            {
                ShowTopLevelControl(control, modal);
            }
            else
            {
                ShowControlInTestForm(modal);
            }

            // clear all controls shown as non-modal after modal control closes 
            if (!modal)
            {
                throw new NotImplementedException();
            }

            WindowsFormsTestHelper.CloseAll(); // just in case, since we have mixed WPF / WF app

            Close();

            if (window != null)
            {
                window.Closed += WindowOnClosed;

                window.Close();
                window.Close();

                while (window.IsVisible)
                {
                    Application.DoEvents();
                    Application.DoEvents();
                    window.Close();
                }

                window.Closed -= WindowOnClosed;
                window = null;
            }
        }

        private static void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            Console.WriteLine(@"Closed");
        }

        private void ThrowIfPropertyObjectsContainsActionDueToLikelyMisuse(object[] propertyObjects)
        {
            if (propertyObjects.Length > 0)
            {
                var firstAsAction = propertyObjects[0] as Action;
                if (firstAsAction != null)
                {
                    throw new InvalidOperationException(
                        "Warning, you've given an Action (class) as argument, but it is being treated as a property object. Check your arguments");
                }
            }
        }

        private void ShowControlInTestForm(bool modal)
        {
            IsVisibleChanged += delegate { wasShown = true; };

            Show();

            WaitOrExit(this, modal);
        }

        private void ShowTopLevelControl(Control control, bool modal)
        {
            var newWindow = control as Window;
            if (newWindow != null)
            {
                window = newWindow;
            }
            else
            {
                window = new Window
                {
                    Content = control
                };
            }

            window.IsVisibleChanged += delegate
            {
                if (window.IsVisible)
                {
                    wasShown = true;
                }
            };
            window.ContentRendered += delegate { wasShown = true; };

            window.Show();

            WaitOrExit(window, modal);
        }

        private void WaitOrExit(Control control, bool modal)
        {
            // wait until control is shown
            while (!wasShown && GuiTestHelper.Exception == null)
            {
                Application.DoEvents();
            }

            // is shown, not trigger action
            try
            {
                Application.DoEvents();

                if (shownAction != null && wasShown)
                {
                    shownAction();
                }
            }
            finally
            {
                shownAction = null;
            }

            // if not on build server - wait until control is closed
            if (!GuiTestHelper.IsBuildServer && modal)
            {
                while (control.IsVisible)
                {
                    Application.DoEvents();
                }
            }

            if (GuiTestHelper.Exception != null)
            {
                GuiTestHelper.RethrowUnhandledException();
            }
        }
    }
}
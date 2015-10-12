using System;
using System.Windows;
using System.Windows.Forms;
using log4net;
using Application = System.Windows.Forms.Application;
using Control = System.Windows.Controls.Control;

namespace DelftTools.TestUtils
{
    /// <summary>
    /// Interaction logic for WpfTestHelper.xaml
    /// </summary>
    public partial class WpfTestHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WindowsFormsTestHelper));

        private static Window window;

        private Action shownAction;

        private readonly GuiTestHelper guiTestHelper;

        private bool wasShown;

        public WpfTestHelper()
        {
            InitializeComponent();
            guiTestHelper = GuiTestHelper.Instance;
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

        public static void ShowModal(Control control, Action<Form> formVisibleChangedAction, params object[] propertyObjects)
        {
            throw new InvalidOperationException("Use Action instead of Action<Form> (WPF)");
        }

        private void ShowTopLevel(Control control, object[] propertyObjects, bool modal, Action shownAction)
        {
            ThrowIfPropertyObjectsContainsActionDueToLikelyMisuse(propertyObjects);

            GuiTestHelper.Initialize();

            this.shownAction = shownAction;

            if (control is Window)
            {
                ShowTopLevelControl(control, modal);
            }
            else
            {
                ShowControlInTestForm(control, modal, propertyObjects);
            }

            // clear all controls shown as non-modal after modal control closes 
            if (!modal)
            {
                throw new NotImplementedException();
            }
            else
            {
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
        }

        private static void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Closed");
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

        private void ShowControlInTestForm(Control control, bool modal, object[] propertyObjects)
        {
            IsVisibleChanged += delegate { wasShown = true; };

            Show();

            WaitOrExit(this, modal);
        }

        private void ShowTopLevelControl(Control control, bool modal)
        {
            if (control is Window)
            {
                window = (Window) control;
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
            while (!wasShown && guiTestHelper.Exception == null)
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

            if (guiTestHelper.Exception != null)
            {
                guiTestHelper.RethrowUnhandledException();
            }
        }
    }
}
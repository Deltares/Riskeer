using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace DelftTools.TestUtils
{
    public partial class WindowsFormsTestHelper : Form
    {
        private static ILog log = LogManager.GetLogger(typeof(WindowsFormsTestHelper));

        private static string nonModalControlsTestName; // current unit test name
        private static readonly IList<Control> nonModalControls = new List<Control>();
        private Action<Form> formShown;
        private readonly GuiTestHelper guiTestHelper;
        private bool wasShown;

        public WindowsFormsTestHelper()
        {
            Application.EnableVisualStyles();

            CheckForIllegalCrossThreadCalls = true;
            Application.EnableVisualStyles();
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
            InitializeComponent();

            guiTestHelper = GuiTestHelper.Instance;
        }

        public static object[] PropertyObjects { get; set; }

        public PropertyGrid PropertyGrid { get; private set; }

        public static void ShowPropertyGridForObject(object selectedObject)
        {
            var mockrepos = new MockRepository();

            /* TODO: review with Bas
            var guiMock = mockrepos.Stub<IGui>();
            var grid = new DeltaShell.Gui.Forms.PropertyGrid.PropertyGrid(guiMock) { Data = new DynamicPropertyBag(selectedObject) };
            */

            var grid = new PropertyGrid
            {
                SelectedObject = selectedObject
            };

            ShowModal(grid);
        }

        public static void Show(Control control, Action<Form> formVisibleChangedAction, params object[] propertyObjects)
        {
            new WindowsFormsTestHelper().ShowTopLevel(control, propertyObjects, false, formVisibleChangedAction);
        }

        public static void Show(Control control, params object[] propertyObjects)
        {
            new WindowsFormsTestHelper().ShowTopLevel(control, propertyObjects, false, null);
        }

        public static void ShowModal(IEnumerable<Control> controls, params object[] propertyObjects)
        {
            ShowModal(controls, true, propertyObjects);
        }

        public static void ShowModal(Control control, params object[] propertyObjects)
        {
            new WindowsFormsTestHelper().ShowTopLevel(control, propertyObjects, true, null);
        }

        public static void ShowModal(Control control, Action<Form> formVisibleChangedAction,
                                     params object[] propertyObjects)
        {
            new WindowsFormsTestHelper().ShowTopLevel(control, propertyObjects, true, formVisibleChangedAction);
        }

        /// <summary>
        ///     Embeds the controls into one container control, and shows this control modally.
        ///     All controls will be using dockstyle top, exect the last control (if useFillForLastControl is true).
        /// </summary>
        /// <param name="controls">Controls to embed</param>
        /// <param name="useFillForLastControl">If true, the last control will be using dockstyle fill</param>
        /// <param name="propertyObjects">Objects to show as property</param>
        public static void ShowModal(IEnumerable<Control> controls, bool useFillForLastControl,
                                     params object[] propertyObjects)
        {
            var containerControl = new Control();
            int numberOfControls = 0;

            foreach (Control control in controls)
            {
                control.Dock = DockStyle.Top;
                containerControl.Controls.Add(control);
                numberOfControls++;
            }

            if (useFillForLastControl)
            {
                containerControl.Controls[numberOfControls - 1].Dock = DockStyle.Fill;
            }

            ShowModal(containerControl, propertyObjects);
        }

        public static void CloseAll()
        {
            foreach (var control in nonModalControls)
            {
                control.Hide();
                control.Dispose();
            }

            nonModalControls.Clear();

            nonModalControlsTestName = string.Empty;
        }

        private void ShowTopLevel(Control control, object[] propertyObjects, bool modal, Action<Form> shownAction)
        {
            //if (TestContext.CurrentContext.Test.Properties["_CATEGORIES"])

            ThrowIfPropertyObjectsContainsActionDueToLikelyMisuse(propertyObjects);

            GuiTestHelper.Initialize();

            formShown = shownAction;

            if (control.TopLevelControl == control)
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
                var testName = TestContext.CurrentContext.Test.FullName;

                if (string.IsNullOrEmpty(nonModalControlsTestName))
                {
                    nonModalControlsTestName = testName;
                }
                else
                {
                    if (nonModalControlsTestName != testName)
                    {
                        var errorMessage = string.Format("Did you forget to call WindowsFormsTestHelper.CloseAll() at the end of the following test: {0}?", nonModalControlsTestName);
                        nonModalControlsTestName = testName; // reset for the next test
                        throw new InvalidOperationException(errorMessage);
                    }
                }

                nonModalControls.Add(this);
            }
            else
            {
                CloseAll();

                Close();
                Dispose();

                control.Dispose();
            }
        }

        private void ThrowIfPropertyObjectsContainsActionDueToLikelyMisuse(object[] propertyObjects)
        {
            if (propertyObjects.Length > 0)
            {
                var firstAsAction = propertyObjects[0] as Action;
                if (firstAsAction != null)
                {
                    throw new InvalidOperationException(
                        "Warning, you've given an Action (class) as argument, but it is being treated as a property object. Use Action<Form>");
                }
            }
        }

        private void ShowControlInTestForm(Control control, bool modal, object[] propertyObjects)
        {
            PropertyObjects = propertyObjects;
            PropertyGrid.SelectedObject = control;
            control.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(control);

            InitializeTree(control);

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            Paint += delegate { wasShown = true; };
            VisibleChanged += delegate { wasShown = true; };

            Show();

            WaitOrExit(this, modal);
        }

        private void ShowTopLevelControl(Control control, bool modal)
        {
            control.Paint += delegate { wasShown = true; };
            control.VisibleChanged += delegate { wasShown = true; };

            control.Show();

            WaitOrExit(control, modal);
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

                if (formShown != null && wasShown)
                {
                    formShown(control is Form ? (Form) control : this);
                }
            }
            finally
            {
                formShown = null;
            }

            // if not on build server - wait until control is closed
            if (!GuiTestHelper.IsBuildServer && modal)
            {
                while (control.Visible)
                {
                    Application.DoEvents();
                }
            }

            if (guiTestHelper.Exception != null)
            {
                guiTestHelper.RethrowUnhandledException();
            }
        }

        private void InitializeTree(Control control)
        {
            IList itemsToShow = new RootNode
            {
                control
            };
            foreach (object o in PropertyObjects)
            {
                itemsToShow.Add(o);
            }

            treeView1.ImageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };
            treeView1.ImageList.Images.Add("Control", Resources.Control);
            treeView1.ImageList.Images.Add("Data", Resources.Data);

            AddAllNodes(treeView1.Nodes, itemsToShow);

            treeView1.NodeMouseClick += delegate { PropertyGrid.SelectedObject = treeView1.SelectedNode.Tag; };
        }

        private void AddAllNodes(TreeNodeCollection nodes, IEnumerable itemsToShow)
        {
            foreach (object item in itemsToShow.Cast<object>().Where(i => i != null))
            {
                int imageIndex = item is Control ? 0 : 1;
                var node = new TreeNode(item.ToString(), imageIndex, imageIndex)
                {
                    Tag = item
                };
                nodes.Add(node);

                if (item is Control)
                {
                    var control = ((Control) item);
                    AddAllNodes(node.Nodes, control.Controls);

                    // hack, try to get Data or DataSource property
                    PropertyInfo dataProperty = null;
                    try
                    {
                        dataProperty = control.GetType().GetProperty("Data");
                    }
                    catch (Exception) {}

                    if (dataProperty != null)
                    {
                        AddAllNodes(node.Nodes, new[]
                        {
                            dataProperty.GetValue(control, null)
                        });
                    }

                    PropertyInfo dataSourceProperty = control.GetType().GetProperty("DataSource");
                    if (dataSourceProperty != null)
                    {
                        AddAllNodes(node.Nodes, new[]
                        {
                            dataSourceProperty.GetValue(control, null)
                        });
                    }
                }

                if (item is IEnumerable)
                {
                    AddAllNodes(node.Nodes, (IEnumerable) item);
                }
            }
        }

        private class RootNode : ArrayList
        {
            public override string ToString()
            {
                return "RootNode";
            }
        }
    }
}
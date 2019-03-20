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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Common.TestUtil.Properties;
using NUnit.Framework;

namespace Core.Common.TestUtil
{
    public partial class WindowsFormsTestHelper : Form
    {
        private static string nonModalControlsTestName; // current unit test name
        private static readonly List<Control> nonModalControls = new List<Control>();
        private Action<Form> formShown;
        private bool wasShown;

        private WindowsFormsTestHelper()
        {
            Application.EnableVisualStyles();

            CheckForIllegalCrossThreadCalls = true;
            Application.EnableVisualStyles();
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
            InitializeComponent();
        }

        public static IEnumerable<object> PropertyObjects { get; set; }

        public PropertyGrid PropertyGrid { get; private set; }

        public static void Show(Control control, Action<Form> formVisibleChangedAction, params object[] propertyObjects)
        {
            new WindowsFormsTestHelper().ShowTopLevel(control, propertyObjects, false, formVisibleChangedAction);
        }

        public static void Show(Control control, params object[] propertyObjects)
        {
            new WindowsFormsTestHelper().ShowTopLevel(control, propertyObjects, false, null);
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

        public static void CloseAll()
        {
            foreach (Control control in nonModalControls)
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
                string testName = TestContext.CurrentContext.Test.FullName;

                if (string.IsNullOrEmpty(nonModalControlsTestName))
                {
                    nonModalControlsTestName = testName;
                }
                else
                {
                    if (nonModalControlsTestName != testName)
                    {
                        string errorMessage = $"Did you forget to call WindowsFormsTestHelper.CloseAll() at the end of the following test: {nonModalControlsTestName}?";
                        nonModalControlsTestName = testName; // reset for the next test
                        throw new InvalidOperationException(errorMessage);
                    }
                }

                nonModalControls.Add(this);
            }
            else
            {
                CloseAll();

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

            Paint += delegate
            {
                wasShown = true;
            };
            VisibleChanged += delegate
            {
                wasShown = true;
            };

            Show();

            WaitOrExit(this, modal);
        }

        private void ShowTopLevelControl(Control control, bool modal)
        {
            control.Paint += delegate
            {
                wasShown = true;
            };
            control.VisibleChanged += delegate
            {
                wasShown = true;
            };

            control.Show();

            WaitOrExit(control, modal);
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

                if (formShown != null && wasShown)
                {
                    var form = control as Form;
                    formShown(form ?? this);
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

            if (GuiTestHelper.Exception != null)
            {
                GuiTestHelper.RethrowUnhandledException();
            }
        }

        private void InitializeTree(Control control)
        {
            var itemsToShow = new List<object>
            {
                control
            };
            itemsToShow.AddRange(PropertyObjects);

            treeView1.ImageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };
            treeView1.ImageList.Images.Add("Control", Resources.Control);
            treeView1.ImageList.Images.Add("Data", Resources.Data);

            AddAllNodes(treeView1.Nodes, itemsToShow);

            treeView1.NodeMouseClick += delegate
            {
                PropertyGrid.SelectedObject = treeView1.SelectedNode.Tag;
            };
        }

        private static void AddAllNodes(TreeNodeCollection nodes, IEnumerable itemsToShow)
        {
            foreach (object item in itemsToShow.Cast<object>().Where(i => i != null))
            {
                int imageIndex = item is Control ? 0 : 1;
                var node = new TreeNode(item.ToString(), imageIndex, imageIndex)
                {
                    Tag = item
                };
                nodes.Add(node);

                var control = item as Control;
                if (control != null)
                {
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

                var iterator = item as IEnumerable;
                if (iterator != null)
                {
                    AddAllNodes(node.Nodes, iterator);
                }
            }
        }
    }
}
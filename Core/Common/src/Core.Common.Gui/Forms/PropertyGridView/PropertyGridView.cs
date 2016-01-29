// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms.PropertyGridView
{
    public class PropertyGridView : PropertyGrid, IPropertyGrid, IObserver
    {
        /// <summary>
        /// This delegate enabled asynchronous calls to methods without arguments.
        /// </summary>
        private delegate void ArgumentlessDelegate();

        private readonly IApplicationSelection applicationSelection;
        private readonly IPropertyResolver propertyResolver;

        private IObservable observable;

        public PropertyGridView(IApplicationSelection applicationSelection, IPropertyResolver propertyResolver)
        {
            HideTabsButton();
            FixDescriptionArea();
            TranslateToolTips();

            PropertySort = PropertySort.Categorized;

            this.propertyResolver = propertyResolver;

            this.applicationSelection = applicationSelection;
            this.applicationSelection.SelectionChanged += GuiSelectionChanged;
        }

        /// <summary>
        /// Comes from IObserver. Reaction to the change in observable.
        /// </summary>
        public void UpdateObserver()
        {
            if (InvokeRequired)
            {
                ArgumentlessDelegate d = UpdateObserver;
                Invoke(d, new object[0]);
            }
            else
            {
                Refresh();
            }
        }

        /// <summary>
        /// Handles properties sorting change.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertySortChanged(EventArgs e)
        {
            // Needed for maintaining property order (no support for both categorized and alphabetical sorting)
            if (PropertySort == PropertySort.CategorizedAlphabetical)
            {
                PropertySort = PropertySort.Categorized;
            }

            base.OnPropertySortChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (applicationSelection != null)
            {
                applicationSelection.SelectionChanged -= GuiSelectionChanged;
            }

            if (observable != null)
            {
                observable.Detach(this);
            }

            base.Dispose(disposing);
        }

        private void GuiSelectionChanged(object sender, EventArgs e)
        {
            if (observable != null)
            {
                observable.Detach(this);
            }

            var selection = applicationSelection.Selection;
            if (selection == null)
            {
                SelectedObject = null;
                return;
            }

            observable = selection as IObservable;
            if (observable != null)
            {
                observable.Attach(this);
            }

            SelectedObject = GetObjectProperties(selection);
        }

        #region IPropertyGrid Members

        public object Data
        {
            get
            {
                return SelectedObject;
            }
            set
            {
                if (!IsDisposed)
                {
                    SelectedObject = value;
                }
            }
        }

        /// <summary>
        /// Retrieves adapter for the sourceData to be shown as the source object in the grid.
        /// </summary>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        public object GetObjectProperties(object sourceData)
        {
            return propertyResolver.GetObjectProperties(sourceData);
        }

        #endregion

        #region Tab key navigation

        /// <summary>
        /// Do special processing for Tab key. 
        /// http://www.codeproject.com/csharp/wdzPropertyGridUtils.asp
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Tab) || (keyData == (Keys.Tab | Keys.Shift)))
            {
                var selectedItem = SelectedGridItem;
                var root = selectedItem;
                if (selectedItem == null)
                {
                    return false;
                }
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
                // Find all expanded items and put them in a list.
                var items = new ArrayList();
                AddExpandedItems(root, items);

                // Find selectedItem.
                int foundIndex = items.IndexOf(selectedItem);
                if ((keyData & Keys.Shift) == Keys.Shift)
                {
                    foundIndex--;
                    if (foundIndex < 0)
                    {
                        foundIndex = items.Count - 1;
                    }
                    SelectedGridItem = (GridItem) items[foundIndex];
                    if (SelectedGridItem.GridItems.Count > 0)
                    {
                        SelectedGridItem.Expanded = false;
                    }
                }
                else
                {
                    foundIndex++;
                    if (items.Count > 0)
                    {
                        if (foundIndex >= items.Count)
                        {
                            foundIndex = 0;
                        }
                        SelectedGridItem = (GridItem) items[foundIndex];
                    }
                    if (SelectedGridItem.GridItems.Count > 0)
                    {
                        SelectedGridItem.Expanded = true;
                    }
                }

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static void AddExpandedItems(GridItem parent, IList items)
        {
            if (parent.PropertyDescriptor != null)
            {
                items.Add(parent);
            }

            if (parent.Expanded)
            {
                foreach (GridItem child in parent.GridItems)
                {
                    AddExpandedItems(child, items);
                }
            }
        }

        #endregion

        #region Visualization tweaks

        /// <summary>
        /// Removes the redundant "tabs" toolstrip button and its corresponding separator.
        /// </summary>
        private void HideTabsButton()
        {
            var strip = Controls.OfType<ToolStrip>().ToList()[0];

            strip.Items[3].Visible = false;
            strip.Items[4].Visible = false;
        }

        private void TranslateToolTips()
        {
            var strip = Controls.OfType<ToolStrip>().ToList()[0];
            strip.Items[0].ToolTipText = Resources.PropertyGridView_Order_Categorized;
            strip.Items[1].ToolTipText = Resources.PropertyGridView_Order_Alphabetically;
        }

        /// <summary>
        /// Ensures the description area is no longer auto-resizing.
        /// </summary>
        private void FixDescriptionArea()
        {
            foreach (var control in Controls)
            {
                var type = control.GetType();

                if (type.Name == "DocComment")
                {
                    var baseType = type.BaseType;
                    if (baseType != null)
                    {
                        var field = baseType.GetField("userSized", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (field != null)
                        {
                            field.SetValue(control, true);
                        }
                    }

                    return;
                }
            }
        }

        #endregion
    }
}
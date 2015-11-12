using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms.PropertyGridView
{
    public class PropertyGridView : PropertyGrid, IPropertyGrid, IObserver
    {
        /// <summary>
        /// This delegate enabled asynchronous calls to methods without arguments.
        /// </summary>
        private delegate void ArgumentlessDelegate();

        private readonly IGui gui;
        private IObservable observable;

        public PropertyGridView(IGui gui)
        {
            HideTabsButton();
            FixDescriptionArea();

            PropertySort = PropertySort.Categorized;

            this.gui = gui;

            gui.SelectionChanged += GuiSelectionChanged;
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
            // Needed for maintaining property order (no support for both categorized and alphabethical sorting)
            if (PropertySort == PropertySort.CategorizedAlphabetical)
            {
                PropertySort = PropertySort.Categorized;
            }

            base.OnPropertySortChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (gui != null)
            {
                gui.SelectionChanged -= GuiSelectionChanged;
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

            var selection = gui.Selection;
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

        public Image Image
        {
            get
            {
                return Resources.PropertiesHS;
            }
            set {}
        }

        public void EnsureVisible(object item) {}

        /// <summary>
        /// Retrieves adapter for the sourceData to be shown as the source object in the grid.
        /// </summary>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        public object GetObjectProperties(object sourceData)
        {
            return gui != null ? PropertyResolver.GetObjectProperties(gui.Plugins.SelectMany(p => p.GetPropertyInfos()).ToList(), sourceData) : null;
        }

        public ViewInfo ViewInfo { get; set; }

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
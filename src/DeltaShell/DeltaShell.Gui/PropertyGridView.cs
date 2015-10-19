using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DelftTools.Utils.PropertyBag.Dynamic;
using DeltaShell.Gui.Properties;
using log4net;

namespace DeltaShell.Gui
{
    public class PropertyGridView : PropertyGrid, IPropertyGrid, IObserver
    {
        /// <summary>
        /// This delegate enabled asynchronous calls to methods without arguments.
        /// </summary>
        private delegate void ArgumentlessDelegate();

        private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyGridView));

        /// <summary>
        /// todo: This is still an unwanted dependency. PropertyGrid uses gui to subscribe to the SelectionChanged
        /// delegate and in responce queries the gui.Selection
        /// nicer? : custom public delegate in IPropertyGrid with selection as parameter
        /// </summary>
        private readonly IGui gui;

        private object selectedObject;
        private IObservable observableProperty;

        public PropertyGridView(IGui gui)
        {
            HideTabsButton();

            this.gui = gui;
            PropertySort = PropertySort.Categorized;

            gui.SelectionChanged += GuiSelectionChanged;
        }

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

        public object GetObjectProperties(object sourceData)
        {
            if (sourceData == null)
            {
                return null;
            }

            // Obtain all property information
            var propertyInfos = gui.Plugins.SelectMany(p => p.GetPropertyInfos()).ToList();

            // 1. Match property information based on ObjectType and on AdditionalDataCheck
            propertyInfos = propertyInfos.Where(pi => pi.ObjectType.IsInstanceOfType(sourceData) && (pi.AdditionalDataCheck == null || pi.AdditionalDataCheck(sourceData))).ToList();

            // 2. Match property information based on object type inheritance
            propertyInfos = FilterPropertyInfoByTypeInheritance(propertyInfos, pi => pi.ObjectType);

            // 3. Match property information based on property type inheritance
            propertyInfos = FilterPropertyInfoByTypeInheritance(propertyInfos, pi => pi.PropertyType);

            if (propertyInfos.Count == 0)
            {
                // No (or multiple) object properties found: return 'null' so that no object properties are shown in the property grid
                return null;
            }

            if (propertyInfos.Count > 1)
            {
                // 4. We assume that the propertyInfos with AdditionalDataCheck are the most specific
                propertyInfos = propertyInfos.Where(pi => pi.AdditionalDataCheck != null).ToList();
            }

            if (propertyInfos.Count == 1)
            {
                return CreateObjectProperties(propertyInfos.ElementAt(0), sourceData);
            }

            Log.Debug(Resources.PropertyGrid_GetObjectProperties_Multiple_object_property_instances_found_for_the_same_data_object__no_object_properties_are_displayed_in_the_property_grid);
            return null;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.PropertyGrid.PropertySortChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnPropertySortChanged(EventArgs e)
        {
            // Needed for maintaining property order
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

            if (observableProperty != null)
            {
                observableProperty.Detach(this);
            }

            base.Dispose(disposing);
        }

        private new object SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                // Performance optimization
                if (selectedObject == value)
                {
                    return;
                }

                selectedObject = value;

                if (selectedObject == null)
                {
                    base.SelectedObject = null;
                    return;
                }

                var selectedType = GetRelevantType(selectedObject);

                Log.DebugFormat(Resources.PropertyGrid_OnSelectedObjectsChanged_Selected_object_of_type___0_, selectedType.Name);

                base.SelectedObject = selectedObject;
            }
        }

        private void HideTabsButton()
        {
            // removing "property tabs" button and separator before it
            // TODO: case we used derived functionality for this?
            var strip = Controls.OfType<ToolStrip>().ToList()[0];
            strip.Items[3].Visible = false;
            strip.Items[4].Visible = false;
        }

        private void GuiSelectionChanged(object sender, EventArgs e)
        {
            if (observableProperty != null)
            {
                observableProperty.Detach(this);
            }

            var selection = gui.Selection;
            if (selection == null)
            {
                SelectedObject = null;
                return;
            }

            observableProperty = selection as IObservable;
            if (observableProperty != null)
            {
                observableProperty.Attach(this);
            }

            SelectedObject = GetObjectProperties(selection);
        }

        private List<PropertyInfo> FilterPropertyInfoByTypeInheritance(List<PropertyInfo> propertyInfo, Func<PropertyInfo, Type> getTypeAction)
        {
            var propertyInfoCount = propertyInfo.Count;
            var propertyInfoWithUnInheritedType = propertyInfo.ToList();

            for (var i = 0; i < propertyInfoCount; i++)
            {
                var firstType = getTypeAction(propertyInfo.ElementAt(i));

                for (var j = 0; j < propertyInfoCount; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var secondType = getTypeAction(propertyInfo.ElementAt(j));

                    if (firstType != secondType && firstType.IsAssignableFrom(secondType))
                    {
                        propertyInfoWithUnInheritedType.Remove(propertyInfo.ElementAt(i));

                        break;
                    }
                }
            }

            return propertyInfoWithUnInheritedType.Any()
                       ? propertyInfoWithUnInheritedType.ToList() // One or more specific property information objects found: return the filtered list
                       : propertyInfo; // No specific property information found: return the original list
        }

        private object CreateObjectProperties(PropertyInfo propertyInfo, object sourceData)
        {
            try
            {
                // Try to create object properties for the source data
                var objectProperties = propertyInfo.CreateObjectProperties(sourceData);

                // Return a dynamic property bag containing the created object properties
                return objectProperties is DynamicPropertyBag
                           ? (object) objectProperties
                           : new DynamicPropertyBag(objectProperties);
            }
            catch (Exception)
            {
                Log.Debug(Resources.PropertyGrid_CreateObjectProperties_Could_not_create_object_properties_for_the_data);

                // Directly return the source data (TODO: Shouldn't we return "null" instead?)
                return sourceData;
            }
        }

        private static Type GetRelevantType(object obj)
        {
            var propertyBag = obj as DynamicPropertyBag;

            return propertyBag != null ? propertyBag.GetContentType() : obj.GetType();
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
                SelectedObject = value;
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

        public ViewInfo ViewInfo { get; set; }

        #endregion

        #region Enable tab key navigation on property grid

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
    }
}
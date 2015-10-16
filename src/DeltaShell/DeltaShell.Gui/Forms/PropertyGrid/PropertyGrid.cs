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

namespace DeltaShell.Gui.Forms.PropertyGrid
{
    /// TODO: get rid of the properties class or create them runtime like this:
    /// http://stackoverflow.com/questions/313822/how-to-modify-propertygrid-at-runtime-add-remove-property-and-dynamic-types-enum
    /// Having to create property classes for everty state of the object is too much code 
    /// <summary>
    /// PropertyGrid extends the functionality of the PropertyGrid by extra support
    /// for objects of different types. The default behaviour of the PropertyGrid is to show
    /// only the common properties of the objects in the selectedObjects array. In some cases
    /// it is desirable to add an extra filter for objects types.
    /// An example were this behaviour is requested is the 1d schematisation editor. The user 
    /// selects the objects in the GIS oriented view by tracking a rectangle. The objects 
    /// inside the rectangle are set as selectedObjects in a propertygrid. In many cases the 
    /// objects in the selection rectangle will be of different types; this results in a very 
    /// limited subset of shared properties.
    /// Typically a user is only interested in river profiles or culverts. This 
    /// PropertyGrid user control automatically makes a subdivision of the different
    /// types in the selectedObjects array and offers the user the chance to make a selection
    /// via a combobox.
    /// 
    /// Note the combobox at the Top of the propertygrid tries to mimic the behaviour of the 
    /// combobox in Visual Studio. If 1 object is selected the Id is shown bold followed by 
    /// the type description.
    ///
    /// </summary>
    public sealed partial class PropertyGrid : UserControl, IPropertyGrid, IObserver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyGrid));

        /// <summary>
        /// todo: This is still an unwanted dependency. PropertyGrid uses gui to subscribe to the SelectionChanged
        /// delegate and in responce queries the gui.Selection
        /// nicer? : custom public delegate in IPropertyGrid with selection as parameter
        /// </summary>
        private readonly IGui gui;

        private object selectedObject;

        public PropertyGrid(IGui gui)
        {
            InitializeComponent();
            MinimumSize = new Size(200, 200);
            this.gui = gui;

            gui.SelectionChanged += GuiSelectionChanged;

            HideTabsButton();
        }

        private void HideTabsButton()
        {
            // removing "property tabs" button and separator before it
            var strip = propertyGrid1.Controls.OfType<ToolStrip>().ToList()[0] as ToolStrip;
            strip.Items[3].Visible = false;
            strip.Items[4].Visible = false;
        }

        public void UpdateObserver()
        {
            propertyGrid1.Refresh();
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
            propertyInfos = propertyInfos.Where(pi => pi.ObjectType.IsAssignableFrom(sourceData.GetType()) && (pi.AdditionalDataCheck == null || pi.AdditionalDataCheck(sourceData))).ToList();

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

        public override void Refresh()
        {
            propertyGrid1.Refresh();
        }

        private object SelectedObject
        {
            get
            {
                return selectedObject;
            }

            set
            {
                if (selectedObject == value)
                    return;

                selectedObject = value;
                OnSelectedObjectsChanged();                
            }
        }

        private void GuiSelectionChanged(object sender, EventArgs e)
        {
            if (IsDisposed)
            {
                return; //event may fire when propertygrid is already disposed.
            }

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
            var propertyInfoCount = propertyInfo.Count();
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

        /// <summary>
        /// OnSelectedObjectsChanged
        /// If the selected objects changed rebuild the internal dictionary that counts the number of
        /// objects of each type.
        /// </summary>
        private void OnSelectedObjectsChanged()
        {
            if (SelectedObject == null)
            {
                propertyGrid1.SelectedObject = null;
                return;
            }
            
            var selectedType = GetRelevantType(SelectedObject);

            Log.DebugFormat(Resources.PropertyGrid_OnSelectedObjectsChanged_Selected_object_of_type___0_, selectedType.Name);

            propertyGrid1.SelectedObject = SelectedObject;
        }

        private static Type GetRelevantType(object obj)
        {
            if (obj is DynamicPropertyBag)
            {
                var bag = obj as DynamicPropertyBag;
                return bag.GetContentType();
            }
            return obj.GetType();
        }

        private void PropertyGrid1PropertySortChanged(object sender, EventArgs e)
        {
            // Needed for maintaining property order
            if (propertyGrid1.PropertySort == PropertySort.CategorizedAlphabetical)
            {
                propertyGrid1.PropertySort = PropertySort.Categorized;
            }
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

        #region enable tab key navigation on propertygrid

        private IObservable observableProperty;

        /// <summary>
        /// Gets or sets whether to expand an item when pressing tab.
        /// </summary>
        /// <remarks>
        /// When <c>true</c> items are also unexpanded when pressing shift-tab.
        /// Note that the enter key will always work to expand.
        /// </remarks>
        public bool ExpandOnTab { get; set; }

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
                GridItem selectedItem = propertyGrid1.SelectedGridItem;
                GridItem root = selectedItem;
                if (selectedItem == null)
                {
                    return false;
                }
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
                // Find all expanded items and put them in a list.
                ArrayList items = new ArrayList();
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
                    propertyGrid1.SelectedGridItem = (GridItem) items[foundIndex];
                    if (ExpandOnTab && (propertyGrid1.SelectedGridItem.GridItems.Count > 0))
                    {
                        propertyGrid1.SelectedGridItem.Expanded = false;
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
                        propertyGrid1.SelectedGridItem = (GridItem) items[foundIndex];
                    }
                    if (ExpandOnTab && (propertyGrid1.SelectedGridItem.GridItems.Count > 0))
                    {
                        propertyGrid1.SelectedGridItem.Expanded = true;
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
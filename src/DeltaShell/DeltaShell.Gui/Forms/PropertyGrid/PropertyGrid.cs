using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Data;
using DelftTools.Utils.PropertyBag.Dynamic;
using DelftTools.Utils.Threading;
using DeltaShell.Gui.Properties;
using log4net;
using PropertyInfo = System.Reflection.PropertyInfo;

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
        private static readonly ILog Log = LogManager.GetLogger(typeof (PropertyGrid));

        private DelayedEventHandler<SelectedItemChangedEventArgs> guiSelectionChangedDelayedEventHandler;

        /// <summary>
        /// todo: This is still an unwanted dependency. PropertyGrid uses gui to subscribe to the SelectionChanged
        /// delegate and in responce queries the gui.Selection
        /// nicer? : custom public delegate in IPropertyGrid with selection as parameter
        /// </summary>
        private readonly IGui gui;

        /// <summary>
        /// objectTypes is a collection of used types. The type of the object is used as key,
        /// the value is the number of occurences in selectedObjects
        /// </summary>
        private readonly Dictionary<Type, int> objectTypes = new Dictionary<Type, int>();

        // The selected object as they are available to the outside world
        private object[] selectedObjects;

        private INotifyPropertyChanged notifiableProperty;

        public PropertyGrid(IGui gui)
        {
            InitializeComponent();
            MinimumSize = new Size(200, 200);
            this.gui = gui;

            guiSelectionChangedDelayedEventHandler = new DelayedEventHandler<SelectedItemChangedEventArgs>(GuiSelectionChanged)
            {
                FullRefreshEventHandler = GuiSelectionChanged,
                FullRefreshEventsCount = 10,
                Delay = 100,
            };
            gui.SelectionChanged += guiSelectionChangedDelayedEventHandler;

            // TODO: make timer start only when property was changed and then stop
            refreshTimer = new Timer();
            refreshTimer.Tick += delegate
                                     {
                                         if (refreshRequired)
                                         {
                                             if (IsInEditMode(propertyGrid1))
                                                 return;
                                             propertyGrid1.Refresh();
                                             comboBox1.Refresh();
                                             refreshRequired = false;
                                         }
                                     };
            refreshTimer.Interval = 300;
            refreshTimer.Enabled = true;
            refreshTimer.Start();
        }

        //from: http://stackoverflow.com/questions/7553423/c-sharp-propertygrid-check-if-a-value-is-currently-beeing-edited
        public static bool IsInEditMode(System.Windows.Forms.PropertyGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            Control gridView = (Control)grid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);
            Control edit = (Control)gridView.GetType().GetField("edit", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gridView);
            Control dropDownHolder = (Control)gridView.GetType().GetField("dropDownHolder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gridView);

            return ((edit != null) && (edit.Visible & edit.Focused)) || ((dropDownHolder != null) && (dropDownHolder.Visible));
        }

        private object[] SelectedObjects
        {
            get { return selectedObjects; }
            [InvokeRequired]
            set
            {
                selectedObjects = value;
                OnSelectedObjectsChanged();
            }
        }

        private object SelectedObject
        {
            get
            {
                if (selectedObjects == null)
                {
                    return null;
                }

                return selectedObjects[0];
            }

            set
            {
                // If the object is only one list, show all the contained items
                IEnumerable ienum = value as IEnumerable;
                if (ienum != null)
                {
                    ArrayList objects = new ArrayList();
                    IEnumerator obj = ienum.GetEnumerator();
                    // Create array of the items in the list
                    while (obj.MoveNext())
                    {
                        objects.Add(obj.Current);
                    }
                    if (objects.Count > 0)
                    {
                        SelectedObjects = objects.ToArray();
                    }
                    else
                    {
                        // If this was an empty list, set the property grid to null
                        SelectedObjects = null;
                    }
                }
                else
                {
                    // Show a single object
                    object[] objects = new object[1];
                    objects[0] = value;
                    SelectedObjects = objects;
                }
            }
        }

        #region IPropertyGrid Members

        public object Data
        {
            get { return SelectedObject; }
            set { SelectedObject = value; }
        }

        /// <summary>
        /// Refreshes propertygrid when changes in the data occur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectedObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            refreshRequired = true;
        }

        public Image Image
        {
            get { return Properties.Resources.PropertiesHS; }
            set { }
        }

        public void EnsureVisible(object item) { }
        public ViewInfo ViewInfo { get; set; }

        #endregion

        private void GuiSelectionChanged(object sender, EventArgs e)
        {
            if (IsDisposed) return; //event may fire when propertygrid is already disposed.

            if (notifiableProperty != null)
            {
                notifiableProperty.PropertyChanged -= selectedObject_PropertyChanged;
                notifiableProperty = null;
            }

            if (notifiableCollection != null)
            {
                notifiableCollection.CollectionChanged -= selectedObject_CollectionChanged;
                notifiableCollection = null;
            }
            if (observableProperty != null)
            {
                observableProperty.Detach(this);
                observableProperty = null;
            }

            var selection = gui.Selection;

            if (selection == null)
            {
                SelectedObject = null;
                return;
            }

            //            if (selection is IEnumerable<object>)
            //            {
            //                SelectedObjects = selection as object[]; // HACK: this is buggy!
            //            }
            //            else

            notifiableProperty = selection as INotifyPropertyChanged;
            if (notifiableProperty != null)
            {
                notifiableProperty.PropertyChanged += selectedObject_PropertyChanged;
            }
            observableProperty = selection as IObservable;
            if (observableProperty != null)
            {
                observableProperty.Attach(this);
            }

            notifiableCollection = selection as INotifyCollectionChanged;
            if (notifiableCollection != null)
            {
                notifiableCollection.CollectionChanged += selectedObject_CollectionChanged;
            }

            object propertyObject;

            if (selection is IEnumerable)
            {
                // Try to get a object properties for the enumerable directly
                propertyObject = GetObjectProperties(selection);

                if (propertyObject == null || propertyObject == selection)
                {
                    // Otherwise, create an array list with object properties for the individual elements of the enumerable
                    var enumerable = (IEnumerable) selection;
                    var providers = new ArrayList();

                    foreach (var element in enumerable)
                    {
                        var o = GetObjectProperties(element);

                        if (o != null)
                        {
                            providers.Add(o);
                        }
                    }

                    propertyObject = providers;
                }
            }
            else
            {
                propertyObject = GetObjectProperties(selection);
            }

            if (selectedObjects != propertyObject)
            {
                SelectedObject = propertyObject;
            }

        }

        private void selectedObject_CollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            refreshRequired = true;
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

        private List<DelftTools.Shell.Gui.PropertyInfo> FilterPropertyInfoByTypeInheritance(List<DelftTools.Shell.Gui.PropertyInfo> propertyInfo, Func<DelftTools.Shell.Gui.PropertyInfo, Type> getTypeAction)
        {
            var propertyInfoCount = propertyInfo.Count();
            var propertyInfoWithUnInheritedType = propertyInfo.ToList();

            for (var i = 0; i < propertyInfoCount; i++)
            {
                var firstType = getTypeAction(propertyInfo.ElementAt(i));

                for (var j = 0; j < propertyInfoCount; j++)
                {
                    if (i == j) continue;

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

        private object CreateObjectProperties(DelftTools.Shell.Gui.PropertyInfo propertyInfo, object sourceData)
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
            objectTypes.Clear();
            comboBox1.Items.Clear();

            if (SelectedObjects == null)
            {
                propertyGrid1.SelectedObject = null;

                return;
            }

            if (SelectedObjects[0] == null)
            {
                if (propertyGrid1.SelectedObjects.Length > 0)
                {
                    propertyGrid1.SelectedObjects = new object[0];
                }

                return;
            }

            if (SelectedObjects.Length == 1)
            {
                var selectedType = GetRelevantType(SelectedObjects[0]);

                Log.DebugFormat(Resources.PropertyGrid_OnSelectedObjectsChanged_Selected_object_of_type___0_, selectedType.Name);

                objectTypes.Add(selectedType, 1);
                comboBox1.Items.Add(selectedType);
            }
            else
            {
                Log.DebugFormat(Resources.PropertyGrid_OnSelectedObjectsChanged_Selected_multiple_objects_);

                int count = 0;

                comboBox1.Items.Add(typeof (Object));
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var selectedType = GetRelevantType(SelectedObjects[i]);

                    if (!objectTypes.ContainsKey(selectedType))
                    {
                        objectTypes.Add(selectedType, 1);
                        comboBox1.Items.Add(selectedType);
                    }
                    else
                    {
                        objectTypes[selectedType] = objectTypes[selectedType] + 1;
                    }

                    //Aggregate logging results for speed reasons:
                    string currOne = selectedType.Name;
                    int hash = currOne.GetHashCode();
                    bool printAggregate = false;
                    count++;
                    if (i < (selectedObjects.Length - 1)) //not at the end yet, look at next one
                    {
                        string nextOne = GetRelevantType(selectedObjects[i + 1]).Name;
                        int nextHash = nextOne.GetHashCode();
                        printAggregate = hash != nextHash;
                    }
                    else //last one, print ourselves, plus previous if same
                    {
                        //(we know there are at least two objects in the list)
                        string prevOne = GetRelevantType(selectedObjects[i - 1]).Name;
                        int prevHash = prevOne.GetHashCode();

                        if (hash != prevHash)   
                            Log.DebugFormat("    1x: {0}", currOne);
                        else 
                            printAggregate = true;
                    }
                    if (printAggregate)
                    {
                        Log.DebugFormat("    {0}x: {1}", count, currOne);
                        count = 0;
                    }
                }
            }

            comboBox1.SelectedIndex = comboBox1.Items.Count > 1 ? 1 : 0;
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

        /// <summary>
        /// comboBox1_SelectedIndexChanged
        /// If the the selection of the embedded combobox changes feed the embedded propertygrid
        /// with an appropriate collection of objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type t = (Type) comboBox1.Items[comboBox1.SelectedIndex];
            if (typeof (Object) == t)
            {
                propertyGrid1.SelectedObjects = selectedObjects;
            }
            else
            {
                var objects = new object[objectTypes[t]];
                var objectTypeCount = 0;
                foreach (var t1 in selectedObjects)
                {
                    if (t1 != null && t == GetRelevantType(t1))
                    {
                        objects[objectTypeCount] = t1;
                        objectTypeCount++;
                    }
                }
                propertyGrid1.SelectedObjects = objects;
            }
        }

        /// <summary>
        /// objectId
        /// Checks if object o has a property named Id and return the value.
        /// If "Id" is not supported an empty string is returned.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static string GetObjectId(object o)
        {
            if (o == null)
                return "(null)";

            PropertyInfo[] propertyCollection = GetRelevantType(o).GetProperties();

            for (int i = 0; i < propertyCollection.Length; i++)
            {
                PropertyInfo propertyInfo = propertyCollection[i];
                if ("Id" == propertyInfo.Name)
                {
                    object value = null;
                    if (o is DynamicPropertyBag)
                    {
                        var bag = o as DynamicPropertyBag;
                        var descr = ((ICustomTypeDescriptor) bag).GetProperties().Find(propertyInfo.Name, false);
                        if (descr != null)
                            value = descr.GetValue(o);
                    }
                    else
                    {
                        value = propertyInfo.GetValue(o, null);
                    }

                    return (value != null) ? value.ToString() : null;
                    //don't just do toString()..the ID might be null
                    //return propertyInfo.GetValue(o, null).ToString();
                }
            }
            return "";
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (-1 == e.Index)
                return;
            Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                              ? SystemBrushes.HighlightText
                              : SystemBrushes.WindowText;
            Rectangle bounds = e.Bounds;

            Type t = (Type) comboBox1.Items[e.Index];

            var displayNameAttribute = t.GetCustomAttributes(true).OfType<DisplayNameAttribute>().FirstOrDefault();
            var name = t.Name;
            if (displayNameAttribute != null)
            {
                name = displayNameAttribute.DisplayName;
            }

            e.DrawBackground();
            if (SelectedObjects.Length > 1)
            {
                string text;
                if (typeof (Object) == t)
                {
                    text = string.Format(Resources.PropertyGrid_comboBox1_DrawItem_All_selected_Objects___0__,SelectedObjects.Length);
                }
                else
                {
                    text = string.Format(Resources.PropertyGrid_comboBox1_DrawItem_selected__0____1__,name, objectTypes[t]);
                }
                e.Graphics.DrawString(text, Font, brush, e.Bounds);
            }
            else if (SelectedObjects.Length == 1)
            {
                // only 1 object; show id in boldfont followed by the type description.

                Font boldFont = new Font(Font, FontStyle.Bold);

                string objectDescription = GetObjectId(SelectedObjects[0]);
                e.Graphics.DrawString(objectDescription, boldFont, brush, bounds);
                // xx = add extra margin
                bounds.X += Convert.ToInt32(e.Graphics.MeasureString(objectDescription + "xx", boldFont).Width);

                e.Graphics.DrawString(name, Font, brush, bounds);
            }
            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                e.DrawFocusRectangle();
        }

        #region enable tab key navigation on propertygrid

        private bool expandOnTab;
        private bool refreshRequired;
        private Timer refreshTimer;
        private INotifyCollectionChanged notifiableCollection;
        private IObservable observableProperty;

        /// <summary>
        /// Gets or sets whether to expand an item when pressing tab.
        /// </summary>
        /// <remarks>
        /// When <c>true</c> items are also unexpanded when pressing shift-tab.
        /// Note that the enter key will always work to expand.
        /// </remarks>
        public bool ExpandOnTab
        {
            get { return expandOnTab; }
            set { expandOnTab = value; }
        }

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
                    if (expandOnTab && (propertyGrid1.SelectedGridItem.GridItems.Count > 0))
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
                        propertyGrid1.SelectedGridItem = (GridItem)items[foundIndex];
                    }
                    if (expandOnTab && (propertyGrid1.SelectedGridItem.GridItems.Count > 0))
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

        public void UpdateObserver()
        {
            refreshRequired = true;
        }

        public override void Refresh()
        {
            refreshRequired = true;
            base.Refresh();
        }

        private void PropertyGrid1PropertySortChanged(object sender, EventArgs e)
        {
            // Needed for maintaining property order
            if (propertyGrid1.PropertySort == PropertySort.CategorizedAlphabetical)
            {
                propertyGrid1.PropertySort = PropertySort.Categorized;
            }
        }
    }
}
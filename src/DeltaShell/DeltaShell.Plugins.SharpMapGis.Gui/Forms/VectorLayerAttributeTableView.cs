using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using DelftTools.Controls.Swf.Table;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.ComponentModel;
using DelftTools.Utils.Reflection;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using GeoAPI.Extensions.Feature;
using NetTopologySuite.Extensions.Features;
using SharpMap.Api.Layers;
using MessageBox = System.Windows.Forms.MessageBox;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms
{
    public partial class VectorLayerAttributeTableView : UserControl, ILayerEditorView
    {
        private TableView tableView;
        private ILayer layer;
        private IList featureRowList;
        private bool removingFeatures;
        private Func<IFeature, IFeatureRowObject> createFeatureRowObject;
        private Type featureRowType;
        private Func<string, bool> dynamicAttributeVisible;
        private bool canAddDeleteAttributes;

        public VectorLayerAttributeTableView()
        {
            InitializeComponent();

            canAddDeleteAttributes = true;

            InitializeDynamicAttributeContextMenu();

            // TODO: extend UpdateTableDataSource() to support add/delete new rows, via layer.FeatureEditor?
            tableView.AllowAddNewRow = false;
            
            tableView.ReadOnlyCellBackColor = SystemColors.ControlLight;
            tableView.ReadOnlyCellFilter = OnReadOnlyCellFilter;
            tableView.RowDeleteHandler = OnDeleteRows;
        }

        private bool OnDeleteRows()
        {
            if (DeleteSelectedFeatures == null) return false;

            DeleteSelectedFeatures();
            SelectedFeatures = Enumerable.Empty<IFeature>();
            return true;
        }

        private bool OnReadOnlyCellFilter(TableViewCell c)
        {
            if (c.Column.IsUnbound)
                return false;

            if (tableView.Data == null) return true;

            var rowObjects = (IList) tableView.Data;
            if (rowObjects.Count <= 0)
                return true;

            var dataSourceIndexByRowIndex = tableView.GetDataSourceIndexByRowIndex(c.RowIndex);
            if (dataSourceIndexByRowIndex < 0)
                return false;

            var rowObject = rowObjects[dataSourceIndexByRowIndex];
            
            return DynamicReadOnlyAttribute.IsDynamicReadOnly(rowObject, c.Column.Name);
        }

        /// <summary>
        /// Optional factory method to create an object to be bound to the table view for a given feature.
        /// </summary>
        public void SetCreateFeatureRowFunction<TFeatureRow>(Func<IFeature, TFeatureRow> createFeatureRowFunction) where TFeatureRow : class, IFeatureRowObject
        {
            createFeatureRowObject = createFeatureRowFunction;
            featureRowType = typeof (TFeatureRow);
            
            if (layer != null)
            {
                UpdateTableDataSource();
            }
        }

        public Action<IFeature> ZoomToFeature { get; set; }

        public Action<object> OpenViewMethod { get; set; }

        public Action DeleteSelectedFeatures { get; set; }

        public object Data
        {
            get { return layer; }
            set
            {
                if (layer != null && layer.DataSource != null)
                {
                    layer.DataSource.FeaturesChanged -= DataSourceFeaturesChanged;
                }

                layer = value as ILayer;

                if (layer != null && layer.DataSource != null)
                {
                    layer.DataSource.FeaturesChanged += DataSourceFeaturesChanged;

                    UpdateTableDataSource();
                }
            }
        }

        public TableView TableView
        {
            get { return tableView; }
        }

        public IEnumerable<IFeature> SelectedFeatures
        {
            get
            {
                if (layer.DataSource != null)
                {
                    var rowIndices = tableView.SelectedCells.Select(c => c.RowIndex).Distinct();

                    return rowIndices.Where(i => i >= 0)
                                     .Select(i => layer.DataSource.Features[tableView.GetDataSourceIndexByRowIndex(i)])
                                     .OfType<IFeature>()
                                     .Where(f => f != null);
                }

                return Enumerable.Empty<IFeature>();
            }
            set
            {
                if (value == null || layer.DataSource == null || layer.DataSource.Features == null)
                {
                    return;
                }

                tableView.ClearSelection();

                var features = value.Where(f => f.GetType().Implements(layer.DataSource.FeatureType)).ToList();

                if (features.Count == 0)
                {
                    return;
                }
                
                tableView.SelectRows(features.Select(f => tableView.GetRowIndexByDataSourceIndex(layer.DataSource.Features.IndexOf(f))).ToArray());

                tableView.FocusedRowIndex = tableView.SelectedRowsIndices.FirstOrDefault();
            }
        }

        public Image Image { get; set; }

        public void EnsureVisible(object item)
        {
        }

        public ViewInfo ViewInfo { get; set; }

        private void DataSourceFeaturesChanged(object sender, EventArgs e)
        {
            if (createFeatureRowObject == null)
            {
                tableView.ScheduleRefresh();
            }
            else if (!removingFeatures)
            {
                tableView.SuspendDrawing();
                tableView.Data = null; // clear previous subscriptions (property changed)
                tableView.Data = CreateFeatureRowList(layer.DataSource.Features);
                tableView.ResumeDrawing();
            }
        }

        private void UpdateTableDataSource()
        {
            ConfigureStaticAttributeColumns();
            ConfigureDynamicAttributeColumns();

            tableView.Data = createFeatureRowObject == null
                                 ? layer.DataSource.Features
                                 : CreateFeatureRowList(layer.DataSource.Features);

            tableView.AllowDeleteRow = !layer.ReadOnly && layer.FeatureEditor != null;

            tableView.RefreshData();
            tableView.BestFitColumns();
        }

        private IList CreateFeatureRowList(IList features)
        {
            CleanUpFeatureRows();

            // Create and initialize EventedList<TFeatureRowObject>. Type needs to be the specific featureRowObject (binding needs the 
            // type to get all the properties)
            featureRowList = (IList) TypeUtils.CreateGeneric(typeof (EventedList<>), new[] {featureRowType});

            foreach (var featureRowObject in features.OfType<IFeature>().Select(createFeatureRowObject))
            {
                featureRowList.Add(featureRowObject);
            }

            ((INotifyCollectionChange) featureRowList).CollectionChanged += FeatureRowListCollectionChanged;

            return featureRowList;
        }

        private void CleanUpFeatureRows()
        {
            if (featureRowList == null)
            {
                return;
            }

            ((INotifyCollectionChange)featureRowList).CollectionChanged -= FeatureRowListCollectionChanged;

            foreach (var featureRowObject in featureRowList.OfType<IDisposable>())
            {
                featureRowObject.Dispose();
            }

            featureRowList = null;
        }

        private void FeatureRowListCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            // if item is removed using this table
            if (e.Action != NotifyCollectionChangeAction.Remove)
            {
                return;
            }

            removingFeatures = true;
            var feature = ((IFeatureRowObject) e.Item).GetFeature();
            layer.FeatureEditor.CreateInteractor(layer,feature).Delete();
            removingFeatures = false;
        }

        private void ConfigureStaticAttributeColumns()
        {
            tableView.Columns.Clear();
            tableView.AutoGenerateColumns = false;
            
            if (createFeatureRowObject == null)
            {
                var featureType = layer.DataSource.FeatureType;

                // use [FeatureAttribute] to show properties of the class in the attributes table
                featureType.GetProperties(); // refresh property cache (ensure that the properties are always returned in the same order)

                var attributeNames = FeatureAttributeAccessorHelper.GetFeatureAttributeNames(featureType).ToList();
                
                foreach (var name in attributeNames)
                {
                    var displayName = FeatureAttributeAccessorHelper.GetPropertyDisplayName(featureType, name);
                    var displayFormat = FeatureAttributeAccessorHelper.GetFormatString(featureType, name);
                    var isReadOnly = FeatureAttributeAccessorHelper.IsReadOnly(featureType, name);

                    tableView.AddColumn(name, displayName ?? name, isReadOnly, 100, displayFormat: displayFormat);
                }
            }
            else
            {
                foreach (var propertyInfo in featureRowType.GetProperties())
                {
                    var customAttributes = propertyInfo.GetCustomAttributes(true);

                    if (customAttributes.OfType<BrowsableAttribute>().Any(a => !a.Browsable))
                        continue;

                    var name = propertyInfo.Name;
                    
                    var displayName = customAttributes.OfType<DisplayNameAttribute>().Select(a => a.DisplayName).FirstOrDefault();
                    var displayFormat = customAttributes.OfType<DisplayFormatAttribute>().Select(a => a.FormatString).FirstOrDefault();
                    var isReadOnly = FeatureAttributeAccessorHelper.IsReadOnly(featureRowType, name);

                    tableView.AddColumn(name, displayName ?? name, isReadOnly, 100, displayFormat: displayFormat);
                }
            }
        }

        public Func<string, bool> DynamicAttributeVisible
        {
            private get { return dynamicAttributeVisible; }
            set
            {
                dynamicAttributeVisible = value;
                UpdateTableDataSource();
            }
        }

        private bool IsVisible(string attributeKey)
        {
            return DynamicAttributeVisible == null || DynamicAttributeVisible(attributeKey);
        }

        private void ConfigureDynamicAttributeColumns()
        {
            tableView.Columns.RemoveAllWhere(c => c.IsUnbound);

            var attributes = layer.DataSource.Features.Cast<IFeature>()
                .Where(f => f.Attributes != null).SelectMany(feature => feature.Attributes.Keys)
                .Distinct().Where(IsVisible);

            var index = tableView.Columns.Count;

            foreach (var attribute in attributes)
            {
                var columnName = attribute;

                var column = tableView.Columns.FirstOrDefault(c => c.Name == columnName);

                if (column != null)
                {
                    continue;
                }

                tableView.AddUnboundColumn(columnName, typeof(string), index++);
            }
        }

        public bool CanAddDeleteAttributes
        {
            get { return canAddDeleteAttributes; }
            set
            {
                if (canAddDeleteAttributes != value)
                {
                    canAddDeleteAttributes = value;
                    InitializeDynamicAttributeContextMenu();
                }
            }
        }

        private void InitializeDynamicAttributeContextMenu()
        {
            const string addAttributeItemName = "btnAddAttribute";
            const string zoomItemName = "btnzoomToMenuItem";
            const string openViewItemName = "btnOpenViewMenuItem";
            const string addAttributeCaption = "Add Attribute";
            const string deleteAttributeCaption = "Delete Attribute";
            const string zoomToItemCaption = "Zoom to item";
            const string openViewCaption = "Open view...";
            
            if (CanAddDeleteAttributes)
            {
                if (tableView.RowContextMenu.Items.OfType<ToolStripItem>().All(mi => mi.Name != addAttributeItemName))
                {
                    var btnAddAttribute = new ToolStripMenuItem
                    {
                        Name = addAttributeItemName,
                        Text = addAttributeCaption,
                        Image = Resources.table_add
                    };
                    btnAddAttribute.Click += AddAttributeItemClick;
                    btnAddAttribute.Tag = tableView;
                    tableView.RowContextMenu.Items.Add(btnAddAttribute);
                }
            }
            else
            {
                if (tableView.RowContextMenu.Items.OfType<ToolStripItem>()
                    .Any(mi => mi.Name == addAttributeItemName))
                {
                    var menuItem = tableView.RowContextMenu.Items.OfType<ToolStripItem>()
                        .First(mi => mi.Name == addAttributeItemName);
                    tableView.RowContextMenu.Items.Remove(menuItem);
                }
            }
            if (tableView.RowContextMenu.Items.OfType<ToolStripItem>().All(mi => mi.Name != zoomItemName))
            {
                var btnzoomToMenuItem = new ToolStripMenuItem
                {
                    Name = zoomItemName,
                    Text = zoomToItemCaption,
                    Image = Resources.magnifier__arrow
                };
                btnzoomToMenuItem.Click += BtnZoomToClick;
                btnzoomToMenuItem.Tag = tableView;
                tableView.RowContextMenu.Items.Add(btnzoomToMenuItem);
            }
            if (tableView.RowContextMenu.Items.OfType<ToolStripItem>().All(mi => mi.Name != openViewItemName))
            {
                var btnOpenViewMenuItem = new ToolStripMenuItem
                {
                    Name = openViewItemName,
                    Text = openViewCaption,
                    Image = Resources.Properties
                };
                btnOpenViewMenuItem.Click += BtnOpenViewClick;
                btnOpenViewMenuItem.Tag = tableView;
                btnOpenViewMenuItem.Font = new Font(btnOpenViewMenuItem.Font, FontStyle.Bold);
                tableView.RowContextMenu.Items.Add(btnOpenViewMenuItem);
            }
            tableView.UnboundColumnData = TableViewUnboundColumnDataUpdating;

            if (CanAddDeleteAttributes)
            {
                if (tableView.ColumnMenuItems.All(mi => mi.Caption != addAttributeCaption))
                {
                    var addAttributeItem = new TableViewColumnMenuItem(addAttributeCaption)
                    {
                        Image = Resources.table_add
                    };
                    addAttributeItem.Click += AddAttributeItemClick;
                    tableView.ColumnMenuItems.Add(addAttributeItem);
                }
                if (tableView.ColumnMenuItems.All(mi => mi.Caption != deleteAttributeCaption))
                {
                    var deleteAttributeItem = new TableViewColumnMenuItem(deleteAttributeCaption)
                    {
                        Image = Resources.table_delete
                    };
                    deleteAttributeItem.Showing += DeleteAttributeItemShowing;
                    deleteAttributeItem.Click += DeleteAttributeItemClick;
                    tableView.ColumnMenuItems.Add(deleteAttributeItem);
                }
            }
            else
            {
                tableView.ColumnMenuItems.RemoveAllWhere(
                    mi => mi.Caption == addAttributeCaption || mi.Caption == deleteAttributeCaption);
            }

            if (tableView.ColumnMenuItems.All(mi => mi.Caption != zoomToItemCaption))
            {
                var zoomToMenuItem = new TableViewColumnMenuItem(zoomToItemCaption) {Image = Resources.magnifier__arrow};
                zoomToMenuItem.Click += BtnZoomToClick;
                tableView.ColumnMenuItems.Add(zoomToMenuItem);
            }
        }

        private void DeleteAttributeItemShowing(object sender, CancelEventArgs e)
        {
            var attributes = layer.DataSource.Features.Cast<IFeature>().Where(f => f.Attributes != null).SelectMany(f => f.Attributes.Keys);

            var column = sender as ITableViewColumn;

            //only show delete option if its a custom attribute
            if (column != null && attributes.Any(a => a == column.Name)) 
            {
                e.Cancel = false;
                return;
            }
            e.Cancel = true;
        }

        private void DeleteAttributeItemClick(object sender, EventArgs e)
        {
            var column = sender as TableViewColumn;

            if (column == null)
            {
                return;
            }

            column.Visible = false;
            tableView.Columns.Remove(column);

            var attributeName = column.Name;

            foreach (var feature in layer.DataSource.Features.Cast<IFeature>().ToList())
            {
                if (feature.Attributes != null && feature.Attributes.ContainsKey(attributeName))
                {
                    feature.Attributes.Remove(attributeName);
                }
            }

            tableView.RefreshData();
            tableView.ResetBindings();
        }

        private void BtnZoomToClick(object sender, EventArgs e)
        {
            var selectedFeature = SelectedFeatures.FirstOrDefault();
            if (selectedFeature != null && ZoomToFeature != null)
            {
                ZoomToFeature(selectedFeature);
            }
        }

        private void BtnOpenViewClick(object sender, EventArgs e)
        {
            var selectedFeature = SelectedFeatures.FirstOrDefault();
            if (selectedFeature != null && OpenViewMethod != null)
            {
                OpenViewMethod(selectedFeature);
            }
        }

        private void AddAttributeItemClick(object sender, EventArgs e)
        {
            if (tableView.FocusedRowIndex < 0)
            {
                return;
            }

            var dialog = new InputTextDialog { Text = "Please give an attribute name" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var attributeName = dialog.EnteredText;

                if (String.IsNullOrEmpty(attributeName) || layer.DataSource.Features.Cast<IFeature>().ToList().Any(f => f.Attributes != null && f.Attributes.ContainsKey(attributeName)))
                {
                    MessageBox.Show("Invalid attribute name: already exists");
                    return;
                }

                foreach (var feature in layer.DataSource.Features.Cast<IFeature>().ToList())
                {
                    if (feature.Attributes == null)
                    {
                        feature.Attributes = new DictionaryFeatureAttributeCollection();
                    }

                    if (!feature.Attributes.ContainsKey(attributeName))
                    {
                        feature.Attributes.Add(new KeyValuePair<string, object>(dialog.EnteredText, ""));
                    }
                }
                ConfigureDynamicAttributeColumns();
                tableView.RefreshData();
            }
        }

        private object TableViewUnboundColumnDataUpdating(int column, int dataSourceIndex, bool isGetData, bool isSetData, object value)
        {
            var featureIndex = dataSourceIndex;
            var feature = layer.DataSource.GetFeature(featureIndex);
            
            if (tableView.Columns.Count <= column)
            {
                return value;
            }

            var attributeName = tableView.Columns[column].Name;

            if (feature.Attributes == null)
            {
                feature.Attributes = new DictionaryFeatureAttributeCollection();
            }

            if (isGetData)
            {
                if (feature.Attributes.ContainsKey(attributeName))
                {
                    return feature.Attributes[attributeName].ToString();
                }

                return "";
            }

            if (isSetData)
            {
                if (!feature.Attributes.ContainsKey(attributeName))
                {
                    feature.Attributes.Add(attributeName, "");
                }

                // match type
                object currentValue;

                var type = typeof (string);
                if (feature.Attributes.TryGetValue(attributeName, out currentValue) && currentValue != null)
                {
                    type = currentValue.GetType();
                    feature.Attributes[attributeName] = DelftTools.Utils.TypeConverter.ConvertValueToTargetType(type,value);
                }
                else
                {
                    feature.Attributes[attributeName] = value;
                }

                layer.RenderRequired = true; // schedule rendering (some feature types may not support events)
            }

            return null;
        }

        private void TableViewSelectionChanged(object sender, TableSelectionChangedEventArgs e)
        {
            if (SelectedFeaturesChanged != null)
                SelectedFeaturesChanged(this, e);
        }

        public event EventHandler SelectedFeaturesChanged;
        public ILayer Layer { set; get; }
        public void OnActivated()
        {
        }

        public void OnDeactivated()
        {
        }
    }
}

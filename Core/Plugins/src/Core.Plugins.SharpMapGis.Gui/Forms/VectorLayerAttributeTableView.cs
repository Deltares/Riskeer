using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Table;
using Core.Common.Gui;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Events;
using Core.Common.Utils.Reflection;
using Core.GIS.GeoAPI.Attributes;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.SharpMap.Api.Collections;
using Core.GIS.SharpMap.Api.Layers;
using Core.Plugins.SharpMapGis.Gui.Properties;
using TypeConverter = Core.Common.Utils.TypeConverter;

namespace Core.Plugins.SharpMapGis.Gui.Forms
{
    public partial class VectorLayerAttributeTableView : UserControl, ILayerEditorView
    {
        public event EventHandler SelectedFeaturesChanged;
        private ILayer layer;
        private IList featureRowList;
        private bool removingFeatures;
        private Func<IFeature, IFeatureRowObject> createFeatureRowObject;
        private Type featureRowType;
        private Func<string, bool> dynamicAttributeVisible;

        public VectorLayerAttributeTableView()
        {
            InitializeComponent();

            InitializeDynamicAttributeContextMenu();

            // TODO: extend UpdateTableDataSource() to support add/delete new rows, via layer.FeatureEditor?
            TableView.AllowAddNewRow = false;

            TableView.ReadOnlyCellBackColor = SystemColors.ControlLight;
            TableView.ReadOnlyCellFilter = OnReadOnlyCellFilter;
            TableView.RowDeleteHandler = OnDeleteRows;
        }

        public Action<IFeature> ZoomToFeature { get; set; }

        public Action<object> OpenViewMethod { get; set; }

        public Action DeleteSelectedFeatures { get; set; }

        public TableView TableView { get; private set; }

        public Func<string, bool> DynamicAttributeVisible
        {
            private get
            {
                return dynamicAttributeVisible;
            }
            set
            {
                dynamicAttributeVisible = value;
                UpdateTableDataSource();
            }
        }

        public object Data
        {
            get
            {
                return layer;
            }
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

        public IEnumerable<IFeature> SelectedFeatures
        {
            get
            {
                if (layer.DataSource != null)
                {
                    var rowIndices = TableView.SelectedCells.Select(c => c.RowIndex).Distinct();

                    return rowIndices.Where(i => i >= 0)
                                     .Select(i => layer.DataSource.Features[TableView.GetDataSourceIndexByRowIndex(i)])
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

                TableView.ClearSelection();

                var features = value.Where(f => f.GetType().Implements(layer.DataSource.FeatureType)).ToList();

                if (features.Count == 0)
                {
                    return;
                }

                TableView.SelectRows(features.Select(f => TableView.GetRowIndexByDataSourceIndex(layer.DataSource.Features.IndexOf(f))).ToArray());

                TableView.FocusedRowIndex = TableView.SelectedRowsIndices.FirstOrDefault();
            }
        }

        public ILayer Layer { set; get; }

        /// <summary>
        /// Optional factory method to create an object to be bound to the table view for a given feature.
        /// </summary>
        public void SetCreateFeatureRowFunction<TFeatureRow>(Func<IFeature, TFeatureRow> createFeatureRowFunction) where TFeatureRow : class, IFeatureRowObject
        {
            createFeatureRowObject = createFeatureRowFunction;
            featureRowType = typeof(TFeatureRow);

            if (layer != null)
            {
                UpdateTableDataSource();
            }
        }

        public void OnActivated() {}

        public void OnDeactivated() {}

        private bool OnDeleteRows()
        {
            if (DeleteSelectedFeatures == null)
            {
                return false;
            }

            DeleteSelectedFeatures();
            SelectedFeatures = Enumerable.Empty<IFeature>();
            return true;
        }

        private bool OnReadOnlyCellFilter(TableViewCell c)
        {
            if (c.Column.IsUnbound)
            {
                return false;
            }

            if (TableView.Data == null)
            {
                return true;
            }

            var rowObjects = (IList) TableView.Data;
            if (rowObjects.Count <= 0)
            {
                return true;
            }

            var dataSourceIndexByRowIndex = TableView.GetDataSourceIndexByRowIndex(c.RowIndex);
            if (dataSourceIndexByRowIndex < 0)
            {
                return false;
            }

            var rowObject = rowObjects[dataSourceIndexByRowIndex];

            return DynamicReadOnlyAttribute.IsReadOnly(rowObject, c.Column.Name);
        }

        private void DataSourceFeaturesChanged(object sender, EventArgs e)
        {
            if (createFeatureRowObject == null)
            {
                TableView.ScheduleRefresh();
            }
            else if (!removingFeatures)
            {
                TableView.Data = null; // clear previous subscriptions (property changed)
                TableView.Data = CreateFeatureRowList(layer.DataSource.Features);
            }
        }

        private void UpdateTableDataSource()
        {
            ConfigureStaticAttributeColumns();
            ConfigureDynamicAttributeColumns();

            TableView.Data = createFeatureRowObject == null
                                 ? layer.DataSource.Features
                                 : CreateFeatureRowList(layer.DataSource.Features);

            TableView.AllowDeleteRow = !layer.ReadOnly && layer.FeatureEditor != null;

            TableView.RefreshData();
            TableView.BestFitColumns();
        }

        private IList CreateFeatureRowList(IList features)
        {
            CleanUpFeatureRows();

            // Create and initialize EventedList<TFeatureRowObject>. Type needs to be the specific featureRowObject (binding needs the 
            // type to get all the properties)
            featureRowList = (IList) TypeUtils.CreateGeneric(typeof(EventedList<>), new[]
            {
                featureRowType
            });

            foreach (var featureRowObject in features.OfType<IFeature>().Select(createFeatureRowObject))
            {
                featureRowList.Add(featureRowObject);
            }

            ((INotifyCollectionChanged) featureRowList).CollectionChanged += FeatureRowListCollectionChanged;

            return featureRowList;
        }

        private void CleanUpFeatureRows()
        {
            if (featureRowList == null)
            {
                return;
            }

            ((INotifyCollectionChanged) featureRowList).CollectionChanged -= FeatureRowListCollectionChanged;

            foreach (var featureRowObject in featureRowList.OfType<IDisposable>())
            {
                featureRowObject.Dispose();
            }

            featureRowList = null;
        }

        private void FeatureRowListCollectionChanged(object sender, NotifyCollectionChangeEventArgs e)
        {
            // if item is removed using this table
            if (e.Action != NotifyCollectionChangeAction.Remove)
            {
                return;
            }

            removingFeatures = true;
            var feature = ((IFeatureRowObject) e.Item).GetFeature();
            layer.FeatureEditor.CreateInteractor(layer, feature).Delete();
            removingFeatures = false;
        }

        private void ConfigureStaticAttributeColumns()
        {
            TableView.ClearColumns();
            TableView.AutoGenerateColumns = false;

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

                    TableView.AddColumn(name, displayName ?? name, isReadOnly, 100, displayFormat: displayFormat);
                }
            }
            else
            {
                foreach (var propertyInfo in featureRowType.GetProperties())
                {
                    var customAttributes = propertyInfo.GetCustomAttributes(true);

                    if (customAttributes.OfType<BrowsableAttribute>().Any(a => !a.Browsable))
                    {
                        continue;
                    }

                    var name = propertyInfo.Name;

                    var displayName = customAttributes.OfType<DisplayNameAttribute>().Select(a => a.DisplayName).FirstOrDefault();
                    var displayFormat = customAttributes.OfType<DisplayFormatAttribute>().Select(a => a.FormatString).FirstOrDefault();
                    var isReadOnly = FeatureAttributeAccessorHelper.IsReadOnly(featureRowType, name);

                    TableView.AddColumn(name, displayName ?? name, isReadOnly, 100, displayFormat: displayFormat);
                }
            }
        }

        private bool IsVisible(string attributeKey)
        {
            return DynamicAttributeVisible == null || DynamicAttributeVisible(attributeKey);
        }

        private void ConfigureDynamicAttributeColumns()
        {
            TableView.RemoveAllWhere(c => c.IsUnbound);

            var attributes = layer.DataSource.Features.Cast<IFeature>()
                                  .Where(f => f.Attributes != null).SelectMany(feature => feature.Attributes.Keys)
                                  .Distinct().Where(IsVisible);

            var index = TableView.Columns.Count;

            foreach (var attribute in attributes)
            {
                var columnName = attribute;

                var column = TableView.Columns.FirstOrDefault(c => c.Name == columnName);

                if (column != null)
                {
                    continue;
                }

                TableView.AddUnboundColumn(columnName, typeof(string), index++);
            }
        }

        private void InitializeDynamicAttributeContextMenu()
        {
            const string zoomItemName = "btnzoomToMenuItem";
            const string openViewItemName = "btnOpenViewMenuItem";
            string zoomToItemCaption = Resources.VectorLayerAttributeTableView_InitializeDynamicAttributeContextMenu_Zoom_to_item;
            string openViewCaption = Resources.VectorLayerAttributeTableView_InitializeDynamicAttributeContextMenu_Open_view;

            if (TableView.RowContextMenu.Items.OfType<ToolStripItem>().All(mi => mi.Name != zoomItemName))
            {
                var btnzoomToMenuItem = new ToolStripMenuItem
                {
                    Name = zoomItemName,
                    Text = zoomToItemCaption,
                    Image = Resources.MagnifierArrow
                };
                btnzoomToMenuItem.Click += BtnZoomToClick;
                btnzoomToMenuItem.Tag = TableView;
                TableView.RowContextMenu.Items.Add(btnzoomToMenuItem);
            }
            if (TableView.RowContextMenu.Items.OfType<ToolStripItem>().All(mi => mi.Name != openViewItemName))
            {
                var btnOpenViewMenuItem = new ToolStripMenuItem
                {
                    Name = openViewItemName,
                    Text = openViewCaption,
                    Image = Resources.Properties
                };
                btnOpenViewMenuItem.Click += BtnOpenViewClick;
                btnOpenViewMenuItem.Tag = TableView;
                btnOpenViewMenuItem.Font = new Font(btnOpenViewMenuItem.Font, FontStyle.Bold);
                TableView.RowContextMenu.Items.Add(btnOpenViewMenuItem);
            }
            TableView.UnboundColumnData = TableViewUnboundColumnDataUpdating;

            if (TableView.ColumnMenuItems.All(mi => mi.Caption != zoomToItemCaption))
            {
                var zoomToMenuItem = new TableViewColumnMenuItem(zoomToItemCaption)
                {
                    Image = Resources.MagnifierArrow
                };
                zoomToMenuItem.Click += BtnZoomToClick;
                TableView.ColumnMenuItems.Add(zoomToMenuItem);
            }
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

        private object TableViewUnboundColumnDataUpdating(int column, int dataSourceIndex, bool isGetData, bool isSetData, object value)
        {
            var featureIndex = dataSourceIndex;
            var feature = layer.DataSource.GetFeature(featureIndex);

            if (TableView.Columns.Count <= column)
            {
                return value;
            }

            var attributeName = TableView.Columns[column].Name;

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

                var type = typeof(string);
                if (feature.Attributes.TryGetValue(attributeName, out currentValue) && currentValue != null)
                {
                    type = currentValue.GetType();
                    feature.Attributes[attributeName] = TypeConverter.ConvertValueToTargetType(type, value);
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
            {
                SelectedFeaturesChanged(this, e);
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Core.Gis.GeoApi.Extensions.Feature;

namespace Core.GIS.SharpMap.Data
{
    /// <summary>
    /// Returns column names and values of the underlying DataRow as attributes of the feature, (key, value) pairs
    /// </summary>
    public class FeatureDataRowAttributeAccessor : IFeatureAttributeCollection
    {
        private IList<string> columnNames;
        private readonly DataRow featureDataRow;

        private readonly DataTable table;

        public FeatureDataRowAttributeAccessor(DataRow row)
        {
            featureDataRow = row;

            table = row.Table;
            row.Table.Columns.CollectionChanged += Columns_CollectionChanged;

            UpdateColumnNames();
        }

        /*object IFeatureAttributeCollection.this[int index]
        {
            get { return featureDataRow[index]; }
            set { featureDataRow[index] = value; }
        }*/

        public IDictionary<string, object> InnerDictionary { get; set; }

        public long Id { get; set; }

        public int Count
        {
            get
            {
                return columnNames.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return columnNames;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return featureDataRow.ItemArray;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            for (int i = 0; i < columnNames.Count; i++)
            {
                yield return new KeyValuePair<string, object>(columnNames[i], featureDataRow[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException("Add it using IFeatureProvider interface");
        }

        public void Clear()
        {
            throw new NotSupportedException("Delete it using IFeatureProvider interface");
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            for (int i = 0; i < columnNames.Count; i++)
            {
                if (columnNames[i] == item.Key && featureDataRow[i] == item.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException("Delete it using IFeatureProvider interface");
        }

        public bool ContainsKey(string key)
        {
            return columnNames.Contains(key);
        }

        public void Add(string key, object value)
        {
            throw new NotSupportedException("Add it using IFeatureProvider interface");
        }

        public bool Remove(string key)
        {
            throw new NotSupportedException("Delete it using IFeatureProvider interface");
        }

        public bool TryGetValue(string key, out object value)
        {
            if (columnNames.Contains(key))
            {
                value = featureDataRow[columnNames.IndexOf(key)];
                return true;
            }

            value = null;
            return false;
        }

        public object Clone()
        {
            var clone = new FeatureDataRowAttributeAccessor(featureDataRow);
            return clone;
        }

        private void Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            UpdateColumnNames();
        }

        private void UpdateColumnNames()
        {
            columnNames = new List<string>();
            foreach (DataColumn column in table.Columns)
            {
                columnNames.Add(column.ColumnName);
            }
        }

        object IDictionary<string, object>.this[string key]
        {
            get
            {
                return featureDataRow[columnNames.IndexOf(key)];
            }
            set
            {
                featureDataRow[columnNames.IndexOf(key)] = value;
            }
        }
    }
}
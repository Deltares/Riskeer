using System;
using System.Data;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;

namespace Core.GIS.SharpMap.Data
{
    /// <summary>
    /// Represents a row of data in a FeatureDataTable.
    /// </summary>
    // [System.Diagnostics.DebuggerStepThrough()]
    [Serializable]
    public class FeatureDataRow : DataRow, IFeature, IComparable
    {
        private IGeometry geometry;
        private IFeatureAttributeCollection attributes;
        //private FeatureDataTable tableFeatureTable;

        internal FeatureDataRow(DataRowBuilder rb) : base(rb) {}

        public string Name { get; set; }

        /// <summary>
        /// The geometry of the current feature
        /// </summary>
        public IGeometry Geometry
        {
            get
            {
                return geometry;
            }
            set
            {
                IGeometry oldGeometry = geometry;
                geometry = value;
                FeatureDataRowChangeEventArgs e = new FeatureDataRowChangeEventArgs(this, DataRowAction.Change);
                e.OldGeometry = oldGeometry;
                ((FeatureDataTable) Table).OnFeatureDataRowGeometryChanged(e);
            }
        }

        public IFeatureAttributeCollection Attributes
        {
            get
            {
                return attributes;
            }
            set
            {
                attributes = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((FeatureDataRow) obj);
        }

        public override int GetHashCode()
        {
            return ItemArray.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            FeatureDataRow row = (FeatureDataRow) obj;

            return Table.Rows.IndexOf(this).CompareTo(Table.Rows.IndexOf(row));
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        protected bool Equals(FeatureDataRow other)
        {
            var thisItems = ItemArray;
            var otherItems = other.ItemArray;

            if (thisItems.Length != otherItems.Length)
            {
                return false;
            }

            for (int i = 0; i < otherItems.Length; i++)
            {
                if (!Equals(thisItems[i], otherItems[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
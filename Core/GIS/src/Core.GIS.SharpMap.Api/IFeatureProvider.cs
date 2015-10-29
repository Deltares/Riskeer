// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using Core.Gis.GeoApi.CoordinateSystems;
using Core.Gis.GeoApi.Extensions.Feature;
using Core.Gis.GeoApi.Geometries;

namespace Core.GIS.SharpMap.Api
{
    /// <summary>
    /// Interface for data providers
    /// TODO: move all editing-related properties / functions into a separate interface, IFeatureInteractor, IFeatureCollection?
    /// </summary>
    public interface IFeatureProvider : IDisposable
    {
        event EventHandler FeaturesChanged;
        event EventHandler CoordinateSystemChanged;

        /// <summary>
        /// Type of the features provided.
        /// </summary>
        Type FeatureType { get; }

        IList Features { get; }

        bool IsReadOnly { get; }

        [Obsolete("Create features somewhere else (e.g. in IFeatureEditor and add them to the repository")]
        Func<IFeatureProvider, IGeometry, IFeature> AddNewFeatureFromGeometryDelegate { get; set; }

        /// <summary>
        /// The spatial reference system code (WKT).
        /// </summary>
        string SrsWkt { get; set; }

        /// <summary>
        /// Gets or sets the coordinate system.
        /// </summary>
        ICoordinateSystem CoordinateSystem { get; set; }

        /// <summary>
        /// Adds a new feature to the feature storage using geometry.
        /// </summary>
        /// <param name="geometry"></param>
        [Obsolete("Create features somewhere else (e.g. in IFeatureEditor and add them to the repository")]
        IFeature Add(IGeometry geometry);

        /// <summary>
        /// Returns the number of features in the dataset
        /// </summary>
        /// <returns>number of features</returns>
        int GetFeatureCount();

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns>geometry</returns>
        IGeometry GetGeometryByID(int oid);

        /// <summary>
        /// Returns a <see cref="SharpMap.Data.FeatureDataRow"/> based on a RowID
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>datarow</returns>
        IFeature GetFeature(int index);

        /// <summary>
        /// Returns true if feature belongs to provider.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        bool Contains(IFeature feature);

        /// <summary>
        /// Returns the index of the feature in the internal list, or -1 if it does not contain the feature
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        int IndexOf(IFeature feature);

        /// <summary>
        /// <see cref="IEnvelope"/> of dataset
        /// </summary>
        /// <returns>boundingbox</returns>
        IEnvelope GetExtents();

        IEnvelope GetBounds(int recordIndex);
    }
}
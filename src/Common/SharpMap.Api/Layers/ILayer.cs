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
using System.Collections.Generic;
using System.Drawing;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using SharpMap.Api.Editors;

namespace SharpMap.Api.Layers
{
    /// <summary>
    /// Interface for map layers
    /// </summary>
    public interface ILayer : ICloneable, IDisposable
    {
        /// <summary>
        /// Image of the layer for current map, layer uses it to render it's content to.
        /// Layer image contains only graphics rendered by one layer.
        /// </summary>
        Image Image { get; }

        void ClearImage();

        /// <summary>
        /// Disposes the layer (releases all resources used by the layer)
        /// </summary>
        /// <param name="disposeDataSource">Option to determine if the <see cref="DataSource"/> should also be disposed</param>
        void Dispose(bool disposeDataSource = true);

        /// <summary>
        /// True if you don't want the extent of this layer to be included in the total map extent.
        /// </summary>
        bool ExcludeFromMapExtent { get; set; }

        /// <summary>
        /// Is not visible by default.
        /// </summary>
        ILabelLayer LabelLayer { get; set; }

        /// <summary>
        /// True if the name is not allowed to be modified in the UI. set_Name will throw exception if you attempt to change the name while this flag is true.
        /// </summary>
        bool NameIsReadOnly { get; }

        /// <summary>
        /// Use this method to render layer manually. Results will be rendered into Image property.
        /// 
        /// This method should call OnRender which can be overriden in the implementations.
        /// </summary>
        void Render();

        /// <summary>
        /// Custom renderers which can be added to the layer and used to render something in addition to / instead of default rendering.
        /// </summary>
        IList<IFeatureRenderer> CustomRenderers { get; set; }

        /// <summary>
        /// True if layers needs to be rendered. Map will check this flag while it will render itself.
        /// If flag is set to true - Render() will be called before Image is drawn on Map.
        /// 
        /// Setting this flag to true in some layers and calling Map.Refresh() will make sure that only required layers will be rendered.
        /// 
        /// Calling Render() resets this flag automatically.
        /// </summary>
        bool RenderRequired { get; set; }

        /// <summary>
        /// Duration of last rendering in ms.
        /// </summary>
        double LastRenderDuration { get; }

        /// <summary>
        /// Minimum visible zoom level
        /// </summary>
        double MinVisible { get; set; }

        /// <summary>
        /// Minimum visible zoom level
        /// </summary>
        double MaxVisible { get; set; }

        /// <summary>
        /// Specifies whether this layer should be rendered or not
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Name of layer
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the boundingbox of the entire layer
        /// </summary>
        GeoAPI.Geometries.IEnvelope Envelope { get; }
        //System.Collections.Generic.List<T> Features { get; }

        /// <summary>
        /// Gets or sets map where this layer belongs to, or null.
        /// </summary>
        IMap Map { get; set; }

        /// <summary>
        /// Defines if layer should be shown in a legend of map layers. Useful to hide supplementary layers such as drawing Trackers or geometries.
        /// </summary>
        bool ShowInLegend { get; set; }

        /// <summary>
        /// Determines if the labels of the layer are shown in the map. Uses the underlying LabelLayer to customize the display style.
        /// </summary>
        bool ShowLabels { get; set; }
        
        /// <summary>
        /// Defines if layer should be shown in a treeview of map layers. Useful to hide supplementary layers such as drawing Trackers or geometries.
        /// </summary>
        bool ShowInTreeView { get; set; }

        /// <summary>
        /// Determines whether the layer is mutable.
        /// </summary>
        bool ReadOnly { get; set; }

        /// <summary>
        /// Gets coordinate system of the layer.
        /// </summary>
        ICoordinateSystem CoordinateSystem { get; }

        /// <summary>
        /// Gets or sets the <see cref="GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation"/> applied to this layer prior to rendering
        /// </summary>
        ICoordinateTransformation CoordinateTransformation { get; set; }

        /// <summary>
        /// Gets features using envelope.
        /// </summary>
        /// <param name="envelope">Envelope, in current layer coordinate system (not in DataSoure)</param>
        /// <returns></returns>
        IEnumerable<IFeature> GetFeatures(IEnvelope envelope);

        /// <summary>
        /// Gets features using geometry.
        /// </summary>
        /// <param name="geometry">Geometry, defined in current layer coordinate system (not in DataSoure)</param>
        /// <param name="useCustomRenderers"></param>
        /// <returns></returns>
        IEnumerable<IFeature> GetFeatures(IGeometry geometry, bool useCustomRenderers = true);

        /// <summary>
        /// Provides access to the features used by the current layer.
        /// </summary>
        IFeatureProvider DataSource { get; set; }

        /// <summary>
        /// Allows editing of features.
        /// </summary>
	    IFeatureEditor FeatureEditor { get; set; }
       
        /// <summary>
        /// Symbology used to render features of the current layer.
        /// </summary>
        ITheme Theme { get; set; }

	    /// <summary>
        /// Can features of the layer be selected. 
        /// This defaults to Selectable and Visible.
        /// </summary>
        bool IsSelectable { get; }

        /// <summary>
        /// Defines whether the features in the layer should be selectable
        /// </summary>
        bool Selectable { get; set; }

        /// <summary>
        /// Determines if the theme should be updated when values of the coverage change.
        /// </summary>
        bool AutoUpdateThemeOnDataSourceChanged { get; set; }

        /// <summary>
        /// Determines if an attribute table can be shown for this layer
        /// </summary>
        bool ShowAttributeTable { get; set; }

        /// <summary>
        /// Determines the order of rendering
        /// </summary>
        int RenderOrder { get; set; }

        string ThemeGroup { get; set; }

        bool ThemeIsDirty { get; set; }

        string ThemeAttributeName { get; }

        double MinDataValue { get; }

        double MaxDataValue { get; }

        bool CanBeRemovedByUser { get; set; }
        
        /// <summary>
        /// Defines the layer opacity, expressed as a value between 0.0 and 1.0. A value of 0.0 indicates fully transparent.
        /// </summary>
        float Opacity { get; set; }
    }
}

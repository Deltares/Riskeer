using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using DelftTools.Utils;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using SharpMap.Api.Delegates;
using SharpMap.Api.Layers;

namespace SharpMap.Api
{
    public interface IMap : INameable, ICloneable
    {
        /// <summary>
        /// Event fired when the zoomlevel or the center point has been changed
        /// </summary>
        event MapViewChangedHandler MapViewOnChange;

        event MapLayerRenderedEventHandler MapLayerRendered;

        /// <summary>
        /// Event fired when all layers have been rendered
        /// </summary>
        event MapRenderedEventHandler MapRendered;

        event MapRenderedEventHandler MapRendering;

        Image Image { get; }

        /// <summary>
        /// Gets or sets a flag indicating if we should draw grid (usually latitude / longitude projected to the current map coordinate system).
        /// 
        /// TODO: extract this into IMapDecoration, together with tools like NorthArrow, ScaleBar ...
        /// </summary>
        bool ShowGrid { get; set; }

        /// <summary>
        /// Gets the extents of the current map based on the current zoom, center and mapsize
        /// </summary>
        IEnvelope Envelope { get; }

        /// <summary>
        /// Using the <see cref="MapTransform"/> you can alter the coordinate system of the map rendering.
        /// This makes it possible to rotate or rescale the image, for instance to have another direction than north upwards.
        /// </summary>
        /// <example>
        /// Rotate the map output 45 degrees around its center:
        /// <code lang="C#">
        /// System.Drawing.Drawing2D.Matrix maptransform = new System.Drawing.Drawing2D.Matrix(); //Create transformation matrix
        ///	maptransform.RotateAt(45,new PointF(myMap.Size.Width/2,myMap.Size.Height/2)); //Apply 45 degrees rotation around the center of the map
        ///	myMap.MapTransform = maptransform; //Apply transformation to map
        /// </code>
        /// </example>
        Matrix MapTransform { get; set; }

        ICoordinateSystem CoordinateSystem { get; set; }

        /// <summary>
        /// A collection of layers. The first layer in the list is drawn first, the last one on top.
        /// </summary>
        IEventedList<ILayer> Layers { get; set; }

        /// <summary>
        /// Map background color (defaults to transparent)
        /// </summary>
        Color BackColor { get; set; }

        /// <summary>
        /// Center of map in WCS
        /// </summary>
        ICoordinate Center { get; set; }

        /// <summary>
        /// Gets or sets the zoom level of map.
        /// </summary>
        /// <remarks>
        /// <para>The zoom level corresponds to the width of the map in WCS units.</para>
        /// <para>A zoomlevel of 0 will result in an empty map being rendered, but will not throw an exception</para>
        /// </remarks>
        double Zoom { get; set; }

        double WorldHeight { get; }
        double WorldLeft { get; }
        double WorldTop { get; }

        /// <summary>
        /// Returns the size of a pixel in world coordinate units
        /// </summary>
        double PixelSize { get; }

        /// <summary>
        /// Returns the width of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="PixelSize"/>.</remarks>
        double PixelWidth { get; }

        /// <summary>
        /// Returns the height of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="PixelSize"/> unless <see cref="PixelAspectRatio"/> is different from 1.</remarks>
        double PixelHeight { get; }

        /// <summary>
        /// Gets or sets the aspect-ratio of the pixel scales. A value less than 
        /// 1 will make the map stretch upwards, and larger than 1 will make it smaller.
        /// </summary>
        /// <exception cref="ArgumentException">Throws an argument exception when value is 0 or less.</exception>
        double PixelAspectRatio { get; set; }

        /// <summary>
        /// Height of map in world units
        /// </summary>
        /// <returns></returns>
        double MapHeight { get; }

        /// <summary>
        /// Size of output map
        /// </summary>
        Size Size { get; set; }

        /// <summary>
        /// Minimum zoom amount allowed
        /// </summary>
        double MinimumZoom { get; set; }

        /// <summary>
        /// Maximum zoom amount allowed
        /// </summary>
        double MaximumZoom { get; set; }

        string Name { get; set; }
        bool HasDefaultEnvelopeSet { get; }

        bool IsDisposing { get; }

        /// <summary>
        /// Replacing layer is used, because we cannot use Layers[i] = layer.
        /// This is because there are a lot of places that have a NotImplementedException when 
        /// a replace event in the collection occurs.
        /// HACK
        /// </summary>
        bool ReplacingLayer { get; }

        void ClearImage();

        /// <summary>
        /// Renders the map to an image
        /// </summary>
        /// <returns></returns>
        Image Render();

        /// <summary>
        /// Returns an enumerable for all layers containing the search parameter in the LayerName property
        /// </summary>
        /// <param name="layername">Search parameter</param>
        /// <returns>IEnumerable</returns>
        IEnumerable<ILayer> FindLayer(string layername);

        /// <summary>
        /// Returns a layer by its name
        /// </summary>
        /// <param name="layerName">Name of layer</param>
        /// <returns>Layer</returns>
        ILayer GetLayerByName(string layerName);

        /// <summary>
        /// Returns the (first) layer on which <paramref name="feature"/> is present.
        /// </summary>
        /// <param name="feature">The feature to search for.</param>
        /// <returns>The layer that contains the <paramref name="feature"/>. Null if not layer can be found.</returns>
        ILayer GetLayerByFeature(IFeature feature);

        /// <summary>
        /// Find the grouplayer for a given layer. Returns null if the layer is not contained in a group.
        /// </summary>
        /// <param name="childLayer">Child layer to be found</param>
        /// <returns>Grouplayer containing the childlayer or null if no grouplayer is found</returns>
        IGroupLayer GetGroupLayerContainingLayer(ILayer childLayer);

        void DoWithLayerRecursive(ILayer layer, Action<ILayer> action);

        IEnumerable<ILayer> GetAllLayers(bool includeGroupLayers);

        IEnumerable<ILayer> GetAllVisibleLayers(bool includeGroupLayers);

        /// <summary>
        /// Zooms to the extents of all layers
        /// Adds an extra 10 % margin to each border
        /// </summary>
        void ZoomToExtents();

        /// <summary>
        /// Sets the layer in front of all other layers (by changing the rendering order number of the layer)
        /// </summary>
        /// <param name="layer"></param>
        void BringToFront(ILayer layer);

        /// <summary>
        /// Sets the layer behind all other layers (by changing the rendering order number of the layer)
        /// </summary>
        /// <param name="layer"></param>
        void SendToBack(ILayer layer);

        void SendBackward(ILayer layer);

        void BringForward(ILayer layer);

        /// <summary>
        /// Zooms the map to fit a bounding box
        /// </summary>
        /// <remarks>
        /// NOTE: If the aspect ratio of the box and the aspect ratio of the mapsize
        /// isn't the same, the resulting map-envelope will be adjusted so that it contains
        /// the bounding box, thus making the resulting envelope larger!
        /// </remarks>
        /// <param name="bbox"></param>
        void ZoomToFit(IEnvelope bbox);

        /// <summary>
        /// Zooms the map to fit a bounding box. 
        /// </summary>
        /// <remarks>
        /// NOTE: If the aspect ratio of the box and the aspect ratio of the mapsize
        /// isn't the same, the resulting map-envelope will be adjusted so that it contains
        /// the bounding box, thus making the resulting envelope larger!
        /// </remarks>
        /// <param name="bbox"></param>
        /// <param name="addMargin">Add a default margin?</param>
        void ZoomToFit(IEnvelope bbox, bool addMargin);

        /// <summary>
        /// Converts a point from world coordinates to image coordinates based on the current
        /// zoom, center and mapsize.
        /// </summary>
        /// <param name="p">Point in world coordinates</param>
        /// <returns>Point in image coordinates</returns>
        PointF WorldToImage(ICoordinate p);

        /// <summary>
        /// Converts a point from image coordinates to world coordinates based on the current
        /// zoom, center and mapsize.
        /// </summary>
        /// <param name="p">Point in image coordinates</param>
        /// <returns>Point in world coordinates</returns>
        ICoordinate ImageToWorld(PointF p);

        /// <summary>
        /// Gets the extents of the map based on the extents of all the layers in the layers collection
        /// </summary>
        /// <returns>Full map extents</returns>
        IEnvelope GetExtents();

        object Clone();

        /// <summary>
        /// Replace a layer with another layer.
        /// This function is created, because we can't simply call Layer[i] = layer;
        /// </summary>
        /// <param name="sourceLayer"></param>
        /// <param name="targetLayer"></param>
        void ReplaceLayer(ILayer sourceLayer, ILayer targetLayer);
    }
}
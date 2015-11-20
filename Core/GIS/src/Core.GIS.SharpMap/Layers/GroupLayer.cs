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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Layers
{
    /// <summary>
    /// Class for holding a group of layers.
    /// </summary>
    /// <remarks>
    /// The Group layer is useful for grouping a set of layers,
    /// for instance a set of image tiles, and expose them as a single layer
    /// </remarks>
    public class GroupLayer : Layer, IGroupLayer //, IDisposable, INotifyCollectionChange
    {
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;
        public virtual event NotifyCollectionChangingEventHandler CollectionChanging;

        protected bool layersReadOnly;

        private readonly bool created = false;

        private EventedList<ILayer> layers;
        private bool isMapInitialized; // performance (lazy initialization)
        private bool cloning;

        public GroupLayer() : this("group layer")
        {
            FeatureEditor = null;
        }

        /// <summary>
        /// Initializes a new group layer
        /// </summary>
        /// <param name="layername">Name of layer</param>
        public GroupLayer(string layername)
        {
            Layers = new EventedList<ILayer>();
            Name = layername;
            created = true;
        }

        public override bool RenderRequired
        {
            get
            {
                if (!created)
                {
                    return false;
                }

                if (Map != null && Map.IsDisposing)
                {
                    return false;
                }

                /* If subLayer needs redrawing the grouplayer needs redrawing.
                 * test with moving cross section along a branch. */
                foreach (ILayer layer in Layers.Where(l => l.Visible))
                {
                    if (layer.RenderRequired)
                    {
                        return true;
                    }
                }
                /**/
                return base.RenderRequired;
            }
            set
            {
                if (!created)
                {
                    return;
                }

                if (Map != null && Map.IsDisposing)
                {
                    return;
                }

                /**/
                if (value) //due to render order, propagating render required false seems wrong
                {
                    foreach (ILayer layer in Layers.Where(l => l.Visible))
                    {
                        layer.RenderRequired = value;
                    }
                }
                /**/
                base.RenderRequired = value;
            }
        }

        public override IMap Map
        {
            get
            {
                return base.Map;
            }
            set
            {
                base.Map = value;
                isMapInitialized = false;
            }
        }

        /// <summary>
        /// Sublayers in the group
        /// </summary>
        public virtual EventedList<ILayer> Layers
        {
            get
            {
                if (!isMapInitialized)
                {
                    isMapInitialized = true;
                    foreach (ILayer layer in Layers)
                    {
                        layer.Map = Map;
                    }
                }

                return layers;
            }
            set
            {
                OnPropertyChanging("Layers");

                if (layers != null)
                {
                    layers.PropertyChanging -= OnPropertyChanging;
                    layers.PropertyChanged -= OnPropertyChanged;
                    layers.CollectionChanging -= LayersCollectionChanging;
                    layers.CollectionChanged -= LayersCollectionChanged;
                }

                layers = value;

                if (layers != null)
                {
                    layers.PropertyChanging += OnPropertyChanging;
                    layers.PropertyChanged += OnPropertyChanged;
                    layers.CollectionChanging += LayersCollectionChanging;
                    layers.CollectionChanged += LayersCollectionChanged;
                }

                OnPropertyChanged("Layers");
            }
        }

        public virtual bool LayersReadOnly
        {
            get
            {
                return layersReadOnly;
            }
            set
            {
                OnPropertyChanging("LayersReadOnly");
                layersReadOnly = value;
                OnPropertyChanged("LayersReadOnly");
            }
        }

        /// <summary>
        /// Returns the extent of the layer
        /// </summary>
        /// <returns>Bounding box corresponding to the extent of the features in the layer</returns>
        public override IEnvelope Envelope
        {
            get
            {
                if (Layers.Count == 0)
                {
                    return null;
                }

                IEnvelope envelope = new Envelope();

                foreach (var layer in Layers.Where(l => l.Visible && !l.ExcludeFromMapExtent))
                {
                    var subEnvelope = layer.Envelope;
                    if (subEnvelope == null || subEnvelope.IsNull)
                    {
                        continue;
                    }
                    envelope.ExpandToInclude(subEnvelope);
                }

                return envelope;
            }
        }

        public virtual IEnumerable<ILayer> GetAllLayers(bool includeGroupLayers)
        {
            return SharpMap.Map.Map.GetLayers(Layers, includeGroupLayers, true);
        }

        /// <summary>
        /// Renders the layer
        /// </summary>
        /// <param name="g">Graphics object reference</param>
        /// <param name="map">Map which is rendered</param>
        public override void OnRender(Graphics g, IMap map)
        {
            // layers of the group layer are rendered by themselves
        }

        public override void ClearImage()
        {
            base.ClearImage();

            foreach (var layer in layers)
            {
                layer.ClearImage();
            }
        }

        /// <summary>
        /// Clones the layer
        /// </summary>
        /// <returns>cloned object</returns>
        public override object Clone()
        {
            var clonedLayerGroup = (GroupLayer) Activator.CreateInstance(GetType());
            clonedLayerGroup.cloning = true;

            clonedLayerGroup.name = name;
            clonedLayerGroup.NameIsReadOnly = NameIsReadOnly;
            //clonedLayerGroup.LayersReadOnly = false;
            clonedLayerGroup.layers.Clear();

            foreach (var layer in layers)
            {
                clonedLayerGroup.layers.Add((ILayer) layer.Clone());
            }

            clonedLayerGroup.Visible = Visible;
            clonedLayerGroup.LayersReadOnly = LayersReadOnly;
            clonedLayerGroup.cloning = false;

            return clonedLayerGroup;
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public override void Dispose(bool disposeDataSource)
        {
            var disposables = Layers.OfType<IDisposable>();

            foreach (var disposable in disposables)
            {
                var layer = disposable as Layer;
                if (layer != null)
                {
                    layer.Dispose(disposeDataSource);
                }
                else
                {
                    disposable.Dispose();
                }
            }
        }

        protected void LayersCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            // performance
            if (!created || cloning)
            {
                return;
            }

            OnLayersCollectionChanged(e);

            if (CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }

        protected override void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!created || cloning)
            {
                return;
            }

            //skip the render required logic for group layers...layers handle this individually
            //group layer should only react to local changes and renderrequired of child layers
            //if not it will result in a lot of loading exceptions during save/load
            if (ReferenceEquals(sender, this))
            {
                base.OnLayerPropertyChanged(sender, e); //handle like a 'normal' layer
            }
        }

        private void OnLayersCollectionChanged(NotifyCollectionChangingEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Add: //set map property for layers being added
                    ((ILayer) e.Item).Map = Map;
                    ((ILayer) e.Item).RenderRequired = true;
                    break;
                case NotifyCollectionChangeAction.Remove:
                    RenderRequired = true; //render the group if a layer got removed.
                    break;
                case NotifyCollectionChangeAction.Replace:
                    throw new NotImplementedException();
            }
        }

        private void LayersCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            // performance
            if (!created || cloning)
            {
                return;
            }

            if (sender == layers) //only for own layer collection
            {
                if (LayersReadOnly)
                {
                    throw new InvalidOperationException("It is not allowed to add or remove layers from a grouplayer that has a read-only layers collection");
                }
            }

            if (CollectionChanging != null)
            {
                CollectionChanging(sender, e);
            }
        }
    }
}
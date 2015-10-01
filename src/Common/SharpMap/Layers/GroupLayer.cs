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
using System.Linq;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using SharpMap.Api;
using SharpMap.Api.Layers;

namespace SharpMap.Layers
{
    /// <summary>
	/// Class for holding a group of layers.
	/// </summary>
	/// <remarks>
	/// The Group layer is useful for grouping a set of layers,
	/// for instance a set of image tiles, and expose them as a single layer
	/// </remarks>
    [Entity(FireOnCollectionChange = false)]
    //[NotifyPropertyChanged(AttributeTargetMembers = "SharpMap.Layers.LayerGroup.Map", AttributeExclude = true, AttributePriority = 2)]
    public class GroupLayer : Layer, IGroupLayer//, IDisposable, INotifyCollectionChange
	{
        public GroupLayer(): this("group layer")
        {
            FeatureEditor = null;
        }

        private bool created = false;

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

        protected void LayersCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            // performance
            if (!created || cloning)
            {
                return;
            }

            OnLayersCollectionChanged(e);

            if(CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }

        
        private void OnLayersCollectionChanged(NotifyCollectionChangingEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Add: //set map property for layers being added
                    SetMapInLayer((ILayer) e.Item);
                    ((ILayer) e.Item).RenderRequired = true;
                    break;
                case NotifyCollectionChangeAction.Remove:
                    RenderRequired = true;//render the group if a layer got removed.
                    break;
                case NotifyCollectionChangeAction.Replace:
                    throw new NotImplementedException();
            }
        }

        [EditAction]
        private void SetMapInLayer(ILayer layer)
        {
            layer.Map = Map;
        }

        void LayersCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            // performance
            if (!created || cloning)
            {
                return;
            }

            if (sender == layers) //only for own layer collection
            {
                CheckIfLayersIsMutableOrThrow();
            }

            if (CollectionChanging != null)
            {
                CollectionChanging(sender, e);
            }
        }

        [EditAction] //a bit of a hack, not strictly an edit action
        private void CheckIfLayersIsMutableOrThrow()
        {
            if (LayersReadOnly)
            {
                throw new InvalidOperationException(
                    "It is not allowed to add or remove layers from a grouplayer that has a read-only layers collection");
            }
        }

        [NoNotifyPropertyChange]
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

	    [NoNotifyPropertyChange]
        public override IMap Map
        {
            get { return base.Map; }
            set
            {
                base.Map = value;
                isMapInitialized = false;
            }
        }

        [EditAction]
        private void AfterMapSet()
        {
            foreach (ILayer layer in Layers)
            {
                layer.Map = Map;
            }
        }

        private IEventedList<ILayer> layers;
        
        /// <summary>
		/// Sublayers in the group
		/// </summary>
        public virtual IEventedList<ILayer> Layers
		{
			get
			{
                if (!isMapInitialized)
                {
                    isMapInitialized = true;
                    AfterMapSet();
                }

                return layers;
			}
			set
			{
                if(layers != null)
                {
                    layers.CollectionChanged -= LayersCollectionChanged;
                    layers.CollectionChanging -= LayersCollectionChanging;
                }
			    layers = value;
                if (layers != null)
                {
                    layers.CollectionChanged += LayersCollectionChanged;
                    layers.CollectionChanging += LayersCollectionChanging;
                }
			}
		}

        protected bool layersReadOnly;
        private bool isMapInitialized; // performance (lazy initialization)
        private bool cloning;

        public virtual bool LayersReadOnly
        {
            get { return layersReadOnly; }
            set { layersReadOnly = value; }
        }

        public override void ClearImage()
        {
            base.ClearImage();

            foreach(var layer in layers)
            {
                layer.ClearImage();
            }
        }

		/// <summary>
		/// Renders the layer
		/// </summary>
		/// <param name="g">Graphics object reference</param>
		/// <param name="map">Map which is rendered</param>
		public override void OnRender(System.Drawing.Graphics g, IMap map)
		{
            // layers of the group layer are rendered by themselves
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
		                continue;
		            envelope.ExpandToInclude(subEnvelope);
		        }

		        return envelope;
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
                clonedLayerGroup.layers.Add((ILayer)layer.Clone());
            }

            clonedLayerGroup.Visible = Visible;
            clonedLayerGroup.LayersReadOnly = LayersReadOnly;
            clonedLayerGroup.cloning = false;

            return clonedLayerGroup;
        }

        protected override void OnLayerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                base.OnLayerPropertyChanged(sender,e);//handle like a 'normal' layer
            }
        }

        public virtual IEnumerable<ILayer> GetAllLayers(bool includeGroupLayers)
        {
            return SharpMap.Map.GetLayers(Layers, includeGroupLayers, true);
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

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;
        public virtual event NotifyCollectionChangingEventHandler CollectionChanging;
        
        bool INotifyCollectionChange.HasParentIsCheckedInItems { get; set; }
        bool INotifyCollectionChange.SkipChildItemEventBubbling { get; set; }
    }
}

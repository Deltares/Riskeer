using System;
using System.Collections.Generic;
using DelftTools.Controls;
using GeoAPI.Extensions.Feature;
using SharpMap.Api.Layers;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// Provides an interface for views that provide extended info/editing for a layer. The view will be 
    /// opened as a tabs inside the central map view. Typically one layer corresponds to one view.
    /// </summary>
    public interface ILayerEditorView : IView
    {
        IEnumerable<IFeature> SelectedFeatures { get; set; }

        event EventHandler SelectedFeaturesChanged;

        /// <summary>
        /// The layer for which this view is the editor.
        /// </summary>
        ILayer Layer { set; get; }

        /// <summary>
        /// Called when this view (the tab) is activated (becomes the selected tab)
        /// </summary>
        void OnActivated();

        /// <summary>
        /// Called when this view (the tab) is no longer active (is no longer the selected tab)
        /// </summary>
        void OnDeactivated();
    }
}
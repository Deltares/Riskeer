﻿using Core.GIS.GeoAPI.Extensions.Feature;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapZoomToFeatureCommand : MapViewCommand
    {
        /// <summary>
        /// first argument IFeature is expected as 
        /// </summary>
        /// <param name="arguments"></param>
        public override void Execute(params object[] arguments)
        {
            MapView activeView = SharpMapGisGuiPlugin.GetFocusedMapView();
            if (activeView == null)
            {
                return;
            }
            var feature = arguments[0] as IFeature;
            if (feature != null)
            {
                activeView.EnsureVisible(feature);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.UI.Tools
{
    public class OpenViewMapTool : MapTool, IDisposable
    {
        public override bool AlwaysActive
        {
            get
            {
                return true;
            }
        }

        public Func<IFeature, bool> CanOpenView { get; set; }

        public Action<IFeature> OpenView { get; set; }

        public override IEnumerable<MapToolContextMenuItem> GetContextMenuItems(ICoordinate worldPosition)
        {
            var editFeatureMenu = CreateContextMenuItemForFeaturesAtLocation(worldPosition, "Edit", OnOpenView, true, OnFilterFeature);
            if (editFeatureMenu == null || editFeatureMenu.DropDownItems.Count == 0)
            {
                yield break;
            }

            yield return new MapToolContextMenuItem
            {
                Priority = 0,
                MenuItem = editFeatureMenu
            };
        }

        public void Dispose()
        {
            CanOpenView = null;
            OpenView = null;
        }

        private bool OnFilterFeature(ILayer layer, IFeature feature)
        {
            return CanOpenView == null || !CanOpenView(feature);
        }

        private void OnOpenView(ILayer layer, IFeature feature)
        {
            if (OpenView == null)
            {
                return;
            }
            OpenView(feature);
        }
    }
}
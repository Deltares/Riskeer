// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Core.Components.Gis.IO.Importers;
using Core.Plugins.Map.Commands;
using Core.Plugins.Map.Helpers;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.PresentationObjects;
using Core.Plugins.Map.Properties;
using Core.Plugins.Map.PropertyClasses;

namespace Core.Plugins.Map
{
    /// <summary>
    /// The plug-in that ties together all the components to enable map interactions.
    /// </summary>
    public class MapPlugin : PluginBase
    {
        private bool activated;
        private IMapView currentMapView;
        private MapRibbon mapRibbon;
        private MapLegendController mapLegendController;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return mapRibbon;
            }
        }

        public override void Activate()
        {
            mapLegendController = CreateLegendController(Gui);
            mapRibbon = CreateMapRibbon();

            mapLegendController.ToggleView();
            Gui.ViewHost.ViewOpened += OnViewOpened;
            Gui.ViewHost.ViewBroughtToFront += OnViewBroughtToFront;
            Gui.ViewHost.ViewClosed += OnViewClosed;
            Gui.ViewHost.ActiveDocumentViewChanged += OnActiveDocumentViewChanged;
            activated = true;
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<MapDataCollectionContext, MapDataCollectionProperties>
            {
                CreateInstance = context => new MapDataCollectionProperties((MapDataCollection) context.WrappedData, MapDataContextHelper.GetParentsFromContext(context))
            };
            yield return new PropertyInfo<MapPointData, MapPointDataProperties>
            {
                CreateInstance = data => new MapPointDataProperties(data)
            };
            yield return new PropertyInfo<MapLineData, MapLineDataProperties>
            {
                CreateInstance = data => new MapLineDataProperties(data)
            };
            yield return new PropertyInfo<MapPolygonData, MapPolygonDataProperties>
            {
                CreateInstance = data => new MapPolygonDataProperties(data)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<MapDataCollection>
            {
                Name = Resources.Name_Layer,
                Category = Resources.Categories_Layer,
                Image = Resources.MapPlusIcon,
                FileFilterGenerator = new FileFilterGenerator(
                    Resources.MapPlugin_GetImportInfos_MapDataCollection_filefilter_Extension,
                    Resources.MapPlugin_GetImportInfos_MapDataCollection_filefilter_Description),
                IsEnabled = mapDataCollection => true,
                CreateFileImporter = (mapDataCollection, filePath) => new FeatureBasedMapDataImporter(mapDataCollection, filePath)
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (activated)
                {
                    Gui.ViewHost.ViewOpened -= OnViewOpened;
                    Gui.ViewHost.ViewBroughtToFront -= OnViewBroughtToFront;
                    Gui.ViewHost.ViewClosed -= OnViewClosed;
                    Gui.ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
                }

                mapLegendController?.Dispose();
            }

            base.Dispose(disposing);
        }

        private MapLegendController CreateLegendController(IViewController viewController)
        {
            if (viewController == null)
            {
                throw new ArgumentNullException(nameof(viewController),
                                                $@"Cannot create a {typeof(MapLegendController).Name} when the view controller is null");
            }

            var controller = new MapLegendController(viewController, Gui);
            controller.OnOpenLegend += (s, e) => UpdateComponentsForActiveDocumentView();
            return controller;
        }

        private MapRibbon CreateMapRibbon()
        {
            return new MapRibbon
            {
                ToggleLegendViewCommand = new ToggleMapLegendViewCommand(mapLegendController)
            };
        }

        private void OnViewOpened(object sender, ViewChangeEventArgs e)
        {
            var view = e.View as IMapView;
            view?.Map.ZoomToAllVisibleLayers();

            UpdateComponentsForView(view);
        }

        private void OnViewBroughtToFront(object sender, ViewChangeEventArgs e)
        {
            UpdateComponentsForView(e.View as IMapView);
        }

        private void OnViewClosed(object sender, ViewChangeEventArgs e)
        {
            if (ReferenceEquals(currentMapView, e.View))
            {
                UpdateComponentsForView(null);
            }
        }

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            UpdateComponentsForActiveDocumentView();
        }

        private void UpdateComponentsForActiveDocumentView()
        {
            UpdateComponentsForView(Gui.ViewHost.ActiveDocumentView as IMapView);
        }

        private void UpdateComponentsForView(IMapView mapView)
        {
            if (ReferenceEquals(currentMapView, mapView))
            {
                return;
            }

            currentMapView = mapView;

            IMapControl mapControl = mapView?.Map;
            mapLegendController.Update(mapControl);
            mapRibbon.Map = mapControl;
        }
    }
}
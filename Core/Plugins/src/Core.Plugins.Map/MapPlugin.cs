﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Core.Components.Gis.IO.Importers;
using Core.Plugins.Map.Commands;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.Properties;
using Core.Plugins.Map.PropertyClasses;

namespace Core.Plugins.Map
{
    /// <summary>
    /// The plug-in that ties together all the components to enable map interactions.
    /// </summary>
    public class MapPlugin : PluginBase
    {
        private MapRibbon mapRibbon;
        private MapLegendController mapLegendController;
        private bool activated;

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
            Gui.ViewHost.ActiveDocumentViewChanged += OnActiveDocumentViewChanged;
            Gui.ViewHost.ViewOpened += OnViewOpened;
            activated = true;
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<MapDataCollection, MapDataCollectionProperties>();
            yield return new PropertyInfo<MapPointData, MapPointDataProperties>();
            yield return new PropertyInfo<MapLineData, MapLineDataProperties>();
            yield return new PropertyInfo<MapPolygonData, MapPolygonDataProperties>();
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

        public override void Dispose()
        {
            if (activated)
            {
                Gui.ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
                Gui.ViewHost.ViewOpened -= OnViewOpened;
            }
            mapLegendController?.Dispose();

            base.Dispose();
        }

        private MapLegendController CreateLegendController(IViewController viewController)
        {
            if (viewController == null)
            {
                throw new ArgumentNullException(nameof(viewController),
                                                $"Cannot create a {typeof(MapLegendController).Name} when the view controller is null");
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

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            UpdateComponentsForActiveDocumentView();
        }

        private static void OnViewOpened(object sender, ViewChangeEventArgs e)
        {
            var view = e.View as IMapView;
            view?.Map.ZoomToAllVisibleLayers();
        }

        /// <summary>
        /// Updates the components which the <see cref="MapPlugin"/> knows about so that
        /// it reflects the currently active view.
        /// </summary>
        private void UpdateComponentsForActiveDocumentView()
        {
            var mapView = Gui.ViewHost.ActiveDocumentView as IMapView;
            IMapControl mapControl = mapView?.Map;
            mapLegendController.Update(mapControl);
            mapRibbon.Map = mapControl;
        }
    }
}
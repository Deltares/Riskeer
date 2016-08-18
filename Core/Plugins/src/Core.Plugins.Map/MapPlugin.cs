﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Gis.Forms;
using Core.Plugins.Map.Commands;
using Core.Plugins.Map.Legend;

namespace Core.Plugins.Map
{
    /// <summary>
    /// The plug-in for the <see cref="DotSpatial"/> map component.
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
            activated = true;
        }

        public override void Dispose()
        {
            if (activated)
            {
                Gui.ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
            }
            if (mapLegendController != null)
            {
                mapLegendController.Dispose();
            }

            base.Dispose();
        }

        private MapLegendController CreateLegendController(IViewController viewController)
        {
            if (viewController == null)
            {
                throw new ArgumentNullException("viewController", @"Cannot create a MapLegendController when the view controller is null");
            }

            var controller = new MapLegendController(viewController, Gui, Gui.MainWindow);
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

        /// <summary>
        /// Updates the components which the <see cref="MapPlugin"/> knows about so that it reflects
        /// the currently active view.
        /// </summary>
        private void UpdateComponentsForActiveDocumentView()
        {
            var mapView = Gui.ViewHost.ActiveDocumentView as IMapView;
            if (mapView != null)
            {
                mapRibbon.Map = mapView.Map;
                mapLegendController.Update(mapView.Map.Data);
            }
            else
            {
                mapRibbon.Map = null;
                mapLegendController.Update(null);
            }
        }
    }
}
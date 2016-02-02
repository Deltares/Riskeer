// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System.Collections.Generic;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Components.DotSpatial.Data;
using Core.Plugins.DotSpatial.Forms;

namespace Core.Plugins.DotSpatial
{
    /// <summary>
    /// The gui plugin for the <see cref="DotSpatial"/> map component.
    /// </summary>
    public class DotSpatialGuiPlugin : GuiPlugin
    {
        private MapRibbon mapRibbon;
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
            mapRibbon = CreateMapRibbon();
            Gui.ActiveViewChanged += GuiOnActiveViewChanged;
            activated = true;
        }

        public override void Dispose()
        {
            if (activated)
            {
                Gui.ActiveViewChanged -= GuiOnActiveViewChanged;
            }
            base.Dispose();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<MapData, MapDataView>
            {
                Image = Resources.DocumentHS,
                GetViewName = (v, o) => Resources.DotSpatialGuiPlugin_GetViewInfoObjects_Map
            };
        }

        private MapRibbon CreateMapRibbon()
        {
            return new MapRibbon();
        }

        private void GuiOnActiveViewChanged(object sender, ActiveViewChangeEventArgs activeViewChangeEventArgs)
        {
            UpdateComponentsForActiveView();
        }

        /// <summary>
        /// Updates the components which the <see cref="OxyPlotGuiPlugin"/> knows about so that it reflects
        /// the currently active view.
        /// </summary>
        private void UpdateComponentsForActiveView()
        {
            var mapView = Gui.ActiveView as IMapView;
            if (mapView != null)
            {
                mapRibbon.Map = mapView.Map;
            }
            else
            {
                mapRibbon.Map = null;
            }
        }
    }
}
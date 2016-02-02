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

        private void GuiOnActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            if (activated)
            {
                Gui.ActiveViewChanged -= GuiOnActiveViewChanged;
            }
            base.Dispose();
        }

        private MapRibbon CreateMapRibbon()
        {
            return new MapRibbon();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<MapData, MapDataView>
            {
                Image = Resources.DocumentHS,
                GetViewName = (v, o) => Resources.DotSpatialGuiPlugin_GetViewInfoObjects_Map
            };
        }
    }
}
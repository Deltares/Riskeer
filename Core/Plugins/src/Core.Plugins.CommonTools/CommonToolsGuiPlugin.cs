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
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Properties;
using Core.Plugins.CommonTools.Property;
using PropertyInfo = Core.Common.Gui.Plugin.PropertyInfo;

namespace Core.Plugins.CommonTools
{
    public class CommonToolsGuiPlugin : GuiPlugin
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<WebLink, UrlProperties>();
            yield return new PropertyInfo<Project, ProjectProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<RichTextFile, RichTextView>
            {
                Image = Common.Gui.Properties.Resources.key,
                GetViewName = (v, o) => o != null ? o.Name : ""
            };
            yield return new ViewInfo<WebLink, HtmlPageView>
            {
                Image = Resources.home,
                Description = Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Browser,
                GetViewName = (v, o) => o != null ? o.Name : ""
            };
        }
    }
}
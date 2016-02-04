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

using System;

using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Plugins.CommonTools.Properties;

namespace Core.Plugins.CommonTools.Property
{
    /// <summary>
    /// This class describes the presentation of properties of a <see cref="WebLink"/>.
    /// </summary>
    public class WebLinkProperties : ObjectProperties<WebLink>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "UrlProperties_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "UrlProperties_Path_DisplayName")]
        [ResourcesDescription(typeof(Resources), "UrlProperties_Path_Description")]
        public string Path
        {
            get
            {
                return data.Path.ToString();
            }
            set
            {
                data.Path = new Uri(value);
            }
        }
    }
}
// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryDatabase"/> for properties panel.
    /// </summary>
    public class HydraulicBoundaryDatabaseProperties : ObjectProperties<HydraulicBoundaryDatabaseContext>
    {
        [ResourcesCategory(typeof(Common.Data.Properties.Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_FilePath_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_FilePath_Description")]
        public string FilePath
        {
            get
            {
                return data.Parent.HydraulicBoundaryDatabase != null ? data.Parent.HydraulicBoundaryDatabase.FilePath : string.Empty;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Common.Data.Properties.Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_Locations_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Locations_Description")]
        public HydraulicBoundaryLocationProperties[] Locations
        {
            get
            {
                return data.Parent.HydraulicBoundaryDatabase != null
                           ? data.Parent.HydraulicBoundaryDatabase.Locations.Select(loc => new HydraulicBoundaryLocationProperties(loc)).ToArray()
                           : new HydraulicBoundaryLocationProperties[0];
            }
        }
    }
}
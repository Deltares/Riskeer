﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Properties;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocation"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class HydraulicBoundaryLocationProperties : ObjectProperties<HydraulicBoundaryLocationContext>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Properties.Resources), "HydraulicBoundaryDatabase_Locations_Id_DisplayName")]
        [ResourcesDescription(typeof(Properties.Resources), "HydraulicBoundaryDatabase_Locations_Id_Description")]
        public virtual long Id
        {
            get
            {
                return data.HydraulicBoundaryLocation.Id;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Properties.Resources), "HydraulicBoundaryDatabase_Locations_Name_DisplayName")]
        [ResourcesDescription(typeof(Properties.Resources), "HydraulicBoundaryDatabase_Locations_Name_Description")]
        public virtual string Name
        {
            get
            {
                return data.HydraulicBoundaryLocation.Name;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Properties.Resources), "HydraulicBoundaryDatabase_Locations_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(Properties.Resources), "HydraulicBoundaryDatabase_Locations_Coordinates_Description")]
        public virtual Point2D Location
        {
            get
            {
                return data.HydraulicBoundaryLocation.Location;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Location);
        }
    }
}
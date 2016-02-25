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

using System;
using System.ComponentModel;
using System.Globalization;

using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;

using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocation"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HydraulicBoundaryLocationProperties : ObjectProperties<HydraulicBoundaryLocation>
    {
        /// <summary>
        /// New instance of <see cref="HydraulicBoundaryLocationProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation"><see cref="HydraulicBoundaryLocation"/> whose data will be used for the properties panel.</param>
        public HydraulicBoundaryLocationProperties(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            data = hydraulicBoundaryLocation;
        }

        /// <summary>
        /// Gets the Id from the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_Locations_Id_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Locations_Id_Description")]
        public long Id
        {
            get
            {
                return data.Id;
            }
        }

        /// <summary>
        /// Gets the Name from the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        [PropertyOrder(2)]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_Locations_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Locations_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        /// <summary>
        /// Gets the Location from the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        [PropertyOrder(3)]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_Locations_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Locations_Coordinates_Description")]
        public Point2D Location
        {
            get
            {
                return data.Location;
            }
        }

        /// <summary>
        /// Gets the DesignWaterLevel from the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        [PropertyOrder(4)]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_Locations_DesignWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Locations_DesignWaterLevel_Description")]
        public string DesignWaterLevel
        {
            get
            {
                return double.IsNaN(data.DesignWaterLevel) ? "" : data.DesignWaterLevel.ToString("G2", CultureInfo.InvariantCulture);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", Name, Location);
        }
    }
}
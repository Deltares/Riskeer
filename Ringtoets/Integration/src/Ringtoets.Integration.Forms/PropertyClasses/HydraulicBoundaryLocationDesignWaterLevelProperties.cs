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

using System.Globalization;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Utils.Attributes;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocation"/> with <see cref="DesignWaterLevel"/> for properties panel.
    /// </summary>
    public class HydraulicBoundaryLocationDesignWaterLevelProperties : HydraulicBoundaryLocationProperties
    {
        [PropertyOrder(1)]
        public override long Id
        {
            get
            {
                return base.Id;
            }
        }

        [PropertyOrder(2)]
        public override string Name
        {
            get
            {
                return base.Name;
            }
        }

        [PropertyOrder(3)]
        public override Point2D Location
        {
            get
            {
                return base.Location;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>.
        /// </summary>
        [PropertyOrder(4)]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryDatabase_Locations_DesignWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Locations_DesignWaterLevel_Description")]
        public string DesignWaterLevel
        {
            get
            {
                return double.IsNaN(data.DesignWaterLevel) ? string.Empty : data.DesignWaterLevel.ToString("F2", CultureInfo.InvariantCulture);
            }
        }
    }
}
// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of an enumeration of <see cref="HydraulicBoundaryLocation"/> with 
    /// <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/> for properties panel.
    /// </summary>
    public class DesignWaterLevelLocationsProperties : ObjectProperties<ObservableList<HydraulicBoundaryLocation>>
    {
        private readonly RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation> hydraulicBoundaryLocationObserver;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelLocationProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The list of hydraulic boundary locations to set as data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/> is <c>null</c>.</exception>
        public DesignWaterLevelLocationsProperties(ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            hydraulicBoundaryLocationObserver = new RecursiveObserver<ObservableList<HydraulicBoundaryLocation>, HydraulicBoundaryLocation>(OnRefreshRequired, list => list);

            Data = hydraulicBoundaryLocations;
        }

        public override object Data
        {
            get
            {
                return base.Data;
            }
            set
            {
                base.Data = value;

                hydraulicBoundaryLocationObserver.Observable = value as ObservableList<HydraulicBoundaryLocation>;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Locations_Description))]
        public DesignWaterLevelLocationProperties[] Locations
        {
            get
            {
                return data.Select(loc => new DesignWaterLevelLocationProperties(loc)).ToArray();
            }
        }

        public override void Dispose()
        {
            hydraulicBoundaryLocationObserver.Dispose();

            base.Dispose();
        }
    }
}
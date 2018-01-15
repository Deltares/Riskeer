﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of an enumeration of <see cref="HydraulicBoundaryLocation"/> with 
    /// <see cref="HydraulicBoundaryLocation.WaveHeight"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveHeightLocationsProperties : HydraulicBoundaryLocationsProperties
    {
        private readonly Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> getCalculationFunc;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveHeightLocationsProperties"/>.
        /// </summary>
        /// <param name="locations">The locations to show the properties for.</param>
        /// <param name="getCalculationFunc"><see cref="Func{T,TResult}"/> for obtaining a <see cref="HydraulicBoundaryLocationCalculation"/>
        /// based on <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveHeightLocationsProperties(ObservableList<HydraulicBoundaryLocation> locations,
                                                                      Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> getCalculationFunc)
            : base(locations)
        {
            if (getCalculationFunc == null)
            {
                throw new ArgumentNullException(nameof(getCalculationFunc));
            }

            this.getCalculationFunc = getCalculationFunc;
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Locations_Description))]
        public GrassCoverErosionOutwardsWaveHeightLocationProperties[] Locations
        {
            get
            {
                return data.Select(loc => new GrassCoverErosionOutwardsWaveHeightLocationProperties(loc, getCalculationFunc(loc))).ToArray();
            }
        }
    }
}
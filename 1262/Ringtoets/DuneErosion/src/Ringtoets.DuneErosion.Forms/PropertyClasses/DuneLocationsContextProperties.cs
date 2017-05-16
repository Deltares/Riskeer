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
using Core.Common.Utils.Attributes;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the <see cref="DuneLocationsContext"/> for the properties panel.
    /// </summary>
    public class DuneLocationsContextProperties : ObjectProperties<ObservableList<DuneLocation>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationsContextProperties"/>.
        /// </summary>
        /// <param name="locations">The locations to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> is <c>null</c>.</exception>
        public DuneLocationsContextProperties(ObservableList<DuneLocation> locations)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }
            Data = locations;
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Locations_Description))]
        public DuneLocationContextProperties[] Locations
        {
            get
            {
                return data.Select(loc => new DuneLocationContextProperties
                {
                    Data = new DuneLocationContext(data, loc)
                }).ToArray();
            }
        }
    }
}
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
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.DuneErosion.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of an enumeration of <see cref="DuneLocation"/> for the properties panel.
    /// </summary>
    public class DuneLocationsProperties : ObjectProperties<ObservableList<DuneLocationCalculation>>, IDisposable
    {
        private readonly RecursiveObserver<ObservableList<DuneLocationCalculation>, DuneLocationCalculation> calculationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationsProperties"/>.
        /// </summary>
        /// <param name="calculations">The collection of dune location calculations to set as data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        public DuneLocationsProperties(ObservableList<DuneLocationCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            calculationsObserver = new RecursiveObserver<ObservableList<DuneLocationCalculation>, DuneLocationCalculation>(OnRefreshRequired, list => list)
            {
                Observable = calculations
            };

            Data = calculations;
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Locations_Description))]
        public DuneLocationProperties[] Locations
        {
            get
            {
                return data.Select(calculation => new DuneLocationProperties(calculation.DuneLocation, calculation)).ToArray();
            }
        }

        public void Dispose()
        {
            calculationsObserver.Dispose();
        }
    }
}
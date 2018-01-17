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
using Ringtoets.DuneErosion.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of an enumeration of <see cref="DuneLocation"/> for the properties panel.
    /// </summary>
    public class DuneLocationsProperties : ObjectProperties<ObservableList<DuneLocation>>
    {
        private readonly Func<DuneLocation, DuneLocationCalculation> getCalculationFunc;
        private readonly RecursiveObserver<ObservableList<DuneLocation>, DuneLocation> locationObserver;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationsProperties"/>.
        /// </summary>
        /// <param name="locations">The list of dune locations to set as data.</param>
        /// <param name="getCalculationFunc"><see cref="Func{T,TResult}"/> for obtaining a <see cref="DuneLocationCalculation"/>
        /// based on <see cref="DuneLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public DuneLocationsProperties(ObservableList<DuneLocation> locations,
                                       Func<DuneLocation, DuneLocationCalculation> getCalculationFunc)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            if (getCalculationFunc == null)
            {
                throw new ArgumentNullException(nameof(getCalculationFunc));
            }

            locationObserver = new RecursiveObserver<ObservableList<DuneLocation>, DuneLocation>(OnRefreshRequired, list => list);

            this.getCalculationFunc = getCalculationFunc;

            Data = locations;
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

                locationObserver.Observable = value as ObservableList<DuneLocation>;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Locations_Description))]
        public DuneLocationProperties[] Locations
        {
            get
            {
                return data.Select(loc => new DuneLocationProperties(loc, getCalculationFunc(loc))).ToArray();
            }
        }

        public override void Dispose()
        {
            locationObserver.Dispose();

            base.Dispose();
        }
    }
}
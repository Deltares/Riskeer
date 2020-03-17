// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.DuneErosion.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="DuneLocationCalculation"/> for the properties panel.
    /// </summary>
    public class DuneLocationCalculationsProperties : ObjectProperties<IObservableEnumerable<DuneLocationCalculation>>, IDisposable
    {
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsProperties"/>.
        /// </summary>
        /// <param name="calculations">The collection of dune location calculations to set as data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        public DuneLocationCalculationsProperties(IObservableEnumerable<DuneLocationCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            calculationsObserver = new RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>(OnRefreshRequired, list => list)
            {
                Observable = calculations
            };

            Data = calculations;
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Locations_Description))]
        public DuneLocationCalculationProperties[] Calculations
        {
            get
            {
                return GetDuneLocationCalculationProperties();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                calculationsObserver.Dispose();
            }
        }

        private DuneLocationCalculationProperties[] GetDuneLocationCalculationProperties()
        {
            return data.Select(calculation => new DuneLocationCalculationProperties(calculation)).ToArray();
        }
    }
}
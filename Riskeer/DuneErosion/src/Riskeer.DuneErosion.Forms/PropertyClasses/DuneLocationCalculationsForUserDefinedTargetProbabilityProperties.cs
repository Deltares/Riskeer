// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.DuneErosion.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a <see cref="DuneLocationCalculationsForTargetProbability"/> for the properties panel.
    /// </summary>
    public class DuneLocationCalculationsForUserDefinedTargetProbabilityProperties : ObjectProperties<DuneLocationCalculationsForTargetProbability>, IDisposable
    {
        private const int targetProbabilityPropertyIndex = 1;
        private const int calculationsPropertyIndex = 2;

        private readonly IObservablePropertyChangeHandler targetProbabilityChangeHandler;
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> calculationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsForUserDefinedTargetProbabilityProperties"/>.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The <see cref="DuneLocationCalculationsForTargetProbability"/> to show the properties for.</param>
        /// <param name="targetProbabilityChangeHandler">The <see cref="IObservablePropertyChangeHandler"/> for when the target probability changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneLocationCalculationsForUserDefinedTargetProbabilityProperties(DuneLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                                                                 IObservablePropertyChangeHandler targetProbabilityChangeHandler)
        {
            if (calculationsForTargetProbability == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbability));
            }

            if (targetProbabilityChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(targetProbabilityChangeHandler));
            }

            Data = calculationsForTargetProbability;

            this.targetProbabilityChangeHandler = targetProbabilityChangeHandler;

            calculationsObserver = new RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>(OnRefreshRequired, list => list)
            {
                Observable = calculationsForTargetProbability.DuneLocationCalculations
            };
        }

        [PropertyOrder(calculationsPropertyIndex)]
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

        [PropertyOrder(targetProbabilityPropertyIndex)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TargetProbability_Description))]
        public double TargetProbability
        {
            get
            {
                return data.TargetProbability;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.TargetProbability = value, targetProbabilityChangeHandler);
            }
        }

        public void Dispose()
        {
            calculationsObserver.Dispose();

            GC.SuppressFinalize(this);
        }

        private DuneLocationCalculationProperties[] GetDuneLocationCalculationProperties()
        {
            return data.DuneLocationCalculations.Select(calculation => new DuneLocationCalculationProperties(calculation)).ToArray();
        }
    }
}
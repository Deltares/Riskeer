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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="HydraulicBoundaryLocationCalculation"/> with a user specified
    /// target probability based wave height calculation result for properties panel.
    /// </summary>
    public class WaveHeightCalculationsForUserDefinedTargetProbabilityProperties : WaveHeightCalculationsProperties
    {
        private const int targetProbabilityPropertyIndex = 1;
        private const int calculationsPropertyIndex = 2;

        private readonly HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability;
        private readonly IObservablePropertyChangeHandler targetProbabilityChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightCalculationsForUserDefinedTargetProbabilityProperties"/>.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/> to show the properties for.</param>
        /// <param name="targetProbabilityChangeHandler">The <see cref="IObservablePropertyChangeHandler"/> for when the target probability changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public WaveHeightCalculationsForUserDefinedTargetProbabilityProperties(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                                                               IObservablePropertyChangeHandler targetProbabilityChangeHandler)
            : base(calculationsForTargetProbability?.HydraulicBoundaryLocationCalculations ?? throw new ArgumentNullException(nameof(calculationsForTargetProbability)))
        {
            if (targetProbabilityChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(targetProbabilityChangeHandler));
            }

            this.calculationsForTargetProbability = calculationsForTargetProbability;
            this.targetProbabilityChangeHandler = targetProbabilityChangeHandler;
        }

        [PropertyOrder(calculationsPropertyIndex)]
        public override WaveHeightCalculationProperties[] Calculations
        {
            get
            {
                return base.Calculations;
            }
        }

        [PropertyOrder(targetProbabilityPropertyIndex)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TargetProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TargetProbability_WaveHeights_Description))]
        public double TargetProbability
        {
            get
            {
                return calculationsForTargetProbability.TargetProbability;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => calculationsForTargetProbability.TargetProbability = value, targetProbabilityChangeHandler);
            }
        }
    }
}
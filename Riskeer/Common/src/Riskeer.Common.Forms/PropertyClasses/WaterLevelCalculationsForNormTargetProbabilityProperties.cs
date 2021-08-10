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
using Core.Common.Base;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="HydraulicBoundaryLocationCalculation"/> with a norm
    /// target probability based design water level calculation result for properties panel.
    /// </summary>
    public class WaterLevelCalculationsForNormTargetProbabilityProperties : DesignWaterLevelCalculationsProperties
    {
        private const int targetProbabilityPropertyIndex = 1;
        private const int calculationsPropertyIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="WaterLevelCalculationsForNormTargetProbabilityProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculations">The collection of hydraulic boundary location calculations to set as data.</param>
        /// <param name="normTargetProbability">The norm target probability.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculations"/> is <c>null</c>.</exception>
        public WaterLevelCalculationsForNormTargetProbabilityProperties(IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations,
                                                                        double normTargetProbability)
            : base(hydraulicBoundaryLocationCalculations)
        {
            this.TargetProbability = normTargetProbability;
        }

        [PropertyOrder(calculationsPropertyIndex)]
        public override DesignWaterLevelCalculationProperties[] Calculations
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
        [ResourcesDescription(typeof(Resources), nameof(Resources.TargetProbability_WaterLevels_Description))]
        public double TargetProbability { get; }
    }
}
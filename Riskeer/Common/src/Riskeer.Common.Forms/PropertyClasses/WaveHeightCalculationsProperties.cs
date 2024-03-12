﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Util.Attributes;
using Core.Gui.Converters;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="HydraulicBoundaryLocationCalculation"/> with
    /// a wave height calculation result for properties panel.
    /// </summary>
    public abstract class WaveHeightCalculationsProperties : HydraulicBoundaryLocationCalculationsProperties
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightCalculationsProperties"/>.
        /// </summary>
        protected WaveHeightCalculationsProperties(IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations, 
                                                   IAssessmentSection assessmentSection)
            : base(hydraulicBoundaryLocationCalculations, assessmentSection) {}

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Locations_Description))]
        public virtual WaveHeightCalculationProperties[] Calculations
        {
            get
            {
                return GetWaveHeightCalculationProperties();
            }
        }

        private WaveHeightCalculationProperties[] GetWaveHeightCalculationProperties()
        {
            return data.Select(calculation => new WaveHeightCalculationProperties(calculation, AssessmentSection)).ToArray();
        }
    }
}
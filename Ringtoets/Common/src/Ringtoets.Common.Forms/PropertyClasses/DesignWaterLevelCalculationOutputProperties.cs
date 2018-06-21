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
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocationCalculation"/> with design water level for properties panel
    /// showing only output.
    /// </summary>
    public class DesignWaterLevelCalculationOutputProperties : HydraulicBoundaryLocationCalculationOutputProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationOutputProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location calculation.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary this calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculation"/> is <c>null</c>.</exception>
        public DesignWaterLevelCalculationOutputProperties(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                           string categoryBoundaryName)
            : base(hydraulicBoundaryLocationCalculation, categoryBoundaryName) {}

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DesignWaterLevelCalculation_Result_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DesignWaterLevelCalculation_Result_Description))]
        public override RoundedDouble Result
        {
            get
            {
                return base.Result;
            }
        }

        [ResourcesDescription(typeof(Resources), nameof(Resources.DesignWaterLevelCalculation_Convergence_Description))]
        public override CalculationConvergence Convergence
        {
            get
            {
                return base.Convergence;
            }
        }
    }
}
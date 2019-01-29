// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a <see cref="HydraulicBoundaryLocationCalculation"/> with design water level results for properties panel
    /// with information about the category boundary type.
    /// </summary>
    public class DesignWaterLevelCalculationCategoryBoundaryProperties : HydraulicBoundaryLocationCalculationCategoryBoundaryProperties
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationCategoryBoundaryProperties"/>.
        /// </summary>
        public DesignWaterLevelCalculationCategoryBoundaryProperties(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                                     string categoryBoundaryName)
            : base(hydraulicBoundaryLocationCalculation, categoryBoundaryName) {}

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaterLevel_DisplayName))]
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
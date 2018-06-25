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
using Core.Common.Gui.Attributes;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocationCalculation"/> for properties panel with information
    /// about the category boundary type.
    /// </summary>
    public abstract class HydraulicBoundaryLocationCalculationCategoryBoundaryProperties : HydraulicBoundaryLocationCalculationBaseProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationCategoryBoundaryProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location calculation.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected HydraulicBoundaryLocationCalculationCategoryBoundaryProperties(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                                                 string categoryBoundaryName)
            : base(hydraulicBoundaryLocationCalculation)
        {
            if (categoryBoundaryName == null)
            {
                throw new ArgumentNullException(nameof(categoryBoundaryName));
            }

            CategoryBoundaryName = categoryBoundaryName;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryLocationCalculationOutputProperties_CategoryBoundaryName_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryLocationCalculationOutputProperties_CategoryBoundaryName_Description))]
        public string CategoryBoundaryName { get; }

        public override string ToString()
        {
            return CategoryBoundaryName;
        }
    }
}
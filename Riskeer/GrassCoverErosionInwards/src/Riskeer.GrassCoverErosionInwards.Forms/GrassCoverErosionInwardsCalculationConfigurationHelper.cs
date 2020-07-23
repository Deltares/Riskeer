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
using System.Collections.Generic;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.Helpers;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms
{
    /// <summary>
    /// Helper class to help views when dealing with <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
    /// </summary>
    public static class GrassCoverErosionInwardsCalculationConfigurationHelper
    {
        /// <summary>
        /// Configures calculations and adds them to the calculation group.
        /// </summary>
        /// <param name="calculationGroup">The calculation group.</param>
        /// <param name="dikeProfiles">The collection of dike profiles.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void GenerateCalculations(CalculationGroup calculationGroup, IEnumerable<DikeProfile> dikeProfiles)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (dikeProfiles == null)
            {
                throw new ArgumentNullException(nameof(dikeProfiles));
            }

            foreach (DikeProfile profile in dikeProfiles)
            {
                var calculation = new GrassCoverErosionInwardsCalculationScenario
                {
                    Name = NamingHelper.GetUniqueName(calculationGroup.Children, profile.Name, c => c.Name),
                    InputParameters =
                    {
                        DikeProfile = profile
                    }
                };
                calculationGroup.Children.Add(calculation);
            }
        }
    }
}
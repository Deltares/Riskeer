// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using log4net;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for design water level.
    /// </summary>
    public class DesignWaterLevelCalculationService : HydraulicBoundaryLocationCalculationService<AssessmentLevelCalculationInput>
    {
        private static IHydraulicBoundaryLocationCalculationService instance;

        /// <summary>
        /// Gets or sets an instance of <see cref="IHydraulicBoundaryLocationCalculationService"/>.
        /// </summary>
        public static IHydraulicBoundaryLocationCalculationService Instance
        {
            get
            {
                return instance ?? (instance = new DesignWaterLevelCalculationService());
            }
            set
            {
                instance = value;
            }
        }

        protected override AssessmentLevelCalculationInput CreateInput(HydraulicBoundaryLocation hydraulicBoundaryLocation, double norm)
        {
            return new AssessmentLevelCalculationInput(1, hydraulicBoundaryLocation.Id, norm);
        }
    }
}
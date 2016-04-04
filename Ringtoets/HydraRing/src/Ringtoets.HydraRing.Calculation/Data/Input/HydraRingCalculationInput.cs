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

using System.Collections.Generic;

namespace Ringtoets.HydraRing.Calculation.Data.Input
{
    /// <summary>
    /// Container of all data necessary for performing a Hydra-Ring calculation.
    /// </summary>
    public abstract class HydraRingCalculationInput
    {
        private readonly int hydraulicBoundaryLocationId;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        protected HydraRingCalculationInput(int hydraulicBoundaryLocationId)
        {
            this.hydraulicBoundaryLocationId = hydraulicBoundaryLocationId;
        }

        /// <summary>
        /// Gets the <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public abstract HydraRingFailureMechanismType FailureMechanismType { get; }

        /// <summary>
        /// Gets the id corresponding to the type of calculation that should be performed.
        /// </summary>
        public abstract int CalculationTypeId { get; }

        /// <summary>
        /// Gets the id of the variable that is relevant during the calculation.
        /// </summary>
        public abstract int VariableId { get; }

        /// <summary>
        /// Gets the id of the hydraulic station to use during the calculation.
        /// </summary>
        public int HydraulicBoundaryLocationId
        {
            get
            {
                return hydraulicBoundaryLocationId;
            }
        }

        /// <summary>
        /// Gets the section to perform the calculation for.
        /// </summary>
        public abstract HydraRingSection Section { get; }

        /// <summary>
        /// Gets the variables to use during the calculation.
        /// </summary>
        public virtual IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the profile points to use during the calculation.
        /// </summary>
        public virtual IEnumerable<HydraRingProfilePoint> ProfilePoints
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the foreland points to use during the calculation.
        /// </summary>
        public virtual IEnumerable<HydraRingForelandPoint> ForelandsPoints
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the break waters to use during the calculation.
        /// </summary>
        public virtual IEnumerable<HydraRingBreakWater> BreakWaters
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the reliability index to use during the calculation.
        /// </summary>
        /// <remarks>Only relevant for type 2 computations.</remarks>
        public virtual double Beta
        {
            get
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Gets the sub mechanism model id corresponding to the provided sub mechanism id.
        /// </summary>
        /// <param name="subMechanismId">The sub mechanim id to get the sub mechanism model id for.</param>
        /// <returns>The corresponding sub mechanism model id or <c>null</c> otherwise.</returns>
        public virtual int? GetSubMechanismModelId(int subMechanismId)
        {
            return null;
        }
    }
}
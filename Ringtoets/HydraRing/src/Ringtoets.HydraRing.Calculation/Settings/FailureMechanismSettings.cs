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

namespace Ringtoets.HydraRing.Calculation.Settings
{
    /// <summary>
    /// Container for failure mechanims settings.
    /// </summary>
    public class FailureMechanismSettings
    {
        private readonly double valueMin;
        private readonly double valueMax;
        private readonly double faultTreeModelId;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismSettings"/> class.
        /// </summary>
        /// <param name="valueMin">The minimum value to use while iterating to a target probability.</param>
        /// <param name="valueMax">The maximum value to use while iterating to a target probability.</param>
        /// <param name="faultTreeModelId">The fault tree model id.</param>
        public FailureMechanismSettings(double valueMin, double valueMax, double faultTreeModelId)
        {
            this.valueMin = valueMin;
            this.valueMax = valueMax;
            this.faultTreeModelId = faultTreeModelId;
        }

        /// <summary>
        /// Gets the minimum value to use while iterating to a target probability.
        /// </summary>
        /// <remarks>This property is only applicable in case of type-2 computations.</remarks>
        public double ValueMin
        {
            get
            {
                return valueMin;
            }
        }

        /// <summary>
        /// Gets the maximum value to use while iterating to a target probability.
        /// </summary>
        /// <remarks>This property is only applicable in case of type-2 computations.</remarks>
        public double ValueMax
        {
            get
            {
                return valueMax;
            }
        }

        /// <summary>
        /// Gets the fault tree model id.
        /// </summary>
        public double FaultTreeModelId
        {
            get
            {
                return faultTreeModelId;
            }
        }
    }
}
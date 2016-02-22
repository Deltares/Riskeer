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

namespace Ringtoets.HydraRing.Calculation
{
    /// <summary>
    /// Container for configuration settings on a per <see cref="HydraRingFailureMechanismType"/> basis.
    /// </summary>
    public class HydraRingConfigurationSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public HydraRingFailureMechanismType HydraRingFailureMechanismType { get; set; }

        /// <summary>
        /// Gets or sets the id of the calculation type that should be used.
        /// </summary>
        public int CalculationTypeId { get; set; }

        /// <summary>
        /// Gets or sets the id of the variable that is considered.
        /// </summary>
        public int VariableId { get; set; }

        /// <summary>
        /// Gets or sets the id of the sub mechanism.
        /// </summary>
        public int SubMechanismId { get; set; }

        /// <summary>
        /// Gets or sets the id of the calculation technique that should be used.
        /// </summary>
        public int CalculationTechniqueId { get; set; }

        /// <summary>
        /// Gets or set the FORM start method.
        /// </summary>
        public int FormStartMethod { get; set; }
    }
}

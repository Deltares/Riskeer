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

using System.Collections.Generic;

namespace Ringtoets.HydraRing.Calculation.Data.Defaults
{
    /// <summary>
    /// Container for failure mechanism defaults.
    /// </summary>
    public class FailureMechanismDefaults
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismDefaults"/> class.
        /// </summary>
        /// <param name="mechanismId">The mechanism id.</param>
        /// <param name="subMechanismIds">The sub mechanism ids that are applicable.</param>
        /// <param name="faultTreeModelId">The fault tree model id.</param>
        /// <param name="preprocessorFaultTreeModelId">The fault tree model id to be used when running the preprocessor.</param>
        /// <param name="preprocessorMechanismId">The mechanism id to be used when running the preprocessor.</param>
        public FailureMechanismDefaults(int mechanismId, IEnumerable<int> subMechanismIds, int faultTreeModelId,
                                        int preprocessorFaultTreeModelId, int preprocessorMechanismId)
        {
            MechanismId = mechanismId;
            SubMechanismIds = subMechanismIds;
            FaultTreeModelId = faultTreeModelId;
            PreprocessorFaultTreeModelId = preprocessorFaultTreeModelId;
            PreprocessorMechanismId = preprocessorMechanismId;
        }

        /// <summary>
        /// Gets the mechanism id.
        /// </summary>
        public int MechanismId { get; }

        /// <summary>
        /// Gets the sub mechanism ids that are applicable.
        /// </summary>
        public IEnumerable<int> SubMechanismIds { get; }

        /// <summary>
        /// Gets the fault tree model id.
        /// </summary>
        public int FaultTreeModelId { get; }

        /// <summary>
        /// Gets the fault tree model id to be used when running the preprocessor.
        /// </summary>
        public int PreprocessorFaultTreeModelId { get; }

        /// <summary>
        /// Gets the mechanism id to be used when running the preprocessor.
        /// </summary>
        public int PreprocessorMechanismId { get; }

        /// <summary>
        /// Gets the sub mechanism id to be used when running the preprocessor.
        /// </summary>
        public int PreprocessorSubMechanismId
        {
            get
            {
                return 7;
            }
        }
    }
}
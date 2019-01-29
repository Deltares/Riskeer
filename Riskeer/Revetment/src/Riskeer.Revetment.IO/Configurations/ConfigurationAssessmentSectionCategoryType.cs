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

namespace Riskeer.Revetment.IO.Configurations
{
    /// <summary> 
    /// Enum defining the possible assessment section category type value in a read wave conditions calculation. 
    /// </summary> 
    public enum ConfigurationAssessmentSectionCategoryType
    {
        /// <summary> 
        /// The factorized signaling norm. 
        /// </summary> 
        FactorizedSignalingNorm = 1,

        /// <summary> 
        /// The signaling norm. 
        /// </summary> 
        SignalingNorm = 2,

        /// <summary> 
        /// The lower limit norm. 
        /// </summary> 
        LowerLimitNorm = 3,

        /// <summary> 
        /// The factorized lower limit norm. 
        /// </summary> 
        FactorizedLowerLimitNorm = 4
    }
}
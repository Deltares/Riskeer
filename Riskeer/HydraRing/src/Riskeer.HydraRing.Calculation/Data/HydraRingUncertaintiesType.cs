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

namespace Riskeer.HydraRing.Calculation.Data
{
    /// <summary>
    /// Enumeration that defines the uncertainties types supported by Hydra-Ring.
    /// </summary>
    /// <remarks>
    /// The integer values correspond to uncertainties ids defined within Hydra-Ring.
    /// </remarks>
    public enum HydraRingUncertaintiesType
    {
        /// <summary>
        /// No uncertainties.
        /// </summary>
        None = 0,

        /// <summary>
        /// All uncertainties.
        /// </summary>
        All = 1,

        /// <summary>
        /// Only model uncertainties.
        /// </summary>
        Model = 2,

        /// <summary>
        /// Only statistical uncertainties.
        /// </summary>
        Statistic = 3
    }
}
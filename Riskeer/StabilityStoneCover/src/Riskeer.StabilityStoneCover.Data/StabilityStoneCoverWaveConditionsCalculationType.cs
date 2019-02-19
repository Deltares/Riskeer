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

using Core.Common.Util.Attributes;
using Riskeer.StabilityStoneCover.Data.Properties;

namespace Riskeer.StabilityStoneCover.Data
{
    /// <summary>
    /// Defines the various types of stability stone cover wave conditions calculations.
    /// </summary>
    public enum StabilityStoneCoverWaveConditionsCalculationType
    {
        /// <summary>
        /// Calculate the blocks.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsCalculationType_Blocks_DisplayName))]
        Blocks = 1,

        /// <summary>
        /// Calculate the columns.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsCalculationType_Columns_DisplayName))]
        Columns = 2,

        /// <summary>
        /// Calculate both the blocks and columns.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsCalculationType_Both_DisplayName))]
        Both = 3
    }
}
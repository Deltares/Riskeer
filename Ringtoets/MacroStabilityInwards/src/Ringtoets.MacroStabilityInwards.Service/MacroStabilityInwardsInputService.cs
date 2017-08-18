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

using System.Collections.Generic;
using System.Linq;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Service
{
    /// <summary>
    /// Class responsible for macro stability inwards input properties in sync.
    /// </summary>
    public static class MacroStabilityInwardsInputService
    {
        /// <summary>
        /// Sets <see cref="MacroStabilityInwardsInput.StochasticSoilModel"/> and <see cref="MacroStabilityInwardsInput.StochasticSoilProfile"/> that match the input of a calculation if there is one matching 
        /// <see cref="StochasticSoilModel"/> or <see cref="StochasticSoilProfile"/> respectively.
        /// </summary>
        /// <param name="macroStabilityInwardsInput">The input parameters to set the <see cref="StochasticSoilModel"/>.</param>
        /// <param name="availableStochasticSoilModels">The available stochastic soil models.</param>
        public static void SetMatchingStochasticSoilModel(MacroStabilityInwardsInput macroStabilityInwardsInput, IEnumerable<StochasticSoilModel> availableStochasticSoilModels)
        {
            List<StochasticSoilModel> available = availableStochasticSoilModels.ToList();
            if (available.Count == 1)
            {
                macroStabilityInwardsInput.StochasticSoilModel = available.First();
            }
            else if (!available.Any() || !available.Contains(macroStabilityInwardsInput.StochasticSoilModel))
            {
                macroStabilityInwardsInput.StochasticSoilModel = null;
            }
            SyncStochasticSoilProfileWithStochasticSoilModel(macroStabilityInwardsInput);
        }

        /// <summary>
        /// Sets the <paramref name="macroStabilityInwardsInput.StochasticSoilProfile"/> to the corresponding <paramref name="macroStabilityInwardsInput.StochasticSoilModel"/>:
        /// <list type="bullet">
        /// <item><c>null</c> if no <paramref name="macroStabilityInwardsInput.StochasticSoilModel"/> is set.</item>
        /// <item>The first element of <paramref name="macroStabilityInwardsInput.StochasticSoilModel.StochasticSoilProfiles"/> when it is the only element.</item>
        /// </list>
        /// </summary>
        /// <param name="macroStabilityInwardsInput">The input parameters to set the <see cref="StochasticSoilProfile"/>.</param>
        public static void SyncStochasticSoilProfileWithStochasticSoilModel(MacroStabilityInwardsInput macroStabilityInwardsInput)
        {
            if (macroStabilityInwardsInput.StochasticSoilModel != null)
            {
                if (macroStabilityInwardsInput.StochasticSoilModel.StochasticSoilProfiles.Count == 1)
                {
                    macroStabilityInwardsInput.StochasticSoilProfile = macroStabilityInwardsInput.StochasticSoilModel.StochasticSoilProfiles.First();
                    return;
                }
                if (macroStabilityInwardsInput.StochasticSoilProfile == null || macroStabilityInwardsInput.StochasticSoilModel.StochasticSoilProfiles.Contains(macroStabilityInwardsInput.StochasticSoilProfile))
                {
                    return;
                }
            }
            macroStabilityInwardsInput.StochasticSoilProfile = null;
        }
    }
}
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
using System.Linq;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Service
{
    /// <summary>
    /// Class responsible for macro stability inwards input properties in sync.
    /// </summary>
    public static class MacroStabilityInwardsInputService
    {
        /// <summary>
        /// Sets <see cref="MacroStabilityInwardsInput.StochasticSoilModel"/> and <see cref="MacroStabilityInwardsInput.StochasticSoilProfile"/> that match the input of a calculation if there is one matching 
        /// <see cref="MacroStabilityInwardsStochasticSoilModel"/> or <see cref="MacroStabilityInwardsStochasticSoilProfile"/> respectively.
        /// </summary>
        /// <param name="macroStabilityInwardsInput">The input parameters to set the <see cref="MacroStabilityInwardsStochasticSoilModel"/>.</param>
        /// <param name="availableStochasticSoilModels">The available stochastic soil models.</param>
        public static void SetMatchingStochasticSoilModel(MacroStabilityInwardsInput macroStabilityInwardsInput, IEnumerable<MacroStabilityInwardsStochasticSoilModel> availableStochasticSoilModels)
        {
            List<MacroStabilityInwardsStochasticSoilModel> available = availableStochasticSoilModels.ToList();
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
        /// Sets the <see cref="MacroStabilityInwardsStochasticSoilProfile"/> to the corresponding <see cref="MacroStabilityInwardsStochasticSoilModel"/>:
        /// <list type="bullet">
        /// <item><c>null</c> if no <see cref="MacroStabilityInwardsStochasticSoilModel"/> is set.</item>
        /// <item>The first element of <see cref="MacroStabilityInwardsStochasticSoilModel.StochasticSoilProfiles"/> when it is the only element.</item>
        /// </list>
        /// </summary>
        /// <param name="macroStabilityInwardsInput">The input parameters to set the <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</param>
        public static void SyncStochasticSoilProfileWithStochasticSoilModel(MacroStabilityInwardsInput macroStabilityInwardsInput)
        {
            if (macroStabilityInwardsInput.StochasticSoilModel != null)
            {
                if (macroStabilityInwardsInput.StochasticSoilModel.StochasticSoilProfiles.Count() == 1)
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
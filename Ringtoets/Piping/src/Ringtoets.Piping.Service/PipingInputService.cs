﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// Class responsible for piping input properties in sync.
    /// </summary>
    public static class PipingInputService
    {
        /// <summary>
        /// Sets <see cref="PipingInput.StochasticSoilModel"/> and <see cref="PipingInput.StochasticSoilProfile"/> that match the input of a calculation if there is one matching 
        /// <see cref="StochasticSoilModel"/> or <see cref="StochasticSoilProfile"/> respectively.
        /// </summary>
        /// <param name="pipingInput">The input parameters to set the <see cref="StochasticSoilModel"/>.</param>
        /// <param name="availableStochasticSoilModels">The available stochastic soil models.</param>
        public static void SetMatchingStochasticSoilModel(PipingInput pipingInput, IEnumerable<StochasticSoilModel> availableStochasticSoilModels)
        {
            var available = availableStochasticSoilModels.ToList();
            switch (available.Count)
            {
                case 0:
                    pipingInput.StochasticSoilModel = null;
                    break;
                case 1:
                    pipingInput.StochasticSoilModel = available.First();
                    break;
            }
            SyncStochasticSoilProfileWithStochasticSoilModel(pipingInput);
        }

        /// <summary>
        /// Sets the <paramref name="pipingInput.StochasticSoilProfile"/> to the corresponding <paramref name="pipingInput.StochasticSoilModel"/>:<list type="bullet">
        /// <item><c>null</c> if no <paramref name="pipingInput.StochasticSoilModel"/> is set.</item>
        /// <item>The first element of <paramref name="pipingInput.StochasticSoilModel.StochasticSoilProfiles"/> when it is the only element.</item>
        /// </list>
        /// </summary>
        /// <param name="pipingInput">The input parameters to set the <see cref="StochasticSoilProfile"/>.</param>
        public static void SyncStochasticSoilProfileWithStochasticSoilModel(PipingInput pipingInput)
        {
            if (pipingInput.StochasticSoilModel != null)
            {
                if (pipingInput.StochasticSoilModel.StochasticSoilProfiles.Count == 1)
                {
                    pipingInput.StochasticSoilProfile = pipingInput.StochasticSoilModel.StochasticSoilProfiles.First();
                    return;
                }
            }
            pipingInput.StochasticSoilProfile = null;
        }
    }
}
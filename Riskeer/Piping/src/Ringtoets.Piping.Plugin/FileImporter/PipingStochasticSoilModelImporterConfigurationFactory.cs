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

using System;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.IO.SoilProfiles;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Factory for creating valid configurations for the <see cref="StochasticSoilModelImporter{T}"/>.
    /// </summary>
    public static class PipingStochasticSoilModelImporterConfigurationFactory
    {
        /// <summary>
        /// Creates a configuration to replace the current stochastic soil models on <see cref="failureMechanism"/> 
        /// with imported stochastic soil models.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to replace the stochastic soil models for.</param>
        /// <returns>The configuration for the replace operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static StochasticSoilModelImporterConfiguration<PipingStochasticSoilModel> CreateReplaceStrategyConfiguration(
            PipingFailureMechanism failureMechanism)
        {
            return new StochasticSoilModelImporterConfiguration<PipingStochasticSoilModel>(
                new PipingStochasticSoilModelTransformer(),
                new PipingStochasticSoilModelFilter(),
                new PipingStochasticSoilModelReplaceDataStrategy(failureMechanism));
        }

        /// <summary>
        /// Creates a configuration to update the current stochastic soil models on <see cref="failureMechanism"/> 
        /// with imported stochastic soil models.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to update the stochastic soil models for.</param>
        /// <returns>The configuration for the update operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static StochasticSoilModelImporterConfiguration<PipingStochasticSoilModel> CreateUpdateStrategyConfiguration(
            PipingFailureMechanism failureMechanism)
        {
            return new StochasticSoilModelImporterConfiguration<PipingStochasticSoilModel>(
                new PipingStochasticSoilModelTransformer(),
                new PipingStochasticSoilModelFilter(),
                new PipingStochasticSoilModelUpdateDataStrategy(failureMechanism));
        }
    }
}
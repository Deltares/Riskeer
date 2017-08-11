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
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Transforms generic <see cref="StochasticSoilModel"/> into <see cref="PipingStochasticSoilModel"/>.
    /// </summary>
    public class PipingStochasticSoilModelTransformer : IStochasticSoilModelTransformer<PipingStochasticSoilModel>
    {
        private readonly Dictionary<ISoilProfile, PipingSoilProfile> soilProfiles = new Dictionary<ISoilProfile, PipingSoilProfile>();

        public PipingStochasticSoilModel Transform(StochasticSoilModel stochasticSoilModel)
        {
            if (stochasticSoilModel.FailureMechanismType != FailureMechanismType.Piping)
            {
                string message = $"Stochastic soil model of failure mechanism type '{stochasticSoilModel.FailureMechanismType}' is not supported." +
                                 $"Only stochastic soil model of failure mechanism type '{FailureMechanismType.Piping}' is supported.";
                throw new ImportedDataTransformException(message);
            }

            IEnumerable<PipingStochasticSoilProfile> pipingStochasticSoilProfiles = TransformStochasticSoilProfiles(stochasticSoilModel.StochasticSoilProfiles);

            var pipingModel = new PipingStochasticSoilModel(stochasticSoilModel.Name);
            pipingModel.Geometry.AddRange(stochasticSoilModel.Geometry);
            pipingModel.StochasticSoilProfiles.AddRange(pipingStochasticSoilProfiles);

            return pipingModel;
        }

        /// <summary>
        /// Transforms all generic <see cref="StochasticSoilProfile"/> into <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="stochasticSoilProfiles">The stochastic soil profiles to use in the transformation.</param>
        /// <returns>The transformed piping stochastic soil profiles.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would 
        /// not result in a valid transformed instance.</exception>
        private IEnumerable<PipingStochasticSoilProfile> TransformStochasticSoilProfiles(IEnumerable<StochasticSoilProfile> stochasticSoilProfiles)
        {
            foreach (StochasticSoilProfile stochasticSoilProfile in stochasticSoilProfiles)
            {
                PipingSoilProfile pipingSoilProfile = GetTransformedPipingSoilProfile(stochasticSoilProfile.SoilProfile);

                if (pipingSoilProfile != null)
                {
                    yield return PipingStochasticSoilProfileTransformer.Transform(stochasticSoilProfile, pipingSoilProfile);
                }
            }
        }

        /// <summary>
        /// Transforms all generic <see cref="ISoilProfile"/> into <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use in the transformation.</param>
        /// <returns>The transformed piping soil profile, or <c>null</c> when <paramref name="soilProfile"/> 
        /// is not of a type that can be transformed to <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would 
        /// not result in a valid transformed instance.</exception>
        private PipingSoilProfile GetTransformedPipingSoilProfile(ISoilProfile soilProfile)
        {
            PipingSoilProfile pipingStochasticSoilProfile;
            if (soilProfiles.ContainsKey(soilProfile))
            {
                pipingStochasticSoilProfile = soilProfiles[soilProfile];
            }
            else
            {
                pipingStochasticSoilProfile = PipingSoilProfileTransformer.Transform(soilProfile);
                soilProfiles.Add(soilProfile, pipingStochasticSoilProfile);
            }
            return pipingStochasticSoilProfile;
        }
    }
}
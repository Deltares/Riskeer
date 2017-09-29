﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.SoilProfiles
{
    /// <summary>
    /// Transforms generic <see cref="StochasticSoilModel"/> into <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
    /// </summary>
    public class MacroStabilityInwardsStochasticSoilModelTransformer : IStochasticSoilModelTransformer<MacroStabilityInwardsStochasticSoilModel>
    {
        private readonly Dictionary<ISoilProfile, IMacroStabilityInwardsSoilProfile> soilProfiles = new Dictionary<ISoilProfile, IMacroStabilityInwardsSoilProfile>();

        public MacroStabilityInwardsStochasticSoilModel Transform(StochasticSoilModel stochasticSoilModel)
        {
            if (stochasticSoilModel == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilModel));
            }

            if (stochasticSoilModel.FailureMechanismType != FailureMechanismType.Stability)
            {
                string message = string.Format(RingtoetsCommonIOResources.IStochasticSoilModelTransformer_Cannot_transform_FailureMechanismType_0_Only_FailureMechanismType_1_supported,
                                               stochasticSoilModel.FailureMechanismType,
                                               FailureMechanismType.Stability);
                throw new ImportedDataTransformException(message);
            }

            var macroStabilityInwardsModel = new MacroStabilityInwardsStochasticSoilModel(stochasticSoilModel.Name,
                                                                                          stochasticSoilModel.Geometry);
            macroStabilityInwardsModel.StochasticSoilProfiles.AddRange(
                TransformStochasticSoilProfiles(stochasticSoilModel.StochasticSoilProfiles, stochasticSoilModel.Name).ToArray());
            return macroStabilityInwardsModel;
        }

        /// <summary>
        /// Transforms all generic <see cref="StochasticSoilProfile"/> into <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="stochasticSoilProfiles">The stochastic soil profiles to use in the transformation.</param>
        /// <param name="soilModelName">The name of the soil model.</param>
        /// <returns>The transformed stochastic soil profiles.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would 
        /// not result in a valid transformed instance.</exception>
        private IEnumerable<MacroStabilityInwardsStochasticSoilProfile> TransformStochasticSoilProfiles(
            IEnumerable<StochasticSoilProfile> stochasticSoilProfiles,
            string soilModelName)
        {
            IEnumerable<StochasticSoilProfile> profilesToTransform =
                StochasticSoilProfileHelper.GetValidatedStochasticSoilProfilesToTransform(stochasticSoilProfiles,
                                                                                          soilModelName);

            return profilesToTransform.Select(
                ssp => MacroStabilityInwardsStochasticSoilProfileTransformer.Transform(
                    ssp,
                    GetTransformedSoilProfile(ssp.SoilProfile))).ToArray();
        }

        /// <summary>
        /// Transforms all generic <see cref="ISoilProfile"/> into <see cref="IMacroStabilityInwardsSoilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use in the transformation.</param>
        /// <returns>The transformed soil profile.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would 
        /// not result in a valid transformed instance.</exception>
        private IMacroStabilityInwardsSoilProfile GetTransformedSoilProfile(ISoilProfile soilProfile)
        {
            IMacroStabilityInwardsSoilProfile macroStabilityInwardsSoilProfile;
            if (soilProfiles.ContainsKey(soilProfile))
            {
                macroStabilityInwardsSoilProfile = soilProfiles[soilProfile];
            }
            else
            {
                macroStabilityInwardsSoilProfile = MacroStabilityInwardsSoilProfileTransformer.Transform(soilProfile);
                soilProfiles.Add(soilProfile, macroStabilityInwardsSoilProfile);
            }
            return macroStabilityInwardsSoilProfile;
        }
    }
}
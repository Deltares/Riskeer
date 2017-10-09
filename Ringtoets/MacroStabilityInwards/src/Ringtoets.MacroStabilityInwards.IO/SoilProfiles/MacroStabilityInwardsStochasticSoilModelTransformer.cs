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

using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(MacroStabilityInwardsStochasticSoilModelTransformer));
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

            try
            {
                MacroStabilityInwardsStochasticSoilProfile[] stochasticSoilProfiles =
                    TransformStochasticSoilProfiles(stochasticSoilModel.StochasticSoilProfiles, stochasticSoilModel.Name).ToArray();

                return new MacroStabilityInwardsStochasticSoilModel(stochasticSoilModel.Name,
                                                                    stochasticSoilModel.Geometry,
                                                                    stochasticSoilProfiles);
            }
            catch (ArgumentException e)
            {
                throw new ImportedDataTransformException(e.Message, e);
            }
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
            MacroStabilityInwardsStochasticSoilProfile[] transformedProfiles = stochasticSoilProfiles.Select(
                ssp => MacroStabilityInwardsStochasticSoilProfileTransformer.Transform(
                    ssp,
                    GetTransformedSoilProfile(ssp.SoilProfile))).ToArray();

            return GetUniqueStochasticSoilProfiles(transformedProfiles, soilModelName);
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

        /// <summary>
        /// Filters a collection of <see cref="MacroStabilityInwardsStochasticSoilProfile"/> to determine which items
        /// are unique.
        /// </summary>
        /// <param name="stochasticSoilProfiles">The collection of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>
        /// to filter.</param>
        /// <param name="soilModelName">The name of the soil model.</param>
        /// <returns>A collection of unique <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when summing the probabilities of  
        /// <see cref="MacroStabilityInwardsStochasticSoilProfile"/> with the same <see cref="MacroStabilityInwardsStochasticSoilProfile.SoilProfile"/>
        /// results in an invalid probability.</exception>
        private static IEnumerable<MacroStabilityInwardsStochasticSoilProfile> GetUniqueStochasticSoilProfiles(
            IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles,
            string soilModelName)
        {
            List<MacroStabilityInwardsStochasticSoilProfile> uniqueStochasticSoilProfiles = stochasticSoilProfiles.ToList();
            MacroStabilityInwardsStochasticSoilProfile[] allStochasticSoilProfiles = uniqueStochasticSoilProfiles.ToArray();

            try
            {
                for (var i = 1; i < allStochasticSoilProfiles.Length; i++)
                {
                    MacroStabilityInwardsStochasticSoilProfile previousProfile = allStochasticSoilProfiles[i - 1];
                    MacroStabilityInwardsStochasticSoilProfile currentProfile = allStochasticSoilProfiles[i];
                    if (ReferenceEquals(currentProfile.SoilProfile, previousProfile.SoilProfile))
                    {
                        log.Warn(string.Format(RingtoetsCommonIOResources.SoilModelTransformer_GetUniqueStochasticSoilProfiles_StochasticSoilProfile_0_has_multiple_occurences_in_SoilModel_1_Probability_Summed,
                                               previousProfile.SoilProfile.Name,
                                               soilModelName));

                        previousProfile.AddProbability(currentProfile.Probability);
                        uniqueStochasticSoilProfiles.Remove(currentProfile);
                    }
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ImportedDataTransformException(e.Message, e);
            }

            return uniqueStochasticSoilProfiles;
        }
    }
}
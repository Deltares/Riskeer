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
using Core.Common.Base.Properties;
using log4net;
using Ringtoets.Common.IO.Exceptions;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class which provides helper methods for the <see cref="StochasticSoilProfile"/>.
    /// </summary>
    public static class StochasticSoilProfileHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StochasticSoilProfileHelper));

        /// <summary>
        /// Validates a collection of <see cref="StochasticSoilProfile"/> to determine which items
        /// should be transformed.
        /// </summary>
        /// <param name="stochasticSoilProfiles">The collection of <see cref="StochasticSoilProfile"/>
        /// to validate.</param>
        /// <param name="soilModelName">The name of the soil model.</param>
        /// <returns>A collection of validated <see cref="StochasticSoilProfile"/> to transform.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when a <see cref="StochasticSoilProfile"/>
        /// is invalid.</exception>
        public static IEnumerable<StochasticSoilProfile> GetValidatedStochasticSoilProfilesToTransform(IEnumerable<StochasticSoilProfile> stochasticSoilProfiles,
                                                                                                   string soilModelName)
        {
            if (stochasticSoilProfiles == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilProfiles));
            }
            if (soilModelName == null)
            {
                throw new ArgumentNullException(nameof(soilModelName));
            }

            List<StochasticSoilProfile> profilesToTransform = stochasticSoilProfiles.ToList();
            StochasticSoilProfile[] allStochasticSoilProfiles = profilesToTransform.ToArray();

            try
            {
                for (var i = 1; i < allStochasticSoilProfiles.Length; i++)
                {
                    StochasticSoilProfile previousProfile = allStochasticSoilProfiles[i - 1];
                    StochasticSoilProfile currentProfile = allStochasticSoilProfiles[i];
                    if (ReferenceEquals(currentProfile.SoilProfile, previousProfile.SoilProfile))
                    {
                        log.Warn(string.Format(Resources.StochasticSoilProfileHelper_GetValidatedStochasticProfilesToTransform_StochasticSoilProfile_0_has_multiple_occurences_in_SoilModel_1_Probability_Summed,
                                               previousProfile.SoilProfile.Name,
                                               soilModelName));

                        previousProfile.AddProbability(currentProfile.Probability);
                        profilesToTransform.Remove(currentProfile);
                    }
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ImportedDataTransformException(e.Message, e);
            }

            return profilesToTransform;
        }
    }
}
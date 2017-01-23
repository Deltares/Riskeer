// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.Properties;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Class providing a base implementation of an update stratagy for stochastic soil models.
    /// </summary>
    public abstract class StochasticSoilModelUpdateStrategyBase : IStochasticSoilModelUpdateStrategy
    {
        public abstract void UpdateModelWithImportedData(
            ICollection<StochasticSoilModel> readStochasticSoilModels, 
            string sourceFilePath, 
            StochasticSoilModelCollection targetCollection,
            Action<string, int, int> notifyProgress);

        /// <summary>
        /// Validate the definition of a <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="stochasticSoilModel">The <see cref="StochasticSoilModel"/> to validate.</param>
        /// <returns><c>false</c> when the stochastic soil model does not contain any stochastic soil profiles
        /// or when a stochastic soil profile does not have a definition for a soil profile; <c>true</c>
        /// otherwise.</returns>
        protected static bool ValidateStochasticSoilModel(StochasticSoilModel stochasticSoilModel)
        {
            var log = LogManager.GetLogger(typeof(StochasticSoilModelImporter));

            if (!stochasticSoilModel.StochasticSoilProfiles.Any())
            {
                log.WarnFormat(Resources.PipingSoilProfilesImporter_ValidateStochasticSoilModel_No_profiles_found_in_stochastic_soil_model_0,
                               stochasticSoilModel.Name);
                return false;
            }
            if (stochasticSoilModel.StochasticSoilProfiles.Any(ssp => ssp.SoilProfile == null))
            {
                log.WarnFormat(Resources.PipingSoilProfilesImporter_ValidateStochasticSoilModel_SoilModel_0_with_stochastic_soil_profile_without_profile,
                               stochasticSoilModel.Name);
                return false;
            }
            if (!IsSumOfAllProbabilitiesEqualToOne(stochasticSoilModel))
            {
                log.WarnFormat(Resources.PipingSoilProfilesImporter_ValidateStochasticSoilModel_Sum_of_probabilities_of_stochastic_soil_model_0_is_not_correct,
                               stochasticSoilModel.Name);
            }
            return true;
        }

        private static bool IsSumOfAllProbabilitiesEqualToOne(StochasticSoilModel stochasticSoilModel)
        {
            double sumOfAllScenarioProbabilities = stochasticSoilModel.StochasticSoilProfiles
                                                                      .Where(s => s.SoilProfile != null)
                                                                      .Sum(s => s.Probability);
            return Math.Abs(sumOfAllScenarioProbabilities - 1.0) < 1e-6;
        }
    }
}
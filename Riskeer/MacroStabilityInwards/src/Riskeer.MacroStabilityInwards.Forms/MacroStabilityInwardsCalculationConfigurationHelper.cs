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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using log4net;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.Helpers;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms
{
    /// <summary>
    /// Class holds methods to help views when dealing with <see cref="MacroStabilityInwardsCalculation"/>
    /// </summary>
    public static class MacroStabilityInwardsCalculationConfigurationHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MacroStabilityInwardsCalculationConfigurationHelper));

        /// <summary>
        /// Creates a structure of <see cref="CalculationGroup"/> and <see cref="MacroStabilityInwardsCalculation"/> based on combination of the
        /// <paramref name="surfaceLines"/> and the <paramref name="soilModels"/>.
        /// </summary>
        /// <param name="surfaceLines">Surface lines to generate the structure for and to use to configure <see cref="MacroStabilityInwardsCalculation"/>
        /// with.</param>
        /// <param name="soilModels">The soil models from which profiles are taken to configure <see cref="MacroStabilityInwardsCalculation"/> with.</param>
        /// <returns>A structure of <see cref="ICalculationBase"/> matching combinations of <paramref name="surfaceLines"/> and
        /// profiles of intersecting <paramref name="soilModels"/>.</returns>
        /// <exception cref="ArgumentNullException">Throw when either:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLines"/> is <c>null</c></item>
        /// <item><paramref name="soilModels"/> is <c>null</c></item>
        /// </list></exception>
        public static IEnumerable<ICalculationBase> GenerateCalculationItemsStructure(IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines, IEnumerable<MacroStabilityInwardsStochasticSoilModel> soilModels)
        {
            if (surfaceLines == null)
            {
                throw new ArgumentNullException(nameof(surfaceLines));
            }

            if (soilModels == null)
            {
                throw new ArgumentNullException(nameof(soilModels));
            }

            var groups = new List<CalculationGroup>();
            foreach (MacroStabilityInwardsSurfaceLine surfaceLine in surfaceLines)
            {
                CalculationGroup group = CreateCalculationGroup(surfaceLine, soilModels);
                if (group.GetCalculations().Any())
                {
                    groups.Add(group);
                }
                else
                {
                    log.WarnFormat(
                        Resources.MacroStabilityInwardsCalculationConfigurationHelper_GenerateCalculationsStructure_No_SoilProfile_found_for_MacroStabilityInwardsSurfaceLine_0_skipped,
                        surfaceLine.Name);
                }
            }

            return groups;
        }

        /// <summary>
        /// Gets the stochastic soil models matching the input of a calculation.
        /// </summary>
        /// <param name="surfaceLine">The surface line used to match a <see cref="MacroStabilityInwardsStochasticSoilModel"/>.</param>
        /// <param name="availableSoilModels">The available stochastic soil models.</param>
        /// <returns>The (sub)set of stochastic soil models from <paramref name="availableSoilModels"/>
        /// or empty if no matching <see cref="MacroStabilityInwardsStochasticSoilModel"/> instances can be found
        /// or when there is not enough information to associate soil profiles to the calculation.</returns>
        public static IEnumerable<MacroStabilityInwardsStochasticSoilModel> GetStochasticSoilModelsForSurfaceLine(MacroStabilityInwardsSurfaceLine surfaceLine, IEnumerable<MacroStabilityInwardsStochasticSoilModel> availableSoilModels)
        {
            if (surfaceLine == null)
            {
                return Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>();
            }

            return availableSoilModels.Where(stochasticSoilModel => stochasticSoilModel.StochasticSoilProfiles.Any() &&
                                                                    stochasticSoilModel.IntersectsWithSurfaceLineGeometry(surfaceLine))
                                      .ToList();
        }

        private static CalculationGroup CreateCalculationGroup(MacroStabilityInwardsSurfaceLine surfaceLine, IEnumerable<MacroStabilityInwardsStochasticSoilModel> soilModels)
        {
            var calculationGroup = new CalculationGroup
            {
                Name = surfaceLine.Name
            };
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels = GetStochasticSoilModelsForSurfaceLine(surfaceLine, soilModels);
            foreach (MacroStabilityInwardsStochasticSoilModel stochasticSoilModel in stochasticSoilModels)
            {
                foreach (MacroStabilityInwardsStochasticSoilProfile soilProfile in stochasticSoilModel.StochasticSoilProfiles)
                {
                    calculationGroup.Children.Add(CreateMacroStabilityInwardsCalculation(surfaceLine, stochasticSoilModel, soilProfile, calculationGroup.Children));
                }
            }

            return calculationGroup;
        }

        private static ICalculationBase CreateMacroStabilityInwardsCalculation(MacroStabilityInwardsSurfaceLine surfaceLine, MacroStabilityInwardsStochasticSoilModel stochasticSoilModel, MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile, IEnumerable<ICalculationBase> calculations)
        {
            string nameBase = $"{surfaceLine.Name} {stochasticSoilProfile}";
            string name = NamingHelper.GetUniqueName(calculations, nameBase, c => c.Name);

            return new MacroStabilityInwardsCalculationScenario
            {
                Name = name,
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                },
                Contribution = (RoundedDouble) stochasticSoilProfile.Probability
            };
        }
    }
}
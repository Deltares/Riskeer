// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.Properties;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms
{
    /// <summary>
    /// Class holds methods to help views when dealing with <see cref="PipingCalculation"/>
    /// </summary>
    public static class PipingCalculationConfigurationHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PipingCalculationConfigurationHelper));

        /// <summary>
        /// Creates a structure of <see cref="CalculationGroup"/> and <see cref="PipingCalculation"/> based on combination of the
        /// <paramref name="surfaceLines"/> and the <paramref name="soilModels"/>.
        /// </summary>
        /// <param name="surfaceLines">Surface lines to generate the structure for and to use to configure <see cref="PipingCalculation"/>
        /// with.</param>
        /// <param name="soilModels">The soil models from which profiles are taken to configure <see cref="PipingCalculation"/> with.</param>
        /// <param name="generalInput">General input to assign to each generated piping calculation.</param>
        /// <returns>A structure of <see cref="ICalculationBase"/> matching combinations of <paramref name="surfaceLines"/> and
        /// profiles of intersecting <paramref name="soilModels"/>.</returns>
        /// <exception cref="ArgumentNullException">Throw when either:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLines"/> is <c>null</c></item>
        /// <item><paramref name="soilModels"/> is <c>null</c></item>
        /// <item><paramref name="generalInput"/> is <c>null</c></item>
        /// </list></exception>
        public static IEnumerable<ICalculationBase> GenerateCalculationItemsStructure(IEnumerable<PipingSurfaceLine> surfaceLines,
                                                                                      IEnumerable<PipingStochasticSoilModel> soilModels,
                                                                                      GeneralPipingInput generalInput)
        {
            if (surfaceLines == null)
            {
                throw new ArgumentNullException(nameof(surfaceLines));
            }

            if (soilModels == null)
            {
                throw new ArgumentNullException(nameof(soilModels));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            var groups = new List<CalculationGroup>();
            foreach (PipingSurfaceLine surfaceLine in surfaceLines)
            {
                CalculationGroup group = CreateCalculationGroup(surfaceLine, soilModels, generalInput);
                if (group.GetCalculations().Any())
                {
                    groups.Add(group);
                }
                else
                {
                    log.WarnFormat(
                        Resources.PipingCalculationConfigurationHelper_GenerateCalculationsStructure_No_PipingSoilProfile_found_for_PipingSurfaceLine_0_skipped,
                        surfaceLine.Name);
                }
            }

            return groups;
        }

        /// <summary>
        /// Gets the stochastic soil models matching the input of a calculation.
        /// </summary>
        /// <param name="surfaceLine">The surface line used to match a <see cref="PipingStochasticSoilModel"/>.</param>
        /// <param name="availableSoilModels">The available stochastic soil models.</param>
        /// <returns>The (sub)set of stochastic soil models from <paramref name="availableSoilModels"/>
        /// or empty if no matching <see cref="PipingStochasticSoilModel"/> instances can be found
        /// or when there is not enough information to associate soil profiles to the calculation.</returns>
        public static IEnumerable<PipingStochasticSoilModel> GetStochasticSoilModelsForSurfaceLine(PipingSurfaceLine surfaceLine,
                                                                                                   IEnumerable<PipingStochasticSoilModel> availableSoilModels)
        {
            if (surfaceLine == null)
            {
                return Enumerable.Empty<PipingStochasticSoilModel>();
            }

            return availableSoilModels.Where(stochasticSoilModel => stochasticSoilModel.StochasticSoilProfiles.Any() &&
                                                                    stochasticSoilModel.IntersectsWithSurfaceLineGeometry(surfaceLine))
                                      .ToList();
        }

        private static CalculationGroup CreateCalculationGroup(PipingSurfaceLine surfaceLine,
                                                               IEnumerable<PipingStochasticSoilModel> soilModels,
                                                               GeneralPipingInput generalInput)
        {
            var calculationGroup = new CalculationGroup
            {
                Name = surfaceLine.Name
            };
            IEnumerable<PipingStochasticSoilModel> stochasticSoilModels = GetStochasticSoilModelsForSurfaceLine(surfaceLine, soilModels);
            foreach (PipingStochasticSoilModel stochasticSoilModel in stochasticSoilModels)
            {
                foreach (PipingStochasticSoilProfile soilProfile in stochasticSoilModel.StochasticSoilProfiles)
                {
                    calculationGroup.Children.Add(CreatePipingCalculation(surfaceLine, stochasticSoilModel, soilProfile, calculationGroup.Children, generalInput));
                }
            }

            return calculationGroup;
        }

        private static ICalculationBase CreatePipingCalculation(PipingSurfaceLine surfaceLine,
                                                                PipingStochasticSoilModel stochasticSoilModel,
                                                                PipingStochasticSoilProfile stochasticSoilProfile,
                                                                IEnumerable<ICalculationBase> calculations,
                                                                GeneralPipingInput generalInput)
        {
            string nameBase = $"{surfaceLine.Name} {stochasticSoilProfile}";
            string name = NamingHelper.GetUniqueName(calculations, nameBase, c => c.Name);

            return new PipingCalculationScenario(generalInput)
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
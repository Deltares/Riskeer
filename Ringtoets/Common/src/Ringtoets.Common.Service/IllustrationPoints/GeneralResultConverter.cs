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
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingWindDirectionClosingSituation = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraRingIllustrationPointTreeNode = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraRingGeneralResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;
using IHydraRingIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IIllustrationPoint;
using HydraRingSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// Converter for <see cref="HydraRingGeneralResult"/> related to creating a <see cref="GeneralResult{T}"/>.
    /// </summary>
    public static class GeneralResultConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult{T}"/> for top level sub
        /// mechanism illustration points based on the information of <paramref name="hydraRingGeneralResult"/>.
        /// </summary>
        /// <param name="hydraRingGeneralResult">The <see cref="HydraRingGeneralResult"/> to base the 
        /// <see cref="GeneralResult{T}"/> to create on.</param>
        /// <returns>The newly created <see cref="GeneralResult{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingGeneralResult"/> is <c>null</c>.</exception>
        public static GeneralResult<TopLevelSubMechanismIllustrationPoint> CreateGeneralResultTopLevelSubMechanismIllustrationPoint(
            HydraRingGeneralResult hydraRingGeneralResult)
        {
            if (hydraRingGeneralResult == null)
            {
                throw new ArgumentNullException(nameof(hydraRingGeneralResult));
            }

            WindDirection windDirection = WindDirectionConverter.Create(hydraRingGeneralResult.GoverningWindDirection);
            IEnumerable<Stochast> stochasts = GetStochasts(hydraRingGeneralResult);
            IEnumerable<TopLevelSubMechanismIllustrationPoint> windDirectionClosingScenarioIllustrationPoints =
                GetWindDirectionClosingSituationIllustrationPoint(hydraRingGeneralResult);

            return new GeneralResult<TopLevelSubMechanismIllustrationPoint>(windDirection, stochasts, windDirectionClosingScenarioIllustrationPoints);
        }

        private static IEnumerable<Stochast> GetStochasts(HydraRingGeneralResult hydraGeneralResult)
        {
            return hydraGeneralResult.Stochasts.Select(StochastConverter.CreateStochast);
        }

        private static IEnumerable<TopLevelSubMechanismIllustrationPoint> GetWindDirectionClosingSituationIllustrationPoint(
            HydraRingGeneralResult hydraGeneralResult)
        {
            var combinations = new List<TopLevelSubMechanismIllustrationPoint>();
            foreach (KeyValuePair<HydraRingWindDirectionClosingSituation, HydraRingIllustrationPointTreeNode> illustrationPointTreeNode in hydraGeneralResult.IllustrationPoints)
            {
                IHydraRingIllustrationPoint hydraIllustrationPoint = illustrationPointTreeNode.Value.Data;
                HydraRingWindDirectionClosingSituation hydraWindDirectionClosingSituation = illustrationPointTreeNode.Key;

                var subMechanismIllustrationPoint = hydraIllustrationPoint as HydraRingSubMechanismIllustrationPoint;
                if (subMechanismIllustrationPoint != null)
                {
                    combinations.Add(TopLevelSubMechanismIllustrationPointConverter.Create(
                                         hydraWindDirectionClosingSituation, subMechanismIllustrationPoint));
                }
            }

            return combinations;
        }
    }
}
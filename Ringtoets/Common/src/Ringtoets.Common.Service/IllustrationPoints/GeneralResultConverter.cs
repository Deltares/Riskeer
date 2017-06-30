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
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using HydraWindDirectionClosingSituation = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraIllustrationPointTreeNode = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraGeneralResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;
using IHydraRingIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IIllustrationPoint;
using SubmechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubmechanismIllustrationPoint;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// Converter for <see cref="HydraGeneralResult"/> related to creating a <see cref="GeneralResult"/>.
    /// </summary>
    public static class GeneralResultConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/> based on the information of <paramref name="hydraGeneralResult"/>.
        /// </summary>
        /// <param name="hydraGeneralResult">The <see cref="HydraGeneralResult"/> to base the 
        /// <see cref="GeneralResult"/> to create on.</param>
        /// <returns>The newly created <see cref="GeneralResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraGeneralResult"/> is <c>null</c>.</exception>
        public static GeneralResult CreateGeneralResult(HydraGeneralResult hydraGeneralResult)
        {
            if (hydraGeneralResult == null)
            {
                throw new ArgumentNullException(nameof(hydraGeneralResult));
            }

            WindDirection windDirection = WindDirectionConverter.CreateWindDirection(hydraGeneralResult.GoverningWindDirection);
            IEnumerable<Stochast> stochasts = GetStochasts(hydraGeneralResult);
            IEnumerable<TopLevelSubmechanismIllustrationPoint> windDirectionClosingScenarioIllustrationPoints =
                GetWindDirectionClosingSituationIllustrationPoint(hydraGeneralResult);

            return new GeneralResult(windDirection, stochasts, windDirectionClosingScenarioIllustrationPoints);
        }

        private static IEnumerable<Stochast> GetStochasts(HydraGeneralResult hydraGeneralResult)
        {
            return hydraGeneralResult.Stochasts.Select(StochastConverter.CreateStochast);
        }

        private static IEnumerable<TopLevelSubmechanismIllustrationPoint> GetWindDirectionClosingSituationIllustrationPoint(
            HydraGeneralResult hydraGeneralResult)
        {
            var combinations = new List<TopLevelSubmechanismIllustrationPoint>();
            foreach (KeyValuePair<HydraWindDirectionClosingSituation, HydraIllustrationPointTreeNode> illustrationPointTreeNode in hydraGeneralResult.IllustrationPoints)
            {
                IHydraRingIllustrationPoint hydraIllustrationPoint = illustrationPointTreeNode.Value.Data;
                HydraWindDirectionClosingSituation hydraWindDirectionClosingSituation = illustrationPointTreeNode.Key;

                var subMechanismIllustrationPoint = hydraIllustrationPoint as SubmechanismIllustrationPoint;
                if (subMechanismIllustrationPoint != null)
                {
                    combinations.Add(TopLevelSubmechanismIllustrationPointConverter.CreateWindDirectionClosingScenarioIllustrationPoint(
                                         hydraWindDirectionClosingSituation, subMechanismIllustrationPoint));
                }
            }

            return combinations;
        }
    }
}
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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingWindDirectionClosingSituation = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraRingIllustrationPointTreeNode = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraRingGeneralResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;
using IHydraRingIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IIllustrationPoint;
using HydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraRingFaultTreeIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// Converter for <see cref="HydraRingGeneralResult"/> related to creating a <see cref="GeneralResult{T}"/>.
    /// </summary>
    public static class GeneralResultConverter
    {
        private static IEnumerable<Stochast> GetStochasts(HydraRingGeneralResult hydraGeneralResult)
        {
            return hydraGeneralResult.Stochasts.Select(StochastConverter.Convert).ToArray();
        }

        #region SubMechanismIllustrationPoint

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult{T}"/> for top level sub
        /// mechanism illustration points based on the information of <paramref name="hydraRingGeneralResult"/>.
        /// </summary>
        /// <param name="hydraRingGeneralResult">The <see cref="HydraRingGeneralResult"/> to base the 
        /// <see cref="GeneralResult{T}"/> to create on.</param>
        /// <returns>The newly created <see cref="GeneralResult{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingGeneralResult"/> is <c>null</c>.</exception>
        /// <exception cref="IllustrationPointConversionException">Thrown when the <paramref name="hydraRingGeneralResult"/>
        /// cannot be converted to a <see cref="GeneralResult{T}"/> with top level sub mechanism illustration points.</exception>
        public static GeneralResult<TopLevelSubMechanismIllustrationPoint> ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint(
            HydraRingGeneralResult hydraRingGeneralResult)
        {
            if (hydraRingGeneralResult == null)
            {
                throw new ArgumentNullException(nameof(hydraRingGeneralResult));
            }

            WindDirection governingWindDirection = WindDirectionConverter.Convert(hydraRingGeneralResult.GoverningWindDirection);
            IEnumerable<Stochast> stochasts = GetStochasts(hydraRingGeneralResult);
            IEnumerable<TopLevelSubMechanismIllustrationPoint> windDirectionClosingScenarioIllustrationPoints =
                GetTopLevelSubMechanismIllustrationPoints(hydraRingGeneralResult.IllustrationPoints);

            return new GeneralResult<TopLevelSubMechanismIllustrationPoint>(governingWindDirection,
                                                                            stochasts,
                                                                            windDirectionClosingScenarioIllustrationPoints);
        }

        /// <summary>
        /// Creates all the top level fault tree illustration points based on the 
        /// combinations of <see cref="HydraRingWindDirectionClosingSituation"/> and 
        /// <see cref="HydraRingFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="hydraRingTopLevelIllustrationPoints">The collection of 
        /// <see cref="HydraRingWindDirectionClosingSituation"/> and <see cref="HydraRingFaultTreeIllustrationPoint"/> 
        /// combinations to base the <see cref="TopLevelFaultTreeIllustrationPoint"/> on.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="TopLevelFaultTreeIllustrationPoint"/>.</returns>
        /// <exception cref="IllustrationPointConversionException">Thrown when the combination of 
        /// <see cref="HydraRingWindDirectionClosingSituation"/> and <see cref="HydraRingFaultTreeIllustrationPoint"/> 
        /// cannot be converted to <see cref="TopLevelSubMechanismIllustrationPoint"/>.</exception>
        private static IEnumerable<TopLevelSubMechanismIllustrationPoint> GetTopLevelSubMechanismIllustrationPoints(
            IDictionary<HydraRingWindDirectionClosingSituation, HydraRingIllustrationPointTreeNode> hydraRingTopLevelIllustrationPoints)
        {
            var topLevelIlustrationPoints = new List<TopLevelSubMechanismIllustrationPoint>();
            foreach (KeyValuePair<HydraRingWindDirectionClosingSituation,
                         HydraRingIllustrationPointTreeNode> topLevelIllustrationPointTreeNode in hydraRingTopLevelIllustrationPoints)
            {
                IHydraRingIllustrationPoint hydraIllustrationPointData = topLevelIllustrationPointTreeNode.Value.Data;
                HydraRingWindDirectionClosingSituation hydraWindDirectionClosingSituation = topLevelIllustrationPointTreeNode.Key;

                var subMechanismIllustrationPoint = hydraIllustrationPointData as HydraRingSubMechanismIllustrationPoint;
                if (subMechanismIllustrationPoint != null)
                {
                    topLevelIlustrationPoints.Add(TopLevelSubMechanismIllustrationPointConverter.Convert(
                                                      hydraWindDirectionClosingSituation, subMechanismIllustrationPoint));
                }
                else
                {
                    string exceptionMessage = $"Expected a fault tree node with data of type {typeof(HydraRingSubMechanismIllustrationPoint)} as root, " +
                                              $"but got {hydraIllustrationPointData.GetType()}";
                    throw new IllustrationPointConversionException(exceptionMessage);
                }
            }

            return topLevelIlustrationPoints;
        }

        #endregion

        #region FaultTreeIllustrationPoint

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult{T}"/> for fault tree illustration points
        /// based on the information of <see cref="HydraRingGeneralResult"/>.
        /// </summary>
        /// <param name="hydraRingGeneralResult">The <see cref="HydraRingGeneralResult"/>
        /// to base the <see cref="GeneralResult{T}"/> to create on.</param>
        /// <returns>The newly created <see cref="GeneralResult{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingGeneralResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IllustrationPointConversionException">Thrown when the <paramref name="hydraRingGeneralResult"/>
        /// cannot be converted to a <see cref="GeneralResult{T}"/> with top level fault tree illustration points.</exception>
        public static GeneralResult<TopLevelFaultTreeIllustrationPoint> ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint(
            HydraRingGeneralResult hydraRingGeneralResult)
        {
            if (hydraRingGeneralResult == null)
            {
                throw new ArgumentNullException(nameof(hydraRingGeneralResult));
            }

            WindDirection governingWindDirection = WindDirectionConverter.Convert(hydraRingGeneralResult.GoverningWindDirection);
            IEnumerable<Stochast> stochasts = GetStochasts(hydraRingGeneralResult);
            IEnumerable<TopLevelFaultTreeIllustrationPoint> topLevelIllustrationPoints =
                GetTopLevelFaultTreeIllustrationPoints(hydraRingGeneralResult.IllustrationPoints);

            return new GeneralResult<TopLevelFaultTreeIllustrationPoint>(governingWindDirection,
                                                                         stochasts,
                                                                         topLevelIllustrationPoints);
        }

        /// <summary>
        /// Creates all the top level fault tree illustration points based on the 
        /// combinations of <see cref="HydraRingWindDirectionClosingSituation"/> and 
        /// <see cref="HydraRingFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="hydraRingTopLevelIllustrationPoints">The collection of 
        /// <see cref="HydraRingWindDirectionClosingSituation"/> and <see cref="HydraRingFaultTreeIllustrationPoint"/> 
        /// combinations to base the <see cref="TopLevelFaultTreeIllustrationPoint"/> on.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="TopLevelFaultTreeIllustrationPoint"/>.</returns>
        /// <exception cref="IllustrationPointConversionException">Thrown when the combination of 
        /// <see cref="HydraRingWindDirectionClosingSituation"/> and <see cref="HydraRingFaultTreeIllustrationPoint"/> 
        /// cannot be converted to <see cref="TopLevelFaultTreeIllustrationPoint"/>.</exception>
        private static IEnumerable<TopLevelFaultTreeIllustrationPoint> GetTopLevelFaultTreeIllustrationPoints(
            IDictionary<HydraRingWindDirectionClosingSituation, HydraRingIllustrationPointTreeNode> hydraRingTopLevelIllustrationPoints)
        {
            var topLevelIllustrationPoints = new List<TopLevelFaultTreeIllustrationPoint>();
            foreach (KeyValuePair<HydraRingWindDirectionClosingSituation,
                         HydraRingIllustrationPointTreeNode> hydraRingillustrationPointTreeNode in hydraRingTopLevelIllustrationPoints)
            {
                HydraRingIllustrationPointTreeNode hydraRingIllustrationPointTreeNode = hydraRingillustrationPointTreeNode.Value;
                IHydraRingIllustrationPoint hydraIllustrationPointData = hydraRingIllustrationPointTreeNode.Data;
                HydraRingWindDirectionClosingSituation hydraWindDirectionClosingSituation = hydraRingillustrationPointTreeNode.Key;

                var faultTreeIllustrationPoint = hydraIllustrationPointData as HydraRingFaultTreeIllustrationPoint;
                if (faultTreeIllustrationPoint != null)
                {
                    topLevelIllustrationPoints.Add(TopLevelFaultTreeIllustrationPointConverter.Convert(
                                                       hydraWindDirectionClosingSituation, hydraRingIllustrationPointTreeNode));
                }
                else
                {
                    string exceptionMessage = $"Expected a fault tree node with data of type {typeof(HydraRingFaultTreeIllustrationPoint)} as root, " +
                                              $"but got {hydraIllustrationPointData.GetType()}";
                    throw new IllustrationPointConversionException(exceptionMessage);
                }
            }

            return topLevelIllustrationPoints;
        }

        #endregion
    }
}
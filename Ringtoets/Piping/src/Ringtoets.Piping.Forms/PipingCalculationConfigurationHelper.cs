﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms
{
    /// <summary>
    /// Class holds methods to help views when dealing with <see cref="PipingCalculation"/>
    /// </summary>
    public static class PipingCalculationConfigurationHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PipingCalculationConfigurationHelper));

        /// <summary>
        /// Creates a structure of <see cref="PipingCalculationGroup"/> and <see cref="PipingCalculation"/> based on combination of the
        /// <paramref name="surfaceLines"/> and the <paramref name="soilModels"/>.
        /// </summary>
        /// <param name="surfaceLines">Surface lines to generate the structure for and to use to configure <see cref="PipingCalculation"/>
        ///     with.</param>
        /// <param name="soilModels">The soil models from which profiles are taken to configure <see cref="PipingCalculation"/> with.</param>
        /// <param name="generalInput">General input to assign to each generated piping calculation.</param>
        /// <param name="semiProbabilisticInput">Semi probabilistic input to assign to each generated piping calculation.</param>
        /// <returns>A structure or <see cref="ICalculationItem"/> matching combinations of <paramref name="surfaceLines"/> and
        /// profiles of intersecting <paramref name="soilModels"/>.</returns>
        /// <exception cref="ArgumentNullException">Throw when either:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLines"/> is <c>null</c></item>
        /// <item><paramref name="soilModels"/> is <c>null</c></item>
        /// <item><paramref name="generalInput"/> is <c>null</c></item>
        /// <item><paramref name="semiProbabilisticInput"/> is <c>null</c></item>
        /// </list></exception>
        public static IEnumerable<ICalculationItem> GenerateCalculationItemsStructure(IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> soilModels, GeneralPipingInput generalInput, SemiProbabilisticPipingInput semiProbabilisticInput)
        {
            if (surfaceLines == null)
            {
                throw new ArgumentNullException("surfaceLines");
            }
            if (soilModels == null)
            {
                throw new ArgumentNullException("soilModels");
            }
            if (generalInput == null)
            {
                throw new ArgumentNullException("generalInput");
            }
            if (semiProbabilisticInput == null)
            {
                throw new ArgumentNullException("semiProbabilisticInput");
            }

            List<ICalculationItem> groups = new List<ICalculationItem>();
            foreach (var surfaceLine in surfaceLines)
            {
                var group = CreateCalculationGroup(surfaceLine, soilModels, generalInput, semiProbabilisticInput);
                if (group.GetPipingCalculations().Any())
                {
                    groups.Add(group);
                }
                else
                {
                    log.WarnFormat(
                        Resources.PipingCalculationConfigurationHelper_GenerateCalculationsStructure_No_PipingSoilProfile_found_for_RingtoetsPipingSurfaceLine_0_skipped,
                        surfaceLine.Name);
                }
            }
            return groups;
        }

        /// <summary>
        /// Gets the stochastic soil models matching the input of a calculation.
        /// </summary>
        /// <param name="surfaceLine">The surface line used to match a <see cref="StochasticSoilModel"/>.</param>
        /// <param name="availableSoilModels">The available stochastic soil models.</param>
        /// <returns>The (sub)set of stochastic soil models from <paramref name="availableSoilModels"/>
        /// or empty if no matching <see cref="StochasticSoilModel"/> instances can be found
        /// or when there is not enough information to associate soil profiles to the calculation.</returns>
        public static IEnumerable<StochasticSoilModel> GetStochasticSoilModelsForSurfaceLine(RingtoetsPipingSurfaceLine surfaceLine, IEnumerable<StochasticSoilModel> availableSoilModels)
        {
            if (surfaceLine == null)
            {
                return Enumerable.Empty<StochasticSoilModel>();
            }

            Segment2D[] surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y))).ToArray();

            var soilModelObjectsForCalculation = new List<StochasticSoilModel>();
            foreach (StochasticSoilModel stochasticSoilModel in availableSoilModels.Where(sm => sm.StochasticSoilProfiles.Any()))
            {
                if (DoesSoilModelGeometryIntersectWithSurfaceLineGeometry(stochasticSoilModel, surfaceLineSegments))
                {
                    soilModelObjectsForCalculation.Add(stochasticSoilModel);
                }
            }
            return soilModelObjectsForCalculation;
        }

        private static ICalculationItem CreateCalculationGroup(RingtoetsPipingSurfaceLine surfaceLine, IEnumerable<StochasticSoilModel> soilModels, GeneralPipingInput generalInput, SemiProbabilisticPipingInput semiProbabilisticInput)
        {
            var pipingCalculationGroup = new PipingCalculationGroup(surfaceLine.Name, true);
            var stochasticSoilModels = GetStochasticSoilModelsForSurfaceLine(surfaceLine, soilModels);
            foreach (var stochasticSoilModel in stochasticSoilModels)
            {
                foreach (var soilProfile in stochasticSoilModel.StochasticSoilProfiles)
                {
                    pipingCalculationGroup.Children.Add(CreatePipingCalculation(surfaceLine, stochasticSoilModel, soilProfile, pipingCalculationGroup.Children, generalInput, semiProbabilisticInput));
                }
            }

            return pipingCalculationGroup;
        }

        private static ICalculationItem CreatePipingCalculation(RingtoetsPipingSurfaceLine surfaceLine, StochasticSoilModel stochasticSoilModel, StochasticSoilProfile stochasticSoilProfile, IEnumerable<ICalculationItem> calculations, GeneralPipingInput generalInput, SemiProbabilisticPipingInput semiProbabilisticInput)
        {
            var nameBase = string.Format("{0} {1}", surfaceLine.Name, stochasticSoilProfile);
            var name = NamingHelper.GetUniqueName(calculations, nameBase, c => c.Name);

            return new PipingCalculationScenario(generalInput, semiProbabilisticInput)
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

        private static bool DoesSoilModelGeometryIntersectWithSurfaceLineGeometry(StochasticSoilModel stochasticSoilModel, Segment2D[] surfaceLineSegments)
        {
            IEnumerable<Segment2D> soilProfileGeometrySegments = Math2D.ConvertLinePointsToLineSegments(stochasticSoilModel.Geometry);
            return soilProfileGeometrySegments.Any(s => DoesSegmentIntersectWithSegmentArray(s, surfaceLineSegments));
        }

        private static bool DoesSegmentIntersectWithSegmentArray(Segment2D segment, Segment2D[] segmentArray)
        {
            // Consider intersections and overlaps similarly
            return segmentArray.Any(sls => Math2D.GetIntersectionBetweenSegments(segment, sls).IntersectionType != Intersection2DType.DoesNotIntersect);
        }
    }
}
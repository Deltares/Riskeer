﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.ClosingStructures.Service;
using Riskeer.Common.Service;
using Riskeer.DuneErosion.Service;
using Riskeer.GrassCoverErosionInwards.Service;
using Riskeer.GrassCoverErosionOutwards.Service;
using Riskeer.HeightStructures.Service;
using Riskeer.Integration.Data;
using Riskeer.MacroStabilityInwards.Service;
using Riskeer.Piping.Service;
using Riskeer.StabilityPointStructures.Service;
using Riskeer.StabilityStoneCover.Service;
using Riskeer.WaveImpactAsphaltCover.Service;

namespace Riskeer.Integration.Service
{
    /// <summary>
    /// This class defines factory methods that can be used to create instances of <see cref="CalculatableActivity"/> for
    /// all calculations in an <see cref="AssessmentSection"/>.
    /// </summary>
    public static class AssessmentSectionCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for all relevant hydraulic load calculations
        /// in the given <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateHydraulicLoadCalculationActivities(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();
            activities.AddRange(AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(
                                    assessmentSection));
            activities.AddRange(StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.StabilityStoneCover,
                                    assessmentSection));
            activities.AddRange(WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.WaveImpactAsphaltCover,
                                    assessmentSection));
            activities.AddRange(GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivities(
                                    assessmentSection.GrassCoverErosionOutwards.CalculationsGroup,
                                    assessmentSection.GrassCoverErosionOutwards,
                                    assessmentSection));
            activities.AddRange(DuneLocationCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.DuneErosion,
                                    assessmentSection));

            return activities;
        }

        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for all relevant calculations
        /// in the given <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateCalculationActivities(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();

            activities.AddRange(PipingCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.Piping,
                                    assessmentSection));
            activities.AddRange(GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.GrassCoverErosionInwards,
                                    assessmentSection));
            activities.AddRange(MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.MacroStabilityInwards,
                                    assessmentSection));
            activities.AddRange(HeightStructuresCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.HeightStructures,
                                    assessmentSection));
            activities.AddRange(ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.ClosingStructures,
                                    assessmentSection));
            activities.AddRange(StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(
                                    assessmentSection.StabilityPointStructures,
                                    assessmentSection));

            return activities;
        }
    }
}
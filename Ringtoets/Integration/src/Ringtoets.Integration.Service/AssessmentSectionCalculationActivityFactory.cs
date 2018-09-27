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
using Ringtoets.ClosingStructures.Service;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Service;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.GrassCoverErosionOutwards.Service;
using Ringtoets.HeightStructures.Service;
using Ringtoets.Integration.Data;
using Ringtoets.MacroStabilityInwards.Service;
using Ringtoets.Piping.Service;
using Ringtoets.StabilityPointStructures.Service;
using Ringtoets.StabilityStoneCover.Service;
using Ringtoets.WaveImpactAsphaltCover.Service;

namespace Ringtoets.Integration.Service
{
    /// <summary>
    /// This class defines factory methods that can be used to create instances of <see cref="CalculatableActivity"/> for
    /// all calculations in an <see cref="AssessmentSection"/>.
    /// </summary>
    public static class AssessmentSectionCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CalculatableActivity"/> for all relevant calculations
        /// in the given <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the activities for.</param>
        /// <returns>A collection of <see cref="CalculatableActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<CalculatableActivity> CreateActivities(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var activities = new List<CalculatableActivity>();
            activities.AddRange(AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(assessmentSection));

            if (assessmentSection.Piping.IsRelevant)
            {
                activities.AddRange(PipingCalculationActivityFactory.CreateCalculationActivities(assessmentSection.Piping, assessmentSection));
            }

            if (assessmentSection.GrassCoverErosionInwards.IsRelevant)
            {
                activities.AddRange(GrassCoverErosionInwardsCalculationActivityFactory.CreateCalculationActivities(assessmentSection.GrassCoverErosionInwards, assessmentSection));
            }

            if (assessmentSection.MacroStabilityInwards.IsRelevant)
            {
                activities.AddRange(MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(assessmentSection.MacroStabilityInwards, assessmentSection));
            }

            if (assessmentSection.StabilityStoneCover.IsRelevant)
            {
                activities.AddRange(StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(assessmentSection.StabilityStoneCover, assessmentSection));
            }

            if (assessmentSection.WaveImpactAsphaltCover.IsRelevant)
            {
                activities.AddRange(WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(assessmentSection.WaveImpactAsphaltCover, assessmentSection));
            }

            if (assessmentSection.GrassCoverErosionOutwards.IsRelevant)
            {
                activities.AddRange(GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivitiesWithoutAssessmentSectionCalculations(assessmentSection.GrassCoverErosionOutwards, assessmentSection));
            }

            if (assessmentSection.HeightStructures.IsRelevant)
            {
                activities.AddRange(HeightStructuresCalculationActivityFactory.CreateCalculationActivities(assessmentSection.HeightStructures, assessmentSection));
            }

            if (assessmentSection.ClosingStructures.IsRelevant)
            {
                activities.AddRange(ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(assessmentSection.ClosingStructures, assessmentSection));
            }

            if (assessmentSection.StabilityPointStructures.IsRelevant)
            {
                activities.AddRange(StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(assessmentSection.StabilityPointStructures, assessmentSection));
            }

            if (assessmentSection.DuneErosion.IsRelevant)
            {
                activities.AddRange(DuneLocationCalculationActivityFactory.CreateCalculationActivities(assessmentSection.DuneErosion, assessmentSection));
            }

            return activities;
        }
    }
}
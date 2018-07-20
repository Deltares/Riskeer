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
using Core.Common.Gui.Commands;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Plugin.Merge
{
    /// <summary>
    /// Class responsible for handling the merge of <see cref="AssessmentSection"/> data.
    /// </summary>
    public class AssessmentSectionMergeHandler : IAssessmentSectionMergeHandler
    {
        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMergeHandler"/>.
        /// </summary>
        /// <param name="viewCommands">The view commands used to close views for the target
        /// <see cref="AssessmentSection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewCommands"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionMergeHandler(IViewCommands viewCommands)
        {
            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.viewCommands = viewCommands;
        }

        public void PerformMerge(AssessmentSection targetAssessmentSection, AssessmentSection sourceAssessmentSection,
                                 IEnumerable<IFailureMechanism> failureMechanismsToMerge)
        {
            if (targetAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(targetAssessmentSection));
            }

            if (sourceAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(sourceAssessmentSection));
            }

            if (failureMechanismsToMerge == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismsToMerge));
            }

            BeforeMerge(targetAssessmentSection);
            MergeHydraulicBoundaryLocations(targetAssessmentSection, sourceAssessmentSection);
        }

        private void MergeHydraulicBoundaryLocations(AssessmentSection targetAssessmentSection, AssessmentSection sourceAssessmentSection)
        {
            for (var i = 0; i < targetAssessmentSection.HydraulicBoundaryDatabase.Locations.Count; i++)
            {
                HydraulicBoundaryLocationCalculation targetCalculation = targetAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(i);
                HydraulicBoundaryLocationCalculation sourceCalculation = sourceAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(i);

                if (ShouldMerge(targetCalculation, sourceCalculation))
                {
                    targetCalculation.Output = sourceCalculation.Output;
                }
            }
        }

        private bool ShouldMerge(HydraulicBoundaryLocationCalculation targetCalculation, HydraulicBoundaryLocationCalculation sourceCalculation)
        {
            bool targetCalculationHasOutput = targetCalculation.HasOutput;
            bool sourceCalculationHasOutput = sourceCalculation.HasOutput;

            if (!targetCalculationHasOutput && !sourceCalculationHasOutput 
                || targetCalculationHasOutput && !sourceCalculationHasOutput
                || targetCalculationHasOutput && targetCalculation.Output.HasGeneralResult && !sourceCalculation.Output.HasGeneralResult)
            {
                return false;
            }

            return true;
        }

        private void BeforeMerge(AssessmentSection assessmentSection)
        {
            viewCommands.RemoveAllViewsForItem(assessmentSection);
        }
    }
}
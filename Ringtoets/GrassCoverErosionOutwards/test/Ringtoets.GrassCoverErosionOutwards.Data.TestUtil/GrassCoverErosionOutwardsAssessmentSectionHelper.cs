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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating assessment sections that can be used for unit tests
    /// related to the grass cover erosion outwards failure mechanism.
    /// </summary>
    public static class GrassCoverErosionOutwardsAssessmentSectionHelper
    {
        /// <summary>
        /// Gets a collection of <see cref="TestCaseData"/> containing an assessment level
        /// configuration for all types of <see cref="FailureMechanismCategoryType"/>.
        /// </summary>
        /// <returns>A collection of <see cref="TestCaseData"/>, in which each item contains:
        /// <list type="bullet">
        /// <item>the configured assessment section;</item>
        /// <item>the configured <see cref="GrassCoverErosionOutwardsFailureMechanism"/>;</item>
        /// <item>the hydraulic boundary location for which the assessment level output has been set;</item>
        /// <item>the category type at stake;</item>
        /// <item>the set assessment level (which makes it the "expected assessment level" given the combination
        /// of the before-mentioned assessment section, failure mechanism, hydraulic boundary location and
        /// category type).</item>
        /// </list>
        /// </returns>
        public static IEnumerable<TestCaseData> GetAssessmentLevelConfigurationPerFailureMechanismCategoryType()
        {
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection,
                new[]
                {
                    hydraulicBoundaryLocation
                }, true);

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificFactorizedSignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificSignalingNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificSignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificLowerLimitNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.LowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("LowerLimitNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.FactorizedLowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("FactorizedLowerLimitNorm");
        }
    }
}
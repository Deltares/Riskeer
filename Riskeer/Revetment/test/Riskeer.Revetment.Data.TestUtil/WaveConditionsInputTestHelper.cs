// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Revetment.Data.TestUtil
{
    /// <summary>
    /// A test helper related to configuring wave conditions input.
    /// </summary>
    public class WaveConditionsInputTestHelper
    {
        /// <summary>
        /// Gets a collection of <see cref="TestCaseData"/> containing an assessment level configuration for all types
        /// of <see cref="WaveConditionsInputWaterLevelType"/>.
        /// </summary>
        /// <returns>A collection of <see cref="TestCaseData"/>, in which each item contains:
        /// <list type="bullet">
        /// <item>the configured assessment section;</item>
        /// <item>a method for configuring the wave conditions input at stake;</item>
        /// <item>the expected assessment level given the combination of the before-mentioned assessment section, the configured
        /// hydraulic boundary location and the configured water level type.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<TestCaseData> GetAssessmentLevelConfigurationPerWaterLevelType()
        {
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            var assessmentSection = new AssessmentSectionStub
            {
                WaterLevelCalculationsForUserDefinedTargetProbabilities =
                {
                    calculationsForTargetProbability
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            yield return new TestCaseData(
                assessmentSection,
                new Action<WaveConditionsInput>(input =>
                {
                    input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;
                    input.WaterLevelType = WaveConditionsInputWaterLevelType.None;
                }),
                RoundedDouble.NaN);

            yield return new TestCaseData(
                assessmentSection,
                new Action<WaveConditionsInput>(input =>
                {
                    input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;
                    input.WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit;
                }),
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First().Output.Result);

            yield return new TestCaseData(
                assessmentSection,
                new Action<WaveConditionsInput>(input =>
                {
                    input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;
                    input.WaterLevelType = WaveConditionsInputWaterLevelType.Signaling;
                }),
                assessmentSection.WaterLevelCalculationsForSignalingNorm.First().Output.Result);

            yield return new TestCaseData(
                assessmentSection,
                new Action<WaveConditionsInput>(input =>
                {
                    input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;
                    input.WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability;
                    input.CalculationsTargetProbability = calculationsForTargetProbability;
                }),
                calculationsForTargetProbability.HydraulicBoundaryLocationCalculations.First().Output.Result);
        }
    }
}
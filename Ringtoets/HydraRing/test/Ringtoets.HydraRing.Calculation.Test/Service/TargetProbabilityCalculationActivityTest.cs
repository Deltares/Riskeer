// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Base.Service;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Service;

namespace Ringtoets.HydraRing.Calculation.Test.Service
{
    [TestFixture]
    public class TargetProbabilityCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var targetProbabilityCalculationInputImplementation = new TargetProbabilityCalculationInputImplementation(1, 10000);

            // Call
            var activity = new TargetProbabilityCalculationActivity("Name of activity", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInputImplementation, output => { });

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual("Name of activity", activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        private class TargetProbabilityCalculationInputImplementation : TargetProbabilityCalculationInput
        {
            public TargetProbabilityCalculationInputImplementation(int hydraulicBoundaryLocationId, double norm) : base(hydraulicBoundaryLocationId, norm) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return HydraRingFailureMechanismType.QVariant;
                }
            }

            public override HydraRingDikeSection DikeSection
            {
                get
                {
                    return new HydraRingDikeSection(1, "Name", 2.2, 3.3, 4.4, 5.5, 6.6, 7.7);
                }
            }
        }
    }
}

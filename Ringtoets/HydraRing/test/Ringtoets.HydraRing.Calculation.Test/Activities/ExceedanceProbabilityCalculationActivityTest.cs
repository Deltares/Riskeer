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
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Activities
{
    [TestFixture]
    public class ExceedanceProbabilityCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var hydraRingCalculationService = mocks.StrictMock<HydraRingCalculationService>();
            var exceedanceProbabilityCalculationInput = mocks.StrictMock<ExceedanceProbabilityCalculationInput>(10000);

            mocks.ReplayAll();

            // Call
            var activity = new ExceedanceProbabilityCalculationActivity("Name of activity", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, exceedanceProbabilityCalculationInput, null, null, hydraRingCalculationService);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual("Name of activity", activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void Run_Always_BeforeRunActionPerformedAsExpected()
        {
            // Setup
            var count = 0;
            var mocks = new MockRepository();
            var hydraRingCalculationService = mocks.StrictMock<HydraRingCalculationService>();
            var exceedanceProbabilityCalculationInput = mocks.StrictMock<ExceedanceProbabilityCalculationInput>(10000);

            mocks.ReplayAll();

            var activity = new ExceedanceProbabilityCalculationActivity("Name of activity", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, exceedanceProbabilityCalculationInput, () => { count++; }, null, hydraRingCalculationService);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(1, count);
        }

        [TestCase(ActivityState.None, false)]
        [TestCase(ActivityState.Executed, true)]
        [TestCase(ActivityState.Cancelled, false)]
        [TestCase(ActivityState.Failed, false)]
        [TestCase(ActivityState.Finished, false)]
        public void Finish_ExceedanceProbabilityCalculationActivityWithSpecificState_OutputActionPerformedAsExpected(ActivityState state, bool outputActionPerformed)
        {
            // Setup
            var count = 0;
            var mocks = new MockRepository();
            var hydraRingCalculationService = mocks.StrictMock<HydraRingCalculationService>();
            var exceedanceProbabilityCalculationInput = mocks.StrictMock<ExceedanceProbabilityCalculationInput>(10000);

            mocks.ReplayAll();

            var activity = new ExceedanceProbabilityCalculationActivity("Name of activity", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, exceedanceProbabilityCalculationInput, null, output => { count++; }, hydraRingCalculationService);

            TypeUtils.SetPrivatePropertyValue(activity, "State", state);

            // Call
            activity.Finish();

            // Assert
            Assert.AreEqual(outputActionPerformed, count > 0);
        }
    }
}
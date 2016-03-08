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
using Rhino.Mocks;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Activities
{
    [TestFixture]
    public class TargetProbabilityCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var hydraRingCalculationService = mocks.StrictMock<HydraRingCalculationService>();
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 10000);

            mocks.ReplayAll();

            // Call
            var activity = new TargetProbabilityCalculationActivity("Name of activity", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput, null, hydraRingCalculationService);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual("Name of activity", activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void Run_TargetProbabilityCalculationActivity_PerformCalculationCalledWithCorrectParameters()
        {
            // Setup
            var mocks = new MockRepository();
            var hydraRingCalculationService = mocks.StrictMock<HydraRingCalculationService>();
            var targetProbabilityCalculationOutput = mocks.StrictMock<TargetProbabilityCalculationOutput>(1.1, 2.2);
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 10000);

            const string hlcdDirectory = "hlcdDirectory";
            const string ringId = "ringId";
            const HydraRingUncertaintiesType uncertaintiesType = HydraRingUncertaintiesType.All;
            const HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType = HydraRingTimeIntegrationSchemeType.FBC;

            hydraRingCalculationService.Expect(hcs => hcs.PerformCalculation(hlcdDirectory, ringId, timeIntegrationSchemeType, uncertaintiesType, targetProbabilityCalculationInput)).Return(targetProbabilityCalculationOutput);

            mocks.ReplayAll();

            var activity = new TargetProbabilityCalculationActivity("Name of activity", hlcdDirectory, ringId, timeIntegrationSchemeType, uncertaintiesType, targetProbabilityCalculationInput, null, hydraRingCalculationService);

            // Call
            activity.Run();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Cancel_TargetProbabilityCalculationActivity_CancelRunningCalculationCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var hydraRingCalculationService = mocks.StrictMock<HydraRingCalculationService>();
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 10000);

            hydraRingCalculationService.Expect(hcs => hcs.CancelRunningCalculation());

            mocks.ReplayAll();

            var activity = new TargetProbabilityCalculationActivity("Name of activity", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput, null, hydraRingCalculationService);

            // Call
            activity.Cancel();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_TargetProbabilityCalculationActivity_OutputActionPerformed()
        {
            // Setup
            var count = 0;
            var mocks = new MockRepository();
            var hydraRingCalculationService = mocks.StrictMock<HydraRingCalculationService>();
            var targetProbabilityCalculationInput = mocks.StrictMock<TargetProbabilityCalculationInput>(1, 10000);

            mocks.ReplayAll();

            var activity = new TargetProbabilityCalculationActivity("Name of activity", "hlcdDirectory", "ringId", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, targetProbabilityCalculationInput, output => { count++; }, hydraRingCalculationService);

            // Call
            activity.Finish();

            // Assert
            Assert.AreEqual(1, count);
        }
    }
}
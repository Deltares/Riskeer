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

using System.Linq;
using Core.Common.Base;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Merge;

namespace Ringtoets.Integration.Plugin.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeHandlerStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new AssessmentSectionMergeHandlerStub();

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionMergeHandler>(handler);
        }

        [Test]
        public void PerformMerge_Always_ReplaceOutputOfAllLocationsAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            IObserver waterLevelForFactorizedSignalingNormObserver = CreateObserver(mocks);
            IObserver waterLevelForSignalingNormObserver = CreateObserver(mocks);
            IObserver waterLevelForLowerLimitNormObserver = CreateObserver(mocks);
            IObserver waterLevelForFactorizedLowerLimitNormObserver = CreateObserver(mocks);
            IObserver waveHeightForFactorizedSignalingNormObserver = CreateObserver(mocks);
            IObserver waveHeightForSignalingNormObserver = CreateObserver(mocks);
            IObserver waveHeightForLowerLimitNormObserver = CreateObserver(mocks);
            IObserver waveHeightForFactorizedLowerLimitNormObserver = CreateObserver(mocks);
            mocks.ReplayAll();

            var locations = new HydraulicBoundaryLocation[]
            {
                new TestHydraulicBoundaryLocation("1"),
                new TestHydraulicBoundaryLocation("2"),
                new TestHydraulicBoundaryLocation("3")
            };

            AssessmentSection originalAssessmentSection = CreateAssessmentSection(locations);
            originalAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ForEachElementDo(c => c.Attach(waterLevelForFactorizedSignalingNormObserver));
            originalAssessmentSection.WaterLevelCalculationsForSignalingNorm.ForEachElementDo(c => c.Attach(waterLevelForSignalingNormObserver));
            originalAssessmentSection.WaterLevelCalculationsForLowerLimitNorm.ForEachElementDo(c => c.Attach(waterLevelForLowerLimitNormObserver));
            originalAssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ForEachElementDo(c => c.Attach(waterLevelForFactorizedLowerLimitNormObserver));
            originalAssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ForEachElementDo(c => c.Attach(waveHeightForFactorizedSignalingNormObserver));
            originalAssessmentSection.WaveHeightCalculationsForSignalingNorm.ForEachElementDo(c => c.Attach(waveHeightForSignalingNormObserver));
            originalAssessmentSection.WaveHeightCalculationsForLowerLimitNorm.ForEachElementDo(c => c.Attach(waveHeightForLowerLimitNormObserver));
            originalAssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ForEachElementDo(c => c.Attach(waveHeightForFactorizedLowerLimitNormObserver));

            AssessmentSection newAssessmentSection = CreateAssessmentSection(locations);
            SetOutput(newAssessmentSection);

            var handler = new AssessmentSectionMergeHandlerStub();

            // Precondition
            Assert.IsTrue(originalAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => !c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaterLevelCalculationsForSignalingNorm.All(c => !c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaterLevelCalculationsForLowerLimitNorm.All(c => !c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.All(c => !c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.All(c => !c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaveHeightCalculationsForSignalingNorm.All(c => !c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaveHeightCalculationsForLowerLimitNorm.All(c => !c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.All(c => !c.HasOutput));

            // Call
            handler.PerformMerge(originalAssessmentSection, newAssessmentSection, null);

            // Assert
            Assert.IsTrue(originalAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaterLevelCalculationsForSignalingNorm.All(c => c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaterLevelCalculationsForLowerLimitNorm.All(c => c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.All(c => c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaveHeightCalculationsForSignalingNorm.All(c => c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaveHeightCalculationsForLowerLimitNorm.All(c => c.HasOutput));
            Assert.IsTrue(originalAssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.All(c => c.HasOutput));
            mocks.VerifyAll();
        }

        private static IObserver CreateObserver(MockRepository mocks)
        {
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(3);
            return observer;
        }

        private static AssessmentSection CreateAssessmentSection(HydraulicBoundaryLocation[] locations)
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "test"
                }
            };
            assessmentSection.HydraulicBoundaryDatabase.Locations.AddRange(locations);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations);
            return assessmentSection;
        }

        private static void SetOutput(AssessmentSection assessmentSection)
        {
            for (var i = 0; i < assessmentSection.HydraulicBoundaryDatabase.Locations.Count; i++)
            {
                assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(i).Output = new TestHydraulicBoundaryLocationCalculationOutput();
                assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(i).Output = new TestHydraulicBoundaryLocationCalculationOutput();
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(i).Output = new TestHydraulicBoundaryLocationCalculationOutput();
                assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(i).Output = new TestHydraulicBoundaryLocationCalculationOutput();

                assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ElementAt(i).Output = new TestHydraulicBoundaryLocationCalculationOutput();
                assessmentSection.WaveHeightCalculationsForSignalingNorm.ElementAt(i).Output = new TestHydraulicBoundaryLocationCalculationOutput();
                assessmentSection.WaveHeightCalculationsForLowerLimitNorm.ElementAt(i).Output = new TestHydraulicBoundaryLocationCalculationOutput();
                assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ElementAt(i).Output = new TestHydraulicBoundaryLocationCalculationOutput();
            }
        }
    }
}
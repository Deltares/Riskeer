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
using System.Linq;
using Core.Common.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Merge;

namespace Ringtoets.Integration.Plugin.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeHandlerTest
    {
        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("viewCommands", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionMergeHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_targetAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Call
            TestDelegate call = () => handler.PerformMerge(null, new AssessmentSection(AssessmentSectionComposition.Dike),
                                                           Enumerable.Empty<IFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("targetAssessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_sourceAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Call
            TestDelegate call = () => handler.PerformMerge(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                           null, Enumerable.Empty<IFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceAssessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_FailureMechanismsToMergeNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Call
            TestDelegate call = () => handler.PerformMerge(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                           new AssessmentSection(AssessmentSectionComposition.Dike),
                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismsToMerge", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_WithAllData_AllViewsForTargetAssessmentSectionClosed()
        {
            // Setup
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(targetAssessmentSection));
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Call
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSection(AssessmentSectionComposition.Dike),
                                 Enumerable.Empty<IFailureMechanism>());

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenTargetAssessmentSectionHasOutput_ThenCalculationsNotChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            foreach (HydraulicBoundaryLocationCalculation calculation in targetAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm)
            {
                calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            }

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Precondition
            Assert.IsTrue(targetAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput));
            Assert.IsTrue(sourceAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => !c.HasOutput));

            // Call
            handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, Enumerable.Empty<IFailureMechanism>());

            // Assert
            Assert.IsTrue(targetAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput));
            Assert.IsTrue(sourceAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => !c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenSourceAssessmentSectionHasOutput_ThenCalculationDataMerged()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            foreach (HydraulicBoundaryLocationCalculation calculation in sourceAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm)
            {
                calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            }

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Precondition
            Assert.IsTrue(targetAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => !c.HasOutput));
            Assert.IsTrue(sourceAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput));

            // Call
            handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, Enumerable.Empty<IFailureMechanism>());

            // Assert
            Assert.IsTrue(targetAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput));
            mocks.VerifyAll();
        }

        private static AssessmentSection CreateAssessmentSection(TestHydraulicBoundaryLocation[] locations)
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.HydraulicBoundaryDatabase.Locations.AddRange(locations);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations);
            return assessmentSection;
        }
    }
}
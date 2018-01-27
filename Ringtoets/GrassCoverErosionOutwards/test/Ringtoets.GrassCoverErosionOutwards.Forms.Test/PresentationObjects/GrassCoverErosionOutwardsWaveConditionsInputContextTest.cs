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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var input = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            // Call
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(input,
                                                                                  calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(input, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(failureMechanism.HydraulicBoundaryLocations, context.HydraulicBoundaryLocations);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, context.ForeshoreProfiles);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsInputContext(input,
                                                                                              calculation,
                                                                                              null,
                                                                                              assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }
    }
}
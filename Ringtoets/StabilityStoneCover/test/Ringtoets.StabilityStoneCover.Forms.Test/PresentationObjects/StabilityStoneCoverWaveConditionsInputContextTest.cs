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

using System;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var foreshoreProfiles = new[]
            {
                new TestForeshoreProfile()
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            // Call
            var context = new StabilityStoneCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                            calculation,
                                                                            foreshoreProfiles,
                                                                            assessmentSection);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(calculation.InputParameters, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(foreshoreProfiles, context.ForeshoreProfiles);
            Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, context.HydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                                        calculation,
                                                                                        null,
                                                                                        assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var foreshoreProfiles = new[]
            {
                new TestForeshoreProfile()
            };

            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                                        calculation,
                                                                                        foreshoreProfiles,
                                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}
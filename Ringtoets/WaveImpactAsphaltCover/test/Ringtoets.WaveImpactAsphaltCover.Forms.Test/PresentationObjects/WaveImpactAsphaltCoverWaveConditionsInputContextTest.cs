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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var foreshoreProfiles = new[]
            {
                new TestForeshoreProfile()
            };

            var assessmentSection = new ObservableTestAssessmentSectionStub();

            // Call
            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                               calculation,
                                                                               assessmentSection,
                                                                               foreshoreProfiles);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(calculation.InputParameters, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(foreshoreProfiles, context.ForeshoreProfiles);
            Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, context.HydraulicBoundaryLocations);
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new WaveImpactAsphaltCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                                           calculation,
                                                                                           assessmentSection,
                                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
            mocks.VerifyAll();
        }
    }
}
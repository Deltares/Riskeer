﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.Revetment.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var assessmentSection = new AssessmentSectionStub();
            IEnumerable<ForeshoreProfile> foreshoreProfiles = Enumerable.Empty<ForeshoreProfile>();

            // Call
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(calculation.InputParameters,
                                                                                  calculation,
                                                                                  assessmentSection,
                                                                                  foreshoreProfiles);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext<GrassCoverErosionOutwardsWaveConditionsInput>>(context);
            Assert.AreSame(calculation.InputParameters, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(foreshoreProfiles, context.ForeshoreProfiles);
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionOutwardsWaveConditionsInputContext(calculation.InputParameters, calculation, assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
            mocks.VerifyAll();
        }
    }
}
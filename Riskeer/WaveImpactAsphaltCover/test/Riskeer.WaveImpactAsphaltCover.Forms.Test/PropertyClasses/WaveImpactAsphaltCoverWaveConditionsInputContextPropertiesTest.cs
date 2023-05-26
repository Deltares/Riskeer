﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.PropertyClasses;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Riskeer.WaveImpactAsphaltCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsInputContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                Enumerable.Empty<ForeshoreProfile>());

            // Call
            var properties = new WaveImpactAsphaltCoverWaveConditionsInputContextProperties(
                context, AssessmentSectionTestHelper.GetTestAssessmentLevel, handler);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContextProperties<WaveImpactAsphaltCoverWaveConditionsInputContext,
                WaveConditionsInput, string>>(properties);
            Assert.AreSame(context, properties.Data);
            Assert.AreEqual("Asfalt", properties.RevetmentType);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RevetmentType_SetNewValue_ThrowsInvalidOperationException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                Enumerable.Empty<ForeshoreProfile>());

            var properties = new WaveImpactAsphaltCoverWaveConditionsInputContextProperties(
                context, AssessmentSectionTestHelper.GetTestAssessmentLevel, handler);

            // Call
            TestDelegate test = () => properties.RevetmentType = string.Empty;

            // Assert
            Assert.Throws<InvalidOperationException>(test);
            mockRepository.VerifyAll();
        }
    }
}
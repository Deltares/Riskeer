// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Riskeer.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssessmentSectionCategoryWaveConditionsInputContextPropertiesTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble assessmentLevel = random.NextRoundedDouble();

            var assessmentSection = new AssessmentSectionStub();
            var input = new AssessmentSectionCategoryWaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation<AssessmentSectionCategoryWaveConditionsInput>(input);
            var inputContext = new TestAssessmentSectionCategoryWaveConditionsInputContext(input, calculation, assessmentSection);

            // Call
            var properties = new TestAssessmentSectionCategoryWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContextProperties<
                TestAssessmentSectionCategoryWaveConditionsInputContext,
                AssessmentSectionCategoryWaveConditionsInput,
                AssessmentSectionCategoryType>>(properties);
            Assert.AreSame(properties.Data, inputContext);
            Assert.AreEqual(assessmentLevel.Value, properties.AssessmentLevel.Value, properties.AssessmentLevel.GetAccuracy());
            mocks.VerifyAll();
        }

        [Test]
        public void GivenWaveConditionsInput_WhenGettingCategoryTypeFromProperties_ThenCategoryTypeOfInputReturned()
        {
            // Given
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble assessmentLevel = random.NextRoundedDouble();

            var assessmentSection = new AssessmentSectionStub();
            var input = new AssessmentSectionCategoryWaveConditionsInput
            {
                CategoryType = random.NextEnumValue<AssessmentSectionCategoryType>()
            };
            var calculation = new TestWaveConditionsCalculation<AssessmentSectionCategoryWaveConditionsInput>(input);
            var inputContext = new TestAssessmentSectionCategoryWaveConditionsInputContext(input, calculation, assessmentSection);
            var properties = new TestAssessmentSectionCategoryWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);

            // When
            AssessmentSectionCategoryType categoryType = properties.CategoryType;

            // Then
            Assert.AreEqual(input.CategoryType, categoryType);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenWaveConditionsInput_WhenSettingCategoryTypeOfProperties_ThenCategoryTypeSetToInput()
        {
            // Given
            var handler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());

            var random = new Random(21);
            RoundedDouble assessmentLevel = random.NextRoundedDouble();

            var assessmentSection = new AssessmentSectionStub();
            var input = new AssessmentSectionCategoryWaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation<AssessmentSectionCategoryWaveConditionsInput>(input);
            var inputContext = new TestAssessmentSectionCategoryWaveConditionsInputContext(input, calculation, assessmentSection);
            var properties = new TestAssessmentSectionCategoryWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);
            var newAssessmentSectionCategoryType = random.NextEnumValue<AssessmentSectionCategoryType>();

            // When
            properties.CategoryType = newAssessmentSectionCategoryType;

            // Then
            Assert.AreEqual(newAssessmentSectionCategoryType, input.CategoryType);
        }

        private class TestAssessmentSectionCategoryWaveConditionsInputContextProperties
            : AssessmentSectionCategoryWaveConditionsInputContextProperties<TestAssessmentSectionCategoryWaveConditionsInputContext>
        {
            public TestAssessmentSectionCategoryWaveConditionsInputContextProperties(TestAssessmentSectionCategoryWaveConditionsInputContext context,
                                                                                     Func<RoundedDouble> getAssessmentLevelFunc,
                                                                                     IObservablePropertyChangeHandler propertyChangeHandler)
                : base(context,
                       getAssessmentLevelFunc,
                       propertyChangeHandler) {}

            public override string RevetmentType
            {
                get
                {
                    return "";
                }
            }
        }

        private class TestAssessmentSectionCategoryWaveConditionsInputContext
            : WaveConditionsInputContext<AssessmentSectionCategoryWaveConditionsInput>
        {
            public TestAssessmentSectionCategoryWaveConditionsInputContext(AssessmentSectionCategoryWaveConditionsInput wrappedData,
                                                                           ICalculation<AssessmentSectionCategoryWaveConditionsInput> calculation,
                                                                           IAssessmentSection assessmentSection)
                : base(wrappedData,
                       calculation,
                       assessmentSection) {}

            public override IEnumerable<ForeshoreProfile> ForeshoreProfiles
            {
                get
                {
                    return new ForeshoreProfile[0];
                }
            }
        }
    }
}
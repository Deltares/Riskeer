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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismCategoryWaveConditionsInputContextPropertiesTest
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
            var input = new FailureMechanismCategoryWaveConditionsInput();
            var calculation = new TestFailureMechanismCategoryWaveConditionsCalculation();
            var inputContext = new TestFailureMechanismCategoryWaveConditionsInputContext(input, calculation, assessmentSection);

            // Call
            var properties = new TestFailureMechanismCategoryWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContextProperties<
                TestFailureMechanismCategoryWaveConditionsInputContext,
                FailureMechanismCategoryWaveConditionsInput,
                FailureMechanismCategoryType>>(properties);
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
            var input = new FailureMechanismCategoryWaveConditionsInput
            {
                CategoryType = random.NextEnumValue<FailureMechanismCategoryType>()
            };
            var calculation = new TestFailureMechanismCategoryWaveConditionsCalculation();
            var inputContext = new TestFailureMechanismCategoryWaveConditionsInputContext(input, calculation, assessmentSection);
            var properties = new TestFailureMechanismCategoryWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);

            // When
            FailureMechanismCategoryType categoryType = properties.CategoryType;

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
            var input = new FailureMechanismCategoryWaveConditionsInput();
            var calculation = new TestFailureMechanismCategoryWaveConditionsCalculation();
            var inputContext = new TestFailureMechanismCategoryWaveConditionsInputContext(input, calculation, assessmentSection);
            var properties = new TestFailureMechanismCategoryWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);
            var newFailureMechanismCategoryType = random.NextEnumValue<FailureMechanismCategoryType>();

            // When
            properties.CategoryType = newFailureMechanismCategoryType;

            // Then
            Assert.AreEqual(newFailureMechanismCategoryType, input.CategoryType);
        }

        private class TestFailureMechanismCategoryWaveConditionsInputContextProperties
            : FailureMechanismCategoryWaveConditionsInputContextProperties<TestFailureMechanismCategoryWaveConditionsInputContext>
        {
            public TestFailureMechanismCategoryWaveConditionsInputContextProperties(TestFailureMechanismCategoryWaveConditionsInputContext context,
                                                                                    Func<RoundedDouble> getNormativeAssessmentLevelFunc,
                                                                                    IObservablePropertyChangeHandler propertyChangeHandler)
                : base(context,
                       getNormativeAssessmentLevelFunc,
                       propertyChangeHandler) {}

            public override string RevetmentType
            {
                get
                {
                    return "";
                }
            }
        }

        private class TestFailureMechanismCategoryWaveConditionsInputContext
            : WaveConditionsInputContext<FailureMechanismCategoryWaveConditionsInput>
        {
            public TestFailureMechanismCategoryWaveConditionsInputContext(FailureMechanismCategoryWaveConditionsInput wrappedData,
                                                                          ICalculation<FailureMechanismCategoryWaveConditionsInput> calculation,
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

        private class TestFailureMechanismCategoryWaveConditionsCalculation : ICalculation<FailureMechanismCategoryWaveConditionsInput>
        {
            public IEnumerable<IObserver> Observers { get; }

            public string Name { get; set; }

            public bool HasOutput { get; }

            public Comment Comments { get; }

            public FailureMechanismCategoryWaveConditionsInput InputParameters { get; }

            public void Attach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void Detach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void NotifyObservers()
            {
                throw new NotImplementedException();
            }

            public object Clone()
            {
                throw new NotImplementedException();
            }

            public void ClearOutput()
            {
                throw new NotImplementedException();
            }
        }
    }
}
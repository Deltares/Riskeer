// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Base;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssessmentSectionPropertiesTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionCompositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new AssessmentSectionProperties(null, assessmentSectionCompositionChangeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionCompositionChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new AssessmentSectionProperties(assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("compositionChangeHandler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Id).Return("1");
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            var assessmentSectionCompositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            assessmentSection.Name = "test";

            // Call
            var properties = new AssessmentSectionProperties(assessmentSection, assessmentSectionCompositionChangeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IAssessmentSection>>(properties);
            Assert.AreSame(assessmentSection, properties.Data);
            Assert.AreEqual(assessmentSection.Id, properties.Id);
            Assert.AreEqual(assessmentSection.Name, properties.Name);
            Assert.AreEqual(assessmentSection.Composition, properties.Composition);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var assessmentSectionCompositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new AssessmentSectionProperties(assessmentSection, assessmentSectionCompositionChangeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor idProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            generalCategoryName,
                                                                            "ID",
                                                                            "ID van het traject.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "Naam van het traject.");

            PropertyDescriptor compositionProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(compositionProperty,
                                                                            generalCategoryName,
                                                                            "Trajecttype",
                                                                            "Selecteert het trajecttype.");
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionProperties_WhenChangingName_ThenUpdatesDataAndNotifiesObservers()
        {
            // Given
            const string newName = "Test";

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers());
            var assessmentSectionCompositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            var properties = new AssessmentSectionProperties(assessmentSection, assessmentSectionCompositionChangeHandler);

            // When
            properties.Name = newName;

            // Then
            Assert.AreEqual(newName, assessmentSection.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionProperties_WhenChangingCompositionValue_ThenCompositionSetAndNotifiesObserver()
        {
            // Given
            const AssessmentSectionComposition newComposition = AssessmentSectionComposition.DikeAndDune;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.FailureMechanismContribution).Return(new FailureMechanismContribution(0.1, 0.1));

            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            var assessmentSectionCompositionChangeHandler = mocks.StrictMock<IAssessmentSectionCompositionChangeHandler>();
            assessmentSectionCompositionChangeHandler.Expect(handler => handler.ChangeComposition(assessmentSection, newComposition))
                                                     .Return(new[]
                                                     {
                                                         observable
                                                     });
            mocks.ReplayAll();

            var properties = new AssessmentSectionProperties(assessmentSection, assessmentSectionCompositionChangeHandler);

            // When
            properties.Composition = newComposition;

            // Then
            mocks.VerifyAll();
        }
    }
}
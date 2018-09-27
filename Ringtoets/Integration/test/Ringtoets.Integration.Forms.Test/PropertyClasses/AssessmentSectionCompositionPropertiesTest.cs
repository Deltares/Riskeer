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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Plugin.Handlers;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssessmentSectionCompositionPropertiesTest : NUnitFormTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new AssessmentSectionCompositionProperties(null, assessmentSectionChangeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
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
            TestDelegate test = () => new AssessmentSectionCompositionProperties(assessmentSection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("compositionChangeHandler", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var assessmentSectionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new AssessmentSectionCompositionProperties(assessmentSection, assessmentSectionChangeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IAssessmentSection>>(properties);
            Assert.AreSame(assessmentSection, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var assessmentSectionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new AssessmentSectionCompositionProperties(assessmentSection, assessmentSectionChangeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(1, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";

            PropertyDescriptor compositionProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(compositionProperty,
                                                                            expectedCategory,
                                                                            "Trajecttype",
                                                                            "Selecteer het type traject, bepalend voor de faalkansbegroting.");
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            const AssessmentSectionComposition composition = AssessmentSectionComposition.DikeAndDune;
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Composition).Return(composition);
            var assessmentSectionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new AssessmentSectionCompositionProperties(assessmentSection, assessmentSectionChangeHandler);

            // Assert
            Assert.AreEqual(composition, properties.Composition);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionCompositionProperties_WhenConfirmingCompositionValueChange_ThenCompositionSetAndNotifiesObserver()
        {
            // Given
            const AssessmentSectionComposition originalComposition = AssessmentSectionComposition.Dike;
            var assessmentSection = new AssessmentSection(originalComposition);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);
            observer.Expect(o => o.UpdateObserver());
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var compositionChangeHandler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            var properties = new AssessmentSectionCompositionProperties(assessmentSection, compositionChangeHandler);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBox.ClickOk();
            };

            // When
            const AssessmentSectionComposition newComposition = AssessmentSectionComposition.DikeAndDune;
            properties.Composition = newComposition;

            // Then
            Assert.AreEqual(newComposition, properties.Composition);
            Assert.AreEqual(newComposition, assessmentSection.Composition);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionCompositionProperties_WhenCancelingCompositionValueChange_ThenDataSameAndObserversNotNotified()
        {
            // Given
            const AssessmentSectionComposition originalComposition = AssessmentSectionComposition.Dike;
            var assessmentSection = new AssessmentSection(originalComposition);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);

            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var compositionChangeHandler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            var properties = new AssessmentSectionCompositionProperties(assessmentSection, compositionChangeHandler);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBox.ClickCancel();
            };

            // When
            const AssessmentSectionComposition newComposition = AssessmentSectionComposition.DikeAndDune;
            properties.Composition = newComposition;

            // Then
            Assert.AreEqual(originalComposition, properties.Composition);
            Assert.AreEqual(originalComposition, assessmentSection.Composition);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void Composition_ValueChanges_NotifiesChangedObjectsInAssessmentSection(
            AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(initialComposition);

            var mocks = new MockRepository();
            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var compositionChangeHandler = mocks.StrictMock<IAssessmentSectionCompositionChangeHandler>();
            compositionChangeHandler.Expect(h => h.ConfirmCompositionChange())
                                    .Return(true);
            compositionChangeHandler.Expect(h => h.ChangeComposition(assessmentSection, newComposition))
                                    .Return(new[]
                                    {
                                        observable1,
                                        observable2
                                    });
            mocks.ReplayAll();

            var properties = new AssessmentSectionCompositionProperties(assessmentSection, compositionChangeHandler);

            // Call
            properties.Composition = newComposition;

            // Assert
            mocks.VerifyAll();
        }
    }
}
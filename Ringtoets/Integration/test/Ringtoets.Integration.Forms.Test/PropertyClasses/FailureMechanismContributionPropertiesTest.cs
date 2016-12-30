// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Plugin.Handlers;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismContributionPropertiesTest : NUnitFormTest
    {
        [Test]
        public void Constructor_WithoutFailureMechanismContribution_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismChangeHandler = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var assessmentSectionChangeHandler = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismContributionProperties(
                null,
                assessmentSection,
                failureMechanismChangeHandler,
                assessmentSectionChangeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanismContribution", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var failureMechanismChangeHandler = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var assessmentSectionChangeHandler = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), double.NaN, 0);

            // Call
            TestDelegate test = () => new FailureMechanismContributionProperties(
                failureMechanismContribution,
                null,
                failureMechanismChangeHandler,
                assessmentSectionChangeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutFailureMechanismContributionNormChangeHandler_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var assessmentSectionChangeHandler = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), double.NaN, 0);

            // Call
            TestDelegate test = () => new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                null,
                assessmentSectionChangeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("normChangeHandler", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutAssessmentSectionCompositionChangeHandler_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismChangeHandler = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), double.NaN, 0);

            // Call
            TestDelegate test = () => new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                failureMechanismChangeHandler,
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("compositionChangeHandler", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithValidParameters_DataSet()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var failureMechanismChangeHandler = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var assessmentSectionChangeHandler = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            mockRepository.ReplayAll();

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), double.NaN, 0);

            // Call
            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                failureMechanismChangeHandler,
                assessmentSectionChangeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismContribution>>(properties);
            Assert.AreSame(failureMechanismContribution, properties.Data);

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            Assert.AreEqual(2, dynamicProperties.Count);

            var expectedCategory = "Algemeen";

            PropertyDescriptor compositionProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(compositionProperty,
                                                                            expectedCategory,
                                                                            "Trajecttype",
                                                                            "Selecteer het type traject, bepalend voor de faalkansbegroting.");

            PropertyDescriptor returnPeriodProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(returnPeriodProperty,
                                                                            expectedCategory,
                                                                            "Norm (terugkeertijd) [jaar]",
                                                                            "Terugkeertijd van de norm, gelijk aan 1/norm.");
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionComposition = AssessmentSectionComposition.DikeAndDune;
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Composition).Return(assessmentSectionComposition);
            var failureMechanismChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var assessmentSectionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            int returnPeriod = 30000;
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1.1, 1.0/returnPeriod);

            // Call
            var properties = new FailureMechanismContributionProperties(
                contribution,
                assessmentSection,
                failureMechanismChangeHandler,
                assessmentSectionChangeHandler);

            // Assert
            Assert.AreEqual(returnPeriod, properties.ReturnPeriod);
            Assert.AreEqual(assessmentSectionComposition, properties.AssessmentSectionComposition);
            mocks.VerifyAll();
        }
        
        [Test]
        public void GivenReturnPeriod_WhenConfirmingReturnPeriodValueChange_ThenReturnPeriodSetAndNotifiesObserver()
        {
            // Given
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var normChangeHandler = new FailureMechanismContributionNormChangeHandler();

            var mocks = new MockRepository();
            var compositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                normChangeHandler,
                compositionChangeHandler);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBox.ClickOk();
            };

            // When
            const int newReturnPeriod = 200;
            properties.ReturnPeriod = newReturnPeriod;

            // Then
            Assert.AreEqual(newReturnPeriod, properties.ReturnPeriod);
            Assert.AreEqual(1.0/newReturnPeriod, failureMechanismContribution.Norm);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenReturnPeriod_WhenCancellingReturnPeriodValueChange_ThenDataSameObserversNotNotified()
        {
            // Given
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            int originalReturnPeriod = Convert.ToInt32(1/failureMechanismContribution.Norm);

            var normChangeHandler = new FailureMechanismContributionNormChangeHandler();

            var mocks = new MockRepository();
            var compositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);
            mocks.ReplayAll();

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                normChangeHandler,
                compositionChangeHandler);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBox.ClickCancel();
            };

            // When
            const int newReturnPeriod = 200;
            properties.ReturnPeriod = newReturnPeriod;

            // Then
            Assert.AreEqual(originalReturnPeriod, properties.ReturnPeriod);
            Assert.AreEqual(1.0/originalReturnPeriod, failureMechanismContribution.Norm);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionComposition_WhenConfirmingCompositionValueChange_ThenAssessmentSectionCompositionSetAndNotifiesObserver()
        {
            // Given
            const AssessmentSectionComposition originalComposition = AssessmentSectionComposition.Dike;
            AssessmentSection assessmentSection = new AssessmentSection(originalComposition);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var compositionChangeHandler = new AssessmentSectionCompositionChangeHandler();

            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();

            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                normChangeHandler,
                compositionChangeHandler);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBox.ClickOk();
            };

            // When
            const AssessmentSectionComposition newComposition = AssessmentSectionComposition.DikeAndDune;
            properties.AssessmentSectionComposition = newComposition;

            // Then
            Assert.AreEqual(newComposition, properties.AssessmentSectionComposition);
            Assert.AreEqual(newComposition, assessmentSection.Composition);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionComposition_WhenCancellingCompositionValueChange__ThenDataSameObserversNotNotified()
        {
            // Given
            const AssessmentSectionComposition originalComposition = AssessmentSectionComposition.Dike;
            AssessmentSection assessmentSection = new AssessmentSection(originalComposition);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var compositionChangeHandler = new AssessmentSectionCompositionChangeHandler();

            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();

            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);
            mocks.ReplayAll();

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                normChangeHandler,
                compositionChangeHandler);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBox.ClickCancel();
            };

            // When
            const AssessmentSectionComposition newComposition = AssessmentSectionComposition.DikeAndDune;
            properties.AssessmentSectionComposition = newComposition;

            // Then
            Assert.AreEqual(originalComposition, properties.AssessmentSectionComposition);
            Assert.AreEqual(originalComposition, assessmentSection.Composition);
            mocks.VerifyAll();
        }

        [Test]
        public void ReturnPeriod_ValueChanges_NotifiesChangedObjectsInAssessmentSection()
        {
            // Setup
            const int returnPeriod = 200;
            const double norm = 1.0/returnPeriod;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mocks = new MockRepository();
            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var normChangeHandler = mocks.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            normChangeHandler.Expect(h => h.ConfirmNormChange()).Return(true);
            normChangeHandler.Expect(h => h.ChangeNorm(assessmentSection, norm))
                             .Return(new[]
                             {
                                 observable1,
                                 observable2
                             });
            mocks.ReplayAll();

            var properties = new FailureMechanismContributionProperties(
                assessmentSection.FailureMechanismContribution,
                assessmentSection,
                normChangeHandler,
                new AssessmentSectionCompositionChangeHandler());

            // Call
            properties.ReturnPeriod = returnPeriod;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        [TestCase(99)]
        [TestCase(1000001)]
        public void ReturnPeriod_InvalidValue_ThrowsArgumentOutOfRangeException(int invalidReturnPeriod)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionComposition = AssessmentSectionComposition.DikeAndDune;
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Composition).Return(assessmentSectionComposition);
            mocks.ReplayAll();

            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 1.0/200);

            var properties = new FailureMechanismContributionProperties(
                contribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(),
                new AssessmentSectionCompositionChangeHandler());

            // Call
            TestDelegate call = () => properties.ReturnPeriod = invalidReturnPeriod;

            // Assert
            string expectedMessage = "De waarde voor de 'Norm (terugkeertijd)' moet in het bereik [100, 1000000] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void AssessmentSectionComposition_CompositionValueChanges_NotifiesChangedObjectsInAssessmentSection(
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

            var properties = new FailureMechanismContributionProperties(
                assessmentSection.FailureMechanismContribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(), 
                compositionChangeHandler);

            // Call
            properties.AssessmentSectionComposition = newComposition;

            // Assert
            mocks.VerifyAll();
        }
    }
}
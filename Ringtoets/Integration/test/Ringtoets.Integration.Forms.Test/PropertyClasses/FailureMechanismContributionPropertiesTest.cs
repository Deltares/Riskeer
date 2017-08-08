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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.TestUtils;

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
            var failureMechanismChangeHandler = mockRepository.Stub<IObservablePropertyChangeHandler>();
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
            var failureMechanismChangeHandler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var assessmentSectionChangeHandler = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            mockRepository.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = CreateFailureMechanismContribution();

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

            FailureMechanismContribution failureMechanismContribution = CreateFailureMechanismContribution();

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
            var failureMechanismChangeHandler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = CreateFailureMechanismContribution();

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
            var failureMechanismChangeHandler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            var assessmentSectionChangeHandler = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            mockRepository.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = CreateFailureMechanismContribution();

            // Call
            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                failureMechanismChangeHandler,
                assessmentSectionChangeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismContribution>>(properties);
            Assert.AreSame(failureMechanismContribution, properties.Data);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";

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
            const AssessmentSectionComposition assessmentSectionComposition = AssessmentSectionComposition.DikeAndDune;
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Composition).Return(assessmentSectionComposition);
            var failureMechanismChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            var assessmentSectionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            const int returnPeriod = 30000;
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1.1);

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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var normChangeHandler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

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
            Assert.AreEqual(1.0 / newReturnPeriod, failureMechanismContribution.Norm);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenReturnPeriod_WhenCancelingReturnPeriodValueChange_ThenDataSameObserversNotNotified()
        {
            // Given
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            int originalReturnPeriod = Convert.ToInt32(1 / failureMechanismContribution.Norm);

            var normChangeHandler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

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
            Assert.AreEqual(1.0 / originalReturnPeriod, failureMechanismContribution.Norm);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSectionComposition_WhenConfirmingCompositionValueChange_ThenAssessmentSectionCompositionSetAndNotifiesObserver()
        {
            // Given
            const AssessmentSectionComposition originalComposition = AssessmentSectionComposition.Dike;
            var assessmentSection = new AssessmentSection(originalComposition);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();

            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);
            observer.Expect(o => o.UpdateObserver());
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var compositionChangeHandler = new AssessmentSectionCompositionChangeHandler(viewCommands);

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
        public void GivenAssessmentSectionComposition_WhenCancelingCompositionValueChange__ThenDataSameObserversNotNotified()
        {
            // Given
            const AssessmentSectionComposition originalComposition = AssessmentSectionComposition.Dike;
            var assessmentSection = new AssessmentSection(originalComposition);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();

            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);

            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var compositionChangeHandler = new AssessmentSectionCompositionChangeHandler(viewCommands);

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
            const double norm = 1.0 / returnPeriod;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mocks = new MockRepository();
            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var normChangeHandler = mocks.StrictMock<IObservablePropertyChangeHandler>();
            normChangeHandler.Expect(h => h.SetPropertyValueAfterConfirmation(null)).IgnoreArguments()
                             .Return(new[]
                             {
                                 observable1,
                                 observable2
                             });
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var properties = new FailureMechanismContributionProperties(
                assessmentSection.FailureMechanismContribution,
                assessmentSection,
                normChangeHandler,
                new AssessmentSectionCompositionChangeHandler(viewCommands));

            // Call
            properties.ReturnPeriod = returnPeriod;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        [TestCase(9)]
        [TestCase(1000001)]
        [TestCase(0)]
        [SetCulture("nl-NL")]
        public void ReturnPeriod_InvalidValue_ThrowsArgumentOutOfRangeException(int invalidReturnPeriod)
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1);

            var properties = new FailureMechanismContributionProperties(
                contribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(assessmentSection),
                new AssessmentSectionCompositionChangeHandler(viewCommands));

            // Call
            TestDelegate call = () => properties.ReturnPeriod = invalidReturnPeriod;

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,000001, 0,1] liggen.";
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
                new FailureMechanismContributionNormChangeHandler(assessmentSection),
                compositionChangeHandler);

            // Call
            properties.AssessmentSectionComposition = newComposition;

            // Assert
            mocks.VerifyAll();
        }

        private static FailureMechanismContribution CreateFailureMechanismContribution()
        {
            return new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), new Random(21).Next(0, 100));
        }
    }
}
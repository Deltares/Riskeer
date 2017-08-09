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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Plugin.Handlers;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismContributionPropertiesTest : NUnitFormTest
    {
        [Test]
        public void Constructor_FailureMechanismContributionNull_ThrowsArgumentNullException()
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
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
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
        public void Constructor_FailureMechanismContributionNormChangeHandlerNull_ThrowsArgumentNullException()
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
        public void Constructor_AssessmentSectionCompositionChangeHandlerNull_ThrowsArgumentNullException()
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

            Assert.AreEqual(4, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";

            PropertyDescriptor compositionProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(compositionProperty,
                                                                            expectedCategory,
                                                                            "Trajecttype",
                                                                            "Selecteer het type traject, bepalend voor de faalkansbegroting.");

            PropertyDescriptor lowerLevelNormProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lowerLevelNormProperty,
                                                                            expectedCategory,
                                                                            "Ondergrens [1/jaar]",
                                                                            "Overstromingskans van het dijktraject die hoort bij het minimale beschermingsniveau dat de kering moet bieden.");

            PropertyDescriptor signalingNormProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(signalingNormProperty,
                                                                            expectedCategory,
                                                                            "Signaleringswaarde [1/jaar]",
                                                                            "Overstromingskans van het dijktraject waarvan overschrijding gemeld moet worden aan de Minister van I en M.");

            PropertyDescriptor normativeNormProperty = dynamicProperties[3];
            Assert.IsInstanceOf<EnumTypeConverter>(normativeNormProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(normativeNormProperty,
                                                                            expectedCategory,
                                                                            "Norm van het dijktraject",
                                                                            "De kans die wordt gebruikt als de norm van het dijktraject.");
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

            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1.1);

            // Call
            var properties = new FailureMechanismContributionProperties(
                contribution,
                assessmentSection,
                failureMechanismChangeHandler,
                assessmentSectionChangeHandler);

            // Assert
            string expectedLowerLimitNorm = ProbabilityFormattingHelper.Format(contribution.LowerLimitNorm);
            string expectedSignalingNorm = ProbabilityFormattingHelper.Format(contribution.SignalingNorm);

            Assert.AreEqual(assessmentSectionComposition, properties.AssessmentSectionComposition);
            Assert.AreEqual(expectedLowerLimitNorm, properties.LowerLimitNorm);
            Assert.AreEqual(expectedSignalingNorm, properties.SignalingNorm);
            Assert.AreEqual(contribution.NormativeNorm, properties.NormativeNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void LowerLimitNorm_Always_ContributionNotifiedAndPropertyChangedCalled()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.LowerLimitNorm = 0.01.ToString(CultureInfo.CurrentCulture));
        }

        [Test]
        public void SignalingNorm_Always_ContributionNotifiedAndPropertyChangedCalled()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.SignalingNorm = 0.00001.ToString(CultureInfo.CurrentCulture));
        }

        [Test]
        public void NormativeNorm_Always_ContributionNotifiedAndPropertyChangedCalled()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.NormativeNorm = NormType.Signaling);
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

        private static void SetPropertyAndVerifyNotifcationsAndOutputForCalculation(Action<FailureMechanismContributionProperties> setProperty)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mocks = new MockRepository();
            var compositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                handler,
                compositionChangeHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private static FailureMechanismContribution CreateFailureMechanismContribution()
        {
            return new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), new Random(21).Next(0, 100));
        }
    }
}
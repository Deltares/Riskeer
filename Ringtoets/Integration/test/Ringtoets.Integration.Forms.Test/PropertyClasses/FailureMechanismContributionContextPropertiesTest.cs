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
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Plugin.Handlers;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismContributionContextPropertiesTest : NUnitFormTest
    {
        private MockRepository mocks;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            mocks = new MockRepository();
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new FailureMechanismContributionContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismContribution>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var failureMechanismContributionProperties = new FailureMechanismContributionContextProperties();

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(failureMechanismContributionProperties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            Assert.AreEqual(2, dynamicProperties.Count);

            var expectedCategory = "Algemeen";

            PropertyDescriptor compositionProperty = dynamicProperties[0];
            Assert.IsNotNull(compositionProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(compositionProperty,
                                                                            expectedCategory,
                                                                            "Trajecttype",
                                                                            "Selecteer het type traject, bepalend voor de faalkansbegroting.");

            PropertyDescriptor returnPeriodProperty = dynamicProperties[1];
            Assert.IsNotNull(returnPeriodProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(returnPeriodProperty,
                                                                            expectedCategory,
                                                                            "Norm [1/jaar]",
                                                                            "De norm waarmee gerekend wordt.");
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionComposition = AssessmentSectionComposition.DikeAndDune;
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Composition).Return(assessmentSectionComposition);
            mocks.ReplayAll();

            int returnPeriod = 30000;
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 1.0/returnPeriod);

            // Call
            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = contribution,
                AssessmentSection = assessmentSection
            };

            // Assert
            Assert.AreEqual(returnPeriod, properties.ReturnPeriod);
            Assert.AreEqual(assessmentSectionComposition, properties.AssessmentSectionComposition);
            mocks.VerifyAll();
        }

        [Test]
        public void NormChangeHandler_SetNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismContributionContextProperties()
            {
                NormChangeHandler = null
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void CompositionChangeHandler_SetNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismContributionContextProperties()
            {
                CompositionChangeHandler = null
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenReturnPeriod_WhenConfirmingChanges_ReturnPeriodSetAndNotifiesObserver(bool confirmChange)
        {
            // Given
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            int originalReturnPeriod = Convert.ToInt32(1/failureMechanismContribution.Norm);

            var normChangeHandler = new FailureMechanismContributionNormChangeHandler();

            var compositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            compositionChangeHandler.Stub(h => h.ConfirmCompositionChange()).Return(false);
            mocks.ReplayAll();

            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);
            if (confirmChange)
            {
                observer.Expect(o => o.UpdateObserver());
            }
            else
            {
                observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            }
            mocks.ReplayAll();

            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection,
                NormChangeHandler = normChangeHandler,
                CompositionChangeHandler = compositionChangeHandler
            };

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                if (confirmChange)
                {
                    messageBox.ClickOk();
                }
                else
                {
                    messageBox.ClickCancel();
                }
            };

            // When
            const int newReturnPeriod = 200;
            properties.ReturnPeriod = newReturnPeriod;

            // Then
            Assert.AreEqual(confirmChange ? newReturnPeriod : originalReturnPeriod, properties.ReturnPeriod);
            Assert.AreEqual(confirmChange ? 1.0/newReturnPeriod : 1.0/originalReturnPeriod, failureMechanismContribution.Norm);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenAssessmentSectionComposition_WhenConfirmingChanges_AssessmentSectionCompositionSetAndNotifiesObserver(bool confirmChange)
        {
            // Given
            const AssessmentSectionComposition originalComposition = AssessmentSectionComposition.Dike;
            AssessmentSection assessmentSection = new AssessmentSection(originalComposition);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var compositionChangeHandler = new AssessmentSectionCompositionChangeHandler();

            var normChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            normChangeHandler.Stub(h => h.ConfirmNormChange())
                             .Return(false);

            var observer = mocks.StrictMock<IObserver>();
            failureMechanismContribution.Attach(observer);
            if (confirmChange)
            {
                observer.Expect(o => o.UpdateObserver());
            }
            else
            {
                observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            }
            mocks.ReplayAll();

            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection,
                NormChangeHandler = normChangeHandler,
                CompositionChangeHandler = compositionChangeHandler
            };

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                if (confirmChange)
                {
                    messageBox.ClickOk();
                }
                else
                {
                    messageBox.ClickCancel();
                }
            };

            // When
            const AssessmentSectionComposition newComposition = AssessmentSectionComposition.DikeAndDune;
            properties.AssessmentSectionComposition = newComposition;

            // Then
            Assert.AreEqual(confirmChange ? newComposition : originalComposition, properties.AssessmentSectionComposition);
            Assert.AreEqual(confirmChange ? newComposition : originalComposition, assessmentSection.Composition);
            mocks.VerifyAll();
        }

        [Test]
        public void ReturnPeriod_ValueChangedAndConfirmed_NotifiesChangedObjects()
        {
            // Setup
            const int returnPeriod = 200;
            const double norm = 1.0/returnPeriod;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

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

            var properties = new FailureMechanismContributionContextProperties()
            {
                AssessmentSection = assessmentSection,
                NormChangeHandler = normChangeHandler,
                Data = assessmentSection.FailureMechanismContribution
            };

            // Call
            properties.ReturnPeriod = returnPeriod;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void AssessmentSectionComposition_WhenConfirmingChanges_NotifiesChangedObjects(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(initialComposition);

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

            var properties = new FailureMechanismContributionContextProperties()
            {
                AssessmentSection = assessmentSection,
                CompositionChangeHandler = compositionChangeHandler,
                Data = assessmentSection.FailureMechanismContribution
            };

            // Call
            properties.AssessmentSectionComposition = newComposition;

            // Assert
            mocks.VerifyAll();
        }
    }
}
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class NormPropertiesTest : NUnitFormTest
    {
        [Test]
        public void Constructor_FailureMechanismContributionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new NormProperties(null, normChangeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanismContribution", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NormChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            TestDelegate test = () => new NormProperties(failureMechanismContribution, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("normChangeHandler", paramName);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            var properties = new NormProperties(failureMechanismContribution, normChangeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismContribution>>(properties);
            Assert.AreSame(failureMechanismContribution, properties.Data);

            TestHelper.AssertTypeConverter<NormProperties, NoProbabilityValueDoubleConverter>(
                nameof(NormProperties.SignalingNorm));
            TestHelper.AssertTypeConverter<NormProperties, NoProbabilityValueDoubleConverter>(
                nameof(NormProperties.LowerLimitNorm));
            TestHelper.AssertTypeConverter<NormProperties, EnumTypeConverter>(
                nameof(NormProperties.NormativeNorm));
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            var properties = new NormProperties(failureMechanismContribution, normChangeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";

            PropertyDescriptor signalingNormProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(signalingNormProperty,
                                                                            expectedCategory,
                                                                            "Signaleringswaarde [1/jaar]",
                                                                            "Overstromingskans van het dijktraject waarvan overschrijding gemeld moet worden aan de Minister van I en M.");

            PropertyDescriptor lowerLevelNormProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lowerLevelNormProperty,
                                                                            expectedCategory,
                                                                            "Ondergrens [1/jaar]",
                                                                            "Overstromingskans van het dijktraject die hoort bij het minimale beschermingsniveau dat de kering moet bieden.");

            PropertyDescriptor normativeNormProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(normativeNormProperty,
                                                                            expectedCategory,
                                                                            "Norm van het dijktraject",
                                                                            "De kans die wordt gebruikt als de norm van het dijktraject.");

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            var properties = new NormProperties(failureMechanismContribution, normChangeHandler);

            // Assert
            Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, properties.LowerLimitNorm);
            Assert.AreEqual(failureMechanismContribution.SignalingNorm, properties.SignalingNorm);
            Assert.AreEqual(failureMechanismContribution.NormativeNorm, properties.NormativeNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void LowerLimitNorm_Always_ContributionNotifiedAndPropertyChangedCalled()
        {
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(properties => properties.LowerLimitNorm = 0.001);
        }

        [Test]
        public void SignalingNorm_Always_ContributionNotifiedAndPropertyChangedCalled()
        {
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(properties => properties.SignalingNorm = 0.00001);
        }

        [Test]
        public void NormativeNorm_Always_ContributionNotifiedAndPropertyChangedCalled()
        {
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(properties => properties.NormativeNorm = NormType.Signaling);
        }

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(Action<NormProperties> setProperty)
        {
            // Setup
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new NormProperties(failureMechanismContribution, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}
﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.TestUtil;
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
            var failureMechanismContributionNormChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new NormProperties(null, failureMechanismContributionNormChangeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismContribution", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NormChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            void Call() => new NormProperties(failureMechanismContribution, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("normChangeHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismContributionNormChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            var properties = new NormProperties(failureMechanismContribution, failureMechanismContributionNormChangeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismContribution>>(properties);
            Assert.AreSame(failureMechanismContribution, properties.Data);

            TestHelper.AssertTypeConverter<NormProperties, NoProbabilityValueDoubleConverter>(
                nameof(NormProperties.SignalFloodingProbability));
            TestHelper.AssertTypeConverter<NormProperties, NoProbabilityValueDoubleConverter>(
                nameof(NormProperties.MaximumAllowableFloodingProbability));
            TestHelper.AssertTypeConverter<NormProperties, EnumTypeConverter>(
                nameof(NormProperties.NormativeNorm));
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismContributionNormChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            var properties = new NormProperties(failureMechanismContribution, failureMechanismContributionNormChangeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";

            PropertyDescriptor maximumAllowableFloodingProbabilityProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(maximumAllowableFloodingProbabilityProperty,
                                                                            expectedCategory,
                                                                            "Omgevingswaarde [1/jaar]",
                                                                            "De maximale toelaatbare overstromingskans van het traject.");

            PropertyDescriptor signalFloodingProbabilityProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(signalFloodingProbabilityProperty,
                                                                            expectedCategory,
                                                                            "Signaleringsparameter [1/jaar]",
                                                                            "De overstromingskans voor de signalering over de veiligheid van het traject.");

            PropertyDescriptor normativeNormProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(normativeNormProperty,
                                                                            expectedCategory,
                                                                            "Rekenwaarde voor waterstanden",
                                                                            "De doelkans die wordt gebruikt om de lokale waterstand te bepalen voor de semi-probabilistische toets voor 'Piping' en 'Macrostabiliteit binnenwaarts'.");

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismContributionNormChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            var properties = new NormProperties(failureMechanismContribution, failureMechanismContributionNormChangeHandler);

            // Assert
            Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, properties.MaximumAllowableFloodingProbability);
            Assert.AreEqual(failureMechanismContribution.SignalingNorm, properties.SignalFloodingProbability);
            Assert.AreEqual(failureMechanismContribution.NormativeNorm, properties.NormativeNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenNormativeNormIsMaximumAllowableFloodingProbability_WhenChangingMaximumAllowableFloodingProbability_ThenHandlerCalledAndPropertySet()
        {
            // Given
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            var mocks = new MockRepository();
            var failureMechanismContributionNormChangeHandler = mocks.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            failureMechanismContributionNormChangeHandler.Expect(h => h.ChangeNormativeNorm(null))
                                                         .IgnoreArguments()
                                                         .WhenCalled(invocation =>
                                                         {
                                                             var actionToPerform = (Action) invocation.Arguments[0];
                                                             actionToPerform();
                                                         });
            mocks.ReplayAll();

            var properties = new NormProperties(failureMechanismContribution, failureMechanismContributionNormChangeHandler);

            const double newValue = 0.001;

            // When
            properties.MaximumAllowableFloodingProbability = newValue;

            // Then
            Assert.AreEqual(newValue, failureMechanismContribution.LowerLimitNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenNormativeNormIsSignalFloodingProbability_WhenChangingMaximumAllowableFloodingProbability_ThenHandlerCalledAndPropertySet()
        {
            // Given
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();
            failureMechanismContribution.NormativeNorm = NormativeProbabilityType.SignalFloodingProbability;

            var mocks = new MockRepository();
            var failureMechanismContributionNormChangeHandler = mocks.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            failureMechanismContributionNormChangeHandler.Expect(h => h.ChangeNorm(null))
                                                         .IgnoreArguments()
                                                         .WhenCalled(invocation =>
                                                         {
                                                             var actionToPerform = (Action) invocation.Arguments[0];
                                                             actionToPerform();
                                                         });
            mocks.ReplayAll();

            var properties = new NormProperties(failureMechanismContribution, failureMechanismContributionNormChangeHandler);

            const double newValue = 0.001;

            // When
            properties.MaximumAllowableFloodingProbability = newValue;

            // Then
            Assert.AreEqual(newValue, failureMechanismContribution.LowerLimitNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenNormativeNormIsSignalFloodingProbability_WhenChangingSignalFloodingProbability_ThenHandlerCalledAndPropertySet()
        {
            // Given
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();
            failureMechanismContribution.NormativeNorm = NormativeProbabilityType.SignalFloodingProbability;

            var mocks = new MockRepository();
            var failureMechanismContributionNormChangeHandler = mocks.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            failureMechanismContributionNormChangeHandler.Expect(h => h.ChangeNormativeNorm(null))
                                                         .IgnoreArguments()
                                                         .WhenCalled(invocation =>
                                                         {
                                                             var actionToPerform = (Action) invocation.Arguments[0];
                                                             actionToPerform();
                                                         });
            mocks.ReplayAll();

            var properties = new NormProperties(failureMechanismContribution, failureMechanismContributionNormChangeHandler);

            const double newValue = 0.00001;

            // When
            properties.SignalFloodingProbability = newValue;

            // Then
            Assert.AreEqual(newValue, failureMechanismContribution.SignalingNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenNormativeNormIsMaximumAllowableFloodingProbability_WhenChangingSignalFloodingProbability_ThenHandlerCalledAndPropertySet()
        {
            // Given
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            var mocks = new MockRepository();
            var failureMechanismContributionNormChangeHandler = mocks.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            failureMechanismContributionNormChangeHandler.Expect(h => h.ChangeNorm(null))
                                                         .IgnoreArguments()
                                                         .WhenCalled(invocation =>
                                                         {
                                                             var actionToPerform = (Action) invocation.Arguments[0];
                                                             actionToPerform();
                                                         });
            mocks.ReplayAll();

            var properties = new NormProperties(failureMechanismContribution, failureMechanismContributionNormChangeHandler);

            const double newValue = 0.00001;

            // When
            properties.SignalFloodingProbability = newValue;

            // Then
            Assert.AreEqual(newValue, failureMechanismContribution.SignalingNorm);
            mocks.VerifyAll();
        }

        [Test]
        public void NormativeNorm_Always_HandlerCalledAndPropertySet()
        {
            // Setup
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            var mocks = new MockRepository();
            var failureMechanismContributionNormChangeHandler = mocks.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            failureMechanismContributionNormChangeHandler.Expect(h => h.ChangeNormativeNormType(null))
                                                         .IgnoreArguments()
                                                         .WhenCalled(invocation =>
                                                         {
                                                             var actionToPerform = (Action) invocation.Arguments[0];
                                                             actionToPerform();
                                                         });
            mocks.ReplayAll();

            var properties = new NormProperties(failureMechanismContribution, failureMechanismContributionNormChangeHandler);

            const NormativeProbabilityType newValue = NormativeProbabilityType.SignalFloodingProbability;

            // Call
            properties.NormativeNorm = newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismContribution.NormativeNorm);
            mocks.VerifyAll();
        }
    }
}
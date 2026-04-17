// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects.HydraulicLoadsState;

namespace Riskeer.WaveImpactAsphaltCover.Forms.Test.PresentationObjects.HydraulicLoadsState
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            var context = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismContext<WaveImpactAsphaltCoverFailureMechanism>>(context);
            Assert.AreSame(failureMechanism, context.WrappedData);
            Assert.AreSame(assessmentSection, context.Parent);
        }
        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(-0.005)]
        [TestCase( 2.005)]
        public void WaterVolumetricWeight_SetInvalidValue_ThrowArgumentExceptionAndDoesNotUpdateObservers(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var roundedValue = (RoundedDouble) value;

            var handler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<WaveImpactAsphaltCoverFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observable
                });

            var properties = new Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses.HydraulicLoadsState.WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism, handler);

            // Call
            void Call() => properties.C = roundedValue;

            // Assert
            const string expectedMessage = "De waarde van parameter 'c' moet binnen het bereik [0,00, 2,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            Assert.IsTrue(handler.Called);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1.5)]
        [TestCase(-0.004)]
        [TestCase( 2.004)]
        public void WaterVolumetricWeight_SetValidValue_SetsValueRoundedAndUpdatesObservers(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var roundedValue = (RoundedDouble) value;

            var handler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<WaveImpactAsphaltCoverFailureMechanism, RoundedDouble>(
                failureMechanism,
                roundedValue,
                new[]
                {
                    observable
                });

            var properties = new Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses.HydraulicLoadsState.WaveImpactAsphaltCoverFailureMechanismProperties(failureMechanism, handler);

            // Call
            properties.C = roundedValue;

            // Assert
            Assert.AreEqual(value, failureMechanism.GeneralInput.C,
                            failureMechanism.GeneralInput.C.GetAccuracy());
            Assert.IsTrue(handler.Called);

            mocks.VerifyAll();
        }
    }
}
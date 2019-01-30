// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class InputContextBaseTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var input = mockRepository.Stub<ICalculationInput>();
            var calculation = mockRepository.Stub<ICalculation>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            var context = new SimpleInputContext<ICalculationInput, ICalculation, IFailureMechanism>(input, calculation, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismItemContextBase<ICalculationInput, IFailureMechanism>>(context);
            Assert.AreSame(input, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_CalculationIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var input = mockRepository.Stub<ICalculationInput>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleInputContext<ICalculationInput, ICalculation, IFailureMechanism>(input, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        private class SimpleInputContext<TInput, TCalculation, TFailureMechanism> : InputContextBase<TInput, TCalculation, TFailureMechanism>
            where TInput : ICalculationInput
            where TCalculation : ICalculation
            where TFailureMechanism : IFailureMechanism
        {
            public SimpleInputContext(TInput input, TCalculation calculation, TFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(input, calculation, failureMechanism, assessmentSection) {}
        }
    }
}
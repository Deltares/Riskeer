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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class InputContextBaseTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var inputStub = mockRepository.Stub<ICalculationInput>();
            var calculationStub = mockRepository.Stub<ICalculation>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            var context = new SimpleInputContext<ICalculationInput, ICalculation, IFailureMechanism>(inputStub, calculationStub, failureMechanismStub, assessmentSectionStub);

            // Assert
            Assert.IsInstanceOf<FailureMechanismItemContextBase<ICalculationInput, IFailureMechanism>>(context);
            Assert.AreSame(inputStub, context.WrappedData);
            Assert.AreSame(calculationStub, context.Calculation);
            Assert.AreSame(failureMechanismStub, context.FailureMechanism);
            Assert.AreSame(assessmentSectionStub, context.AssessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_CalculationIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            var inputStub = mockRepository.Stub<ICalculationInput>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleInputContext<ICalculationInput, ICalculation, IFailureMechanism>(inputStub, null, failureMechanismStub, assessmentSectionStub);

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
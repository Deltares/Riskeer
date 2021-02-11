// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.ChangeHandlers;

namespace Riskeer.Piping.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class PipingFailureMechanismCalculationChangeHandlerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            var changeHandler = new PipingFailureMechanismCalculationChangeHandler(new PipingFailureMechanism(), string.Empty, inquiryHelper);

            // Assert
            Assert.IsInstanceOf<FailureMechanismCalculationChangeHandler>(changeHandler);
            mocks.VerifyAll();
        }

        [Test]
        public void RequireConfirmation_NoProbabilisticCalculations_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario(),
                new TestPipingCalculationScenario()
            });

            var changeHandler = new PipingFailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHelper);

            // Call
            bool requireConfirmation = changeHandler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
            mocks.VerifyAll();
        }

        [Test]
        public void RequireConfirmation_ProbabilisticCalculationsWithoutOutput_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new ProbabilisticPipingCalculationScenario());

            var changeHandler = new PipingFailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHelper);

            // Call
            bool requireConfirmation = changeHandler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
            mocks.VerifyAll();
        }

        [Test]
        public void RequireConfirmation_ProbabilisticCalculationsWithOutput_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            });

            var changeHandler = new PipingFailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHelper);

            // Call
            bool requireConfirmation = changeHandler.RequireConfirmation();

            // Assert
            Assert.IsTrue(requireConfirmation);
            mocks.VerifyAll();
        }
    }
}
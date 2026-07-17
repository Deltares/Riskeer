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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class SynchronizeCalculationWithForeshoreProfileHelperTest
    {
        [Test]
        public void UpdateForeshoreProfileDerivedCalculationInput_ForeshoreProfileSynchronized_DoesNotNotifyObservers()
        {
            // Setup
            var calculationInput = Substitute.For<ICalculationInputWithForeshoreProfile>();
            calculationInput.IsForeshoreProfileInputSynchronized.Returns(true);

            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculation.InputParameters.Returns(calculationInput);
            // Call
            SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput(calculation);

            // Assert;
            calculationInput.DidNotReceive().NotifyObservers();
            calculation.DidNotReceive().NotifyObservers();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpdateForeshoreProfileDerivedCalculationInput_ForeshoreProfileNotSynchronized_NotifyObservers(bool hasOutput)
        {
            // Setup
            var calculationInput = Substitute.For<ICalculationInputWithForeshoreProfile>();
            calculationInput.IsForeshoreProfileInputSynchronized.Returns(false);

            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculation.InputParameters.Returns(calculationInput);
            calculation.HasOutput.Returns(hasOutput);

            // Call
            SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput(calculation);

            // Assert
            calculationInput.Received().SynchronizeForeshoreProfileInput();
            calculationInput.Received().NotifyObservers();
            if (hasOutput)
            {
                calculation.Received().ClearOutput();
                calculation.Received().NotifyObservers();
            }
        }

        public interface ICalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile {}
    }
}
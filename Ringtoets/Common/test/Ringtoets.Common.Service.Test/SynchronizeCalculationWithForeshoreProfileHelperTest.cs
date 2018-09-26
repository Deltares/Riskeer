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

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class SynchronizeCalculationWithForeshoreProfileHelperTest
    {
        [Test]
        public void UpdateForeshoreProfileDerivedCalculationInput_ForeshoreProfileSynchronized_DoesNotNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationInput = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            calculationInput.Expect(ci => ci.IsForeshoreProfileInputSynchronized).Return(true);

            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculation.Stub(c => c.InputParameters).Return(calculationInput);
            mocks.ReplayAll();

            // Call
            SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput(calculation);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpdateForeshoreProfileDerivedCalculationInput_ForeshoreProfileNotSynchronized_NotifyObservers(bool hasOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationInput = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            calculationInput.Expect(ci => ci.IsForeshoreProfileInputSynchronized).Return(false);
            calculationInput.Expect(ci => ci.SynchronizeForeshoreProfileInput());
            calculationInput.Expect(ci => ci.NotifyObservers());

            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculation.Stub(c => c.InputParameters).Return(calculationInput);
            calculation.Expect(c => c.HasOutput).Return(hasOutput);
            if (hasOutput)
            {
                calculation.Expect(c => c.ClearOutput());
                calculation.Expect(c => c.NotifyObservers());
            }

            mocks.ReplayAll();

            // Call
            SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput(calculation);

            // Assert
            mocks.VerifyAll();
        }

        public interface ICalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile {}
    }
}
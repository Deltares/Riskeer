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
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class SynchronizeCalculationWithForeshoreProfileHelperTest
    {
        [Test]
        public void UpdateForeshoreProfileDerivedCalculationInput_ForeshoreProfileSynchronized_DoesNotNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationInputMock = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            calculationInputMock.Expect(ci => ci.IsForeshoreProfileInputSynchronized).Return(true);

            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculationMock.Stub(c => c.InputParameters).Return(calculationInputMock);
            mocks.ReplayAll();

            // Call
            SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput(calculationMock);

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
            var calculationInputMock = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            calculationInputMock.Expect(ci => ci.IsForeshoreProfileInputSynchronized).Return(false);
            calculationInputMock.Expect(ci => ci.SynchronizeForeshoreProfileInput());
            calculationInputMock.Expect(ci => ci.NotifyObservers());

            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculationMock.Stub(c => c.InputParameters).Return(calculationInputMock);
            calculationMock.Expect(c => c.HasOutput).Return(hasOutput);
            if (hasOutput)
            {
                calculationMock.Expect(c => c.ClearOutput());
                calculationMock.Expect(c => c.NotifyObservers());
            }
            mocks.ReplayAll();

            // Call
            SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput(calculationMock);

            // Assert
            mocks.VerifyAll();
        }

        public interface ICalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile {}
    }
}
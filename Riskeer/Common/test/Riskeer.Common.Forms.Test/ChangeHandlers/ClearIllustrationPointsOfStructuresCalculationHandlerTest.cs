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

using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfStructuresCalculationHandlerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

            // Call
            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler(calculation, inquiryHelper, viewCommands);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationChangeHandlerBase<IStructuresCalculation>>(handler);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationWithoutIllustrationPoints_WhenClearIllustrationPoints_ThenNothingHappensAndReturnFalse(
            bool hasOutput)
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                Output = hasOutput
                             ? new TestStructuresOutput()
                             : null
            };

            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler(calculation, inquiryHelper, viewCommands);

            // When
            bool isCalculationAffected = handler.ClearIllustrationPoints();

            // Then
            Assert.IsFalse(isCalculationAffected);
            Assert.AreEqual(hasOutput, calculation.HasOutput);
            Assert.IsNull(calculation.Output?.GeneralResult);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationWithIllustrationPoints_WhenClearIllustrationPoints_ThenViewClosedAndIllustrationPointsClearedAndReturnTrue()
        {
            // Given
            var calculation = new TestStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculation.Output.GeneralResult));
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler(calculation, inquiryHelper, viewCommands);

            // When
            bool isCalculationAffected = handler.ClearIllustrationPoints();

            // Then
            Assert.IsTrue(isCalculationAffected);
            Assert.IsTrue(calculation.HasOutput);
            Assert.IsNull(calculation.Output.GeneralResult);
            mocks.VerifyAll();
        }
    }
}
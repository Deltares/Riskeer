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

using System.Linq;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.ChangeHandlers;

namespace Riskeer.Piping.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearProbabilisticPipingCalculationOutputChangeHandlerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var changeHandler = new ClearProbabilisticPipingCalculationOutputChangeHandler(
                Enumerable.Empty<ProbabilisticPipingCalculationScenario>(), inquiryHelper, viewCommands);

            // Assert
            Assert.IsInstanceOf<ClearCalculationOutputChangeHandlerBase<ProbabilisticPipingCalculationScenario>>(changeHandler);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenChangeHandler_WhenClearingCalculations_ThenViewsClosedForOutputs()
        {
            // Given
            var calculations = new[]
            {
                new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
                },
                new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
                }
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculations[0].Output));
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculations[1].Output));
            mocks.ReplayAll();

            var changeHandler = new ClearProbabilisticPipingCalculationOutputChangeHandler(
                calculations, inquiryHelper, viewCommands);

            // When
            changeHandler.ClearCalculations();

            // Then
            mocks.VerifyAll();
        }
    }
}
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
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.ChangeHandlers;

namespace Riskeer.Piping.Forms.Test.ChangeHandlers
{
    public class ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandlerTest
    {
        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();

            // Call
            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(inquiryHelper, calculation);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationChangeHandlerBase<ProbabilisticPipingCalculationScenario>>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_CalculationWithoutOutput_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(inquiryHelper, calculation);

            // Call
            bool result = handler.ClearIllustrationPoints();

            // Assert
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_CalculationWithOutputWithoutIllustrationPoints_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
            };

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(inquiryHelper, calculation);

            // Call
            bool result = handler.ClearIllustrationPoints();

            // Assert
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_CalculationWithOutputWithIllustrationPoints_ClearsIllustrationPointsAndReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(inquiryHelper, calculation);

            // Call
            bool result = handler.ClearIllustrationPoints();

            // Assert
            Assert.IsTrue(result);

            Assert.IsNull(((PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint>) calculation.Output.ProfileSpecificOutput).GeneralResult);
            Assert.IsNull(((PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint>) calculation.Output.SectionSpecificOutput).GeneralResult);
            mocks.VerifyAll();
        }
    }
}
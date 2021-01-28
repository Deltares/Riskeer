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
using System.Collections.Generic;
using Core.Common.Gui.Commands;
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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();

            // Call
            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(calculation, inquiryHelper, viewCommands);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationChangeHandlerBase<ProbabilisticPipingCalculationScenario>>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationWithoutOutput_WhenClearIllustrationPoints_ThenNothingHappensAndReturnFalse()
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(
                calculation, inquiryHelper, viewCommands);

            // When
            bool isCalculationAffected = handler.ClearIllustrationPoints();

            // Then
            Assert.IsFalse(isCalculationAffected);
            Assert.IsFalse(calculation.HasOutput);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_SectionSpecificWithIllustrationAndUnsupportedPartialOutput_ThrowsNotSupportedException()
        {
            // Given
            var sectionSpecificOutput = new TestPartialProbabilisticPipingOutput(0, new TestGeneralResultTopLevelIllustrationPoint());
            var profileSpecificOutput = new TestPartialProbabilisticPipingOutput(0, null);
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput)
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(calculation, inquiryHelper, viewCommands);

            // When
            void Call() => handler.ClearIllustrationPoints();

            // Then
            Assert.Throws<NotSupportedException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_ProfileSpecificWithIllustrationAndUnsupportedPartialOutput_ThrowsNotSupportedException()
        {
            // Given
            var sectionSpecificOutput = new TestPartialProbabilisticPipingOutput(0, null);
            var profileSpecificOutput = new TestPartialProbabilisticPipingOutput(0, new TestGeneralResultTopLevelIllustrationPoint());
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput)
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(calculation, inquiryHelper, viewCommands);

            // When
            void Call() => handler.ClearIllustrationPoints();

            // Then
            Assert.Throws<NotSupportedException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetPartialOutputs))]
        public void GivenCalculationWithOutputWithVariousIllustrationPointsConfigurations_WhenClearIllustrationPoints_ThenViewClosedAndIllustrationPointsClearedAndReturnTrue(
            IPartialProbabilisticPipingOutput sectionSpecificOutput,
            IPartialProbabilisticPipingOutput profileSpecificOutput)
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput)
            };
            
            bool hasSectionSpecificIllustrationPoints = sectionSpecificOutput.HasGeneralResult;
            bool hasProfileSpecificIllustrationPoints = profileSpecificOutput.HasGeneralResult;

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            if (hasSectionSpecificIllustrationPoints)
            {
                SetViewCommandsExpectancies(sectionSpecificOutput, viewCommands);
            }
            
            if (hasProfileSpecificIllustrationPoints)
            {
                SetViewCommandsExpectancies(profileSpecificOutput, viewCommands);
            }

            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(calculation, inquiryHelper, viewCommands);

            // Call
            bool isCalculationAffected = handler.ClearIllustrationPoints();

            // Assert
            bool expectedResult = hasSectionSpecificIllustrationPoints || hasProfileSpecificIllustrationPoints;
            Assert.AreEqual(expectedResult, isCalculationAffected);
            Assert.IsTrue(calculation.HasOutput);
            Assert.IsFalse(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            Assert.IsFalse(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
            mocks.VerifyAll();
        }

        private static void SetViewCommandsExpectancies(IPartialProbabilisticPipingOutput sectionSpecificOutput, IViewCommands viewCommands)
        {
            if (sectionSpecificOutput is PartialProbabilisticFaultTreePipingOutput faultTreeOutput)
            {
                viewCommands.Expect(vc => vc.RemoveAllViewsForItem(faultTreeOutput.GeneralResult));
            }
            else if (sectionSpecificOutput is PartialProbabilisticSubMechanismPipingOutput subMechanismOutput)
            {
                viewCommands.Expect(vc => vc.RemoveAllViewsForItem(subMechanismOutput.GeneralResult));
            }
        }

        private static IEnumerable<TestCaseData> GetPartialOutputs()
        {
            PartialProbabilisticFaultTreePipingOutput faultTreeOutputWithIllustrationPoints = PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput();
            PartialProbabilisticFaultTreePipingOutput faultTreeOutputWithoutIllustrationPoints = PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(null);
            PartialProbabilisticSubMechanismPipingOutput subMechanismOutputWithIllustrationPoints = PipingTestDataGenerator.GetRandomPartialProbabilisticSubMechanismPipingOutput();
            PartialProbabilisticSubMechanismPipingOutput subMechanismOutputWithoutIllustrationPoints = PipingTestDataGenerator.GetRandomPartialProbabilisticSubMechanismPipingOutput(null);

            yield return new TestCaseData(faultTreeOutputWithIllustrationPoints, faultTreeOutputWithIllustrationPoints);
            yield return new TestCaseData(faultTreeOutputWithIllustrationPoints, faultTreeOutputWithoutIllustrationPoints);
            yield return new TestCaseData(faultTreeOutputWithoutIllustrationPoints, faultTreeOutputWithIllustrationPoints);
            yield return new TestCaseData(faultTreeOutputWithoutIllustrationPoints, faultTreeOutputWithoutIllustrationPoints);

            yield return new TestCaseData(subMechanismOutputWithIllustrationPoints, subMechanismOutputWithIllustrationPoints);
            yield return new TestCaseData(subMechanismOutputWithIllustrationPoints, subMechanismOutputWithoutIllustrationPoints);
            yield return new TestCaseData(subMechanismOutputWithoutIllustrationPoints, subMechanismOutputWithIllustrationPoints);
            yield return new TestCaseData(subMechanismOutputWithoutIllustrationPoints, subMechanismOutputWithoutIllustrationPoints);
        }
    }
}
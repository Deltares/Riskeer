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
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.ChangeHandlers;
using Riskeer.Piping.Util;

namespace Riskeer.Piping.Forms.Test.ChangeHandlers
{
    public class ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandlerTest
    {
        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            void Call() => new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(
                null, inquiryHelper, viewCommands);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(
                Enumerable.Empty<ProbabilisticPipingCalculationScenario>(), inquiryHelper, viewCommands);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsAndCloseViewOfCalculationCollectionChangeHandlerBase>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void InquireConfirmation_Always_ReturnsInquiry()
        {
            // Setup
            var random = new Random(21);
            bool expectedConfirmation = random.NextBoolean();

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation("Weet u zeker dat u alle illustratiepunten wilt wissen?"))
                         .Return(expectedConfirmation);
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(
                Enumerable.Empty<ProbabilisticPipingCalculationScenario>(), inquiryHelper, viewCommands);

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_ProfileSpecificWithIllustrationAndUnsupportedPartialOutput_ThrowsNotSupportedException()
        {
            // Setup
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

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(new[]
            {
                calculation
            }, inquiryHelper, viewCommands);

            // Call
            void Call() => handler.ClearIllustrationPoints();

            // Assert
            Assert.Throws<NotSupportedException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetPartialOutputs))]
        public void GivenCalculationsWithOutputWithVariousIllustrationPointsConfigurations_WhenClearIllustrationPoints_ThenViewsClosedAndIllustrationPointsClearedAndReturnTrue(
            IPartialProbabilisticPipingOutput sectionSpecificOutput,
            IPartialProbabilisticPipingOutput profileSpecificOutput)
        {
            // Given
            var calculationWithoutIllustrationPoints = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
            };

            var calculationWithIllustrationPoints = new ProbabilisticPipingCalculationScenario
            {
                Output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput)
            };

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationWithoutIllustrationPoints,
                calculationWithIllustrationPoints,
                new ProbabilisticPipingCalculationScenario()
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            if (sectionSpecificOutput.HasGeneralResult)
            {
                SetViewCommandsExpectancies(sectionSpecificOutput, viewCommands);
            }

            if (profileSpecificOutput.HasGeneralResult)
            {
                SetViewCommandsExpectancies(profileSpecificOutput, viewCommands);
            }

            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(
                calculations, inquiryHelper, viewCommands);

            // When
            IEnumerable<IObservable> affectedObjects = handler.ClearIllustrationPoints();

            // Then
            CollectionAssert.AreEquivalent(new[]
            {
                calculationWithIllustrationPoints
            }, affectedObjects);

            ProbabilisticPipingCalculationScenario[] calculationsWithOutput =
            {
                calculationWithoutIllustrationPoints,
                calculationWithIllustrationPoints
            };
            Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsFalse(calculationsWithOutput.Any(ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints));
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

            yield return new TestCaseData(subMechanismOutputWithIllustrationPoints, subMechanismOutputWithIllustrationPoints);
            yield return new TestCaseData(subMechanismOutputWithIllustrationPoints, subMechanismOutputWithoutIllustrationPoints);
            yield return new TestCaseData(subMechanismOutputWithoutIllustrationPoints, subMechanismOutputWithIllustrationPoints);
        }
    }
}
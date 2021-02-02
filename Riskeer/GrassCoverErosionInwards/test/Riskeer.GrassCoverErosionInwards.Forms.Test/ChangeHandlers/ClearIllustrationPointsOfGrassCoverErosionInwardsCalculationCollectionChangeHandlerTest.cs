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
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.ChangeHandlers;
using Riskeer.GrassCoverErosionInwards.Util;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandlerTest
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
            void Call() => new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler(null, inquiryHelper, viewCommands);

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
            var handler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler(
                Enumerable.Empty<GrassCoverErosionInwardsCalculation>(), inquiryHelper, viewCommands);

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
            inquiryHelper.Expect(h => h.InquireContinuation("Weet u zeker dat u alle illustratiepunten wilt wissen?")).Return(expectedConfirmation);
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler(
                Enumerable.Empty<GrassCoverErosionInwardsCalculation>(), inquiryHelper, viewCommands);

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_Always_ReturnsAffectedCalculations()
        {
            // Setup
            var random = new Random(21);
            var calculationWithOvertoppingOutputWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(
                    new TestOvertoppingOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
                    null, null)
            };

            var calculationWithDikeHeightWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(
                    new TestOvertoppingOutput(random.NextDouble()),
                    new TestDikeHeightOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
                    null)
            };

            var calculationWithOvertoppingRateWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(
                    new TestOvertoppingOutput(random.NextDouble()),
                    null, new TestOvertoppingRateOutput(new TestGeneralResultFaultTreeIllustrationPoint()))
            };

            var calculationWitNoIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(
                    new TestOvertoppingOutput(random.NextDouble()), null, null)
            };

            GrassCoverErosionInwardsCalculation[] calculations =
            {
                calculationWitNoIllustrationPoints,
                calculationWithOvertoppingOutputWithIllustrationPoints,
                calculationWithOvertoppingRateWithIllustrationPoints,
                calculationWithDikeHeightWithIllustrationPoints,
                new GrassCoverErosionInwardsCalculation()
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculationWithOvertoppingOutputWithIllustrationPoints.Output.OvertoppingOutput.GeneralResult));
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculationWithOvertoppingRateWithIllustrationPoints.Output.OvertoppingRateOutput.GeneralResult));
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculationWithDikeHeightWithIllustrationPoints.Output.DikeHeightOutput.GeneralResult));
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler(calculations, inquiryHelper, viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ClearIllustrationPoints();

            // Assert
            CollectionAssert.AreEquivalent(new[]
            {
                calculationWithOvertoppingOutputWithIllustrationPoints,
                calculationWithOvertoppingRateWithIllustrationPoints,
                calculationWithDikeHeightWithIllustrationPoints
            }, affectedObjects);

            GrassCoverErosionInwardsCalculation[] calculationsWithOutput =
            {
                calculationWitNoIllustrationPoints,
                calculationWithOvertoppingOutputWithIllustrationPoints,
                calculationWithOvertoppingRateWithIllustrationPoints,
                calculationWithDikeHeightWithIllustrationPoints
            };
            Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsFalse(calculationsWithOutput.Any(GrassCoverErosionInwardsIllustrationPointsHelper.HasIllustrationPoints));
            mocks.VerifyAll();
        }
    }
}
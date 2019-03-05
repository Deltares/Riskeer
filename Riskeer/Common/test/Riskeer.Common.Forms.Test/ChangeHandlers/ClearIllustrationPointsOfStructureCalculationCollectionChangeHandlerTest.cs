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
using Core.Common.Gui;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfStructureCalculationCollectionChangeHandlerTest
    {
        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler<TestStructuresInput, TestStructure>(
                inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            var handler = new ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler<TestStructuresInput, TestStructure>(
                inquiryHelper, Enumerable.Empty<TestStructuresCalculation>());

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase>(handler);
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
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler<TestStructuresInput, TestStructure>(
                inquiryHelper, Enumerable.Empty<TestStructuresCalculation>());

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.ReplayAll();
        }

        [Test]
        public void ClearIllustrationPoints_Always_ReturnsAffectedCalculations()
        {
            // Setup
            var calculationWithIllustrationPoints = new TestStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };

            TestStructuresCalculation[] calculations =
            {
                new TestStructuresCalculation(),
                calculationWithOutput,
                calculationWithIllustrationPoints
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();

            var calculationWithIllustrationPointsObserver = mocks.StrictMock<IObserver>();
            calculationWithIllustrationPoints.Attach(calculationWithIllustrationPointsObserver);

            var calculationWithoutIllustrationPointsObserver = mocks.StrictMock<IObserver>();
            calculationWithOutput.Attach(calculationWithoutIllustrationPointsObserver);
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler<TestStructuresInput, TestStructure>(inquiryHelper, calculations);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ClearIllustrationPoints();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                calculationWithIllustrationPoints
            }, affectedObjects);

            TestStructuresCalculation[] calculationsWithOutput =
            {
                calculationWithOutput,
                calculationWithIllustrationPoints
            };
            Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithOutput.All(calc => !calc.Output.HasGeneralResult));

            mocks.VerifyAll();
        }
    }
}
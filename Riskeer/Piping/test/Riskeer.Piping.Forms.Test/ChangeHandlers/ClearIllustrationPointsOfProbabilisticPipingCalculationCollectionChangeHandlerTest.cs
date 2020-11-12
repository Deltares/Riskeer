using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Helpers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.ChangeHandlers;

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
            mocks.ReplayAll();

            // Call
            void Call() => new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(
                inquiryHelper, Enumerable.Empty<ProbabilisticPipingCalculationScenario>());

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase>(handler);
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
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(
                inquiryHelper, Enumerable.Empty<ProbabilisticPipingCalculationScenario>());

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
            var outputWithNoIllustrationPoints = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints();
            var calculationWitNoIllustrationPoints = new ProbabilisticPipingCalculationScenario
            {
                Output = outputWithNoIllustrationPoints
            };

            var outputWithIllustrationPoints = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints();
            var calculationWithIllustrationPoints = new ProbabilisticPipingCalculationScenario
            {
                Output = outputWithIllustrationPoints
            };

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationWitNoIllustrationPoints,
                calculationWithIllustrationPoints,
                new ProbabilisticPipingCalculationScenario()
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(
                inquiryHelper, calculations);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ClearIllustrationPoints();

            // Assert
            CollectionAssert.AreEquivalent(new[]
            {
                calculationWithIllustrationPoints
            }, affectedObjects);

            ProbabilisticPipingCalculationScenario[] calculationsWithOutput =
            {
                calculationWitNoIllustrationPoints,
                calculationWithIllustrationPoints
            };
            Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(calculationsWithOutput.All(calc =>
            {
                ProbabilisticPipingOutput output = calc.Output;

                return !output.ProfileSpecificOutput.HasGeneralResult
                       && output.SectionSpecificOutput?.GeneralResult == null;
            }));
            mocks.VerifyAll();
        }
    }
}
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
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
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationChangeHandlerBase<ProbabilisticPipingCalculation>>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_WithCalculationWithIllustrationPoints_ClearsIllustrationPointsAndReturnsExpectedResult()
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

            Assert.IsNull(calculation.Output?.ProfileSpecificOutput.GeneralResult);
            Assert.IsNull(calculation.Output?.SectionSpecificOutput.GeneralResult);
            mocks.VerifyAll();
        }
    }
}
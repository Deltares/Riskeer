using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingCalculationGroupContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilModels = new StochasticSoilModel[]
            {
                new TestStochasticSoilModel()
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var groupContext = new PipingCalculationGroupContext(calculationGroup, surfaceLines, soilModels, pipingFailureMechanismMock, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservable>(groupContext);
            Assert.IsInstanceOf<PipingContext<CalculationGroup>>(groupContext);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, PipingFailureMechanism>>(groupContext);
            Assert.AreSame(calculationGroup, groupContext.WrappedData);
            Assert.AreSame(surfaceLines, groupContext.AvailablePipingSurfaceLines);
            Assert.AreSame(soilModels, groupContext.AvailableStochasticSoilModels);
            Assert.AreSame(pipingFailureMechanismMock, groupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, groupContext.AssessmentSection);
        }
    }
}
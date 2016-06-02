using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingCalculationScenarioContextTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var presentationObject = new PipingCalculationScenarioContext(calculation, surfacelines, soilModels, pipingFailureMechanismMock, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<IObservable>(presentationObject);
            Assert.IsInstanceOf<PipingContext<PipingCalculationScenario>>(presentationObject);
            Assert.IsInstanceOf<ICalculationContext<PipingCalculationScenario, PipingFailureMechanism>>(presentationObject);
            Assert.AreSame(calculation, presentationObject.WrappedData);
            Assert.AreSame(surfacelines, presentationObject.AvailablePipingSurfaceLines);
            Assert.AreSame(soilModels, presentationObject.AvailableStochasticSoilModels);
            Assert.AreSame(pipingFailureMechanismMock, presentationObject.FailureMechanism);
            Assert.AreSame(assessmentSectionMock, presentationObject.AssessmentSection);
        }
    }
}
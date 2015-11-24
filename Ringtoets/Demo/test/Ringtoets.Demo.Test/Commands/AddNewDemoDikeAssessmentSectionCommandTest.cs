using System.Linq;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Demo.Commands;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;

namespace Ringtoets.Demo.Test.Commands
{
    [TestFixture]
    public class AddNewDemoDikeAssessmentSectionCommandTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var command = new AddNewDemoDikeAssessmentSectionCommand();

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsInstanceOf<IGuiCommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            Assert.IsNull(command.Gui);
        }

        [Test]
        public void Execute_GuiIsProperlyInitialized_AddNewDikeAssessmentSectionWithDemoDataToRootProject()
        {
            // Setup
            var project = new Project();

            var mocks = new MockRepository();

            var guiMock = mocks.Stub<IGui>();

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var ringtoetsApplication = new RingtoetsApplication
            {
                Project = project
            };

            guiMock.Application = ringtoetsApplication;

            var command = new AddNewDemoDikeAssessmentSectionCommand
            {
                Gui = guiMock
            };

            project.Attach(observerMock);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.Items.Count);
            var demoAssessmentSection = (DikeAssessmentSection) project.Items[0];
            Assert.AreEqual("Demo dijktraject", demoAssessmentSection.Name);

            var profiles = demoAssessmentSection.PipingFailureMechanism.SoilProfiles.ToArray();
            Assert.AreEqual(26, profiles.Length);
            var surfaceLines = demoAssessmentSection.PipingFailureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(4, surfaceLines.Length);

            Assert.AreEqual(1, demoAssessmentSection.PipingFailureMechanism.Calculations.Count);
            var calculation = demoAssessmentSection.PipingFailureMechanism.Calculations.First();
            AssertCalculationAbleToCalculate(calculation);
            mocks.VerifyAll();
        }

        private void AssertCalculationAbleToCalculate(PipingCalculationData calculation)
        {
            PipingInputParameters inputParameters = calculation.InputParameters;
            AssertExpectedPipingInputParameters(inputParameters);

            Assert.IsTrue(PipingCalculationService.Validate(calculation));
            PipingCalculationService.Calculate(calculation);
            Assert.IsTrue(calculation.HasOutput);
            Assert.AreEqual(0.0021, calculation.Output.HeaveFactorOfSafety, 1e-3);
            Assert.AreEqual(-143.3235, calculation.Output.HeaveZValue, 1e-3);
            Assert.AreEqual(4.4072, calculation.Output.UpliftFactorOfSafety, 1e-3);
            Assert.AreEqual(5.6044, calculation.Output.UpliftZValue, 1e-3);
            Assert.AreEqual(0.0016, calculation.Output.SellmeijerFactorOfSafety, 1e-3);
            Assert.AreEqual(-1.6387, calculation.Output.SellmeijerZValue, 1e-3);
        }

        private static void AssertExpectedPipingInputParameters(PipingInputParameters inputParameters)
        {
            Assert.AreEqual(1.0, inputParameters.UpliftModelFactor, 1e-3);
            Assert.AreEqual(1.0, inputParameters.SellmeijerModelFactor, 1e-3);

            Assert.AreEqual(10.0, inputParameters.WaterVolumetricWeight, 1e-3);
            Assert.AreEqual(0.0, inputParameters.AssessmentLevel, 1e-3);
            Assert.AreEqual(0.0, inputParameters.PiezometricHeadExit, 1e-3);
            Assert.AreEqual(0.0, inputParameters.PiezometricHeadPolder, 1e-3);
            Assert.AreEqual(0.3, inputParameters.SellmeijerReductionFactor, 1e-3);
            Assert.AreEqual(16.5, inputParameters.SandParticlesVolumicWeight, 1e-3);
            Assert.AreEqual(0.25, inputParameters.WhitesDragCoefficient, 1e-3);
            Assert.AreEqual(1.33e-6, inputParameters.WaterKinematicViscosity, 1e-3);
            Assert.AreEqual(9.81, inputParameters.Gravity, 1e-3);
            Assert.AreEqual(0.000208, inputParameters.MeanDiameter70, 1e-3);
            Assert.AreEqual(37, inputParameters.BeddingAngle, 1e-3);

            Assert.AreEqual("PK001_0001", inputParameters.SurfaceLine.Name);
            Assert.AreEqual("AD640M00_Segment_36005_1D2", inputParameters.SoilProfile.Name);

            Assert.AreEqual(3.666, PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(inputParameters).GetDesignValue(), 1e-3);
            Assert.AreEqual(-1.645, PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(inputParameters).GetDesignValue(), 1e-3);
            Assert.AreEqual(0.011, PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(inputParameters).GetDesignValue(), 1e-3);
            Assert.AreEqual(0.011, PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(inputParameters).GetDesignValue(), 1e-3);
            Assert.AreEqual(0.011, PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters).GetDesignValue(), 1e-3);
            Assert.AreEqual(2.345, PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(inputParameters).GetDesignValue(), 1e-3);
            Assert.AreEqual(2.345, PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(inputParameters).GetDesignValue(), 1e-3);
        }
    }
}
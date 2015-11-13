using System.Linq;

using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Demo.Commands;
using Ringtoets.Piping.Calculation.Piping;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;

namespace Ringtoets.Demo.Test.Commands
{
    [TestFixture]
    public class AddNewDemoProjectCommandTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var command = new AddNewDemoProjectCommand();

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsInstanceOf<IGuiCommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            Assert.IsNull(command.Gui);
        }

        [Test]
        public void Execute_GuiIsProperlyInitialized_AddNewDemoProjectToRootProject()
        {
            // Setup
            var project = new Project();

            var mocks = new MockRepository();
            var applicationMock = mocks.Stub<IApplication>();
            applicationMock.Stub(a => a.Project).Return(project);
            var guiMock = mocks.Stub<IGui>();
            guiMock.Application = applicationMock;

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var command = new AddNewDemoProjectCommand();
            command.Gui = guiMock;

            project.Attach(observerMock);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.Items.Count);
            var demoProject = (AssessmentSection) project.Items[0];
            Assert.AreEqual("Demo traject", demoProject.Name);

            var profiles = demoProject.PipingFailureMechanism.SoilProfiles.ToArray();
            Assert.AreEqual(26, profiles.Length);
            var surfaceLines = demoProject.PipingFailureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(4, surfaceLines.Length);

            Assert.AreEqual(1, demoProject.PipingFailureMechanism.Calculations.Count);
            var calculation = demoProject.PipingFailureMechanism.Calculations.First();
            AssertCalculationAbleToCalculate(calculation);
            mocks.VerifyAll();
        }

        private void AssertCalculationAbleToCalculate(PipingData calculation)
        {
            Assert.AreEqual(1.0, calculation.UpliftModelFactor, 1e-3);
            Assert.AreEqual(1.0, calculation.SellmeijerModelFactor, 1e-3);

            Assert.AreEqual(10.0, calculation.WaterVolumetricWeight, 1e-3);
            Assert.AreEqual(0.0, calculation.AssessmentLevel, 1e-3);
            Assert.AreEqual(0.0, calculation.PiezometricHeadExit, 1e-3);
            Assert.AreEqual(0.0, calculation.PiezometricHeadPolder, 1e-3);
            Assert.AreEqual(0.3, calculation.SellmeijerReductionFactor, 1e-3);
            Assert.AreEqual(16.5, calculation.SandParticlesVolumicWeight, 1e-3);
            Assert.AreEqual(0.25, calculation.WhitesDragCoefficient, 1e-3);
            Assert.AreEqual(1.33e-6, calculation.WaterKinematicViscosity, 1e-3);
            Assert.AreEqual(9.81, calculation.Gravity, 1e-3);
            Assert.AreEqual(0.000208, calculation.MeanDiameter70, 1e-3);
            Assert.AreEqual(37, calculation.BeddingAngle, 1e-3);

            Assert.AreEqual("PK001_0001", calculation.SurfaceLine.Name);
            Assert.AreEqual("AD640M00_Segment_36005_1D2", calculation.SoilProfile.Name);

            Assert.AreEqual(3.666, PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(calculation).GetDesignValue(), 1e-3);
            Assert.AreEqual(-1.645, PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(calculation).GetDesignValue(), 1e-3);
            Assert.AreEqual(0.011, PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(calculation).GetDesignValue(), 1e-3);
            Assert.AreEqual(0.011, PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(calculation).GetDesignValue(), 1e-3);
            Assert.AreEqual(0.011, PipingSemiProbabilisticDesignValueFactory.GetDiameter70(calculation).GetDesignValue(), 1e-3);
            Assert.AreEqual(2.345, PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(calculation).GetDesignValue(), 1e-3);
            Assert.AreEqual(2.345, PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(calculation).GetDesignValue(), 1e-3);

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
    }
}
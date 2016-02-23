using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Demo.Ringtoets.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;

namespace Demo.Ringtoets.Test.Commands
{
    [TestFixture]
    public class AddNewDemoDikeAssessmentSectionCommandTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            // Call
            var command = new AddNewDemoDikeAssessmentSectionCommand(projectOwner);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_GuiIsProperlyInitialized_AddNewDikeAssessmentSectionWithDemoDataToRootProject()
        {
            // Setup
            var project = new Project();

            var mocks = new MockRepository();
            var projectOwnerStub = mocks.Stub<IProjectOwner>();
            projectOwnerStub.Project = project;

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var command = new AddNewDemoDikeAssessmentSectionCommand(projectOwnerStub);

            project.Attach(observerMock);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.Items.Count);
            var demoAssessmentSection = (DikeAssessmentSection) project.Items[0];
            Assert.AreEqual("Demo dijktraject", demoAssessmentSection.Name);

            Assert.IsNotEmpty(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath);
            var hydraulicBoundaryLocations = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(18, hydraulicBoundaryLocations.Length);

            Assert.AreEqual(1669, demoAssessmentSection.ReferenceLine.Points.Count());

            var profiles = demoAssessmentSection.PipingFailureMechanism.SoilProfiles.ToArray();
            Assert.AreEqual(26, profiles.Length);
            var surfaceLines = demoAssessmentSection.PipingFailureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(4, surfaceLines.Length);
            AssertCharacteristicPointsOnSurfaceLines(surfaceLines);

            Assert.AreEqual(1, demoAssessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Count);
            var calculation = demoAssessmentSection.PipingFailureMechanism.CalculationsGroup.GetPipingCalculations().First();
            AssertCalculationAbleToCalculate(calculation);
            mocks.VerifyAll();
        }

        private void AssertCharacteristicPointsOnSurfaceLines(RingtoetsPipingSurfaceLine[] surfaceLines)
        {
            var surfaceLine1 = surfaceLines.FirstOrDefault(s => s.Name == "PK001_0001");
            var surfaceLine2 = surfaceLines.FirstOrDefault(s => s.Name == "PK001_0002");
            var surfaceLine3 = surfaceLines.FirstOrDefault(s => s.Name == "PK001_0003");
            var surfaceLine4 = surfaceLines.FirstOrDefault(s => s.Name == "PK001_0004");
            
            Assert.IsNotNull(surfaceLine1);
            Assert.IsNotNull(surfaceLine2);
            Assert.IsNotNull(surfaceLine3);
            Assert.IsNotNull(surfaceLine4);

            Assert.AreEqual(new Point3D {X = 155883.762, Y = 569864.416, Z = 0.53}, surfaceLine1.DitchPolderSide);
            Assert.AreEqual(new Point3D {X = 155882.067, Y = 569866.157, Z = -1.9}, surfaceLine1.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D {X = 155874.184, Y = 569874.252, Z = -1.9}, surfaceLine1.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D {X = 155872.224, Y = 569876.265, Z = 0.91}, surfaceLine1.DitchDikeSide);
            Assert.AreEqual(new Point3D {X = 155864.173, Y = 569884.532, Z = 0.95}, surfaceLine1.DikeToeAtPolder);
            Assert.AreEqual(new Point3D {X = 155797.109, Y = 569953.4, Z = -4}, surfaceLine1.DikeToeAtRiver);
            
            Assert.IsNull(surfaceLine2.DitchPolderSide);
            Assert.IsNull(surfaceLine2.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine2.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine2.DitchDikeSide);
            Assert.AreEqual(new Point3D { X = 155558.754, Y = 569618.729, Z = 1.45}, surfaceLine2.DikeToeAtPolder);
            Assert.AreEqual(new Point3D { X = 155505.259, Y = 569701.229, Z = -4 }, surfaceLine2.DikeToeAtRiver);
            
            Assert.AreEqual(new Point3D { X = 155063.763, Y = 569276.113, Z = -0.5}, surfaceLine3.DitchPolderSide);
            Assert.AreEqual(new Point3D { X = 155063.272, Y = 569276.926, Z = -1.45 }, surfaceLine3.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D { X = 155056.855, Y = 569287.56, Z = -1.45 }, surfaceLine3.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D { X = 155056.297, Y = 569288.485, Z = -0.37}, surfaceLine3.DitchDikeSide);
            Assert.AreEqual(new Point3D { X = 155047.587, Y = 569302.917, Z = 1.42}, surfaceLine3.DikeToeAtPolder);
            Assert.AreEqual(new Point3D { X = 154999.006, Y = 569383.419, Z = -4 }, surfaceLine3.DikeToeAtRiver);

            Assert.IsNull(surfaceLine4.DitchPolderSide);
            Assert.IsNull(surfaceLine4.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine4.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine4.DitchDikeSide);
            Assert.AreEqual(new Point3D { X = 154682.383, Y = 568112.623, Z = 1.55}, surfaceLine4.DikeToeAtPolder);
            Assert.AreEqual(new Point3D { X = 154586.088, Y = 568119.17, Z = -4 }, surfaceLine4.DikeToeAtRiver);
        }

        private void AssertCalculationAbleToCalculate(PipingCalculation calculation)
        {
            PipingInput inputParameters = calculation.InputParameters;
            AssertExpectedPipingInput(inputParameters);

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

        private static void AssertExpectedPipingInput(PipingInput inputParameters)
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
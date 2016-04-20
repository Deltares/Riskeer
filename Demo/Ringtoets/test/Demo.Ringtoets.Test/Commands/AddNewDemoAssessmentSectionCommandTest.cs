using System;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Demo.Ringtoets.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;

namespace Demo.Ringtoets.Test.Commands
{
    [TestFixture]
    public class AddNewDemoAssessmentSectionCommandTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            // Call
            var command = new AddNewDemoAssessmentSectionCommand(projectOwner);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_GuiIsProperlyInitialized_AddNewAssessmentSectionWithDemoDataToRootProject()
        {
            // Setup
            var project = new Project();

            var mocks = new MockRepository();
            var projectOwnerStub = mocks.Stub<IProjectOwner>();
            projectOwnerStub.Project = project;

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var command = new AddNewDemoAssessmentSectionCommand(projectOwnerStub);

            project.Attach(observerMock);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.Items.Count);
            var demoAssessmentSection = (AssessmentSection) project.Items[0];
            Assert.AreEqual("Demo traject", demoAssessmentSection.Name);

            Assert.IsNotEmpty(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath);
            Assert.IsTrue(File.Exists(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath));
            Assert.IsTrue(File.Exists(Path.Combine(Path.GetDirectoryName(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath), "HLCD.sqlite")));
            var hydraulicBoundaryLocations = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(18, hydraulicBoundaryLocations.Length);
            AssertValuesOnHydraulicBoundaryLocations(hydraulicBoundaryLocations);

            Assert.AreEqual(2380, demoAssessmentSection.ReferenceLine.Points.Count());

            var soilModels = demoAssessmentSection.PipingFailureMechanism.StochasticSoilModels.ToArray();
            Assert.AreEqual(4, soilModels.Length);
            var surfaceLines = demoAssessmentSection.PipingFailureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(4, surfaceLines.Length);
            AssertCharacteristicPointsOnSurfaceLines(surfaceLines);

            Assert.AreEqual(1, demoAssessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Count);
            var calculation = demoAssessmentSection.PipingFailureMechanism.CalculationsGroup.GetPipingCalculations().First();
            AssertCalculationAbleToCalculate(calculation);

            foreach (var failureMechanism in demoAssessmentSection.GetFailureMechanisms())
            {
                Assert.AreEqual(283, failureMechanism.Sections.Count());
            }
            mocks.VerifyAll();
        }

        private void AssertValuesOnHydraulicBoundaryLocations(HydraulicBoundaryLocation[] hydraulicBoundaryLocations)
        {
            Assert.AreEqual(5.78, hydraulicBoundaryLocations[0].DesignWaterLevel);
            Assert.AreEqual(5.77, hydraulicBoundaryLocations[1].DesignWaterLevel);
            Assert.AreEqual(5.77, hydraulicBoundaryLocations[2].DesignWaterLevel);
            Assert.AreEqual(5.77, hydraulicBoundaryLocations[3].DesignWaterLevel);
            Assert.AreEqual(5.77, hydraulicBoundaryLocations[4].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[5].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[6].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[7].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[8].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[9].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[10].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[11].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[12].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[13].DesignWaterLevel);
            Assert.AreEqual(5.93, hydraulicBoundaryLocations[14].DesignWaterLevel);
            Assert.AreEqual(5.54, hydraulicBoundaryLocations[15].DesignWaterLevel);
            Assert.AreEqual(5.86, hydraulicBoundaryLocations[16].DesignWaterLevel);
            Assert.AreEqual(6.0, hydraulicBoundaryLocations[17].DesignWaterLevel);
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

            Assert.AreEqual(new Point3D(155883.762, 569864.416, 0.53), surfaceLine1.DitchPolderSide);
            Assert.AreEqual(new Point3D(155882.067, 569866.157, -1.9), surfaceLine1.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D(155874.184, 569874.252, -1.9), surfaceLine1.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D(155872.224, 569876.265, 0.91), surfaceLine1.DitchDikeSide);
            Assert.AreEqual(new Point3D(155864.173, 569884.532, 0.95), surfaceLine1.DikeToeAtPolder);
            Assert.AreEqual(new Point3D(155797.109, 569953.4, -4), surfaceLine1.DikeToeAtRiver);

            Assert.IsNull(surfaceLine2.DitchPolderSide);
            Assert.IsNull(surfaceLine2.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine2.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine2.DitchDikeSide);
            Assert.AreEqual(new Point3D(155558.754, 569618.729, 1.45), surfaceLine2.DikeToeAtPolder);
            Assert.AreEqual(new Point3D(155505.259, 569701.229, -4), surfaceLine2.DikeToeAtRiver);

            Assert.AreEqual(new Point3D(155063.763, 569276.113, -0.5), surfaceLine3.DitchPolderSide);
            Assert.AreEqual(new Point3D(155063.272, 569276.926, -1.45), surfaceLine3.BottomDitchPolderSide);
            Assert.AreEqual(new Point3D(155056.855, 569287.56, -1.45), surfaceLine3.BottomDitchDikeSide);
            Assert.AreEqual(new Point3D(155056.297, 569288.485, -0.37), surfaceLine3.DitchDikeSide);
            Assert.AreEqual(new Point3D(155047.587, 569302.917, 1.42), surfaceLine3.DikeToeAtPolder);
            Assert.AreEqual(new Point3D(154999.006, 569383.419, -4), surfaceLine3.DikeToeAtRiver);

            Assert.IsNull(surfaceLine4.DitchPolderSide);
            Assert.IsNull(surfaceLine4.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine4.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine4.DitchDikeSide);
            Assert.AreEqual(new Point3D(154682.383, 568112.623, 1.55), surfaceLine4.DikeToeAtPolder);
            Assert.AreEqual(new Point3D(154586.088, 568119.17, -4), surfaceLine4.DikeToeAtRiver);
        }

        private void AssertCalculationAbleToCalculate(PipingCalculationScenario calculation)
        {
            PipingInput inputParameters = calculation.InputParameters;
            AssertExpectedPipingInput(inputParameters);

            Assert.IsTrue(PipingCalculationService.Validate(calculation));

            PipingCalculationService.Calculate(calculation);
            Assert.IsTrue(calculation.HasOutput);
            Assert.AreEqual(0.563, calculation.Output.HeaveFactorOfSafety, 1e-3);
            Assert.AreEqual(-0.233, calculation.Output.HeaveZValue, 1e-3);
            Assert.AreEqual(3.377, calculation.Output.UpliftFactorOfSafety, 1e-3);
            Assert.AreEqual(7.358, calculation.Output.UpliftZValue, 1e-3);
            Assert.AreEqual(0.408, calculation.Output.SellmeijerFactorOfSafety, 1e-3);
            Assert.AreEqual(-1.588, calculation.Output.SellmeijerZValue, 1e-3);
        }

        private static void AssertExpectedPipingInput(PipingInput inputParameters)
        {
            Assert.AreEqual(1.0, inputParameters.UpliftModelFactor, 1e-3);
            Assert.AreEqual(1.0, inputParameters.SellmeijerModelFactor, 1e-3);

            Assert.AreEqual(9.81, inputParameters.WaterVolumetricWeight, 1e-3);
            Assert.AreEqual(0.3, inputParameters.SellmeijerReductionFactor, 1e-3);
            Assert.AreEqual(16.5, inputParameters.SandParticlesVolumicWeight, 1e-3);
            Assert.AreEqual(0.25, inputParameters.WhitesDragCoefficient, 1e-3);
            Assert.AreEqual(1.33e-6, inputParameters.WaterKinematicViscosity, 1e-3);
            Assert.AreEqual(9.81, inputParameters.Gravity, 1e-3);
            Assert.AreEqual(0.000208, inputParameters.MeanDiameter70, 1e-3);
            Assert.AreEqual(37, inputParameters.BeddingAngle, 1e-3);

            Assert.AreEqual("PK001_0001", inputParameters.SurfaceLine.Name);
            Assert.AreEqual("PK001_0001_Piping", inputParameters.StochasticSoilModel.Name);
            Assert.AreEqual("W1-6_0_1D1", inputParameters.StochasticSoilProfile.SoilProfile.Name);
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
            Assert.AreEqual(5.78, inputParameters.HydraulicBoundaryLocation.DesignWaterLevel, 1e-3);

            Assert.AreEqual(0.7, PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DampingFactorExit));
            Assert.AreEqual(1.355, PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.PhreaticLevelExit));
            Assert.AreEqual(0.011, PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.Diameter70));
            Assert.AreEqual(2.347, PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DarcyPermeability));

            Assert.AreEqual(4.45, inputParameters.PiezometricHeadExit, 1e-2);
            Assert.AreEqual(106.13, inputParameters.ExitPointL, 1e-2);
            Assert.AreEqual(81.45, PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DampingFactorExit));
            Assert.AreEqual(5.81, PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DampingFactorExit));
            Assert.AreEqual(20.29, PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DampingFactorExit));
        }

        private static double GetAccuracy(IDistribution distribution)
        {
            return Math.Pow(10.0, -distribution.Mean.NumberOfDecimalPlaces);
        }
    }
}
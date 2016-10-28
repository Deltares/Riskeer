// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

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
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_GuiIsProperlyInitialized_AddNewAssessmentSectionWithDemoDataToRootProject()
        {
            // Setup
            var project = new RingtoetsProject();

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
            Assert.AreEqual(1, project.AssessmentSections.Count);
            var demoAssessmentSection = project.AssessmentSections[0];
            Assert.AreEqual("Demo traject", demoAssessmentSection.Name);
            Assert.AreEqual("6-3", demoAssessmentSection.Id);

            AssertHydraulicBoundaryDatabase(demoAssessmentSection);

            Assert.AreEqual(2380, demoAssessmentSection.ReferenceLine.Points.Count());

            foreach (var failureMechanism in demoAssessmentSection.GetFailureMechanisms())
            {
                Assert.AreEqual(283, failureMechanism.Sections.Count());
            }

            AssertGrassCoverErosionInwardsFailureMechanism(demoAssessmentSection);
            AssertGrassCoverErosionOutwardsFailureMechanism(demoAssessmentSection);
            AssertHeightStructuresFailureMechanism(demoAssessmentSection);
            AssertClosingStructuresFailureMechanism(demoAssessmentSection);
            AssertPipingFailureMechanism(demoAssessmentSection);
            AssertStabilityPointStructuresFailureMechanism(demoAssessmentSection);
            AssertStabilityStoneCoverFailureMechanism(demoAssessmentSection);
            AssertWaveImpactAsphaltCoverFailureMechanism(demoAssessmentSection);
            mocks.VerifyAll();
        }

        private static void AssertCharacteristicPointsOnSurfaceLines(RingtoetsPipingSurfaceLine[] surfaceLines)
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

        private static double GetAccuracy(IDistribution distribution)
        {
            return Math.Pow(10.0, -distribution.Mean.NumberOfDecimalPlaces);
        }

        #region FailureMechanisms

        #region GrassCoverErosionInwardsFailureMechanism

        private static void AssertGrassCoverErosionInwardsFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            Assert.AreEqual(1, demoAssessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Count);
            GrassCoverErosionInwardsCalculation calculation = demoAssessmentSection.GrassCoverErosionInwards
                                                                                   .CalculationsGroup.GetCalculations()
                                                                                   .OfType<GrassCoverErosionInwardsCalculation>()
                                                                                   .First();
            AssertExpectedGrassCoverErosionInwardsInput(calculation.InputParameters);
        }

        private static void AssertExpectedGrassCoverErosionInwardsInput(GrassCoverErosionInwardsInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
        }

        #endregion

        #region GrassCoverErosionOutwardsFailureMechanism

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            HydraulicBoundaryLocation[] hydraulicBoundaryLocations = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();

            Assert.AreEqual(demoAssessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Count, hydraulicBoundaryLocations.Length);

            AssertDesignWaterLevelValuesOnGrassCoverErosionOutwardsHydraulicBoundaryLocation(demoAssessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.ToArray());
            AssertDesignWaterLevelConvergenceOnGrassCoverErosionOutwardsHydraulicBoundaryLocations(hydraulicBoundaryLocations);

            AssertWaveHeightValuesOnGrassCoverErosionOutwardsHydraulicBoundaryLocation(demoAssessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.ToArray());
            AssertWaveHeightConvergenceOnGrassCoverErosionOutwardsHydraulicBoundaryLocations(hydraulicBoundaryLocations);

            Assert.AreEqual(1, demoAssessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Count);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = demoAssessmentSection.GrassCoverErosionOutwards
                                                                                                  .WaveConditionsCalculationGroup.GetCalculations()
                                                                                                  .OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                                  .First();
            AssertExpectedGrassCoverErosionOutwardsWaveConditionsInput(calculation.InputParameters);
        }

        private static void AssertExpectedGrassCoverErosionOutwardsWaveConditionsInput(WaveConditionsInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
        }

        private static void AssertDesignWaterLevelValuesOnGrassCoverErosionOutwardsHydraulicBoundaryLocation(HydraulicBoundaryLocation[] locations)
        {
            Assert.AreEqual((RoundedDouble) 7.19, locations[0].DesignWaterLevel, locations[0].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.19, locations[1].DesignWaterLevel, locations[1].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.18, locations[2].DesignWaterLevel, locations[2].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.18, locations[3].DesignWaterLevel, locations[3].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.18, locations[4].DesignWaterLevel, locations[4].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.39, locations[5].DesignWaterLevel, locations[5].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.39, locations[6].DesignWaterLevel, locations[6].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.39, locations[7].DesignWaterLevel, locations[7].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.40, locations[8].DesignWaterLevel, locations[8].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.40, locations[9].DesignWaterLevel, locations[9].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.40, locations[10].DesignWaterLevel, locations[10].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.40, locations[11].DesignWaterLevel, locations[11].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.41, locations[12].DesignWaterLevel, locations[12].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.41, locations[13].DesignWaterLevel, locations[13].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.41, locations[14].DesignWaterLevel, locations[14].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 6.91, locations[15].DesignWaterLevel, locations[15].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.53, locations[16].DesignWaterLevel, locations[16].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 7.81, locations[17].DesignWaterLevel, locations[17].DesignWaterLevel.GetAccuracy());
        }

        private static void AssertDesignWaterLevelConvergenceOnGrassCoverErosionOutwardsHydraulicBoundaryLocations(HydraulicBoundaryLocation[] locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            }
        }

        private static void AssertWaveHeightValuesOnGrassCoverErosionOutwardsHydraulicBoundaryLocation(HydraulicBoundaryLocation[] locations)
        {
            Assert.AreEqual((RoundedDouble) 4.99, locations[0].WaveHeight, locations[0].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.04, locations[1].WaveHeight, locations[1].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.87, locations[2].WaveHeight, locations[2].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.73, locations[3].WaveHeight, locations[3].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.59, locations[4].WaveHeight, locations[4].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.35, locations[5].WaveHeight, locations[5].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.83, locations[6].WaveHeight, locations[6].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.00, locations[7].WaveHeight, locations[7].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.20, locations[8].WaveHeight, locations[8].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.41, locations[9].WaveHeight, locations[9].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.50, locations[10].WaveHeight, locations[10].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.57, locations[11].WaveHeight, locations[11].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.63, locations[12].WaveHeight, locations[12].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.68, locations[13].WaveHeight, locations[13].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.17, locations[14].WaveHeight, locations[14].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 11.14, locations[15].WaveHeight, locations[15].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 9.24, locations[16].WaveHeight, locations[16].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.34, locations[17].WaveHeight, locations[17].WaveHeight.GetAccuracy());
        }

        private static void AssertWaveHeightConvergenceOnGrassCoverErosionOutwardsHydraulicBoundaryLocations(HydraulicBoundaryLocation[] locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            }
        }

        #endregion

        #region HeightStructuresFailureMechanism

        private static void AssertHeightStructuresFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            Assert.AreEqual(1, demoAssessmentSection.HeightStructures.HeightStructures.Count);
            AssertExpectedHeightStructureValues(demoAssessmentSection.HeightStructures.HeightStructures[0]);

            Assert.AreEqual(1, demoAssessmentSection.HeightStructures.CalculationsGroup.Children.Count);
            StructuresCalculation<HeightStructuresInput> calculation = demoAssessmentSection.HeightStructures
                                                                           .Calculations
                                                                           .OfType<StructuresCalculation<HeightStructuresInput>>()
                                                                           .First();
            AssertExpectedHeightStructuresInput(calculation.InputParameters);
        }

        private static void AssertExpectedHeightStructureValues(HeightStructure heightStructure)
        {
            Assert.AreEqual("KUNST1", heightStructure.Name);
            Assert.AreEqual("KUNST1", heightStructure.Id);
            Assert.AreEqual(new Point2D(12345.56789, 9876.54321), heightStructure.Location);
            Assert.AreEqual(10.0, heightStructure.StructureNormalOrientation.Value);
            Assert.AreEqual(4.95, heightStructure.LevelCrestStructure.Mean.Value);
            Assert.AreEqual(0.05, heightStructure.LevelCrestStructure.StandardDeviation.Value);
            Assert.AreEqual(25.0, heightStructure.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(0.05, heightStructure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(0.1, heightStructure.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(0.15, heightStructure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(21.0, heightStructure.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(0.05, heightStructure.WidthFlowApertures.CoefficientOfVariation.Value);
            Assert.AreEqual(1.0, heightStructure.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(20000.0, heightStructure.StorageStructureArea.Mean.Value);
            Assert.AreEqual(0.1, heightStructure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(0.2, heightStructure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(0.1, heightStructure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
        }

        private static void AssertExpectedHeightStructuresInput(HeightStructuresInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
            Assert.AreEqual(10, inputParameters.DeviationWaveDirection, inputParameters.DeviationWaveDirection.GetAccuracy());
            AssertExpectedHeightStructureValues(inputParameters.Structure);
        }

        #endregion

        #region ClosingStructuresFailureMechanism

        private static void AssertClosingStructuresFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            Assert.AreEqual(1, demoAssessmentSection.ClosingStructures.ClosingStructures.Count);
            AssertExpectedClosingStructureValues(demoAssessmentSection.ClosingStructures.ClosingStructures[0]);

            Assert.AreEqual(1, demoAssessmentSection.ClosingStructures.CalculationsGroup.Children.Count);
            StructuresCalculation<ClosingStructuresInput> calculation = demoAssessmentSection.ClosingStructures
                                                                            .Calculations
                                                                            .OfType<StructuresCalculation<ClosingStructuresInput>>()
                                                                            .First();
            AssertExpectedClosingStructuresInput(calculation.InputParameters);
        }

        private static void AssertExpectedClosingStructureValues(ClosingStructure closingStructure)
        {
            Assert.AreEqual("KUNST1", closingStructure.Name);
            Assert.AreEqual("KUNST1", closingStructure.Id);
            Assert.AreEqual(new Point2D(12345.56789, 9876.54321), closingStructure.Location);
            Assert.AreEqual(20000.0, closingStructure.StorageStructureArea.Mean.Value);
            Assert.AreEqual(0.1, closingStructure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(0.2, closingStructure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(0.1, closingStructure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
            Assert.AreEqual(10.0, closingStructure.StructureNormalOrientation.Value);
            Assert.AreEqual(21.0, closingStructure.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(0.05, closingStructure.WidthFlowApertures.CoefficientOfVariation.Value);
            Assert.AreEqual(4.95, closingStructure.LevelCrestStructureNotClosing.Mean.Value);
            Assert.AreEqual(0.05, closingStructure.LevelCrestStructureNotClosing.StandardDeviation.Value);
            Assert.AreEqual(0.5, closingStructure.InsideWaterLevel.Mean.Value);
            Assert.AreEqual(0.1, closingStructure.InsideWaterLevel.StandardDeviation.Value);
            Assert.AreEqual(4.95, closingStructure.ThresholdHeightOpenWeir.Mean.Value);
            Assert.AreEqual(0.1, closingStructure.ThresholdHeightOpenWeir.StandardDeviation.Value);
            Assert.AreEqual(31.5, closingStructure.AreaFlowApertures.Mean.Value);
            Assert.AreEqual(0.01, closingStructure.AreaFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(1.0, closingStructure.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(0.15, closingStructure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(25.0, closingStructure.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(0.05, closingStructure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(1.0, closingStructure.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(0.1, closingStructure.FailureProbabilityOpenStructure);
            Assert.AreEqual(4, closingStructure.IdenticalApertures);
            Assert.AreEqual(1.0, closingStructure.FailureProbabilityReparation);
            Assert.AreEqual(ClosingStructureInflowModelType.VerticalWall, closingStructure.InflowModelType);
        }

        private static void AssertExpectedClosingStructuresInput(ClosingStructuresInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
            Assert.AreEqual(10, inputParameters.DeviationWaveDirection, inputParameters.DeviationWaveDirection.GetAccuracy());
            AssertExpectedClosingStructureValues(inputParameters.Structure);
        }

        #endregion

        #region PipingFailureMechanism

        private static void AssertPipingFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            var soilModels = demoAssessmentSection.PipingFailureMechanism.StochasticSoilModels.ToArray();
            Assert.AreEqual(4, soilModels.Length);
            var surfaceLines = demoAssessmentSection.PipingFailureMechanism.SurfaceLines.ToArray();
            Assert.AreEqual(4, surfaceLines.Length);
            AssertCharacteristicPointsOnSurfaceLines(surfaceLines);

            Assert.AreEqual(1, demoAssessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Count);
            PipingCalculationScenario pipingCalculationScenario = demoAssessmentSection.PipingFailureMechanism
                                                                                       .CalculationsGroup.GetCalculations()
                                                                                       .OfType<PipingCalculationScenario>()
                                                                                       .First();
            AssertCalculationAbleToCalculate(pipingCalculationScenario);
            AssertCalculationInFailureMechanismSectionResult(
                pipingCalculationScenario,
                demoAssessmentSection.PipingFailureMechanism.SectionResults.ToArray(),
                demoAssessmentSection.PipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>());
        }

        private static void AssertExpectedPipingInput(PipingInput inputParameters)
        {
            Console.WriteLine("{0} en {1}", Math.Exp(-0.5), Math.Sqrt((Math.Exp(1) - 1)*Math.Exp(1)));
            Assert.AreEqual(1.0, inputParameters.UpliftModelFactor, 1e-3);
            Assert.AreEqual(1.0, inputParameters.SellmeijerModelFactor, 1e-3);

            Assert.AreEqual(9.81, inputParameters.WaterVolumetricWeight, 1e-3);
            Assert.AreEqual(0.3, inputParameters.SellmeijerReductionFactor, 1e-3);
            Assert.AreEqual(16.5, inputParameters.SandParticlesVolumicWeight, 1e-3);
            Assert.AreEqual(0.25, inputParameters.WhitesDragCoefficient, 1e-3);
            Assert.AreEqual(1.33e-6, inputParameters.WaterKinematicViscosity, 1e-3);
            Assert.AreEqual(9.81, inputParameters.Gravity, 1e-3);
            Assert.AreEqual(0.000, inputParameters.MeanDiameter70, 1e-3);
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
            Assert.AreEqual(0.011453, PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.Diameter70));
            Assert.AreEqual(2.345281, PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DarcyPermeability));
            Assert.AreEqual(17.5, PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.SaturatedVolumicWeightOfCoverageLayer));

            Assert.AreEqual(4.45, inputParameters.PiezometricHeadExit, 1e-2);
            Assert.AreEqual(106.13, inputParameters.ExitPointL, 1e-2);
            Assert.AreEqual(81.18, PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DampingFactorExit));
            Assert.AreEqual(5.86, PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DampingFactorExit));
            Assert.AreEqual(20.13, PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(inputParameters).GetDesignValue(),
                            GetAccuracy(inputParameters.DampingFactorExit));
        }

        private static void AssertCalculationAbleToCalculate(PipingCalculationScenario calculation)
        {
            PipingInput inputParameters = calculation.InputParameters;
            AssertExpectedPipingInput(inputParameters);

            Assert.IsTrue(PipingCalculationService.Validate(calculation));

            PipingCalculationService.Calculate(calculation);
            Assert.IsTrue(calculation.HasOutput);
            Assert.AreEqual(0.568, calculation.Output.HeaveFactorOfSafety, 1e-3);
            Assert.AreEqual(-0.228, calculation.Output.HeaveZValue, 1e-3);
            Assert.AreEqual(1.484, calculation.Output.UpliftFactorOfSafety, 1e-3);
            Assert.AreEqual(1.499, calculation.Output.UpliftZValue, 1e-3);
            Assert.AreEqual(0.432, calculation.Output.SellmeijerFactorOfSafety, 1e-3);
            Assert.AreEqual(-1.514, calculation.Output.SellmeijerZValue, 1e-3);
        }

        private static void AssertCalculationInFailureMechanismSectionResult(PipingCalculationScenario calculation, PipingFailureMechanismSectionResult[] sectionResults, IEnumerable<PipingCalculationScenario> calculations)
        {
            Assert.AreEqual(283, sectionResults.Length);
            var sectionResultWithCalculation = sectionResults[22];

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, sectionResultWithCalculation.GetCalculationScenarios(calculations));
        }

        #endregion

        #region StabilityPointStructuresFailureMechanism

        private static void AssertStabilityPointStructuresFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            Assert.AreEqual(1, demoAssessmentSection.StabilityPointStructures.CalculationsGroup.Children.Count);
            AssertExpectedStabilityPointStructureValues(demoAssessmentSection.StabilityPointStructures.StabilityPointStructures[0]);
            StructuresCalculation<StabilityPointStructuresInput> calculation = demoAssessmentSection.StabilityPointStructures
                                                                                   .Calculations
                                                                                   .OfType<StructuresCalculation<StabilityPointStructuresInput>>()
                                                                                   .First();
            AssertExpectedStabilityPointStructuresInput(calculation.InputParameters);
        }

        private static void AssertExpectedStabilityPointStructureValues(StabilityPointStructure structure)
        {
            Assert.AreEqual("Kunstwerk", structure.Name);
            Assert.AreEqual("Kunstwerk id", structure.Id);
            Assert.AreEqual(new Point2D(131470.777221421, 548329.82912364), structure.Location);
            Assert.AreEqual(10, structure.StructureNormalOrientation, structure.StructureNormalOrientation.GetAccuracy());
            Assert.AreEqual(20000, structure.StorageStructureArea.Mean, structure.StorageStructureArea.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.StorageStructureArea.CoefficientOfVariation,
                            structure.StorageStructureArea.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(0.2, structure.AllowedLevelIncreaseStorage.Mean, structure.AllowedLevelIncreaseStorage.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.AllowedLevelIncreaseStorage.StandardDeviation,
                            structure.AllowedLevelIncreaseStorage.StandardDeviation.GetAccuracy());
            Assert.AreEqual(21, structure.WidthFlowApertures.Mean, structure.WidthFlowApertures.Mean.GetAccuracy());
            Assert.AreEqual(0.05, structure.WidthFlowApertures.CoefficientOfVariation, structure.WidthFlowApertures.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(0.5, structure.InsideWaterLevel.Mean, structure.InsideWaterLevel.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.InsideWaterLevel.StandardDeviation, structure.InsideWaterLevel.StandardDeviation.GetAccuracy());
            Assert.AreEqual(4.95, structure.ThresholdHeightOpenWeir.Mean, structure.ThresholdHeightOpenWeir.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.ThresholdHeightOpenWeir.StandardDeviation, structure.ThresholdHeightOpenWeir.StandardDeviation.GetAccuracy());
            Assert.AreEqual(1, structure.CriticalOvertoppingDischarge.Mean, structure.CriticalOvertoppingDischarge.Mean.GetAccuracy());
            Assert.AreEqual(0.15, structure.CriticalOvertoppingDischarge.CoefficientOfVariation,
                            structure.CriticalOvertoppingDischarge.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(25, structure.FlowWidthAtBottomProtection.Mean, structure.FlowWidthAtBottomProtection.Mean.GetAccuracy());
            Assert.AreEqual(1.25, structure.FlowWidthAtBottomProtection.StandardDeviation,
                            structure.FlowWidthAtBottomProtection.StandardDeviation.GetAccuracy());
            Assert.AreEqual(10, structure.ConstructiveStrengthLinearLoadModel.Mean, structure.ConstructiveStrengthLinearLoadModel.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                            structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(10, structure.ConstructiveStrengthQuadraticLoadModel.Mean,
                            structure.ConstructiveStrengthQuadraticLoadModel.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                            structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(0, structure.BankWidth.Mean, structure.BankWidth.Mean.GetAccuracy());
            Assert.AreEqual(0, structure.BankWidth.StandardDeviation, structure.BankWidth.StandardDeviation.GetAccuracy());
            Assert.AreEqual(0.5, structure.InsideWaterLevelFailureConstruction.Mean, structure.InsideWaterLevelFailureConstruction.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.InsideWaterLevelFailureConstruction.StandardDeviation,
                            structure.InsideWaterLevelFailureConstruction.StandardDeviation.GetAccuracy());
            Assert.AreEqual(0, structure.EvaluationLevel, structure.EvaluationLevel.GetAccuracy());
            Assert.AreEqual(4.95, structure.LevelCrestStructure.Mean, structure.LevelCrestStructure.Mean.GetAccuracy());
            Assert.AreEqual(0.05, structure.LevelCrestStructure.StandardDeviation, structure.LevelCrestStructure.StandardDeviation.GetAccuracy());
            Assert.AreEqual(0, structure.VerticalDistance, structure.VerticalDistance.GetAccuracy());
            Assert.AreEqual(0.5, structure.FailureProbabilityRepairClosure);
            Assert.AreEqual(10, structure.FailureCollisionEnergy.Mean, structure.FailureCollisionEnergy.Mean.GetAccuracy());
            Assert.AreEqual(0.3, structure.FailureCollisionEnergy.CoefficientOfVariation,
                            structure.FailureCollisionEnergy.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(16000, structure.ShipMass.Mean, structure.ShipMass.Mean.GetAccuracy());
            Assert.AreEqual(0.2, structure.ShipMass.CoefficientOfVariation, structure.ShipMass.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(2, structure.ShipVelocity.Mean, structure.ShipVelocity.Mean.GetAccuracy());
            Assert.AreEqual(0.2, structure.ShipVelocity.CoefficientOfVariation, structure.ShipVelocity.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(0, structure.LevellingCount);
            Assert.AreEqual(0, structure.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(1, structure.FlowVelocityStructureClosable.Mean, structure.FlowVelocityStructureClosable.Mean.GetAccuracy());
            Assert.AreEqual(1, structure.FlowVelocityStructureClosable.StandardDeviation,
                            structure.FlowVelocityStructureClosable.StandardDeviation.GetAccuracy());
            Assert.AreEqual(15, structure.StabilityLinearLoadModel.Mean, structure.StabilityLinearLoadModel.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.StabilityLinearLoadModel.CoefficientOfVariation,
                            structure.StabilityLinearLoadModel.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(15, structure.StabilityQuadraticLoadModel.Mean, structure.StabilityQuadraticLoadModel.Mean.GetAccuracy());
            Assert.AreEqual(0.1, structure.StabilityQuadraticLoadModel.CoefficientOfVariation,
                            structure.StabilityQuadraticLoadModel.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(2.5, structure.AreaFlowApertures.Mean, structure.AreaFlowApertures.Mean.GetAccuracy());
            Assert.AreEqual(0.01, structure.AreaFlowApertures.StandardDeviation, structure.AreaFlowApertures.StandardDeviation.GetAccuracy());
            Assert.AreEqual(StabilityPointStructureInflowModelType.FloodedCulvert, structure.InflowModelType);
        }

        private static void AssertExpectedStabilityPointStructuresInput(StabilityPointStructuresInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
        }

        #endregion

        #region StabilityStoneCoverFailureMechanism

        private static void AssertStabilityStoneCoverFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            Assert.AreEqual(1, demoAssessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Count);
            StabilityStoneCoverWaveConditionsCalculation calculation = demoAssessmentSection.StabilityStoneCover
                                                                                            .WaveConditionsCalculationGroup.GetCalculations()
                                                                                            .OfType<StabilityStoneCoverWaveConditionsCalculation>()
                                                                                            .First();
            AssertExpectedStabilityStoneCoverWaveConditionsInputInput(calculation.InputParameters);
        }

        private static void AssertExpectedStabilityStoneCoverWaveConditionsInputInput(WaveConditionsInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
        }

        #endregion

        #region WaveImpactAsphaltCoverFailureMechanism

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            Assert.AreEqual(1, demoAssessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Count);
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = demoAssessmentSection.WaveImpactAsphaltCover
                                                                                               .WaveConditionsCalculationGroup.GetCalculations()
                                                                                               .OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                                                               .First();
            AssertExpectedWaveImpactAsphaltCoverWaveConditionsInputInput(calculation.InputParameters);
        }

        private static void AssertExpectedWaveImpactAsphaltCoverWaveConditionsInputInput(WaveConditionsInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
        }

        #endregion

        #endregion

        #region HydraulicBoundaryDatabase

        private static void AssertHydraulicBoundaryDatabase(AssessmentSection demoAssessmentSection)
        {
            Assert.IsNotEmpty(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath);
            Assert.IsTrue(File.Exists(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath));
            var directoryName = Path.GetDirectoryName(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath);
            Assert.IsNotNull(directoryName);
            Assert.IsTrue(File.Exists(Path.Combine(directoryName, "HLCD.sqlite")));
            var hydraulicBoundaryLocations = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(18, hydraulicBoundaryLocations.Length);
            AssertDesignWaterLevelValuesOnHydraulicBoundaryLocations(hydraulicBoundaryLocations);
            AssertDesignWaterLevelCalculationConvergenceOnHydraulicBoundaryLocations(hydraulicBoundaryLocations);
            AssertWaveHeightValuesOnHydraulicBoundaryLocations(hydraulicBoundaryLocations);
            AssertWaveHeightCalculationConvergenceOnHydraulicBoundaryLocations(hydraulicBoundaryLocations);
        }

        private static void AssertDesignWaterLevelValuesOnHydraulicBoundaryLocations(HydraulicBoundaryLocation[] locations)
        {
            Assert.AreEqual((RoundedDouble) 5.78, locations[0].DesignWaterLevel, locations[0].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.77, locations[1].DesignWaterLevel, locations[1].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.77, locations[2].DesignWaterLevel, locations[2].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.77, locations[3].DesignWaterLevel, locations[3].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.77, locations[4].DesignWaterLevel, locations[4].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[5].DesignWaterLevel, locations[5].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[6].DesignWaterLevel, locations[6].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[7].DesignWaterLevel, locations[7].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[8].DesignWaterLevel, locations[8].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[9].DesignWaterLevel, locations[9].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[10].DesignWaterLevel, locations[10].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[11].DesignWaterLevel, locations[11].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[12].DesignWaterLevel, locations[12].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[13].DesignWaterLevel, locations[13].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.93, locations[14].DesignWaterLevel, locations[14].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.54, locations[15].DesignWaterLevel, locations[15].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 5.86, locations[16].DesignWaterLevel, locations[16].DesignWaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 6.0, locations[17].DesignWaterLevel, locations[17].DesignWaterLevel.GetAccuracy());
        }

        private static void AssertDesignWaterLevelCalculationConvergenceOnHydraulicBoundaryLocations(HydraulicBoundaryLocation[] locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            }
        }

        private static void AssertWaveHeightValuesOnHydraulicBoundaryLocations(HydraulicBoundaryLocation[] locations)
        {
            Assert.AreEqual((RoundedDouble) 4.13374, locations[0].WaveHeight, locations[0].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.19044, locations[1].WaveHeight, locations[1].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.01717, locations[2].WaveHeight, locations[2].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.87408, locations[3].WaveHeight, locations[3].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.73281, locations[4].WaveHeight, locations[4].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 2.65268, locations[5].WaveHeight, locations[5].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.04333, locations[6].WaveHeight, locations[6].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.19952, locations[7].WaveHeight, locations[7].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.3554, locations[8].WaveHeight, locations[8].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.52929, locations[9].WaveHeight, locations[9].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.62194, locations[10].WaveHeight, locations[10].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.6851, locations[11].WaveHeight, locations[11].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.72909, locations[12].WaveHeight, locations[12].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.74794, locations[13].WaveHeight, locations[13].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 3.29686, locations[14].WaveHeight, locations[14].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 9.57558, locations[15].WaveHeight, locations[15].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 8.01959, locations[16].WaveHeight, locations[16].WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 4.11447, locations[17].WaveHeight, locations[17].WaveHeight.GetAccuracy());
        }

        private static void AssertWaveHeightCalculationConvergenceOnHydraulicBoundaryLocations(HydraulicBoundaryLocation[] locations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            }
        }

        #endregion
    }
}
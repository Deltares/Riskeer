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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Demo.Riskeer.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service;
using Riskeer.Revetment.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Demo.Riskeer.Test.Commands
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
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var command = new AddNewDemoAssessmentSectionCommand(projectOwner, viewCommands);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_GuiIsProperlyInitialized_AddNewAssessmentSectionWithDemoDataToRootProject()
        {
            // Setup
            var project = new RiskeerProject();

            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var command = new AddNewDemoAssessmentSectionCommand(projectOwner, viewCommands);
            project.Attach(observer);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.AssessmentSections.Count);
            AssessmentSection demoAssessmentSection = project.AssessmentSections[0];
            Assert.AreEqual("Demo traject", demoAssessmentSection.Name);
            Assert.AreEqual("6-3", demoAssessmentSection.Id);

            AssertHydraulicBoundaryDatabase(demoAssessmentSection);

            Assert.AreEqual(2380, demoAssessmentSection.ReferenceLine.Points.Count());

            foreach (IFailureMechanism failureMechanism in demoAssessmentSection.GetFailureMechanisms())
            {
                Assert.AreEqual(283, failureMechanism.Sections.Count());
                Assert.IsTrue(failureMechanism.FailureMechanismSectionSourcePath.Contains("traject_6-3_vakken.shp"));
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

        #region General

        private static void AssertCalculationConvergence(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            Assert.IsTrue(calculations.All(c => c.Output.CalculationConvergence == CalculationConvergence.CalculatedConverged));
        }

        #region WaveConditions

        private static void AssertFailureMechanismCategoryWaveConditionsInput(FailureMechanismCategoryWaveConditionsInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
            Assert.AreEqual(FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm, inputParameters.CategoryType);
        }

        private static void AssertAssessmentSectionCategoryWaveConditionsInput(AssessmentSectionCategoryWaveConditionsInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
            Assert.AreEqual(AssessmentSectionCategoryType.LowerLimitNorm, inputParameters.CategoryType);
        }

        #endregion

        #endregion

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
            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = demoAssessmentSection.HydraulicBoundaryDatabase.Locations;
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = demoAssessmentSection.GrassCoverErosionOutwards;

            AssertHydraulicBoundaryLocationCalculations(grassCoverErosionOutwardsFailureMechanism, hydraulicBoundaryLocations);
            AssertDesignWaterLevelsForGrassCoverErosionOutwards(grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            AssertCalculationConvergence(grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            AssertWaveHeightsForGrassCoverErosionOutwards(grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
            AssertCalculationConvergence(grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);

            Assert.AreEqual(1, demoAssessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Count);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.GetCalculations()
                                                                                                                      .OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                                                      .First();
            AssertFailureMechanismCategoryWaveConditionsInput(calculation.InputParameters);
        }

        private static void AssertHydraulicBoundaryLocationCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IList<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
        }

        private static void AssertDesignWaterLevelsForGrassCoverErosionOutwards(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            AssertHydraulicBoundaryCalculationResult(7.19, calculations, 0);
            AssertHydraulicBoundaryCalculationResult(7.19, calculations, 1);
            AssertHydraulicBoundaryCalculationResult(7.18, calculations, 2);
            AssertHydraulicBoundaryCalculationResult(7.18, calculations, 3);
            AssertHydraulicBoundaryCalculationResult(7.18, calculations, 4);
            AssertHydraulicBoundaryCalculationResult(7.39, calculations, 5);
            AssertHydraulicBoundaryCalculationResult(7.39, calculations, 6);
            AssertHydraulicBoundaryCalculationResult(7.39, calculations, 7);
            AssertHydraulicBoundaryCalculationResult(7.40, calculations, 8);
            AssertHydraulicBoundaryCalculationResult(7.40, calculations, 9);
            AssertHydraulicBoundaryCalculationResult(7.40, calculations, 10);
            AssertHydraulicBoundaryCalculationResult(7.40, calculations, 11);
            AssertHydraulicBoundaryCalculationResult(7.40, calculations, 12);
            AssertHydraulicBoundaryCalculationResult(7.41, calculations, 13);
            AssertHydraulicBoundaryCalculationResult(7.41, calculations, 14);
            AssertHydraulicBoundaryCalculationResult(6.91, calculations, 15);
            AssertHydraulicBoundaryCalculationResult(7.53, calculations, 16);
            AssertHydraulicBoundaryCalculationResult(7.80, calculations, 17);
        }

        private static void AssertWaveHeightsForGrassCoverErosionOutwards(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            AssertHydraulicBoundaryCalculationResult(4.99, calculations, 0);
            AssertHydraulicBoundaryCalculationResult(5.04, calculations, 1);
            AssertHydraulicBoundaryCalculationResult(4.87, calculations, 2);
            AssertHydraulicBoundaryCalculationResult(4.73, calculations, 3);
            AssertHydraulicBoundaryCalculationResult(4.59, calculations, 4);
            AssertHydraulicBoundaryCalculationResult(3.35, calculations, 5);
            AssertHydraulicBoundaryCalculationResult(3.83, calculations, 6);
            AssertHydraulicBoundaryCalculationResult(4.00, calculations, 7);
            AssertHydraulicBoundaryCalculationResult(4.20, calculations, 8);
            AssertHydraulicBoundaryCalculationResult(4.41, calculations, 9);
            AssertHydraulicBoundaryCalculationResult(4.50, calculations, 10);
            AssertHydraulicBoundaryCalculationResult(4.57, calculations, 11);
            AssertHydraulicBoundaryCalculationResult(4.63, calculations, 12);
            AssertHydraulicBoundaryCalculationResult(4.68, calculations, 13);
            AssertHydraulicBoundaryCalculationResult(4.17, calculations, 14);
            AssertHydraulicBoundaryCalculationResult(11.13, calculations, 15);
            AssertHydraulicBoundaryCalculationResult(9.24, calculations, 16);
            AssertHydraulicBoundaryCalculationResult(5.34, calculations, 17);
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
            Assert.AreEqual("Kunstwerk 1", heightStructure.Name);
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
            Assert.AreEqual(0.05, heightStructure.WidthFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(1.0, heightStructure.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(20000.0, heightStructure.StorageStructureArea.Mean.Value);
            Assert.AreEqual(0.1, heightStructure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(0.2, heightStructure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(0.1, heightStructure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
        }

        private static void AssertExpectedHeightStructuresInput(HeightStructuresInput inputParameters)
        {
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);
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
            Assert.AreEqual("Kunstwerk 1", closingStructure.Name);
            Assert.AreEqual("KUNST1", closingStructure.Id);
            Assert.AreEqual(new Point2D(12345.56789, 9876.54321), closingStructure.Location);
            Assert.AreEqual(20000.0, closingStructure.StorageStructureArea.Mean.Value);
            Assert.AreEqual(0.1, closingStructure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(0.2, closingStructure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(0.1, closingStructure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
            Assert.AreEqual(10.0, closingStructure.StructureNormalOrientation.Value);
            Assert.AreEqual(21.0, closingStructure.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(0.05, closingStructure.WidthFlowApertures.StandardDeviation.Value);
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
            AssertExpectedClosingStructureValues(inputParameters.Structure);
        }

        #endregion

        #region PipingFailureMechanism

        private static void AssertPipingFailureMechanism(AssessmentSection demoAssessmentSection)
        {
            PipingStochasticSoilModel[] soilModels = demoAssessmentSection.Piping.StochasticSoilModels.ToArray();
            Assert.AreEqual(4, soilModels.Length);
            PipingSurfaceLine[] surfaceLines = demoAssessmentSection.Piping.SurfaceLines.ToArray();
            Assert.AreEqual(4, surfaceLines.Length);
            AssertCharacteristicPointsOnSurfaceLines(surfaceLines);

            Assert.AreEqual(1, demoAssessmentSection.Piping.CalculationsGroup.Children.Count);
            PipingCalculationScenario pipingCalculationScenario = demoAssessmentSection.Piping
                                                                                       .CalculationsGroup.GetCalculations()
                                                                                       .OfType<PipingCalculationScenario>()
                                                                                       .First();
            AssertCalculationAbleToCalculate(pipingCalculationScenario, demoAssessmentSection);
            AssertCalculationInFailureMechanismSectionResult(
                pipingCalculationScenario,
                demoAssessmentSection.Piping.SectionResults.ToArray(),
                demoAssessmentSection.Piping.Calculations.OfType<PipingCalculationScenario>());
        }

        private static void AssertCharacteristicPointsOnSurfaceLines(PipingSurfaceLine[] surfaceLines)
        {
            PipingSurfaceLine surfaceLine1 = surfaceLines.FirstOrDefault(s => s.Name == "PK001_0001");
            PipingSurfaceLine surfaceLine2 = surfaceLines.FirstOrDefault(s => s.Name == "PK001_0002");
            PipingSurfaceLine surfaceLine3 = surfaceLines.FirstOrDefault(s => s.Name == "PK001_0003");
            PipingSurfaceLine surfaceLine4 = surfaceLines.FirstOrDefault(s => s.Name == "PK001_0004");

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

        private static void AssertExpectedPipingInput(PipingInput inputParameters)
        {
            Console.WriteLine(@"{0} en {1}", Math.Exp(-0.5), Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1)));
            Assert.AreEqual(1.0, inputParameters.UpliftModelFactor, 1e-3);
            Assert.AreEqual(1.0, inputParameters.SellmeijerModelFactor, 1e-3);

            Assert.AreEqual(9.81, inputParameters.WaterVolumetricWeight, 1e-3);
            Assert.AreEqual(0.3, inputParameters.SellmeijerReductionFactor, 1e-3);
            Assert.AreEqual(16.19, inputParameters.SandParticlesVolumicWeight, 1e-3);
            Assert.AreEqual(0.25, inputParameters.WhitesDragCoefficient, 1e-3);
            Assert.AreEqual(1.33e-6, inputParameters.WaterKinematicViscosity, 1e-3);
            Assert.AreEqual(9.81, inputParameters.Gravity, 1e-3);
            Assert.AreEqual(0.000, inputParameters.MeanDiameter70, 1e-3);
            Assert.AreEqual(37, inputParameters.BeddingAngle, 1e-3);

            Assert.AreEqual("PK001_0001", inputParameters.SurfaceLine.Name);
            Assert.AreEqual("PK001_0001_Piping", inputParameters.StochasticSoilModel.Name);
            Assert.AreEqual("W1-6_0_1D1", inputParameters.StochasticSoilProfile.SoilProfile.Name);
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);

            Assert.AreEqual(0.875, PipingSemiProbabilisticDesignVariableFactory.GetDampingFactorExit(inputParameters).GetDesignValue(),
                            inputParameters.DampingFactorExit.GetAccuracy());
            Assert.AreEqual(2.836, PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(inputParameters).GetDesignValue(),
                            inputParameters.PhreaticLevelExit.GetAccuracy());

            Assert.AreEqual(106.13, inputParameters.ExitPointL, 1e-2);
            Assert.AreEqual(81.18, PipingSemiProbabilisticDesignVariableFactory.GetSeepageLength(inputParameters).GetDesignValue(),
                            inputParameters.DampingFactorExit.GetAccuracy());
            Assert.AreEqual(5.86, PipingSemiProbabilisticDesignVariableFactory.GetThicknessCoverageLayer(inputParameters).GetDesignValue(),
                            inputParameters.DampingFactorExit.GetAccuracy());
            Assert.AreEqual(20.13, PipingSemiProbabilisticDesignVariableFactory.GetThicknessAquiferLayer(inputParameters).GetDesignValue(),
                            inputParameters.DampingFactorExit.GetAccuracy());
        }

        private static void AssertCalculationAbleToCalculate(PipingCalculationScenario calculation, IAssessmentSection assessmentSection)
        {
            PipingInput inputParameters = calculation.InputParameters;
            AssertExpectedPipingInput(inputParameters);

            RoundedDouble assessmentLevel = assessmentSection.GetNormativeAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation);

            Assert.IsTrue(PipingCalculationService.Validate(calculation, assessmentLevel));

            PipingCalculationService.Calculate(calculation, assessmentLevel);
            Assert.IsTrue(calculation.HasOutput);
            Assert.AreEqual(0.683, calculation.Output.HeaveFactorOfSafety, 1e-3);
            Assert.AreEqual(-0.139, calculation.Output.HeaveZValue, 1e-3);
            Assert.AreEqual(1.784, calculation.Output.UpliftFactorOfSafety, 1e-3);
            Assert.AreEqual(2.019, calculation.Output.UpliftZValue, 1e-3);
            Assert.AreEqual(1.199, calculation.Output.SellmeijerFactorOfSafety, 1e-3);
            Assert.AreEqual(0.237, calculation.Output.SellmeijerZValue, 1e-3);
        }

        private static void AssertCalculationInFailureMechanismSectionResult(PipingCalculationScenario calculation, PipingFailureMechanismSectionResult[] sectionResults, IEnumerable<PipingCalculationScenario> calculations)
        {
            Assert.AreEqual(283, sectionResults.Length);
            PipingFailureMechanismSectionResult sectionResultWithCalculation = sectionResults[22];

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
            Assert.AreEqual(0.05, structure.WidthFlowApertures.StandardDeviation, structure.WidthFlowApertures.StandardDeviation.GetAccuracy());
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
            AssertAssessmentSectionCategoryWaveConditionsInput(calculation.InputParameters);
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
            AssertAssessmentSectionCategoryWaveConditionsInput(calculation.InputParameters);
        }

        #endregion

        #endregion

        #region HydraulicBoundaryDatabase

        private static void AssertHydraulicBoundaryDatabase(AssessmentSection demoAssessmentSection)
        {
            Assert.IsNotEmpty(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath);
            Assert.IsTrue(File.Exists(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath));
            string directoryName = Path.GetDirectoryName(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath);
            Assert.IsNotNull(directoryName);
            Assert.IsTrue(File.Exists(Path.Combine(directoryName, "HLCD.sqlite")));

            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = demoAssessmentSection.HydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(18, hydraulicBoundaryLocations.Count);
            AssertHydraulicBoundaryLocationCalculations(demoAssessmentSection);
            AssertDesignWaterLevelsForAssessmentSection(demoAssessmentSection);
            AssertCalculationConvergence(demoAssessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            AssertWaveHeightsForAssessmentSection(demoAssessmentSection);
            AssertCalculationConvergence(demoAssessmentSection.WaveHeightCalculationsForLowerLimitNorm);
        }

        private static void AssertHydraulicBoundaryLocationCalculations(IAssessmentSection assessmentSection)
        {
            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;

            CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaterLevelCalculationsForSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaveHeightCalculationsForSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
        }

        private static void AssertDesignWaterLevelsForAssessmentSection(IAssessmentSection assessmentSection)
        {
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = assessmentSection.WaterLevelCalculationsForLowerLimitNorm;

            AssertHydraulicBoundaryCalculationResult(5.78, calculations, 0);
            AssertHydraulicBoundaryCalculationResult(5.77, calculations, 1);
            AssertHydraulicBoundaryCalculationResult(5.77, calculations, 2);
            AssertHydraulicBoundaryCalculationResult(5.77, calculations, 3);
            AssertHydraulicBoundaryCalculationResult(5.77, calculations, 4);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 5);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 6);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 7);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 8);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 9);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 10);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 11);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 12);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 13);
            AssertHydraulicBoundaryCalculationResult(5.93, calculations, 14);
            AssertHydraulicBoundaryCalculationResult(5.54, calculations, 15);
            AssertHydraulicBoundaryCalculationResult(5.86, calculations, 16);
            AssertHydraulicBoundaryCalculationResult(6.00, calculations, 17);
        }

        private static void AssertWaveHeightsForAssessmentSection(IAssessmentSection assessmentSection)
        {
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = assessmentSection.WaveHeightCalculationsForLowerLimitNorm;

            AssertHydraulicBoundaryCalculationResult(4.13, calculations, 0);
            AssertHydraulicBoundaryCalculationResult(4.19, calculations, 1);
            AssertHydraulicBoundaryCalculationResult(4.02, calculations, 2);
            AssertHydraulicBoundaryCalculationResult(3.87, calculations, 3);
            AssertHydraulicBoundaryCalculationResult(3.73, calculations, 4);
            AssertHydraulicBoundaryCalculationResult(2.65, calculations, 5);
            AssertHydraulicBoundaryCalculationResult(3.04, calculations, 6);
            AssertHydraulicBoundaryCalculationResult(3.20, calculations, 7);
            AssertHydraulicBoundaryCalculationResult(3.35, calculations, 8);
            AssertHydraulicBoundaryCalculationResult(3.53, calculations, 9);
            AssertHydraulicBoundaryCalculationResult(3.62, calculations, 10);
            AssertHydraulicBoundaryCalculationResult(3.68, calculations, 11);
            AssertHydraulicBoundaryCalculationResult(3.73, calculations, 12);
            AssertHydraulicBoundaryCalculationResult(3.75, calculations, 13);
            AssertHydraulicBoundaryCalculationResult(3.30, calculations, 14);
            AssertHydraulicBoundaryCalculationResult(9.57, calculations, 15);
            AssertHydraulicBoundaryCalculationResult(8.02, calculations, 16);
            AssertHydraulicBoundaryCalculationResult(4.11, calculations, 17);
        }

        private static void AssertHydraulicBoundaryCalculationResult(double expectedResult, IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations, int index)
        {
            RoundedDouble result = calculations.ElementAt(index).Output.Result;

            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        #endregion
    }
}
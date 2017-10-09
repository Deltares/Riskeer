// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenario(double.NaN, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        [TestCase(double.NaN, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(NaN)")]
        [TestCase(0.0, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(0.0)")]
        [TestCase(0.8, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(0.8)")]
        [TestCase(1.0, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(1.0)")]
        public void CreateMacroStabilityInwardsCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(double probability)
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenario(probability, section);

            // Assert
            Assert.NotNull(scenario.Output);
            Assert.NotNull(scenario.SemiProbabilisticOutput);
            Assert.AreEqual(probability, scenario.SemiProbabilisticOutput.MacroStabilityInwardsProbability, 1e-6);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateFailedMacroStabilityInwardsCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateFailedMacroStabilityInwardsCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSetToNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.NotNull(scenario.Output);
            Assert.NotNull(scenario.SemiProbabilisticOutput);
            Assert.IsNaN(scenario.SemiProbabilisticOutput.MacroStabilityInwardsProbability);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateIrrelevantMacroStabilityInwardsCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationScenarioFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateIrrelevantMacroStabilityInwardsCalculationScenario_WithSection_CreatesIrrelevantCalculation()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.IsFalse(scenario.IsRelevant);
        }

        [Test]
        public void CreateNotCalculatedMacroStabilityInwardsCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationScenarioFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateNotCalculatedMacroStabilityInwardsCalculationScenario_WithSection_CreatesRelevantCalculationWithoutOutput()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.IsNull(scenario.Output);
            Assert.IsNull(scenario.SemiProbabilisticOutput);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput_ReturnMacroStabilityInwardsCalculationScenario()
        {
            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();

            // Assert
            MacroStabilityInwardsInput inputParameters = scenario.InputParameters;
            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(inputParameters.SoilProfileUnderSurfaceLine);

            Assert.IsInstanceOf<RoundedDouble>(inputParameters.AssessmentLevel);
            Assert.IsNaN(inputParameters.AssessmentLevel);

            Assert.IsFalse(inputParameters.UseAssessmentLevelManualInput);

            Assert.AreEqual(0, inputParameters.SlipPlaneMinimumDepth, inputParameters.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(0, inputParameters.SlipPlaneMinimumLength, inputParameters.SlipPlaneMinimumLength.GetAccuracy());
            Assert.AreEqual(1, inputParameters.MaximumSliceWidth, inputParameters.MaximumSliceWidth.GetAccuracy());

            Assert.IsTrue(inputParameters.MoveGrid);
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, inputParameters.DikeSoilScenario);

            Assert.IsNaN(inputParameters.WaterLevelRiverAverage);
            Assert.IsNaN(inputParameters.WaterLevelPolder);

            Assert.IsFalse(inputParameters.DrainageConstructionPresent);

            Assert.IsNaN(inputParameters.XCoordinateDrainageConstruction);
            Assert.IsNaN(inputParameters.ZCoordinateDrainageConstruction);
            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsTrue(inputParameters.UseDefaultOffsets);
            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.IsTrue(inputParameters.AdjustPhreaticLine3And4ForUplift);
            Assert.IsNaN(inputParameters.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNaN(inputParameters.LeakageLengthInwardsPhreaticLine3);
            Assert.IsNaN(inputParameters.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNaN(inputParameters.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNaN(inputParameters.PiezometricHeadPhreaticLine2Outwards);
            Assert.IsNaN(inputParameters.PiezometricHeadPhreaticLine2Inwards);
            Assert.IsNaN(inputParameters.PenetrationLength);
            Assert.AreEqual(MacroStabilityInwardsGridDeterminationType.Automatic, inputParameters.GridDeterminationType);
            Assert.AreEqual(MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated, inputParameters.TangentLineDeterminationType);
            Assert.IsNaN(inputParameters.TangentLineZTop);
            Assert.IsNaN(inputParameters.TangentLineZBottom);
            Assert.IsNotNull(inputParameters.LeftGrid);
            Assert.IsNotNull(inputParameters.RightGrid);
            Assert.IsTrue(inputParameters.CreateZones);
            Assert.AreEqual(MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, inputParameters.ZoningBoundariesDeterminationType);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithValidInput_ReturnMacroStabilityInwardsCalculationScenario()
        {
            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

            // Assert
            MacroStabilityInwardsInput inputParameters = scenario.InputParameters;
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, inputParameters.DikeSoilScenario);
            Assert.AreEqual(0.5, inputParameters.WaterLevelPolder);
            Assert.AreEqual(1, inputParameters.PiezometricHeadPhreaticLine2Outwards.Value);
            Assert.AreEqual(1, inputParameters.PiezometricHeadPhreaticLine2Inwards.Value);
            Assert.IsTrue(inputParameters.AdjustPhreaticLine3And4ForUplift);
            Assert.IsFalse(inputParameters.UseDefaultOffsets);
            Assert.AreEqual(1, inputParameters.LeakageLengthOutwardsPhreaticLine3.Value);
            Assert.AreEqual(1, inputParameters.LeakageLengthInwardsPhreaticLine3.Value);
            Assert.AreEqual(1, inputParameters.LeakageLengthInwardsPhreaticLine4.Value);
            Assert.AreEqual(1, inputParameters.LeakageLengthOutwardsPhreaticLine4.Value);
            Assert.AreEqual(1, inputParameters.PenetrationLength.Value);
            Assert.AreEqual(1, inputParameters.SlipPlaneMinimumDepth.Value);
            Assert.AreEqual(1, inputParameters.SlipPlaneMinimumLength.Value);
            Assert.AreEqual(1, inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder.Value);
            Assert.AreEqual(1, inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver.Value);
            Assert.AreEqual(1, inputParameters.PhreaticLineOffsetBelowDikeTopAtRiver.Value);
            Assert.AreEqual(1, inputParameters.PhreaticLineOffsetBelowDikeToeAtPolder.Value);
            Assert.AreEqual(1, inputParameters.PhreaticLineOffsetBelowDikeTopAtPolder.Value);
            Assert.AreEqual(1, inputParameters.PhreaticLineOffsetBelowShoulderBaseInside.Value);
            Assert.IsTrue(inputParameters.DrainageConstructionPresent);
            Assert.AreEqual(1, inputParameters.XCoordinateDrainageConstruction.Value);
            Assert.AreEqual(1, inputParameters.ZCoordinateDrainageConstruction.Value);
            Assert.AreEqual(MacroStabilityInwardsGridDeterminationType.Manual, inputParameters.GridDeterminationType);
            Assert.AreEqual(MacroStabilityInwardsTangentLineDeterminationType.Specified, inputParameters.TangentLineDeterminationType);
            Assert.AreEqual(1, inputParameters.TangentLineZTop.Value);
            Assert.AreEqual(1, inputParameters.TangentLineZBottom.Value);
            Assert.AreEqual(1, inputParameters.LeftGrid.XLeft.Value);
            Assert.AreEqual(1, inputParameters.LeftGrid.XRight.Value);
            Assert.AreEqual(1, inputParameters.LeftGrid.ZTop.Value);
            Assert.AreEqual(1, inputParameters.LeftGrid.ZBottom.Value);
            Assert.AreEqual(1, inputParameters.LeftGrid.NumberOfVerticalPoints);
            Assert.AreEqual(1, inputParameters.LeftGrid.NumberOfHorizontalPoints);
            Assert.AreEqual(1, inputParameters.RightGrid.XLeft.Value);
            Assert.AreEqual(1, inputParameters.RightGrid.XRight.Value);
            Assert.AreEqual(1, inputParameters.RightGrid.ZTop.Value);
            Assert.AreEqual(1, inputParameters.RightGrid.ZBottom.Value);
            Assert.AreEqual(1, inputParameters.RightGrid.NumberOfVerticalPoints);
            Assert.AreEqual(1, inputParameters.RightGrid.NumberOfHorizontalPoints);
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
        }
    }
}
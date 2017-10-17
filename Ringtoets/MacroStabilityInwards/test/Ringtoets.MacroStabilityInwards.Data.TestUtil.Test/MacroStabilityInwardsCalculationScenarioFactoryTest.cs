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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

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

            Assert.IsFalse(inputParameters.DrainageConstructionPresent);

            Assert.IsNaN(inputParameters.XCoordinateDrainageConstruction);
            Assert.IsNaN(inputParameters.ZCoordinateDrainageConstruction);
            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder);

            Assert.IsNaN(inputParameters.LocationInputExtreme.WaterLevelPolder);
            Assert.IsTrue(inputParameters.LocationInputExtreme.UseDefaultOffsets);
            Assert.IsNaN(inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNaN(inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNaN(inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNaN(inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.IsNaN(inputParameters.LocationInputExtreme.PenetrationLength);

            Assert.IsNaN(inputParameters.LocationInputDaily.WaterLevelPolder);
            Assert.IsTrue(inputParameters.LocationInputDaily.UseDefaultOffsets);
            Assert.IsNaN(inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNaN(inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNaN(inputParameters.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNaN(inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(0, inputParameters.LocationInputDaily.PenetrationLength, inputParameters.LocationInputDaily.PenetrationLength.GetAccuracy());

            Assert.IsTrue(inputParameters.AdjustPhreaticLine3And4ForUplift);
            Assert.IsNaN(inputParameters.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNaN(inputParameters.LeakageLengthInwardsPhreaticLine3);
            Assert.IsNaN(inputParameters.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNaN(inputParameters.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNaN(inputParameters.PiezometricHeadPhreaticLine2Outwards);
            Assert.IsNaN(inputParameters.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(MacroStabilityInwardsGridDeterminationType.Automatic, inputParameters.GridDeterminationType);
            Assert.AreEqual(MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated, inputParameters.TangentLineDeterminationType);
            Assert.IsNaN(inputParameters.TangentLineZTop);
            Assert.IsNaN(inputParameters.TangentLineZBottom);
            Assert.AreEqual(1, inputParameters.TangentLineNumber);
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
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay,
                            inputParameters.DikeSoilScenario);
            Assert.AreEqual(1, inputParameters.PiezometricHeadPhreaticLine2Outwards,
                            inputParameters.PiezometricHeadPhreaticLine2Outwards.GetAccuracy());
            Assert.AreEqual(1, inputParameters.PiezometricHeadPhreaticLine2Inwards,
                            inputParameters.PiezometricHeadPhreaticLine2Inwards.GetAccuracy());
            Assert.IsTrue(inputParameters.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(1, inputParameters.LeakageLengthOutwardsPhreaticLine3,
                            inputParameters.LeakageLengthOutwardsPhreaticLine3.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LeakageLengthInwardsPhreaticLine3,
                            inputParameters.LeakageLengthInwardsPhreaticLine3.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LeakageLengthInwardsPhreaticLine4,
                            inputParameters.LeakageLengthInwardsPhreaticLine4.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LeakageLengthOutwardsPhreaticLine4,
                            inputParameters.LeakageLengthOutwardsPhreaticLine4.GetAccuracy());
            Assert.AreEqual(1, inputParameters.SlipPlaneMinimumDepth,
                            inputParameters.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(1, inputParameters.SlipPlaneMinimumLength,
                            inputParameters.SlipPlaneMinimumLength.GetAccuracy());
            Assert.AreEqual(1, inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder,
                            inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder.GetAccuracy());
            Assert.AreEqual(1, inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver,
                            inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver.GetAccuracy());

            Assert.AreEqual(0.5, inputParameters.LocationInputExtreme.WaterLevelPolder,
                            inputParameters.LocationInputExtreme.WaterLevelPolder.GetAccuracy());
            Assert.IsFalse(inputParameters.LocationInputExtreme.UseDefaultOffsets);
            Assert.AreEqual(1, inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver,
                            inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder,
                            inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder,
                            inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside,
                            inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LocationInputExtreme.PenetrationLength,
                            inputParameters.LocationInputExtreme.PenetrationLength.GetAccuracy());

            Assert.AreEqual(0.5, inputParameters.LocationInputDaily.WaterLevelPolder);
            Assert.IsFalse(inputParameters.LocationInputDaily.UseDefaultOffsets);
            Assert.AreEqual(1, inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver,
                            inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder,
                            inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder,
                            inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside,
                            inputParameters.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside.GetAccuracy());
            Assert.AreEqual(0, inputParameters.LocationInputDaily.PenetrationLength,
                            inputParameters.LocationInputDaily.PenetrationLength.GetAccuracy());

            Assert.IsTrue(inputParameters.DrainageConstructionPresent);
            Assert.AreEqual(1, inputParameters.XCoordinateDrainageConstruction,
                            inputParameters.XCoordinateDrainageConstruction.GetAccuracy());
            Assert.AreEqual(1, inputParameters.ZCoordinateDrainageConstruction,
                            inputParameters.ZCoordinateDrainageConstruction.GetAccuracy());
            Assert.AreEqual(MacroStabilityInwardsGridDeterminationType.Manual, inputParameters.GridDeterminationType);
            Assert.AreEqual(MacroStabilityInwardsTangentLineDeterminationType.Specified, inputParameters.TangentLineDeterminationType);
            Assert.AreEqual(1, inputParameters.TangentLineZTop,
                            inputParameters.TangentLineZTop.GetAccuracy());
            Assert.AreEqual(1, inputParameters.TangentLineZBottom,
                            inputParameters.TangentLineZBottom.GetAccuracy());
            Assert.AreEqual(10, inputParameters.TangentLineNumber);
            Assert.AreEqual(1, inputParameters.LeftGrid.XLeft,
                            inputParameters.LeftGrid.XLeft.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LeftGrid.XRight,
                            inputParameters.LeftGrid.XRight.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LeftGrid.ZTop,
                            inputParameters.LeftGrid.ZTop.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LeftGrid.ZBottom,
                            inputParameters.LeftGrid.ZBottom.GetAccuracy());
            Assert.AreEqual(1, inputParameters.LeftGrid.NumberOfVerticalPoints);
            Assert.AreEqual(1, inputParameters.LeftGrid.NumberOfHorizontalPoints);

            Assert.AreEqual(1, inputParameters.RightGrid.XLeft,
                            inputParameters.RightGrid.XLeft.GetAccuracy());
            Assert.AreEqual(1, inputParameters.RightGrid.XRight,
                            inputParameters.RightGrid.XRight.GetAccuracy());
            Assert.AreEqual(1, inputParameters.RightGrid.ZTop,
                            inputParameters.RightGrid.ZTop.GetAccuracy());
            Assert.AreEqual(1, inputParameters.RightGrid.ZBottom,
                            inputParameters.RightGrid.ZBottom.GetAccuracy());
            Assert.AreEqual(1, inputParameters.RightGrid.NumberOfVerticalPoints);
            Assert.AreEqual(1, inputParameters.RightGrid.NumberOfHorizontalPoints);

            Assert.AreEqual(0, inputParameters.StochasticSoilProfile.Probability);
            Assert.AreEqual(string.Empty, inputParameters.StochasticSoilProfile.SoilProfile.Name);
            Assert.IsInstanceOf<MacroStabilityInwardsSoilProfile1D>(inputParameters.StochasticSoilProfile.SoilProfile);
            var soilProfile1D = (MacroStabilityInwardsSoilProfile1D) inputParameters.StochasticSoilProfile.SoilProfile;
            Assert.AreEqual(0, soilProfile1D.Bottom);
            CollectionAssert.AreEqual(new[]
            {
                new MacroStabilityInwardsSoilLayer1D(10.56)
                {
                    Data =
                    {
                        IsAquifer = false,
                        Cohesion = new VariationCoefficientLogNormalDistribution(),
                        FrictionAngle = new VariationCoefficientLogNormalDistribution(),
                        AbovePhreaticLevel =
                        {
                            Mean = (RoundedDouble) 0.3,
                            CoefficientOfVariation = (RoundedDouble) 0.2,
                            Shift = (RoundedDouble) 0.1
                        },
                        BelowPhreaticLevel =
                        {
                            Mean = (RoundedDouble) 15,
                            CoefficientOfVariation = (RoundedDouble) 0.5,
                            Shift = (RoundedDouble) 0.2
                        }
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(6.0)
                {
                    Data =
                    {
                        IsAquifer = true,
                        Cohesion = new VariationCoefficientLogNormalDistribution(),
                        FrictionAngle = new VariationCoefficientLogNormalDistribution(),
                        AbovePhreaticLevel =
                        {
                            Mean = (RoundedDouble) 0.3,
                            CoefficientOfVariation = (RoundedDouble) 0.2,
                            Shift = (RoundedDouble) 0.1
                        },
                        BelowPhreaticLevel =
                        {
                            Mean = (RoundedDouble) 15,
                            CoefficientOfVariation = (RoundedDouble) 0.5,
                            Shift = (RoundedDouble) 0.2
                        }
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(0.1)
                {
                    Data =
                    {
                        IsAquifer = false,
                        Cohesion = new VariationCoefficientLogNormalDistribution(),
                        FrictionAngle = new VariationCoefficientLogNormalDistribution(),
                        AbovePhreaticLevel =
                        {
                            Mean = (RoundedDouble) 0.3,
                            CoefficientOfVariation = (RoundedDouble) 0.2,
                            Shift = (RoundedDouble) 0.1
                        },
                        BelowPhreaticLevel =
                        {
                            Mean = (RoundedDouble) 15,
                            CoefficientOfVariation = (RoundedDouble) 0.5,
                            Shift = (RoundedDouble) 0.2
                        }
                    }
                }
            }, soilProfile1D.Layers);

            var expectedSurfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var firstCharacteristicPointLocation = new Point3D(0.1, 0.0, 2);
            var secondCharacteristicPointLocation = new Point3D(0.2, 0.0, 2);
            var thirdCharacteristicPointLocation = new Point3D(0.3, 0.0, 3);
            var fourthCharacteristicPointLocation = new Point3D(0.4, 0.0, 3);
            var fifthCharacteristicPointLocation = new Point3D(0.5, 0.0, 1);
            var sixthCharacteristicPointLocation = new Point3D(0.6, 0.0, 1);

            expectedSurfaceLine.SetGeometry(new[]
            {
                firstCharacteristicPointLocation,
                secondCharacteristicPointLocation,
                thirdCharacteristicPointLocation,
                fourthCharacteristicPointLocation,
                fifthCharacteristicPointLocation,
                sixthCharacteristicPointLocation
            });
            expectedSurfaceLine.SetSurfaceLevelOutsideAt(firstCharacteristicPointLocation);
            expectedSurfaceLine.SetDikeToeAtRiverAt(secondCharacteristicPointLocation);
            expectedSurfaceLine.SetDikeTopAtRiverAt(thirdCharacteristicPointLocation);
            expectedSurfaceLine.SetDikeTopAtPolderAt(fourthCharacteristicPointLocation);
            expectedSurfaceLine.SetDikeToeAtPolderAt(fifthCharacteristicPointLocation);
            expectedSurfaceLine.SetSurfaceLevelInsideAt(sixthCharacteristicPointLocation);
            Assert.AreEqual(expectedSurfaceLine, inputParameters.SurfaceLine);
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
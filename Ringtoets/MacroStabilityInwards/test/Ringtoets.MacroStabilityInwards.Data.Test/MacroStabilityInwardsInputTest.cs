// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsInputTest
    {
        [Test]
        public void Constructor_PropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValuesSet_ExpectedValues()
        {
            // Call
            var inputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(inputParameters);
            Assert.IsInstanceOf<ICalculationInputWithHydraulicBoundaryLocation>(inputParameters);
            Assert.IsInstanceOf<IMacroStabilityInwardsWaternetInput>(inputParameters);

            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(inputParameters.SoilProfileUnderSurfaceLine);

            Assert.IsNaN(inputParameters.AssessmentLevel);
            Assert.AreEqual(2, inputParameters.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.IsFalse(inputParameters.UseAssessmentLevelManualInput);

            Assert.AreEqual(0, inputParameters.SlipPlaneMinimumDepth, inputParameters.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(2, inputParameters.SlipPlaneMinimumDepth.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.SlipPlaneMinimumLength, inputParameters.SlipPlaneMinimumLength.GetAccuracy());
            Assert.AreEqual(2, inputParameters.SlipPlaneMinimumLength.NumberOfDecimalPlaces);

            Assert.AreEqual(1, inputParameters.MaximumSliceWidth, inputParameters.MaximumSliceWidth.GetAccuracy());
            Assert.AreEqual(2, inputParameters.MaximumSliceWidth.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.MoveGrid);
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, inputParameters.DikeSoilScenario);

            Assert.IsNaN(inputParameters.WaterLevelRiverAverage);
            Assert.AreEqual(2, inputParameters.WaterLevelRiverAverage.NumberOfDecimalPlaces);

            Assert.IsNotNull(inputParameters.LocationInputExtreme);
            Assert.IsNotNull(inputParameters.LocationInputDaily);

            Assert.IsFalse(inputParameters.DrainageConstructionPresent);

            Assert.IsNaN(inputParameters.XCoordinateDrainageConstruction);
            Assert.AreEqual(2, inputParameters.XCoordinateDrainageConstruction.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.ZCoordinateDrainageConstruction);
            Assert.AreEqual(2, inputParameters.ZCoordinateDrainageConstruction.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(2, inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(2, inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.AdjustPhreaticLine3And4ForUplift);

            Assert.IsNaN(inputParameters.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(2, inputParameters.LeakageLengthOutwardsPhreaticLine3.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(2, inputParameters.LeakageLengthInwardsPhreaticLine3.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(2, inputParameters.LeakageLengthOutwardsPhreaticLine4.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(2, inputParameters.LeakageLengthInwardsPhreaticLine4.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(2, inputParameters.PiezometricHeadPhreaticLine2Outwards.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(2, inputParameters.PiezometricHeadPhreaticLine2Inwards.NumberOfDecimalPlaces);

            Assert.AreEqual(MacroStabilityInwardsGridDeterminationType.Automatic, inputParameters.GridDeterminationType);
            Assert.AreEqual(MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated, inputParameters.TangentLineDeterminationType);

            Assert.IsNaN(inputParameters.TangentLineZTop);
            Assert.AreEqual(2, inputParameters.TangentLineZTop.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.TangentLineZBottom);
            Assert.AreEqual(2, inputParameters.TangentLineZBottom.NumberOfDecimalPlaces);

            Assert.AreEqual(1, inputParameters.TangentLineNumber);

            Assert.IsNotNull(inputParameters.LeftGrid);
            Assert.IsNotNull(inputParameters.RightGrid);

            Assert.IsTrue(inputParameters.CreateZones);
            Assert.AreEqual(MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, inputParameters.ZoningBoundariesDeterminationType);

            Assert.IsNaN(inputParameters.ZoneBoundaryLeft);
            Assert.AreEqual(2, inputParameters.ZoneBoundaryLeft.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.ZoneBoundaryRight);
            Assert.AreEqual(2, inputParameters.ZoneBoundaryRight.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValuesSet_ExpectedValues()
        {
            // Setup
            var random = new Random(21);

            double leftGridXLeft = random.NextDouble(0.0, 1.0);
            double leftGridXRight = random.NextDouble(1.0, 2.0);
            double leftGridZTop = random.NextDouble(1.0, 2.0);
            double leftGridZBottom = random.NextDouble(0.0, 1.0);

            double rightGridXLeft = random.NextDouble(0.0, 1.0);
            double rightGridXRight = random.NextDouble(1.0, 2.0);
            double rightGridZTop = random.NextDouble(1.0, 2.0);
            double rightGridZBottom = random.NextDouble(0.0, 1.0);

            double tangentLineZTop = random.NextDouble(1.0, 2.0);
            double tangentLineZBottom = random.NextDouble(0.0, 1.0);

            var properties = new MacroStabilityInwardsInput.ConstructionProperties
            {
                LeftGridXLeft = leftGridXLeft,
                LeftGridXRight = leftGridXRight,
                LeftGridZTop = leftGridZTop,
                LeftGridZBottom = leftGridZBottom,

                RightGridXLeft = rightGridXLeft,
                RightGridXRight = rightGridXRight,
                RightGridZTop = rightGridZTop,
                RightGridZBottom = rightGridZBottom,

                TangentLineZTop = tangentLineZTop,
                TangentLineZBottom = tangentLineZBottom
            };

            // Call
            var inputParameters = new MacroStabilityInwardsInput(properties);

            // Assert
            Assert.AreEqual(leftGridXLeft, inputParameters.LeftGrid.XLeft, inputParameters.LeftGrid.XLeft.GetAccuracy());
            Assert.AreEqual(leftGridXRight, inputParameters.LeftGrid.XRight, inputParameters.LeftGrid.XRight.GetAccuracy());
            Assert.AreEqual(leftGridZTop, inputParameters.LeftGrid.ZTop, inputParameters.LeftGrid.ZTop.GetAccuracy());
            Assert.AreEqual(leftGridZBottom, inputParameters.LeftGrid.ZBottom, inputParameters.LeftGrid.ZBottom.GetAccuracy());

            Assert.AreEqual(rightGridXLeft, inputParameters.RightGrid.XLeft, inputParameters.RightGrid.XLeft.GetAccuracy());
            Assert.AreEqual(rightGridXRight, inputParameters.RightGrid.XRight, inputParameters.RightGrid.XRight.GetAccuracy());
            Assert.AreEqual(rightGridZTop, inputParameters.RightGrid.ZTop, inputParameters.RightGrid.ZTop.GetAccuracy());
            Assert.AreEqual(rightGridZBottom, inputParameters.RightGrid.ZBottom, inputParameters.RightGrid.ZBottom.GetAccuracy());

            Assert.AreEqual(tangentLineZTop, inputParameters.TangentLineZTop, inputParameters.TangentLineZTop.GetAccuracy());
            Assert.AreEqual(tangentLineZBottom, inputParameters.TangentLineZBottom, inputParameters.TangentLineZBottom.GetAccuracy());
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidConstructionPropertiesCombinations))]
        public void Constructor_InvalidConstructionProperties_ThrowsArgumentException(MacroStabilityInwardsInput.ConstructionProperties properties,
                                                                                      string expectedMessage)
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsInput(properties);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_SetProperties_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double assessmentLevel = random.NextDouble();
            double slipPlaneMinimumDepth = random.NextDouble();
            double slipPlaneMinimumLength = random.NextDouble();
            double maximumSliceWidth = random.NextDouble();
            double waterLevelRiverAverage = random.NextDouble();
            double xCoordinateDrainageConstruction = random.NextDouble();
            double zCoordinateDrainageConstruction = random.NextDouble();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble();
            double leakageLengthOutwardsPhreaticLine3 = random.NextDouble();
            double leakageLengthInwardsPhreaticLine3 = random.NextDouble();
            double leakageLengthOutwardsPhreaticLine4 = random.NextDouble();
            double leakageLengthInwardsPhreaticLine4 = random.NextDouble();
            double piezometricHeadPhreaticLine2Outwards = random.NextDouble();
            double piezometricHeadPhreaticLine2Inwards = random.NextDouble();
            double tangentLineZTop = random.NextDouble(1.0, 2.0);
            double tangentLineZBottom = random.NextDouble(0.0, 1.0);
            int tangentLineNumber = random.Next(1, 51);
            double zoneBoundaryLeft = random.NextDouble();
            double zoneBoundaryRight = random.NextDouble();

            // Call
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                AssessmentLevel = (RoundedDouble) assessmentLevel,
                SlipPlaneMinimumDepth = (RoundedDouble) slipPlaneMinimumDepth,
                SlipPlaneMinimumLength = (RoundedDouble) slipPlaneMinimumLength,
                MaximumSliceWidth = (RoundedDouble) maximumSliceWidth,
                WaterLevelRiverAverage = (RoundedDouble) waterLevelRiverAverage,
                XCoordinateDrainageConstruction = (RoundedDouble) xCoordinateDrainageConstruction,
                ZCoordinateDrainageConstruction = (RoundedDouble) zCoordinateDrainageConstruction,
                MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) minimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) minimumLevelPhreaticLineAtDikeTopPolder,
                LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) leakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) leakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) leakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) leakageLengthInwardsPhreaticLine4,
                PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) piezometricHeadPhreaticLine2Outwards,
                PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) piezometricHeadPhreaticLine2Inwards,
                TangentLineZTop = (RoundedDouble) tangentLineZTop,
                TangentLineZBottom = (RoundedDouble) tangentLineZBottom,
                TangentLineNumber = tangentLineNumber,
                ZoneBoundaryLeft = (RoundedDouble) zoneBoundaryLeft,
                ZoneBoundaryRight = (RoundedDouble) zoneBoundaryRight
            };

            // Assert
            Assert.AreEqual(2, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(assessmentLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());

            Assert.AreEqual(2, input.SlipPlaneMinimumDepth.NumberOfDecimalPlaces);
            Assert.AreEqual(slipPlaneMinimumDepth, input.SlipPlaneMinimumDepth, input.SlipPlaneMinimumDepth.GetAccuracy());

            Assert.AreEqual(2, input.SlipPlaneMinimumLength.NumberOfDecimalPlaces);
            Assert.AreEqual(slipPlaneMinimumLength, input.SlipPlaneMinimumLength, input.SlipPlaneMinimumLength.GetAccuracy());

            Assert.AreEqual(2, input.MaximumSliceWidth.NumberOfDecimalPlaces);
            Assert.AreEqual(maximumSliceWidth, input.MaximumSliceWidth, input.MaximumSliceWidth.GetAccuracy());

            Assert.AreEqual(2, input.WaterLevelRiverAverage.NumberOfDecimalPlaces);
            Assert.AreEqual(waterLevelRiverAverage, input.WaterLevelRiverAverage, input.WaterLevelRiverAverage.GetAccuracy());

            Assert.AreEqual(2, input.XCoordinateDrainageConstruction.NumberOfDecimalPlaces);
            Assert.AreEqual(xCoordinateDrainageConstruction, input.XCoordinateDrainageConstruction, input.XCoordinateDrainageConstruction.GetAccuracy());

            Assert.AreEqual(2, input.ZCoordinateDrainageConstruction.NumberOfDecimalPlaces);
            Assert.AreEqual(zCoordinateDrainageConstruction, input.ZCoordinateDrainageConstruction, input.ZCoordinateDrainageConstruction.GetAccuracy());

            Assert.AreEqual(2, input.MinimumLevelPhreaticLineAtDikeTopRiver.NumberOfDecimalPlaces);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, input.MinimumLevelPhreaticLineAtDikeTopRiver, input.MinimumLevelPhreaticLineAtDikeTopRiver.GetAccuracy());

            Assert.AreEqual(2, input.MinimumLevelPhreaticLineAtDikeTopPolder.NumberOfDecimalPlaces);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, input.MinimumLevelPhreaticLineAtDikeTopPolder, input.MinimumLevelPhreaticLineAtDikeTopPolder.GetAccuracy());

            Assert.AreEqual(2, input.LeakageLengthOutwardsPhreaticLine3.NumberOfDecimalPlaces);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, input.LeakageLengthOutwardsPhreaticLine3, input.LeakageLengthOutwardsPhreaticLine3.GetAccuracy());

            Assert.AreEqual(2, input.LeakageLengthInwardsPhreaticLine3.NumberOfDecimalPlaces);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, input.LeakageLengthInwardsPhreaticLine3, input.LeakageLengthInwardsPhreaticLine3.GetAccuracy());

            Assert.AreEqual(2, input.LeakageLengthOutwardsPhreaticLine4.NumberOfDecimalPlaces);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, input.LeakageLengthOutwardsPhreaticLine4, input.LeakageLengthOutwardsPhreaticLine4.GetAccuracy());

            Assert.AreEqual(2, input.LeakageLengthInwardsPhreaticLine4.NumberOfDecimalPlaces);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, input.LeakageLengthInwardsPhreaticLine4, input.LeakageLengthInwardsPhreaticLine4.GetAccuracy());

            Assert.AreEqual(2, input.TangentLineZTop.NumberOfDecimalPlaces);
            Assert.AreEqual(tangentLineZTop, input.TangentLineZTop, input.TangentLineZTop.GetAccuracy());

            Assert.AreEqual(2, input.TangentLineZBottom.NumberOfDecimalPlaces);
            Assert.AreEqual(tangentLineZBottom, input.TangentLineZBottom, input.TangentLineZBottom.GetAccuracy());

            Assert.AreEqual(tangentLineNumber, input.TangentLineNumber);

            Assert.AreEqual(2, input.ZoneBoundaryLeft.NumberOfDecimalPlaces);
            Assert.AreEqual(zoneBoundaryLeft, input.ZoneBoundaryLeft, input.ZoneBoundaryLeft.GetAccuracy());

            Assert.AreEqual(2, input.ZoneBoundaryRight.NumberOfDecimalPlaces);
            Assert.AreEqual(zoneBoundaryRight, input.ZoneBoundaryRight, input.ZoneBoundaryRight.GetAccuracy());
        }

        [Test]
        public void GivenInput_WhenSurfaceLineSetAndStochasticSoilProfileNull_ThenSoilProfileUnderSurfaceLineNull()
        {
            // Given
            var inputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // When
            inputParameters.SurfaceLine = new MacroStabilityInwardsSurfaceLine("test");

            // Then
            Assert.IsNull(inputParameters.SoilProfileUnderSurfaceLine);
        }

        [Test]
        public void GivenInput_WhenStochasticSoilProfileSetAndSurfaceLineNull_ThenSoilProfileUnderSurfaceLineNull()
        {
            // Given
            var inputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // When
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            inputParameters.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, soilProfile);

            // Then
            Assert.IsNull(inputParameters.SoilProfileUnderSurfaceLine);
        }

        [Test]
        public void GivenInput_WhenSurfaceLineAndStochasticSoilProfileSet_ThenSoilProfileUnderSurfaceLineSet()
        {
            // Given
            var inputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // When
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 1, 1)
            });
            inputParameters.SurfaceLine = surfaceLine;
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            inputParameters.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, soilProfile);

            // Then
            Assert.IsNotNull(inputParameters.SoilProfileUnderSurfaceLine);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidTangentCombinations))]
        public void TangentLineZTop_InvalidTangentLineZTop_ThrowsArgumentException(double zBottom, double zTop)
        {
            // Setup
            var inputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties
            {
                TangentLineZBottom = zBottom
            });

            // Call
            TestDelegate test = () => inputParameters.TangentLineZTop = (RoundedDouble) zTop;

            // Assert
            const string expectedMessage = "Tangentlijn Z-boven moet groter zijn dan of gelijk zijn aan tangentlijn Z-onder, of NaN.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidTangentCombinations))]
        public void TangentLineZBottom_InvalidTangentLineZBottom_ThrowsArgumentException(double zBottom, double zTop)
        {
            // Setup
            var inputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties
            {
                TangentLineZTop = zTop
            });

            // Call
            TestDelegate test = () => inputParameters.TangentLineZBottom = (RoundedDouble) zBottom;

            // Assert
            const string expectedMessage = "Tangentlijn Z-onder moet kleiner zijn dan of gelijk zijn aan tangentlijn Z-boven, of NaN.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(0)]
        [TestCase(51)]
        public void TangentLineNumber_SetValueOutsideValidRange_ThrowArgumentOutOfRangeException(int tangentLineNumber)
        {
            // Setup
            var inputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            TestDelegate call = () => inputParameters.TangentLineNumber = tangentLineNumber;

            // Assert
            const string message = "De waarde voor het aantal raaklijnen moet in het bereik [1, 50] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message);
        }

        [Test]
        [TestCase(1)]
        [TestCase(25)]
        [TestCase(50)]
        public void TangentLineNumber_SetValueInsideValidRange_GetNewlySetValue(int tangentLineNumber)
        {
            // Setup
            var inputParameters = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            inputParameters.TangentLineNumber = tangentLineNumber;

            // Assert
            Assert.AreEqual(tangentLineNumber, inputParameters.TangentLineNumber);
        }

        [Test]
        public void Clone_NoPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 1, 1)
            });

            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = stochasticSoilModel.StochasticSoilProfiles.First();

            // Precondition
            Assert.IsNotNull(stochasticSoilProfile);

            var original = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            MacroStabilityInwardsTestDataGenerator.SetRandomMacroStabilityInwardsInput(original);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        private static IEnumerable<TestCaseData> GetInvalidConstructionPropertiesCombinations()
        {
            const string expectedXMessage = "X links moet kleiner zijn dan of gelijk zijn aan X rechts, of NaN.";
            const string expectedZMessage = "Z boven moet groter zijn dan of gelijk zijn aan Z onder, of NaN.";
            const string expectedTangentLineMessage = "Tangentlijn Z-boven moet groter zijn dan of gelijk zijn aan tangentlijn Z-onder, of NaN.";

            yield return new TestCaseData(new MacroStabilityInwardsInput.ConstructionProperties
                {
                    LeftGridXLeft = 1.0,
                    LeftGridXRight = 0.0
                }, expectedXMessage)
                .SetName("LeftGrid XRight smaller than XLeft");

            yield return new TestCaseData(new MacroStabilityInwardsInput.ConstructionProperties
                {
                    LeftGridZTop = 0.0,
                    LeftGridZBottom = 1.0
                }, expectedZMessage)
                .SetName("LeftGrid ZTop smaller than ZBottom");

            yield return new TestCaseData(new MacroStabilityInwardsInput.ConstructionProperties
                {
                    RightGridXLeft = 1.0,
                    RightGridXRight = 0.0
                }, expectedXMessage)
                .SetName("RightGrid XRight smaller than XLeft");

            yield return new TestCaseData(new MacroStabilityInwardsInput.ConstructionProperties
                {
                    RightGridZTop = 0.0,
                    RightGridZBottom = 1.0
                }, expectedZMessage)
                .SetName("RightGrid ZTop smaller than ZBottom");

            yield return new TestCaseData(new MacroStabilityInwardsInput.ConstructionProperties
                {
                    TangentLineZTop = 0.0,
                    TangentLineZBottom = 1.0
                }, expectedTangentLineMessage)
                .SetName("TangentLine ZTop smaller than ZBottom");
        }

        private static IEnumerable<TestCaseData> GetInvalidTangentCombinations()
        {
            yield return new TestCaseData(1.0, 0.0);
            yield return new TestCaseData(0.0, -1.0);
        }
    }
}
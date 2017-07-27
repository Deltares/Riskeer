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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var phreaticLevelExit = new NormalDistribution(3)
            {
                Mean = (RoundedDouble) 0,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var dampingFactorExit = new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.7,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var diameter70 = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            var darcyPermeability = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            var thicknessCoverageLayer = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.5
            };

            var effectiveThicknessCoverageLayer = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.5
            };

            var saturatedVolumicWeightOfCoverageLayer = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            };

            var thicknessAquiferLayer = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.5
            };

            var seepageLength = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var generalInputParameters = new GeneralPipingInput();

            // Call
            var inputParameters = new PipingInput(generalInputParameters);

            // Assert
            Assert.IsInstanceOf<Observable>(inputParameters);
            Assert.IsInstanceOf<ICalculationInput>(inputParameters);

            DistributionAssert.AreEqual(phreaticLevelExit, inputParameters.PhreaticLevelExit);
            DistributionAssert.AreEqual(dampingFactorExit, inputParameters.DampingFactorExit);
            DistributionAssert.AreEqual(diameter70, inputParameters.Diameter70);
            DistributionAssert.AreEqual(darcyPermeability, inputParameters.DarcyPermeability);

            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);

            Assert.AreEqual(generalInputParameters.UpliftModelFactor, inputParameters.UpliftModelFactor);
            Assert.AreEqual(generalInputParameters.SellmeijerModelFactor, inputParameters.SellmeijerModelFactor);
            Assert.AreEqual(generalInputParameters.CriticalHeaveGradient, inputParameters.CriticalHeaveGradient);
            Assert.AreEqual(generalInputParameters.SellmeijerReductionFactor, inputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(generalInputParameters.Gravity, inputParameters.Gravity);
            Assert.AreEqual(generalInputParameters.WaterKinematicViscosity, inputParameters.WaterKinematicViscosity);
            Assert.AreEqual(generalInputParameters.WaterVolumetricWeight, inputParameters.WaterVolumetricWeight);
            Assert.AreEqual(generalInputParameters.SandParticlesVolumicWeight, inputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(generalInputParameters.WhitesDragCoefficient, inputParameters.WhitesDragCoefficient);
            Assert.AreEqual(generalInputParameters.BeddingAngle, inputParameters.BeddingAngle);
            Assert.AreEqual(generalInputParameters.MeanDiameter70, inputParameters.MeanDiameter70);

            DistributionAssert.AreEqual(thicknessCoverageLayer, inputParameters.ThicknessCoverageLayer);
            DistributionAssert.AreEqual(effectiveThicknessCoverageLayer, inputParameters.EffectiveThicknessCoverageLayer);

            DistributionAssert.AreEqual(saturatedVolumicWeightOfCoverageLayer, inputParameters.SaturatedVolumicWeightOfCoverageLayer);
            Assert.AreEqual(2, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift);

            DistributionAssert.AreEqual(thicknessAquiferLayer, inputParameters.ThicknessAquiferLayer);
            DistributionAssert.AreEqual(seepageLength, inputParameters.SeepageLength);

            Assert.IsNaN(inputParameters.ExitPointL);
            Assert.AreEqual(2, inputParameters.ExitPointL.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.EntryPointL);
            Assert.AreEqual(2, inputParameters.EntryPointL.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.PiezometricHeadExit);
            Assert.AreEqual(2, inputParameters.PiezometricHeadExit.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<RoundedDouble>(inputParameters.AssessmentLevel);
            Assert.AreEqual(2, inputParameters.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.AssessmentLevel);

            Assert.IsFalse(inputParameters.UseAssessmentLevelManualInput);
        }

        [Test]
        public void Constructor_GeneralPipingInputIsNull_ArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingInput(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        [TestCase(1.23456)]
        [TestCase(3.5)]
        public void ExitPointL_ExitPointEqualSmallerThanEntryPoint_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                EntryPointL = (RoundedDouble) 3.5
            };

            // Call
            TestDelegate call = () => pipingInput.ExitPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void ExitPointL_Always_SameNumberOfDecimalsAsSurfaceLineLocalGeometry()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = CreateSurfaceLine()
            };

            // Call
            RoundedPoint2DCollection localGeometry = pipingInput.SurfaceLine.LocalGeometry;

            // Assert
            Assert.AreEqual(localGeometry.NumberOfDecimalPlaces, pipingInput.ExitPointL.NumberOfDecimalPlaces);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(5.4)]
        [TestCase(1.006)]
        [TestCase(-0.005)]
        [TestCase(-5.4)]
        public void ExitPointL_ExitPointNotOnSurfaceLine_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = RoundedDouble.NaN;

            // Call
            TestDelegate call = () => input.ExitPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 1,0]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(-1e-3, Description = "Valid ExitPointL due to rounding to 0.0")]
        [TestCase(0.1004)]
        [TestCase(0.50)]
        public void ExitPointL_SetToNew_ValueIsRounded(double exitPointValue)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = RoundedDouble.NaN;

            int originalNumberOfDecimalPlaces = input.ExitPointL.NumberOfDecimalPlaces;

            // Call
            input.ExitPointL = (RoundedDouble) exitPointValue;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.ExitPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlaces, exitPointValue), input.ExitPointL);
        }

        [Test]
        [TestCase(5.0)]
        [TestCase(3.5)]
        public void EntryPointL_EntryPointEqualOrGreaterThanExitPoint_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 3.5
            };

            // Call
            TestDelegate call = () => pipingInput.EntryPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(5.4)]
        [TestCase(1.006)]
        [TestCase(-0.005)]
        [TestCase(-5.4)]
        public void EntryPointL_EntryPointNotOnSurfaceLine_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = RoundedDouble.NaN;
            // Call
            TestDelegate call = () => input.EntryPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 1,0]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(-1e-3, Description = "Valid EntryPointL due to rounding to 0.0")]
        [TestCase(0.005)]
        [TestCase(0.1004)]
        [TestCase(0.50)]
        public void EntryPointL_SetToNew_ValueIsRounded(double entryPointValue)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = RoundedDouble.NaN;

            int originalNumberOfDecimalPlaces = input.EntryPointL.NumberOfDecimalPlaces;

            // Call
            input.EntryPointL = (RoundedDouble) entryPointValue;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.EntryPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlaces, entryPointValue), input.EntryPointL);
        }

        [Test]
        public void EntryPointL_Always_SameNumberOfDecimalsAsSurfaceLineLocalGeometry()
        {
            // Setup
            PipingSurfaceLine surfaceLine = CreateSurfaceLine();
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            RoundedPoint2DCollection localGeometry = surfaceLine.LocalGeometry;

            // Assert
            Assert.AreEqual(localGeometry.NumberOfDecimalPlaces, pipingInput.EntryPointL.NumberOfDecimalPlaces);
        }

        [Test]
        public void SurfaceLine_WithDikeToes_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(1, 0, 2));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(2, 0, 3));

            // Call
            input.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, 1), input.EntryPointL);
            Assert.AreEqual(new RoundedDouble(2, 2), input.ExitPointL);
        }

        [Test]
        public void SurfaceLine_DikeToesBeyondSetExitPointL_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            input.SurfaceLine = surfaceLine;
            input.EntryPointL = (RoundedDouble) 0;
            input.ExitPointL = (RoundedDouble) 1;

            // Call
            input.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, 2), input.EntryPointL);
            Assert.AreEqual(new RoundedDouble(3, 3), input.ExitPointL);
        }

        [Test]
        public void SurfaceLine_DikeToesBeforeSetEntryPointL_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            input.SurfaceLine = surfaceLine;
            input.ExitPointL = (RoundedDouble) 5;
            input.EntryPointL = (RoundedDouble) 4;

            // Call
            input.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, 2), input.EntryPointL);
            Assert.AreEqual(new RoundedDouble(2, 3), input.ExitPointL);
        }

        [Test]
        public void SynchronizeEntryAndExitPointInput_SurfaceLineNull_EntryPointLAndExitPointLNaN()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput())
            {
                EntryPointL = (RoundedDouble) 3,
                ExitPointL = (RoundedDouble) 5
            };

            // Call
            input.SynchronizeEntryAndExitPointInput();

            // Assert
            Assert.IsNaN(input.EntryPointL);
            Assert.IsNaN(input.ExitPointL);
        }

        [Test]
        public void SynchronizeEntryAndExitPointInput_DikeToesBeyondSetExitPointL_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            input.SurfaceLine = surfaceLine;
            input.EntryPointL = (RoundedDouble) 0;
            input.ExitPointL = (RoundedDouble) 1;

            // Call
            input.SynchronizeEntryAndExitPointInput();

            // Assert
            Assert.AreEqual(new RoundedDouble(2, 2), input.EntryPointL);
            Assert.AreEqual(new RoundedDouble(3, 3), input.ExitPointL);
        }

        [Test]
        public void SynchronizeEntryAndExitPointInput_DikeToesBeforeSetEntryPointL_ExitPointLAndEntryPointLUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            input.SurfaceLine = surfaceLine;
            input.ExitPointL = (RoundedDouble) 5;
            input.EntryPointL = (RoundedDouble) 4;

            // Call
            input.SynchronizeEntryAndExitPointInput();

            // Assert
            Assert.AreEqual(new RoundedDouble(2, 2), input.EntryPointL);
            Assert.AreEqual(new RoundedDouble(2, 3), input.ExitPointL);
        }

        [Test]
        public void IsEntryAndExitPointInputSynchronized_SurfaceLineNull_ReturnFalse()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            // Call
            bool synchronized = input.IsEntryAndExitPointInputSynchronized;

            // Assert
            Assert.IsFalse(synchronized);
        }

        [Test]
        public void IsEntryAndExitPointInputSynchronized_SurfaceLineAndInputInSync_ReturnTrue()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            // Call
            bool synchronized = input.IsEntryAndExitPointInputSynchronized;

            // Assert
            Assert.IsTrue(synchronized);
        }

        [Test]
        [TestCaseSource(nameof(DifferentSurfaceLineProperties))]
        public void IsEntryAndExitPointInputSynchronized_SurfaceLineAndInputNotInSync_ReturnFalse(Point3D newDikeToeAtRiver, Point3D newDikeToeAtPolder)
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            input.SurfaceLine.SetDikeToeAtRiverAt(newDikeToeAtRiver);
            input.SurfaceLine.SetDikeToeAtPolderAt(newDikeToeAtPolder);

            // Call
            bool synchronized = input.IsEntryAndExitPointInputSynchronized;

            // Assert
            Assert.IsFalse(synchronized);
        }

        private static IEnumerable<TestCaseData> DifferentSurfaceLineProperties
        {
            get
            {
                yield return new TestCaseData(new Point3D(3, 0, 0), new Point3D(3, 0, 0))
                    .SetName("DifferentDikeToeAtRiver");
                yield return new TestCaseData(new Point3D(2, 0, 3), new Point3D(4, 0, 2))
                    .SetName("DifferentDikeToeAtPolder");
            }
        }

        [Test]
        public void GivenSurfaceLineSet_WhenSurfaceLineNull_ThenEntryAndExitPointsNaN()
        {
            // Given
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            input.SurfaceLine = surfaceLine;
            input.ExitPointL = (RoundedDouble) 5;
            input.EntryPointL = (RoundedDouble) 4;

            // When
            input.SurfaceLine = null;

            // Then
            Assert.IsNaN(input.EntryPointL);
            Assert.IsNaN(input.ExitPointL);
        }

        [Test]
        public void PhreaticLevelExit_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var random = new Random(22);
            var input = new PipingInput(new GeneralPipingInput());
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(3)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.PhreaticLevelExit = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.PhreaticLevelExit, distributionToSet, expectedDistribution);
        }

        [Test]
        public void DampingFactorExit_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var random = new Random(22);
            var input = new PipingInput(new GeneralPipingInput());
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new LogNormalDistribution(3)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.DampingFactorExit = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.DampingFactorExit, distributionToSet, expectedDistribution);
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputIsFalse_ReturnsNaN()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput())
            {
                UseAssessmentLevelManualInput = false,
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
            };

            // Call
            RoundedDouble calculatedAssessmentLevel = input.AssessmentLevel;

            // Assert
            Assert.IsNaN(calculatedAssessmentLevel);
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputIsFalseWithHydraulicLocationSetAndDesignWaterLevelOutputSet_ReturnCalculatedAssessmentLevel()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            HydraulicBoundaryLocation testHydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            input.HydraulicBoundaryLocation = testHydraulicBoundaryLocation;

            double calculatedAssessmentLevel = new Random(21).NextDouble();
            testHydraulicBoundaryLocation.DesignWaterLevelCalculation.Output = new TestHydraulicBoundaryLocationOutput(calculatedAssessmentLevel);

            // Call
            RoundedDouble newAssessmentLevel = input.AssessmentLevel;

            // Assert
            Assert.AreEqual(calculatedAssessmentLevel, newAssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputFalseAndSettingValue_ThrowsInvalidOperationException()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput())
            {
                UseAssessmentLevelManualInput = false
            };

            var testLevel = (RoundedDouble) new Random(21).NextDouble();

            // Call 
            TestDelegate call = () => input.AssessmentLevel = testLevel;

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual("UseAssessmentLevelManualInput is false", message);
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputTrueAndSettingValue_ReturnSetValue()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput())
            {
                UseAssessmentLevelManualInput = true
            };

            var testLevel = (RoundedDouble) new Random(21).NextDouble();

            // Call
            input.AssessmentLevel = testLevel;

            // Assert
            Assert.AreEqual(2, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(testLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void GivenAssessmentLevelSetByHydraulicBoundaryLocation_WhenManualAssessmentLevelTrueAndNewLevelSet_ThenLevelUpdatedAndLocationRemoved()
        {
            // Given
            var random = new Random(21);
            var testLevel = (RoundedDouble) random.NextDouble();
            var input = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(testLevel)
            };

            var newLevel = (RoundedDouble) random.NextDouble();

            // When
            input.UseAssessmentLevelManualInput = true;
            input.AssessmentLevel = newLevel;

            // Then
            Assert.AreEqual(2, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(newLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
            Assert.IsNull(input.HydraulicBoundaryLocation);
        }

        [Test]
        public void GivenAssessmentLevelSetByManualInput_WhenManualAssessmentLevelFalseAndHydraulicBoundaryLocationSet_ThenAssessmentLevelUpdatedAndLocationSet()
        {
            // Given
            var random = new Random(21);
            var testLevel = (RoundedDouble) random.NextDouble();
            var input = new PipingInput(new GeneralPipingInput())
            {
                UseAssessmentLevelManualInput = true,
                AssessmentLevel = testLevel
            };

            var newLevel = (RoundedDouble) random.NextDouble();
            TestHydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(newLevel);

            // When
            input.UseAssessmentLevelManualInput = false;
            input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Then
            Assert.AreEqual(2, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreSame(hydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(newLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void PiezometricHeadExit_ValidInput_SetsParametersForCalculatorAndReturnsPiezometricHead()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                RoundedDouble piezometricHead = input.PiezometricHeadExit;

                // Assert
                Assert.AreEqual(2, piezometricHead.NumberOfDecimalPlaces);
                Assert.IsFalse(double.IsNaN(piezometricHead));

                var factory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                PiezoHeadCalculatorStub piezometricHeadAtExitCalculator = factory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(piezometricHeadAtExitCalculator.HRiver, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                input.PhreaticLevelExit.GetAccuracy());
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                input.DampingFactorExit.GetAccuracy());
            }
        }

        [Test]
        public void PiezometricHeadExit_InputWithAssessmentLevelMissing_PiezometricHeadSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);

            // Call
            RoundedDouble piezometricHead = input.PiezometricHeadExit;

            // Assert
            Assert.IsNaN(piezometricHead);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsThicknessAquiferLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_AquiferMeanSetToConsecutiveAquiferLayerThickness()
        {
            // Setup
            double expectedThickness;
            PipingInput input = PipingInputFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetExitPointSetToNaN_ThicknessAquiferLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = RoundedDouble.NaN;

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_ProfileWithoutAquiferLayer_ThicknessAquiferLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquifer();

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToConsecutiveAquiferLayerThickness()
        {
            // Setup
            double expectedThickness;
            PipingInput input = PipingInputFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetSoilProfileSetToNull_ThicknessAquiferLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputResultsInZeroAquiferThickness_ThicknessAquiferLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(0.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.5)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(1.5)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(2.0, thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessCoverageLayer_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = input.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = input.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void EffectiveThicknessCoverageLayer_SoilProfileSingleAquiferAboveSurfaceLine_EffectiveThicknessCoverageLayerNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = input.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_MeanSetSoilProfileSetToNull_EffectiveThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EffectiveThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = input.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_ProfileWithoutAquiferLayer_EffectiveThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = input.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputResultsInZeroCoverageThickness_EffectiveThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = input.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SeepageLength_ValidData_ReturnsSeepageLength()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            VariationCoefficientLogNormalDistribution seepageLength = input.SeepageLength;

            // Assert
            Assert.AreEqual(0.5, seepageLength.Mean);
            Assert.AreEqual(0.1, seepageLength.CoefficientOfVariation);
        }

        [Test]
        public void SeepageLength_EntryPointNaN_SeepageLengthNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = RoundedDouble.NaN;

            // Call
            VariationCoefficientLogNormalDistribution seepageLength = input.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.AreEqual(0.1, seepageLength.CoefficientOfVariation);
        }

        [Test]
        public void SeepageLength_ExitPointNaN_SeepageLengthNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = RoundedDouble.NaN;

            // Call
            VariationCoefficientLogNormalDistribution seepageLength = input.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.AreEqual(0.1, seepageLength.CoefficientOfVariation);
        }

        private static PipingSurfaceLine CreateSurfaceLine()
        {
            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(2, 0, 2)
            });

            return surfaceLine;
        }

        private static void AssertDistributionCorrectlySet(IDistribution distributionToAssert, IDistribution setDistribution, IDistribution expectedDistribution)
        {
            Assert.AreNotSame(setDistribution, distributionToAssert);
            DistributionAssert.AreEqual(expectedDistribution, distributionToAssert);
        }
    }
}
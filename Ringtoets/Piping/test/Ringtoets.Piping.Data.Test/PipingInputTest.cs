using System;

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();

            // Call
            var inputParameters = new PipingInput(generalInputParameters);

            // Assert
            Assert.IsInstanceOf<Observable>(inputParameters);

            Assert.IsInstanceOf<NormalDistribution>(inputParameters.PhreaticLevelExit);
            Assert.AreEqual(0, inputParameters.PhreaticLevelExit.Mean.Value);
            Assert.AreEqual(3, inputParameters.PhreaticLevelExit.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.PhreaticLevelExit.StandardDeviation);

            double defaultLogNormalMean = Math.Exp(-0.5);
            double defaultLogNormalStandardDev = Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1));
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.DampingFactorExit);
            Assert.AreEqual(1, inputParameters.DampingFactorExit.Mean.Value);
            Assert.AreEqual(3, inputParameters.DampingFactorExit.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.DampingFactorExit.StandardDeviation);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.Diameter70);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.Diameter70.Mean,
                            Math.Pow(10.0, -inputParameters.Diameter70.Mean.NumberOfDecimalPlaces));
            Assert.AreEqual(2, inputParameters.Diameter70.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.Diameter70.StandardDeviation);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.DarcyPermeability);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.DarcyPermeability.Mean,
                            Math.Pow(10.0, -inputParameters.DarcyPermeability.Mean.NumberOfDecimalPlaces));
            Assert.AreEqual(3, inputParameters.DarcyPermeability.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.DarcyPermeability.StandardDeviation);

            Assert.AreEqual(0, inputParameters.PiezometricHeadExit);
            Assert.AreEqual(0, inputParameters.PiezometricHeadPolder);
            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.SoilProfile);
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

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.ThicknessCoverageLayer);
            Assert.IsNaN(inputParameters.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(2, inputParameters.ThicknessCoverageLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, inputParameters.ThicknessCoverageLayer.StandardDeviation);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.ThicknessAquiferLayer);
            Assert.IsNaN(inputParameters.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(2, inputParameters.ThicknessAquiferLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, inputParameters.ThicknessAquiferLayer.StandardDeviation);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.SeepageLength);
            Assert.IsNaN(inputParameters.SeepageLength.Mean);
            Assert.AreEqual(2, inputParameters.SeepageLength.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.SeepageLength.StandardDeviation);

            Assert.IsNaN(inputParameters.ExitPointL);
            Assert.AreEqual(2, inputParameters.ExitPointL.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.EntryPointL);
            Assert.AreEqual(2, inputParameters.EntryPointL.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<RoundedDouble>(inputParameters.AssessmentLevel);
            Assert.AreEqual(2, inputParameters.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.AssessmentLevel);
        }

        [Test]
        public void Constructor_GeneralPipingInputIsNull_ArgumentNullException()
        {
            // Setup

            // Call
            TestDelegate call = () => new PipingInput(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void ExitPointL_SetToValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            int originalNumberOfDecimalPlaces = pipingInput.ExitPointL.NumberOfDecimalPlaces;

            // Call
            pipingInput.ExitPointL = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, pipingInput.ExitPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, pipingInput.ExitPointL.Value);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-21)]
        public void ExitPointL_ValueLessThanOrEqualToZero_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            TestDelegate test = () => pipingInput.ExitPointL = (RoundedDouble)value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, Resources.PipingInput_ExitPointL_Value_must_be_greater_than_zero);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-21)]
        public void EntryPointL_ValueLessThanZero_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            TestDelegate test = () => pipingInput.EntryPointL = (RoundedDouble)value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, Resources.PipingInput_EntryPointL_Value_must_be_greater_than_or_equal_to_zero);
        }

        [Test]
        public void EntryPointL_SetToNewValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            int originalNumberOfDecimalPlaces = pipingInput.EntryPointL.NumberOfDecimalPlaces;

            // Call
            pipingInput.EntryPointL = new RoundedDouble(5, 9.87654);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, pipingInput.EntryPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(9.88, pipingInput.EntryPointL.Value);
        }

        [Test]
        [TestCase(double.NaN, double.NaN, double.NaN)]
        [TestCase(double.NaN, 3, double.NaN)]
        [TestCase(2, double.NaN, double.NaN)]
        [TestCase(2, 4, 2.0)]
        [TestCase(0, 3, 3.0)]
        [TestCase(4e-3, 6, 6.0)]
        public void SeepageLength_DifferentCombinationsOfEntryAndExitPoint_SeepageLengthUpdatedAsExpected(double entryPointL, double exitPointL, double expectedSeepageLength)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            pipingInput.ExitPointL = (RoundedDouble)exitPointL;
            pipingInput.EntryPointL = (RoundedDouble)entryPointL;

            // Assert
            Assert.AreEqual(expectedSeepageLength, pipingInput.SeepageLength.Mean.Value);
            Assert.AreEqual(expectedSeepageLength * 0.1, pipingInput.SeepageLength.StandardDeviation);
        }

        [Test]
        [TestCase(4, 2)]
        [TestCase(3 + 1e-6, 3)]
        public void SeepageLength_SettingEntryPastExitPoint_SeepageLengthSetToNaN(double entryPointL, double exitPointL)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());
            pipingInput.ExitPointL = (RoundedDouble)exitPointL;

            // Call
            pipingInput.EntryPointL = (RoundedDouble)entryPointL;

            // Assert
            Assert.IsNaN(pipingInput.SeepageLength.Mean);
        }

        [Test]
        public void AssessmentLevel_SetToNewValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            int originalNumberOfDecimalPlaces = pipingInput.AssessmentLevel.NumberOfDecimalPlaces;

            // Call
            pipingInput.AssessmentLevel = new RoundedDouble(5, -8.29292);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, pipingInput.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(-8.29, pipingInput.AssessmentLevel.Value);
        }
    }
}
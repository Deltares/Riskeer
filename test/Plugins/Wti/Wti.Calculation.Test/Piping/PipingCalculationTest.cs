using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wti.Calculation.Piping;

namespace Wti.Calculation.Test.Piping
{
    public class PipingCalculationTest
    {
        [Test]
        public void GivenACompleteInput_WhenCalculationPerformed_ThenResultContainsNoNaN()
        {
            PipingCalculationInput input = new TestPipingInput().AsRealInput();

            PipingCalculationResult actual = new PipingCalculation(input).Calculate();

            Assert.NotNull(actual);
            Assert.IsFalse(double.IsNaN(actual.UpliftZValue));
            Assert.IsFalse(double.IsNaN(actual.UpliftFactorOfSafety));
            Assert.IsFalse(double.IsNaN(actual.HeaveZValue));
            Assert.IsFalse(double.IsNaN(actual.HeaveFactorOfSafety));
            Assert.IsFalse(double.IsNaN(actual.SellmeijerZValue));
            Assert.IsFalse(double.IsNaN(actual.SellmeijerFactorOfSafety));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_ZeroOrNegativeSeepageLength_ValidationMessageForPipingLength(double seepageLength)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                SeepageLength = seepageLength
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("Piping length")));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_ZeroOrNegativeAquiferThickness_ValidationMessageForDAquifer(double aquiferThickness)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                ThicknessAquiferLayer = aquiferThickness
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("DAquifer")));
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_NegativeBeddingAngle_ValidationMessageForBeddingAngle(double beddingAngle)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                BeddingAngle = beddingAngle
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("Bedding angle")));
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_PiezometricHeadExitSameAsPhreaticLevelExit_ValidationMessageForPhiExitAndHExit(double level)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                PiezometricHeadExit = level,
                PhreaticLevelExit = level
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(2, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("PhiExit -  HExit")));
            Assert.IsTrue(validationMessages.Any(message => message.Contains("phiExit - hExit")));
        }

        class TestPipingInput
        {
            public double WaterVolumetricWeight;
            public double UpliftModelFactor;
            public double AssessmentLevel;
            public double PiezometricHeadExit;
            public double DampingFactorExit;
            public double PhreaticLevelExit;
            public double PiezometricHeadPolder;
            public double CriticalHeaveGradient;
            public double ThicknessCoverageLayer;
            public double SellmeijerModelFactor;
            public double SellmeijerReductionFactor;
            public double SeepageLength;
            public double SandParticlesVolumicWeight;
            public double WhitesDragCoefficient;
            public double Diameter70;
            public double DarcyPermeability;
            public double WaterKinematicViscosity;
            public double Gravity;
            public double ExitPointXCoordinate;
            public double BeddingAngle;
            public double MeanDiameter70;
            public double ThicknessAquiferLayer;

            private readonly Random random = new Random(22);
            private double last;

            public TestPipingInput()
            {
                WaterVolumetricWeight = NextDouble();
                UpliftModelFactor = NextDouble();
                AssessmentLevel = NextDouble();
                PiezometricHeadExit = NextDouble();
                PhreaticLevelExit = NextDouble();
                DampingFactorExit = NextDouble();
                PiezometricHeadPolder = NextDouble();
                CriticalHeaveGradient = NextDouble();
                ThicknessCoverageLayer = NextDouble();
                SellmeijerModelFactor = NextDouble();
                SellmeijerReductionFactor = NextDouble();
                SeepageLength = NextDouble();
                SandParticlesVolumicWeight = NextDouble();
                WhitesDragCoefficient = NextDouble();
                Diameter70 = NextDouble();
                DarcyPermeability = NextDouble();
                WaterKinematicViscosity = NextDouble();
                Gravity = NextDouble();
                ExitPointXCoordinate = NextDouble();
                BeddingAngle = NextDouble();
                MeanDiameter70 = NextDouble();
                ThicknessAquiferLayer = NextDouble();
            }

            /// <summary>
            /// The returned double is sure to be different from the last time it was called.
            /// </summary>
            /// <returns></returns>
            private double NextDouble()
            {
                return last += random.NextDouble() + 1e-6;
            }

            /// <summary>
            /// Returns the current set value as a <see cref="PipingCalculationInput"/>
            /// </summary>
            /// <returns></returns>
            public PipingCalculationInput AsRealInput()
            {
                return new PipingCalculationInput(
                    WaterVolumetricWeight,
                    UpliftModelFactor,
                    AssessmentLevel,
                    PiezometricHeadExit,
                    DampingFactorExit,
                    PhreaticLevelExit,
                    PiezometricHeadPolder,
                    CriticalHeaveGradient,
                    ThicknessCoverageLayer,
                    SellmeijerModelFactor,
                    SellmeijerReductionFactor,
                    SeepageLength,
                    SandParticlesVolumicWeight,
                    WhitesDragCoefficient,
                    Diameter70,
                    DarcyPermeability,
                    WaterKinematicViscosity,
                    Gravity,
                    ThicknessAquiferLayer,
                    MeanDiameter70,
                    BeddingAngle,
                    ExitPointXCoordinate
                );
            }
        }
    }
}
﻿using System;
using System.Linq;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Deltares.WTIPiping;
using Ringtoets.Piping.Calculation.TestUtil;

using NUnit.Framework;
using Ringtoets.Piping.Calculation;
using Ringtoets.Piping.Calculation.TestUtil.SubCalculator;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Service.TestUtil;

namespace Ringtoets.Piping.Service.Test
{
    public class PipingCalculationServiceTest
    {
        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation pipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            pipingCalculation.Name = name;

            // Call
            Action call = () => PipingCalculationService.Validate(pipingCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void Validate_InValidPipingCalculationWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestPipingOutput();
            PipingCalculation invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidPipingCalculation.Output = output;

            // Call
            var isValid = PipingCalculationService.Validate(invalidPipingCalculation);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidPipingCalculation.Output);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_InValidPipingCalculationWithOutput_LogsError()
        {
            // Setup
            PipingCalculation invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();

            // Make invalid by having surfaceline partially above soil profile:
            double highestLevelSurfaceLine = invalidPipingCalculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
            var soilProfileTop = highestLevelSurfaceLine - 0.5;
            var soilProfileBottom = soilProfileTop - 0.5;
            invalidPipingCalculation.InputParameters.SoilProfile = new PipingSoilProfile("A", soilProfileBottom, new[]
            {
                new PipingSoilLayer(soilProfileTop)
                {
                    IsAquifer = true
                } 
            });

            // Call
            Action call = () => PipingCalculationService.CalculateThicknessCoverageLayer(invalidPipingCalculation.InputParameters);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith("Berekenen van de dikte van de deklaag niet gelukt: ", msgs[0]);
            });
        }

        [Test]
        public void CalculateThicknessCoverageLayer_ValidInput_ReturnsThickness()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 10),
                new Point3D(20, 0, 10)
            });

            var soilProfile = new PipingSoilProfile(string.Empty, 0, new[]
            {
                new PipingSoilLayer(5)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(20)
                {
                    IsAquifer = false
                }
            });

            PipingInput input = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 10,
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            // Call
            var thickness = PipingCalculationService.CalculateThicknessCoverageLayer(input);

            // Assert
            Assert.AreEqual(5, thickness);
        }

        [Test]
        public void CalculatePiezometricHeadAtExit_Always_ReturnsResult()
        {
            // Setup
            PipingInput input = new PipingInput(new GeneralPipingInput())
            {
                AssessmentLevel = (RoundedDouble) 0.0
            };

            // Call
            var result = PipingCalculationService.CalculatePiezometricHeadAtExit(input);

            // Assert
            Assert.IsFalse(double.IsNaN(result));
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingCalculation_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Name = name;

            // Call
            Action call = () =>
            {
                Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));
                PipingCalculationService.Calculate(validPipingCalculation);
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);

                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", name), msgs[2]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingCalculationNoOutput_ShouldSetOutput()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();

            // Precondition
            Assert.IsNull(validPipingCalculation.Output);

            // Call
            Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));
            PipingCalculationService.Calculate(validPipingCalculation);

            // Assert
            Assert.IsNotNull(validPipingCalculation.Output);
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingCalculationWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestPipingOutput();

            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Output = output;

            // Call
            Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));
            PipingCalculationService.Calculate(validPipingCalculation);

            // Assert
            Assert.AreNotSame(output, validPipingCalculation.Output);
        }

        [Test]
        public void Validate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingCalculationServiceConfig())
            {
                // Call
                PipingCalculationService.Validate(validPipingCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        [Test]
        public void Calculate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingCalculationServiceConfig())
            {
                // Call
                PipingCalculationService.Calculate(validPipingCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        [Test]
        public void CalculateThicknessCoverageLayer_CompleteInput_InputSetOnSubCalculator()
        {

            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingCalculationServiceConfig())
            {
                // Call
                PipingCalculationService.CalculateThicknessCoverageLayer(validPipingCalculation.InputParameters);

                // Assert
                var testFactory = (TestPipingSubCalculatorFactory)PipingCalculationService.SubCalculatorFactory;
                var effectiveThicknessCalculator = testFactory.LastCreatedEffectiveThicknessCalculator;

                Assert.AreEqual(input.ExitPointL.Value, effectiveThicknessCalculator.ExitPointXCoordinate);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), effectiveThicknessCalculator.PhreaticLevel,
                                Math.Pow(10.0, -input.PhreaticLevelExit.Mean.NumberOfDecimalPlaces));
                AssertEqualSoilProfiles(input.SoilProfile, effectiveThicknessCalculator.SoilProfile);
                AssertEqualSurfaceLines(input.SurfaceLine, effectiveThicknessCalculator.SurfaceLine);
                Assert.AreEqual(input.WaterVolumetricWeight, effectiveThicknessCalculator.VolumicWeightOfWater);
            }
        }

        [Test]
        public void CalculatePiezometricHeadAtExit_CompleteInput_InputSetOnSubCalculator()
        {

            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingCalculationServiceConfig())
            {
                // Call
                PipingCalculationService.CalculatePiezometricHeadAtExit(validPipingCalculation.InputParameters);

                // Assert
                var testFactory = (TestPipingSubCalculatorFactory)PipingCalculationService.SubCalculatorFactory;
                var piezometricHeadAtExitCalculator = testFactory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(input.AssessmentLevel.Value, piezometricHeadAtExitCalculator.HRiver);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                GetAccuracy(input.PhreaticLevelExit.Mean));
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                GetAccuracy(input.DampingFactorExit.Mean));
            }
        }

        private void AssertSubCalculatorInputs(PipingInput input)
        {
            var testFactory = (TestPipingSubCalculatorFactory) PipingCalculationService.SubCalculatorFactory;
            var heaveCalculator = testFactory.LastCreatedHeaveCalculator;
            var upliftCalculator = testFactory.LastCreatedUpliftCalculator;
            var sellmeijerCalculator = testFactory.LastCreatedSellmeijerCalculator;
            
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue(), heaveCalculator.DTotal,
                            GetAccuracy(input.ThicknessCoverageLayer.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), heaveCalculator.HExit,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(input.CriticalHeaveGradient, heaveCalculator.Ich);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), heaveCalculator.PhiPolder,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(input.PiezometricHeadExit.Value, heaveCalculator.PhiExit);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), heaveCalculator.RExit,
                            GetAccuracy(input.DampingFactorExit.Mean));

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), upliftCalculator.HExit,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(input.AssessmentLevel.Value, upliftCalculator.HRiver);
            Assert.AreEqual(input.UpliftModelFactor, upliftCalculator.ModelFactorUplift);
            Assert.AreEqual(input.PiezometricHeadExit.Value, upliftCalculator.PhiExit);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), upliftCalculator.PhiPolder,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), upliftCalculator.RExit,
                            GetAccuracy(input.DampingFactorExit.Mean));
            Assert.AreEqual(input.WaterVolumetricWeight, upliftCalculator.VolumetricWeightOfWater);
            RoundedDouble effectiveStress = PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue() * 
                (PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue() - input.WaterVolumetricWeight);
            Assert.AreEqual(effectiveStress, upliftCalculator.EffectiveStress,
                            GetAccuracy(effectiveStress));

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(input).GetDesignValue(), sellmeijerCalculator.SeepageLength,
                            GetAccuracy(input.SeepageLength.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), sellmeijerCalculator.HExit,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(input.AssessmentLevel.Value, sellmeijerCalculator.HRiver);
            Assert.AreEqual(input.WaterKinematicViscosity, sellmeijerCalculator.KinematicViscosityWater);
            Assert.AreEqual(input.SellmeijerModelFactor, sellmeijerCalculator.ModelFactorPiping);
            Assert.AreEqual(input.SellmeijerReductionFactor, sellmeijerCalculator.Rc);
            Assert.AreEqual(input.WaterVolumetricWeight, sellmeijerCalculator.VolumetricWeightOfWater);
            Assert.AreEqual(input.WhitesDragCoefficient, sellmeijerCalculator.WhitesDragCoefficient);
            Assert.AreEqual(input.BeddingAngle, sellmeijerCalculator.BeddingAngle);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue(), sellmeijerCalculator.DTotal,
                            GetAccuracy(input.ThicknessCoverageLayer.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDiameter70(input).GetDesignValue(), sellmeijerCalculator.D70,
                            GetAccuracy(input.Diameter70.Mean));
            Assert.AreEqual(input.MeanDiameter70, sellmeijerCalculator.D70Mean);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(input).GetDesignValue(), sellmeijerCalculator.DAquifer,
                            GetAccuracy(input.ThicknessAquiferLayer.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(input).GetDesignValue(), sellmeijerCalculator.DarcyPermeability,
                            GetAccuracy(input.DarcyPermeability.Mean));
            Assert.AreEqual(input.SandParticlesVolumicWeight, sellmeijerCalculator.GammaSubParticles);
            Assert.AreEqual(input.Gravity, sellmeijerCalculator.Gravity);
        }

        private static double GetAccuracy(RoundedDouble roundedDouble)
        {
            return Math.Pow(10.0, -roundedDouble.NumberOfDecimalPlaces);
        }

        private void AssertEqualSurfaceLines(RingtoetsPipingSurfaceLine pipingSurfaceLine, PipingSurfaceLine otherSurfaceLine)
        {
            AssertPointsAreEqual(pipingSurfaceLine.DitchDikeSide, otherSurfaceLine.DitchDikeSide);
            AssertPointsAreEqual(pipingSurfaceLine.BottomDitchDikeSide, otherSurfaceLine.BottomDitchDikeSide);
            AssertPointsAreEqual(pipingSurfaceLine.BottomDitchPolderSide, otherSurfaceLine.BottomDitchPolderSide);
            AssertPointsAreEqual(pipingSurfaceLine.DitchPolderSide, otherSurfaceLine.DitchPolderSide);
            AssertPointsAreEqual(pipingSurfaceLine.DikeToeAtPolder, otherSurfaceLine.DikeToeAtPolder);

            Assert.AreEqual(pipingSurfaceLine.Points.Length, otherSurfaceLine.Points.Count);
            for (int i = 0; i < pipingSurfaceLine.Points.Length; i++)
            {
                AssertPointsAreEqual(pipingSurfaceLine.Points[i], otherSurfaceLine.Points[i]);
            }
        }

        private void AssertPointsAreEqual(Point3D point, PipingPoint otherPoint)
        {
            if (point == null)
            {
                Assert.IsNull(otherPoint);
                return;
            }
            if (otherPoint == null)
            {
                Assert.Fail("Expected value for otherPoint.");
            }
            Assert.AreEqual(point.X, otherPoint.X);
            Assert.AreEqual(point.Y, otherPoint.Y);
            Assert.AreEqual(point.Z, otherPoint.Z);
        }

        private void AssertEqualSoilProfiles(PipingSoilProfile pipingProfile, PipingProfile otherPipingProfile)
        {
            Assert.AreEqual(pipingProfile.Bottom, otherPipingProfile.BottomLevel);
            Assert.AreEqual(pipingProfile.Layers.First().Top, otherPipingProfile.TopLevel);
            Assert.AreEqual(pipingProfile.Layers.Last(l => l.IsAquifer).Top, otherPipingProfile.BottomAquiferLayer.TopLevel);
            Assert.AreEqual(pipingProfile.Layers.First(l => l.IsAquifer).Top, otherPipingProfile.TopAquiferLayer.TopLevel);

            Assert.AreEqual(pipingProfile.Layers.Count(), otherPipingProfile.Layers.Count);
            for (int i = 0; i < pipingProfile.Layers.Count(); i++)
            {
                Assert.AreEqual(pipingProfile.Layers.ElementAt(i).Top, otherPipingProfile.Layers[i].TopLevel);
            }
        }
    }
}
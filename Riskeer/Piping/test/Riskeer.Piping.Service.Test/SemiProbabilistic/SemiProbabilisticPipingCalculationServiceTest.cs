﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.KernelWrapper;
using Riskeer.Piping.KernelWrapper.SubCalculator;
using Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service.SemiProbabilistic;

namespace Riskeer.Piping.Service.Test.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingCalculationServiceTest
    {
        private const string averagingSoilLayerPropertiesMessage = "Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaardafwijkingen en verschuivingen voor alle lagen gelijk zijn.";
        private double testSurfaceLineTopLevel;
        private SemiProbabilisticPipingCalculation testCalculation;

        [SetUp]
        public void Setup()
        {
            testCalculation = SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestSemiProbabilisticPipingCalculation>(
                new TestHydraulicBoundaryLocation());
            testSurfaceLineTopLevel = testCalculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
        }

        private static void AssertSubCalculatorInputs(PipingInput input, GeneralPipingInput generalPipingInput, RoundedDouble expectedAssessmentLevel)
        {
            var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
            HeaveCalculatorStub heaveCalculator = testFactory.LastCreatedHeaveCalculator;
            UpliftCalculatorStub upliftCalculator = testFactory.LastCreatedUpliftCalculator;
            SellmeijerCalculatorStub sellmeijerCalculator = testFactory.LastCreatedSellmeijerCalculator;

            RoundedDouble expectedThicknessCoverageLayerDesignValue = SemiProbabilisticPipingDesignVariableFactory.GetThicknessCoverageLayer(input).GetDesignValue();
            double thicknessCoverageLayerAccuracy = DerivedPipingInput.GetThicknessCoverageLayer(input).GetAccuracy();
            RoundedDouble expectedPhreaticLevelExitDesignValue = SemiProbabilisticPipingDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue();
            double phreaticLevelExitDesignAccuracy = input.PhreaticLevelExit.GetAccuracy();
            double expectedPiezometricHeadExit = DerivedSemiProbabilisticPipingInput.GetPiezometricHeadExit(input, expectedAssessmentLevel).Value;
            RoundedDouble expectedDampingFactorExitDesignValue = SemiProbabilisticPipingDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue();
            double dampingFactorExitAccuracy = input.DampingFactorExit.GetAccuracy();

            Assert.AreEqual(expectedThicknessCoverageLayerDesignValue, heaveCalculator.DTotal, thicknessCoverageLayerAccuracy);
            Assert.AreEqual(expectedPhreaticLevelExitDesignValue, heaveCalculator.HExit, phreaticLevelExitDesignAccuracy);
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetCriticalHeaveGradientDesignVariable(generalPipingInput).GetDesignValue(), heaveCalculator.Ich);
            Assert.AreEqual(expectedPhreaticLevelExitDesignValue, heaveCalculator.PhiPolder, phreaticLevelExitDesignAccuracy);
            Assert.AreEqual(expectedPiezometricHeadExit, heaveCalculator.PhiExit);
            Assert.AreEqual(expectedDampingFactorExitDesignValue, heaveCalculator.RExit, dampingFactorExitAccuracy);
            Assert.AreEqual(expectedPhreaticLevelExitDesignValue, upliftCalculator.HExit, phreaticLevelExitDesignAccuracy);
            Assert.AreEqual(expectedAssessmentLevel, upliftCalculator.HRiver);
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetUpliftModelFactorDesignVariable(generalPipingInput).GetDesignValue(), upliftCalculator.ModelFactorUplift);
            Assert.AreEqual(expectedPiezometricHeadExit, upliftCalculator.PhiExit);
            Assert.AreEqual(expectedPhreaticLevelExitDesignValue, upliftCalculator.PhiPolder, phreaticLevelExitDesignAccuracy);
            Assert.AreEqual(expectedDampingFactorExitDesignValue, upliftCalculator.RExit, dampingFactorExitAccuracy);
            Assert.AreEqual(generalPipingInput.WaterVolumetricWeight, upliftCalculator.VolumetricWeightOfWater);

            RoundedDouble effectiveThickness = SemiProbabilisticPipingDesignVariableFactory.GetEffectiveThicknessCoverageLayer(input, generalPipingInput).GetDesignValue();
            RoundedDouble saturatedVolumicWeight = SemiProbabilisticPipingDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue();
            RoundedDouble expectedEffectiveStress = effectiveThickness * (saturatedVolumicWeight - generalPipingInput.WaterVolumetricWeight);
            Assert.AreEqual(expectedEffectiveStress, upliftCalculator.EffectiveStress, expectedEffectiveStress.GetAccuracy());

            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetSeepageLength(input).GetDesignValue(),
                            sellmeijerCalculator.SeepageLength,
                            DerivedPipingInput.GetSeepageLength(input).GetAccuracy());
            Assert.AreEqual(expectedPhreaticLevelExitDesignValue, sellmeijerCalculator.HExit, phreaticLevelExitDesignAccuracy);
            Assert.AreEqual(expectedAssessmentLevel, sellmeijerCalculator.HRiver);
            Assert.AreEqual(generalPipingInput.WaterKinematicViscosity, sellmeijerCalculator.KinematicViscosityWater);
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetSellmeijerModelFactorDesignVariable(generalPipingInput).GetDesignValue(), sellmeijerCalculator.ModelFactorPiping);
            Assert.AreEqual(generalPipingInput.SellmeijerReductionFactor, sellmeijerCalculator.Rc);
            Assert.AreEqual(generalPipingInput.WaterVolumetricWeight, sellmeijerCalculator.VolumetricWeightOfWater);
            Assert.AreEqual(generalPipingInput.WhitesDragCoefficient, sellmeijerCalculator.WhitesDragCoefficient);
            Assert.AreEqual(generalPipingInput.BeddingAngle, sellmeijerCalculator.BeddingAngle);
            Assert.AreEqual(expectedThicknessCoverageLayerDesignValue, sellmeijerCalculator.DTotal, thicknessCoverageLayerAccuracy);
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetDiameter70(input).GetDesignValue(),
                            sellmeijerCalculator.D70,
                            DerivedPipingInput.GetDiameterD70(input).GetAccuracy());
            Assert.AreEqual(generalPipingInput.MeanDiameter70, sellmeijerCalculator.D70Mean);
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetThicknessAquiferLayer(input).GetDesignValue(),
                            sellmeijerCalculator.DAquifer,
                            DerivedPipingInput.GetThicknessAquiferLayer(input).GetAccuracy());
            Assert.AreEqual(SemiProbabilisticPipingDesignVariableFactory.GetDarcyPermeability(input).GetDesignValue(),
                            sellmeijerCalculator.DarcyPermeability,
                            DerivedPipingInput.GetDarcyPermeability(input).GetAccuracy());
            Assert.AreEqual(generalPipingInput.SandParticlesVolumicWeight, sellmeijerCalculator.GammaSubParticles);
            Assert.AreEqual(generalPipingInput.Gravity, sellmeijerCalculator.Gravity);
        }

        #region Validate

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingCalculationService.Validate(null,
                                                                              new GeneralPipingInput(),
                                                                              RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingCalculationService.Validate(new TestSemiProbabilisticPipingCalculation(),
                                                                              null,
                                                                              RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void Validate_Always_LogsStartAndEndOfValidation()
        {
            // Call
            void Call() => SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                              new GeneralPipingInput(),
                                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
        }

        [Test]
        public void Validate_InvalidPipingCalculationWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            SemiProbabilisticPipingOutput output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();
            var invalidPipingCalculation =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<TestSemiProbabilisticPipingCalculation>();
            invalidPipingCalculation.Output = output;

            // Call
            bool isValid = SemiProbabilisticPipingCalculationService.Validate(invalidPipingCalculation,
                                                                              new GeneralPipingInput(),
                                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidPipingCalculation.Output);
        }

        [Test]
        public void Validate_InvalidCalculationInput_LogsMessagesAndReturnsFalse()
        {
            // Setup
            var calculation = new TestSemiProbabilisticPipingCalculation();

            // Call
            var isValid = false;

            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(calculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(7, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische belastingenlocatie geselecteerd.", msgs[1]);
                Assert.AreEqual("Er is geen profielschematisatie geselecteerd.", msgs[2]);
                Assert.AreEqual("Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                Assert.AreEqual("De waarde voor 'uittredepunt' moet een concreet getal zijn.", msgs[4]);
                Assert.AreEqual("De waarde voor 'intredepunt' moet een concreet getal zijn.", msgs[5]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[6]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NormativeAssessmentLevelNotCalculated_LogsMessagesAndReturnsFalse()
        {
            // Setup
            testCalculation.InputParameters.UseAssessmentLevelManualInput = false;

            // Call
            var isValid = false;

            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        RoundedDouble.NaN);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan de waterstand niet afleiden op basis van de invoer.", msgs[1]);
                Assert.AreEqual("Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidManualAssessmentLevel_LogsMessagesAndReturnsFalse(double assessmentLevel)
        {
            // Setup
            testCalculation.InputParameters.UseAssessmentLevelManualInput = true;
            testCalculation.InputParameters.AssessmentLevel = (RoundedDouble) assessmentLevel;

            // Call
            var isValid = false;

            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De waarde voor 'Waterstand' moet een concreet getal zijn.", msgs[1]);
                Assert.AreEqual("Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_ValidData_SubCalculatorsValidated()
        {
            // Setup
            using (new PipingSubCalculatorFactoryConfig())
            {
                var factory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;

                // Call
                SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                   new GeneralPipingInput(),
                                                                   AssessmentSectionTestHelper.GetTestAssessmentLevel());

                // Assert
                Assert.IsTrue(factory.LastCreatedHeaveCalculator.Validated);
                Assert.IsTrue(factory.LastCreatedSellmeijerCalculator.Validated);
                Assert.IsTrue(factory.LastCreatedUpliftCalculator.Validated);
                Assert.IsTrue(factory.LastCreatedEffectiveThicknessCalculator.Validated);
                Assert.IsTrue(factory.LastCreatedPipingProfilePropertyCalculator.Validated);
            }
        }

        [Test]
        public void Validate_WithoutAquiferLayer_LogsMessagesAndReturnsFalse()
        {
            // Setup
            var aquitardLayer = new PipingSoilLayer(2.0)
            {
                IsAquifer = false
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    aquitardLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                Assert.AreEqual("Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", msgs[3]);
                Assert.AreEqual("Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[4]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[5]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutAquitardLayer_LogsMessagesAndReturnsTrue()
        {
            // Setup
            var aquiferLayer = new PipingSoilLayer(10.56)
            {
                IsAquifer = true,
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    aquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_WithoutCoverageLayer_LogsMessagesAndReturnsTrue()
        {
            // Setup
            var coverageLayerAboveSurfaceLine = new PipingSoilLayer(13.0)
            {
                IsAquifer = false
            };
            var bottomAquiferLayer = new PipingSoilLayer(11.0)
            {
                IsAquifer = true,
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    coverageLayerAboveSurfaceLine,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_MultipleCoverageLayers_LogsMessageAndReturnsTrue()
        {
            // Setup
            var random = new Random(21);
            const double belowPhreaticLevelDeviation = 0.5;
            const int belowPhreaticLevelShift = 1;
            const double belowPhreaticLevelMeanBase = 15.0;

            var topCoverageLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) (belowPhreaticLevelMeanBase + belowPhreaticLevelShift + random.NextDouble()),
                    StandardDeviation = (RoundedDouble) belowPhreaticLevelDeviation,
                    Shift = (RoundedDouble) belowPhreaticLevelShift
                }
            };
            var middleCoverageLayer = new PipingSoilLayer(8.5)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) (belowPhreaticLevelMeanBase + belowPhreaticLevelShift + random.NextDouble()),
                    StandardDeviation = (RoundedDouble) belowPhreaticLevelDeviation,
                    Shift = (RoundedDouble) belowPhreaticLevelShift
                }
            };
            var bottomAquiferLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    topCoverageLayer,
                                                    middleCoverageLayer,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(averagingSoilLayerPropertiesMessage, msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_IncompleteDiameterD70Definition_LogsMessageAndReturnsFalse(bool meanSet, bool coefficientOfVariationSet)
        {
            // Setup
            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = meanSet
                               ? random.NextRoundedDouble(1, double.MaxValue)
                               : RoundedDouble.NaN,
                    CoefficientOfVariation = coefficientOfVariationSet
                                                 ? random.NextRoundedDouble()
                                                 : RoundedDouble.NaN
                }
            };

            var completeLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(15.0, 999.999),
                    StandardDeviation = random.NextRoundedDouble(1e-6, 5.0),
                    Shift = random.NextRoundedDouble(1e-6, 10)
                }
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    completeLayer,
                                                    incompletePipingSoilLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan de definitie voor het 70%-fraktiel van de korreldiameter van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(6.2e-5)]
        [TestCase(5.1e-3)]
        public void Validate_InvalidDiameterD70Value_LogsMessageAndReturnsTrue(double diameter70Value)
        {
            // Setup
            var random = new Random(21);
            var coverageLayerInvalidD70 = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) diameter70Value,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };
            var validLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(15.0, 999.999),
                    StandardDeviation = random.NextRoundedDouble(1e-6, 5.0),
                    Shift = random.NextRoundedDouble(1e-6, 10)
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    validLayer,
                                                    coverageLayerInvalidD70
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"Rekenwaarde voor d70 ({new RoundedDouble(6, diameter70Value)} m) ligt buiten het geldigheidsbereik van dit model. Geldige waarden liggen tussen 0.000063 m en 0.0005 m.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_IncompletePermeabilityDefinition_LogsMessageAndReturnsFalse(bool meanSet, bool coefficientOfVariationSet)
        {
            // Setup
            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = meanSet
                               ? random.NextRoundedDouble(1, double.MaxValue)
                               : RoundedDouble.NaN,
                    CoefficientOfVariation = coefficientOfVariationSet
                                                 ? random.NextRoundedDouble()
                                                 : RoundedDouble.NaN
                }
            };

            var completeLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(15.0, 999.999),
                    StandardDeviation = random.NextRoundedDouble(1e-6, 999.999),
                    Shift = random.NextRoundedDouble(1e-6, 10)
                }
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    completeLayer,
                                                    incompletePipingSoilLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan de definitie voor de doorlatendheid van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void Validate_IncompleteSaturatedVolumicWeightDefinition_LogsMessageAndReturnsFalse(bool meanSet, bool deviationSet, bool shiftSet)
        {
            // Setup
            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = meanSet
                               ? random.NextRoundedDouble(1, double.MaxValue)
                               : RoundedDouble.NaN,
                    StandardDeviation = deviationSet
                                            ? random.NextRoundedDouble()
                                            : RoundedDouble.NaN,
                    Shift = shiftSet
                                ? random.NextRoundedDouble()
                                : RoundedDouble.NaN
                }
            };

            var completeLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    incompletePipingSoilLayer,
                                                    completeLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_SaturatedCoverageLayerVolumicWeightLessThanWaterVolumicWeight_LogsMessageAndReturnsFalse()
        {
            // Setup
            var coverageLayerInvalidSaturatedVolumicWeight = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) 9.81,
                    StandardDeviation = (RoundedDouble) 2,
                    Shift = (RoundedDouble) 0
                }
            };
            var validLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 0.0002,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    coverageLayerInvalidSaturatedVolumicWeight,
                                                    validLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                                        new GeneralPipingInput(),
                                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(
                    "Het verzadigd volumetrisch gewicht van de deklaag moet groter zijn dan het volumetrisch gewicht van water.",
                    msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_SaturatedCoverageLayerLessThanWaterLayerAndMissingSaturatedParameter_LogsMessageOnlyForIncompleteDefinition()
        {
            // Setup
            var topCoverageLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) 5,
                    StandardDeviation = (RoundedDouble) 2,
                    Shift = (RoundedDouble) 0
                }
            };
            var middleCoverageLayerMissingParameter = new PipingSoilLayer(8.5)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) 5,
                    StandardDeviation = (RoundedDouble) 2,
                    Shift = RoundedDouble.NaN
                }
            };
            var bottomAquiferLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 0.3,
                    CoefficientOfVariation = (RoundedDouble) 0.6
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 0.0002,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    topCoverageLayer,
                                                    middleCoverageLayerMissingParameter,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            testCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            void Call() => SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                              new GeneralPipingInput(),
                                                                              AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(averagingSoilLayerPropertiesMessage, msgs[1]);
                Assert.AreEqual(
                    "Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.",
                    msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Validate_CompleteInput_InputSetOnSubCalculators(bool useAssessmentLevelManualInput)
        {
            // Setup
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();
            SemiProbabilisticPipingInput input = testCalculation.InputParameters;

            input.AssessmentLevel = (RoundedDouble) 2.2;

            input.UseAssessmentLevelManualInput = useAssessmentLevelManualInput;

            var generalPipingInput = new GeneralPipingInput();

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                SemiProbabilisticPipingCalculationService.Validate(testCalculation,
                                                                   generalPipingInput,
                                                                   normativeAssessmentLevel);

                // Assert
                RoundedDouble expectedAssessmentLevel = useAssessmentLevelManualInput
                                                            ? input.AssessmentLevel
                                                            : normativeAssessmentLevel;
                AssertSubCalculatorInputs(input, generalPipingInput, expectedAssessmentLevel);
            }
        }

        #endregion

        #region Calculate

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingCalculationService.Calculate(null,
                                                                               new GeneralPipingInput(),
                                                                               RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Calculate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingCalculationService.Calculate(new TestSemiProbabilisticPipingCalculation(),
                                                                               null,
                                                                               RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void Calculate_ErrorWhileCalculating_LogMessageAndThrowsException()
        {
            // Setup
            using (new PipingSubCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                calculatorFactory.LastCreatedUpliftCalculator.ThrowExceptionOnCalculate = true;

                var exceptionThrown = false;

                // Call
                void Call()
                {
                    try
                    {
                        SemiProbabilisticPipingCalculationService.Calculate(testCalculation,
                                                                            new GeneralPipingInput(),
                                                                            AssessmentSectionTestHelper.GetTestAssessmentLevel());
                    }
                    catch (Exception)
                    {
                        exceptionThrown = true;
                    }
                }

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, tuples =>
                {
                    Tuple<string, Level, Exception>[] messages = tuples as Tuple<string, Level, Exception>[] ?? tuples.ToArray();
                    Assert.AreEqual(3, messages.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(messages[0].Item1);

                    Tuple<string, Level, Exception> tuple1 = messages[1];
                    Assert.AreEqual("Er is een onverwachte fout opgetreden tijdens het uitvoeren van de berekening.", tuple1.Item1);
                    Assert.AreEqual(Level.Error, tuple1.Item2);
                    Assert.IsInstanceOf<PipingCalculatorException>(tuple1.Item3);

                    CalculationServiceTestHelper.AssertCalculationEndMessage(messages[2].Item1);
                });

                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(testCalculation.Output);
            }
        }

        [Test]
        public void Calculate_ValidPipingCalculation_LogsStartAndEndOfCalculation()
        {
            // Call
            void Call()
            {
                using (new PipingSubCalculatorFactoryConfig())
                {
                    SemiProbabilisticPipingCalculationService.Calculate(testCalculation,
                                                                        new GeneralPipingInput(),
                                                                        AssessmentSectionTestHelper.GetTestAssessmentLevel());
                }
            }

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[1]);
            });
        }

        [Test]
        public void Calculate_ValidPipingCalculationNoOutput_ShouldSetOutput()
        {
            // Setup
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();

            // Precondition
            Assert.IsNull(testCalculation.Output);

            // Call
            using (new PipingSubCalculatorFactoryConfig())
            {
                SemiProbabilisticPipingCalculationService.Calculate(testCalculation,
                                                                    new GeneralPipingInput(),
                                                                    normativeAssessmentLevel);
            }

            // Assert
            SemiProbabilisticPipingOutput pipingOutput = testCalculation.Output;
            Assert.IsNotNull(pipingOutput);
            Assert.IsFalse(double.IsNaN(pipingOutput.UpliftEffectiveStress));
            Assert.IsFalse(double.IsNaN(pipingOutput.UpliftFactorOfSafety));
            Assert.IsFalse(double.IsNaN(pipingOutput.HeaveFactorOfSafety));
            Assert.IsFalse(double.IsNaN(pipingOutput.SellmeijerFactorOfSafety));
        }

        [Test]
        public void Calculate_ValidPipingCalculationWithOutput_ShouldChangeOutput()
        {
            // Setup
            SemiProbabilisticPipingOutput output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();

            testCalculation.Output = output;

            // Call
            using (new PipingSubCalculatorFactoryConfig())
            {
                SemiProbabilisticPipingCalculationService.Calculate(testCalculation,
                                                                    new GeneralPipingInput(),
                                                                    normativeAssessmentLevel);
            }

            // Assert
            Assert.AreNotSame(output, testCalculation.Output);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_CompleteInput_InputSetOnSubCalculators(bool useAssessmentLevelManualInput)
        {
            // Setup
            RoundedDouble normativeAssessmentLevel = AssessmentSectionTestHelper.GetTestAssessmentLevel();
            SemiProbabilisticPipingInput input = testCalculation.InputParameters;

            input.UseAssessmentLevelManualInput = useAssessmentLevelManualInput;

            var generalPipingInput = new GeneralPipingInput();

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                SemiProbabilisticPipingCalculationService.Calculate(testCalculation,
                                                                    generalPipingInput,
                                                                    normativeAssessmentLevel);

                // Assert
                RoundedDouble expectedAssessmentLevel = useAssessmentLevelManualInput
                                                            ? input.AssessmentLevel
                                                            : normativeAssessmentLevel;
                AssertSubCalculatorInputs(input, generalPipingInput, expectedAssessmentLevel);
            }
        }

        #endregion
    }
}
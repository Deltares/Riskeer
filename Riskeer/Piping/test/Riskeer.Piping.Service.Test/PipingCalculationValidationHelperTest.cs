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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationValidationHelperTest
    {
        private PipingCalculation<PipingInput> calculation;
        private double testSurfaceLineTopLevel;

        [SetUp]
        public void SetUp()
        {
            calculation = SemiProbabilisticPipingCalculationScenarioTestFactory.CreatePipingCalculationWithValidInput(
                new TestHydraulicBoundaryLocation());
            testSurfaceLineTopLevel = calculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
        }

        [Test]
        public void ValidateInput_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationValidationHelper.ValidateInput(null, new GeneralPipingInput());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void ValidateInput_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationValidationHelper.ValidateInput(new TestPipingInput(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void ValidateInput_InvalidCalculationInput_ReturnsErrorMessages()
        {
            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.ValidateInput(
                new TestPipingInput(), new GeneralPipingInput());

            // Assert
            Assert.AreEqual(4, messages.Count());
            Assert.AreEqual("Er is geen profielschematisatie geselecteerd.", messages.ElementAt(0));
            Assert.AreEqual("Er is geen ondergrondschematisatie geselecteerd.", messages.ElementAt(1));
            Assert.AreEqual("De waarde voor 'uittredepunt' moet een concreet getal zijn.", messages.ElementAt(2));
            Assert.AreEqual("De waarde voor 'intredepunt' moet een concreet getal zijn.", messages.ElementAt(3));
        }

        [Test]
        public void ValidateInput_WithoutAquiferLayer_ReturnsErrorMessages()
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.ValidateInput(calculation.InputParameters,
                                                                                           new GeneralPipingInput());

            // Assert
            Assert.AreEqual(2, messages.Count());
            Assert.AreEqual("Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", messages.ElementAt(0));
            Assert.AreEqual("Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", messages.ElementAt(1));
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void ValidateInput_IncompleteDiameterD70Definition_ReturnsErrorMessages(bool meanSet, bool coefficientOfVariationSet)
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.ValidateInput(calculation.InputParameters,
                                                                                           new GeneralPipingInput());

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Kan de definitie voor het 70%-fraktiel van de korreldiameter van de watervoerende laag niet (volledig) afleiden.", messages.ElementAt(0));
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void ValidateInput_IncompletePermeabilityDefinition_ReturnsErrorMessage(bool meanSet, bool coefficientOfVariationSet)
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.ValidateInput(calculation.InputParameters,
                                                                                           new GeneralPipingInput());

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Kan de definitie voor de doorlatendheid van de watervoerende laag niet (volledig) afleiden.", messages.ElementAt(0));
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void ValidateInput_IncompleteSaturatedVolumicWeightDefinition_ReturnsErrorMessage(bool meanSet, bool deviationSet, bool shiftSet)
        {
            // Setup
            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false
            };

            incompletePipingSoilLayer.BelowPhreaticLevel = new LogNormalDistribution
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.ValidateInput(calculation.InputParameters,
                                                                                           new GeneralPipingInput());

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", messages.ElementAt(0));
        }

        [Test]
        public void ValidateInput_SaturatedCoverageLayerVolumicWeightLessThanWaterVolumicWeight_ReturnsErrorMessage()
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.ValidateInput(calculation.InputParameters,
                                                                                           new GeneralPipingInput());

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual(
                "Het verzadigd volumetrisch gewicht van de deklaag moet groter zijn dan het volumetrisch gewicht van water.",
                messages.ElementAt(0));
        }

        [Test]
        public void ValidateInput_SaturatedCoverageLayerLessThanWaterLayerAndMissingSaturatedParameter_ReturnsErrorMessage()
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.ValidateInput(calculation.InputParameters, new GeneralPipingInput());

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", messages.ElementAt(0));
        }
    }
}
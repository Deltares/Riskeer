// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Geometry;
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
        private IPipingCalculation<PipingInput> calculation;
        private double testSurfaceLineTopLevel;

        [SetUp]
        public void SetUp()
        {
            calculation = PipingCalculationTestFactory.CreateCalculationWithValidInput(new TestHydraulicBoundaryLocation());
            testSurfaceLineTopLevel = calculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
        }

        [Test]
        public void GetValidationWarnings_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationValidationHelper.GetValidationWarnings(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void GetValidationWarnings_WithoutSurfaceLine_ReturnsNoMessages()
        {
            // Setup
            calculation.InputParameters.SurfaceLine = null;

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters);

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void GetValidationWarnings_WithoutStochasticSoilProfile_ReturnsNoMessages()
        {
            // Setup
            calculation.InputParameters.StochasticSoilProfile = null;

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters);

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void GetValidationWarnings_WithoutExitPointL_ReturnsNoMessages()
        {
            // Setup
            calculation.InputParameters.ExitPointL = RoundedDouble.NaN;

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters);

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void GetValidationWarnings_WithoutAquitardLayer_ReturnsMessage()
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters);

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt. Er wordt een deklaagdikte gebruikt gelijk aan 0.", messages.ElementAt(0));
        }

        [Test]
        public void GetValidationWarnings_WithoutCoverageLayer_ReturnsMessage()
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters);

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt. Er wordt een deklaagdikte gebruikt gelijk aan 0.", messages.ElementAt(0));
        }

        [Test]
        public void GetValidationWarnings_MultipleCoverageLayers_ReturnsMessage()
        {
            // Setup
            var random = new Random(21);
            const double belowPhreaticLevelDeviation = 0.5;
            const int belowPhreaticLevelShift = 10;
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

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters);

            // Assert
            Assert.AreEqual(1, messages.Count());
            const string expectedMessage = "Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaardafwijkingen en verschuivingen voor alle lagen gelijk zijn.";
            Assert.AreEqual(expectedMessage, messages.ElementAt(0));
        }

        [Test]
        public void GetValidationWarnings_MultipleAquiferLayers_ReturnsMessage()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.3),
                new Point3D(1.0, 0, 3.3)
            });

            var profile = new PipingSoilProfile(string.Empty, 0, new[]
            {
                new PipingSoilLayer(4.3)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(3.3)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            }, SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters);

            // Assert
            Assert.AreEqual(1, messages.Count());
            const string expectedMessage = "Meerdere aaneengesloten watervoerende lagen gevonden. Er wordt geprobeerd de d70 en doorlatendheid van de bovenste watervoerende laag af te leiden.";
            Assert.AreEqual(expectedMessage, messages.ElementAt(0));
        }

        [Test]
        public void GetValidationErrors_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationValidationHelper.GetValidationErrors(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void GetValidationErrors_InvalidCalculationInput_ReturnsMessages()
        {
            // Call
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationErrors(new TestPipingInput());

            // Assert
            Assert.AreEqual(4, messages.Count());
            Assert.AreEqual("Er is geen profielschematisatie geselecteerd.", messages.ElementAt(0));
            Assert.AreEqual("Er is geen ondergrondschematisatie geselecteerd.", messages.ElementAt(1));
            Assert.AreEqual("De waarde voor 'uittredepunt' moet een concreet getal zijn.", messages.ElementAt(2));
            Assert.AreEqual("De waarde voor 'intredepunt' moet een concreet getal zijn.", messages.ElementAt(3));
        }

        [Test]
        public void GetValidationErrors_WithoutAquiferLayer_ReturnsMessages()
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
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationErrors(calculation.InputParameters);

            // Assert
            Assert.AreEqual(2, messages.Count());
            Assert.AreEqual("Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", messages.ElementAt(0));
            Assert.AreEqual("Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", messages.ElementAt(1));
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GetValidationErrors_IncompleteDiameterD70Definition_ReturnsMessage(bool meanSet, bool coefficientOfVariationSet)
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
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationErrors(calculation.InputParameters);

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Kan de definitie voor het 70%-fraktiel van de korreldiameter van de watervoerende laag niet (volledig) afleiden.", messages.ElementAt(0));
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GetValidationErrors_IncompletePermeabilityDefinition_ReturnsMessage(bool meanSet, bool coefficientOfVariationSet)
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
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationErrors(calculation.InputParameters);

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Kan de definitie voor de doorlatendheid van de watervoerende laag niet (volledig) afleiden.", messages.ElementAt(0));
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void GetValidationErrors_IncompleteSaturatedVolumicWeightDefinition_ReturnsMessage(bool meanSet, bool deviationSet, bool shiftSet)
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
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationErrors(calculation.InputParameters);

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", messages.ElementAt(0));
        }

        [Test]
        public void GetValidationErrors_SaturatedCoverageLayerLessThanWaterLayerAndMissingSaturatedParameter_ReturnsMessage()
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
            IEnumerable<string> messages = PipingCalculationValidationHelper.GetValidationErrors(calculation.InputParameters);

            // Assert
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", messages.ElementAt(0));
        }
    }
}
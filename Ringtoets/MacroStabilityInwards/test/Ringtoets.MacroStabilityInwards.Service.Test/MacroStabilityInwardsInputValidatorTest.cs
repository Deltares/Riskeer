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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsInputValidatorTest
    {
        private MacroStabilityInwardsInput input;

        [SetUp]
        public void Setup()
        {
            input = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput().InputParameters;
        }

        [Test]
        public void Validate_InputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsInputValidator.Validate(null, RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("inputParameters", exception.ParamName);
        }

        [Test]
        public void Validate_ValidInput_ReturnsEmpty()
        {
            // Call
            string[] messages = MacroStabilityInwardsInputValidator.Validate(input, GetTestNormativeAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void Validate_InvalidCalculationInput_ReturnsErrors()
        {
            // Setup
            input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input, GetTestNormativeAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Er is geen hydraulische randvoorwaardenlocatie geselecteerd.",
                "Er is geen profielschematisatie geselecteerd.",
                "Er is geen ondergrondschematisatie geselecteerd."
            }, messages);
        }

        [Test]
        public void Validate_AssessmentLevelNotCalculated_ReturnsError()
        {
            // Setup
            input.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            input.UseAssessmentLevelManualInput = false;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Kan het toetspeil niet afleiden op basis van de invoer."
            }, messages);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidManualAssessmentLevel_ReturnsError(double assessmentLevel)
        {
            // Setup
            input.UseAssessmentLevelManualInput = true;
            input.AssessmentLevel = (RoundedDouble) assessmentLevel;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "De waarde voor 'toetspeil' moet een concreet getal zijn."
            }, messages);
        }

        [Test]
        public void Validate_WithoutSurfaceLine_ReturnsError()
        {
            // Setup
            input.SurfaceLine = null;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Er is geen profielschematisatie geselecteerd."
            }, messages);
        }

        [Test]
        public void Validate_WithoutStochasticSoilProfile_ReturnsError()
        {
            // Setup
            input.StochasticSoilProfile = null;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Er is geen ondergrondschematisatie geselecteerd."
            }, messages);
        }

        [Test]
        [TestCase(0.06)]
        [TestCase(1)]
        public void Validate_SoilProfile1DTopDoesNotExceedSurfaceLineTop_ReturnsError(double differenceFromTop)
        {
            // Setup
            const double surfaceLineTop = 5.0;

            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1, new MacroStabilityInwardsSoilProfile1D("profile", 0.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(surfaceLineTop - differenceFromTop)
            }));

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            var firstCharacteristicPointLocation = new Point3D(0.1, 0.0, 0.0);
            var secondCharacteristicPointLocation = new Point3D(0.2, 0.0, 1.0);
            var thirdCharacteristicPointLocation = new Point3D(0.3, 0.0, 2.0);
            var fourthCharacteristicPointLocation = new Point3D(0.4, 0.0, 3.0);
            var fifthCharacteristicPointLocation = new Point3D(0.5, 0.0, 4.0);
            var sixthCharacteristicPointLocation = new Point3D(0.6, 0.0, surfaceLineTop);

            surfaceLine.SetGeometry(new[]
            {
                firstCharacteristicPointLocation,
                secondCharacteristicPointLocation,
                thirdCharacteristicPointLocation,
                fourthCharacteristicPointLocation,
                fifthCharacteristicPointLocation,
                sixthCharacteristicPointLocation
            });
            surfaceLine.SetSurfaceLevelOutsideAt(firstCharacteristicPointLocation);
            surfaceLine.SetDikeToeAtRiverAt(secondCharacteristicPointLocation);
            surfaceLine.SetDikeTopAtRiverAt(thirdCharacteristicPointLocation);
            surfaceLine.SetDikeTopAtPolderAt(fourthCharacteristicPointLocation);
            surfaceLine.SetDikeToeAtPolderAt(fifthCharacteristicPointLocation);
            surfaceLine.SetSurfaceLevelInsideAt(sixthCharacteristicPointLocation);

            input.StochasticSoilProfile = stochasticSoilProfile;
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "De ondergrondschematisatie moet boven de profielschematisatie liggen."
            }, messages);
        }

        [Test]
        [TestCase(0.05)]
        [TestCase(0)]
        [TestCase(-1)]
        public void Validate_SoilProfile1DTopExceedsSurfaceLineTop_ReturnsEmpty(double differenceFromTop)
        {
            // Setup
            const double surfaceLineTop = 5.0;

            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1, new MacroStabilityInwardsSoilProfile1D("profile", 0.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(surfaceLineTop - differenceFromTop)
            }));

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            var firstCharacteristicPointLocation = new Point3D(0.1, 0.0, 0.0);
            var secondCharacteristicPointLocation = new Point3D(0.2, 0.0, 1.0);
            var thirdCharacteristicPointLocation = new Point3D(0.3, 0.0, 2.0);
            var fourthCharacteristicPointLocation = new Point3D(0.4, 0.0, 3.0);
            var fifthCharacteristicPointLocation = new Point3D(0.5, 0.0, 4.0);
            var sixthCharacteristicPointLocation = new Point3D(0.6, 0.0, surfaceLineTop);

            surfaceLine.SetGeometry(new[]
            {
                firstCharacteristicPointLocation,
                secondCharacteristicPointLocation,
                thirdCharacteristicPointLocation,
                fourthCharacteristicPointLocation,
                fifthCharacteristicPointLocation,
                sixthCharacteristicPointLocation
            });
            surfaceLine.SetSurfaceLevelOutsideAt(firstCharacteristicPointLocation);
            surfaceLine.SetDikeToeAtRiverAt(secondCharacteristicPointLocation);
            surfaceLine.SetDikeTopAtRiverAt(thirdCharacteristicPointLocation);
            surfaceLine.SetDikeTopAtPolderAt(fourthCharacteristicPointLocation);
            surfaceLine.SetDikeToeAtPolderAt(fifthCharacteristicPointLocation);
            surfaceLine.SetSurfaceLevelInsideAt(sixthCharacteristicPointLocation);

            input.StochasticSoilProfile = stochasticSoilProfile;
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        [TestCaseSource(nameof(SurfacelineNotOnMacroStabilityInwardsSoilProfile2D))]
        public void Validate_SurfaceLineNotNear2DProfile_ReturnsError(MacroStabilityInwardsSoilProfile2D soilProfile)
        {
            // Setup

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0.0, 10),
                new Point3D(1, 0.0, 20),
                new Point3D(2, 0.0, 10)
            });

            input.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0,
                                                                                         soilProfile);
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "De profielschematisatie moet op de ondergrondschematisatie liggen."
            }, messages);
        }

        [Test]
        [TestCaseSource(nameof(SurfaceLineOnMacroStabilityInwardsSoilProfile2D))]
        public void Validate_SurfaceLineNear2DProfile_ReturnsEmpty(MacroStabilityInwardsSoilProfile2D soilProfile)
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 10),
                new Point3D(0.1, 0.0, 20),
                new Point3D(0.2, 0.0, 10)
            });

            input.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0,
                                                                                         soilProfile);
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void Validate_ZoneBoundaryRightSmallerThanZoneBoundaryLeft_ReturnsError()
        {
            // Setup
            input.ZoneBoundaryLeft = (RoundedDouble) 0.5;
            input.ZoneBoundaryRight = (RoundedDouble) 0.2;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Zoneringsgrens links moet kleiner zijn dan of gelijk zijn aan zoneringsgrens rechts."
            }, messages);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(0.1, 2)]
        [TestCase(-2, 0.3)]
        public void Validate_ZoneBoundariesOutsideSurfaceLine_ReturnsError(double zoneBoundaryLeft, double zoneBoundaryRight)
        {
            // Setup
            input.ZoneBoundaryLeft = (RoundedDouble) zoneBoundaryLeft;
            input.ZoneBoundaryRight = (RoundedDouble) zoneBoundaryRight;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Zoneringsgrenzen moeten op het profiel liggen (bereik [0,0, 0,5])."
            }, messages);
        }

        [Test]
        [TestCase(MacroStabilityInwardsZoningBoundariesDeterminationType.Manual, false)]
        [TestCase(MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, false)]
        [TestCase(MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, true)]
        public void Validate_ZoningBoundariesDeterminationTypeManualOrCreateZonesFalse_ReturnsEmpty(
            MacroStabilityInwardsZoningBoundariesDeterminationType determinationType,
            bool createZones)
        {
            // Setup
            input.ZoneBoundaryLeft = (RoundedDouble) 1;
            input.ZoneBoundaryRight = (RoundedDouble) 0.6;
            input.ZoningBoundariesDeterminationType = determinationType;
            input.CreateZones = createZones;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void Validate_MultipleTangentLinesWithTangentLineTopAndBottomSame_ReturnsError()
        {
            // Setup
            var random = new Random(21);
            input.TangentLineNumber = random.Next(2, 50);
            input.TangentLineZTop = (RoundedDouble) 1.5;
            input.TangentLineZBottom = (RoundedDouble) 1.5;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Het aantal tangentlijnen moet 1 zijn wanneer tangentlijn Z-boven gelijk is aan tangentlijn Z-onder."
            }, messages);
        }

        [Test]
        public void Validate_MultipleTangentLinesWithTangentLineTopAndBottomNotSame_ReturnsEmpty()
        {
            // Setup
            var random = new Random(21);
            input.TangentLineNumber = random.Next(2, 50);
            input.TangentLineZTop = (RoundedDouble) 1.5;
            input.TangentLineZBottom = (RoundedDouble) 0.2;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void Validate_SingleTangentLineWithTangentLineTopAndBottomSame_ReturnsEmpty()
        {
            // Setup
            input.TangentLineNumber = 1;
            input.TangentLineZTop = (RoundedDouble) 1.59;
            input.TangentLineZBottom = (RoundedDouble) 1.59;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        private static IEnumerable<TestCaseData> SurfacelineNotOnMacroStabilityInwardsSoilProfile2D()
        {
            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(1, -20),
                                new Point2D(2, 10),
                                new Point2D(1, -20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0, -10.0),
                                new Point2D(1, -20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("First surface line point way too high");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(1, 20),
                                new Point2D(2, 10),
                                new Point2D(1, 20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0, 100.0),
                                new Point2D(1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("First surface line point way too low");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(1, 200),
                                new Point2D(2, 100),
                                new Point2D(1, 200)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0, 10.0 - 0.05),
                                new Point2D(1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("First surface line point too high");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(1, 200),
                                new Point2D(2, 100),
                                new Point2D(1, 200)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0, 10.0 + 0.05),
                                new Point2D(1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("First surface line point too low");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(1, 20),
                                new Point2D(3, -1),
                                new Point2D(1, 20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0, 10.0),
                                new Point2D(1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("X further than x of surfaceLine, Y not within limit");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0, 10),
                                new Point2D(1, 21),
                                new Point2D(1, 19),
                                new Point2D(2, 10),
                                new Point2D(0, 10)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("Second segment is vertical and exceeds surfaceLine");
        }

        private static IEnumerable<TestCaseData> SurfaceLineOnMacroStabilityInwardsSoilProfile2D()
        {
            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.1, 20),
                                new Point2D(0.2, 10),
                                new Point2D(0.15, -10),
                                new Point2D(0.1, 20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 10),
                                new Point2D(0.1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("Irrelevant surface line point");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.1, 20),
                                new Point2D(0.2, 10),
                                new Point2D(0.1, 20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 10.0049),
                                new Point2D(0.1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("First surface line point within upper limit");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.1, 20),
                                new Point2D(0.2, 10),
                                new Point2D(0.1, 20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 10.0 - 0.0049),
                                new Point2D(0.1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("First surface line point within lower limit");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.1, 20),
                                new Point2D(0.3, 0),
                                new Point2D(0.1, 20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 10.0),
                                new Point2D(0.1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("X further than x of surfaceLine");
        }

        private static RoundedDouble GetTestNormativeAssessmentLevel()
        {
            return (RoundedDouble) 1.1;
        }
    }
}
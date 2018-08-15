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
            input = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation()).InputParameters;
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
            string[] messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                             AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void Validate_InvalidCalculationInput_ReturnsErrors()
        {
            // Setup
            input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Er is geen hydraulische randvoorwaardenlocatie geselecteerd.",
                "Er is geen profielschematisatie geselecteerd.",
                "Er is geen ondergrondschematisatie geselecteerd."
            }, messages);
        }

        [Test]
        public void Validate_NormativeAssessmentLevelNotCalculated_LogsErrorAndReturnsFalse()
        {
            // Setup
            input.UseAssessmentLevelManualInput = false;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input, RoundedDouble.NaN).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Kan de waterstand niet afleiden op basis van de invoer."
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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "De waarde voor 'waterstand' moet een concreet getal zijn."
            }, messages);
        }

        [Test]
        public void Validate_WithoutSurfaceLine_ReturnsError()
        {
            // Setup
            input.SurfaceLine = null;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "De profielschematisatie moet op de ondergrondschematisatie liggen."
            }, messages);
        }

        /// <remarks>
        /// The soil profile used in this test contains two outer layers (outer rings) and a surfaceline:
        /// <list type="bullet">
        /// <item>Soil layer one (1) is defined as shown below:</item>
        /// <item>Soil layer two (2) is defined as shown below:</item>
        /// </list>
        /// <code>
        ///                                  
        ///   20    1 -- 1       2 -------- 2
        ///         |    |       |          |
        ///         |    |       |          |
        ///   15    |    |       |          |
        ///         |    |       |          |
        ///         |    |       |          |
        ///   10    1 -- 1       2 -------- 2
        ///        0     5      10     15   20 
        /// </code>
        /// </remarks>
        [Test]
        public void Validate_SurfaceLineNear2DProfileWithLayersAndHorizontalInterruptionDefinitions_ReturnsError()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0.0, 20),
                new Point3D(20, 0.0, 20)
            });

            var soilProfile = new MacroStabilityInwardsSoilProfile2D(
                "profile",
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 10),
                        new Point2D(0, 20),
                        new Point2D(5, 20),
                        new Point2D(5, 10)
                    })),
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(10, 10),
                        new Point2D(10, 20),
                        new Point2D(20, 20),
                        new Point2D(20, 10)
                    }))
                }, new MacroStabilityInwardsPreconsolidationStress[0]);

            input.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0,
                                                                                         soilProfile);
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "De profielschematisatie moet op de ondergrondschematisatie liggen."
            }, messages);
        }

        /// <remarks>
        /// The soil profile used in this test contains one outer layer (outer ring) and a surfaceline:
        /// Soil layer (X) is defined as shown below:
        /// <code>
        ///                                  
        ///   20    X -- X       X -------- X
        ///         |    |       |          |
        ///         |    |       |          |
        ///   15    |    X ----- X          |
        ///         |                       |
        ///         |                       |
        ///   10    X --------------------- X
        ///        0     5      10     15   20 
        /// </code>
        /// </remarks>
        [Test]
        public void Validate_SurfaceLineNear2DProfileWithLayerHasVerticalHoleXCoordinateDefinition_ReturnsError()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0.0, 20),
                new Point3D(20, 0.0, 20)
            });

            var soilProfile = new MacroStabilityInwardsSoilProfile2D(
                "profile",
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 20),
                        new Point2D(5, 20),
                        new Point2D(5, 15),
                        new Point2D(10, 15),
                        new Point2D(10, 20),
                        new Point2D(20, 20),
                        new Point2D(20, 10),
                        new Point2D(0, 10)
                    }))
                }, new MacroStabilityInwardsPreconsolidationStress[0]);

            input.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0,
                                                                                         soilProfile);
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "De profielschematisatie moet op de ondergrondschematisatie liggen."
            }, messages);
        }

        /// <remarks>
        /// The soil profile used in this test contains one outer layer (outer ring) and a surfaceline:
        /// <item>Soil layer (X) is defined as shown below:</item>
        /// <code>
        ///                                           
        ///   20   X              X ---------------- X
        ///        | \           /                   |
        ///        |  \         /                    |
        ///   15   |   \       /                     |
        ///        |    \     /                      |
        ///        |     \   /                       |
        ///   10   |       X                         |
        ///        |                                 |
        ///        |                                 |
        ///    5   X ------------------------------- X
        ///        0     0.025   0.05      ...      0.2
        /// </code>
        /// </remarks>
        [Test]
        public void Validate_SurfaceLineNear2DProfileWithLayersWithSmallInflectionDefinition_ReturnsError()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0.0, 20),
                new Point3D(0.2, 0.0, 20)
            });

            var soilProfile = new MacroStabilityInwardsSoilProfile2D(
                "profile",
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 20),
                        new Point2D(0.025, 10),
                        new Point2D(0.05, 20),
                        new Point2D(0.2, 20),
                        new Point2D(0.2, 5),
                        new Point2D(0, 5)
                    }))
                }, new MacroStabilityInwardsPreconsolidationStress[0]);

            input.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0,
                                                                                         soilProfile);
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "De profielschematisatie moet op de ondergrondschematisatie liggen."
            }, messages);
        }

        /// <remarks>
        /// The soil profile used in this test contains two outer layers (outer rings) and a surfaceline:
        /// <list type="bullet">
        /// <item>Soil layer (1) is defined as shown below:
        /// <code>
        ///                                  
        ///   20    1 -- 1                   
        ///         |    |                   
        ///         |    |                   
        ///   15    |    1 ---------------- 1
        ///         |                       |
        ///         |                       |
        ///   10    1 --------------------- 1
        ///        0     5      10     15   20 
        /// </code></item>
        /// <item>Soil layer (2) is defined as shown below:
        /// <code>
        ///                                  
        ///   20                 2 -------- 2
        ///                      |          |
        ///                      |          |
        ///   17                 2 -------- 2
        ///                                  
        ///        0     5      10     15   20
        /// </code></item>
        /// </list>
        /// </remarks>
        [Test]
        public void Validate_SurfaceLineNear2DProfileWithFloatingLayerDefinitions_ReturnsError()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0.0, 20),
                new Point3D(20, 0.0, 20)
            });

            var soilProfile = new MacroStabilityInwardsSoilProfile2D(
                "profile",
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 20),
                        new Point2D(5, 20),
                        new Point2D(5, 15),
                        new Point2D(20, 15),
                        new Point2D(20, 10),
                        new Point2D(0, 10)
                    })),
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(10, 17),
                        new Point2D(10, 20),
                        new Point2D(20, 20),
                        new Point2D(20, 17)
                    }))
                }, new MacroStabilityInwardsPreconsolidationStress[0]);

            input.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0,
                                                                                         soilProfile);
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        /// <remarks>
        /// The soil profile used in this test contains two outer layers (outer rings) and a surfaceline:
        /// <list type="bullet">
        /// <item>Soil layer one (1) is defined directly on the surface line as shown below:
        /// <code>
        ///                                    
        ///   20                 1             
        ///                   /     \          
        ///                 /        \         
        ///   15          /            \       
        ///             /                \     
        ///           /                    \   
        ///   10   1 ---------------------- 1  
        ///        0     5      10     15   20 
        /// </code></item>
        /// <item>Soil layer two (2) is defined as shown below:
        /// <code>
        ///                                    
        ///   20                              
        ///                                    
        ///                                    
        ///   15         2              2      
        ///           /     \         /   \    
        ///          /       \       /     \   
        ///   10   2 -------- 2 --- 2 ----- 2 
        ///        0     5      10     15   20 
        /// </code></item>
        /// </list>
        /// </remarks>
        [Test]
        public void Validate_SurfaceLineNear2DProfileWithLayersWithTriangularXCoordinateDefinitions_ReturnsEmpty()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0.0, 10),
                new Point3D(10, 0.0, 20),
                new Point3D(20, 0.0, 10)
            });

            var soilProfile = new MacroStabilityInwardsSoilProfile2D(
                "profile",
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 10),
                        new Point2D(10, 20),
                        new Point2D(20, 10)
                    })),
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 10),
                        new Point2D(5, 15),
                        new Point2D(7.5, 10),
                        new Point2D(12.5, 10),
                        new Point2D(15, 15),
                        new Point2D(20, 10)
                    }))
                }, new MacroStabilityInwardsPreconsolidationStress[0]);

            input.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0,
                                                                                         soilProfile);
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        /// <remarks>
        /// The soil profile used in this test contains two outer layers (outer rings) and a surfaceline:
        /// <list type="bullet">
        /// <item>Soil layer one (1) is defined directly vertically below the X coordinates of the surface line and is shown below:</item>
        /// <item>Soil layer two (2) is defined as shown below:</item>
        /// </list>
        /// <code>
        ///                                  
        ///   20    1 --------------------- 1
        ///         |                       |
        ///         |                       |
        ///   15   1+2 ------------------- 1+2
        ///         |                       |
        ///         |                       |
        ///   10    2                       2
        ///             \              /
        ///              \            /
        ///    5                2            
        ///        0     5      10     15   20 
        /// </code>
        /// </remarks>
        [Test]
        public void Validate_SurfaceLineNear2DProfileWithLayersWithSquareXCoordinateDefinitions_ReturnsEmpty()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0.0, 20),
                new Point3D(20, 0.0, 20)
            });

            var soilProfile = new MacroStabilityInwardsSoilProfile2D(
                "profile",
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 20),
                        new Point2D(20, 20),
                        new Point2D(20, 15),
                        new Point2D(0, 15)
                    })),
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 15),
                        new Point2D(20, 15),
                        new Point2D(20, 10),
                        new Point2D(10, 5),
                        new Point2D(0, 10)
                    }))
                }, new MacroStabilityInwardsPreconsolidationStress[0]);

            input.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0,
                                                                                         soilProfile);
            input.SurfaceLine = surfaceLine;

            // Call
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
            IEnumerable<string> messages = MacroStabilityInwardsInputValidator.Validate(input,
                                                                                        AssessmentSectionHelper.GetTestAssessmentLevel()).ToArray();

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
                                new Point2D(1, 20),
                                new Point2D(2, 10),
                                new Point2D(1, 20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0, 10.0 - 0.06),
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
                                new Point2D(1, 20),
                                new Point2D(2, 10),
                                new Point2D(1, 20)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0, 10.0 + 0.06),
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

            yield return new TestCaseData(new MacroStabilityInwardsSoilProfile2D(
                                              "profile",
                                              new[]
                                              {
                                                  new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                  {
                                                      new Point2D(0, 10.06),
                                                      new Point2D(1, 20.06),
                                                      new Point2D(2, 10.06)
                                                  })),
                                                  new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                  {
                                                      new Point2D(0, 10),
                                                      new Point2D(1, 20),
                                                      new Point2D(2, 10)
                                                  }))
                                              }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("Top soilLayer offset above surfaceline and not within limit");
        }

        private static IEnumerable<TestCaseData> SurfaceLineOnMacroStabilityInwardsSoilProfile2D()
        {
            const double withinSurfaceLineLimit = 0.05;

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
                                new Point2D(0.0, 10 + withinSurfaceLineLimit),
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
                                new Point2D(0.0, 10.0 - withinSurfaceLineLimit),
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

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.2, 10),
                                new Point2D(0.0, 10),
                                new Point2D(0.1, 20)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("SoilLayer X start and end point on right side of surfaceline");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.1, 20),
                                new Point2D(0.2, 10),
                                new Point2D(0.0, 10)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("SoilLayer X start and end point on left side of surfaceline");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 10.0 + withinSurfaceLineLimit),
                                new Point2D(0.1, 20.0 + withinSurfaceLineLimit),
                                new Point2D(0.2, 10.0 + withinSurfaceLineLimit)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 10),
                                new Point2D(0.1, 20),
                                new Point2D(0.2, 10)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("Top soilLayer offset above surfaceline within limit");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 9),
                                new Point2D(0.1, 19),
                                new Point2D(0.2, 9)
                            })),
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 10),
                                new Point2D(0.1, 20),
                                new Point2D(0.2, 10)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("Top soilLayer offset below surfaceline within limit");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(0.0, 10),
                                new Point2D(0.1, 20),
                                new Point2D(0.2, 10),
                                new Point2D(0.25, 10)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("Max X soilLayer larger than surfaceline");

            yield return new TestCaseData(
                    new MacroStabilityInwardsSoilProfile2D(
                        "profile",
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                            {
                                new Point2D(-0.1, 10),
                                new Point2D(0.0, 10),
                                new Point2D(0.1, 20),
                                new Point2D(0.2, 10)
                            }))
                        }, new MacroStabilityInwardsPreconsolidationStress[0]))
                .SetName("Min X soilLayer smaller than surfaceline");
        }
    }
}
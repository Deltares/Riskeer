// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Service.Test
{
    [TestFixture]
    public class WaveConditionsCalculationServiceTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_NoHydraulicBoundaryDatabase_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(null, null, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabaseFileLocation_ReturnsFalseAndLogsError()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "NonExisting.sqlite")
            };
            
            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(null, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone);

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(input, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocationDesignWaterLevel_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            };

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(input, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN, 10.0, 12.0)]
        [TestCase(1.0, double.NaN, 12.0)]
        public void Validate_NoWaterLevels_ReturnsFalseAndLogsValidationError(double lowerBoundaryRevetments, double upperBoundaryRevetments, double designWaterLevel)
        {
            // Setup
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) designWaterLevel
                },
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetments,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetments,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(input, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_ForeShoreProfileDoesNotUseBreakWaterAndHasInvalidBreakwaterHeight_ReturnsTrueAndLogsValidationMessages(double breakWaterHeight)
        {
            // Setup
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            ForeshoreProfile foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                                     new[]
                                                                     {
                                                                         new Point2D(3.3, 4.4),
                                                                         new Point2D(5.5, 6.6)
                                                                     },
                                                                     new BreakWater(BreakWaterType.Dam, breakWaterHeight),
                                                                     new ForeshoreProfile.ConstructionProperties());

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 12.0
                },
                ForeshoreProfile = foreshoreProfile,
                UseBreakWater = false,
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(input, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_ForeShoreProfileUseBreakWaterAndHasInvalidBreakWaterHeight_ReturnsFalseAndLogsValidationMessages(double breakWaterHeight)
        {
            // Setup
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            ForeshoreProfile foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                                     new[]
                                                                     {
                                                                         new Point2D(3.3, 4.4),
                                                                         new Point2D(5.5, 6.6)
                                                                     },
                                                                     new BreakWater(BreakWaterType.Dam, breakWaterHeight),
                                                                     new ForeshoreProfile.ConstructionProperties());

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 12.0
                },
                ForeshoreProfile = foreshoreProfile,
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(input, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_AllInputConditionsSatisfiedWithoutForeshoreProfile_ReturnsTrueAndLogsValidationMessages()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 12.0
                },
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(input, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_AllInputConditionsSatisfiedWithBreakWater_ReturnsTrueAndLogsValidationMessages()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            ForeshoreProfile foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                                     new[]
                                                                     {
                                                                         new Point2D(3.3, 4.4),
                                                                         new Point2D(5.5, 6.6)
                                                                     },
                                                                     new BreakWater(BreakWaterType.Dam, 10.0),
                                                                     new ForeshoreProfile.ConstructionProperties());

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 12.0
                },
                ForeshoreProfile = foreshoreProfile,
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(input, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_AllInputConditionsSatisfiedWithoutBreakWater_ReturnsTrueAndLogsValidationMessages()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            ForeshoreProfile foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                                     new[]
                                                                     {
                                                                         new Point2D(3.3, 4.4),
                                                                         new Point2D(5.5, 6.6)
                                                                     },
                                                                     null,
                                                                     new ForeshoreProfile.ConstructionProperties());

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 12.0
                },
                ForeshoreProfile = foreshoreProfile,
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            Action action = () => isValid = WaveConditionsCalculationService.Instance.Validate(input, database, name);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        [Combinatorial]
        public void Calculate_Always_StartsCalculationWithRightParameters(
            [Values(true, false)] bool useForeshore,
            [Values(true, false)] bool useBreakWater)
        {
            // Setup
            RoundedDouble waterLevel = (RoundedDouble) 4.20;
            double a = 1.0;
            double b = 0.8;
            double c = 0.4;
            int norm = 5;
            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0),
                ForeshoreProfile = CreateForeshoreProfile(),
                UseBreakWater = useBreakWater,
                UseForeshore = useForeshore
            };

            string hlcdDirectory = "C:/temp";
            string ringId = "11-1";
            string name = "test";

            using (new HydraRingCalculationServiceConfig())
            {
                var testService = (TestHydraRingCalculationService) HydraRingCalculationService.Instance;

                // Call
                WaveConditionsCalculationService.Instance.Calculate(waterLevel, a, b, c, norm, input, hlcdDirectory, ringId, name);

                // Assert
                Assert.AreEqual(hlcdDirectory, testService.HlcdDirectory);
                Assert.AreEqual(ringId, testService.RingId);
                Assert.AreEqual(HydraRingUncertaintiesType.All, testService.UncertaintiesType);
                var parsers = testService.Parsers.ToArray();
                Assert.AreEqual(1, parsers.Length);
                Assert.IsInstanceOf<WaveConditionsCalculationParser>(parsers[0]);
                var expectedInput = CreateInput(waterLevel, a, b, c, norm, input, useForeshore, useBreakWater);
                AssertInput(expectedInput, testService.HydraRingCalculationInput, useBreakWater);
            }
        }

        [Test]
        public void Calculate_CalculationOutputNull_LogError()
        {
            // Setup
            RoundedDouble waterLevel = (RoundedDouble) 4.20;
            double a = 1.0;
            double b = 0.8;
            double c = 0.4;
            int norm = 5;
            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0),
                ForeshoreProfile = CreateForeshoreProfile()
            };

            string hlcdDirectory = "C:/temp";
            string ringId = "11-1";
            string name = "test";

            using (new HydraRingCalculationServiceConfig())
            {
                // Call
                Action call = () => WaveConditionsCalculationService.Instance.Calculate(waterLevel, a, b, c, norm, input, hlcdDirectory, ringId, name);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' is niet gelukt.", name, waterLevel), msgs[0]);
                });
            }
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return new ForeshoreProfile(new Point2D(0, 0),
                                        new[]
                                        {
                                            new Point2D(2.2, 3.3),
                                            new Point2D(4.4, 5.5)
                                        },
                                        new BreakWater(BreakWaterType.Wall, 5.5),
                                        new ForeshoreProfile.ConstructionProperties());
        }

        private static WaveConditionsCosineCalculationInput CreateInput(double waterLevel, double a, double b, double c, double norm, WaveConditionsInput input, bool useForeshore, bool useBreakWater)
        {
            return new WaveConditionsCosineCalculationInput(1,
                                                            input.Orientation,
                                                            input.HydraulicBoundaryLocation.Id,
                                                            norm,
                                                            useForeshore ?
                                                                input.ForeshoreGeometry.Select(coordinate => new HydraRingForelandPoint(coordinate.X, coordinate.Y))
                                                                : new HydraRingForelandPoint[0],
                                                            useBreakWater
                                                                ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height)
                                                                : null,
                                                            waterLevel,
                                                            a,
                                                            b,
                                                            c);
        }

        private static void AssertInput(WaveConditionsCosineCalculationInput expectedInput, HydraRingCalculationInput actualInput, bool useBreakWater)
        {
            Assert.AreEqual(expectedInput.Beta, actualInput.Beta);
            if (useBreakWater)
            {
                Assert.AreEqual(expectedInput.BreakWater.Height, actualInput.BreakWater.Height);
                Assert.AreEqual(expectedInput.BreakWater.Type, actualInput.BreakWater.Type);
            }
            else
            {
                Assert.IsNull(actualInput.BreakWater);
            }

            var expectedForelandPoints = expectedInput.ForelandsPoints.ToArray();
            var actualForelandPoints = actualInput.ForelandsPoints.ToArray();
            Assert.AreEqual(expectedForelandPoints.Length, actualForelandPoints.Length);

            for (int i = 0; i < expectedForelandPoints.Length; i++)
            {
                Assert.AreEqual(expectedForelandPoints[i].X, actualForelandPoints[i].X);
                Assert.AreEqual(expectedForelandPoints[i].Z, actualForelandPoints[i].Z);
            }

            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal);

            HydraRingVariableAssert.AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
        }
    }
}
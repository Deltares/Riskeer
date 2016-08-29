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

using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputTest
    {
        private static IEnumerable<TestCaseData> StepSizeDataSource
        {
            get
            {
                yield return new TestCaseData(5.88, 3.58, 3.40, 6, 0.5, new[]
                {
                    new RoundedDouble(2, 3.58),
                    new RoundedDouble(2, 4),
                    new RoundedDouble(2, 4.5),
                    new RoundedDouble(2, 5),
                    new RoundedDouble(2, 5.5),
                    new RoundedDouble(2, 5.87)
                });

                yield return new TestCaseData(6.10, -1.20, -1.30, 6.01, 1, new[]
                {
                    new RoundedDouble(2, -1.20),
                    new RoundedDouble(2, -1),
                    new RoundedDouble(2, 0),
                    new RoundedDouble(2, 1),
                    new RoundedDouble(2, 2),
                    new RoundedDouble(2, 3),
                    new RoundedDouble(2, 4),
                    new RoundedDouble(2, 5),
                    new RoundedDouble(2, 6),
                    new RoundedDouble(2, 6.01)
                });
            }
        }
        
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new WaveConditionsInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsNull(input.DikeProfile);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            Assert.AreEqual(new RoundedDouble(2), input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(new RoundedDouble(2), input.UpperRevetmentLevel);
            Assert.AreEqual(new RoundedDouble(2), input.LowerRevetmentLevel);
            Assert.AreEqual(new RoundedDouble(2), input.LowerWaterLevel);
            Assert.AreEqual(new RoundedDouble(2), input.UpperWaterLevel);
            Assert.AreEqual(new RoundedDouble(1), input.StepSize);
            CollectionAssert.IsEmpty(input.WaterLevels);
        }

        [Test]
        [Combinatorial]
        public void DikeProfile_SetNewValue_InputSyncedAccordingly(
            [Values(true, false)] bool withBreakWater,
            [Values(true, false)] bool withValidForeshore)
        {
            // Setup
            var input = new WaveConditionsInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreShoreGeometry = new List<Point2D>
            {
                new Point2D(2.2, 3.3)
            };

            if (withValidForeshore)
            {
                foreShoreGeometry.Add(new Point2D(4.4, 5.5));
            }

            BreakWater breakWater = null;
            if (withBreakWater)
            {
                var nonDefaultBreakWaterType = BreakWaterType.Wall;
                var nonDefaultBreakWaterHeight = 5.5;

                // Precondition
                Assert.AreNotEqual(nonDefaultBreakWaterType, input.BreakWater.Type);
                Assert.AreNotEqual(nonDefaultBreakWaterHeight, input.BreakWater.Height);

                breakWater = new BreakWater(nonDefaultBreakWaterType, nonDefaultBreakWaterHeight);
            }

            var dikeProfile = new DikeProfile(new Point2D(0, 0),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(6.6, 7.7), 0.8)
                                              }, foreShoreGeometry.ToArray(), breakWater,
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Orientation = 1.1,
                                                  DikeHeight = 4.4
                                              });

            // Call
            input.DikeProfile = dikeProfile;

            // Assert
            Assert.AreSame(dikeProfile, input.DikeProfile);
            Assert.AreEqual(withBreakWater, input.UseBreakWater);
            Assert.AreEqual(withBreakWater ? dikeProfile.BreakWater.Type : originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(withBreakWater ? dikeProfile.BreakWater.Height : originalBreakWaterHeight, input.BreakWater.Height);
            Assert.AreEqual(withValidForeshore, input.UseForeshore);
            CollectionAssert.AreEqual(dikeProfile.ForeshoreGeometry, input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void DikeProfile_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new WaveConditionsInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var dikeProfile = new DikeProfile(new Point2D(0, 0),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(7.7, 8.8), 0.6)
                                              }, new[]
                                              {
                                                  new Point2D(3.3, 4.4),
                                                  new Point2D(5.5, 6.6)
                                              },
                                              new BreakWater(BreakWaterType.Caisson, 2.2),
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Orientation = 1.1,
                                                  DikeHeight = 9.9
                                              });

            input.DikeProfile = dikeProfile;

            // Precondition
            Assert.AreSame(dikeProfile, input.DikeProfile);
            Assert.IsTrue(input.UseBreakWater);
            Assert.AreNotEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreNotEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsTrue(input.UseForeshore);
            CollectionAssert.IsNotEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);

            // Call
            input.DikeProfile = null;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void UpperRevetmentLevel_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.UpperRevetmentLevel.NumberOfDecimalPlaces;

            // Call
            input.UpperRevetmentLevel = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.UpperRevetmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.UpperRevetmentLevel.Value);
        }

        [Test]
        public void LowerRevetmentLevel_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.LowerRevetmentLevel.NumberOfDecimalPlaces;

            // Call
            input.LowerRevetmentLevel = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.LowerRevetmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.LowerRevetmentLevel.Value);
        }

        [Test]
        public void LowerWaterLevel_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.LowerWaterLevel.NumberOfDecimalPlaces;

            // Call
            input.LowerWaterLevel = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.LowerWaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.LowerWaterLevel.Value);
        }

        [Test]
        public void StepSize_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.StepSize.NumberOfDecimalPlaces;

            // Call
            input.StepSize = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.StepSize.NumberOfDecimalPlaces);
            Assert.AreEqual(1.2, input.StepSize.Value);
        }

        [Test]
        public void HydraulicBoundaryLocation_SetNewValue_UpperWaterLevelUpdated()
        {
            // Setup
            var input = new WaveConditionsInput();

            var designWaterLevel = 8.19;
            var expectedWaterLevel = 8.18;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            {
                DesignWaterLevel = (RoundedDouble) designWaterLevel
            };

            // Call
            input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(expectedWaterLevel, input.UpperWaterLevel, input.UpperWaterLevel.GetAccuracy());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void HydraulicBoundaryLocation_WithoutDesignWaterLevel_UpperWaterLevelSetToDefault(bool withDesignWaterLevel)
        {
            // Setup
            var input = new WaveConditionsInput();

            if (withDesignWaterLevel)
            {
                var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = new RoundedDouble(2, 6.34)
                };

                input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

                // Precondition
                Assert.AreEqual(6.33, input.UpperWaterLevel, input.UpperWaterLevel.GetAccuracy());
            }

            var newHydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            // Call
            input.HydraulicBoundaryLocation = newHydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(new RoundedDouble(2), input.UpperWaterLevel);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void HydraulicBoundaryLocation_HydraulicBoundaryLocationNull_UpperWaterLevelSetToDefault(bool withDesignWaterLevel)
        {
            // Setup
            var input = new WaveConditionsInput();

            if (withDesignWaterLevel)
            {
                var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = new RoundedDouble(2, 6.34)
                };

                input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

                // Precondition
                Assert.AreEqual(6.33, input.UpperWaterLevel, input.UpperWaterLevel.GetAccuracy());
            }

            // Call
            input.HydraulicBoundaryLocation = null;

            // Assert
            Assert.AreEqual(new RoundedDouble(2), input.UpperWaterLevel);
        }

        [Test]
        public void StepSize_SetNewValueBoundariesNotDefined_NoWaterLevelCalculations()
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            input.StepSize = (RoundedDouble) 0.5;

            // Assert
            CollectionAssert.IsEmpty(input.WaterLevels);
        }

        [Test]
        public void StepSize_SetNewValueBoundariesEqual_NoWaterLevelCalculations()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 6.34
                },
                LowerRevetmentLevel = (RoundedDouble) 6.33,
                LowerWaterLevel = (RoundedDouble) 6.33,
                UpperRevetmentLevel = (RoundedDouble) 6.33
            };

            // Call
            input.StepSize = (RoundedDouble) 0.5;

            // Assert
            CollectionAssert.IsEmpty(input.WaterLevels);
        }

        [Test]
        [TestCaseSource("StepSizeDataSource")]
        public void StepSize_SetNewValue_WaterLevelCalculationsSyncedAccordingly(double designWaterLevel,
                                                                                 double lowerRevetmentLevel,
                                                                                 double lowerWaterLevel,
                                                                                 double upperRevetmentLevel,
                                                                                 double stepSize,
                                                                                 RoundedDouble[] expectedWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) designWaterLevel
                },
                LowerRevetmentLevel = (RoundedDouble) lowerRevetmentLevel,
                LowerWaterLevel = (RoundedDouble) lowerWaterLevel,
                UpperRevetmentLevel = (RoundedDouble) upperRevetmentLevel
            };

            // Call
            input.StepSize = (RoundedDouble) stepSize;

            // Assert
            CollectionAssert.AreEqual(expectedWaterLevels, input.WaterLevels);
        }

        [Test]
        public void UpperRevetmentLevel_SetNewValue_WaterLevelCalculationsSyncedAccordingly()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                LowerRevetmentLevel = new RoundedDouble(2, -4.29),
                LowerWaterLevel = new RoundedDouble(2, -5),
                StepSize = (RoundedDouble) 0.5,
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 4
                }
            };

            // Call
            input.UpperRevetmentLevel = (RoundedDouble) 2.20;

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new RoundedDouble(2, -4.29), 
                new RoundedDouble(2, -4), 
                new RoundedDouble(2, -3.5), 
                new RoundedDouble(2, -3), 
                new RoundedDouble(2, -2.5), 
                new RoundedDouble(2, -2), 
                new RoundedDouble(2, -1.5), 
                new RoundedDouble(2, -1),
                new RoundedDouble(2, -0.5),
                new RoundedDouble(2),
                new RoundedDouble(2, 0.5),
                new RoundedDouble(2, 1),
                new RoundedDouble(2, 1.5),
                new RoundedDouble(2, 2),
                new RoundedDouble(2, 2.20)
            }, input.WaterLevels);
        }

        [Test]
        public void UpperRevetmentLevel_SetNewValueNoStepSize_NoWaterLevelCalculations()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                LowerRevetmentLevel = new RoundedDouble(2, -4.29),
                LowerWaterLevel = new RoundedDouble(2, -5),
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble)4
                }
            };

            // Call
            input.UpperRevetmentLevel = (RoundedDouble)2.20;

            // Assert
            CollectionAssert.IsEmpty(input.WaterLevels);
        }

        [Test]
        public void LowerRevetmentLevel_SetNewValue_WaterLevelCalculationsSyncedAccordingly()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperRevetmentLevel = (RoundedDouble)5,
                LowerWaterLevel = new RoundedDouble(2, -5),
                StepSize = (RoundedDouble) 0.5,
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 4
                }
            };

            // Call
            input.LowerRevetmentLevel = (RoundedDouble) 2.20;

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new RoundedDouble(2, 2.2),
                new RoundedDouble(2, 2.5),
                new RoundedDouble(2, 3),
                new RoundedDouble(2, 3.5),
                new RoundedDouble(2, 3.99)
            }, input.WaterLevels);
        }

        [Test]
        public void LowerRevetmentLevel_SetNewValueNoStepSize_NoWaterLevelCalculations()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperRevetmentLevel = (RoundedDouble)5,
                LowerWaterLevel = new RoundedDouble(2, -5),
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble)4
                }
            };

            // Call
            input.LowerRevetmentLevel = (RoundedDouble) 1;

            // Assert
            CollectionAssert.IsEmpty(input.WaterLevels);
        }

        [Test]
        public void LowerWaterLevel_SetNewValue_WaterLevelCalculationsSyncedAccordingly()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperRevetmentLevel = (RoundedDouble) 5,
                LowerRevetmentLevel = new RoundedDouble(2, -5),
                StepSize = (RoundedDouble) 0.5,
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 4
                }
            };

            // Call
            input.LowerWaterLevel = (RoundedDouble) 2.20;

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new RoundedDouble(2, 2.2),
                new RoundedDouble(2, 2.5),
                new RoundedDouble(2, 3),
                new RoundedDouble(2, 3.5),
                new RoundedDouble(2, 3.99)
            }, input.WaterLevels);
        }

        [Test]
        public void LowerWaterLevel_SetNewValueNoStepSize_NoWaterLevelCalculations()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperRevetmentLevel = (RoundedDouble)5,
                LowerRevetmentLevel = new RoundedDouble(2, -5),
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble)4
                }
            };

            // Call
            input.LowerWaterLevel = (RoundedDouble) 1;

            // Assert
            CollectionAssert.IsEmpty(input.WaterLevels);
        }

        [Test]
        public void HydraulicBoundaryLocation_SetNewValue_WaterLevelCalculationsSyncedAccordingly()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperRevetmentLevel = (RoundedDouble) 5,
                LowerRevetmentLevel = new RoundedDouble(2, -5),
                LowerWaterLevel = (RoundedDouble) 2.20,
                StepSize = (RoundedDouble) 0.5
            };

            // Call
            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            {
                DesignWaterLevel = (RoundedDouble) 4
            };

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new RoundedDouble(2, 2.2),
                new RoundedDouble(2, 2.5),
                new RoundedDouble(2, 3),
                new RoundedDouble(2, 3.5),
                new RoundedDouble(2, 3.99)
            }, input.WaterLevels);
        }

        [Test]
        public void HydraulicBoundaryLocation_SetNewValueNoStepSize_NoWaterLevelCalculations()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperRevetmentLevel = (RoundedDouble)5,
                LowerRevetmentLevel = new RoundedDouble(2, -5),
                LowerWaterLevel = (RoundedDouble)2.20
            };

            // Call
            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            {
                DesignWaterLevel = (RoundedDouble)4
            };

            // Assert
            CollectionAssert.IsEmpty(input.WaterLevels);
        }
    }
}
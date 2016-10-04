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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class StabilityPointStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new StabilityPointStructuresInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsNull(input.HydraulicBoundaryLocation);

            Assert.IsNull(input.StabilityPointStructure);

            Assert.IsNull(input.ForeshoreProfile);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            Assert.AreEqual(new RoundedDouble(2), input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);

            Assert.AreEqual(2, input.VolumicWeightWater.NumberOfDecimalPlaces);
            AssertEqualValues(9.81, input.VolumicWeightWater);

            Assert.IsInstanceOf<NormalDistribution>(input.InsideWaterLevelFailureConstruction);
            AssertEqualValues(0.1, input.InsideWaterLevelFailureConstruction.StandardDeviation);

            Assert.IsInstanceOf<NormalDistribution>(input.InsideWaterLevel);
            AssertEqualValues(0.1, input.InsideWaterLevel.StandardDeviation);

            Assert.IsInstanceOf<LogNormalDistribution>(input.StormDuration);
            AssertEqualValues(6.0, input.StormDuration.Mean);
            AssertEqualValues(0.25, input.StormDuration.GetVariationCoefficient());
        }

        #region Hydraulic loads and data

        [Test]
        public void Properties_HydraulicBoundaryLocation_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            var location = new HydraulicBoundaryLocation(0, "test", 0, 0);

            // Call
            input.HydraulicBoundaryLocation = location;

            // Assert
            Assert.AreEqual(location, input.HydraulicBoundaryLocation);
        }

        [Test]
        [Combinatorial]
        public void ForeshoreProfile_SetNewValue_InputSyncedAccordingly(
            [Values(true, false)] bool withBreakWater,
            [Values(true, false)] bool withValidForeshore)
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreshoreGeometry = new List<Point2D>
            {
                new Point2D(2.2, 3.3)
            };

            if (withValidForeshore)
            {
                foreshoreGeometry.Add(new Point2D(4.4, 5.5));
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

            double orientation = 96;
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        foreshoreGeometry.ToArray(),
                                                        breakWater,
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = orientation
                                                        });

            // Call
            input.ForeshoreProfile = foreshoreProfile;

            // Assert
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.AreEqual(withBreakWater, input.UseBreakWater);
            Assert.AreEqual(withBreakWater ? foreshoreProfile.BreakWater.Type : originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(withBreakWater ? foreshoreProfile.BreakWater.Height : originalBreakWaterHeight, input.BreakWater.Height);
            Assert.AreEqual(withValidForeshore, input.UseForeshore);
            CollectionAssert.AreEqual(foreshoreProfile.Geometry, input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void Foreshore_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        new[]
                                                        {
                                                            new Point2D(3.3, 4.4),
                                                            new Point2D(5.5, 6.6)
                                                        },
                                                        new BreakWater(BreakWaterType.Caisson, 2.2),
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = 96
                                                        });

            input.ForeshoreProfile = foreshoreProfile;

            // Precondition
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.IsTrue(input.UseBreakWater);
            Assert.AreNotEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreNotEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsTrue(input.UseForeshore);
            CollectionAssert.IsNotEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);

            // Call
            input.ForeshoreProfile = null;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void Properties_VolumicWeightWater_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            var random = new Random(22);

            RoundedDouble volumicWeightWater = new RoundedDouble(5, random.NextDouble());

            // Call
            input.VolumicWeightWater = volumicWeightWater;

            // Assert
            Assert.AreEqual(2, input.VolumicWeightWater.NumberOfDecimalPlaces);
            AssertEqualValues(volumicWeightWater, input.VolumicWeightWater);
        }

        [Test]
        public void Properties_InsideWaterLevelFailureConstruction_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            NormalDistribution insideWaterLevelFailureConstruction = GenerateNormalDistribution();

            //Call
            input.InsideWaterLevelFailureConstruction = insideWaterLevelFailureConstruction;

            //Assert
            Assert.AreEqual(insideWaterLevelFailureConstruction.Mean, input.InsideWaterLevelFailureConstruction.Mean);
            Assert.AreEqual(insideWaterLevelFailureConstruction.StandardDeviation, input.InsideWaterLevelFailureConstruction.StandardDeviation);
        }

        [Test]
        public void Properties_InsideWaterLevel_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            NormalDistribution insideWaterLevel = GenerateNormalDistribution();

            //Call
            input.InsideWaterLevel = insideWaterLevel;

            //Assert
            Assert.AreEqual(insideWaterLevel.Mean, input.InsideWaterLevel.Mean);
            Assert.AreEqual(insideWaterLevel.StandardDeviation, input.InsideWaterLevel.StandardDeviation);
        }

        [Test]
        public void Properties_StormDuration_ExpectedValues()
        {
            // Setup
            var input = new StabilityPointStructuresInput();
            LogNormalDistribution stormDuration = GenerateLogNormalDistribution();

            RoundedDouble initialStd = input.StormDuration.StandardDeviation;

            //Call
            input.StormDuration = stormDuration;

            //Assert
            Assert.AreEqual(stormDuration.Mean, input.StormDuration.Mean);
            AssertEqualValues(initialStd, input.StormDuration.StandardDeviation);
        }

        #endregion

        #region Schematization

        [Test]
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-0.004)]
        public void Properties_StructureNormalOrientationValidValues_NewValueSet(double orientation)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            input.StructureNormalOrientation = (RoundedDouble) orientation;

            // Assert
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);
            AssertEqualValues(orientation, input.StructureNormalOrientation);
        }

        [Test]
        [TestCase(400, 360)]
        [TestCase(360.05, 360)]
        [TestCase(-0.005, 0)]
        [TestCase(-23, 0)]
        public void Properties_StructureNormalOrientationInValidValues_ValueRoundedToValidValue(double invalidValue, double validValue)
        {
            // Setup
            var input = new StabilityPointStructuresInput();

            // Call
            input.StructureNormalOrientation = (RoundedDouble) invalidValue;

            // Assert
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);
            AssertEqualValues(validValue, input.StructureNormalOrientation);
        }

        #endregion

        #region Helpers

        private void AssertEqualValues(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }

        private static LogNormalDistribution GenerateLogNormalDistribution()
        {
            var random = new Random(22);
            return new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };
        }

        private static NormalDistribution GenerateNormalDistribution()
        {
            var random = new Random(22);
            return new NormalDistribution(2)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };
        }

        #endregion
    }
}
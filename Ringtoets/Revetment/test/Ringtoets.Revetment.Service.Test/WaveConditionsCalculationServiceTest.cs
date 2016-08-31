﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
            double norm = 5;
            var input = new WaveConditionsInput
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
            double norm = 5;
            var input = new WaveConditionsInput
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
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' is niet gelukt.", name, waterLevel.ToString()), msgs[0]);
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

            HydraRingVariableAssert.AreEqual(expectedInput.Variables.ToArray(), actualInput.Variables.ToArray());
        }
    }
}
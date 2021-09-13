﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.PresentationObjects;

namespace Riskeer.Revetment.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class SelectableTargetProbabilityTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SelectableTargetProbability(null, WaveConditionsInputWaterLevelType.None, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocationCalculations", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<HydraulicBoundaryLocationCalculation> calculations = Enumerable.Empty<HydraulicBoundaryLocationCalculation>();
            var waveConditionsInputWaterLevelType = random.NextEnumValue<WaveConditionsInputWaterLevelType>();
            double targetProbability = random.NextDouble(0, 0.1);

            // Call
            var selectableTargetProbability = new SelectableTargetProbability(calculations, waveConditionsInputWaterLevelType, targetProbability);

            // Assert
            Assert.AreSame(calculations, selectableTargetProbability.HydraulicBoundaryLocationCalculations);
            Assert.AreEqual(waveConditionsInputWaterLevelType, selectableTargetProbability.WaterLevelType);
            Assert.AreEqual(targetProbability, selectableTargetProbability.TargetProbability);
        }

        [Test]
        public void ToString_Always_ReturnsExpectedString()
        {
            // Setup
            var random = new Random(21);
            double targetProbability = random.NextDouble(0, 0.1);

            var selectableTargetProbability = new SelectableTargetProbability(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                              random.NextEnumValue<WaveConditionsInputWaterLevelType>(),
                                                                              targetProbability);

            // Call
            var actualString = selectableTargetProbability.ToString();

            // Assert
            Assert.AreEqual(ProbabilityFormattingHelper.Format(targetProbability), actualString);
        }

        [TestFixture]
        private class SelectableTargetProbabilityEqualsTest : EqualsTestFixture<SelectableTargetProbability>
        {
            private static readonly IEnumerable<HydraulicBoundaryLocationCalculation> calculations = Enumerable.Empty<HydraulicBoundaryLocationCalculation>();

            [Test]
            [TestCaseSource(nameof(GetEqualTestCases))]
            public void Equals_ToOtherWithSameTargetProbabilityAndWaterLevelType_ReturnsTrue(SelectableTargetProbability selectableTargetProbability1,
                                                                                             SelectableTargetProbability selectableTargetProbability2)
            {
                // Call
                bool areEqualObjects12 = selectableTargetProbability1.Equals(selectableTargetProbability2);
                bool areEqualObjects21 = selectableTargetProbability2.Equals(selectableTargetProbability1);

                // Assert
                Assert.IsTrue(areEqualObjects12);
                Assert.IsTrue(areEqualObjects21);
            }

            protected override SelectableTargetProbability CreateObject()
            {
                return new SelectableTargetProbability(calculations, WaveConditionsInputWaterLevelType.Signaling, 0.1);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new SelectableTargetProbability(
                                                  Array.Empty<HydraulicBoundaryLocationCalculation>(),
                                                  WaveConditionsInputWaterLevelType.Signaling,
                                                  0.01));
                yield return new TestCaseData(new SelectableTargetProbability(
                                                  Array.Empty<HydraulicBoundaryLocationCalculation>(),
                                                  WaveConditionsInputWaterLevelType.LowerLimit,
                                                  0.1));
            }
            
            private static IEnumerable<TestCaseData> GetEqualTestCases()
            {
                yield return new TestCaseData(new SelectableTargetProbability(calculations, WaveConditionsInputWaterLevelType.None, 0.1),
                                              new SelectableTargetProbability(calculations, WaveConditionsInputWaterLevelType.Signaling, 0.01));
                yield return new TestCaseData(new SelectableTargetProbability(calculations, WaveConditionsInputWaterLevelType.LowerLimit, 0.1),
                                              new SelectableTargetProbability(Array.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                              WaveConditionsInputWaterLevelType.LowerLimit, 0.1));
            }
        }
    }
}
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
using Core.Common.Base.Data;
using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(5);
            var requiredProbability = new RoundedDouble(2, random.NextDouble());
            var requiredReliability = new RoundedDouble(3, random.NextDouble());
            var probability = new RoundedDouble(2, random.NextDouble());
            var reliability = new RoundedDouble(3, random.NextDouble());
            var factorOfSafety = new RoundedDouble(3, random.NextDouble());

            // Call
            var output = new GrassCoverErosionInwardsOutput(requiredProbability, requiredReliability, probability, reliability, factorOfSafety);

            // Assert
            Assert.IsNotNull(output);
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
            Assert.AreEqual(requiredReliability, output.RequiredReliability);
            Assert.AreEqual(probability, output.Probability);
            Assert.AreEqual(reliability, output.Reliability);
            Assert.AreEqual(factorOfSafety, output.FactorOfSafety);
        }

        [Test]
        public void FactorOfSafety_VariousValues_ReturnsExpectedValues()
        {
            // Setup
            var bigRoundedDouble = new RoundedDouble(15, 9.12345678901234567);
            var output = new GrassCoverErosionInwardsOutput(0.0, 0.0, 0.0, 0.0, bigRoundedDouble);

            // Call
            output.FactorOfSafety = bigRoundedDouble;

            // Assert
            var expectedRoundedDouble = new RoundedDouble(3, bigRoundedDouble);
            Assert.AreEqual(expectedRoundedDouble, output.FactorOfSafety);
        }

        [Test]
        public void Reliability_VariousValues_ReturnsExpectedValues()
        {
            // Setup
            var bigRoundedDouble = new RoundedDouble(15, 9.12345678901234567);
            var output = new GrassCoverErosionInwardsOutput(0.0, 0.0, 0.0, bigRoundedDouble, 0.0);

            // Call
            output.Reliability = bigRoundedDouble;

            // Assert
            var expectedRoundedDouble = new RoundedDouble(3, bigRoundedDouble);
            Assert.AreEqual(expectedRoundedDouble, output.Reliability);
        }

        [Test]
        public void Probability_VariousValues_ReturnsExpectedValues()
        {
            // Setup
            var bigRoundedDouble = new RoundedDouble(15, 9.12345678901234567);
            var output = new GrassCoverErosionInwardsOutput(0.0, 0.0, bigRoundedDouble, 0.0, 0.0);

            // Call
            output.Probability = bigRoundedDouble;

            // Assert
            var expectedRoundedDouble = new RoundedDouble(2, bigRoundedDouble);
            Assert.AreEqual(expectedRoundedDouble, output.Probability);
        }
    }
}
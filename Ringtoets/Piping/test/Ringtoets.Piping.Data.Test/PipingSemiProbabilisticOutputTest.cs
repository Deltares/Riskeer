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

using Core.Common.Base.Storage;

using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    public class PipingSemiProbabilisticOutputTest
    {
        [Test]
        public void Constructor_DefaultPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Assert
            Assert.IsInstanceOf<IStorable>(output);

            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety, output.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(upliftReliability, output.UpliftReliability, output.UpliftReliability.GetAccuracy());
            Assert.AreEqual(upliftProbability, output.UpliftProbability, output.UpliftProbability.GetAccuracy());
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety, output.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(heaveReliability, output.HeaveReliability, output.HeaveReliability.GetAccuracy());
            Assert.AreEqual(heaveProbability, output.HeaveProbability, output.HeaveProbability.GetAccuracy());
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(sellmeijerReliability, output.SellmeijerReliability, output.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(sellmeijerProbability, output.SellmeijerProbability, output.SellmeijerProbability.GetAccuracy());
            Assert.AreEqual(requiredProbability, output.RequiredProbability, output.RequiredProbability.GetAccuracy());
            Assert.AreEqual(requiredReliability, output.RequiredReliability, output.RequiredReliability.GetAccuracy());
            Assert.AreEqual(pipingProbability, output.PipingProbability, output.PipingProbability.GetAccuracy());
            Assert.AreEqual(pipingReliability, output.PipingReliability, output.PipingReliability.GetAccuracy());
            Assert.AreEqual(pipingFactorOfSafety, output.PipingFactorOfSafety, output.PipingFactorOfSafety.GetAccuracy());

            Assert.AreEqual(0, output.StorageId);
        }
    }
}
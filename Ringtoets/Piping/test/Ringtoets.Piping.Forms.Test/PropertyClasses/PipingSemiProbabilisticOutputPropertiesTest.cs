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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingSemiProbabilisticOutputPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingSemiProbabilisticOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingSemiProbabilisticOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(22);
            double upliftFactorOfSafety = random.NextDouble();
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
            var semiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
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

            var properties = new PipingSemiProbabilisticOutputProperties
            {
                Data = semiProbabilisticOutput
            };

            // Call & Assert
            var probabilityFormat = "1/{0:n0}";
            Assert.AreEqual(upliftFactorOfSafety, properties.UpliftFactorOfSafety, properties.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0 / upliftProbability), properties.UpliftProbability);
            Assert.AreEqual(heaveFactorOfSafety, properties.HeaveFactorOfSafety, properties.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(heaveReliability, properties.HeaveReliability, properties.HeaveReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0 / heaveProbability), properties.HeaveProbability);
            Assert.AreEqual(sellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(sellmeijerReliability, properties.SellmeijerReliability, properties.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0 / sellmeijerProbability), properties.SellmeijerProbability);
            Assert.AreEqual(string.Format(probabilityFormat, 1.0 / requiredProbability), properties.RequiredProbability);
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0 / pipingProbability), properties.PipingProbability);
            Assert.AreEqual(pipingReliability, properties.PipingReliability, properties.PipingReliability.GetAccuracy());
            Assert.AreEqual(pipingFactorOfSafety, properties.PipingFactorOfSafety, properties.PipingFactorOfSafety.GetAccuracy());
        }

        [Test]
        public void GetProperties_WithZeroValues_ReturnTranslatedFormat()
        {
            // Setup
            var random = new Random(22);
            double upliftFactorOfSafety = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            double upliftProbability = 0;
            double heaveProbability = 0;
            double sellmeijerProbability = 0;
            double requiredProbability = 0;
            double pipingProbability = 0;

            // Call
            var semiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
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

            var properties = new PipingSemiProbabilisticOutputProperties
            {
                Data = semiProbabilisticOutput
            };

            // Call & Assert
            var probability = "1/Oneindig";
            Assert.AreEqual(probability, properties.UpliftProbability);
            Assert.AreEqual(probability, properties.HeaveProbability);
            Assert.AreEqual(probability, properties.SellmeijerProbability);
            Assert.AreEqual(probability, properties.RequiredProbability);
            Assert.AreEqual(probability, properties.PipingProbability);
        }
    }
}
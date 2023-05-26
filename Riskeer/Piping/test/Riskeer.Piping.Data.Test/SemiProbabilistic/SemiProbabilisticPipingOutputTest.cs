﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingOutputTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperties_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SemiProbabilisticPipingOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValuesSet_PropertiesAreDefault()
        {
            // Call
            var semiProbabilisticPipingOutput = new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties());

            // Assert
            Assert.IsNaN(semiProbabilisticPipingOutput.UpliftFactorOfSafety);
            Assert.IsNaN(semiProbabilisticPipingOutput.HeaveFactorOfSafety);
            Assert.IsNaN(semiProbabilisticPipingOutput.SellmeijerFactorOfSafety);
            Assert.IsNaN(semiProbabilisticPipingOutput.UpliftEffectiveStress);
            Assert.IsNaN(semiProbabilisticPipingOutput.HeaveGradient);
            Assert.IsNaN(semiProbabilisticPipingOutput.SellmeijerCreepCoefficient);
            Assert.IsNaN(semiProbabilisticPipingOutput.SellmeijerCriticalFall);
            Assert.IsNaN(semiProbabilisticPipingOutput.SellmeijerReducedFall);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            double foSuValue = random.NextDouble();
            double foShValue = random.NextDouble();
            double foSsValue = random.NextDouble();
            double upliftEffectiveStress = random.NextDouble();
            double heaveGradient = random.NextDouble();
            double sellmeijerCreepCoefficient = random.NextDouble();
            double sellmeijerCriticalFall = random.NextDouble();
            double sellmeijerReducedFall = random.NextDouble();

            // Call
            var output = new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties
            {
                UpliftFactorOfSafety = foSuValue,
                HeaveFactorOfSafety = foShValue,
                SellmeijerFactorOfSafety = foSsValue,
                UpliftEffectiveStress = upliftEffectiveStress,
                HeaveGradient = heaveGradient,
                SellmeijerCreepCoefficient = sellmeijerCreepCoefficient,
                SellmeijerCriticalFall = sellmeijerCriticalFall,
                SellmeijerReducedFall = sellmeijerReducedFall
            });

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);

            Assert.AreEqual(foSuValue, output.UpliftFactorOfSafety);
            Assert.AreEqual(foShValue, output.HeaveFactorOfSafety);
            Assert.AreEqual(foSsValue, output.SellmeijerFactorOfSafety);

            Assert.AreEqual(2, output.UpliftEffectiveStress.NumberOfDecimalPlaces);
            Assert.AreEqual(upliftEffectiveStress, output.UpliftEffectiveStress, output.UpliftEffectiveStress.GetAccuracy());
            Assert.AreEqual(2, output.HeaveGradient.NumberOfDecimalPlaces);
            Assert.AreEqual(heaveGradient, output.HeaveGradient, output.HeaveGradient.GetAccuracy());
            Assert.AreEqual(1, output.SellmeijerCreepCoefficient.NumberOfDecimalPlaces);
            Assert.AreEqual(sellmeijerCreepCoefficient, output.SellmeijerCreepCoefficient, output.SellmeijerCreepCoefficient.GetAccuracy());
            Assert.AreEqual(2, output.SellmeijerCriticalFall.NumberOfDecimalPlaces);
            Assert.AreEqual(sellmeijerCriticalFall, output.SellmeijerCriticalFall, output.SellmeijerCriticalFall.GetAccuracy());
            Assert.AreEqual(2, output.SellmeijerReducedFall.NumberOfDecimalPlaces);
            Assert.AreEqual(sellmeijerReducedFall, output.SellmeijerReducedFall, output.SellmeijerReducedFall.GetAccuracy());
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            SemiProbabilisticPipingOutput original = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }
    }
}
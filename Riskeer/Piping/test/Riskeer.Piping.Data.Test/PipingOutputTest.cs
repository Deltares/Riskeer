// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingOutputTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingOutput(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("constructionProperties", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValuesSet_PropertiesAreDefault()
        {
            // Call
            var actual = new PipingOutput(new PipingOutput.ConstructionProperties());

            // Assert
            Assert.IsNaN(actual.UpliftZValue);
            Assert.IsNaN(actual.UpliftFactorOfSafety);
            Assert.IsNaN(actual.HeaveZValue);
            Assert.IsNaN(actual.HeaveFactorOfSafety);
            Assert.IsNaN(actual.SellmeijerZValue);
            Assert.IsNaN(actual.SellmeijerFactorOfSafety);
            Assert.IsNaN(actual.UpliftEffectiveStress);
            Assert.IsNaN(actual.HeaveGradient);
            Assert.IsNaN(actual.SellmeijerCreepCoefficient);
            Assert.IsNaN(actual.SellmeijerCriticalFall);
            Assert.IsNaN(actual.SellmeijerReducedFall);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            var random = new Random(22);
            double zuValue = random.NextDouble();
            double foSuValue = random.NextDouble();
            double zhValue = random.NextDouble();
            double foShValue = random.NextDouble();
            double zsValue = random.NextDouble();
            double foSsValue = random.NextDouble();
            double upliftEffectiveStress = random.NextDouble();
            double heaveGradient = random.NextDouble();
            double sellmeijerCreepCoefficient = random.NextDouble();
            double sellmeijerCriticalFall = random.NextDouble();
            double sellmeijerReducedFall = random.NextDouble();

            var output = new PipingOutput(new PipingOutput.ConstructionProperties
            {
                UpliftZValue = zuValue,
                UpliftFactorOfSafety = foSuValue,
                HeaveZValue = zhValue,
                HeaveFactorOfSafety = foShValue,
                SellmeijerZValue = zsValue,
                SellmeijerFactorOfSafety = foSsValue,
                UpliftEffectiveStress = upliftEffectiveStress,
                HeaveGradient = heaveGradient,
                SellmeijerCreepCoefficient = sellmeijerCreepCoefficient,
                SellmeijerCriticalFall = sellmeijerCriticalFall,
                SellmeijerReducedFall = sellmeijerReducedFall
            });

            Assert.IsInstanceOf<CloneableObservable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);
            Assert.IsInstanceOf<ICloneable>(output);

            Assert.AreEqual(zuValue, output.UpliftZValue);
            Assert.AreEqual(foSuValue, output.UpliftFactorOfSafety);
            Assert.AreEqual(zhValue, output.HeaveZValue);
            Assert.AreEqual(foShValue, output.HeaveFactorOfSafety);
            Assert.AreEqual(zsValue, output.SellmeijerZValue);
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
            PipingOutput original = PipingTestDataGenerator.GetRandomPipingOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }
    }
}
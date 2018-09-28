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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Settings
{
    [TestFixture]
    public class PreprocessorSettingTest
    {
        [Test]
        public void ParameterlessConstructor_ExpectedValues()
        {
            // Call
            var preprocessorSetting = new PreprocessorSetting();

            // Assert
            Assert.IsFalse(preprocessorSetting.RunPreprocessor);
            Assert.IsNaN(preprocessorSetting.ValueMin);
            Assert.IsNaN(preprocessorSetting.ValueMax);
            Assert.IsNull(preprocessorSetting.NumericsSetting);
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double valueMin = random.NextDouble();
            double valueMax = random.NextDouble();
            var numericsSetting = new NumericsSetting(1, 4, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 20000, 100000, 0.1, -6, 6);

            // Call
            var preprocessorSetting = new PreprocessorSetting(valueMin, valueMax, numericsSetting);

            // Assert
            Assert.IsTrue(preprocessorSetting.RunPreprocessor);
            Assert.AreEqual(valueMin, preprocessorSetting.ValueMin);
            Assert.AreEqual(valueMax, preprocessorSetting.ValueMax);
            Assert.AreSame(numericsSetting, preprocessorSetting.NumericsSetting);
        }

        [Test]
        public void ParameteredConstructor_NumericsSettingNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PreprocessorSetting(1.0, 6.0, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("numericsSetting", exception.ParamName);
        }
    }
}
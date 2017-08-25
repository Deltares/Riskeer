// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class ShearStrengthModelConverterTest
    {
        [Test]
        public void Convert_Null_ReturnsDefaultValue()
        {
            // Call
            ShearStrengthModel model = ShearStrengthModelConverter.Convert(null);

            // Assert
            Assert.AreEqual(ShearStrengthModel.None, model);
        }

        [Test]
        [TestCase(0, ShearStrengthModel.None)]
        [TestCase(1, ShearStrengthModel.SuCalculated)]
        [TestCase(2, ShearStrengthModel.CPhi)]
        [TestCase(3, ShearStrengthModel.CPhiOrSuCalculated)]
        public void Convert_ValidValues_ReturnsExpectedValues(double value, ShearStrengthModel expectedShearStrengthModel)
        {
            // Call
            ShearStrengthModel model = ShearStrengthModelConverter.Convert(value);

            // Assert
            Assert.AreEqual(expectedShearStrengthModel, model);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(3.5)]
        [TestCase(-0.5)]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Convert_InvalidNumericValues_ThrowsArgumentException(double value)
        {
            // Call
            TestDelegate call = () => ShearStrengthModelConverter.Convert(value);

            // Assert
            string expectedErrorMessage = $"Cannot convert a value of {value} to {typeof(ShearStrengthModel)}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedErrorMessage);
        }
    }
}
// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Core.Common.TestUtil
{
    [TestFixture]
    public abstract class EnumValuesTestFixture<TEnum, TInnerValue>
    {
        protected abstract IDictionary<TEnum, TInnerValue> ExpectedValueForEnumValues { get; }

        [Test]
        public void ConvertToInnerValueType_AllEnumValues_ReturnExpectedValues()
        {
            // Setup
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                // Call
                object actual = Convert.ChangeType(value, typeof(TInnerValue));

                // Assert
                if (!ExpectedValueForEnumValues.ContainsKey(value))
                {
                    Assert.Fail($"Missing test case for value '{typeof(TEnum)}.{value}'.");
                }

                Assert.AreEqual(ExpectedValueForEnumValues[value], actual,
                                $"Value for {value} incorrect.");
            }
        }
    }
}
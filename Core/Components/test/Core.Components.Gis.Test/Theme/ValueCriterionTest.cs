// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Components.Gis.Theme;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Theme
{
    [TestFixture]
    public class ValueCriterionTest
    {
        [Test]
        [TestCase("test value")]
        [TestCase("")]
        public void Constructor_ReturnsExpectedProperties(string value)
        {
            // Setup
            var random = new Random(21);
            var valueOperator = random.NextEnumValue<ValueCriterionOperator>();

            // Call
            var criteria = new ValueCriterion(valueOperator, value);

            // Assert
            Assert.AreEqual(valueOperator, criteria.ValueOperator);
            Assert.AreEqual(value, criteria.Value);
        }

        [Test]
        public void Constructor_ValueNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var valueOperator = random.NextEnumValue<ValueCriterionOperator>();

            // Call
            TestDelegate call = () => new ValueCriterion(valueOperator, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("value", exception.ParamName);
        }

        [Test]
        public void Constructor_InvalidOperator_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const ValueCriterionOperator invalidOperator = (ValueCriterionOperator) 9999;

            // Call
            TestDelegate call = () => new ValueCriterion(invalidOperator, "test");

            // Assert
            string expectedMessage = $"The value of argument 'valueOperator' ({invalidOperator}) is invalid for Enum type '{nameof(ValueCriterionOperator)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("valueOperator", parameterName);
        }
    }
}
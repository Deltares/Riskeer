﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Components.Gis.Theme.Criteria;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Themes.Criteria
{
    [TestFixture]
    public class ValueCriteriaTest
    {
        [Test]
        public void Constructor_ReturnsExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var valueOperator = random.NextEnumValue<ValueCriteriaOperator>();
            double value = random.NextDouble();

            // Call
            var criteria = new ValueCriteria(valueOperator, value);

            // Assert
            Assert.IsInstanceOf<IMapCriteria>(criteria);
            Assert.AreEqual(valueOperator, criteria.ValueOperator);
            Assert.AreEqual(value, criteria.Value);
        }

        [Test]
        public void Constructor_InvalidEqualityBasedOperator_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            const ValueCriteriaOperator invalidOperator = (ValueCriteriaOperator) 9999;
            
            // Call
            TestDelegate call = () => new ValueCriteria(invalidOperator, random.NextDouble());

            // Assert
            string expectedMessage = $"The value of argument 'valueOperator' ({invalidOperator}) is invalid for Enum type '{nameof(ValueCriteriaOperator)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("valueOperator", parameterName);
        }
    }
}
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
using Core.Components.Gis.Theme;
using NUnit.Framework;

namespace Core.Plugins.Map.TestUtil
{
    /// <summary>
    /// Test helper which can be used to assert <see cref="ValueCriterion"/>.
    /// </summary>
    public static class ValueCriterionTestHelper
    {
        /// <summary>
        /// Asserts whether the <paramref name="actualExpression"/> matches with
        /// the expected expression based on the <paramref name="expectedAttributeName"/>
        /// and <paramref name="valueCriterion"/>.
        /// </summary>
        /// <param name="expectedAttributeName">The expected attribute name.</param>
        /// <param name="valueCriterion">The <see cref="ValueCriterion"/> the expression
        /// is based on.</param>
        /// <param name="actualExpression">The actual expression to assert.</param>
        /// <exception cref="NotSupportedException">Thrown when <see cref="ValueCriterion"/>
        /// contains an invalid value for the criterion operator.</exception>
        /// <exception cref="AssertionException">Thrown when <paramref name="actualExpression"/>
        /// does not match with the expected expression.</exception>
        public static void AssertValueCriterionFormatExpression(string expectedAttributeName,
                                                                ValueCriterion valueCriterion,
                                                                string actualExpression)
        {
            string expectedExpression = GetExpectedFormatExpression(valueCriterion, expectedAttributeName);
            Assert.AreEqual(expectedExpression, actualExpression);
        }

        private static string GetExpectedFormatExpression(ValueCriterion valueCriterion, string attributeName)
        {
            string valueCriterionValue = valueCriterion.Value;
            switch (valueCriterion.ValueOperator)
            {
                case ValueCriterionOperator.EqualValue:
                    return $"{attributeName} = {valueCriterionValue}";
                case ValueCriterionOperator.UnequalValue:
                    return $"{attributeName} ≠ {valueCriterionValue}";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class DistributionPropertiesTestHelperTest
    {
        [Test]
        [TestCaseSource(nameof(DistributionTestCases))]
        public void AssertPropertiesAreReadOnly_DifferentDistributions_ExpectedAssertionsCalled(
            DistributionPropertiesBase<SimpleDistribution> distribution,
            bool expectedMeanReadOnly,
            bool expectedDeviationReadOnly,
            bool assertionShouldSucceed)
        {
            // Call
            TestDelegate assertion = () => DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(distribution, expectedMeanReadOnly, expectedDeviationReadOnly);

            // Assert
            if (assertionShouldSucceed)
            {
                Assert.DoesNotThrow(assertion);
            }
            else
            {
                Assert.Throws<AssertionException>(assertion);
            }
        }

        [Test]
        [TestCaseSource(nameof(VariationCoefficientDistributionTestCases))]
        public void AssertPropertiesAreReadOnly_DifferentVariationCoefficientDistributions_ExpectedAssertionsCalled(
            VariationCoefficientDistributionPropertiesBase<SimpleDistribution> distribution,
            bool expectedMeanReadOnly,
            bool expectedDeviationReadOnly,
            bool assertionShouldSucceed)
        {
            // Call
            TestDelegate assertion = () => DistributionPropertiesTestHelper.AssertPropertiesAreReadOnly(distribution, expectedMeanReadOnly, expectedDeviationReadOnly);

            // Assert
            if (assertionShouldSucceed)
            {
                Assert.DoesNotThrow(assertion);
            }
            else
            {
                Assert.Throws<AssertionException>(assertion);
            }
        }

        private class SimpleDistributionProperties : DistributionPropertiesBase<SimpleDistribution>
        {
            public SimpleDistributionProperties(DistributionReadOnlyProperties readOnlyProperties)
                : base(readOnlyProperties,
                       new SimpleDistribution(),
                       new SimplePropertyChangeHandler()) {}

            public override string DistributionType { get; }
        }

        private class SimpleVariationCoefficientDistributionProperties : VariationCoefficientDistributionPropertiesBase<SimpleDistribution>
        {
            public SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionReadOnlyProperties readOnlyProperties)
                : base(readOnlyProperties,
                       new SimpleDistribution(),
                       new SimplePropertyChangeHandler()) {}

            public override string DistributionType { get; }
        }

        private class SimplePropertyChangeHandler : IObservablePropertyChangeHandler
        {
            public IEnumerable<IObservable> SetPropertyValueAfterConfirmation(SetObservablePropertyValueDelegate setValue)
            {
                throw new NotImplementedException();
            }
        }

        public class SimpleDistribution : IDistribution, IVariationCoefficientDistribution
        {
            public RoundedDouble Mean { get; set; }
            public RoundedDouble StandardDeviation { get; set; }
            public RoundedDouble CoefficientOfVariation { get; set; }

            public object Clone()
            {
                throw new NotImplementedException();
            }
        }

        #region Test case data

        private static IEnumerable<TestCaseData> DistributionTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new SimpleDistributionProperties(DistributionReadOnlyProperties.All),
                        true,
                        true,
                        true)
                    .SetName("Distribution, All read-only, correct assertions.");
                yield return new TestCaseData(
                        new SimpleDistributionProperties(DistributionReadOnlyProperties.All),
                        false,
                        true,
                        false)
                    .SetName("Distribution, All read-only, incorrect assertion mean.");
                yield return new TestCaseData(
                        new SimpleDistributionProperties(DistributionReadOnlyProperties.All),
                        true,
                        false,
                        false)
                    .SetName("Distribution, All read-only, incorrect assertion deviation.");
                yield return new TestCaseData(
                        new SimpleDistributionProperties(DistributionReadOnlyProperties.Mean),
                        true,
                        false,
                        true)
                    .SetName("Distribution, Mean read-only, correct assertions.");
                yield return new TestCaseData(
                        new SimpleDistributionProperties(DistributionReadOnlyProperties.StandardDeviation),
                        false,
                        true,
                        true)
                    .SetName("Distribution, StandardDeviation read-only, correct assertions.");
                yield return new TestCaseData(
                        new SimpleDistributionProperties(DistributionReadOnlyProperties.None),
                        false,
                        false,
                        true)
                    .SetName("Distribution, None read-only, correct assertions.");
            }
        }

        private static IEnumerable<TestCaseData> VariationCoefficientDistributionTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionReadOnlyProperties.All),
                        true,
                        true,
                        true)
                    .SetName("VariationDistribution, All read-only, correct assertions.");
                yield return new TestCaseData(
                        new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionReadOnlyProperties.All),
                        false,
                        true,
                        false)
                    .SetName("VariationDistribution, All read-only, incorrect assertion mean.");
                yield return new TestCaseData(
                        new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionReadOnlyProperties.All),
                        true,
                        false,
                        false)
                    .SetName("VariationDistribution, All read-only, incorrect assertion variation.");
                yield return new TestCaseData(
                        new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionReadOnlyProperties.Mean),
                        true,
                        false,
                        true)
                    .SetName("VariationDistribution, Mean read-only, correct assertions.");
                yield return new TestCaseData(
                        new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionReadOnlyProperties.CoefficientOfVariation),
                        false,
                        true,
                        true)
                    .SetName("VariationDistribution, CoefficientOfVariation read-only, correct assertions.");
                yield return new TestCaseData(
                        new SimpleVariationCoefficientDistributionProperties(VariationCoefficientDistributionReadOnlyProperties.None),
                        false,
                        false,
                        true)
                    .SetName("VariationDistribution, None read-only, correct assertions.");
            }
        }

        #endregion
    }
}
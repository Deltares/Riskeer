﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class DistributionAssertTest
    {
        [Test]
        [TestCaseSource("IdenticalDistributionProperties")]
        public void AreDistributionPropertiesEqual_IdenticalDistributionProperties_DoesNotThrowException(IDistribution distributionOne,
                                                                                                         IDistribution distributionTwo)
        {
            // Call
            TestDelegate call = () => DistributionAssert.AreEqual(distributionOne, distributionTwo);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource("IdenticalVariationCoefficientDistributionProperties")]
        public void AreDistributionPropertiesEqual_IdenticalVariationCoefficientDistributionProperties_DoesNotThrowException(IVariationCoefficientDistribution distributionOne,
                                                                                                                             IVariationCoefficientDistribution distributionTwo)
        {
            // Call
            TestDelegate call = () => DistributionAssert.AreEqual(distributionOne, distributionTwo);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource("DifferentDistributionProperties")]
        public void AreDistributionPropertiesEqual_DifferentDistributionProperties_ThrowsAssertionException(IDistribution distributionOne,
                                                                                                            IDistribution distributionTwo)
        {
            // Call
            TestDelegate call = () => DistributionAssert.AreEqual(distributionOne, distributionTwo);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCaseSource("DifferentVariationCoefficientDistributionProperties")]
        public void AreDistributionPropertiesEqual_DifferentVariationCoefficientDistributionProperties_ThrowsAssertionException(IVariationCoefficientDistribution distributionOne,
                                                                                                                                IVariationCoefficientDistribution distributionTwo)
        {
            // Call
            TestDelegate call = () => DistributionAssert.AreEqual(distributionOne, distributionTwo);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        #region Test data

        private static IEnumerable<TestCaseData> IdenticalDistributionProperties
        {
            get
            {
                yield return new TestCaseData(
                    new NormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    },
                    new NormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    },
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    });
            }
        }

        private static IEnumerable<TestCaseData> IdenticalVariationCoefficientDistributionProperties
        {
            get
            {
                yield return new TestCaseData(
                    new VariationCoefficientNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    },
                    new VariationCoefficientNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    },
                    new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
            }
        }

        private static IEnumerable<TestCaseData> DifferentDistributionProperties
        {
            get
            {
                yield return new TestCaseData(
                    new NormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    },
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new NormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    },
                    new NormalDistribution(3)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    },
                    new LogNormalDistribution(3)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new NormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    },
                    new NormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 2,
                        StandardDeviation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    },
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 2,
                        StandardDeviation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new NormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 1
                    },
                    new NormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 1
                    },
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2
                    });
            }
        }

        private static IEnumerable<TestCaseData> DifferentVariationCoefficientDistributionProperties
        {
            get
            {
                yield return new TestCaseData(
                    new VariationCoefficientNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    },
                    new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new VariationCoefficientNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    },
                    new VariationCoefficientNormalDistribution(3)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    },
                    new VariationCoefficientLogNormalDistribution(3)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new VariationCoefficientNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    },
                    new VariationCoefficientNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    },
                    new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new VariationCoefficientNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 1
                    },
                    new VariationCoefficientNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
                yield return new TestCaseData(
                    new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 1
                    },
                    new VariationCoefficientLogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        CoefficientOfVariation = (RoundedDouble) 2
                    });
            }
        }

        #endregion
    }
}
// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        [TestCaseSource("IdenticalLogNormalDistributionProperties")]
        public void AreDistributionPropertiesEqual_IdenticalLogNormalDistributionProperties_DoesNotThrowException(LogNormalDistribution distributionOne,
                                                                                                         LogNormalDistribution distributionTwo)
        {
            // Call
            TestDelegate call = () => DistributionAssert.AreEqual(distributionOne, distributionTwo);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource("IdenticalTruncatedNormalDistributionProperties")]
        public void AreDistributionPropertiesEqual_IdenticalTruncatedNormalDistributionProperties_DoesNotThrowException(TruncatedNormalDistribution distributionOne,
                                                                                                                        TruncatedNormalDistribution distributionTwo)
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
        [TestCaseSource("DifferentLogNormalDistributionProperties")]
        public void AreDistributionPropertiesEqual_DifferentLogNormalDistributionProperties_ThrowsAssertionException(LogNormalDistribution distributionOne,
                                                                                                            LogNormalDistribution distributionTwo)
        {
            // Call
            TestDelegate call = () => DistributionAssert.AreEqual(distributionOne, distributionTwo);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCaseSource("DifferentTruncatedNormalDistributionProperties")]
        public void AreDistributionPropertiesEqual_DifferentTruncatedNormalDistributionProperties_DoesNotThrowException(TruncatedNormalDistribution distributionOne,
                                                                                                                        TruncatedNormalDistribution distributionTwo)
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
                    }).SetName("IdenticalNormalDistribution");
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
                    }).SetName("IdenticalLogNormalDistribution");
            }
        }

        private static IEnumerable<TestCaseData> IdenticalLogNormalDistributionProperties
        {
            get
            {
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
                    }).SetName("IdenticalLogNormalDistribution");
            }
        }

        private static IEnumerable<TestCaseData> IdenticalTruncatedNormalDistributionProperties
        {
            get
            {
                yield return new TestCaseData(
                    new TruncatedNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 1,
                        StandardDeviation = (RoundedDouble) 2,
                        LowerBoundary = (RoundedDouble) 3,
                        UpperBoundary = (RoundedDouble) 4
                    },
                    new TruncatedNormalDistribution(2)
                    {
                        Mean = (RoundedDouble)1,
                        StandardDeviation = (RoundedDouble)2,
                        LowerBoundary = (RoundedDouble)3,
                        UpperBoundary = (RoundedDouble)4
                    }).SetName("IdenticalTruncatedNormalDistribution");
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
                    }).SetName("IdenticalLogVariationCoefficientNormalDistribution");
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
                    }).SetName("IdenticalLogVariationCoefficientLogNormalDistribution");
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
                    }).SetName("DifferentDistributionTypes");
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
                    }).SetName("DifferentRoundingNormalDistribution");
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
                    }).SetName("DifferentRoundingLogNormalDistribution");
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
                    }).SetName("DifferentMeanNormalDistribution");
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
                    }).SetName("DifferentMeanLogNormalDistribution");
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
                    }).SetName("DifferentStandardDeviationNormalDistribution");
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
                    }).SetName("DifferentStandardDeviationLogNormalDistribution");
            }
        }

        private static IEnumerable<TestCaseData> DifferentLogNormalDistributionProperties
        {
            get
            {
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
                    }).SetName("DifferentRoundingLogNormalDistribution");
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
                    }).SetName("DifferentMeanLogNormalDistribution");
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
                    }).SetName("DifferentStandardDeviationLogNormalDistribution");
                yield return new TestCaseData(
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 4,
                        StandardDeviation = (RoundedDouble) 2,
                        Shift = (RoundedDouble) 2
                    },
                    new LogNormalDistribution(2)
                    {
                        Mean = (RoundedDouble) 4,
                        StandardDeviation = (RoundedDouble) 2,
                        Shift = (RoundedDouble) 3
                    }).SetName("DifferentShiftLogNormalDistribution");
            }
        }

        private static IEnumerable<TestCaseData> DifferentTruncatedNormalDistributionProperties
        {
            get
            {
                yield return new TestCaseData(
                     new TruncatedNormalDistribution(2)
                     {
                         Mean = (RoundedDouble)1,
                         StandardDeviation = (RoundedDouble)2,
                         LowerBoundary = (RoundedDouble)3,
                         UpperBoundary = (RoundedDouble)4
                     },
                    new TruncatedNormalDistribution(3)
                    {
                        Mean = (RoundedDouble)1,
                        StandardDeviation = (RoundedDouble)2,
                        LowerBoundary = (RoundedDouble)3,
                        UpperBoundary = (RoundedDouble)4
                    }).SetName("DifferentRoundingTruncatedNormalDistribution");
                yield return new TestCaseData(
                     new TruncatedNormalDistribution(2)
                     {
                         Mean = (RoundedDouble)1,
                         StandardDeviation = (RoundedDouble)2,
                         LowerBoundary = (RoundedDouble)3,
                         UpperBoundary = (RoundedDouble)4
                     },
                    new TruncatedNormalDistribution(2)
                    {
                        Mean = (RoundedDouble)2,
                        StandardDeviation = (RoundedDouble)2,
                        LowerBoundary = (RoundedDouble)3,
                        UpperBoundary = (RoundedDouble)4
                    }).SetName("DifferentMeanTruncatedNormalDistribution");
                yield return new TestCaseData(
                     new TruncatedNormalDistribution(2)
                     {
                         Mean = (RoundedDouble)1,
                         StandardDeviation = (RoundedDouble)2,
                         LowerBoundary = (RoundedDouble)3,
                         UpperBoundary = (RoundedDouble)4
                     },
                    new TruncatedNormalDistribution(2)
                    {
                        Mean = (RoundedDouble)1,
                        StandardDeviation = (RoundedDouble)3,
                        LowerBoundary = (RoundedDouble)3,
                        UpperBoundary = (RoundedDouble)4
                    }).SetName("DifferentStandardDeviationTruncatedNormalDistribution");
                yield return new TestCaseData(
                     new TruncatedNormalDistribution(2)
                     {
                         Mean = (RoundedDouble)1,
                         StandardDeviation = (RoundedDouble)2,
                         LowerBoundary = (RoundedDouble)3,
                         UpperBoundary = (RoundedDouble)4
                     },
                    new TruncatedNormalDistribution(2)
                    {
                        Mean = (RoundedDouble)1,
                        StandardDeviation = (RoundedDouble)2,
                        LowerBoundary = (RoundedDouble)4,
                        UpperBoundary = (RoundedDouble)4
                    }).SetName("DifferentLowerBoundaryTruncatedNormalDistribution");
                yield return new TestCaseData(
                     new TruncatedNormalDistribution(2)
                     {
                         Mean = (RoundedDouble)1,
                         StandardDeviation = (RoundedDouble)2,
                         LowerBoundary = (RoundedDouble)3,
                         UpperBoundary = (RoundedDouble)4
                     },
                    new TruncatedNormalDistribution(2)
                    {
                        Mean = (RoundedDouble)1,
                        StandardDeviation = (RoundedDouble)2,
                        LowerBoundary = (RoundedDouble)3,
                        UpperBoundary = (RoundedDouble)5
                    }).SetName("DifferentUpperBoundaryTruncatedNormalDistribution");
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
                    }).SetName("DifferentVariationCoefficientDistributionTypes");
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
                    }).SetName("DifferentRoundingVariationCoefficientNormalDistribution");
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
                    }).SetName("DifferentRoundingVariationCoefficientLogNormalDistribution");
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
                    }).SetName("DifferentMeanVariationCoefficientNormalDistribution");
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
                    }).SetName("DifferentMeanVariationCoefficientLogNormalDistribution");
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
                    }).SetName("DifferentStandardDeviationVariationCoefficientNormalDistribution");
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
                    }).SetName("DifferentStandardDeviationVariationCoefficientLogNormalDistribution");
            }
        }

        #endregion
    }
}
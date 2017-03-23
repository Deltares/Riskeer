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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Test.Writers
{
    [TestFixture]
    public class CalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            SimpleCalculationConfigurationWriter,
            TestCalculation>
    {
        [Test]
        public void WriteDistribution_WithoutDistributions_ArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WithoutDistributions_ArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var writer = new SimpleCalculationConfigurationWriter();

                    // Call
                    Assert.Throws<ArgumentNullException>(() => writer.PublicWriteDistributions(null, xmlWriter));
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);

                Assert.IsEmpty(actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteDistribution_EmptyDistributions_NothingWrittenToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_EmptyDistributions_NothingWrittenToFile));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var writer = new SimpleCalculationConfigurationWriter();

                    // Call
                    writer.PublicWriteDistributions(new Dictionary<string, IDistribution>(), xmlWriter);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);

                Assert.IsEmpty(actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteDistribution_WithDistributions_WritesEachDistributionAsElement()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WithDistributions_WritesEachDistributionAsElement));
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Common.IO,
                Path.Combine(nameof(CalculationConfigurationWriter<ICalculation>), "distributions.xml"));

            var distributions = new Dictionary<string, IDistribution>
            {
                {
                    "normal", new NormalDistribution
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    }
                },
                {
                    "lognormal", new LogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.4,
                        StandardDeviation = (RoundedDouble) 0.3
                    }
                }
            };

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var writer = new SimpleCalculationConfigurationWriter();

                    // Call
                    writer.PublicWriteDistributions(distributions, xmlWriter);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteVariationCoefficientDistribution_WithoutVariationCoefficientDistributions_ArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                nameof(WriteVariationCoefficientDistribution_WithoutVariationCoefficientDistributions_ArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var writer = new SimpleCalculationConfigurationWriter();

                    // Call
                    Assert.Throws<ArgumentNullException>(() => writer.PublicWriteDistributions(null, xmlWriter));
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);

                Assert.IsEmpty(actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteVariationCoefficientDistribution_EmptyVariationCoefficientDistributions_NothingWrittenToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                nameof(WriteVariationCoefficientDistribution_EmptyVariationCoefficientDistributions_NothingWrittenToFile));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var writer = new SimpleCalculationConfigurationWriter();

                    // Call
                    writer.PublicWriteDistributions(new Dictionary<string, IDistribution>(), xmlWriter);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);

                Assert.IsEmpty(actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteVariationCoefficientDistribution_WithVariationCoefficientDistributions_WritesEachDistributionAsElement()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                nameof(WriteVariationCoefficientDistribution_WithVariationCoefficientDistributions_WritesEachDistributionAsElement));
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Common.IO,
                Path.Combine(nameof(CalculationConfigurationWriter<ICalculation>), "variationCoefficientDistributions.xml"));

            var distributions = new Dictionary<string, IVariationCoefficientDistribution>
            {
                {
                    "normal", new VariationCoefficientNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    }
                },
                {
                    "lognormal", new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.4,
                        CoefficientOfVariation = (RoundedDouble) 0.3
                    }
                }
            };

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var writer = new SimpleCalculationConfigurationWriter();

                    // Call
                    writer.PublicWriteVariationCoefficientDistributions(distributions, xmlWriter);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteBreakWaterProperties_BreakWaterNull_NothingWrittenToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(WriteBreakWaterProperties_WithBreakWater_WritesPropertiesToFile)}.xml");

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var writer = new SimpleCalculationConfigurationWriter();

                    // Call
                    writer.PublicWriteBreakWaterProperties(null, xmlWriter);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);

                Assert.IsEmpty(actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [TestCase(BreakWaterType.Wall, 26.3, "breakWaterWall.xml")]
        [TestCase(BreakWaterType.Caisson, 1.5, "breakWaterCaisson.xml")]
        [TestCase(BreakWaterType.Dam, -55.1, "breakWaterDam.xml")]
        public void WriteBreakWaterProperties_WithBreakWater_WritesPropertiesToFile(
            BreakWaterType type,
            double height,
            string expectedContentFilePath)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(WriteBreakWaterProperties_WithBreakWater_WritesPropertiesToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Common.IO,
                Path.Combine(nameof(CalculationConfigurationWriter<ICalculation>), expectedContentFilePath));

            var breakWater = new BreakWater(type, (RoundedDouble) height);

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var writer = new SimpleCalculationConfigurationWriter();

                    // Call
                    writer.PublicWriteBreakWaterProperties(breakWater, xmlWriter);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Write_CalculationOfTypeOtherThanGiven_ThrowsCriticalFileWriteExceptionWithInnerArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetScratchPadPath("test.xml");

            try
            {
                // Call
                TestDelegate test = () => new SimpleCalculationConfigurationWriter().Write(new[]
                {
                    calculation
                }, filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(test);
                var innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual($"Cannot write calculation of type '{calculation.GetType()}' using this writer.", innerException.Message);
            }
            finally
            {
                File.Delete(filePath);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationConfigurations))]
        public void Write_DifferentCalculationAndCalculationGroupConfigurations_ValidFile(IEnumerable<ICalculationBase> configuration, string expectedFileContentsFileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Common.IO,
                Path.Combine(nameof(CalculationConfigurationWriter<ICalculation>), expectedFileContentsFileName));

            try
            {
                // Call
                new SimpleCalculationConfigurationWriter().Write(configuration, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static XmlWriter CreateXmlWriter(string filePath)
        {
            return XmlWriter.Create(filePath, new XmlWriterSettings
            {
                Indent = true,
                ConformanceLevel = ConformanceLevel.Fragment
            });
        }

        private static IEnumerable<TestCaseData> GetCalculationConfigurations()
        {
            var calculation1 = new TestCalculation("calculation1");
            var calculation2 = new TestCalculation("calculation2");

            var calculationGroup1 = new CalculationGroup("group1", false);
            var calculationGroup2 = new CalculationGroup("group2", false)
            {
                Children =
                {
                    calculation2,
                    calculationGroup1
                }
            };

            yield return new TestCaseData(
                    new[]
                    {
                        calculationGroup1
                    },
                    "singleGroup.xml")
                .SetName("Single group");
            yield return new TestCaseData(
                    new[]
                    {
                        calculation1
                    },
                    "singleCalculation.xml")
                .SetName("Single calculation");
            yield return new TestCaseData(
                    new ICalculationBase[]
                    {
                        calculationGroup1,
                        calculation1
                    },
                    "calculationGroupAndCalculation.xml")
                .SetName("Calculation group and calculation");
            yield return new TestCaseData(
                    new ICalculationBase[]
                    {
                        calculation1,
                        calculationGroup2
                    },
                    "calculationAndGroupWithNesting.xml")
                .SetName("Calculation and group with nesting");
        }
    }

    public class SimpleCalculationConfigurationWriter : CalculationConfigurationWriter<TestCalculation>
    {
        public const string CalculationElementTag = "calculation";

        public void PublicWriteDistributions(IDictionary<string, IDistribution> distributions, XmlWriter writer)
        {
            WriteDistributions(distributions, writer);
        }
        public void PublicWriteVariationCoefficientDistributions(
            IDictionary<string, IVariationCoefficientDistribution> distributions, 
            XmlWriter writer)
        {
            WriteVariationCoefficientDistributions(distributions, writer);
        }

        public void PublicWriteBreakWaterProperties(BreakWater breakWater, XmlWriter writer)
        {
            WriteBreakWaterProperties(breakWater, writer);
        }

        protected override void WriteCalculation(TestCalculation calculation, XmlWriter writer)
        {
            writer.WriteElementString(CalculationElementTag, calculation.Name);
        }
    }
}
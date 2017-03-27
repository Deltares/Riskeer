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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Test.Writers
{
    [TestFixture]
    public class StructureCalculationConfigurationXmlWriterExtensionsTest
    {
        private static readonly string testDirectory = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            nameof(StructureCalculationConfigurationXmlWriterExtensions));

        public static IEnumerable<TestCaseData> GetStructureCalculationsWithProperties()
        {
            yield return new TestCaseData(
                    new SimpleStructureCalculationConfiguration("some name"),
                    "structureCalculationWithoutParametersSet.xml")
                .SetName("Structure calculation without any of its paramters set.");

            yield return new TestCaseData(
                    CreateStructureWithAllParametersSet("some other name"),
                    "structureCalculationWithAllParametersSet.xml")
                .SetName("Structure calculation with all of its paramters set.");
        }

        [Test]
        public void Write_WithoutConfiguration_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            using (var writer = CreateXmlWriter(filePath))
            {
                // Call
                TestDelegate test = () => writer.WriteStructure(
                    (SimpleStructureCalculationConfiguration) null,
                    delegate { },
                    delegate { });

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("configuration", exception.ParamName);
            }
        }

        [Test]
        public void Write_WithoutWriteProperties_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            using (var writer = CreateXmlWriter(filePath))
            {
                // Call
                TestDelegate test = () => writer.WriteStructure(
                    new SimpleStructureCalculationConfiguration(""),
                    null,
                    delegate { });

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("writeProperties", exception.ParamName);
            }
        }

        [Test]
        public void Write_WithoutWriteStochasts_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            using (var writer = CreateXmlWriter(filePath))
            {
                // Call
                TestDelegate test = () => writer.WriteStructure(
                    new SimpleStructureCalculationConfiguration(""),
                    delegate { },
                    null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("writeStochasts", exception.ParamName);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetStructureCalculationsWithProperties))]
        public void Write_WithoutDifferentSetParameters_WritesSetStructureProperties(StructureCalculationConfiguration structureCalculation, string fileName)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Write_WithoutDifferentSetParameters_WritesSetStructureProperties));
            try
            {
                // Call
                bool writePropertiesCalled = false;
                bool writeStochastsCalled = false;

                using (var writer = CreateXmlWriter(filePath))
                {
                    writer.WriteStructure(
                        structureCalculation,
                        (configuration, xmlWriter) => writePropertiesCalled = true,
                        (configuration, xmlWriter) => writeStochastsCalled = true);
                }

                // Assert
                Assert.IsTrue(writePropertiesCalled);
                Assert.IsTrue(writeStochastsCalled);
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = GetTestFileContent(fileName);
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static SimpleStructureCalculationConfiguration CreateStructureWithAllParametersSet(string name)
        {
            return new SimpleStructureCalculationConfiguration(name)
            {
                AllowedLevelIncreaseStorage = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 1.2,
                    StandardDeviation = 3.4
                },
                CriticalOvertoppingDischarge = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 22.2,
                    VariationCoefficient = 2.1
                },
                FailureProbabilityStructureWithErosion = 2.1,
                FlowWidthAtBottomProtection = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 5.4,
                    StandardDeviation = 1.1
                },
                ForeshoreProfileName = "Voorland",
                HydraulicBoundaryLocationName = "Randvoorwaardelocatie",
                ModelFactorSuperCriticalFlow = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 322.2,
                    StandardDeviation = 91.2
                },
                StorageStructureArea = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 11.122,
                    VariationCoefficient = 32.111
                },
                StormDuration = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 21.22,
                    VariationCoefficient = 1.2
                },
                StructureNormalOrientation = 5.6,
                StructureName = "Kunstwerk",
                WaveReduction = new WaveReductionConfiguration
                {
                    BreakWaterType = ReadBreakWaterType.Caisson,
                    UseBreakWater = false,
                    BreakWaterHeight = 111111.2,
                    UseForeshoreProfile = true
                },
                WidthFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 121.3,
                    StandardDeviation = 222.1
                }
            };
        }

        private static XmlWriter CreateXmlWriter(string filePath)
        {
            return XmlWriter.Create(filePath, new XmlWriterSettings
            {
                Indent = true,
                ConformanceLevel = ConformanceLevel.Fragment
            });
        }

        private string GetTestFileContent(string testFile)
        {
            return File.ReadAllText(Path.Combine(testDirectory, testFile));
        }

        private class SimpleStructureCalculationConfiguration : StructureCalculationConfiguration
        {
            public SimpleStructureCalculationConfiguration(string name) : base(name) {}
        }
    }
}
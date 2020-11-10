﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.Piping.IO.Configurations;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationReaderTest
    {
        private static readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Piping.IO,
                                                                                      nameof(PipingCalculationConfigurationReader));

        private static IEnumerable<TestCaseData> InvalidConfigurationTypes
        {
            get
            {
                yield return new TestCaseData("invalidCalculationNoCalculationType.xml",
                                              "The element 'berekening' has incomplete content. List of possible elements expected: 'toets'.")
                    .SetName("invalidCalculationNoCalculationType");
                yield return new TestCaseData("invalidCalculationMultipleCalculationType.xml",
                                              "Element 'toets' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleCalculationType");
                yield return new TestCaseData("invalidCalculationTypeEmpty.xml",
                                              "The 'toets' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationTypeEmpty");
                yield return new TestCaseData("invalidCalculationTypeNoString.xml",
                                              "The 'toets' element is invalid - The value '1' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationTypeNoString");
                yield return new TestCaseData("invalidCalculationTypeUnknownValue.xml",
                                              "The 'toets' element is invalid - The value 'toets' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationTypeUnknownValue");
            }
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurationTypes))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            void Call() => new PipingCalculationConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
        }

        /// <summary>
        /// Test fixture base class for running <see cref="PipingCalculationConfigurationReader"/> tests
        /// that need to be run for both <see cref="PipingCalculationConfigurationType.SemiProbabilistic"/> and
        /// <see cref="PipingCalculationConfigurationType.Probabilistic"/>. 
        /// </summary>
        private abstract class PipingCalculationConfigurationReaderTestFixture
        {
            [Test]
            [TestCaseSource(typeof(PipingCalculationConfigurationReaderTestFixture), nameof(InvalidConfigurations))]
            public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException));
                SetCalculationType(Path.Combine(testDirectoryPath, fileName), filePath);

                try
                {
                    // Call
                    void Call() => new PipingCalculationConfigurationReader(filePath);

                    // Assert
                    var exception = Assert.Throws<CriticalFileReadException>(Call);
                    Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
                    StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Constructor_ValidConfiguration_ExpectedValues()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Constructor_ValidConfiguration_ExpectedValues));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml"), filePath);

                try
                {
                    // Call
                    var reader = new PipingCalculationConfigurationReader(filePath);

                    // Assert
                    Assert.IsInstanceOf<CalculationConfigurationReader<PipingCalculationConfiguration>>(reader);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithEmptyCalculation_ReturnExpectedReadPipingCalculation()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithEmptyCalculation_ReturnExpectedReadPipingCalculation));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.AreEqual("Calculation", configuration.Name);
                    Assert.AreEqual(CalculationConfigurationType, configuration.CalculationType);
                    Assert.IsNull(configuration.AssessmentLevel);
                    Assert.IsNull(configuration.HydraulicBoundaryLocationName);
                    Assert.IsNull(configuration.SurfaceLineName);
                    Assert.IsNull(configuration.EntryPointL);
                    Assert.IsNull(configuration.ExitPointL);
                    Assert.IsNull(configuration.StochasticSoilModelName);
                    Assert.IsNull(configuration.StochasticSoilProfileName);
                    Assert.IsNull(configuration.PhreaticLevelExit);
                    Assert.IsNull(configuration.DampingFactorExit);
                    Assert.IsNull(configuration.Scenario);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithCalculationContainingEmptyStochasts_ReturnExpectedReadPipingCalculation()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithCalculationContainingEmptyStochasts_ReturnExpectedReadPipingCalculation));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingEmptyStochasts.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.IsNull(configuration.PhreaticLevelExit);
                    Assert.IsNull(configuration.DampingFactorExit);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadPipingCalculation()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadPipingCalculation));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingNaNs.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.IsNaN(configuration.EntryPointL);
                    Assert.IsNaN(configuration.ExitPointL);
                    Assert.IsNaN(configuration.PhreaticLevelExit.Mean);
                    Assert.IsNaN(configuration.PhreaticLevelExit.StandardDeviation);
                    Assert.IsNaN(configuration.DampingFactorExit.Mean);
                    Assert.IsNaN(configuration.DampingFactorExit.StandardDeviation);
                    Assert.IsNaN(configuration.Scenario.Contribution);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadPipingCalculation()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadPipingCalculation));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingInfinities.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.IsNotNull(configuration.EntryPointL);
                    Assert.IsNotNull(configuration.ExitPointL);
                    Assert.IsNotNull(configuration.PhreaticLevelExit.Mean);
                    Assert.IsNotNull(configuration.PhreaticLevelExit.StandardDeviation);
                    Assert.IsNotNull(configuration.DampingFactorExit.Mean);
                    Assert.IsNotNull(configuration.DampingFactorExit.StandardDeviation);
                    Assert.IsNotNull(configuration.Scenario.Contribution);

                    Assert.IsTrue(double.IsNegativeInfinity(configuration.EntryPointL.Value));
                    Assert.IsTrue(double.IsPositiveInfinity(configuration.ExitPointL.Value));
                    Assert.IsTrue(double.IsNegativeInfinity(configuration.PhreaticLevelExit.Mean.Value));
                    Assert.IsTrue(double.IsPositiveInfinity(configuration.PhreaticLevelExit.StandardDeviation.Value));
                    Assert.IsTrue(double.IsPositiveInfinity(configuration.DampingFactorExit.Mean.Value));
                    Assert.IsTrue(double.IsPositiveInfinity(configuration.DampingFactorExit.StandardDeviation.Value));
                    Assert.IsTrue(double.IsNegativeInfinity(configuration.Scenario.Contribution.Value));
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithPartialCalculation_ReturnExpectedReadPipingCalculation()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithPartialCalculation_ReturnExpectedReadPipingCalculation));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationPartialCalculation.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.AreEqual("Calculation", configuration.Name);
                    Assert.AreEqual(CalculationConfigurationType, configuration.CalculationType);
                    Assert.IsNull(configuration.AssessmentLevel);
                    Assert.IsNull(configuration.HydraulicBoundaryLocationName);
                    Assert.IsNull(configuration.SurfaceLineName);
                    Assert.IsNull(configuration.EntryPointL);
                    Assert.AreEqual(2.2, configuration.ExitPointL);
                    Assert.IsNull(configuration.StochasticSoilModelName);
                    Assert.AreEqual("Ondergrondschematisatie", configuration.StochasticSoilProfileName);
                    Assert.AreEqual(3.3, configuration.PhreaticLevelExit.Mean);
                    Assert.AreEqual(4.4, configuration.PhreaticLevelExit.StandardDeviation);
                    Assert.IsNull(configuration.DampingFactorExit);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithMissingStochastMean_ExpectedValues()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithMissingStochastMean_ExpectedValues));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationStochastsNoMean.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.IsNull(configuration.PhreaticLevelExit.Mean);
                    Assert.AreEqual(0.1, configuration.PhreaticLevelExit.StandardDeviation);
                    Assert.IsNull(configuration.DampingFactorExit.Mean);
                    Assert.AreEqual(7.7, configuration.DampingFactorExit.StandardDeviation);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithMissingStochastStandardDeviation_ExpectedValues()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithMissingStochastStandardDeviation_ExpectedValues));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationStochastsNoStandardDeviation.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.AreEqual(0.0, configuration.PhreaticLevelExit.Mean);
                    Assert.IsNull(configuration.PhreaticLevelExit.StandardDeviation);
                    Assert.AreEqual(6.6, configuration.DampingFactorExit.Mean);
                    Assert.IsNull(configuration.DampingFactorExit.StandardDeviation);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithEmptyStochast_ExpectedValues()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithEmptyStochast_ExpectedValues));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationEmptyStochasts.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.IsNull(configuration.PhreaticLevelExit.Mean);
                    Assert.IsNull(configuration.PhreaticLevelExit.StandardDeviation);
                    Assert.IsNull(configuration.DampingFactorExit.Mean);
                    Assert.IsNull(configuration.DampingFactorExit.StandardDeviation);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Read_ValidConfigurationWithEmptyScenario_ExpectedValues()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Read_ValidConfigurationWithEmptyScenario_ExpectedValues));
                SetCalculationType(Path.Combine(testDirectoryPath, "validConfigurationEmptyScenario.xml"), filePath);

                var reader = new PipingCalculationConfigurationReader(filePath);

                try
                {
                    // Call
                    IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                    // Assert
                    var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                    Assert.IsNull(configuration.Scenario.Contribution);
                    Assert.IsNull(configuration.Scenario.IsRelevant);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            protected abstract string CalculationType { get; }

            protected abstract PipingCalculationConfigurationType CalculationConfigurationType { get; }

            protected abstract void AssertConfiguration(PipingCalculationConfiguration configuration, bool hydraulicBoundaryLocation = true);

            protected void AssertConfigurationGeneralProperties(PipingCalculationConfiguration configuration)
            {
                Assert.AreEqual(CalculationConfigurationType, configuration.CalculationType);

                Assert.AreEqual("Calculation", configuration.Name);

                Assert.AreEqual("Profielschematisatie", configuration.SurfaceLineName);
                Assert.AreEqual(2.2, configuration.EntryPointL);
                Assert.AreEqual(3.3, configuration.ExitPointL);
                Assert.AreEqual("Ondergrondmodel", configuration.StochasticSoilModelName);
                Assert.AreEqual("Ondergrondschematisatie", configuration.StochasticSoilProfileName);
                Assert.AreEqual(4.4, configuration.PhreaticLevelExit.Mean);
                Assert.AreEqual(5.5, configuration.PhreaticLevelExit.StandardDeviation);
                Assert.AreEqual(6.6, configuration.DampingFactorExit.Mean);
                Assert.AreEqual(7.7, configuration.DampingFactorExit.StandardDeviation);
                Assert.AreEqual(8.8, configuration.Scenario.Contribution);
                Assert.IsFalse(configuration.Scenario.IsRelevant);
            }

            private static IEnumerable<TestCaseData> InvalidConfigurations
            {
                get
                {
                    yield return new TestCaseData("invalidEntryPointEmpty.xml",
                                                  "The 'intredepunt' element is invalid - The value '' is invalid according to its datatype 'Double'")
                        .SetName("invalidEntryPointEmpty");
                    yield return new TestCaseData("invalidEntryPointNoDouble.xml",
                                                  "The 'intredepunt' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                        .SetName("invalidEntryPointNoDouble");
                    yield return new TestCaseData("invalidExitPointEmpty.xml",
                                                  "The 'uittredepunt' element is invalid - The value '' is invalid according to its datatype 'Double'")
                        .SetName("invalidExitPointEmpty");
                    yield return new TestCaseData("invalidExitPointNoDouble.xml",
                                                  "The 'uittredepunt' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                        .SetName("invalidExitPointNoDouble");
                    yield return new TestCaseData("invalidStochastNoName.xml",
                                                  "The required attribute 'naam' is missing.")
                        .SetName("invalidStochastNoName");
                    yield return new TestCaseData("invalidStochastUnknownName.xml",
                                                  "The 'naam' attribute is invalid - The value 'Test' is invalid according to its datatype 'nameType' - The Enumeration constraint failed.")
                        .SetName("invalidStochastUnknownName");
                    yield return new TestCaseData("invalidStochastMultipleMean.xml",
                                                  "Element 'verwachtingswaarde' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidStochastMultipleMean");
                    yield return new TestCaseData("invalidStochastMultipleStandardDeviation.xml",
                                                  "Element 'standaardafwijking' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidStochastMultipleStandardDeviation");
                    yield return new TestCaseData("invalidStochastMeanEmpty.xml",
                                                  "The 'verwachtingswaarde' element is invalid - The value '' is invalid according to its datatype 'Double'")
                        .SetName("invalidStochastMeanEmpty");
                    yield return new TestCaseData("invalidStochastMeanNoDouble.xml",
                                                  "The 'verwachtingswaarde' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                        .SetName("invalidStochastMeanNoDouble");
                    yield return new TestCaseData("invalidStochastStandardDeviationEmpty.xml",
                                                  "The 'standaardafwijking' element is invalid - The value '' is invalid according to its datatype 'Double'")
                        .SetName("invalidStochastStandardDeviationEmpty");
                    yield return new TestCaseData("invalidStochastStandardDeviationNoDouble.xml",
                                                  "The 'standaardafwijking' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                        .SetName("invalidStochastStandardDeviationNoDouble");
                    yield return new TestCaseData("invalidMultiplePhreaticLevelExitStochast.xml",
                                                  "There is a duplicate key sequence 'binnendijksewaterstand' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                        .SetName("invalidMultiplePhreaticLevelExitStochast");
                    yield return new TestCaseData("invalidMultipleDampingFactorExitStochast.xml",
                                                  "There is a duplicate key sequence 'dempingsfactor' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                        .SetName("invalidMultipleDampingFactorExitStochast");
                    yield return new TestCaseData("invalidCalculationMultipleSurfaceLine.xml",
                                                  "Element 'profielschematisatie' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidCalculationMultipleSurfaceLine");
                    yield return new TestCaseData("invalidCalculationMultipleEntryPoint.xml",
                                                  "Element 'intredepunt' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidCalculationMultipleEntryPoint");
                    yield return new TestCaseData("invalidCalculationMultipleExitPoint.xml",
                                                  "Element 'uittredepunt' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidCalculationMultipleExitPoint");
                    yield return new TestCaseData("invalidCalculationMultipleStochasticSoilModel.xml",
                                                  "Element 'ondergrondmodel' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidCalculationMultipleStochasticSoilModel");
                    yield return new TestCaseData("invalidCalculationMultipleStochasticSoilProfile.xml",
                                                  "Element 'ondergrondschematisatie' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidCalculationMultipleStochasticSoilProfile");
                    yield return new TestCaseData("invalidCalculationMultipleStochasts.xml",
                                                  "Element 'stochasten' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidCalculationMultipleStochasts");
                    yield return new TestCaseData("invalidConfigurationCalculationContainingEmptySurfaceLine.xml",
                                                  "The 'profielschematisatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                        .SetName("invalidConfigurationCalculationContainingEmptySurfaceLine");
                    yield return new TestCaseData("invalidConfigurationCalculationContainingEmptySoilModel.xml",
                                                  "The 'ondergrondmodel' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                        .SetName("invalidConfigurationCalculationContainingEmptySoilModel");
                    yield return new TestCaseData("invalidConfigurationCalculationContainingEmptySoilProfile.xml",
                                                  "The 'ondergrondschematisatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                        .SetName("invalidConfigurationCalculationContainingEmptySoilProfile");
                    yield return new TestCaseData("invalidCalculationMultipleScenario.xml",
                                                  "Element 'scenario' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidCalculationMultipleScenario");
                    yield return new TestCaseData("invalidScenarioMultipleContribution.xml",
                                                  "Element 'bijdrage' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidScenarioMultipleContribution");
                    yield return new TestCaseData("invalidScenarioContributionEmpty.xml",
                                                  "The 'bijdrage' element is invalid - The value '' is invalid according to its datatype 'Double'")
                        .SetName("invalidScenarioContributionEmpty");
                    yield return new TestCaseData("invalidScenarioContributionNoDouble.xml",
                                                  "The 'bijdrage' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                        .SetName("invalidScenarioContributionNoDouble");
                    yield return new TestCaseData("invalidScenarioMultipleRelevant.xml",
                                                  "Element 'gebruik' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidScenarioMultipleRelevant");
                    yield return new TestCaseData("invalidScenarioRelevantEmpty.xml",
                                                  "The 'gebruik' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                        .SetName("invalidScenarioRelevantEmpty");
                    yield return new TestCaseData("invalidScenarioRelevantNoBoolean.xml",
                                                  "The 'gebruik' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                        .SetName("invalidScenarioRelevantNoBoolean");

                    yield return new TestCaseData("invalidContainingBothSemiProbabilisticAndProbabilistic.xml",
                                                  "Element 'probabilistisch' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidContainingBothSemiProbabilisticAndProbabilistic");
                }
            }

            private void SetCalculationType(string readFilePath, string writeFilePath)
            {
                string text = File.ReadAllText(readFilePath);
                text = text.Replace("<toetstype/>", $"<{CalculationType}/>");
                File.WriteAllText(writeFilePath, text);
            }
        }

        /// <summary>
        /// Test Fixture for running <see cref="PipingCalculationConfigurationReader"/> tests for
        /// <see cref="PipingCalculationConfigurationType.SemiProbabilistic"/>. 
        /// </summary>
        [TestFixture]
        private class SemiProbabilisticPipingCalculationConfigurationReaderTest : PipingCalculationConfigurationReaderTestFixture
        {
            protected override string CalculationType => "semi-probabilistisch";

            protected override PipingCalculationConfigurationType CalculationConfigurationType => PipingCalculationConfigurationType.SemiProbabilistic;

            private static IEnumerable<TestCaseData> InvalidSemiProbabilisticConfigurations
            {
                get
                {
                    yield return new TestCaseData("invalidWaterLevelEmpty.xml",
                                                  "The 'waterstand' element is invalid - The value '' is invalid according to its datatype 'Double'")
                        .SetName("invalidWaterLevelEmpty");
                    yield return new TestCaseData("invalidWaterLevelNoDouble.xml",
                                                  "The 'waterstand' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                        .SetName("invalidWaterLevelNoDouble");

                    yield return new TestCaseData("invalidContainingBothWaterLevelAndHydraulicBoundaryLocation.xml",
                                                  "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidContainingBothWaterLevelAndHydraulicBoundaryLocation");
                    yield return new TestCaseData("invalidCalculationMultipleWaterLevel.xml",
                                                  "Element 'waterstand' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidCalculationMultipleWaterLevel");

                    yield return new TestCaseData("invalidSemiProbabilisticCalculationMultipleHydraulicBoundaryLocation.xml",
                                                  "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidSemiProbabilisticCalculationMultipleHydraulicBoundaryLocation");

                    yield return new TestCaseData("invalidConfigurationSemiProbabilisticCalculationContainingEmptyHydraulicBoundaryLocation.xml",
                                                  "The 'hblocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                        .SetName("invalidConfigurationSemiProbabilisticCalculationContainingEmptyHydraulicBoundaryLocation");
                }
            }

            [Test]
            [TestCaseSource(nameof(InvalidSemiProbabilisticConfigurations))]
            public void Constructor_SemiProbabilisticFileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(
                string fileName, string expectedParsingMessage)
            {
                // Setup
                string filePath = Path.Combine(testDirectoryPath, fileName);

                // Call
                void Call() => new PipingCalculationConfigurationReader(filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(Call);
                Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
                StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
            }

            [Test]
            [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation", true)]
            [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation_differentOrder", true)]
            [TestCase("validConfigurationFullCalculationContainingWaterLevel", false)]
            [TestCase("validConfigurationFullCalculationContainingWaterLevel_differentOrder", false)]
            public void Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadPipingCalculation(
                string fileName, bool hydraulicBoundaryLocation)
            {
                // Setup
                string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
                var reader = new PipingCalculationConfigurationReader(filePath);

                // Call
                IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                // Assert
                var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                AssertConfiguration(configuration, hydraulicBoundaryLocation);
            }

            [Test]
            [TestCase("version0ValidConfigurationFullCalculationContainingHydraulicBoundaryLocation", true)]
            [TestCase("version0ValidConfigurationFullCalculationContainingAssessmentLevel", false)]
            public void Read_ValidPreviousVersionConfigurationWithFullCalculation_ReturnExpectedReadPipingCalculation(
                string fileName, bool hydraulicBoundaryLocation)
            {
                // Setup
                string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
                var reader = new PipingCalculationConfigurationReader(filePath);

                // Call
                IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                // Assert
                var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                AssertConfiguration(configuration, hydraulicBoundaryLocation);
            }

            [Test]
            public void Read_ValidConfigurationWithSemiProbabilisticCalculationContainingInfinities_ReturnExpectedReadPipingCalculation()
            {
                // Setup
                string filePath = Path.Combine(testDirectoryPath, "validConfigurationSemiProbabilisticCalculationContainingInfinities.xml");

                var reader = new PipingCalculationConfigurationReader(filePath);

                // Call
                IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                // Assert
                var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                Assert.IsNotNull(configuration.AssessmentLevel);
                Assert.IsTrue(double.IsNegativeInfinity(configuration.AssessmentLevel.Value));
            }

            [Test]
            public void Read_ValidConfigurationWithSemiProbabilisticCalculationContainingNaNs_ReturnExpectedReadPipingCalculation()
            {
                // Setup
                string filePath = Path.Combine(testDirectoryPath, "validConfigurationSemiProbabilisticCalculationContainingNaNs.xml");
                var reader = new PipingCalculationConfigurationReader(filePath);

                // Call
                IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                // Assert
                var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                Assert.IsNaN(configuration.AssessmentLevel);
            }

            protected override void AssertConfiguration(PipingCalculationConfiguration configuration, bool hydraulicBoundaryLocation = true)
            {
                AssertConfigurationGeneralProperties(configuration);

                if (hydraulicBoundaryLocation)
                {
                    Assert.IsNull(configuration.AssessmentLevel);
                    Assert.AreEqual("Locatie", configuration.HydraulicBoundaryLocationName);
                }
                else
                {
                    Assert.AreEqual(1.1, configuration.AssessmentLevel);
                    Assert.IsNull(configuration.HydraulicBoundaryLocationName);
                }
            }
        }

        /// <summary>
        /// Test Fixture for running <see cref="PipingCalculationConfigurationReader"/> tests for
        /// <see cref="PipingCalculationConfigurationType.Probabilistic"/>. 
        /// </summary>
        [TestFixture]
        private class ProbabilisticPipingCalculationConfigurationReaderTest : PipingCalculationConfigurationReaderTestFixture
        {
            protected override string CalculationType => "probabilistisch";

            protected override PipingCalculationConfigurationType CalculationConfigurationType => PipingCalculationConfigurationType.Probabilistic;

            private static IEnumerable<TestCaseData> InvalidProbabilisticConfigurations
            {
                get
                {
                    yield return new TestCaseData("invalidProbabilisticCalculationMultipleHydraulicBoundaryLocation.xml",
                                                  "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                        .SetName("invalidProbabilisticCalculationMultipleHydraulicBoundaryLocation");

                    yield return new TestCaseData("invalidConfigurationProbabilisticCalculationContainingEmptyHydraulicBoundaryLocation.xml",
                                                  "The 'hblocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                        .SetName("invalidConfigurationProbabilisticCalculationContainingEmptyHydraulicBoundaryLocation");
                }
            }

            [Test]
            [TestCaseSource(nameof(InvalidProbabilisticConfigurations))]
            public void Constructor_ProbabilisticFileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(
                string fileName, string expectedParsingMessage)
            {
                // Setup
                string filePath = Path.Combine(testDirectoryPath, fileName);

                // Call
                void Call() => new PipingCalculationConfigurationReader(filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(Call);
                Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
                StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
            }

            [Test]
            [TestCase("validConfigurationFullCalculationProbabilistic")]
            [TestCase("validConfigurationFullCalculationProbabilistic_differentOrder")]
            public void Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadPipingCalculation(string fileName)
            {
                // Setup
                string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
                var reader = new PipingCalculationConfigurationReader(filePath);

                // Call
                IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

                // Assert
                var configuration = (PipingCalculationConfiguration) readConfigurationItems.Single();

                AssertConfiguration(configuration);
            }

            [Test]
            public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException()
            {
                // Setup
                string filePath = Path.Combine(testDirectoryPath, "invalidProbabilisticContainingWaterLevel.xml");

                // Call
                void Call() => new PipingCalculationConfigurationReader(filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(Call);
                Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
                StringAssert.Contains("Element 'waterstand' cannot appear more than once if content model type is \"all\".", exception.InnerException?.Message);
            }

            protected override void AssertConfiguration(PipingCalculationConfiguration configuration, bool hydraulicBoundaryLocation = true)
            {
                AssertConfigurationGeneralProperties(configuration);

                Assert.IsNull(configuration.AssessmentLevel);
                Assert.AreEqual("Locatie", configuration.HydraulicBoundaryLocationName);
            }
        }
    }
}
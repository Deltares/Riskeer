﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.IO;
using System.IO.Compression;
using System.Linq;
using Components.Persistence.Stability;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.Exporters;
using Riskeer.MacroStabilityInwards.IO.TestUtil;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;

namespace Riskeer.MacroStabilityInwards.IO.Test.Exporters
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationGroupExporterTest
    {
        private const string fileExtension = "stix";

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            var exporter = new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), new GeneralMacroStabilityInwardsInput(),
                                                                             persistenceFactory, "ValidFolderPath", "extension",
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(null, new GeneralMacroStabilityInwardsInput(), persistenceFactory,
                                                                             string.Empty, string.Empty, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), null, persistenceFactory,
                                                                             string.Empty, string.Empty, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PersistenceFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), new GeneralMacroStabilityInwardsInput(),
                                                                             null, string.Empty, string.Empty, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("persistenceFactory", exception.ParamName);
        }

        [Test]
        public void Constructor_GetAssessmentLevelFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), new GeneralMacroStabilityInwardsInput(),
                                                                             persistenceFactory, string.Empty, string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getNormativeAssessmentLevelFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_InvalidFilePath_ThrowsArgumentException(string filePath)
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), new GeneralMacroStabilityInwardsInput(), persistenceFactory,
                                                                             filePath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            Assert.Throws<ArgumentException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationGroupConfigurations))]
        public void Export_CalculationExportReturnsFalse_LogsErrorAndReturnsFalse(CalculationGroup calculationGroup)
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationExportReturnsFalse_LogsErrorAndReturnsFalse)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory
                {
                    ThrowException = true
                };

                var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, new GeneralMacroStabilityInwardsInput(),
                                                                                 persistenceFactory, filePath, fileExtension,
                                                                                 c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

                try
                {
                    // Call
                    var exportResult = true;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    ICalculation calculation = calculationGroup.GetCalculations().First();
                    var expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het exporteren van '{calculation.Name}'. Er is geen D-GEO Suite Stability Project geëxporteerd.";
                    TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                    Assert.IsFalse(exportResult);
                    Assert.IsFalse(File.Exists(filePath));
                }
                finally
                {
                    DirectoryHelper.TryDelete(folderPath);
                }
            }
        }

        [Test]
        public void Export_CalculationGroupWithOnlyCalculations_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupWithOnlyCalculations_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation1);
            calculationGroup.Children.Add(calculation2);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    AssertFilesExistInZip(new[]
                    {
                        $"{calculation1.Name}.{fileExtension}",
                        $"{calculation2.Name}.{fileExtension}"
                    }, filePath);
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_CalculationsWithoutOutput_LogWarningsAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationsWithoutOutput_LogWarningsAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1", false);
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2", false);

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation1);
            calculationGroup.Children.Add(calculation2);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, new[]
                    {
                        new Tuple<string, LogLevelConstant>($"Berekening '{calculation1.Name}' heeft geen uitvoer. Deze berekening wordt overgeslagen.", LogLevelConstant.Warn),
                        new Tuple<string, LogLevelConstant>($"Berekening '{calculation2.Name}' heeft geen uitvoer. Deze berekening wordt overgeslagen.", LogLevelConstant.Warn)
                    });
                    Assert.IsTrue(exportResult);
                    Assert.IsFalse(File.Exists(filePath));
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_CalculationGroupWithNestedGroupsAndCalculations_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupWithNestedGroupsAndCalculations_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");
            MacroStabilityInwardsCalculationScenario calculation3 = CreateCalculation("calculation3");
            MacroStabilityInwardsCalculationScenario calculation4 = CreateCalculation("calculation4");

            var rootCalculationGroup = new CalculationGroup();
            var nestedGroup1 = new CalculationGroup
            {
                Name = "NestedGroup1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "NestedGroup2"
            };
            nestedGroup2.Children.Add(calculation4);
            nestedGroup1.Children.Add(calculation3);
            nestedGroup1.Children.Add(nestedGroup2);
            rootCalculationGroup.Children.Add(calculation1);
            rootCalculationGroup.Children.Add(calculation2);
            rootCalculationGroup.Children.Add(nestedGroup1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootCalculationGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    AssertFilesExistInZip(new[]
                    {
                        $"{calculation1.Name}.{fileExtension}",
                        $"{calculation2.Name}.{fileExtension}",
                        $"{nestedGroup1.Name}/{calculation3.Name}.{fileExtension}",
                        $"{nestedGroup1.Name}/{nestedGroup2.Name}/{calculation4.Name}.{fileExtension}"
                    }, filePath);
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_CalculationGroupWithNestedGroupsAndCalculationsWithExportWarnings_LogsMessagesAndWritesFile()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupWithNestedGroupsAndCalculationsWithExportWarnings_LogsMessagesAndWritesFile)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            calculation1.InputParameters.StochasticSoilProfile.SoilProfile.Layers.ForEachElementDo(layer => layer.Data.IsAquifer = true);
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");
            calculation2.InputParameters.StochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D(new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 1)),
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 2))
            });
            MacroStabilityInwardsCalculationScenario calculation3 = CreateCalculation("calculation3");
            MacroStabilityInwardsCalculationScenario calculation4 = CreateCalculation("calculation4");

            var rootCalculationGroup = new CalculationGroup();
            var nestedGroup1 = new CalculationGroup
            {
                Name = "NestedGroup1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "NestedGroup2"
            };
            nestedGroup2.Children.Add(calculation4);
            nestedGroup1.Children.Add(calculation3);
            nestedGroup1.Children.Add(nestedGroup2);
            rootCalculationGroup.Children.Add(calculation1);
            rootCalculationGroup.Children.Add(calculation2);
            rootCalculationGroup.Children.Add(nestedGroup1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootCalculationGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    TestHelper.AssertLogMessagesAreGenerated(Call, new[]
                    {
                        $"'{calculation1.Name}': De schematisatie van de berekening bevat meerdere aquifer lagen. De volgorde van de aquifer lagen kan niet bepaald worden tijdens exporteren. Er worden daarom geen lagen als aquifer geëxporteerd.",
                        $"'{calculation2.Name}': De schematisatie van de berekening bevat meerdere stresspunten binnen één laag of stresspunten die niet aan een laag gekoppeld kunnen worden. Er worden daarom geen POP en grensspanningen geëxporteerd."
                    });
                    Assert.IsTrue(exportResult);
                    AssertFilesExistInZip(new[]
                    {
                        $"{calculation1.Name}.{fileExtension}",
                        $"{calculation2.Name}.{fileExtension}",
                        $"{nestedGroup1.Name}/{calculation3.Name}.{fileExtension}",
                        $"{nestedGroup1.Name}/{nestedGroup2.Name}/{calculation4.Name}.{fileExtension}"
                    }, filePath);
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_CalculationGroupWithCalculationsWithSameName_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupWithCalculationsWithSameName_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");
            MacroStabilityInwardsCalculationScenario calculation3 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation4 = CreateCalculation("calculation1");

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation1);
            calculationGroup.Children.Add(calculation2);
            calculationGroup.Children.Add(calculation3);
            calculationGroup.Children.Add(calculation4);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);

                    AssertFilesExistInZip(new[]
                    {
                        $"{calculation1.Name}.{fileExtension}",
                        $"{calculation2.Name}.{fileExtension}",
                        $"{calculation3.Name} (1).{fileExtension}",
                        $"{calculation4.Name} (2).{fileExtension}"
                    }, filePath);
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_CalculationGroupsWithSameName_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupsWithSameName_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");
            MacroStabilityInwardsCalculationScenario calculation3 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation4 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation5 = CreateCalculation("calculation1");

            var rootGroup = new CalculationGroup
            {
                Name = "root"
            };
            var nestedGroup1 = new CalculationGroup
            {
                Name = "group1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "group1"
            };
            var nestedGroup3 = new CalculationGroup
            {
                Name = "group1"
            };
            nestedGroup1.Children.Add(calculation1);
            nestedGroup1.Children.Add(calculation2);
            nestedGroup2.Children.Add(calculation3);
            nestedGroup2.Children.Add(calculation4);
            nestedGroup3.Children.Add(calculation5);
            rootGroup.Children.Add(nestedGroup1);
            rootGroup.Children.Add(nestedGroup2);
            rootGroup.Children.Add(nestedGroup3);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);

                    AssertFilesExistInZip(new[]
                    {
                        $"{nestedGroup1.Name}/{calculation1.Name}.{fileExtension}",
                        $"{nestedGroup1.Name}/{calculation2.Name}.{fileExtension}",
                        $"{nestedGroup2.Name} (1)/{calculation3.Name}.{fileExtension}",
                        $"{nestedGroup2.Name} (1)/{calculation4.Name} (1).{fileExtension}",
                        $"{nestedGroup3.Name} (2)/{calculation5.Name}.{fileExtension}"
                    }, filePath);
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_NestedCalculationGroupWithoutCalculations_FolderNotExportedAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupsWithSameName_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            var rootGroup = new CalculationGroup
            {
                Name = "root"
            };
            var nestedGroup1 = new CalculationGroup
            {
                Name = "group1"
            };
            rootGroup.Children.Add(nestedGroup1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    Assert.IsFalse(File.Exists(filePath));
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_NestedCalculationGroupWithEmptyCalculationGroups_FolderNotExportedAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_NestedCalculationGroupWithEmptyCalculationGroups_FolderNotExportedAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            var rootGroup = new CalculationGroup
            {
                Name = "root"
            };
            var nestedGroup1 = new CalculationGroup
            {
                Name = "group1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "group2"
            };
            nestedGroup1.Children.Add(nestedGroup2);
            rootGroup.Children.Add(nestedGroup1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    Assert.IsFalse(File.Exists(filePath));
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_NestedCalculationGroupWithCalculationsWithoutOutput_FolderNotExportedAndMessageLoggedAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_NestedCalculationGroupWithCalculationsWithoutOutput_FolderNotExportedAndMessageLoggedAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1", false);

            var rootGroup = new CalculationGroup
            {
                Name = "root"
            };
            var nestedGroup1 = new CalculationGroup
            {
                Name = "group1"
            };
            nestedGroup1.Children.Add(calculation1);
            rootGroup.Children.Add(nestedGroup1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, new[]
                    {
                        new Tuple<string, LogLevelConstant>($"Berekening '{calculation1.Name}' heeft geen uitvoer. Deze berekening wordt overgeslagen.", LogLevelConstant.Warn)
                    });
                    Assert.IsTrue(exportResult);
                    Assert.IsFalse(File.Exists(filePath));
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_NestedCalculationGroupWithGroupsWithCalculationsWithoutOutput_FolderNotExportedAndMessageLoggedAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_NestedCalculationGroupWithGroupsWithCalculationsWithoutOutput_FolderNotExportedAndMessageLoggedAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1", false);
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2", false);

            var rootGroup = new CalculationGroup
            {
                Name = "root"
            };
            var nestedGroup1 = new CalculationGroup
            {
                Name = "group1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "group2"
            };
            nestedGroup2.Children.Add(calculation2);
            nestedGroup1.Children.Add(calculation1);
            nestedGroup1.Children.Add(nestedGroup2);
            rootGroup.Children.Add(nestedGroup1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, new[]
                    {
                        new Tuple<string, LogLevelConstant>($"Berekening '{calculation1.Name}' heeft geen uitvoer. Deze berekening wordt overgeslagen.", LogLevelConstant.Warn),
                        new Tuple<string, LogLevelConstant>($"Berekening '{calculation2.Name}' heeft geen uitvoer. Deze berekening wordt overgeslagen.", LogLevelConstant.Warn)
                    });
                    Assert.IsFalse(Directory.Exists(Path.Combine(folderPath, nestedGroup1.Name, nestedGroup2.Name)));
                    Assert.IsFalse(Directory.Exists(Path.Combine(folderPath, nestedGroup1.Name)));
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_NestedCalculationGroupWithGroupsWithCalculationsWithAndWithoutOutput_FolderNotExportedAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_NestedCalculationGroupWithGroupsWithCalculationsWithAndWithoutOutput_FolderNotExportedAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "export.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1", false);
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");

            var rootGroup = new CalculationGroup
            {
                Name = "root"
            };
            var nestedGroup1 = new CalculationGroup
            {
                Name = "group1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "group2"
            };
            nestedGroup2.Children.Add(calculation2);
            nestedGroup2.Children.Add(calculation1);
            nestedGroup1.Children.Add(nestedGroup2);
            rootGroup.Children.Add(nestedGroup1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, new[]
                    {
                        new Tuple<string, LogLevelConstant>($"Berekening '{calculation1.Name}' heeft geen uitvoer. Deze berekening wordt overgeslagen.", LogLevelConstant.Warn)
                    });
                    Assert.IsTrue(exportResult);

                    AssertFilesExistInZip(new[]
                    {
                        $"{nestedGroup1.Name}/{nestedGroup2.Name}/{calculation2.Name}.{fileExtension}"
                    }, filePath);
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        [Test]
        public void Export_CreatingZipFileThrowsCriticalFileWriteException_LogErrorAndReturnFalse()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CreatingZipFileThrowsCriticalFileWriteException_LogErrorAndReturnFalse)}");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "test.zip");

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, new GeneralMacroStabilityInwardsInput(),
                                                                             new PersistenceFactory(), filePath, fileExtension,
                                                                             c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                using (var helper = new FileDisposeHelper(filePath))
                {
                    helper.LockFiles();

                    // Call
                    var isExported = true;
                    void Call() => isExported = exporter.Export();

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. " +
                                             "Er zijn geen D-GEO Suite Stability Projecten geëxporteerd.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage);
                    Assert.IsFalse(isExported);
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        private static IEnumerable<TestCaseData> GetCalculationGroupConfigurations()
        {
            MacroStabilityInwardsCalculationScenario calculation = CreateCalculation("calculation");

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);
            yield return new TestCaseData(calculationGroup);

            var parentGroup = new CalculationGroup();
            parentGroup.Children.Add(calculationGroup);
            yield return new TestCaseData(parentGroup);
        }

        private static void AssertFilesExistInZip(IEnumerable<string> expectedFiles, string filePath)
        {
            using (ZipArchive zipArchive = ZipFile.OpenRead(filePath))
            {
                CollectionAssert.AreEquivalent(expectedFiles, zipArchive.Entries.Select(e => e.FullName));
            }
        }

        private static MacroStabilityInwardsCalculationScenario CreateCalculation(string calculationName, bool setOutput = true)
        {
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Name = calculationName;
            if (setOutput)
            {
                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();
            }

            return calculation;
        }
    }
}
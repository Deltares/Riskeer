// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Structures;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class StructuresImporterTest
    {
        private readonly StructureCollection<TestStructure> testImportTarget = new StructureCollection<TestStructure>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            // Call
            var importer = new TestStructuresImporter(testImportTarget,
                                                      testReferenceLine,
                                                      testFilePath,
                                                      updateStrategy,
                                                      messageProvider);

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestStructuresImporter(null,
                                                                 testReferenceLine,
                                                                 testFilePath,
                                                                 updateStrategy,
                                                                 messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void Constructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestStructuresImporter(testImportTarget,
                                                                 null,
                                                                 testFilePath,
                                                                 updateStrategy,
                                                                 messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestStructuresImporter(testImportTarget,
                                                                 testReferenceLine,
                                                                 null,
                                                                 updateStrategy,
                                                                 messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void Constructor_StructureUpdateStrategyNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestStructuresImporter(testImportTarget,
                                                                 testReferenceLine,
                                                                 testFilePath,
                                                                 null,
                                                                 messageProvider);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structureUpdateStrategy", paramName);
        }

        [Test]
        public void Constructor_ImporterMessageProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestStructuresImporter(testImportTarget,
                                                                 testReferenceLine,
                                                                 testFilePath,
                                                                 updateStrategy,
                                                                 null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("messageProvider", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        public void Import_FromInvalidEmptyPath_FalseAndLogError(string filePath)
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            var testStructuresImporter = new TestStructuresImporter(testImportTarget,
                                                                    testReferenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilResources.Error_Path_must_be_specified);
                StringAssert.StartsWith(expectedMessage, messageArray[0]);
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromPathContainingInvalidPathCharacters_FalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            const string filePath = "c:\\Invalid_Characters.shp";

            char[] invalidPathChars = Path.GetInvalidPathChars();
            string invalidPath = filePath.Replace('_', invalidPathChars[0]);

            var testStructuresImporter = new TestStructuresImporter(testImportTarget,
                                                                    testReferenceLine,
                                                                    invalidPath,
                                                                    updateStrategy,
                                                                    messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string expectedMessage = new FileReaderErrorMessageBuilder(invalidPath)
                    .Build("Er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.");
                StringAssert.StartsWith(expectedMessage, messages.First());
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromDirectoryPath_FalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string folderPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO) + Path.DirectorySeparatorChar;

            var testStructuresImporter = new TestStructuresImporter(testImportTarget,
                                                                    testReferenceLine,
                                                                    folderPath,
                                                                    updateStrategy,
                                                                    messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(folderPath)
                .Build(CoreCommonUtilResources.Error_Path_must_not_point_to_empty_file_name);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        [TestCase("Multiple_PolyLine_with_ID.shp")]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Single_Multi-PolyLine_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        [TestCase("Single_PolyLine_with_ID.shp")]
        public void Import_FromFileWithNonPointFeatures_FalseAndLogError(string shapeFileName)
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                         shapeFileName);

            var profilesImporter = new TestStructuresImporter(testImportTarget,
                                                              testReferenceLine,
                                                              filePath,
                                                              updateStrategy,
                                                              messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = profilesImporter.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': kon geen punten vinden in dit bestand.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_InvalidShapefile_ReturnsFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("Structures", "StructuresWithoutKWKIDENT", "Kunstwerken.shp"));

            var profilesImporter = new TestStructuresImporter(testImportTarget,
                                                              testReferenceLine,
                                                              invalidFilePath,
                                                              updateStrategy,
                                                              messageProvider);

            // Call
            bool importResult = profilesImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_InvalidCsvFile_ReturnsFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("Structures", "CorrectShpIncorrectCsv", "CorrectKunstwerken_IncorrectCsv.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var profilesImporter = new TestStructuresImporter(testImportTarget,
                                                              referenceLine,
                                                              invalidFilePath,
                                                              updateStrategy,
                                                              messageProvider);

            // Call
            bool importResult = profilesImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingLocations_CancelsImportAndLogs()
        {
            // Setup
            const string messageText = "importeren is afgebroken";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Kunstwerken")).Return(messageText);
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var importTarget = new StructureCollection<TestStructure>();
            var testStructuresImporter = new TestStructuresImporter(importTarget,
                                                                    referenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);
            testStructuresImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van kunstwerklocaties uit een shapebestand."))
                {
                    testStructuresImporter.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, messageText);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhenCreatingStructures_ContinueImportAndLogWarning()
        {
            // Setup
            const string addDataToModelProgressText = "addDataToModelProgressText";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(addDataToModelProgressText);
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var importTarget = new StructureCollection<TestStructure>();
            var testStructuresImporter = new TestStructuresImporter(importTarget,
                                                                    referenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);
            testStructuresImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(addDataToModelProgressText))
                {
                    testStructuresImporter.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingStructureData_FalseAndLogInfo()
        {
            // Setup
            const string messageText = "importeren is afgebroken";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Kunstwerken")).Return(messageText);
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var importTarget = new StructureCollection<TestStructure>();
            var testStructuresImporter = new TestStructuresImporter(importTarget,
                                                                    referenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);
            testStructuresImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van kunstwerkgegevens uit een kommagescheiden bestand."))
                {
                    testStructuresImporter.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(messageText, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_ReturnsTrue()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var importTarget = new StructureCollection<TestStructure>();
            var testStructuresImporter = new TestStructuresImporter(importTarget,
                                                                    referenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);
            testStructuresImporter.SetProgressChanged((description, step, steps) => testStructuresImporter.Cancel());

            bool importResult = testStructuresImporter.Import();

            // Precondition
            Assert.IsFalse(importResult);
            testStructuresImporter.SetProgressChanged(null);

            // Call
            importResult = testStructuresImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_LocationOutsideReferenceLine_FalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testStructuresImporter = new TestStructuresImporter(new StructureCollection<TestStructure>(),
                                                                    referenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            const string expectedMessage = "Een kunstwerklocatie met KWKIDENT 'KUNST6' ligt niet op de referentielijn.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_DuplicateLocation_FalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "DuplicateLocation", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var testStructuresImporter = new TestStructuresImporter(new StructureCollection<TestStructure>(),
                                                                    referenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            const string expectedMessage = "Kunstwerklocatie met KWKIDENT 'KUNST3' is opnieuw ingelezen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_LocationKWKIDENTNull_FalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresWithNullKWKident", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var testStructuresImporter = new TestStructuresImporter(new StructureCollection<TestStructure>(),
                                                                    referenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            const string expectedMessages = "Fout bij het lezen van kunstwerk op regel 1. Het kunstwerk heeft geen geldige waarde voor attribuut 'KWKIDENT'.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessages, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_ValidImportFile_CallsUpdateStrategyWithExpectedArguments()
        {
            // Setup
            var targetCollection = new StructureCollection<TestStructure>();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles",
                                                                      "Kunstwerken.shp"));

            var createdStructures = new[]
            {
                new TestStructure()
            };

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStructureUpdateStrategy<TestStructure>>();
            updateStrategy.Expect(strat => strat.UpdateStructuresWithImportedData(createdStructures, filePath));
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new TestStructuresImporter(targetCollection,
                                                      referenceLine,
                                                      filePath,
                                                      updateStrategy,
                                                      messageProvider)
            {
                CreatedTestStructures = createdStructures
            };

            // Call
            importer.Import();

            // Assert
            Assert.IsTrue(importer.CreateStructuresCalled);

            // Further assertions done in the TearDown
        }

        [Test]
        public void Import_IllegalCsvFile_ReturnsFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "IllegalCsv", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var testStructuresImporter = new TestStructuresImporter(new StructureCollection<TestStructure>(),
                                                                    referenceLine,
                                                                    filePath,
                                                                    updateStrategy,
                                                                    messageProvider);

            // Call
            bool importResult = testStructuresImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_UpdateWithCreatedStructuresThrowsUpdateDataException_ReturnFalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return("");
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText("Kunstwerken")).Return("error {0}");
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            updateStrategy.Expect(us => us.UpdateStructuresWithImportedData(null, null))
                          .IgnoreArguments()
                          .Throw(new UpdateDataException("Exception message"));
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles",
                                                                      "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();
            var importer = new TestStructuresImporter(new StructureCollection<TestStructure>(),
                                                      referenceLine,
                                                      filePath,
                                                      updateStrategy,
                                                      messageProvider);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Then
            const string expectedMessage = "error Exception message";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void DoPostImport_UpdateWithCreatedStructuresReturnsObservables_ObservablesNotified()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();

            var observableA = mocks.StrictMock<IObservable>();
            observableA.Expect(o => o.NotifyObservers());
            var observableB = mocks.StrictMock<IObservable>();
            observableB.Expect(o => o.NotifyObservers());

            var updateStrategy = mocks.StrictMock<IStructureUpdateStrategy<TestStructure>>();
            updateStrategy.Expect(strat => strat.UpdateStructuresWithImportedData(null, null))
                          .IgnoreArguments()
                          .Return(new[]
                          {
                              observableA,
                              observableB
                          });
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles",
                                                                      "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new TestStructuresImporter(new StructureCollection<TestStructure>(),
                                                      referenceLine,
                                                      filePath,
                                                      updateStrategy,
                                                      messageProvider);

            importer.Import();

            // Call
            importer.DoPostImport();

            // Then
            // Assertions performed in TearDown
        }

        [Test]
        public void GetStandardDeviation_RowHasStandardDeviation_ReturnVarianceValue()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referenceLine = new ReferenceLine();
            var importTarget = new StructureCollection<TestStructure>();

            var importer = new TestStructuresImporter(importTarget,
                                                      referenceLine,
                                                      filePath,
                                                      updateStrategy,
                                                      messageProvider);

            var parameter = new StructuresParameterRow
            {
                AlphanumericValue = "",
                LineNumber = 3,
                LocationId = "A",
                NumericalValue = -2,
                ParameterId = "B",
                VarianceType = VarianceType.StandardDeviation,
                VarianceValue = 1.2
            };

            // Call
            var standardDeviation = (RoundedDouble) 0.0;
            Action call = () => standardDeviation = importer.GetStandardDeviation(parameter, "<naam kunstwerk>");

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(parameter.VarianceValue, standardDeviation, standardDeviation.GetAccuracy());
        }

        [Test]
        public void GetStandardDeviation_RowHasCoefficientOfVariation_LogWarningReturnConvertedVarianceValue()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referenceLine = new ReferenceLine();
            var importTarget = new StructureCollection<TestStructure>();

            var importer = new TestStructuresImporter(importTarget,
                                                      referenceLine,
                                                      filePath,
                                                      updateStrategy,
                                                      messageProvider);

            var parameter = new StructuresParameterRow
            {
                AlphanumericValue = "",
                LineNumber = 3,
                LocationId = "A",
                NumericalValue = -2,
                ParameterId = "B",
                VarianceType = VarianceType.CoefficientOfVariation,
                VarianceValue = 1.2
            };

            const string structureName = "<naam kunstwerk>";

            // Call
            var standardDeviation = (RoundedDouble) 0.0;
            Action call = () => standardDeviation = importer.GetStandardDeviation(parameter, structureName);

            // Assert
            string expectedMessage =
                string.Format("De variatie voor parameter '{2}' van kunstwerk '{0}' ({1}) wordt omgerekend in een standaardafwijking (regel {3}).",
                              structureName, parameter.LocationId, parameter.ParameterId, parameter.LineNumber);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);
            double expectedStandardDeviation = parameter.VarianceValue * Math.Abs(parameter.NumericalValue);
            Assert.AreEqual(expectedStandardDeviation, standardDeviation, standardDeviation.GetAccuracy());
        }

        [Test]
        public void GetCoefficientOfVariation_RowHasCoefficientOfVariation_ReturnVarianceValue()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referenceLine = new ReferenceLine();
            var importTarget = new StructureCollection<TestStructure>();

            var importer = new TestStructuresImporter(importTarget,
                                                      referenceLine,
                                                      filePath,
                                                      updateStrategy,
                                                      messageProvider);

            var parameter = new StructuresParameterRow
            {
                AlphanumericValue = "",
                LineNumber = 3,
                LocationId = "A",
                NumericalValue = -3,
                ParameterId = "B",
                VarianceType = VarianceType.CoefficientOfVariation,
                VarianceValue = 2.3
            };

            // Call
            var coefficientOfVariation = (RoundedDouble) 0.0;
            Action call = () => coefficientOfVariation = importer.GetCoefficientOfVariation(parameter, "<naam kunstwerk>");

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(parameter.VarianceValue, coefficientOfVariation, coefficientOfVariation.GetAccuracy());
        }

        [Test]
        public void GetCoefficientOfVariation_RowHasStandardDeviation_LogWarningReturnConvertedVarianceValue()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<TestStructure>>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referenceLine = new ReferenceLine();
            var importTarget = new StructureCollection<TestStructure>();

            var importer = new TestStructuresImporter(importTarget,
                                                      referenceLine,
                                                      filePath,
                                                      updateStrategy,
                                                      messageProvider);

            var parameter = new StructuresParameterRow
            {
                AlphanumericValue = "",
                LineNumber = 3,
                LocationId = "A",
                NumericalValue = -3,
                ParameterId = "B",
                VarianceType = VarianceType.StandardDeviation,
                VarianceValue = 2.3
            };

            const string structureName = "<naam kunstwerk>";

            // Call
            var coefficientOfVariation = (RoundedDouble) 0.0;
            Action call = () => coefficientOfVariation = importer.GetCoefficientOfVariation(parameter, structureName);

            // Assert
            string expectedMessage = string.Format(
                "De variatie voor parameter '{2}' van kunstwerk '{0}' ({1}) wordt omgerekend in een variatiecoëfficiënt (regel {3}).",
                structureName, parameter.LocationId, parameter.ParameterId, parameter.LineNumber);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);
            double expectedStandardDeviation = parameter.VarianceValue / Math.Abs(parameter.NumericalValue);
            Assert.AreEqual(expectedStandardDeviation, coefficientOfVariation, coefficientOfVariation.GetAccuracy());
        }

        private static ReferenceLine CreateReferenceLine()
        {
            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            return referenceLine;
        }

        private class TestStructuresImporter : StructuresImporter<TestStructure>
        {
            public TestStructuresImporter(StructureCollection<TestStructure> importTarget,
                                          ReferenceLine referenceLine,
                                          string filePath,
                                          IStructureUpdateStrategy<TestStructure> structureUpdateStrategy,
                                          IImporterMessageProvider messageProvider)
                : base(importTarget, referenceLine, filePath, messageProvider, structureUpdateStrategy) {}

            public IEnumerable<TestStructure> CreatedTestStructures { private get; set; } = Enumerable.Empty<TestStructure>();

            public bool CreateStructuresCalled { get; private set; }

            public new RoundedDouble GetStandardDeviation(StructuresParameterRow parameter, string structureName)
            {
                return base.GetStandardDeviation(parameter, structureName);
            }

            public new RoundedDouble GetCoefficientOfVariation(StructuresParameterRow parameter, string structureName)
            {
                return base.GetCoefficientOfVariation(parameter, structureName);
            }

            protected override IEnumerable<TestStructure> CreateStructures(IEnumerable<StructureLocation> structureLocations,
                                                                           IDictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
            {
                CreateStructuresCalled = true;

                return CreatedTestStructures;
            }
        }
    }
}
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
using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Util;
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.Exceptions;
using Riskeer.Storage.Core.TestUtil;

namespace Riskeer.Storage.Core.Test
{
    [TestFixture]
    public class StorageSqLiteTest
    {
        private readonly string testPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Storage.Core, "DatabaseFiles");
        private readonly string workingDirectory = TestHelper.GetScratchPadPath(nameof(StorageSqLiteTest));
        private DirectoryDisposeHelper directoryDisposeHelper;

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var storage = new StorageSqLite();

            // Assert
            Assert.IsInstanceOf<IStoreProject>(storage);
            Assert.AreEqual("Alle projecten (*.risk;*.rtd)|*.risk;*.rtd|" +
                            "Riskeerproject (*.risk)|*.risk|Ringtoetsproject (*.rtd)|*.rtd", storage.OpenProjectFileFilter);

            Assert.AreEqual("Riskeerproject (*.risk)|*.risk", storage.SaveProjectFileFilter);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void LoadProject_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            void Call() => new StorageSqLite().LoadProject(invalidPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($"Fout bij het lezen van bestand '{invalidPath}': bestandspad mag niet leeg of ongedefinieerd zijn.",
                            exception.Message);
        }

        [Test]
        public void LoadProject_NonExistingPath_ThrowsStorageExceptionAndCouldNotConnectException()
        {
            // Setup
            const string nonExistingPath = "fileDoesNotExist";

            // Call
            void Call() => new StorageSqLite().LoadProject(nonExistingPath);

            // Assert
            StorageException exception = Assert.Throws<CouldNotConnectException>(Call);

            Assert.AreEqual($@"Fout bij het lezen van bestand '{nonExistingPath}': het bestand bestaat niet.",
                            exception.Message);
        }

        [Test]
        public void LoadProject_ProjectFileWithoutSchema_ThrowsStorageExceptionAndStorageValidationException()
        {
            // Setup
            const string validPath = "empty.risk";
            string tempFile = Path.Combine(testPath, validPath);

            // Call
            void Call() => new StorageSqLite().LoadProject(tempFile);

            // Assert
            StorageException exception = Assert.Throws<StorageValidationException>(Call);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempFile}': het bestand is geen geldig Riskeer bestand.",
                            exception.Message);
        }

        [Test]
        public void LoadProject_ProjectFileWithTwoProjects_ThrowsStorageExceptionAndStorageValidationException()
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(LoadProject_ProjectFileWithTwoProjects_ThrowsStorageExceptionAndStorageValidationException));

            void Precondition() => SqLiteDatabaseHelper.CreateCompleteDatabaseFileWithoutProjectData(tempProjectFilePath);
            Assert.DoesNotThrow(Precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            void Call() => new StorageSqLite().LoadProject(tempProjectFilePath);

            // Assert
            var expectedMessage = $@"Fout bij het lezen van bestand '{tempProjectFilePath}': het bestand is geen geldig Riskeer bestand.";

            var exception = Assert.Throws<StorageException>(Call);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void LoadProject_CorruptProjectFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace()
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(LoadProject_CorruptProjectFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace));
            void Precondition() => SqLiteDatabaseHelper.CreateCorruptDatabaseFile(tempProjectFilePath);
            Assert.DoesNotThrow(Precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            void Call() => new StorageSqLite().LoadProject(tempProjectFilePath);

            // Assert
            var exception = Assert.Throws<StorageException>(Call);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempProjectFilePath}': het bestand is geen geldig Riskeer bestand.",
                            exception.Message);

            Assert.IsInstanceOf<DataException>(exception.InnerException);
            Assert.AreEqual("An error occurred while executing the command definition. See the inner exception for details.",
                            exception.InnerException.Message);

            Assert.IsInstanceOf<SQLiteException>(exception.InnerException.InnerException);
            Assert.AreEqual($"SQL logic error{Environment.NewLine}"
                            + "no such table: HydraulicBoundaryDataEntity", exception.InnerException.InnerException.Message);
        }

        [Test]
        public void LoadProject_DatabaseWithoutVersionEntities_ThrowStorageValidationException()
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(LoadProject_DatabaseWithoutVersionEntities_ThrowStorageValidationException));

            void Precondition() => SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(tempProjectFilePath);
            Assert.DoesNotThrow(Precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            void Call() => new StorageSqLite().LoadProject(tempProjectFilePath);

            // Assert
            StorageException exception = Assert.Throws<StorageValidationException>(Call);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempProjectFilePath}': database moet één rij in de VersionEntity tabel hebben.",
                            exception.Message);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        [Test]
        public void LoadProject_DatabaseWithMultipleVersionEntities_ThrowStorageValidationException()
        {
            // Setup
            string projectFilePath = Path.Combine(workingDirectory, nameof(LoadProject_DatabaseWithMultipleVersionEntities_ThrowStorageValidationException));
            string currentDatabaseVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();

            void Precondition()
            {
                SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(projectFilePath);
                SqLiteDatabaseHelper.AddVersionEntity(projectFilePath, currentDatabaseVersion);
                SqLiteDatabaseHelper.AddVersionEntity(projectFilePath, currentDatabaseVersion);
            }

            Assert.DoesNotThrow(Precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            void Call() => new StorageSqLite().LoadProject(projectFilePath);

            // Assert
            StorageException exception = Assert.Throws<StorageValidationException>(Call);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{projectFilePath}': database moet één rij in de VersionEntity tabel hebben.",
                            exception.Message);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        [Test]
        public void LoadProject_DatabaseWithInvalidHydraulicBoundaryDataEntity_LogError()
        {
            // Setup
            string projectFilePath = Path.Combine(workingDirectory, nameof(LoadProject_DatabaseWithInvalidHydraulicBoundaryDataEntity_LogError));
            const string invalidFolderPath = "invalid folder path";
            
            RiskeerProject fullProject = RiskeerProjectTestHelper.GetFullTestProject();

            void Precondition()
            {
                fullProject.AssessmentSection.HydraulicBoundaryData.SetNewFolderPath(invalidFolderPath);
                SqLiteDatabaseHelper.CreateValidProjectDatabase(projectFilePath, fullProject);
            }

            Assert.DoesNotThrow(Precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            void Call() => new StorageSqLite().LoadProject(projectFilePath);

            // Assert
            TestHelper.AssertLogMessagesCount(Call, 1);
            TestHelper.AssertLogMessageIsGenerated(Call, $"Herstellen van de verbinding met de hydraulische belastingendatabases is mislukt. De bestandsmap '{invalidFolderPath}' bestaat niet.");
        }

        [Test]
        [TestCase(1)]
        [TestCase(500)]
        public void LoadProject_DatabaseFromFutureVersion_ThrowStorageValidationException(int additionalVersionNumber)
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, $"{nameof(LoadProject_DatabaseFromFutureVersion_ThrowStorageValidationException)}_{Path.GetRandomFileName()}");
            string currentDatabaseVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();
            string versionCode = additionalVersionNumber + currentDatabaseVersion;

            void Precondition()
            {
                SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(tempProjectFilePath);
                SqLiteDatabaseHelper.AddVersionEntity(tempProjectFilePath, versionCode);
            }

            Assert.DoesNotThrow(Precondition, "Precondition failed: creating future database file failed");

            // Call
            void Call() => new StorageSqLite().LoadProject(tempProjectFilePath);

            // Assert
            StorageException exception = Assert.Throws<StorageValidationException>(Call);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempProjectFilePath}': riskeer "
                            + $"bestand versie '{versionCode}' is hoger dan de huidig ondersteunde versie "
                            + $"('{currentDatabaseVersion}'). Update Riskeer naar een nieuwere versie.",
                            exception.Message);
        }

        [Test]
        [TestCase("0")]
        [TestCase("-567")]
        public void LoadProject_DatabaseWithInvalidVersionCode_ThrowStorageValidationException(string versionCode)
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, $"{nameof(LoadProject_DatabaseWithInvalidVersionCode_ThrowStorageValidationException)}_{Path.GetRandomFileName()}");

            void Precondition()
            {
                SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(tempProjectFilePath);
                SqLiteDatabaseHelper.AddVersionEntity(tempProjectFilePath, versionCode);
            }

            Assert.DoesNotThrow(Precondition, "Precondition failed: creating future database file failed");

            // Call
            void Call() => new StorageSqLite().LoadProject(tempProjectFilePath);

            // Assert
            StorageException exception = Assert.Throws<StorageValidationException>(Call);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempProjectFilePath}': riskeer "
                            + $"bestand versie '{versionCode}' is niet valide. De versie van het Riskeer projectbestand "
                            + "dient '16.4' of hoger te zijn.", exception.Message);
        }

        [Test]
        public void LoadProject_ReadProjectThrowsEntityReadException_ThrowsStorageException()
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(LoadProject_ReadProjectThrowsEntityReadException_ThrowsStorageException));
            var storage = new StorageSqLite();

            RiskeerProject project = CreateProject();

            // Precondition
            void Precondition()
            {
                SqLiteDatabaseHelper.CreateValidProjectDatabase(tempProjectFilePath, project);
                SqLiteDatabaseHelper.SetInvalidNumberOfAssessmentSectionEntities(tempProjectFilePath);
            }

            Assert.DoesNotThrow(Precondition);

            // Call
            void Call() => storage.LoadProject(tempProjectFilePath);

            // Assert
            var exception = Assert.Throws<StorageException>(Call);
            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<EntityReadException>(innerException);

            var expectedMessage = $"Fout bij het lezen van bestand '{tempProjectFilePath}': {innerException.Message.FirstToLower()}";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void LoadProject_ValidDatabase_ReturnsProject()
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(LoadProject_ValidDatabase_ReturnsProject));
            string projectName = Path.GetFileNameWithoutExtension(tempProjectFilePath);
            var storage = new StorageSqLite();

            RiskeerProject project = CreateProject();
            project.Description = "<some description>";

            // Precondition
            SqLiteDatabaseHelper.CreateValidProjectDatabase(tempProjectFilePath, project);

            // Call
            IProject loadedProject = storage.LoadProject(tempProjectFilePath);

            // Assert
            Assert.IsInstanceOf<RiskeerProject>(loadedProject);
            Assert.AreEqual(projectName, loadedProject.Name);
            Assert.AreEqual(project.Description, loadedProject.Description);
        }

        [Test]
        public void StageProject_ProjectIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var storage = new StorageSqLite();

            // Call
            void Call() => storage.StageProject(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Call);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void SaveProjectAs_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Setup
            RiskeerProject project = CreateProject();

            var storage = new StorageSqLite();
            storage.StageProject(project);

            // Call
            void Call() => storage.SaveProjectAs(invalidPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Fout bij het lezen van bestand '{invalidPath}': bestandspad mag niet "
                            + "leeg of ongedefinieerd zijn.", exception.Message);
        }

        [Test]
        public void SaveProjectAs_ValidPathToNonExistingFile_DoesNotThrowException()
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(SaveProjectAs_ValidPathToNonExistingFile_DoesNotThrowException));
            RiskeerProject project = CreateProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            // Precondition
            Assert.IsFalse(File.Exists(tempProjectFilePath));

            // Call
            void Call() => storage.SaveProjectAs(tempProjectFilePath);

            // Assert
            Assert.DoesNotThrow(Call);
        }

        [Test]
        public void SaveProjectAs_ValidPathToExistingFile_DoesNotThrowException()
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(SaveProjectAs_ValidPathToExistingFile_DoesNotThrowException));
            RiskeerProject project = CreateProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            using (File.Create(tempProjectFilePath)) {}

            // Call
            void Call() => storage.SaveProjectAs(tempProjectFilePath);

            // Assert
            Assert.DoesNotThrow(Call);
        }

        [Test]
        public void SaveProjectAs_ValidPathToLockedFile_ThrowsUpdateStorageException()
        {
            // Setup
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(SaveProjectAs_ValidPathToLockedFile_ThrowsUpdateStorageException));
            RiskeerProject project = CreateProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            using (var fileDisposeHelper = new FileDisposeHelper(tempProjectFilePath))
            {
                try
                {
                    fileDisposeHelper.LockFiles();

                    // Call
                    void Call() => storage.SaveProjectAs(tempProjectFilePath);

                    // Assert
                    var exception = Assert.Throws<StorageException>(Call);

                    Assert.IsInstanceOf<Exception>(exception);
                    Assert.IsInstanceOf<IOException>(exception.InnerException);
                    Assert.IsInstanceOf<Exception>(exception);
                    Assert.AreEqual("Het doelbestand is momenteel in gebruik.", exception.Message);
                }
                finally
                {
                    CallGarbageCollector();
                }
            }
        }

        [Test]
        public void SaveProjectAs_NoStagedProject_ThrowInvalidOperationException()
        {
            // Setup
            string tempProjectFilePath = TestHelper.GetScratchPadPath(nameof(SaveProjectAs_NoStagedProject_ThrowInvalidOperationException));
            var storage = new StorageSqLite();

            // Precondition
            Assert.IsFalse(storage.HasStagedProject);

            // Call
            void Call() => storage.SaveProjectAs(tempProjectFilePath);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(Call).Message;
            Assert.AreEqual("Call 'StageProject(IProject)' first before calling this method.", message);
        }

        [Test]
        public void HasStagedProjectChanges_InvalidPath_ThrowsArgumentException()
        {
            // Setup
            var storage = new StorageSqLite();
            storage.StageProject(CreateProject());

            string path = Path.Combine(testPath, "ValidCharacteristics.csv");
            char[] invalidCharacters = Path.GetInvalidPathChars();
            string corruptPath = path.Replace('V', invalidCharacters[0]);

            // Call
            void Call() => storage.HasStagedProjectChanges(corruptPath);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void HasStagedProjectChanges_NoProjectStaged_ThrowInvalidOperationException()
        {
            // Setup
            var storage = new StorageSqLite();

            // Precondition
            Assert.IsFalse(storage.HasStagedProject);

            // Call
            void Call() => storage.HasStagedProjectChanges(null);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(Call).Message;
            Assert.AreEqual("Call 'StageProject(IProject)' first before calling this method.", message);
        }

        [Test]
        public void HasStagedProjectChanges_NoPathGiven_ReturnsTrue()
        {
            // Setup
            var storageSqLite = new StorageSqLite();
            storageSqLite.StageProject(CreateProject());

            // Call
            bool hasChanges = storageSqLite.HasStagedProjectChanges(null);

            // Assert
            Assert.IsTrue(hasChanges);
        }

        [Test]
        public void HasStagedProjectChanges_ValidProjectLoaded_ReturnsFalse()
        {
            // Setup
            var storageSqLite = new StorageSqLite();
            RiskeerProject storedProject = CreateProject();
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(HasStagedProjectChanges_ValidProjectLoaded_ReturnsFalse));

            SqLiteDatabaseHelper.CreateValidProjectDatabase(tempProjectFilePath, storedProject);
            IProject loadedProject = storageSqLite.LoadProject(tempProjectFilePath);
            storageSqLite.StageProject(loadedProject);

            // Call
            bool hasChanges = storageSqLite.HasStagedProjectChanges(tempProjectFilePath);

            // Assert
            Assert.IsFalse(hasChanges);
        }

        [Test]
        public void HasStagedProjectChanges_ValidProjectLoadedWithUnaffectedChange_ReturnsFalse()
        {
            // Setup
            var storageSqLite = new StorageSqLite();
            RiskeerProject storedProject = CreateProject();
            const string changedName = "some name";
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(HasStagedProjectChanges_ValidProjectLoadedWithUnaffectedChange_ReturnsFalse));

            SqLiteDatabaseHelper.CreateValidProjectDatabase(tempProjectFilePath, storedProject);
            IProject loadedProject = storageSqLite.LoadProject(tempProjectFilePath);
            storageSqLite.StageProject(loadedProject);

            // Call
            loadedProject.Name = changedName;
            bool hasChanges = storageSqLite.HasStagedProjectChanges(tempProjectFilePath);

            // Assert
            Assert.IsFalse(hasChanges);
        }

        [Test]
        public void HasStagedProjectChanges_ValidProjectLoadedWithAffectedChange_ReturnsTrue()
        {
            // Setup
            var storageSqLite = new StorageSqLite();
            RiskeerProject storedProject = RiskeerProjectTestHelper.GetFullTestProject();
            const string changedDescription = "some description";
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(HasStagedProjectChanges_ValidProjectLoadedWithAffectedChange_ReturnsTrue));

            SqLiteDatabaseHelper.CreateValidProjectDatabase(tempProjectFilePath, storedProject);
            IProject loadedProject = storageSqLite.LoadProject(tempProjectFilePath);

            loadedProject.Description = changedDescription;
            storageSqLite.StageProject(loadedProject);

            // Call
            bool hasChanges = storageSqLite.HasStagedProjectChanges(tempProjectFilePath);

            // Assert
            Assert.IsTrue(hasChanges);
        }

        [Test]
        public void HasStagedProjectChanges_SavedToEmptyDatabaseFile_ReturnsFalse()
        {
            // Setup
            var storage = new StorageSqLite();
            string tempProjectFilePath = Path.Combine(workingDirectory, nameof(HasStagedProjectChanges_SavedToEmptyDatabaseFile_ReturnsFalse));

            RiskeerProject project = CreateProject();

            // Precondition, required to set the connection string
            storage.StageProject(project);
            void Precondition() => storage.SaveProjectAs(tempProjectFilePath);
            Assert.DoesNotThrow(Precondition, "Precondition failed: creating database file failed");

            storage.StageProject(project);

            // Call
            bool hasChanges = storage.HasStagedProjectChanges(tempProjectFilePath);

            // Assert
            Assert.IsFalse(hasChanges);
        }

        private static RiskeerProject CreateProject()
        {
            return new RiskeerProject(CreateAssessmentSection());
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var random = new Random(21);
            return new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(StorageSqLiteTest));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            CallGarbageCollector();
            directoryDisposeHelper.Dispose();
        }

        private static void CallGarbageCollector()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
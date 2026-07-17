// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;
using Riskeer.Integration.Forms.Merge;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.Plugin.Merge;
using Riskeer.Integration.Service.Comparers;

namespace Riskeer.Integration.Plugin.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergerTest
    {
        [Test]
        public void Constructor_FilePathProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();

            // Call
            void Call() => new AssessmentSectionMerger(null, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("filePathProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();

            // Call
            void Call() => new AssessmentSectionMerger(filePathProvider, null, comparer, mergeDataProvider, mergeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSectionProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_MergeComparerNull_ThrowsArgumentNullException()
        {
            // Setup
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();

            // Call
            void Call() => new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, null, mergeDataProvider, mergeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("mergeComparer", exception.ParamName);
        }

        [Test]
        public void Constructor_MergeDataProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();

            // Call
            void Call() => new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, null, mergeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("mergeDataProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_MergeHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();

            // Call
            void Call() => new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("mergeHandler", exception.ParamName);
        }

        [Test]
        public void StartMerge_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();
            var hydraulicBoundaryDataUpdateHandler = Substitute.For<IHydraulicBoundaryDataUpdateHandler>();
            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // Call
            void Call() => merger.StartMerge(null, hydraulicBoundaryDataUpdateHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void StartMerge_HydraulidBoundaryDataUpdatHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();
            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // Call
            void Call() => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDataUpdateHandler", exception.ParamName);
        }

        [Test]
        public void StartMerge_FilePathNull_LogCancelMessageAndAbort()
        {
            // Setup
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.GetFilePath().Returns((string) null);
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();
            var hydraulicBoundaryDataUpdateHandler = Substitute.For<IHydraulicBoundaryDataUpdateHandler>();
            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // Call
            void Call() => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike), hydraulicBoundaryDataUpdateHandler);

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>("Importeren van gegevens is geannuleerd.", LogLevelConstant.Warn), 1);
        }

        [Test]
        public void GivenValidFilePath_WhenAssessmentSectionProviderThrowsAssessmentSectionProviderException_ThenAbort()
        {
            // Given
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.GetFilePath().Returns(string.Empty);
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            assessmentSectionProvider.GetAssessmentSection(Arg.Any<string>()).Returns(_ => throw new AssessmentSectionProviderException());

            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();
            var hydraulicBoundaryDataUpdateHandler = Substitute.For<IHydraulicBoundaryDataUpdateHandler>();
            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            void Call() => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike), hydraulicBoundaryDataUpdateHandler);

            // Then
            TestHelper.AssertLogMessagesCount(Call, 0);
            assessmentSectionProvider.Received().GetAssessmentSection(Arg.Any<string>());
        }

        [Test]
        public void GivenAssessmentSection_WhenComparerReturnsFalse_ThenLogErrorAndAbort()
        {
            // Given
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.GetFilePath().Returns(string.Empty);
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            assessmentSectionProvider.GetAssessmentSection(Arg.Any<string>())
                                     .Returns(new AssessmentSection(AssessmentSectionComposition.Dike));
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            comparer.Compare(Arg.Any<AssessmentSection>(), Arg.Any<AssessmentSection>()).Returns(false);
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();
            var hydraulicBoundaryDataUpdateHandler = Substitute.For<IHydraulicBoundaryDataUpdateHandler>();
            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            void Call() => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike), hydraulicBoundaryDataUpdateHandler);

            // Then
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>("Er is geen traject gevonden dat samengevoegd kan worden.", LogLevelConstant.Error), 1);
            comparer.Received().Compare(Arg.Any<AssessmentSection>(), Arg.Any<AssessmentSection>());
        }

        [Test]
        public void GivenMatchedAssessmentSection_WhenMergeDataProviderReturnsNull_ThenLogCancelMessageAndAbort()
        {
            // Given
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.GetFilePath().Returns(string.Empty);
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            assessmentSectionProvider.GetAssessmentSection(Arg.Any<string>())
                                     .Returns(new AssessmentSection(AssessmentSectionComposition.Dike));
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            comparer.Compare(Arg.Any<AssessmentSection>(), Arg.Any<AssessmentSection>()).Returns(true);
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            mergeDataProvider.GetMergeData(Arg.Any<AssessmentSection>()).Returns((AssessmentSectionMergeData) null);
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();
            var hydraulicBoundaryDataUpdateHandler = Substitute.For<IHydraulicBoundaryDataUpdateHandler>();
            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            void Call() => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike), hydraulicBoundaryDataUpdateHandler);

            // Then
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>("Importeren van gegevens is geannuleerd.", LogLevelConstant.Warn), 1);
            mergeDataProvider.Received().GetMergeData(Arg.Any<AssessmentSection>());
        }

        [Test]
        public void GivenMatchedAssessmentSection_WhenAllDataValid_ThenMergePerformedAndLogged()
        {
            // Given
            const string filePath = "filePath";
            var originalAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var assessmentSectionToMerge = new AssessmentSection(AssessmentSectionComposition.Dike);
            var mergeData = new AssessmentSectionMergeData(assessmentSectionToMerge, CreateDefaultConstructionProperties());
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.GetFilePath().Returns(filePath);
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            assessmentSectionProvider.GetAssessmentSection(filePath).Returns(assessmentSectionToMerge);
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            comparer.Compare(originalAssessmentSection, assessmentSectionToMerge).Returns(true);
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            mergeDataProvider.GetMergeData(Arg.Any<AssessmentSection>()).Returns(mergeData);
            var hydraulicBoundaryDataUpdateHandler = Substitute.For<IHydraulicBoundaryDataUpdateHandler>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();
            mergeHandler.PerformMerge(originalAssessmentSection, mergeData, hydraulicBoundaryDataUpdateHandler);
            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            void Call() => merger.StartMerge(originalAssessmentSection, hydraulicBoundaryDataUpdateHandler);

            // Then
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, new[]
            {
                new Tuple<string, LogLevelConstant>("Samenvoegen van trajectinformatie is gestart.", LogLevelConstant.Info),
                new Tuple<string, LogLevelConstant>("Samenvoegen van trajectinformatie is gelukt.", LogLevelConstant.Info)
            });
            mergeHandler.Received().PerformMerge(originalAssessmentSection, mergeData, hydraulicBoundaryDataUpdateHandler);
        }

        [Test]
        public void GivenMatchedAssessmentSection_WhenMergeHandlerThrowsException_ThenMergeFailedAndLogged()
        {
            // Given
            var originalAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var assessmentSectionToMerge = new AssessmentSection(AssessmentSectionComposition.Dike);
            var mergeData = new AssessmentSectionMergeData(assessmentSectionToMerge, CreateDefaultConstructionProperties());
            var filePathProvider = Substitute.For<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.GetFilePath().Returns(string.Empty);
            var assessmentSectionProvider = Substitute.For<IAssessmentSectionProvider>();
            assessmentSectionProvider.GetAssessmentSection(Arg.Any<string>())
                                     .Returns(assessmentSectionToMerge);
            var comparer = Substitute.For<IAssessmentSectionMergeComparer>();
            comparer.Compare(originalAssessmentSection, assessmentSectionToMerge).Returns(true);
            var mergeDataProvider = Substitute.For<IAssessmentSectionMergeDataProvider>();
            mergeDataProvider.GetMergeData(Arg.Any<AssessmentSection>()).Returns(mergeData);
            var hydraulicBoundaryDataUpdateHandler = Substitute.For<IHydraulicBoundaryDataUpdateHandler>();
            var mergeHandler = Substitute.For<IAssessmentSectionMergeHandler>();
            mergeHandler.When(x => x.PerformMerge(originalAssessmentSection, mergeData, hydraulicBoundaryDataUpdateHandler)).Do(_ => throw new Exception());
            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            void Call() => merger.StartMerge(originalAssessmentSection, hydraulicBoundaryDataUpdateHandler);

            // Then
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
            {
                mergeHandler.Received().PerformMerge(originalAssessmentSection, mergeData, hydraulicBoundaryDataUpdateHandler);

                Assert.AreEqual(3, messages.Count());

                Assert.AreEqual("Samenvoegen van trajectinformatie is gestart.", messages.ElementAt(0).Item1);

                Tuple<string, Level, Exception> expectedLog = messages.ElementAt(1);
                Assert.AreEqual("Er is een onverwachte fout opgetreden tijdens het samenvoegen van de trajecten.", expectedLog.Item1);
                Assert.AreEqual(Level.Error, expectedLog.Item2);
                Exception loggedException = expectedLog.Item3;
                Assert.IsInstanceOf<Exception>(loggedException);

                Assert.AreEqual("Samenvoegen van trajectinformatie is mislukt.", messages.ElementAt(2).Item1);
            });
        }

        private static AssessmentSectionMergeData.ConstructionProperties CreateDefaultConstructionProperties()
        {
            return new AssessmentSectionMergeData.ConstructionProperties();
        }
    }
}
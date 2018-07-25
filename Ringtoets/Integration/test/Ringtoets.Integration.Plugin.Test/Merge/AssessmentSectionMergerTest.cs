// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Forms.Merge;
using Ringtoets.Integration.Plugin.Merge;
using Ringtoets.Integration.Service.Comparers;

namespace Ringtoets.Integration.Plugin.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergerTest
    {
        [Test]
        public void Constructor_FilePathProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(null, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePathProvider", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(filePathProvider, null, comparer, mergeDataProvider, mergeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionProvider", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_MergeComparerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, null, mergeDataProvider, mergeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mergeComparer", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_MergeDataProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, null, mergeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mergeDataProvider", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_MergeHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mergeHandler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void StartMerge_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // Call
            TestDelegate call = () => merger.StartMerge(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void StartMerge_FilePathNull_LogCancelMessageAndAbort()
        {
            // Setup
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.Expect(helper => helper.GetFilePath()).Return(null);
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // Call
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Importeren van gegevens is geannuleerd.", LogLevelConstant.Warn), 1);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenValidFilePath_WhenAssessmentSectionProviderThrowsAssessmentSectionProviderException_ThenAbort()
        {
            // Given
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.Expect(helper => helper.GetFilePath()).Return(string.Empty);
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            assessmentSectionProvider.Expect(asp => asp.GetAssessmentSections(null)).IgnoreArguments()
                                     .Throw(new AssessmentSectionProviderException());
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Then
            TestHelper.AssertLogMessagesCount(call, 0);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenValidFilePath_WhenAssessmentSectionProviderReturnsEmptyCollection_ThenLogErrorAndAbort()
        {
            // Given
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.Expect(helper => helper.GetFilePath()).Return(string.Empty);
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            assessmentSectionProvider.Expect(asp => asp.GetAssessmentSections(null)).IgnoreArguments()
                                     .Return(Enumerable.Empty<AssessmentSection>());
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Then
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Er zijn geen trajecten gevonden die samengevoegd kunnen worden.", LogLevelConstant.Error), 1);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSection_WhenComparerReturnsFalse_ThenLogErrorAndAbort()
        {
            // Given
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.Expect(helper => helper.GetFilePath()).Return(string.Empty);
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            assessmentSectionProvider.Expect(asp => asp.GetAssessmentSections(null)).IgnoreArguments()
                                     .Return(new[]
                                     {
                                         new AssessmentSection(AssessmentSectionComposition.Dike)
                                     });
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            comparer.Expect(c => c.Compare(null, null)).IgnoreArguments().Return(false);
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Then
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Er zijn geen trajecten gevonden die samengevoegd kunnen worden.", LogLevelConstant.Error), 1);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenMatchedAssessmentSection_WhenMergeDataProviderReturnsNull_ThenLogCancelMessageAndAbort()
        {
            // Given
            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.Expect(helper => helper.GetFilePath()).Return(string.Empty);
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            assessmentSectionProvider.Expect(asp => asp.GetAssessmentSections(null)).IgnoreArguments()
                                     .Return(new[]
                                     {
                                         new AssessmentSection(AssessmentSectionComposition.Dike)
                                     });
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            comparer.Expect(c => c.Compare(null, null)).IgnoreArguments().Return(true);
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            mergeDataProvider.Expect(mdp => mdp.GetMergeData(null)).IgnoreArguments().Return(null);
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Then
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Importeren van gegevens is geannuleerd.", LogLevelConstant.Warn), 1);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenMatchedAssessmentSection_WhenAllDataValid_ThenMergePerformedAndLogged()
        {
            // Given
            var filePath = "Filepath";
            var originalAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var assessmentSectionToMerge = new AssessmentSection(AssessmentSectionComposition.Dike);
            var mergeData = new AssessmentSectionMergeData(assessmentSectionToMerge, new AssessmentSectionMergeData.ConstructionProperties());

            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.Expect(helper => helper.GetFilePath()).Return(filePath);
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            assessmentSectionProvider.Expect(asp => asp.GetAssessmentSections(filePath))
                                     .Return(new[]
                                     {
                                         assessmentSectionToMerge
                                     });
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            comparer.Expect(c => c.Compare(originalAssessmentSection, assessmentSectionToMerge)).Return(true);
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            mergeDataProvider.Expect(mdp => mdp.GetMergeData(null)).IgnoreArguments().Return(mergeData);
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mergeHandler.Expect(mh => mh.PerformMerge(originalAssessmentSection, mergeData));
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            Action call = () => merger.StartMerge(originalAssessmentSection);

            // Then
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, new[]
            {
                new Tuple<string, LogLevelConstant>("Samenvoegen van trajectinformatie is gestart.", LogLevelConstant.Info),
                new Tuple<string, LogLevelConstant>("Samenvoegen van trajectinformatie is gelukt.", LogLevelConstant.Info)
            });
            mocks.VerifyAll();
        }

        [Test]
        public void GivenMatchedAssessmentSection_WhenMergeHandlerThrowsException_ThenMergeFailedAndLogged()
        {
            // Given
            var originalAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var assessmentSectionToMerge = new AssessmentSection(AssessmentSectionComposition.Dike);
            var mergeData = new AssessmentSectionMergeData(assessmentSectionToMerge, new AssessmentSectionMergeData.ConstructionProperties());

            var mocks = new MockRepository();
            var filePathProvider = mocks.StrictMock<IAssessmentSectionMergeFilePathProvider>();
            filePathProvider.Expect(helper => helper.GetFilePath()).Return(string.Empty);
            var assessmentSectionProvider = mocks.StrictMock<IAssessmentSectionProvider>();
            assessmentSectionProvider.Expect(asp => asp.GetAssessmentSections(null)).IgnoreArguments()
                                     .Return(new[]
                                     {
                                         assessmentSectionToMerge
                                     });
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            comparer.Expect(c => c.Compare(originalAssessmentSection, assessmentSectionToMerge)).Return(true);
            var mergeDataProvider = mocks.StrictMock<IAssessmentSectionMergeDataProvider>();
            mergeDataProvider.Expect(mdp => mdp.GetMergeData(null)).IgnoreArguments().Return(mergeData);
            var mergeHandler = mocks.StrictMock<IAssessmentSectionMergeHandler>();
            mergeHandler.Expect(mh => mh.PerformMerge(originalAssessmentSection, mergeData)).Throw(new Exception());
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(filePathProvider, assessmentSectionProvider, comparer, mergeDataProvider, mergeHandler);

            // When
            Action call = () => merger.StartMerge(originalAssessmentSection);

            // Then
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Assert.AreEqual(3, messages.Count());

                Assert.AreEqual("Samenvoegen van trajectinformatie is gestart.", messages.ElementAt(0).Item1);

                Tuple<string, Level, Exception> expectedLog = messages.ElementAt(1);
                Assert.AreEqual("Er is een onverwachte fout opgetreden tijdens het samenvoegen van de trajecten.", expectedLog.Item1);
                Assert.AreEqual(Level.Error, expectedLog.Item2);
                Exception loggedException = expectedLog.Item3;
                Assert.IsInstanceOf<Exception>(loggedException);

                Assert.AreEqual("Samenvoegen van trajectinformatie is mislukt.", messages.ElementAt(2).Item1);
            });
            mocks.VerifyAll();
        }
    }
}
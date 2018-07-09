﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Service.Comparers;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class AssessmentSectionMergerTest
    {
        [Test]
        public void Constructor_InquiryHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(null, (path, owner) => {}, comparer);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiryHandler", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetAssessmentSectionsActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(inquiryHelper, null, comparer);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getAssessmentSectionsAction", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ComparerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(inquiryHelper, (path, owner) => {}, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("comparer", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void StartMerge_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(inquiryHelper, (path, owner) => {}, comparer);

            // Call
            TestDelegate call = () => merger.StartMerge(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void StartMerge_FilePathNull_AbortAndShowCancelMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.GetSourceFileLocation(null)).IgnoreArguments().Return(null);
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(inquiryHelper, (path, owner) => {}, comparer);

            // Call
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Importeren van gegevens is geannuleerd.", LogLevelConstant.Info), 1);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenValidFilePath_WhenGetAssessmentSectionActionReturnNull_ThenAbort()
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.GetSourceFileLocation(null)).IgnoreArguments().Return(string.Empty);
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(inquiryHelper, (path, owner) => {}, comparer);

            // When
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Then
            TestHelper.AssertLogMessagesCount(call, 0);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenValidFilePath_WhenAssessmentSectionProviderReturnEmptyCollection_ThenLogErrorAndAbort()
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.GetSourceFileLocation(null)).IgnoreArguments().Return(string.Empty);
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            mocks.ReplayAll();

            Action<string, AssessmentSectionsOwner> getAssessmentSectionsAction = (path, owner) =>
            {
                owner.AssessmentSections = Enumerable.Empty<AssessmentSection>();
            };

            var merger = new AssessmentSectionMerger(inquiryHelper, getAssessmentSectionsAction, comparer);

            // When
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Then
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Er zijn geen trajecten gevonden die samengevoegd kunnen worden.", LogLevelConstant.Error), 1);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSection_WhenComparerReturnFalse_ThenLogErrorAndAbort()
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.GetSourceFileLocation(null)).IgnoreArguments().Return(string.Empty);
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            comparer.Expect(c => c.Compare(null, null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();

            Action<string, AssessmentSectionsOwner> getAssessmentSectionsAction = (path, owner) =>
            {
                owner.AssessmentSections = new []
                {
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                };
            };

            var merger = new AssessmentSectionMerger(inquiryHelper, getAssessmentSectionsAction, comparer);

            // When
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Then
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Er zijn geen trajecten gevonden die samengevoegd kunnen worden.", LogLevelConstant.Error), 1);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenAssessmentSection_WhenComparerReturnTrue_ThenContinue()
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.GetSourceFileLocation(null)).IgnoreArguments().Return(string.Empty);
            var comparer = mocks.StrictMock<IAssessmentSectionMergeComparer>();
            comparer.Expect(c => c.Compare(null, null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();

            Action<string, AssessmentSectionsOwner> getAssessmentSectionsAction = (path, owner) =>
            {
                owner.AssessmentSections = new []
                {
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                };
            };

            var merger = new AssessmentSectionMerger(inquiryHelper, getAssessmentSectionsAction, comparer);

            // When
            Action call = () => merger.StartMerge(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Then
            TestHelper.AssertLogMessagesCount(call, 0);
            mocks.VerifyAll();
        }
    }
}
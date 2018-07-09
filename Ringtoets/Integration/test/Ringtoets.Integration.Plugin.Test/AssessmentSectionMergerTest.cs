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
using Core.Common.Gui;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class AssessmentSectionMergerTest
    {
        [Test]
        public void Constructor_InquiryHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMerger(null, (path, owner) => {});

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiryHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_GetAssessmentSectionsActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionMerger(inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getAssessmentSectionsAction", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void StartMerge_FilePathNull_AbortAndShowCancelMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.GetSourceFileLocation(null)).IgnoreArguments().Return(null);
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(inquiryHelper, (path, owner) => {});

            // Call
            Action call = () => merger.StartMerge();

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Importeren van gegevens is geannuleerd.", LogLevelConstant.Info), 1);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenValidFilePath_WhenGetAssessmentSectionActionReturnNull_Abort()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.GetSourceFileLocation(null)).IgnoreArguments().Return(string.Empty);
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(inquiryHelper, (path, owner) => {});

            // Call
            Action call = () => merger.StartMerge();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenValidFilePath_WhenAssessmentSectionProviderReturnEmptyCollection_LogErrorAndAbort()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(helper => helper.GetSourceFileLocation(null)).IgnoreArguments().Return(string.Empty);
            mocks.ReplayAll();

            var merger = new AssessmentSectionMerger(inquiryHelper, (path, owner) => { owner.AssessmentSections = Enumerable.Empty<AssessmentSection>(); });

            // Call
            Action call = () => merger.StartMerge();

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>("Er zijn geen trajecten gevonden die samengevoegd kunnen worden.", LogLevelConstant.Error), 1);
            mocks.VerifyAll();
        }
    }
}
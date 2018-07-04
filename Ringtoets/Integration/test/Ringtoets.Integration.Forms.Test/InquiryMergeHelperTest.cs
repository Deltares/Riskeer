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
using Core.Common.Gui;
using NUnit.Framework;
using Ringtoets.Integration.Forms.Merge;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class InquiryMergeHelperTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var helper = new InquiryMergeHelper();

            // Assert
            Assert.IsInstanceOf<IInquiryHelper>(helper);
        }

        [Test]
        public void GetSourceFileLocation_Always_ThrowsNotImplementedException()
        {
            // Setup
            var helper = new InquiryMergeHelper();

            // Call
            TestDelegate test = () => helper.GetSourceFileLocation();

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void GetSourceFileLocationWithFileFilter_Always_ReturnNull()
        {
            // Setup
            var helper = new InquiryMergeHelper();

            // Call
            string fileLocation = helper.GetSourceFileLocation(null);

            // Assert
            Assert.IsNull(fileLocation);
        }

        [Test]
        public void GetTargetFileLocation_Always_ThrowsNotImplementedException()
        {
            // Setup
            var helper = new InquiryMergeHelper();

            // Call
            TestDelegate test = () => helper.GetTargetFileLocation();

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void GetTargetFileLocationWithParameters_Always_ThrowsNotImplementedException()
        {
            // Setup
            var helper = new InquiryMergeHelper();

            // Call
            TestDelegate test = () => helper.GetTargetFileLocation(null, null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void InquireContinuation_Always_ThrowsNotImplementedException()
        {
            // Setup
            var helper = new InquiryMergeHelper();

            // Call
            TestDelegate test = () => helper.InquireContinuation(null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void InquirePerformOptionalStep_Always_ThrowsNotImplementedException()
        {
            // Setup
            var helper = new InquiryMergeHelper();

            // Call
            TestDelegate test = () => helper.InquirePerformOptionalStep(null, null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }
    }
}
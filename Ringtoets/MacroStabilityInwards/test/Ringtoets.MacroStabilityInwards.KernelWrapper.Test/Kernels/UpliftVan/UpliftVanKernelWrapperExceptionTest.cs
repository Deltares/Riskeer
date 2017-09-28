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
using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Kernels.UpliftVan
{
    [TestFixture]
    public class UpliftVanKernelWrapperExceptionTest
        : CustomExceptionDesignGuidelinesTestFixture<UpliftVanKernelWrapperException, Exception>
    {
        [Test]
        public void MessagesListConstructor_ExpectedValues()
        {
            // Setup
            var messages = new List<string>
            {
                "Message 1",
                "Message 2"
            };

            // Call
            var exception = new UpliftVanKernelWrapperException(messages);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            CollectionAssert.AreEqual(messages, exception.Messages);
            Assert.IsNull(exception.HelpLink);
            Assert.IsNull(exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
            CollectionAssert.IsEmpty(exception.Data);
        }

        [Test]
        public void InnerExceptionConstructor_ExpectedValues()
        {
            // Setup
            var innerException = new Exception();

            // Call
            var exception = new UpliftVanKernelWrapperException(innerException);

            // Assert
            AssertMessageAndInnerExceptionConstructedInstance(exception, $"Exception of type '{typeof(UpliftVanKernelWrapperException).FullName}' was thrown.", innerException);
        }
    }
}
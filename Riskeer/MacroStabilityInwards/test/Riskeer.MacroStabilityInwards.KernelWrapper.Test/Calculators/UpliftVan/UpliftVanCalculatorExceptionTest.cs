// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan
{
    [TestFixture]
    public class UpliftVanCalculatorExceptionTest : CustomExceptionDesignGuidelinesTestFixture<UpliftVanCalculatorException, Exception>
    {
        [Test]
        public void MessageAndInnerExceptionAndKernelMessagesConstructor_ExpectedValues()
        {
            // Setup
            const string messageText = "Message";
            var innerException = new Exception();
            IEnumerable<MacroStabilityInwardsKernelMessage> kernelMessages = Enumerable.Empty<MacroStabilityInwardsKernelMessage>();

            // Call
            var exception = new UpliftVanCalculatorException(messageText, innerException, kernelMessages);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(messageText, exception.Message);
            Assert.IsNull(exception.HelpLink);
            Assert.AreEqual(innerException, exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.AreSame(kernelMessages, exception.KernelMessages);
        }
    }
}
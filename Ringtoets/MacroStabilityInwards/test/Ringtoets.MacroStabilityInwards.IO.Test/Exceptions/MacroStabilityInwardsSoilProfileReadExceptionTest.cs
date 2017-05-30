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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Exceptions
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileReadExceptionTest :
        CustomExceptionDesignGuidelinesTestFixture<MacroStabilityInwardsSoilProfileReadException, Exception>
    {
        [Test]
        public void TypeParameterAndMessageConstructor_ExpectedValues()
        {
            // Setup
            const string profileName = "<Name of profile>";
            const string messageText = "<insert exception message>";

            // Call
            var exception = new MacroStabilityInwardsSoilProfileReadException(messageText, profileName);

            // Assert
            base.AssertMessageConstructedInstance(exception, messageText, false);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreEqual(1, exception.Data.Count);
            Assert.AreEqual(exception.ProfileName, exception.Data[nameof(exception.ProfileName)]);
        }

        [Test]
        public void TypeParameterAndMessageAndInnerExceptionConstructor_ExpectedValues()
        {
            // Setup
            var innerException = new Exception();
            const string profileName = "<Name of profile>";
            const string messageText = "<insert exception message>";

            // Call
            var exception = new MacroStabilityInwardsSoilProfileReadException(messageText, profileName, innerException);

            // Assert
            AssertMessageAndInnerExceptionConstructedInstance(exception, messageText, innerException, false);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreEqual(1, exception.Data.Count);
            Assert.AreEqual(exception.ProfileName, exception.Data[nameof(exception.ProfileName)]);
        }

        protected override void AssertDefaultConstructedInstance(MacroStabilityInwardsSoilProfileReadException exception)
        {
            base.AssertDefaultConstructedInstance(exception);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.IsNull(exception.ProfileName);
        }

        protected override void AssertMessageConstructedInstance(MacroStabilityInwardsSoilProfileReadException exception, string messageText,
                                                                 bool assertData = true)
        {
            base.AssertMessageConstructedInstance(exception, messageText, assertData);
            if (assertData)
            {
                Assert.IsNull(exception.ProfileName);
            }
        }

        protected override void AssertMessageAndInnerExceptionConstructedInstance(MacroStabilityInwardsSoilProfileReadException exception, string messageText,
                                                                                  Exception innerException, bool assertData = true)
        {
            base.AssertMessageAndInnerExceptionConstructedInstance(exception, messageText, innerException, assertData);
            if (assertData)
            {
                Assert.IsNull(exception.ProfileName);
            }
        }

        protected override MacroStabilityInwardsSoilProfileReadException CreateFullyConfiguredException()
        {
            var originalInnerException = new Exception("inner");
            return new MacroStabilityInwardsSoilProfileReadException("<message>", "<parameter>", originalInnerException);
        }

        protected override void AssertRoundTripResult(MacroStabilityInwardsSoilProfileReadException originalException, MacroStabilityInwardsSoilProfileReadException persistedException)
        {
            base.AssertRoundTripResult(originalException, persistedException);
            Assert.AreEqual(originalException.ProfileName, persistedException.ProfileName);
        }
    }
}
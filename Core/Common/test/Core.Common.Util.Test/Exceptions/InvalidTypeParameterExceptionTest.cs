// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.TestUtil;
using Core.Common.Util.Exceptions;
using NUnit.Framework;

namespace Core.Common.Util.Test.Exceptions
{
    [TestFixture]
    public class InvalidTypeParameterExceptionTest :
        CustomExceptionDesignGuidelinesTestFixture<InvalidTypeParameterException, Exception>
    {
        [Test]
        public void TypeParameterAndMessageConstructor_ExpectedValues()
        {
            // Setup
            const string typeParamName = "T";
            const string messageText = "<insert exception message>";

            // Call
            var exception = new InvalidTypeParameterException(messageText, typeParamName);

            // Assert
            base.AssertMessageConstructedInstance(exception, messageText, false);
            Assert.AreEqual(typeParamName, exception.TypeParamName);
            Assert.AreEqual(1, exception.Data.Count);
            Assert.AreEqual(exception.TypeParamName, exception.Data[nameof(exception.TypeParamName)]);
        }

        [Test]
        public void TypeParameterAndMessageAndInnerExceptionConstructor_ExpectedValues()
        {
            // Setup
            var innerException = new Exception();
            const string typeParamName = "T";
            const string messageText = "<insert exception message>";

            // Call
            var exception = new InvalidTypeParameterException(messageText, typeParamName, innerException);

            // Assert
            AssertMessageAndInnerExceptionConstructedInstance(exception, messageText, innerException, false);
            Assert.AreEqual(typeParamName, exception.TypeParamName);
            Assert.AreEqual(1, exception.Data.Count);
            Assert.AreEqual(exception.TypeParamName, exception.Data[nameof(exception.TypeParamName)]);
        }

        protected override void AssertDefaultConstructedInstance(InvalidTypeParameterException exception)
        {
            base.AssertDefaultConstructedInstance(exception);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.IsNull(exception.TypeParamName);
        }

        protected override void AssertMessageConstructedInstance(InvalidTypeParameterException exception, string messageText,
                                                                 bool assertData = true)
        {
            base.AssertMessageConstructedInstance(exception, messageText, assertData);
            if (assertData)
            {
                Assert.IsNull(exception.TypeParamName);
            }
        }

        protected override void AssertMessageAndInnerExceptionConstructedInstance(InvalidTypeParameterException exception, string messageText,
                                                                                  Exception innerException, bool assertData = true)
        {
            base.AssertMessageAndInnerExceptionConstructedInstance(exception, messageText, innerException, assertData);
            if (assertData)
            {
                Assert.IsNull(exception.TypeParamName);
            }
        }

        protected override InvalidTypeParameterException CreateFullyConfiguredException()
        {
            var originalInnerException = new Exception("inner");
            return new InvalidTypeParameterException("<message>", "<parameter>", originalInnerException);
        }

        protected override void AssertRoundTripResult(InvalidTypeParameterException originalException, InvalidTypeParameterException persistedException)
        {
            base.AssertRoundTripResult(originalException, persistedException);
            Assert.AreEqual(originalException.TypeParamName, persistedException.TypeParamName);
        }
    }
}
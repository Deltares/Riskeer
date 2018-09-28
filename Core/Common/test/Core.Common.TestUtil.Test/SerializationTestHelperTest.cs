// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class SerializationTestHelperTest
    {
        [Test]
        public void SerializeAndDeserializeException_Exception_ReturnProperlyInitializedException()
        {
            // Setup
            var originalInnerException = new Exception("inner");
            var originalException = new Exception("outer", originalInnerException);

            // Precondition
            Assert.IsNotNull(originalException.InnerException);
            Assert.IsNull(originalException.InnerException.InnerException);

            // Call
            Exception deserializedException = SerializationTestHelper.SerializeAndDeserializeException(originalException);

            // Assert
            Assert.AreNotSame(originalException, deserializedException);
            Assert.AreEqual(originalException.Message, deserializedException.Message);

            Assert.IsNotNull(deserializedException.InnerException);
            Assert.AreEqual(originalException.InnerException.GetType(), deserializedException.InnerException.GetType());
            Assert.AreEqual(originalException.InnerException.Message, deserializedException.InnerException.Message);

            Assert.IsNull(deserializedException.InnerException.InnerException);
        }
    }
}
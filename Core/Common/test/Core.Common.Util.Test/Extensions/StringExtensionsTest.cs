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

using Core.Common.Util.Extensions;
using NUnit.Framework;

namespace Core.Common.Util.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [Test]
        public void DeepClone_StringIsNull_ReturnNull()
        {
            // Setup
            const string original = null;

            // Call
            string result = original.DeepClone();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void DeepClone_StringIsNotNull_ReturnEqualStringWithNewReference()
        {
            // Setup
            const string original = "I'm a pretty string!";

            // Call
            string result = original.DeepClone();

            // Assert
            Assert.AreNotSame(original, result);
            Assert.AreEqual(original, result);
        }

        [Test]
        public void FirstToUpper_StringIsNull_ReturnNull()
        {
            // Setup
            const string str = null;

            // Call
            string result = str.FirstToUpper();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("t", "T")]
        [TestCase("T", "T")]
        [TestCase("test", "Test")]
        [TestCase("Test", "Test")]
        [TestCase("tesT Test", "TesT Test")]
        [TestCase("TesT Test", "TesT Test")]
        public void FirstToUpper_StringIsNotNull_ReturnStringWithFirstLetterUpperCase(string str, string expectedResult)
        {
            // Call
            string result = str.FirstToUpper();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void FirstToLower_StringIsNull_ReturnNull()
        {
            // Setup
            const string str = null;

            // Call
            string result = str.FirstToLower();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("t", "t")]
        [TestCase("T", "t")]
        [TestCase("test", "test")]
        [TestCase("Test", "test")]
        [TestCase("tesT Test", "tesT Test")]
        [TestCase("TesT Test", "tesT Test")]
        public void FirstToLower_StringIsNotNull_ReturnStringWithFirstLetterLowerCase(string str, string expectedResult)
        {
            // Call
            string result = str.FirstToLower();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
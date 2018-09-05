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

using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.Helpers;

namespace Ringtoets.AssemblyTool.IO.Test.Model.Helpers
{
    [TestFixture]
    public class SerializableIdValidatorTest
    {
        [Test]
        [TestCase("AValidId1-2.3")]
        [TestCase("_AValidId1-2.3")]
        [TestCase("aValidId1-2.3")]
        public void Validate_WithValidIds_ReturnsTrue(string validId)
        {
            // Call
            bool result = SerializableIdValidator.Validate(validId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("-invalidId1-2.3")]
        [TestCase("1nvalidId1-2.3")]
        [TestCase(".invalidId1-2.3")]
        [TestCase("invalidId#")]
        [TestCase("invalid#Id")]
        [TestCase("#invalidId")]
        [TestCase("i nvalidId")]
        [TestCase(" invalidId")]
        [TestCase("invalidId ")]
        [TestCase("i\rnvalidId")]
        [TestCase("\rinvalidId")]
        [TestCase("invalidId\r")]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void Validate_WithInvalidIds_ReturnsFalse(string invalidId)
        {
            // Call
            bool result = SerializableIdValidator.Validate(invalidId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
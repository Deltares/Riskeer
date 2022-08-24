// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Helpers;

namespace Riskeer.AssemblyTool.IO.Test.Helpers
{
    [TestFixture]
    public class IdValidationHelperTest
    {
        [Test]
        [TestCase("AValidId1-2.3")]
        [TestCase("_AValidId1-2.3")]
        [TestCase("aValidId1-2.3")]
        public void ThrowIfInvalid_WithValidIds_DoesNotThrow(string validId)
        {
            // Call & Assert
            IdValidationHelper.ThrowIfInvalid(validId);
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
        public void Validate_WithInvalidIds_ThrowsArgumentException(string invalidId)
        {
            // Call
            void Call() => IdValidationHelper.ThrowIfInvalid(invalidId);

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }
    }
}
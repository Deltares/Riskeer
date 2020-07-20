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

using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class EnumDisplayWrapperHelperTest
    {
        [Test]
        public void GetEnumTypes_Always_ReturnsCorrectEnumDisplayWrappers()
        {
            // Call
            EnumDisplayWrapper<TestEnumType>[] displayWrappers = EnumDisplayWrapperHelper.GetEnumTypes<TestEnumType>();

            // Assert
            Assert.AreEqual(3, displayWrappers.Length);
            Assert.AreEqual("Type1", displayWrappers[0].DisplayName);
            Assert.AreEqual(TestEnumType.Type1, displayWrappers[0].Value);
            Assert.AreEqual("Type2", displayWrappers[1].DisplayName);
            Assert.AreEqual(TestEnumType.Type2, displayWrappers[1].Value);
            Assert.AreEqual("Type3", displayWrappers[2].DisplayName);
            Assert.AreEqual(TestEnumType.Type3, displayWrappers[2].Value);
        }

        private enum TestEnumType
        {
            Type1 = 1,
            Type2 = 2,
            Type3 = 3
        }
    }
}
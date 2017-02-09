// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Common.Utils.Attributes;
using NUnit.Framework;

namespace Core.Common.TestUtil
{
    [TestFixture]
    public abstract class EnumTestFixture<TEnumType>
    {
        protected abstract IDictionary<TEnumType, string> ExpectedDisplayNameForEnumValues { get; }

        [Test]
        public void DisplayName_Always_ReturnExpectedValues()
        {
            foreach (TEnumType value in Enum.GetValues(typeof(TEnumType)))
            {
                Assert.AreEqual(ExpectedDisplayNameForEnumValues[value], GetDisplayName(value),
                                $"Display name for {value} incorrect.");
            }
        }

        private static string GetDisplayName(TEnumType value)
        {
            var type = typeof(TEnumType);
            MemberInfo[] memInfo = type.GetMember(value.ToString());
            object[] attributes = memInfo[0].GetCustomAttributes(typeof(ResourcesDisplayNameAttribute), false);
            if (attributes.Length > 0)
            {
                return ((ResourcesDisplayNameAttribute) attributes[0]).DisplayName;
            }
            else
            {
                return null;
            }
        }
    }
}
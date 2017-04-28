// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.TestUtil.Test.Properties;
using Core.Common.Utils.Attributes;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class EnumTestFixtureTest : EnumTestFixture<EnumTestFixtureTest.DisplayNameTestEnum>
    {
        protected override IDictionary<DisplayNameTestEnum, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<DisplayNameTestEnum, string>
                {
                    {
                        DisplayNameTestEnum.NoDisplayName, null
                    },
                    {
                        DisplayNameTestEnum.HasResourcesDisplayName, Resources.SomeDisplayName
                    }
                };
            }
        }

        protected override IDictionary<DisplayNameTestEnum, byte> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<DisplayNameTestEnum, byte>
                {
                    {
                        DisplayNameTestEnum.NoDisplayName, 0
                    },
                    {
                        DisplayNameTestEnum.HasResourcesDisplayName, 4
                    }
                };
            }
        }

        public enum DisplayNameTestEnum
        {
            NoDisplayName,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.SomeDisplayName))]
            HasResourcesDisplayName = 4
        }
    }
}
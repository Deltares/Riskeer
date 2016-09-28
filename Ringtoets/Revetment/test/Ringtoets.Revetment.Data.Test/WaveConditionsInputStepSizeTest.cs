// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Utils.Attributes;
using NUnit.Framework;

namespace Ringtoets.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputStepSizeTest
    {
        [Test]
        public void DisplayName_Always_ReturnExpectedValues()
        {
            // Assert
            Assert.AreEqual("0.5", GetDisplayName(WaveConditionsInputStepSize.Half));
            Assert.AreEqual("1.0", GetDisplayName(WaveConditionsInputStepSize.One));
            Assert.AreEqual("2.0", GetDisplayName(WaveConditionsInputStepSize.Two));
        }

        private string GetDisplayName(WaveConditionsInputStepSize value)
        {
            var type = typeof(WaveConditionsInputStepSize);
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(ResourcesDisplayNameAttribute), false);
            return ((ResourcesDisplayNameAttribute) attributes[0]).DisplayName;
        }
    }
}
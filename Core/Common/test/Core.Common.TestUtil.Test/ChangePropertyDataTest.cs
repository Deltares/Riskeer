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
    public class ChangePropertyDataTest
    {
        [Test]
        public void Constructor_ActionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ChangePropertyData<object>(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("actionToChangeProperty", exception.ParamName);
        }

        [Test]
        public void Constructor_PropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ChangePropertyData<object>(o => {}, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("propertyName", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            Action<object> actionToChangeProperty = o => {};
            const string propertyName = "name";

            // Call
            var data = new ChangePropertyData<object>(actionToChangeProperty, propertyName);

            // Assert
            Assert.AreSame(actionToChangeProperty, data.ActionToChangeProperty);
            Assert.AreEqual(propertyName, data.PropertyName);
        }
    }
}
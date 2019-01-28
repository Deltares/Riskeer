// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class SelectableRowTest
    {
        [Test]
        public void Constructor_WithoutItem_ThrowsArgumentNullException()
        {
            // Setup
            const string name = "name";

            // Call
            TestDelegate test = () => new SelectableRow<object>(null, name);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("item", paramName);
        }

        [Test]
        public void Constructor_WithoutName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SelectableRow<object>(new object(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("name", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_SetPropertyAsExpected()
        {
            // Setup
            const string name = "name";
            var item = new object();

            // Call
            var row = new SelectableRow<object>(item, name);

            // Assert
            Assert.IsFalse(row.Selected);
            Assert.AreEqual(name, row.Name);
            Assert.AreSame(item, row.Item);
        }
    }
}
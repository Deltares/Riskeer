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

using System;
using NUnit.Framework;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class WmtsCapabilityRowTest
    {
        [Test]
        public void Constructor_WmtsCapabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsCapabilityRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wmtsCapability", paramName);
        }

        [Test]
        public void Constructor_ValidWmtsCapability_ExpectedProperties()
        {
            // Setup
            const string id = "laag1(abc)";
            const string format = "image/png";
            const string title = "Eerste kaartlaag";
            const string coordinateSystem = "Coördinatenstelsel";
            var wmtsCapability = new WmtsCapability(id, format, title, coordinateSystem);

            // Call
            var row = new WmtsCapabilityRow(wmtsCapability);

            // Assert
            Assert.AreEqual(id, row.Id);
            Assert.AreEqual(format, row.Format);
            Assert.AreEqual(title, row.Title);
            Assert.AreEqual(coordinateSystem, row.CoordinateSystem);
        }
    }
}
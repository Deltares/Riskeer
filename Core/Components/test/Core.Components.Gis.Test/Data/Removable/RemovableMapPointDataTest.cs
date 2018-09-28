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

using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Removable;
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data.Removable
{
    [TestFixture]
    public class RemovableMapPointDataTest
    {
        [Test]
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Call
            var data = new RemovableMapPointData("test data");

            // Assert
            Assert.IsInstanceOf<MapPointData>(data);
            Assert.IsInstanceOf<IRemovable>(data);
        }

        [Test]
        public void Constructor_WithStyle_ExpectedValues()
        {
            // Setup
            var style = new PointStyle();

            // Call
            var data = new RemovableMapPointData("test data", style);

            // Assert
            Assert.IsInstanceOf<MapPointData>(data);
            Assert.IsInstanceOf<IRemovable>(data);
        }
    }
}
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

using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.IO.Structures;

namespace Riskeer.Common.IO.Test.Structures
{
    [TestFixture]
    public class StructureLocationTest
    {
        private const string id = "anId";
        private const string name = "aName";
        private readonly Point2D point = new Point2D(0, 0);

        [Test]
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StructureLocation(null, name, point);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("id", paramName);
        }

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StructureLocation(id, null, point);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("name", paramName);
        }

        [Test]
        public void Constructor_PointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StructureLocation(id, name, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("point", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedProperties()
        {
            // Call
            var structure = new StructureLocation(id, name, point);

            // Assert
            Assert.AreEqual("anId", structure.Id);
            Assert.AreEqual("aName", structure.Name);
            Assert.AreEqual(new Point2D(0, 0), structure.Point);
        }
    }
}
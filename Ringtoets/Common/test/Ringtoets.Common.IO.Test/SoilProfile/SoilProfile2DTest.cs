// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfile2DTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var soilLayer2Ds = new[]
            {
                new SoilLayer2D()
            };

            // Call
            TestDelegate test = () => new SoilProfile2D(1, null, soilLayer2Ds);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_LayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SoilProfile2D(1, "name", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("layers", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            const long id = 12;
            const string name = "some name";
            var soilLayer2Ds = new[]
            {
                new SoilLayer2D()
            };

            // Call
            var soilProfile2D = new SoilProfile2D(id, name, soilLayer2Ds);

            // Assert
            Assert.IsInstanceOf<ISoilProfile>(soilProfile2D);
            Assert.AreEqual(id, soilProfile2D.Id);
            Assert.AreEqual(name, soilProfile2D.Name);
            CollectionAssert.AreEqual(soilLayer2Ds, soilProfile2D.Layers);
            Assert.IsNaN(soilProfile2D.IntersectionX);
        }
    }
}
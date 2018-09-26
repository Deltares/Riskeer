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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfile2DTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            SoilLayer2D[] soilLayer2Ds =
            {
                SoilLayer2DTestFactory.CreateSoilLayer2D()
            };

            // Call
            TestDelegate test = () => new SoilProfile2D(1, null, soilLayer2Ds, Enumerable.Empty<PreconsolidationStress>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_LayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SoilProfile2D(1, "name", null, Enumerable.Empty<PreconsolidationStress>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("layers", exception.ParamName);
        }

        [Test]
        public void Constructor_PreconsolidationStressesNull_ThrowsArgumentNullException()
        {
            // Setup
            SoilLayer2D[] soilLayer2Ds =
            {
                SoilLayer2DTestFactory.CreateSoilLayer2D()
            };

            // Call
            TestDelegate call = () => new SoilProfile2D(1, string.Empty, soilLayer2Ds, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preconsolidationStresses", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            const long id = 12;
            const string name = "some name";
            IEnumerable<SoilLayer2D> soilLayer2Ds = Enumerable.Empty<SoilLayer2D>();
            IEnumerable<PreconsolidationStress> stresses = Enumerable.Empty<PreconsolidationStress>();

            // Call
            var soilProfile2D = new SoilProfile2D(id, name, soilLayer2Ds, stresses);

            // Assert
            Assert.IsInstanceOf<ISoilProfile>(soilProfile2D);
            Assert.AreEqual(id, soilProfile2D.Id);
            Assert.AreEqual(name, soilProfile2D.Name);
            Assert.AreSame(soilLayer2Ds, soilProfile2D.Layers);
            Assert.AreSame(stresses, soilProfile2D.PreconsolidationStresses);
            Assert.IsNaN(soilProfile2D.IntersectionX);
        }
    }
}
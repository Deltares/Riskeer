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

using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfile1DTest
    {
        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            const long id = 123;
            const string name = "some name";
            const int bottom = 1;
            var soilLayer1Ds = new[]
            {
                new SoilLayer1D(2)
            };

            // Call
            var soilProfile1D = new SoilProfile1D(id, name, bottom, soilLayer1Ds);

            // Assert
            Assert.IsInstanceOf<ISoilProfile>(soilProfile1D);
            Assert.AreEqual(id, soilProfile1D.Id);
            Assert.AreEqual(name, soilProfile1D.Name);
            Assert.AreEqual(bottom, soilProfile1D.Bottom);
            CollectionAssert.AreEqual(soilLayer1Ds, soilProfile1D.Layers);
        }
    }
}
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
    public class SoilProfile1DTest
    {
        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            const string name = "some name";
            var random = new Random(9);
            int bottom = random.Next();

            // Call
            var soilProfile1D = new SoilProfile1D(name, bottom);

            // Assert
            Assert.IsInstanceOf<ISoilProfile>(soilProfile1D);
            Assert.AreEqual(name, soilProfile1D.Name);
            Assert.AreEqual(bottom, soilProfile1D.Bottom);
        }
    }
}
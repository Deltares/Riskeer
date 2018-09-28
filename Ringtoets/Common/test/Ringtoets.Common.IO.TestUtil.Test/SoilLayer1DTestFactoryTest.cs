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
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.TestUtil.Test
{
    public class SoilLayer1DTestFactoryTest
    {
        [Test]
        public void CreateSoilLayer1DWithValidAquifer_ParameterLess_ReturnsExpectedProperties()
        {
            // Call
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();

            // Assert
            Assert.AreEqual(3.14, layer.Top);
            Assert.AreEqual(0.0, layer.IsAquifer);
        }

        [Test]
        public void CreateSoilLayer1DWithValidAquifer_WithTop_ReturnsExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            double top = random.NextDouble();

            // Call
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer(top);

            // Assert
            Assert.AreEqual(top, layer.Top);
            Assert.AreEqual(0.0, layer.IsAquifer);
        }
    }
}
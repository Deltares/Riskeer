﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.IO.Builders;

namespace Ringtoets.Piping.IO.Test.Builders
{
    [TestFixture]
    public class GenericSoilLayerParametersTest
    {
        [Test]
        public void DefaultConstructor_SetsDefaultProperties()
        {
            // Call
            var parameters = new TestGenericSoilLayerParameters();

            // Assert
            Assert.IsNull(parameters.Color);
            Assert.IsNull(parameters.MaterialName);
            Assert.IsNull(parameters.BelowPhreaticLevelDeviation);
            Assert.IsNull(parameters.BelowPhreaticLevelDistribution);
            Assert.IsNull(parameters.BelowPhreaticLevelMean);
            Assert.IsNull(parameters.BelowPhreaticLevelShift);
            Assert.IsNull(parameters.DiameterD70Deviation);
            Assert.IsNull(parameters.DiameterD70Distribution);
            Assert.IsNull(parameters.DiameterD70Mean);
            Assert.IsNull(parameters.DiameterD70Shift);
            Assert.IsNull(parameters.PermeabilityDeviation);
            Assert.IsNull(parameters.PermeabilityMean);
            Assert.IsNull(parameters.PermeabilityDistribution);
            Assert.IsNull(parameters.PermeabilityShift);
        }

        private class TestGenericSoilLayerParameters : GenericSoilLayerParameters {}
    }
}
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
using Ringtoets.Common.IO.Hydraulics;
using Ringtoets.Integration.Plugin.MetaDataAttributeNameProviders;

namespace Ringtoets.Integration.Plugin.Test.MetaDataAttributeNameProviders
{
    [TestFixture]
    public class HydraulicBoundaryLocationMetaDataAttributeNameProviderTest
    {
        [Test]
        public void Constructor_Always_ReturnsExpectedValues()
        {
            // Call
            var provider = new HydraulicBoundaryLocationMetaDataAttributeNameProvider();

            // Assert
            Assert.IsInstanceOf<IHydraulicBoundaryLocationMetaDataAttributeNameProvider>(provider);

            Assert.AreEqual("h(A+->A)", provider.DesignWaterLevelCalculation1AttributeName);
            Assert.AreEqual("h(A->B)", provider.DesignWaterLevelCalculation2AttributeName);
            Assert.AreEqual("h(B->C)", provider.DesignWaterLevelCalculation3AttributeName);
            Assert.AreEqual("h(C->D)", provider.DesignWaterLevelCalculation4AttributeName);
            Assert.AreEqual("Hs(A+->A)", provider.WaveHeightCalculation1AttributeName);
            Assert.AreEqual("Hs(A->B)", provider.WaveHeightCalculation2AttributeName);
            Assert.AreEqual("Hs(B->C)", provider.WaveHeightCalculation3AttributeName);
            Assert.AreEqual("Hs(C->D)", provider.WaveHeightCalculation4AttributeName);
        }
    }
}
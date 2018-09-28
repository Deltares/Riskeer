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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Primitives.TestUtil.Test
{
    [TestFixture]
    public class PipingSoilProfileTestFactoryTest
    {
        [Test]
        public void CreatePipingSoilProfile_ReturnsPipingSoilProfile()
        {
            // Call
            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();

            // Assert
            Assert.AreEqual("name", soilProfile.Name);
            Assert.AreEqual(0, soilProfile.Bottom);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, soilProfile.SoilProfileSourceType);

            var expectedLayer = new PipingSoilLayer(1.0);
            Assert.AreEqual(1, soilProfile.Layers.Count());
            AssertAreEqual(expectedLayer, soilProfile.Layers.Single());
        }

        [Test]
        public void CreatePipingSoilProfileWithName_ReturnsPipingSoilProfile()
        {
            // Setup
            const string name = "Some name";

            // Call
            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile(name);

            // Assert
            Assert.AreEqual(name, soilProfile.Name);
            Assert.AreEqual(0, soilProfile.Bottom);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, soilProfile.SoilProfileSourceType);

            var expectedLayer = new PipingSoilLayer(1.0);
            Assert.AreEqual(1, soilProfile.Layers.Count());
            AssertAreEqual(expectedLayer, soilProfile.Layers.Single());
        }

        [Test]
        public void CreatePipingSoilProfileWithNameAndType_ReturnsPipingSoilProfile()
        {
            // Setup
            const string name = "Some name";
            var type = new Random(123).NextEnumValue<SoilProfileType>();

            // Call
            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile(name, type);

            // Assert
            Assert.AreEqual(name, soilProfile.Name);
            Assert.AreEqual(0, soilProfile.Bottom);
            Assert.AreEqual(type, soilProfile.SoilProfileSourceType);

            var expectedLayer = new PipingSoilLayer(1.0);
            Assert.AreEqual(1, soilProfile.Layers.Count());
            AssertAreEqual(expectedLayer, soilProfile.Layers.Single());
        }

        private static void AssertAreEqual(PipingSoilLayer expected, PipingSoilLayer actual)
        {
            Assert.AreEqual(expected.Top, actual.Top);
            Assert.AreEqual(expected.MaterialName, actual.MaterialName);
            DistributionAssert.AreEqual(expected.BelowPhreaticLevel, actual.BelowPhreaticLevel);
            DistributionAssert.AreEqual(expected.DiameterD70, actual.DiameterD70);
            DistributionAssert.AreEqual(expected.Permeability, actual.Permeability);
        }
    }
}
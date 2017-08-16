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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class TestPipingSoilProfileTest
    {
        [Test]
        public void DefaultConstructor_ExpectedPropertiesSet()
        {
            // Call
            var profile = new TestPipingSoilProfile();

            // Assert
            Assert.IsEmpty(profile.Name);
            Assert.AreEqual(0.0, profile.Bottom);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, profile.SoilProfileSourceType);
            CollectionAssert.AreEquivalent(new[]
            {
                true
            }, profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEquivalent(new[]
            {
                0.0
            }, profile.Layers.Select(l => l.Top));
        }

        [Test]
        public void Constructor_WitValidName_ExpectedPropertiesSet()
        {
            // Setup
            const string name = "some name";

            // Call
            var profile = new TestPipingSoilProfile(name);

            // Assert
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(0.0, profile.Bottom);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, profile.SoilProfileSourceType);
            CollectionAssert.AreEquivalent(new[]
            {
                true
            }, profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEquivalent(new[]
            {
                0.0
            }, profile.Layers.Select(l => l.Top));
        }

        [Test]
        public void Constructor_WitValidNameAndType_ExpectedPropertiesSet()
        {
            // Setup
            const string name = "some name";
            var soilProfileType = new Random(6543).NextEnumValue<SoilProfileType>();

            // Call
            var profile = new TestPipingSoilProfile(name, soilProfileType);

            // Assert
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(0.0, profile.Bottom);
            Assert.AreEqual(soilProfileType, profile.SoilProfileSourceType);
            CollectionAssert.AreEquivalent(new[]
            {
                true
            }, profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEquivalent(new[]
            {
                0.0
            }, profile.Layers.Select(l => l.Top));
        }
    }
}
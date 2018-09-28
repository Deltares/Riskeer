// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.DotSpatial.TestUtil.Test
{
    [TestFixture]
    public class TestFeatureBasedMapDataTest
    {
        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            const string name = "A";

            // Call
            var mapData = new TestFeatureBasedMapData(name);

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapData>(mapData);
            Assert.AreEqual(name, mapData.Name);
            CollectionAssert.IsEmpty(mapData.Features);
            Assert.IsTrue(mapData.IsVisible);
            CollectionAssert.IsEmpty(mapData.MetaData);
            Assert.IsNull(mapData.SelectedMetaDataAttribute);
            Assert.IsFalse(mapData.ShowLabels);
            Assert.IsNull(mapData.MapTheme);
        }
    }
}
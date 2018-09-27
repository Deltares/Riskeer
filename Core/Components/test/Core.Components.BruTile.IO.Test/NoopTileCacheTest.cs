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

using BruTile;
using NUnit.Framework;

namespace Core.Components.BruTile.IO.Test
{
    [TestFixture]
    public class NoopTileCacheTest
    {
        [Test]
        public void Add_DoesNothing()
        {
            // Call
            TestDelegate call = () => NoopTileCache.Instance.Add(new TileIndex(), null);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Remove_DoesNothing()
        {
            // Call
            TestDelegate call = () => NoopTileCache.Instance.Remove(new TileIndex());

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Find_ReturnsNull()
        {
            // Call
            byte[] tileData = NoopTileCache.Instance.Find(new TileIndex());

            // Assert
            Assert.IsNull(tileData);
        }
    }
}
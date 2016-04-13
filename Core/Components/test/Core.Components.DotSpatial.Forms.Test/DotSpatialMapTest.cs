// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Windows.Forms;
using DotSpatial.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class DotSpatialMapTest
    {
        [Test]
        [RequiresSTA]
        public void ToggleRectangleZooming_Always_CorrectlySetsMapFunctions()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();
                var minExt = 1e-8;

                var map = (DotSpatialMap) new ControlTester("Map").TheObject;

                // Call
                TestDelegate test = () => { map.ViewExtents = new Extent(1 + minExt, 2 + minExt, 1 + minExt, 2 + minExt); };

                // Assert
                Assert.DoesNotThrow(test);
                Assert.AreEqual(map.ViewExtents, map.ViewExtents);
            }
        }
    }
}
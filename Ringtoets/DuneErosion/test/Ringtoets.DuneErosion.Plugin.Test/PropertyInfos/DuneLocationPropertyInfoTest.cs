﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PropertyClasses;

namespace Ringtoets.DuneErosion.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class DuneLocationPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(DuneLocation), info.DataType);
                Assert.AreEqual(typeof(DuneLocationProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_WithDuneLocation_SetsDuneLocationAsData()
        {
            // Setup
            var duneLocation = new DuneLocation(1, "", new Point2D(0, 0), new DuneLocation.ConstructionProperties());

            using (var plugin = new DuneErosionPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(duneLocation);

                // Assert
                Assert.IsInstanceOf<DuneLocationProperties>(objectProperties);
                Assert.AreSame(duneLocation, objectProperties.Data);
            }
        }

        private static PropertyInfo GetInfo(DuneErosionPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(DuneLocation));
        }
    }
}
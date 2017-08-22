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

using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    public class MacroStabilityInwardsSoilLayer2DPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new MacroStabilityInwardsSoilLayer2DProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsSoilLayer2D>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                             {
                                                                 new Point2D(20.210230, 26.00001),
                                                                 new Point2D(3.830, 1.040506)
                                                             }),
                                                             new Ring[0])
            {
                Properties =
                {
                    MaterialName = "Test Name",
                    IsAquifer = true
                }
            };

            // Call
            var properties = new MacroStabilityInwardsSoilLayer2DProperties
            {
                Data = layer
            };

            // Assert
            Assert.AreEqual("Test Name", properties.Name);
            Assert.IsTrue(properties.IsAquifer);
            Assert.AreEqual(layer.OuterRing.Points.ToArray(), properties.Geometry);
        }

        [Test]
        public void ToString_Always_ReturnName()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                             {
                                                                 new Point2D(20.210230, 26.00001),
                                                                 new Point2D(3.830, 1.040506)
                                                             }),
                                                             new Ring[0])
            {
                Properties =
                {
                    MaterialName = "Layer A 2D"
                }
            };

            var properties = new MacroStabilityInwardsSoilLayer2DProperties
            {
                Data = layer
            };
            // Call
            string name = properties.ToString();

            // Assert
            Assert.AreEqual("Layer A 2D", name);
        }
    }
}
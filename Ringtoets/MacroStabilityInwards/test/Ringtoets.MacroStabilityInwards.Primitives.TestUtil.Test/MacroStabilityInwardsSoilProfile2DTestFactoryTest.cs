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

using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.Primitives.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile2DTestFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsSoilProfile2D_ReturnsExpectedMacroStabilityInwardsSoilProfile2D()
        {
            // Call
            MacroStabilityInwardsSoilProfile2D soilProfile = MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D();

            // Assert
            Assert.IsNotNull(soilProfile);
            Assert.AreEqual(typeof(MacroStabilityInwardsSoilProfile2D), soilProfile.GetType());
            Assert.AreEqual("MacroStabilityInwardsSoilProfile2D", soilProfile.Name);
            CollectionAssert.AreEqual(new[]
            {
                new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }), new Ring[0]),
                new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                {
                    new Point2D(1, 1),
                    new Point2D(2, 2)
                }), new Ring[0])
            }, soilProfile.Layers);
            CollectionAssert.IsEmpty(soilProfile.PreconsolidationStresses);
        }
    }
}
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

using System;
using System.Linq;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilCreatorTest
    {
        [Test]
        public void Create_ProfileWithLayers_ReturnsProfileWithLayers()
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    UsePop = true,
                    ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.CPhi,
                    MaterialName = "Sand"
                })),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    UsePop = false,
                    ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.None,
                    MaterialName = "Mud"
                })),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    UsePop = true,
                    ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated,
                    MaterialName = "Clay"
                })),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    UsePop = true,
                    ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.SuCalculated,
                    MaterialName = "Grass"
                }))
            });

            // Call
            Soil[] soils = MacroStabilityInwardsSoilCreator.Create(profile);

            // Assert
            Assert.AreEqual(4, soils.Length);

            CollectionAssert.AreEqual(profile.LayersUnderSurfaceLine.Select(l => l.Properties.UsePop), soils.Select(s => s.UsePop));
            CollectionAssert.AreEqual(profile.LayersUnderSurfaceLine.Select(l => l.Properties.MaterialName), soils.Select(s => s.Name));
            CollectionAssert.AreEqual(new[]
            {
                ShearStrengthModel.CPhi,
                ShearStrengthModel.None,
                ShearStrengthModel.CPhiOrCuCalculated,
                ShearStrengthModel.CuCalculated, 
            }, soils.Select(s => s.ShearStrengthModel));
        }

        [Test]
        public void Create_InvalidShearStrengthModel_ThrowNotSupportedException()
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
           {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    ShearStrengthModel = (MacroStabilityInwardsShearStrengthModel) 99,
                }))
            });

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilCreator.Create(profile);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }
    }
}
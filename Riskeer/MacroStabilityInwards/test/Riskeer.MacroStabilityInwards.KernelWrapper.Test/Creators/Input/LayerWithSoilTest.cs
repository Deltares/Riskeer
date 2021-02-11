﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class LayerWithSoilTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var outerRing = new Point2D[0];
            var innerRings = new Point2D[0][];
            var soil = new Soil();
            bool isAquifer = random.NextBoolean();
            var waterPressureInterpolationModel = random.NextEnumValue<WaterPressureInterpolationModel>();

            // Call
            var layerWithSoil = new LayerWithSoil(outerRing, innerRings, soil, isAquifer, waterPressureInterpolationModel);

            // Assert
            Assert.AreSame(outerRing, layerWithSoil.OuterRing);
            Assert.AreSame(innerRings, layerWithSoil.InnerRings);
            Assert.AreSame(soil, layerWithSoil.Soil);
            Assert.AreEqual(isAquifer, layerWithSoil.IsAquifer);
            Assert.AreEqual(waterPressureInterpolationModel, layerWithSoil.WaterPressureInterpolationModel);
        }
    }
}
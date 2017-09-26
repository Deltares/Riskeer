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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Input
{
    [TestFixture]
    public class UpliftVanSoilLayerTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new UpliftVanSoilLayer(new Point2D[0], new Point2D[0][], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_OuterRingNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new UpliftVanSoilLayer(null, new Point2D[0][], new UpliftVanSoilLayer.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("outerRing", exception.ParamName);
        }

        [Test]
        public void Constructor_HolesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new UpliftVanSoilLayer(new Point2D[0], null, new UpliftVanSoilLayer.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("holes", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAllParameters_ExpectedValues()
        {
            // Setup
            var outerRing = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            };
            var holes = new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }
            };

            // Call
            var layer = new UpliftVanSoilLayer(outerRing, holes, new UpliftVanSoilLayer.ConstructionProperties());

            // Assert
            CollectionAssert.AreEqual(outerRing, layer.OuterRing);
            CollectionAssert.AreEqual(holes, layer.Holes);
        }

        [Test]
        public void Constructor_EmptyConstructionProperties_ExpectedValues()
        {
            // Call
            var layer = new UpliftVanSoilLayer(new Point2D[0], new Point2D[0][], new UpliftVanSoilLayer.ConstructionProperties());

            // Assert
            Assert.IsFalse(layer.IsAquifer);
            Assert.IsFalse(layer.UsePop);
            Assert.IsEmpty(layer.MaterialName);
            Assert.AreEqual(UpliftVanShearStrengthModel.CPhi, layer.ShearStrengthModel);
            Assert.IsNaN(layer.AbovePhreaticLevel);
            Assert.IsNaN(layer.BelowPhreaticLevel);
            Assert.IsNaN(layer.Cohesion);
            Assert.IsNaN(layer.FrictionAngle);
            Assert.IsNaN(layer.ShearStrengthRatio);
            Assert.IsNaN(layer.StrengthIncreaseExponent);
            Assert.IsNaN(layer.Pop);
        }

        [Test]
        public void Constructor_WithConstructionProperties_ExpectedValues()
        {
            // Setup
            const string materialName = "test";
            var random = new Random(11);
            bool isAquifer = random.NextBoolean();
            bool usePop = random.NextBoolean();
            var shearStrengthModel = random.NextEnumValue<UpliftVanShearStrengthModel>();
            double abovePhreaticLevel = random.NextDouble();
            double belowPhreaticLevel = random.NextDouble();
            double cohesion = random.NextDouble();
            double frictionAngle = random.NextDouble();
            double shearStrengthRatio = random.NextDouble();
            double strengthIncreaseExponent = random.NextDouble();
            double pop = random.NextDouble();

            // Call
            var layer = new UpliftVanSoilLayer(new Point2D[0], new Point2D[0][], new UpliftVanSoilLayer.ConstructionProperties
            {
                MaterialName = materialName,
                IsAquifer = isAquifer,
                UsePop = usePop,
                ShearStrengthModel = shearStrengthModel,
                AbovePhreaticLevel = abovePhreaticLevel,
                BelowPhreaticLevel = belowPhreaticLevel,
                Cohesion = cohesion,
                FrictionAngle = frictionAngle,
                ShearStrengthRatio = shearStrengthRatio,
                StrengthIncreaseExponent = strengthIncreaseExponent,
                Pop = pop
            });

            // Assert
            Assert.AreEqual(materialName, layer.MaterialName);
            Assert.AreEqual(isAquifer, layer.IsAquifer);
            Assert.AreEqual(usePop, layer.UsePop);
            Assert.AreEqual(shearStrengthModel, layer.ShearStrengthModel);
            Assert.AreEqual(abovePhreaticLevel, layer.AbovePhreaticLevel);
            Assert.AreEqual(belowPhreaticLevel, layer.BelowPhreaticLevel);
            Assert.AreEqual(cohesion, layer.Cohesion);
            Assert.AreEqual(frictionAngle, layer.FrictionAngle);
            Assert.AreEqual(shearStrengthRatio, layer.ShearStrengthRatio);
            Assert.AreEqual(strengthIncreaseExponent, layer.StrengthIncreaseExponent);
            Assert.AreEqual(pop, layer.Pop);
        }
    }
}
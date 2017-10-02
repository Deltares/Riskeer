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
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class SoilCreatorTest
    {
        [Test]
        public void Create_ProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("profile", exception.ParamName);
        }

        [Test]
        public void Create_ProfileWithLayers_ReturnsProfileWithLayers()
        {
            // Setup
            var random = new Random(11);

            var profile = new UpliftVanSoilProfile(new[]
            {
                new UpliftVanSoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties
                    {
                        UsePop = true,
                        IsAquifer = false,
                        ShearStrengthModel = UpliftVanShearStrengthModel.CPhi,
                        MaterialName = "Sand",
                        AbovePhreaticLevel = random.NextDouble(),
                        BelowPhreaticLevel = random.NextDouble(),
                        Cohesion = random.NextDouble(),
                        FrictionAngle = random.NextDouble(),
                        ShearStrengthRatio = random.NextDouble(),
                        StrengthIncreaseExponent = random.NextDouble(),
                        Pop = random.NextDouble(),
                        DilatancyType = UpliftVanDilatancyType.Phi
                    }),
                new UpliftVanSoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties
                    {
                        UsePop = true,
                        IsAquifer = true,
                        ShearStrengthModel = UpliftVanShearStrengthModel.CPhiOrSuCalculated,
                        MaterialName = "Clay",
                        AbovePhreaticLevel = random.NextDouble(),
                        BelowPhreaticLevel = random.NextDouble(),
                        Cohesion = random.NextDouble(),
                        FrictionAngle = random.NextDouble(),
                        ShearStrengthRatio = random.NextDouble(),
                        StrengthIncreaseExponent = random.NextDouble(),
                        Pop = random.NextDouble(),
                        DilatancyType = UpliftVanDilatancyType.Zero
                    }),
                new UpliftVanSoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties
                    {
                        UsePop = false,
                        IsAquifer = true,
                        ShearStrengthModel = UpliftVanShearStrengthModel.SuCalculated,
                        MaterialName = "Grass",
                        AbovePhreaticLevel = random.NextDouble(),
                        BelowPhreaticLevel = random.NextDouble(),
                        Cohesion = random.NextDouble(),
                        FrictionAngle = random.NextDouble(),
                        ShearStrengthRatio = random.NextDouble(),
                        StrengthIncreaseExponent = random.NextDouble(),
                        Pop = random.NextDouble(),
                        DilatancyType = UpliftVanDilatancyType.MinusPhi
                    })
            }, Enumerable.Empty<UpliftVanPreconsolidationStress>());

            // Call
            Soil[] soils = SoilCreator.Create(profile);

            // Assert
            Assert.AreEqual(3, soils.Length);

            CollectionAssert.AreEqual(profile.Layers.Select(l => l.UsePop), soils.Select(s => s.UsePop));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.MaterialName), soils.Select(s => s.Name));
            CollectionAssert.AreEqual(new[]
            {
                ShearStrengthModel.CPhi,
                ShearStrengthModel.CPhiOrCuCalculated,
                ShearStrengthModel.CuCalculated
            }, soils.Select(s => s.ShearStrengthModel));
            CollectionAssert.AreEqual(new[]
            {
                DilatancyType.Phi,
                DilatancyType.Zero,
                DilatancyType.MinusPhi
            }, soils.Select(s => s.DilatancyType));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.AbovePhreaticLevel), soils.Select(s => s.AbovePhreaticLevel));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.BelowPhreaticLevel), soils.Select(s => s.BelowPhreaticLevel));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.Cohesion), soils.Select(s => s.Cohesion));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.FrictionAngle), soils.Select(s => s.FrictionAngle));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.ShearStrengthRatio), soils.Select(s => s.RatioCuPc));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.StrengthIncreaseExponent), soils.Select(s => s.StrengthIncreaseExponent));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.Pop), soils.Select(s => s.PoP));

            Assert.IsTrue(soils.All(s => double.IsNaN(s.Ocr)));
            Assert.IsTrue(soils.All(s => double.IsNaN(s.CuBottom))); // Only for CuMeasured
            Assert.IsTrue(soils.All(s => double.IsNaN(s.CuTop))); // Only for CuMeasured
        }

        [Test]
        public void Create_InvalidShearStrengthModel_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new UpliftVanSoilProfile(new[]
            {
                new UpliftVanSoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties
                    {
                        ShearStrengthModel = (UpliftVanShearStrengthModel) 99
                    })
            }, Enumerable.Empty<UpliftVanPreconsolidationStress>());

            // Call
            TestDelegate test = () => SoilCreator.Create(profile);

            // Assert
            string message = $"The value of argument 'shearStrengthModel' ({99}) is invalid for Enum type '{typeof(UpliftVanShearStrengthModel).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        public void Create_InvalidDilatancyType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new UpliftVanSoilProfile(new[]
            {
                new UpliftVanSoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties
                    {
                        DilatancyType = (UpliftVanDilatancyType) 99
                    })
            }, Enumerable.Empty<UpliftVanPreconsolidationStress>());

            // Call
            TestDelegate test = () => SoilCreator.Create(profile);

            // Assert
            string message = $"The value of argument 'dilatancyType' ({99}) is invalid for Enum type '{typeof(UpliftVanDilatancyType).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }
    }
}
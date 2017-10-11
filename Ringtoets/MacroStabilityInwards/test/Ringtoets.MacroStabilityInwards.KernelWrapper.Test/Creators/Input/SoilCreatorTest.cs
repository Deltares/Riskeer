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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using DilatancyType = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.DilatancyType;
using Point2D = Core.Common.Base.Geometry.Point2D;
using ShearStrengthModel = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.ShearStrengthModel;
using SoilLayer = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;
using WTIStabilityShearStrengthModel = Deltares.WTIStability.Data.Geo.ShearStrengthModel;
using WTIStabilityDilatancyType = Deltares.WTIStability.Data.Geo.DilatancyType;

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

            var profile = new SoilProfile(new[]
            {
                new SoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties
                    {
                        UsePop = true,
                        IsAquifer = false,
                        ShearStrengthModel = ShearStrengthModel.CPhi,
                        MaterialName = "Sand",
                        AbovePhreaticLevel = random.NextDouble(),
                        BelowPhreaticLevel = random.NextDouble(),
                        Cohesion = random.NextDouble(),
                        FrictionAngle = random.NextDouble(),
                        ShearStrengthRatio = random.NextDouble(),
                        StrengthIncreaseExponent = random.NextDouble(),
                        Pop = random.NextDouble(),
                        DilatancyType = DilatancyType.Phi
                    }),
                new SoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties
                    {
                        UsePop = true,
                        IsAquifer = true,
                        ShearStrengthModel = ShearStrengthModel.CPhiOrSuCalculated,
                        MaterialName = "Clay",
                        AbovePhreaticLevel = random.NextDouble(),
                        BelowPhreaticLevel = random.NextDouble(),
                        Cohesion = random.NextDouble(),
                        FrictionAngle = random.NextDouble(),
                        ShearStrengthRatio = random.NextDouble(),
                        StrengthIncreaseExponent = random.NextDouble(),
                        Pop = random.NextDouble(),
                        DilatancyType = DilatancyType.Zero
                    }),
                new SoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties
                    {
                        UsePop = false,
                        IsAquifer = true,
                        ShearStrengthModel = ShearStrengthModel.SuCalculated,
                        MaterialName = "Grass",
                        AbovePhreaticLevel = random.NextDouble(),
                        BelowPhreaticLevel = random.NextDouble(),
                        Cohesion = random.NextDouble(),
                        FrictionAngle = random.NextDouble(),
                        ShearStrengthRatio = random.NextDouble(),
                        StrengthIncreaseExponent = random.NextDouble(),
                        Pop = random.NextDouble(),
                        DilatancyType = DilatancyType.MinusPhi
                    })
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            Soil[] soils = SoilCreator.Create(profile);

            // Assert
            Assert.AreEqual(3, soils.Length);

            CollectionAssert.AreEqual(profile.Layers.Select(l => l.UsePop), soils.Select(s => s.UsePop));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.MaterialName), soils.Select(s => s.Name));
            CollectionAssert.AreEqual(new[]
            {
                WTIStabilityShearStrengthModel.CPhi,
                WTIStabilityShearStrengthModel.CPhiOrCuCalculated,
                WTIStabilityShearStrengthModel.CuCalculated
            }, soils.Select(s => s.ShearStrengthModel));
            CollectionAssert.AreEqual(new[]
            {
                WTIStabilityDilatancyType.Phi,
                WTIStabilityDilatancyType.Zero,
                WTIStabilityDilatancyType.MinusPhi
            }, soils.Select(s => s.DilatancyType));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.AbovePhreaticLevel), soils.Select(s => s.AbovePhreaticLevel));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.BelowPhreaticLevel), soils.Select(s => s.BelowPhreaticLevel));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.Cohesion), soils.Select(s => s.Cohesion));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.FrictionAngle), soils.Select(s => s.FrictionAngle));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.ShearStrengthRatio), soils.Select(s => s.RatioCuPc));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.StrengthIncreaseExponent), soils.Select(s => s.StrengthIncreaseExponent));
            CollectionAssert.AreEqual(profile.Layers.Select(l => l.Pop), soils.Select(s => s.PoP));

            Assert.IsTrue(soils.All(s => double.IsNaN(s.Ocr))); // OCR is only used as output
            Assert.IsTrue(soils.All(s => double.IsNaN(s.CuBottom))); // Only for CuMeasured
            Assert.IsTrue(soils.All(s => double.IsNaN(s.CuTop))); // Only for CuMeasured
        }

        [Test]
        public void Create_InvalidShearStrengthModel_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new SoilProfile(new[]
            {
                new SoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties
                    {
                        ShearStrengthModel = (ShearStrengthModel) 99
                    })
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            TestDelegate test = () => SoilCreator.Create(profile);

            // Assert
            string message = $"The value of argument 'shearStrengthModel' ({99}) is invalid for Enum type '{typeof(ShearStrengthModel).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        public void Create_InvalidDilatancyType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new SoilProfile(new[]
            {
                new SoilLayer(
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties
                    {
                        DilatancyType = (DilatancyType) 99
                    })
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            TestDelegate test = () => SoilCreator.Create(profile);

            // Assert
            string message = $"The value of argument 'dilatancyType' ({99}) is invalid for Enum type '{typeof(DilatancyType).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }
    }
}
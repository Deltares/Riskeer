// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections;
using System.Collections.Generic;
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan.Input
{
    [TestFixture]
    public class SoilComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var comparer = new SoilComparer();

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<Soil>>(comparer);
        }

        [Test]
        public void Compare_FirstObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var firstObject = new object();
            object secondObject = new Soil();

            var comparer = new SoilComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(Soil)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SecondObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = new Soil();
            var secondObject = new object();

            var comparer = new SoilComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(Soil)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SameInstance_ReturnZero()
        {
            // Setup
            var soil = new Soil();

            // Call
            int result = new SoilComparer().Compare(soil, soil);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_EqualProperties_ReturnZero()
        {
            // Setup
            Soil soil1 = CreateSoil();
            Soil soil2 = CreateSoil();

            // Call
            int result = new SoilComparer().Compare(soil1, soil2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        [TestCaseSource(nameof(GetNotEqualProperties))]
        public void Compare_NotEqualProperties_ReturnOne(Action<Soil> setSoil1PropertyAction,
                                                         Action<Soil> setSoil2PropertyAction)
        {
            // Setup
            var soil1 = new Soil();
            var soil2 = new Soil();

            setSoil1PropertyAction(soil1);
            setSoil2PropertyAction(soil2);

            // Call
            int result = new SoilComparer().Compare(soil1, soil2);

            // Assert
            Assert.AreEqual(1, result);
        }

        private static IEnumerable<TestCaseData> GetNotEqualProperties()
        {
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.Name = "Soil 1"),
                new Action<Soil>(soil => soil.Name = "Soil 2"));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.AbovePhreaticLevel = double.NaN),
                new Action<Soil>(soil => soil.AbovePhreaticLevel = 0.1));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.AbovePhreaticLevel = 0.1),
                new Action<Soil>(soil => soil.AbovePhreaticLevel = 0.2));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.BelowPhreaticLevel = double.NaN),
                new Action<Soil>(soil => soil.BelowPhreaticLevel = 0.1));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.BelowPhreaticLevel = 0.1),
                new Action<Soil>(soil => soil.BelowPhreaticLevel = 0.2));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.Dilatancy = double.NaN),
                new Action<Soil>(soil => soil.Dilatancy = 0.1));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.Dilatancy = 0.1),
                new Action<Soil>(soil => soil.Dilatancy = 0.2));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.Cohesion = double.NaN),
                new Action<Soil>(soil => soil.Cohesion = 0.1));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.Cohesion = 0.1),
                new Action<Soil>(soil => soil.Cohesion = 0.2));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.FrictionAngle = double.NaN),
                new Action<Soil>(soil => soil.FrictionAngle = 0.1));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.FrictionAngle = 0.1),
                new Action<Soil>(soil => soil.FrictionAngle = 0.2));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.RatioCuPc = double.NaN),
                new Action<Soil>(soil => soil.RatioCuPc = 0.1));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.RatioCuPc = 0.1),
                new Action<Soil>(soil => soil.RatioCuPc = 0.2));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.StrengthIncreaseExponent = double.NaN),
                new Action<Soil>(soil => soil.StrengthIncreaseExponent = 0.1));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.StrengthIncreaseExponent = 0.1),
                new Action<Soil>(soil => soil.StrengthIncreaseExponent = 0.2));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.ShearStrengthAbovePhreaticLevelModel = ShearStrengthModelType.MohrCoulomb),
                new Action<Soil>(soil => soil.ShearStrengthAbovePhreaticLevelModel = ShearStrengthModelType.Shansep));
            yield return new TestCaseData(
                new Action<Soil>(soil => soil.ShearStrengthBelowPhreaticLevelModel = ShearStrengthModelType.MohrCoulomb),
                new Action<Soil>(soil => soil.ShearStrengthBelowPhreaticLevelModel = ShearStrengthModelType.Shansep));
        }

        private static Soil CreateSoil()
        {
            return new Soil
            {
                Name = "soil",
                AbovePhreaticLevel = 0.1,
                BelowPhreaticLevel = 0.2,
                Dilatancy = 0.3,
                Cohesion = 0.4,
                FrictionAngle = 0.5,
                RatioCuPc = 0.6,
                StrengthIncreaseExponent = 0.7,
                ShearStrengthAbovePhreaticLevelModel = ShearStrengthModelType.MohrCoulomb,
                ShearStrengthBelowPhreaticLevelModel = ShearStrengthModelType.MohrCoulomb
            };
        }
    }
}
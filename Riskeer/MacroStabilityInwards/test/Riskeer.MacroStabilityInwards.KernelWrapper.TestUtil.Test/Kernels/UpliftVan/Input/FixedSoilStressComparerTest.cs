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
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan.Input
{
    [TestFixture]
    public class FixedSoilStressComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var comparer = new FixedSoilStressComparer();

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<FixedSoilStress>>(comparer);
        }

        [Test]
        public void Compare_FirstObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var firstObject = new object();
            object secondObject = new FixedSoilStress
            {
                POP = 0,
                Soil = new Soil()
            };

            var comparer = new FixedSoilStressComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(FixedSoilStress)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SecondObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = new FixedSoilStress
            {
                POP = 0,
                Soil = new Soil()
            };
            var secondObject = new object();

            var comparer = new FixedSoilStressComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(FixedSoilStress)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SameInstance_ReturnZero()
        {
            // Setup
            var soilStress = new FixedSoilStress
            {
                POP = 0,
                Soil = new Soil()
            };

            // Call
            int result = new FixedSoilStressComparer().Compare(soilStress, soilStress);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_EqualSoilAndPop_ReturnZero()
        {
            // Setup
            const double pop = 1.1;
            var soil = new Soil();
            var fixedSoilStress1 = new FixedSoilStress
            {
                POP = pop,
                Soil = soil
            };
            var fixedSoilStress2 = new FixedSoilStress
            {
                POP = pop,
                Soil = soil
            };

            // Call
            int result = new FixedSoilStressComparer().Compare(fixedSoilStress1, fixedSoilStress2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_DifferentSoils_ReturnOne()
        {
            // Setup
            const double pop = 1.1;
            var fixedSoilStress1 = new FixedSoilStress
            {
                POP = pop,
                Soil = new Soil
                {
                    Name = "Soil 1"
                }
            };
            var fixedSoilStress2 = new FixedSoilStress
            {
                POP = pop,
                Soil = new Soil
                {
                    Name = "Soil 2"
                }
            };

            // Call
            int result = new FixedSoilStressComparer().Compare(fixedSoilStress1, fixedSoilStress2);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Compare_DifferentPop_ReturnOne()
        {
            // Setup
            var soil = new Soil();
            var fixedSoilStress1 = new FixedSoilStress
            {
                POP = 1.1,
                Soil = soil
            };
            var fixedSoilStress2 = new FixedSoilStress
            {
                POP = 2.2,
                Soil = soil
            };

            // Call
            int result = new FixedSoilStressComparer().Compare(fixedSoilStress1, fixedSoilStress2);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
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
using Deltares.MacroStability.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan.Input
{
    [TestFixture]
    public class PreConsolidationStressComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var comparer = new PreConsolidationStressComparer();

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<PreConsolidationStress>>(comparer);
        }

        [Test]
        public void Compare_FirstObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var firstObject = new object();
            object secondObject = new PreConsolidationStress();

            var comparer = new PreConsolidationStressComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(PreConsolidationStress)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SecondObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = new PreConsolidationStress();
            var secondObject = new object();

            var comparer = new PreConsolidationStressComparer();

            // Call
            void Call() => comparer.Compare(firstObject, secondObject);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"Cannot compare objects other than {typeof(PreConsolidationStress)} with this comparer.", exception.Message);
        }

        [Test]
        public void Compare_SameInstance_ReturnZero()
        {
            // Setup
            var preConsolidationStress = new PreConsolidationStress();

            // Call
            int result = new PreConsolidationStressComparer().Compare(preConsolidationStress, preConsolidationStress);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_EqualProperties_ReturnZero()
        {
            // Setup
            const double stressValue = 8.4;
            const string name = "testName";
            const double x = 2.01;
            const double z = 40.486;
            var preConsolidationStress1 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = name,
                X = x,
                Z = z
            };
            var preConsolidationStress2 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = name,
                X = x,
                Z = z
            };

            // Call
            int result = new PreConsolidationStressComparer().Compare(preConsolidationStress1, preConsolidationStress2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_DifferentName_ReturnOne()
        {
            // Setup
            const double stressValue = 8.4;
            const string name = "testName";
            const double x = 2.01;
            const double z = 40.486;
            var preConsolidationStress1 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = name,
                X = x,
                Z = z
            };
            var preConsolidationStress2 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = "newName",
                X = x,
                Z = z
            };

            // Call
            int result = new PreConsolidationStressComparer().Compare(preConsolidationStress1, preConsolidationStress2);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Compare_DifferentStressValue_ReturnOne()
        {
            // Setup
            const double stressValue = 8.4;
            const string name = "testName";
            const double x = 2.01;
            const double z = 40.486;
            var preConsolidationStress1 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = name,
                X = x,
                Z = z
            };
            var preConsolidationStress2 = new PreConsolidationStress
            {
                StressValue = 16.8,
                Name = name,
                X = x,
                Z = z
            };

            // Call
            int result = new PreConsolidationStressComparer().Compare(preConsolidationStress1, preConsolidationStress2);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Compare_DifferentX_ReturnOne()
        {
            // Setup
            const double stressValue = 8.4;
            const string name = "testName";
            const double x = 2.01;
            const double z = 40.486;
            var preConsolidationStress1 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = name,
                X = x,
                Z = z
            };
            var preConsolidationStress2 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = name,
                X = 18.78,
                Z = z
            };

            // Call
            int result = new PreConsolidationStressComparer().Compare(preConsolidationStress1, preConsolidationStress2);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Compare_DifferentZ_ReturnOne()
        {
            // Setup
            const double stressValue = 8.4;
            const string name = "testName";
            const double x = 2.01;
            const double z = 40.486;
            var preConsolidationStress1 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = name,
                X = x,
                Z = z
            };
            var preConsolidationStress2 = new PreConsolidationStress
            {
                StressValue = stressValue,
                Name = name,
                X = x,
                Z = 40.487
            };

            // Call
            int result = new PreConsolidationStressComparer().Compare(preConsolidationStress1, preConsolidationStress2);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
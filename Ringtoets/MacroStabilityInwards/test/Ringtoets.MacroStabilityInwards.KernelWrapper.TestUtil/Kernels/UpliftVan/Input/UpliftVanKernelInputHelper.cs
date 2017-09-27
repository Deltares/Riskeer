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

using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// Helper that can be used in tests.
    /// </summary>
    public static class UpliftVanKernelInputHelper
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SoilModel"/>.</param>
        /// <param name="actual">The actual <see cref="SoilModel"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSoilModels(SoilModel expected, SoilModel actual)
        {
            Assert.AreEqual(expected.Soils.Count, actual.Soils.Count);

            for (var i = 0; i < expected.Soils.Count; i++)
            {
                Soil expectedSoil = expected.Soils[i];
                Soil actualSoil = actual.Soils[i];

                Assert.AreEqual(expectedSoil.Name, actualSoil.Name);
                Assert.AreEqual(expectedSoil.UsePop, actualSoil.UsePop);
                Assert.AreEqual(expectedSoil.ShearStrengthModel, actualSoil.ShearStrengthModel);
                Assert.AreEqual(expectedSoil.AbovePhreaticLevel, actualSoil.AbovePhreaticLevel);
                Assert.AreEqual(expectedSoil.BelowPhreaticLevel, actualSoil.BelowPhreaticLevel);
                Assert.AreEqual(expectedSoil.Cohesion, actualSoil.Cohesion);
                Assert.AreEqual(expectedSoil.FrictionAngle, actualSoil.FrictionAngle);
                Assert.AreEqual(expectedSoil.RatioCuPc, actualSoil.RatioCuPc);
                Assert.AreEqual(expectedSoil.StrengthIncreaseExponent, actualSoil.StrengthIncreaseExponent);
                Assert.AreEqual(expectedSoil.PoP, actualSoil.PoP);
                Assert.AreEqual(expectedSoil.DilatancyType, actualSoil.DilatancyType);
            }
        }
    }
}
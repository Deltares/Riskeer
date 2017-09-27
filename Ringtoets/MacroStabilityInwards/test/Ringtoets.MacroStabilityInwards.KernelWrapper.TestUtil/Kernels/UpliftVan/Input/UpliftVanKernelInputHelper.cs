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

using System.Linq;
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
                AssertSoils(expected.Soils[i], actual.Soils[i]);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SoilProfile2D"/>.</param>
        /// <param name="actual">The actual <see cref="SoilProfile2D"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSoilProfiles(SoilProfile2D expected, SoilProfile2D actual)
        {
            AssertSoilLayers(expected.Surfaces.ToArray(), actual.Surfaces.ToArray());
            AssertPreconsolidationStresses(expected.PreconsolidationStresses.ToArray(), actual.PreconsolidationStresses.ToArray());
            AssertGeometryDatas(expected.Geometry, actual.Geometry);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SoilLayer2D"/> array.</param>
        /// <param name="actual">The actual <see cref="SoilLayer2D"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertSoilLayers(SoilLayer2D[] expected, SoilLayer2D[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                SoilLayer2D expectedSurface = expected[i];
                SoilLayer2D actualSurface = actual[i];

                Assert.AreEqual(expectedSurface.IsAquifer, actualSurface.IsAquifer);
                AssertGeometrySurfaces(expectedSurface.GeometrySurface, actualSurface.GeometrySurface);
                AssertSoils(expectedSurface.Soil, actualSurface.Soil);
                Assert.AreEqual(expectedSurface.WaterpressureInterpolationModel, actualSurface.WaterpressureInterpolationModel);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="PreConsolidationStress"/> array.</param>
        /// <param name="actual">The actual <see cref="PreConsolidationStress"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertPreconsolidationStresses(PreConsolidationStress[] expected, PreConsolidationStress[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                PreConsolidationStress expectedPreconsolidationStress = expected[i];
                PreConsolidationStress actualPreconsolidationStress = actual[i];

                Assert.AreEqual(expectedPreconsolidationStress.StressValue, actualPreconsolidationStress.StressValue);
                Assert.AreEqual(expectedPreconsolidationStress.X, actualPreconsolidationStress.X);
                Assert.AreEqual(expectedPreconsolidationStress.Z, actualPreconsolidationStress.Z);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="GeometrySurface"/>.</param>
        /// <param name="actual">The actual <see cref="GeometrySurface"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGeometrySurfaces(GeometrySurface expected, GeometrySurface actual)
        {
            CollectionAssert.AreEqual(new[]
                                      {
                                          expected.OuterLoop
                                      }.Concat(expected.InnerLoops),
                                      new[]
                                      {
                                          actual.OuterLoop
                                      }.Concat(actual.InnerLoops),
                                      new WTIStabilityGeometryLoopComparer());
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="Soil"/>.</param>
        /// <param name="actual">The actual <see cref="Soil"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertSoils(Soil expected, Soil actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.UsePop, actual.UsePop);
            Assert.AreEqual(expected.ShearStrengthModel, actual.ShearStrengthModel);
            Assert.AreEqual(expected.AbovePhreaticLevel, actual.AbovePhreaticLevel);
            Assert.AreEqual(expected.BelowPhreaticLevel, actual.BelowPhreaticLevel);
            Assert.AreEqual(expected.Cohesion, actual.Cohesion);
            Assert.AreEqual(expected.FrictionAngle, actual.FrictionAngle);
            Assert.AreEqual(expected.RatioCuPc, actual.RatioCuPc);
            Assert.AreEqual(expected.StrengthIncreaseExponent, actual.StrengthIncreaseExponent);
            Assert.AreEqual(expected.PoP, actual.PoP);
            Assert.AreEqual(expected.DilatancyType, actual.DilatancyType);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="GeometryData"/>.</param>
        /// <param name="actual">The actual <see cref="GeometryData"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGeometryDatas(GeometryData expected, GeometryData actual)
        {
            AssertGeometrySurfaces(expected.Surfaces.ToArray(), actual.Surfaces.ToArray());
            CollectionAssert.AreEqual(expected.Loops, actual.Loops, new WTIStabilityGeometryLoopComparer());
            CollectionAssert.AreEqual(expected.Curves, actual.Curves, new WTIStabilityGeometryCurveComparer());
            CollectionAssert.AreEqual(expected.Points, actual.Points, new WTIStabilityPoint2DComparer());
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="GeometrySurface"/> array.</param>
        /// <param name="actual">The actual <see cref="GeometrySurface"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGeometrySurfaces(GeometrySurface[] expected, GeometrySurface[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                AssertGeometrySurfaces(expected[i], actual[i]);
            }
        }
    }
}
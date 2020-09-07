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

using System.Collections.Generic;
using System.Linq;
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// Class for asserting kernel input.
    /// </summary>
    public static class KernelInputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SoilProfile"/>.</param>
        /// <param name="actual">The actual <see cref="SoilProfile"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSoilProfile(SoilProfile expected, SoilProfile actual)
        {
            AssertSoilProfileSurfaces(expected.SoilSurfaces, actual.SoilSurfaces);
            AssertGeometries(expected.Geometry, actual.Geometry);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="Location"/>.</param>
        /// <param name="actual">The actual <see cref="Location"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertLocations(Location expected, Location actual)
        {
            Assert.AreEqual(expected.DikeSoilScenario, actual.DikeSoilScenario);
            Assert.AreEqual(expected.WaternetCreationMode, actual.WaternetCreationMode);
            Assert.AreEqual(expected.PlLineCreationMethod, actual.PlLineCreationMethod);
            Assert.AreEqual(expected.WaterLevelRiver, actual.WaterLevelRiver);
            Assert.AreEqual(expected.WaterLevelRiverAverage, actual.WaterLevelRiverAverage);
            Assert.AreEqual(expected.WaterLevelPolder, actual.WaterLevelPolder);
            Assert.AreEqual(expected.WaterLevelRiverLow, actual.WaterLevelRiverLow);
            Assert.AreEqual(expected.DrainageConstructionPresent, actual.DrainageConstructionPresent);
            Assert.AreEqual(expected.XCoordMiddleDrainageConstruction, actual.XCoordMiddleDrainageConstruction);
            Assert.AreEqual(expected.ZCoordMiddleDrainageConstruction, actual.ZCoordMiddleDrainageConstruction);
            Assert.AreEqual(expected.MinimumLevelPhreaticLineAtDikeTopRiver, actual.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(expected.MinimumLevelPhreaticLineAtDikeTopPolder, actual.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(expected.UseDefaultOffsets, actual.UseDefaultOffsets);
            Assert.AreEqual(expected.PlLineOffsetBelowPointBRingtoetsWti2017, actual.PlLineOffsetBelowPointBRingtoetsWti2017);
            Assert.AreEqual(expected.PlLineOffsetBelowDikeTopAtPolder, actual.PlLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(expected.PlLineOffsetBelowShoulderBaseInside, actual.PlLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(expected.PlLineOffsetBelowDikeToeAtPolder, actual.PlLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(expected.HeadInPlLine2Outwards, actual.HeadInPlLine2Outwards);
            Assert.AreEqual(expected.HeadInPlLine2Inwards, actual.HeadInPlLine2Inwards);
            Assert.AreEqual(expected.AdjustPl3And4ForUplift, actual.AdjustPl3And4ForUplift);
            Assert.AreEqual(expected.PenetrationLength, actual.PenetrationLength);
            Assert.AreEqual(expected.LeakageLengthOutwardsPl3, actual.LeakageLengthOutwardsPl3);
            Assert.AreEqual(expected.LeakageLengthInwardsPl3, actual.LeakageLengthInwardsPl3);
            Assert.AreEqual(expected.LeakageLengthOutwardsPl4, actual.LeakageLengthOutwardsPl4);
            Assert.AreEqual(expected.LeakageLengthInwardsPl4, actual.LeakageLengthInwardsPl4);
            Assert.AreEqual(expected.HeadInPlLine3, actual.HeadInPlLine3);
            Assert.AreEqual(expected.HeadInPlLine4, actual.HeadInPlLine4);
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.Y, actual.Y);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SurfaceLine"/>.</param>
        /// <param name="actual">The actual <see cref="SurfaceLine"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSurfaceLine(SurfaceLine expected, SurfaceLine actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            AssertGeometryPointStrings(expected.Geometry, actual.Geometry);
            AssertCharacteristicPointSets(expected.CharacteristicPoints, actual.CharacteristicPoints);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SoilProfileSurface"/> array.</param>
        /// <param name="actual">The actual <see cref="SoilProfileSurface"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertSoilProfileSurfaces(ICollection<SoilProfileSurface> expected, ICollection<SoilProfileSurface> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                SoilProfileSurface expectedSoilProfileSurface = expected.ElementAt(i);
                SoilProfileSurface actualSoilProfileSurface = actual.ElementAt(i);

                Assert.AreEqual(expectedSoilProfileSurface.Name, actualSoilProfileSurface.Name);
                Assert.AreEqual(expectedSoilProfileSurface.IsAquifer, actualSoilProfileSurface.IsAquifer);
                AssertGeometrySurfaces(expectedSoilProfileSurface.Surface, actualSoilProfileSurface.Surface);
                AssertSoils(expectedSoilProfileSurface.Soil, actualSoilProfileSurface.Soil);
                Assert.AreEqual(expectedSoilProfileSurface.WaterPressureInterpolationModel, actualSoilProfileSurface.WaterPressureInterpolationModel);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="Surface"/>.</param>
        /// <param name="actual">The actual <see cref="Surface"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGeometrySurfaces(Surface expected, Surface actual)
        {
            CollectionAssert.AreEqual(new[]
                                      {
                                          expected.OuterLoop
                                      }.Concat(expected.InnerLoops),
                                      new[]
                                      {
                                          actual.OuterLoop
                                      }.Concat(actual.InnerLoops),
                                      new LoopComparer());
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="Surface"/> array.</param>
        /// <param name="actual">The actual <see cref="Surface"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGeometrySurfaces(ICollection<Surface> expected, ICollection<Surface> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                AssertGeometrySurfaces(expected.ElementAt(i), actual.ElementAt(i));
            }
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
            Assert.AreEqual(expected.ShearStrengthAbovePhreaticLevelModel, actual.ShearStrengthAbovePhreaticLevelModel);
            Assert.AreEqual(expected.ShearStrengthBelowPhreaticLevelModel, actual.ShearStrengthBelowPhreaticLevelModel);
            Assert.AreEqual(expected.AbovePhreaticLevel, actual.AbovePhreaticLevel);
            Assert.AreEqual(expected.BelowPhreaticLevel, actual.BelowPhreaticLevel);
            Assert.AreEqual(expected.Cohesion, actual.Cohesion);
            Assert.AreEqual(expected.FrictionAngle, actual.FrictionAngle);
            Assert.AreEqual(expected.RatioCuPc, actual.RatioCuPc);
            Assert.AreEqual(expected.StrengthIncreaseExponent, actual.StrengthIncreaseExponent);
            Assert.AreEqual(expected.Dilatancy, actual.Dilatancy);
            Assert.AreEqual(expected.RatioCuPc, actual.RatioCuPc);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="Geometry"/>.</param>
        /// <param name="actual">The actual <see cref="Geometry"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGeometries(Geometry expected, Geometry actual)
        {
            AssertGeometrySurfaces(expected.Surfaces, actual.Surfaces);

            CollectionAssert.AreEqual(expected.Loops, actual.Loops, new LoopComparer());
            CollectionAssert.AreEqual(expected.Curves, actual.Curves, new CurveComparer());
            CollectionAssert.AreEqual(expected.Points, actual.Points, new StabilityPointComparer());
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="CharacteristicPointSet"/>.</param>
        /// <param name="actual">The actual <see cref="CharacteristicPointSet"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertCharacteristicPointSets(CharacteristicPointSet expected, CharacteristicPointSet actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.AreEqual(expected.GeometryMustContainPoint, actual.GeometryMustContainPoint);

            for (var i = 0; i < expected.Count; i++)
            {
                CharacteristicPoint expectedCharacteristicPoint = expected[i];
                CharacteristicPoint actualCharacteristicPoint = actual[i];

                AssertGeometryPoints(expectedCharacteristicPoint.GeometryPoint, actualCharacteristicPoint.GeometryPoint);
                Assert.AreEqual(expectedCharacteristicPoint.CharacteristicPointType, actualCharacteristicPoint.CharacteristicPointType);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="GeometryPointString"/>.</param>
        /// <param name="actual">The actual <see cref="GeometryPointString"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGeometryPointStrings(GeometryPointString expected, GeometryPointString actual)
        {
            Assert.AreEqual(expected.Points.Count, actual.Points.Count);

            for (var i = 0; i < expected.Points.Count; i++)
            {
                AssertGeometryPoints(expected.Points[i], actual.Points[i]);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="GeometryPoint"/>.</param>
        /// <param name="actual">The actual <see cref="GeometryPoint"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGeometryPoints(GeometryPoint expected, GeometryPoint actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Owner, actual.Owner);
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.Z, actual.Z);
        }
    }
}
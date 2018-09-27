// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Piping.IO.SurfaceLines;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.SurfaceLines
{
    [TestFixture]
    public class PipingSurfaceLineExtensionsTest
    {
        private static IEnumerable<TestCaseData> DifferentValidCharacteristicPointConfigurations
        {
            get
            {
                var dikeToeAtRiver = new Point3D(3, 2, 5);
                var dikeToeAtPolder = new Point3D(3.4, 3, 8);
                var ditchDikeSide = new Point3D(4.4, 6, 8);
                var bottomDitchDikeSide = new Point3D(5.1, 6, 6.5);
                var bottomDitchPolderSide = new Point3D(8.5, 7.2, 4.2);
                var ditchPolderSide = new Point3D(9.6, 7.5, 3.9);

                var name = "All present";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing DikeToeAtRiver";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing DikeToeAtPolder";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing DitchDikeSide";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing BottomDitchDikeSide";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing BottomDitchPolderSide";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing DitchPolderSide";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide
                }).SetName(name);
            }
        }

        [Test]
        public void SetCharacteristicPoints_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((PipingSurfaceLine) null).SetCharacteristicPoints(new CharacteristicPoints("Empty"));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void SetCharacteristicPoints_CharacteristicPointsNull_NoCharacteristicPointsSet()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3, 2, 5),
                new Point3D(3.4, 3, 8),
                new Point3D(4.4, 6, 8),
                new Point3D(5.1, 6, 6.5),
                new Point3D(8.5, 7.2, 4.2),
                new Point3D(9.6, 7.5, 3.9)
            });

            // Call
            surfaceLine.SetCharacteristicPoints(null);

            // Assert
            Assert.IsNull(surfaceLine.DikeToeAtRiver);
            Assert.IsNull(surfaceLine.DikeToeAtPolder);
            Assert.IsNull(surfaceLine.DitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine.DitchPolderSide);
        }

        [Test]
        public void SetCharacteristicPoints_DikeToesReversed_ThrowsImportedDataTransformException()
        {
            // Setup
            const string name = "Reversed dike toes";
            var points = new CharacteristicPoints(name)
            {
                DikeToeAtPolder = new Point3D(3, 2, 5),
                DikeToeAtRiver = new Point3D(3.4, 3, 8),
                DitchDikeSide = new Point3D(4.4, 6, 8),
                BottomDitchDikeSide = new Point3D(5.1, 6, 6.5),
                BottomDitchPolderSide = new Point3D(8.5, 7.2, 4.2),
                DitchPolderSide = new Point3D(9.6, 7.5, 3.9)
            };
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(CharacteristicPointsToGeometry(points));

            // Call
            TestDelegate test = () => surfaceLine.SetCharacteristicPoints(points);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Het uittredepunt moet landwaarts van het intredepunt liggen voor locatie '{name}'.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(DifferentValidCharacteristicPointConfigurations))]
        public void SetCharacteristicPoints_ValidSituations_PointsAreSet(CharacteristicPoints points)
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(CharacteristicPointsToGeometry(points));

            // Call
            surfaceLine.SetCharacteristicPoints(points);

            // Assert
            Assert.AreEqual(points.DikeToeAtRiver, surfaceLine.DikeToeAtRiver);
            Assert.AreEqual(points.DikeToeAtPolder, surfaceLine.DikeToeAtPolder);
            Assert.AreEqual(points.DitchDikeSide, surfaceLine.DitchDikeSide);
            Assert.AreEqual(points.BottomDitchDikeSide, surfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(points.BottomDitchPolderSide, surfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(points.DitchPolderSide, surfaceLine.DitchPolderSide);
        }

        private static IEnumerable<Point3D> CharacteristicPointsToGeometry(CharacteristicPoints points)
        {
            return new[]
            {
                points.DikeToeAtRiver,
                points.DikeToeAtPolder,
                points.DitchDikeSide,
                points.BottomDitchDikeSide,
                points.BottomDitchPolderSide,
                points.DitchPolderSide
            }.Where(p => p != null);
        }
    }
}
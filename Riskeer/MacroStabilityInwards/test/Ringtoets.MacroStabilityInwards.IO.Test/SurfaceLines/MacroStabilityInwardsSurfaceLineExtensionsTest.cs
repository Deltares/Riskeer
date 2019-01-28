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
using Ringtoets.MacroStabilityInwards.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SurfaceLines
{
    [TestFixture]
    public class MacroStabilityInwardsSurfaceLineExtensionsTest
    {
        private static IEnumerable<TestCaseData> DifferentCharacteristicPointConfigurationsWithMissingMandatoryPoint
        {
            get
            {
                var name = "Missing SurfaceLevelOutside";
                CharacteristicPoints set = CreateCompleteCharacteristicPointSet(name);
                set.SurfaceLevelOutside = null;
                yield return new TestCaseData(set, RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelOutside).SetName(name);

                name = "Missing DikeToeAtRiver";
                set = CreateCompleteCharacteristicPointSet(name);
                set.DikeToeAtRiver = null;
                yield return new TestCaseData(set, RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtRiver).SetName(name);

                name = "Missing DikeTopAtPolder";
                set = CreateCompleteCharacteristicPointSet(name);
                set.DikeTopAtPolder = null;
                yield return new TestCaseData(set, RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtPolder).SetName(name);

                name = "Missing DikeTopAtRiver";
                set = CreateCompleteCharacteristicPointSet(name);
                set.DikeTopAtRiver = null;
                yield return new TestCaseData(set, RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtRiver).SetName(name);

                name = "Missing DikeToeAtPolder";
                set = CreateCompleteCharacteristicPointSet(name);
                set.DikeToeAtPolder = null;
                yield return new TestCaseData(set, RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtPolder).SetName(name);

                name = "Missing SurfaceLevelInside";
                set = CreateCompleteCharacteristicPointSet(name);
                set.SurfaceLevelInside = null;
                yield return new TestCaseData(set, RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelInside).SetName(name);
            }
        }

        private static IEnumerable<TestCaseData> DifferentCharacteristicPointConfigurationsWithMandatoryPointNotOnSurfaceLine
        {
            get
            {
                var name = "Moved SurfaceLevelOutside";
                CharacteristicPoints set = CreateCompleteCharacteristicPointSet(name);
                yield return new TestCaseData(
                    set,
                    new Action<CharacteristicPoints, Point3D>((points, p) => points.SurfaceLevelOutside = p),
                    RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelOutside).SetName(name);

                name = "Moved DikeToeAtRiver";
                set = CreateCompleteCharacteristicPointSet(name);
                yield return new TestCaseData(
                    set,
                    new Action<CharacteristicPoints, Point3D>((points, p) => points.DikeToeAtRiver = p),
                    RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtRiver).SetName(name);

                name = "Moved DikeTopAtPolder";
                set = CreateCompleteCharacteristicPointSet(name);
                yield return new TestCaseData(
                    set,
                    new Action<CharacteristicPoints, Point3D>((points, p) => points.DikeTopAtPolder = p),
                    RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtPolder).SetName(name);

                name = "Moved DikeTopAtRiver";
                set = CreateCompleteCharacteristicPointSet(name);
                yield return new TestCaseData(
                    set,
                    new Action<CharacteristicPoints, Point3D>((points, p) => points.DikeTopAtRiver = p),
                    RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtRiver).SetName(name);

                name = "Moved DikeToeAtPolder";
                set = CreateCompleteCharacteristicPointSet(name);
                yield return new TestCaseData(
                    set,
                    new Action<CharacteristicPoints, Point3D>((points, p) => points.DikeToeAtPolder = p),
                    RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtPolder).SetName(name);

                name = "Moved SurfaceLevelInside";
                set = CreateCompleteCharacteristicPointSet(name);
                yield return new TestCaseData(
                    set,
                    new Action<CharacteristicPoints, Point3D>((points, p) => points.SurfaceLevelInside = p),
                    RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelInside).SetName(name);
            }
        }

        private static IEnumerable<TestCaseData> DifferentValidCharacteristicPointConfigurations
        {
            get
            {
                var name = "All present";
                CharacteristicPoints set = CreateCompleteCharacteristicPointSet(name);
                yield return new TestCaseData(set).SetName(name);

                name = "Missing ShoulderBaseInside";
                set = CreateCompleteCharacteristicPointSet(name);
                set.ShoulderBaseInside = null;
                yield return new TestCaseData(set).SetName(name);

                name = "Missing ShoulderTopInside";
                set = CreateCompleteCharacteristicPointSet(name);
                set.ShoulderTopInside = null;
                yield return new TestCaseData(set).SetName(name);

                name = "Missing DitchDikeSide";
                set = CreateCompleteCharacteristicPointSet(name);
                set.DitchDikeSide = null;
                yield return new TestCaseData(set).SetName(name);

                name = "Missing BottomDitchDikeSide";
                set = CreateCompleteCharacteristicPointSet(name);
                set.BottomDitchDikeSide = null;
                yield return new TestCaseData(set).SetName(name);

                name = "Missing BottomDitchPolderSide";
                set = CreateCompleteCharacteristicPointSet(name);
                set.BottomDitchPolderSide = null;
                yield return new TestCaseData(set).SetName(name);

                name = "Missing DitchPolderSide";
                set = CreateCompleteCharacteristicPointSet(name);
                set.DitchPolderSide = null;
                yield return new TestCaseData(set).SetName(name);
            }
        }

        [Test]
        public void SetCharacteristicPoints_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsSurfaceLine) null).SetCharacteristicPoints(new CharacteristicPoints("Empty"));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void SetCharacteristicPoints_CharacteristicPointsNull_ThrowsImportedDataTransformException()
        {
            // Setup
            const string name = "Some line name";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);
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
            TestDelegate test = () => surfaceLine.SetCharacteristicPoints(null);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Karakteristieke punten definitie voor profielschematisatie '{name}' is verplicht.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(DifferentValidCharacteristicPointConfigurations))]
        public void SetCharacteristicPoints_ValidSituations_PointsAreSet(CharacteristicPoints points)
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(CharacteristicPointsToGeometry(points));

            // Call
            surfaceLine.SetCharacteristicPoints(points);

            // Assert
            Assert.AreEqual(points.DikeTopAtPolder, surfaceLine.DikeTopAtPolder);
            Assert.AreEqual(points.DikeTopAtRiver, surfaceLine.DikeTopAtRiver);
            Assert.AreEqual(points.ShoulderBaseInside, surfaceLine.ShoulderBaseInside);
            Assert.AreEqual(points.ShoulderTopInside, surfaceLine.ShoulderTopInside);
            Assert.AreEqual(points.SurfaceLevelOutside, surfaceLine.SurfaceLevelOutside);
            Assert.AreEqual(points.SurfaceLevelInside, surfaceLine.SurfaceLevelInside);
            Assert.AreEqual(points.DikeToeAtRiver, surfaceLine.DikeToeAtRiver);
            Assert.AreEqual(points.DikeToeAtPolder, surfaceLine.DikeToeAtPolder);
            Assert.AreEqual(points.DitchDikeSide, surfaceLine.DitchDikeSide);
            Assert.AreEqual(points.BottomDitchDikeSide, surfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(points.BottomDitchPolderSide, surfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(points.DitchPolderSide, surfaceLine.DitchPolderSide);
        }

        [Test]
        [TestCaseSource(nameof(DifferentCharacteristicPointConfigurationsWithMissingMandatoryPoint))]
        public void SetCharacteristicPoints_UndefinedMandatoryPoint_ThrowsImportedDataTransformException(CharacteristicPoints points, string pointDescription)
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(points.Name);
            surfaceLine.SetGeometry(CharacteristicPointsToGeometry(points));

            // Call
            TestDelegate test = () => surfaceLine.SetCharacteristicPoints(points);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            string message = $"Profielschematisatie '{points.Name}' kan niet gebruikt worden. Karakteristiek punt \'{pointDescription}\' is niet gedefiniëerd. Dit karakteristieke punt is verplicht.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(DifferentCharacteristicPointConfigurationsWithMandatoryPointNotOnSurfaceLine))]
        public void SetCharacteristicPoints_MandatoryPointNotOnSurfaceLine_ThrowsImportedDataTransformException(
            CharacteristicPoints points,
            Action<CharacteristicPoints, Point3D> setPoint,
            string pointDescription)
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(points.Name);
            surfaceLine.SetGeometry(CharacteristicPointsToGeometry(points));

            var changedPoint = new Point3D(-1, -1, -1);
            setPoint(points, changedPoint);

            // Call
            TestDelegate test = () => surfaceLine.SetCharacteristicPoints(points);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            string message = $"Profielschematisatie '{points.Name}' kan niet gebruikt worden. " +
                             $"De geometrie bevat geen punt op locatie {changedPoint} om als \'{pointDescription}\' in te stellen. " +
                             "Dit karakteristieke punt is verplicht.";
            Assert.AreEqual(message, exception.Message);
        }

        private static IEnumerable<Point3D> CharacteristicPointsToGeometry(CharacteristicPoints points)
        {
            return new[]
            {
                points.DikeTopAtPolder,
                points.DikeTopAtRiver,
                points.ShoulderBaseInside,
                points.ShoulderTopInside,
                points.SurfaceLevelOutside,
                points.SurfaceLevelInside,
                points.DikeToeAtRiver,
                points.DikeToeAtPolder,
                points.DitchDikeSide,
                points.BottomDitchDikeSide,
                points.BottomDitchPolderSide,
                points.DitchPolderSide
            }.Where(p => p != null);
        }

        private static CharacteristicPoints CreateCompleteCharacteristicPointSet(string name)
        {
            var surfaceLevelOutside = new Point3D(2, 2, 5);
            var dikeToeAtRiver = new Point3D(2.1, 2, 5);
            var dikeTopAtPolder = new Point3D(2.7, 2, 5);
            var dikeTopAtRiver = new Point3D(2.6, 2, 5);
            var shoulderBaseInside = new Point3D(3.2, 2, 5);
            var shoulderTopInside = new Point3D(3.5, 2, 5);
            var dikeToeAtPolder = new Point3D(4.4, 3, 8);
            var ditchDikeSide = new Point3D(6.3, 6, 8);
            var bottomDitchDikeSide = new Point3D(5.1, 6, 6.5);
            var bottomDitchPolderSide = new Point3D(8.5, 7.2, 4.2);
            var ditchPolderSide = new Point3D(9.6, 7.5, 3.9);
            var surfaceLevelInside = new Point3D(10.1, 2, 5);

            return new CharacteristicPoints(name)
            {
                DikeTopAtPolder = dikeTopAtPolder,
                ShoulderBaseInside = shoulderBaseInside,
                ShoulderTopInside = shoulderTopInside,
                SurfaceLevelOutside = surfaceLevelOutside,
                SurfaceLevelInside = surfaceLevelInside,
                DikeToeAtRiver = dikeToeAtRiver,
                DikeToeAtPolder = dikeToeAtPolder,
                DikeTopAtRiver = dikeTopAtRiver,
                DitchDikeSide = ditchDikeSide,
                BottomDitchDikeSide = bottomDitchDikeSide,
                BottomDitchPolderSide = bottomDitchPolderSide,
                DitchPolderSide = ditchPolderSide
            };
        }
    }
}
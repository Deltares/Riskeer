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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SurfaceLines
{
    [TestFixture]
    public class MacroStabilityInwardsSurfaceLineTransformerTest
    {
        private static IEnumerable<TestCaseData> MoveOptionalCharacteristicPoint
        {
            get
            {
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.ShoulderBaseInside = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.ShoulderBaseInside),
                        "Insteek binnenberm")
                    .SetName("Move ShoulderBaseInside");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.ShoulderTopInside = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.ShoulderTopInside),
                        "Kruin binnenberm")
                    .SetName("Move ShoulderTopInside");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.DitchDikeSide = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.DitchDikeSide),
                        "Insteek sloot dijkzijde")
                    .SetName("Move DitchDikeSide");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.BottomDitchDikeSide = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.BottomDitchDikeSide),
                        "Slootbodem dijkzijde")
                    .SetName("Move BottomDitchDikeSide");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.BottomDitchPolderSide = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.BottomDitchPolderSide),
                        "Slootbodem polderzijde")
                    .SetName("Move BottomDitchPolderSide");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.DitchPolderSide = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.DitchPolderSide),
                        "Insteek sloot polderzijde")
                    .SetName("Move DitchPolderSide");
            }
        }

        private static IEnumerable<TestCaseData> MoveMandatoryCharacteristicPoint
        {
            get
            {
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.SurfaceLevelOutside = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.SurfaceLevelOutside),
                        "Maaiveld buitenwaarts")
                    .SetName("Move SurfaceLevelOutside");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.DikeToeAtRiver = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.DikeToeAtRiver),
                        "Teen dijk buitenwaarts")
                    .SetName("Move DikeToeAtRiver");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.DikeTopAtPolder = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.DikeTopAtPolder),
                        "Kruin binnentalud")
                    .SetName("Move DikeTopAtPolder");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.DikeTopAtRiver = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.DikeTopAtRiver),
                        "Kruin buitentalud")
                    .SetName("Move DikeTopAtRiver");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.DikeToeAtPolder = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.DikeToeAtPolder),
                        "Teen dijk binnenwaarts")
                    .SetName("Move DikeToeAtPolder");
                yield return new TestCaseData(
                        new Action<CharacteristicPoints, Point3D>((cp, p) => cp.SurfaceLevelInside = p),
                        new Func<MacroStabilityInwardsSurfaceLine, Point3D>(sl => sl.SurfaceLevelInside),
                        "Maaiveld binnenwaarts")
                    .SetName("Move SurfaceLevelInside");
            }
        }

        [Test]
        public void Constructor_WithoutReferenceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSurfaceLineTransformer(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Transform_SurfaceLineNotOnReferenceLine_ThrowsImportedDataTransformException()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var transformer = new MacroStabilityInwardsSurfaceLineTransformer(referenceLine);

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 4.0, 2.1),
                new Point3D(3.0, 5.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(2.0, 4.0)
            });

            // Call
            TestDelegate test = () => transformer.Transform(surfaceLine, null);

            // Assert
            string message = $"Profielschematisatie {surfaceLineName} doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd. Dit kan komen doordat de profielschematisatie een lokaal coördinaatsysteem heeft.";
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void Transform_SurfaceLineIntersectsReferenceLineMultipleTimes_ThrowsImportedDataTransformException()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var transformer = new MacroStabilityInwardsSurfaceLineTransformer(referenceLine);

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 5.0, 2.1),
                new Point3D(1.0, 3.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 5.0),
                new Point2D(2.0, 4.0),
                new Point2D(0.0, 3.0)
            });

            // Call
            TestDelegate test = () => transformer.Transform(surfaceLine, null);

            // Assert
            string message = $"Profielschematisatie {surfaceLineName} doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd.";
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void Transform_WithoutCharacteristicPoints_ThrowsImportedDataTransformException()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var transformer = new MacroStabilityInwardsSurfaceLineTransformer(referenceLine);
            const string locationName = "a location";

            var random = new Random(21);
            double randomZ = random.NextDouble();

            var surfaceLine = new SurfaceLine
            {
                Name = locationName
            };

            var point1 = new Point3D(3.5, 4.8, randomZ);
            var point2 = new Point3D(7.2, 9.3, randomZ);
            var point3 = new Point3D(12.0, 5.6, randomZ);

            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2,
                point3
            });

            referenceLine.SetGeometry(new[]
            {
                new Point2D(5.6, 2.5),
                new Point2D(6.8, 15)
            });

            MacroStabilityInwardsSurfaceLine result = null;

            // Call
            TestDelegate test = () => result = transformer.Transform(surfaceLine, null);

            // Assert
            string message = $"Karakteristieke punten definitie voor profielschematisatie '{locationName}' is verplicht.";
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource(nameof(MoveOptionalCharacteristicPoint))]
        public void Transform_OptionalCharacteristicPointNotOnSurfaceLine_LogErrorAndReturnSurfaceLineWithoutCharacteristicPointSet(
            Action<CharacteristicPoints, Point3D> pointChange,
            Func<MacroStabilityInwardsSurfaceLine, Point3D> pointWhichIsNull,
            string changedCharacteristicPointName)
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var transformer = new MacroStabilityInwardsSurfaceLineTransformer(referenceLine);
            const string locationName = "a location";

            var random = new Random(21);
            double randomZ = random.NextDouble();

            var surfaceLine = new SurfaceLine
            {
                Name = locationName
            };

            var point1 = new Point3D(3.5, 4.8, randomZ);
            var point2 = new Point3D(7.2, 9.3, randomZ);
            var point3 = new Point3D(12.0, 5.6, randomZ);
            var notOnSurfaceLinePoint = new Point3D(7.3, 9.3, randomZ);

            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2,
                point3
            });

            var characteristicPoints = new CharacteristicPoints(locationName)
            {
                SurfaceLevelOutside = point2,
                DikeToeAtRiver = point2,
                DikeTopAtPolder = point2,
                DikeTopAtRiver = point2,
                ShoulderBaseInside = point3,
                ShoulderTopInside = point3,
                BottomDitchDikeSide = point3,
                BottomDitchPolderSide = point3,
                DitchPolderSide = point3,
                DitchDikeSide = point3,
                DikeToeAtPolder = point3,
                SurfaceLevelInside = point3
            };

            pointChange(characteristicPoints, notOnSurfaceLinePoint);

            referenceLine.SetGeometry(new[]
            {
                new Point2D(5.6, 2.5),
                new Point2D(6.8, 15)
            });

            MacroStabilityInwardsSurfaceLine result = null;

            // Call
            Action call = () => result = transformer.Transform(surfaceLine, characteristicPoints);

            // Assert
            string message = $"Karakteristiek punt van profielschematisatie '{locationName}' is overgeslagen. De geometrie bevat geen punt op locatie {notOnSurfaceLinePoint} om als '{changedCharacteristicPointName}' in te stellen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error));
            CollectionAssert.AreEqual(new[]
            {
                point1,
                point2,
                point3
            }, result.Points);
            Assert.IsNull(pointWhichIsNull(result));
            Assert.AreEqual(1, new Collection<Point3D>
            {
                result.SurfaceLevelOutside,
                result.DikeToeAtRiver,
                result.DikeTopAtPolder,
                result.ShoulderBaseInside,
                result.ShoulderTopInside,
                result.BottomDitchDikeSide,
                result.BottomDitchPolderSide,
                result.DitchPolderSide,
                result.DitchDikeSide,
                result.DikeToeAtPolder,
                result.SurfaceLevelInside
            }.Count(p => p == null));
        }

        [Test]
        [TestCaseSource(nameof(MoveMandatoryCharacteristicPoint))]
        public void Transform_MandatoryCharacteristicPointNotOnSurfaceLine_ThrowsImportedDataTransformException(
            Action<CharacteristicPoints, Point3D> pointChange,
            Func<MacroStabilityInwardsSurfaceLine, Point3D> pointWhichIsNull,
            string changedCharacteristicPointName)
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var transformer = new MacroStabilityInwardsSurfaceLineTransformer(referenceLine);
            const string locationName = "a location";

            var random = new Random(21);
            double randomZ = random.NextDouble();

            var surfaceLine = new SurfaceLine
            {
                Name = locationName
            };

            var point1 = new Point3D(3.5, 4.8, randomZ);
            var point2 = new Point3D(7.2, 9.3, randomZ);
            var point3 = new Point3D(12.0, 5.6, randomZ);
            var notOnSurfaceLinePoint = new Point3D(7.3, 9.3, randomZ);

            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2,
                point3
            });

            var characteristicPoints = new CharacteristicPoints(locationName)
            {
                SurfaceLevelOutside = point2,
                DikeToeAtRiver = point2,
                DikeTopAtPolder = point2,
                DikeTopAtRiver = point2,
                ShoulderBaseInside = point3,
                ShoulderTopInside = point3,
                BottomDitchDikeSide = point3,
                BottomDitchPolderSide = point3,
                DitchPolderSide = point3,
                DitchDikeSide = point3,
                DikeToeAtPolder = point3,
                SurfaceLevelInside = point3
            };

            pointChange(characteristicPoints, notOnSurfaceLinePoint);

            referenceLine.SetGeometry(new[]
            {
                new Point2D(5.6, 2.5),
                new Point2D(6.8, 15)
            });

            // Call
            TestDelegate test = () => transformer.Transform(surfaceLine, characteristicPoints);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            string message = $"Profielschematisatie '{locationName}' kan niet gebruikt worden. " +
                             $"De geometrie bevat geen punt op locatie {notOnSurfaceLinePoint} om als \'{changedCharacteristicPointName}\' in te stellen. " +
                             "Dit karakteristieke punt is verplicht.";
            Assert.AreEqual(message, exception.Message);
        }
    }
}
// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Projections;
using Core.Components.DotSpatial.Test.Properties;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using NUnit.Framework;
using WorldFile = Core.Components.DotSpatial.Projections.WorldFile;

namespace Core.Components.DotSpatial.Test.Projections
{
    [TestFixture]
    public class TileReprojectorTest
    {
        [Test]
        public void Constructor_MapArgsNull_ThrowArgumentNullException()
        {
            // Setup
            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            TestDelegate call = () => new TileReprojector(null, projection, projection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("mapArgs", paramName);
        }

        [Test]
        public void Reproject_SourceProjectionNull_ReturnSourceMaterial()
        {
            // Setup
            ProjectionInfo target = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
            var mapArgs = new MapArgs(new Rectangle(), new Extent());

            var projector = new TileReprojector(mapArgs, null, target);

            var sourceReference = new WorldFile(1.0, 0.0, 0.0, -1.0, 0.0, 0.0);
            Bitmap sourceTile = Resources.testImage;

            WorldFile targetReference;
            Bitmap targetTile;

            // Call
            projector.Reproject(sourceReference, sourceTile, out targetReference, out targetTile);

            // Assert
            Assert.AreSame(sourceReference, targetReference);
            Assert.AreSame(sourceTile, targetTile);
        }

        [Test]
        public void Reproject_TargetProjectionNull_ReturnSourceMaterial()
        {
            // Setup
            ProjectionInfo source = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
            var mapArgs = new MapArgs(new Rectangle(), new Extent());

            var projector = new TileReprojector(mapArgs, source, null);

            var sourceReference = new WorldFile(1.0, 0.0, 0.0, -1.0, 0.0, 0.0);
            Bitmap sourceTile = Resources.testImage;

            WorldFile targetReference;
            Bitmap targetTile;

            // Call
            projector.Reproject(sourceReference, sourceTile, out targetReference, out targetTile);

            // Assert
            Assert.AreSame(sourceReference, targetReference);
            Assert.AreSame(sourceTile, targetTile);
        }

        [Test]
        public void Reproject_SameProjection_ReturnSourceMaterial()
        {
            // Setup
            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
            var mapArgs = new MapArgs(new Rectangle(), new Extent());

            var projector = new TileReprojector(mapArgs, projection, projection);

            var sourceReference = new WorldFile(1.0, 0.0, 0.0, -1.0, 0.0, 0.0);
            Bitmap sourceTile = Resources.testImage;

            WorldFile targetReference;
            Bitmap targetTile;

            // Call
            projector.Reproject(sourceReference, sourceTile, out targetReference, out targetTile);

            // Assert
            Assert.AreSame(sourceReference, targetReference);
            Assert.AreSame(sourceTile, targetTile);
        }

        [Test]
        public void Reproject_TargetTileWouldNotBeVisibleInViewport_ReturnNull()
        {
            // Setup
            ProjectionInfo sourceProjection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
            ProjectionInfo targetProjection = KnownCoordinateSystems.Projected.World.WebMercator;
            var mapArgs = new MapArgs(new Rectangle(0, 0, 10, 10), new Extent(5, 50, 10, 100));

            var projector = new TileReprojector(mapArgs, sourceProjection, targetProjection);

            var sourceReference = new WorldFile(1.0, 0.0, 0.0, -1.0, 0.0, 0.0);
            Bitmap sourceTile = Resources.testImage;

            WorldFile targetReference;
            Bitmap targetTile;

            // Call
            projector.Reproject(sourceReference, sourceTile, out targetReference, out targetTile);

            // Assert
            Assert.IsNull(targetReference);
            Assert.IsNull(targetTile);
        }

        [Test]
        public void Reproject_DifferentCoordinateSystems_ReprojectImageAndMetaData()
        {
            // Setup
            var mapArgs = new MapArgs(new Rectangle(0, 0, 722, 349),
                                      new Extent(520981.864447542, 6853700.54100246, 709995.365081098, 6945065.79269375));
            ProjectionInfo sourceProjection = ProjectionInfo.FromEpsgCode(25831);
            ProjectionInfo targetProjection = KnownCoordinateSystems.Projected.World.WebMercator;
            var projector = new TileReprojector(mapArgs, sourceProjection, targetProjection);

            var sourceReference = new WorldFile(140, 0.0, 0.0, -140, 641716.59261121, 5825498);
            Bitmap sourceTile = Resources.source;

            WorldFile targetReference;
            Bitmap targetTile;

            // Call
            projector.Reproject(sourceReference, sourceTile, out targetReference, out targetTile);

            // Assert
            // Note: These ground truth values have been defined using https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Reprojection/TileReprojector.cs
            Assert.AreEqual(261.791552124038, targetReference.A11, 1e-8);
            Assert.AreEqual(0.0, targetReference.A21);
            Assert.AreEqual(0.0, targetReference.A12);
            Assert.AreEqual(-261.79155212403651, targetReference.A22, 1e-8);
            Assert.AreEqual(564962.84520438069, targetReference.B1, 1e-8);
            Assert.AreEqual(6902131.9781454066, targetReference.B2, 1e-8);

            TestHelper.AssertImagesAreEqual(Resources.target, targetTile);
        }
    }
}
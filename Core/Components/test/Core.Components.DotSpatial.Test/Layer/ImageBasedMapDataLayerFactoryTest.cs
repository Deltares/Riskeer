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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using BruTile;
using BruTile.Predefined;
using Core.Common.TestUtil;
using Core.Common.Util.TestUtil.Settings;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.DotSpatial.Layer;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.Gis.Data;
using Core.Components.Gis.Exceptions;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.DotSpatial.Test.Layer
{
    [TestFixture]
    public class ImageBasedMapDataLayerFactoryTest
    {
        [Test]
        public void Create_MapDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ImageBasedMapDataLayerFactory.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("backgroundMapData", exception.ParamName);
        }

        [Test]
        public void Create_MapDataNotWellKnownOrWmts_ThrowNotSupportedException()
        {
            // Setup
            var mapData = new TestImageBasedMapData("test", true);

            // Call
            TestDelegate test = () => ImageBasedMapDataLayerFactory.Create(mapData);

            // Assert
            var exception = Assert.Throws<NotSupportedException>(test);
            Assert.AreEqual($"Cannot create a configuration for type {mapData.GetType()}.", exception.Message);
        }

        [Test]
        public void Create_WellKnownTileSourceFactoryThrowsNotSupportedException_ThrowConfigurationInitializationException()
        {
            // Setup
            var factoryThrowingNotSupportedException = MockRepository.GenerateStub<ITileSourceFactory>();
            factoryThrowingNotSupportedException.Stub(f => f.GetKnownTileSource(Arg<KnownTileSource>.Is.NotNull))
                                                .Throw(new NotSupportedException());

            using (new UseCustomTileSourceFactoryConfig(factoryThrowingNotSupportedException))
            {
                var wellKnownMapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);

                // Call
                TestDelegate test = () => ImageBasedMapDataLayerFactory.Create(wellKnownMapData);

                // Assert
                var exception = Assert.Throws<ConfigurationInitializationException>(test);
                Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
                Assert.AreEqual("Verbinden met 'Bing Maps - Satelliet' is mislukt waardoor geen kaartgegevens ingeladen kunnen worden.", exception.Message);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetBackgroundData), new object[]
        {
            "CannotCreateFileCache"
        })]
        public void Create_CannotCreateFileCache_ThrowConfigurationInitializationException(ImageBasedMapData mapData)
        {
            // Setup
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = Path.Combine(TestHelper.GetScratchPadPath(), nameof(ImageBasedMapDataLayerFactoryTest))
            }))
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(ImageBasedMapDataLayerFactoryTest)))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                TestDelegate test = () => ImageBasedMapDataLayerFactory.Create(mapData);

                // Assert
                var exception = Assert.Throws<ConfigurationInitializationException>(test);
                Assert.IsInstanceOf<CannotCreateTileCacheException>(exception.InnerException);
                Assert.AreEqual("Configuratie van kaartgegevens hulpbestanden is mislukt.", exception.Message);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetProblematicTileSourceFactoryTestCaseData), new object[]
        {
            "Create"
        })]
        public void Create_ProblematicTileSourceFactory_ThrowConfigurationInitializationException(ITileSourceFactory factory)
        {
            // Setup
            WmtsMapData backgroundMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            using (new UseCustomTileSourceFactoryConfig(factory))
            {
                // Call
                TestDelegate test = () => ImageBasedMapDataLayerFactory.Create(backgroundMapData);

                // Assert
                var exception = Assert.Throws<ConfigurationInitializationException>(test);
                Assert.IsInstanceOf<CannotFindTileSourceException>(exception.InnerException);
                Assert.AreEqual("Verbinden met WMTS is mislukt waardoor geen kaartgegevens ingeladen kunnen worden.", exception.Message);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetBackgroundData), new object[]
        {
            "ValidData"
        })]
        public void Create_ValidData_ReturnBruTileLayer(ImageBasedMapData mapData)
        {
            // Setup
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = Path.Combine(TestHelper.GetScratchPadPath(), nameof(ImageBasedMapDataLayerFactoryTest))
            }))
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(ImageBasedMapDataLayerFactoryTest)))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                // Call
                BruTileLayer layer = ImageBasedMapDataLayerFactory.Create(mapData);

                // Assert
                Assert.AreEqual(mapData.Transparency.Value, layer.Transparency);
                Assert.AreEqual(mapData.IsVisible, layer.IsVisible);
            }
        }

        /// <summary>
        /// Generates <see cref="TestCaseData"/> containing problematic <see cref="ITileSourceFactory"/>.
        /// </summary>
        /// <param name="prefix">The test-name prefix.</param>
        /// <returns>The data for the test cases.</returns>
        /// <remarks>Some test runners, like TeamCity, cannot properly deal with reuse of
        /// <see cref="TestCaseData"/> sources where the source defines a name of the test,
        /// as these testrunners to not display tests in hierarchical form.</remarks>
        private static IEnumerable<TestCaseData> GetProblematicTileSourceFactoryTestCaseData(string prefix)
        {
            var factoryWithoutRequiredTileSource = MockRepository.GenerateStub<ITileSourceFactory>();
            factoryWithoutRequiredTileSource.Stub(f => f.GetWmtsTileSources(Arg<string>.Is.NotNull))
                                            .Return(Enumerable.Empty<ITileSource>());

            var factoryThrowingCannotFindTileSourceException = MockRepository.GenerateStub<ITileSourceFactory>();
            factoryThrowingCannotFindTileSourceException.Stub(f => f.GetWmtsTileSources(Arg<string>.Is.NotNull))
                                                        .Throw(new CannotFindTileSourceException());

            yield return new TestCaseData(factoryWithoutRequiredTileSource)
                .SetName($"{prefix}: Required tile source not returned by factory.");

            yield return new TestCaseData(factoryThrowingCannotFindTileSourceException)
                .SetName($"{prefix}: Tile source factory throws CannotFindTileSourceException.");
        }

        private static IEnumerable<TestCaseData> GetBackgroundData(string prefix)
        {
            yield return new TestCaseData(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial))
                .SetName($"{prefix}: WellKnownMapData");

            yield return new TestCaseData(WmtsMapDataTestHelper.CreateDefaultPdokMapData())
                .SetName($"{prefix}: WmtsMapData");
        }
    }
}
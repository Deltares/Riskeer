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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil;
using Riskeer.Storage.Core.TestUtil.MacroStabilityInwards;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class StochasticSoilModelEntityReadExtensionsTest
    {
        [Test]
        public void ReadAsPipingStochasticSoilModel_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StochasticSoilModelEntity();

            // Call
            TestDelegate test = () => entity.ReadAsPipingStochasticSoilModel(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void ReadAsPipingStochasticSoilModel_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => ((StochasticSoilModelEntity) null).ReadAsPipingStochasticSoilModel(collector);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void ReadAsPipingStochasticSoilModel_StochasticSoilModelSegmentPointXmlEmpty_ThrowsArgumentException()
        {
            // Setup
            var entity = new StochasticSoilModelEntity
            {
                Name = "Name",
                StochasticSoilModelSegmentPointXml = string.Empty
            };

            // Call
            TestDelegate test = () => entity.ReadAsPipingStochasticSoilModel(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void ReadAsPipingStochasticSoilModel_WithValidEntity_ReturnsNewStochasticSoilModelWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            var entity = new StochasticSoilModelEntity
            {
                Name = "StochasticSoilModel",
                StochasticSoilModelSegmentPointXml = new Point2DCollectionXmlSerializer().ToXml(geometry),
                PipingStochasticSoilProfileEntities =
                {
                    new PipingStochasticSoilProfileEntity
                    {
                        PipingSoilProfileEntity = new PipingSoilProfileEntity
                        {
                            PipingSoilLayerEntities =
                            {
                                new PipingSoilLayerEntity()
                            },
                            Name = "A"
                        },
                        Order = 1
                    },
                    new PipingStochasticSoilProfileEntity
                    {
                        PipingSoilProfileEntity = new PipingSoilProfileEntity
                        {
                            PipingSoilLayerEntities =
                            {
                                new PipingSoilLayerEntity()
                            },
                            Name = "B"
                        },
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilModel model = entity.ReadAsPipingStochasticSoilModel(collector);

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(entity.Name, model.Name);
            CollectionAssert.AreEqual(geometry, model.Geometry);

            Assert.AreEqual(entity.PipingStochasticSoilProfileEntities.Count, model.StochasticSoilProfiles.Count());
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, model.StochasticSoilProfiles.Select(ssp => ssp.SoilProfile.Name));
        }

        [Test]
        public void ReadAsPipingStochasticSoilModel_SameStochasticSoilModelEntityMultipleTimes_ReturnSameStochasticSoilModel()
        {
            // Setup
            var random = new Random(21);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };
            var entity = new StochasticSoilModelEntity
            {
                Name = "StochasticSoilModel",
                StochasticSoilModelSegmentPointXml = new Point2DCollectionXmlSerializer().ToXml(geometry),
                PipingStochasticSoilProfileEntities =
                {
                    PipingStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilModel soilModel1 = entity.ReadAsPipingStochasticSoilModel(collector);
            PipingStochasticSoilModel soilModel2 = entity.ReadAsPipingStochasticSoilModel(collector);

            // Assert
            Assert.AreSame(soilModel1, soilModel2);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsStochasticSoilModel_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StochasticSoilModelEntity();

            // Call
            TestDelegate test = () => entity.ReadAsMacroStabilityInwardsStochasticSoilModel(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsStochasticSoilModel_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => ((StochasticSoilModelEntity) null).ReadAsMacroStabilityInwardsStochasticSoilModel(collector);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsStochasticSoilModel_StochasticSoilModelSegmentPointXmlEmpty_ThrowsArgumentException()
        {
            // Setup
            var entity = new StochasticSoilModelEntity
            {
                Name = "Name",
                StochasticSoilModelSegmentPointXml = string.Empty
            };

            // Call
            TestDelegate test = () => entity.ReadAsMacroStabilityInwardsStochasticSoilModel(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsStochasticSoilModel_WithValidEntity_ReturnsNewStochasticSoilModelWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            var entity = new StochasticSoilModelEntity
            {
                Name = "testName",
                StochasticSoilModelSegmentPointXml = new Point2DCollectionXmlSerializer().ToXml(geometry),
                MacroStabilityInwardsStochasticSoilProfileEntities =
                {
                    new MacroStabilityInwardsStochasticSoilProfileEntity
                    {
                        MacroStabilityInwardsSoilProfileOneDEntity = new MacroStabilityInwardsSoilProfileOneDEntity
                        {
                            MacroStabilityInwardsSoilLayerOneDEntities =
                            {
                                new MacroStabilityInwardsSoilLayerOneDEntity()
                            },
                            Name = "A"
                        },
                        Order = 1
                    },
                    new MacroStabilityInwardsStochasticSoilProfileEntity
                    {
                        MacroStabilityInwardsSoilProfileTwoDEntity = new MacroStabilityInwardsSoilProfileTwoDEntity
                        {
                            MacroStabilityInwardsSoilLayerTwoDEntities =
                            {
                                MacroStabilityInwardsSoilLayerTwoDEntityTestFactory.CreateMacroStabilityInwardsSoilLayerTwoDEntity()
                            },
                            Name = "B"
                        },
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsStochasticSoilModel model = entity.ReadAsMacroStabilityInwardsStochasticSoilModel(collector);

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(entity.Name, model.Name);
            CollectionAssert.AreEqual(geometry, model.Geometry);

            Assert.AreEqual(entity.MacroStabilityInwardsStochasticSoilProfileEntities.Count,
                            model.StochasticSoilProfiles.Count());
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, model.StochasticSoilProfiles.Select(ssp => ssp.SoilProfile.Name));
        }

        [Test]
        public void ReadAsMacroStabilityInwardsStochasticSoilModel_SameStochasticSoilModelEntityMultipleTimes_ReturnSameStochasticSoilModel()
        {
            // Setup
            var random = new Random(21);
            string segmentPointsXml = new Point2DCollectionXmlSerializer().ToXml(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            });

            var entity = new StochasticSoilModelEntity
            {
                Name = "StochasticSoilModel",
                StochasticSoilModelSegmentPointXml = segmentPointsXml,
                MacroStabilityInwardsStochasticSoilProfileEntities =
                {
                    MacroStabilityInwardsStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsStochasticSoilModel soilModel1 = entity.ReadAsMacroStabilityInwardsStochasticSoilModel(collector);
            MacroStabilityInwardsStochasticSoilModel soilModel2 = entity.ReadAsMacroStabilityInwardsStochasticSoilModel(collector);

            // Assert
            Assert.AreSame(soilModel1, soilModel2);
        }
    }
}
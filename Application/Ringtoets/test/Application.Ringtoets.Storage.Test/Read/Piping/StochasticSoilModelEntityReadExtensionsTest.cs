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

using System;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.Piping;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Data.SoilProfile;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class StochasticSoilModelEntityReadExtensionsTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StochasticSoilModelEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Read_StochasticSoilModelSegmentPointXmlNullOrEmpty_ThrowsArgumentException(string xml)
        {
            // Setup
            var entity = new StochasticSoilModelEntity
            {
                Name = "Name",
                StochasticSoilModelSegmentPointXml = xml
            };

            // Call
            TestDelegate test = () => entity.Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewStochasticSoilModelWithPropertiesSetAndEntityRegistered()
        {
            // Setup
            const string testName = "testName";
            var entity = new StochasticSoilModelEntity
            {
                Name = testName,
                StochasticSoilModelSegmentPointXml = new Point2DXmlSerializer().ToXml(new Point2D[0])
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilModel model = entity.Read(collector);

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(testName, model.Name);
            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_WithCollectorWithStochasticSoilProfiles_ReturnsNewStochasticSoilModelWithStochasticSoilProfiles()
        {
            // Setup
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelSegmentPointXml = new Point2DXmlSerializer().ToXml(new Point2D[0]),
                StochasticSoilProfileEntities =
                {
                    new StochasticSoilProfileEntity
                    {
                        SoilProfileEntity = new SoilProfileEntity
                        {
                            SoilLayerEntities =
                            {
                                new SoilLayerEntity()
                            },
                            Name = "A"
                        },
                        Order = 1
                    },
                    new StochasticSoilProfileEntity
                    {
                        SoilProfileEntity = new SoilProfileEntity
                        {
                            SoilLayerEntities =
                            {
                                new SoilLayerEntity()
                            },
                            Name = "B"
                        },
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilModel model = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, model.StochasticSoilProfiles.Select(ssp => ssp.SoilProfile.Name));
        }

        [Test]
        public void Read_WithCollectorWithStochasticSoilModelSegmentPointEntity_ReturnsNewStochasticSoilModelWithGeometryPoints()
        {
            // Setup
            var segmentPoints = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelSegmentPointXml = new Point2DXmlSerializer().ToXml(segmentPoints)
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilModel model = entity.Read(collector);

            // Assert
            CollectionAssert.AreEqual(segmentPoints, model.Geometry);
        }

        [Test]
        public void Read_SameStochasticSoilModelEntityMultipleTimes_ReturnSameStochasticSoilModel()
        {
            // Setup
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelSegmentPointXml = new Point2DXmlSerializer().ToXml(new Point2D[0])
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilModel soilModel1 = entity.Read(collector);
            PipingStochasticSoilModel soilModel2 = entity.Read(collector);

            // Assert
            Assert.AreSame(soilModel1, soilModel2);
        }
    }
}
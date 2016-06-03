// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read
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
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewStochasticSoilModelWithPropertiesSet()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            string testName = "testName";
            string testSegmentName = "testSegmentName";
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = entityId,
                Name = testName,
                SegmentName = testSegmentName,
            };
            var collector = new ReadConversionCollector();

            // Call
            var model = entity.Read(collector);

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(entityId, model.StorageId);
            Assert.AreEqual(testName, model.Name);
            Assert.AreEqual(testSegmentName, model.SegmentName);
        } 

        [Test]
        public void Read_WithCollectorWithStochasticSoilProfiles_ReturnsNewStochasticSoilModelWithStochasticSoilProfiles()
        {
            // Setup
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilProfileEntities =
                {
                    new StochasticSoilProfileEntity
                    {
                        SoilProfileEntity = new SoilProfileEntity
                        {
                            SoilLayerEntities =
                            {
                                new SoilLayerEntity()
                            }
                        }
                    },
                    new StochasticSoilProfileEntity
                    {
                        SoilProfileEntity = new SoilProfileEntity
                        {
                            SoilLayerEntities =
                            {
                                new SoilLayerEntity()
                            }
                        }
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var model = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
        }

        [Test]
        public void Read_WithCollectorWithStochasticSoilModelSegmentPointEntity_ReturnsNewStochasticSoilModelWithGeometryPoints()
        {
            // Setup
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelSegmentPointEntities =
                {
                    new StochasticSoilModelSegmentPointEntity(),
                    new StochasticSoilModelSegmentPointEntity()
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var model = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, model.Geometry.Count);
        }

        [Test]
        public void Read_SameStochasticSoilModelEntityMultipleTimes_ReturnSameStochasticSoilModel()
        {
            // Setup
            var entity = new StochasticSoilModelEntity();

            var collector = new ReadConversionCollector();

            // Call
            StochasticSoilModel soilModel1 = entity.Read(collector);
            StochasticSoilModel soilModel2 = entity.Read(collector);

            // Assert
            Assert.AreSame(soilModel1, soilModel2);
        }
    }
}
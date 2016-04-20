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
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class StochasticSoilModelConverterTest
    {
        [Test]
        public void Constructor_Always_NewInstance()
        {
            // Call
            var converter = new StochasticSoilModelConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<StochasticSoilModel, StochasticSoilModelEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ConvertEntityToModel_Always_ReturnsTheEntityAsModelWithId()
        {
            // Setup
            var storageId = new Random(21).Next();
            var segmentName = "SomeSegmentName";
            var name = "SomeName";
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = storageId,
                Name = name,
                SegmentName = segmentName
            };
            var converter = new StochasticSoilModelConverter();

            // Call
            StochasticSoilModel model = converter.ConvertEntityToModel(entity);

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
            Assert.AreEqual(name, model.Name);
            Assert.AreEqual(segmentName, model.SegmentName);
        }

        [Test]
        public void ConvertEntityToModel_WithStochasticSoilProfiles_ReturnsTheEntityAsModelWithoutStochasticSoilProfiles()
        {
            // Setup
            var storageId = new Random(21).Next();
            var segmentName = "SomeSegmentName";
            var name = "SomeName";
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = storageId,
                Name = name,
                SegmentName = segmentName,
                StochasticSoilProfileEntities =
                {
                    new StochasticSoilProfileEntity
                    {
                        Probability = Convert.ToDecimal(3.0)
                    },
                    new StochasticSoilProfileEntity
                    {
                        Probability = Convert.ToDecimal(8.0)
                    }
                }
            };
            var converter = new StochasticSoilModelConverter();

            // Call
            StochasticSoilModel model = converter.ConvertEntityToModel(entity);

            // Assert
            Assert.AreEqual(storageId, model.StorageId);
            Assert.AreEqual(name, model.Name);
            Assert.AreEqual(segmentName, model.SegmentName);

            Assert.IsEmpty(model.StochasticSoilProfiles);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, new StochasticSoilModelEntity());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("modelObject", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(new StochasticSoilModel(-1, string.Empty, string.Empty), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_ValidModelValidEntity_ReturnsModelAsEntity()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();
            var random = new Random(21);
            var entity = new StochasticSoilModelEntity();

            string segmentName = "someSegmentName";
            string name = "someName";
            long id = random.Next();
            long storageId = random.Next();
            var model = new StochasticSoilModel(id, name, segmentName)
            {
                StorageId = storageId
            };

            // Call
            converter.ConvertModelToEntity(model, entity);

            // Assert
            Assert.AreEqual(storageId, entity.StochasticSoilModelEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(segmentName, entity.SegmentName);
        }

        [Test]
        
        public void ConvertModelToEntity_ValidModelValidEntityWithStochasticSoilProfiles_ReturnsModelAsEntityWithoutStochasticSoilProfiles()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();
            var random = new Random(21);
            var entity = new StochasticSoilModelEntity();

            string segmentName = "someSegmentName";
            string name = "someName";
            long id = random.Next();
            long storageId = random.Next();
            var model = new StochasticSoilModel(id, name, segmentName)
            {
                StorageId = storageId,
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(3.0, SoilProfileType.SoilProfile1D, -1),
                    new StochasticSoilProfile(8.0, SoilProfileType.SoilProfile1D, -1)
                }
            };

            // Call
            converter.ConvertModelToEntity(model, entity);

            // Assert
            Assert.AreEqual(storageId, entity.StochasticSoilModelEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(segmentName, entity.SegmentName);

            Assert.IsEmpty(entity.StochasticSoilProfileEntities);
        }
    }
}
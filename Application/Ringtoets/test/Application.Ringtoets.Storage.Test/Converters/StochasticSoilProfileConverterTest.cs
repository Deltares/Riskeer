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
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Converters
{
    public class StochasticSoilProfileConverterTest
    {
        [Test]
        public void Constructor_Always_NewInstance()
        {
            // Call
            var converter = new StochasticSoilProfileConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<StochasticSoilProfile, StochasticSoilProfileEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilProfileConverter();

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
            var name = "SomeName";
            var probability = Convert.ToDecimal(2.1);
            var entity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = storageId,
                SoilProfileEntity = new SoilProfileEntity
                {
                    Name = name
                },
                Probability = probability
            };
            entity.SoilProfileEntity.SoilLayerEntities.Add(new SoilLayerEntity());
            var converter = new StochasticSoilProfileConverter();

            // Call
            var location = converter.ConvertEntityToModel(entity);

            // Assert
            Assert.AreEqual(storageId, location.StorageId);
            Assert.AreEqual(probability, location.Probability);
            Assert.AreEqual(name, location.SoilProfile.Name);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilProfileConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, new StochasticSoilProfileEntity());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("modelObject", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilProfileConverter();
            var model = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(model, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_ValidModelValidEntity_ReturnsModelAsEntity()
        {
            // Setup
            var converter = new StochasticSoilProfileConverter();
            var random = new Random(21);
            var entity = new StochasticSoilProfileEntity();

            long storageId = random.Next();

            var model = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile(),
                StorageId = storageId
            };

            // Call
            converter.ConvertModelToEntity(model, entity);

            // Assert
            var profile = entity.SoilProfileEntity;
            Assert.AreEqual(storageId, entity.StochasticSoilProfileEntityId);
            Assert.AreEqual(model.SoilProfile.Name, profile.Name);
            Assert.AreEqual(model.SoilProfile.Bottom, profile.Bottom);
            Assert.AreEqual(1, profile.SoilLayerEntities.Count);

            var layer = profile.SoilLayerEntities.ElementAt(0);
            Assert.AreEqual(0, layer.Top);
            Assert.AreEqual(1, layer.IsAquifer);
        }
    }
}
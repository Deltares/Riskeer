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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class HydraulicLocationConverterTest
    {
        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            HydraulicLocationConverter converter = new HydraulicLocationConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null).ToList();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entities", exception.ParamName);
        }

        [Test]
        public void ConvertEntityToModel_ValidEntityValidModel_ReturnsTheEntityAsModel()
        {
            // Setup
            const string name = "test";
            const double designWaterLevel = 15.6;
            const long locationId = 1300001;
            const long storageId = 1234L;
            const decimal locationX = 253;
            const decimal locationY = 123;
            var entity = new HydraulicLocationEntity()
            {
                LocationId = locationId,
                Name = name,
                DesignWaterLevel = designWaterLevel,
                HydraulicLocationEntityId = storageId,
                LocationX = locationX,
                LocationY = locationY
            };
            HydraulicLocationConverter converter = new HydraulicLocationConverter();

            // Call
            List<HydraulicBoundaryLocation> locations = converter.ConvertEntityToModel(new List<HydraulicLocationEntity> {entity}).ToList();

            // Assert
            Assert.AreEqual(1, locations.Count);
            var location = locations[0];
            Assert.AreNotEqual(entity, location);
            Assert.AreEqual(locationId, location.Id);
            Assert.AreEqual(storageId, location.StorageId);
            Assert.AreEqual(name, location.Name);
            Assert.AreEqual(designWaterLevel, location.DesignWaterLevel);
            Assert.AreEqual(locationX, location.Location.X);
            Assert.AreEqual(locationY, location.Location.Y);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            HydraulicLocationConverter converter = new HydraulicLocationConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, new HydraulicLocationEntity());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("modelObject", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            HydraulicLocationConverter converter = new HydraulicLocationConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(new HydraulicBoundaryLocation(1, "test", 1, 1), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_ValidModelValidEntity_ReturnsModelAsEntity()
        {
            // Setup
            HydraulicLocationConverter converter = new HydraulicLocationConverter();
            
            var entity = new HydraulicLocationEntity();
            const long storageId = 1234L;
            const long locationId = 130002;
            const string name = "test";
            const double locationX = 39.3;
            const double locationY = 583.2;
            const double designWaterLever = 14.7;

            var model = new HydraulicBoundaryLocation(locationId, name, locationX, locationY)
            {
                StorageId = storageId,
                DesignWaterLevel = designWaterLever
            };

            // Call
            converter.ConvertModelToEntity(model, entity);

            // Assert
            Assert.AreEqual(model.StorageId, entity.HydraulicLocationEntityId);
            Assert.AreEqual(model.Id, entity.LocationId);
            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.Location.X, entity.LocationX);
            Assert.AreEqual(model.Location.Y, entity.LocationY);
            Assert.AreEqual(model.DesignWaterLevel, entity.DesignWaterLevel);
        }
    }
}

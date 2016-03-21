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
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class HydraulicLocationConverterTest
    {
        private static IEnumerable<Func<HydraulicBoundaryLocation>> TestCases
        {
            get
            {
                yield return () => null;
                yield return null;
            }
        }
        
        [Test]
        public void Constructor_Always_NewInstance()
        {
            // Call
            HydraulicLocationConverter converter = new HydraulicLocationConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<HydraulicBoundaryLocation, HydraulicLocationEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            HydraulicLocationConverter converter = new HydraulicLocationConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null, () => new HydraulicBoundaryLocation());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void ConvertEntityToModel_NullModel_ThrowsArgumentNullException(Func<HydraulicBoundaryLocation> funcModel)
        {
            // Setup
            HydraulicLocationConverter converter = new HydraulicLocationConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(new HydraulicLocationEntity(), funcModel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("model", exception.ParamName);
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
            HydraulicBoundaryLocation location = converter.ConvertEntityToModel(entity, () => new HydraulicBoundaryLocation());

            // Assert
            Assert.AreNotEqual(entity, location);
            Assert.AreEqual(locationId, location.Id);
            Assert.AreEqual(storageId, location.StorageId);
            Assert.AreEqual(name, location.Name);
            Assert.AreEqual(designWaterLevel, location.DesignWaterLevel);
            Assert.AreEqual(locationX, location.Location.X);
            Assert.AreEqual(locationY, location.Location.Y);
        }
    }
}

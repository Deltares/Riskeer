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
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class PipingFailureMechanismConverterTest
    {
        [Test]
        public void DefaultConstructor_Always_NewFailureMechanismEntityConverter()
        {
            // Call
            PipingFailureMechanismConverter converter = new PipingFailureMechanismConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<Piping, FailureMechanismEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            PipingFailureMechanismConverter converter = new PipingFailureMechanismConverter();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ConvertEntityToModel_ValidEntityValidModel_ReturnsEntityAsModel()
        {
            // Setup
            PipingFailureMechanismConverter converter = new PipingFailureMechanismConverter();

            const long storageId = 1234L;
            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism,
            };

            // Call
            Piping failureMechanism = converter.ConvertEntityToModel(entity);

            // Assert
            Assert.AreEqual(entity.FailureMechanismEntityId, failureMechanism.StorageId);
        }

        [Test]
        public void ConvertEntityToModel_EntityWithIncorrectType_ThrowsArgumentException()
        {
            // Setup
            PipingFailureMechanismConverter converter = new PipingFailureMechanismConverter();

            const long storageId = 1234L;
            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.MacrostabilityInwardsFailureMechanism,
            };

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(entity);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            PipingFailureMechanismConverter converter = new PipingFailureMechanismConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, new FailureMechanismEntity());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("modelObject", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            PipingFailureMechanismConverter converter = new PipingFailureMechanismConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(new Piping(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_ValidModelValidEntity_ReturnsModelAsEntity()
        {
            // Setup
            PipingFailureMechanismConverter converter = new PipingFailureMechanismConverter();

            const long storageId = 1234L;
            var entity = new FailureMechanismEntity();

            var model = new Piping
            {
                StorageId = storageId
            };

            // Call
            converter.ConvertModelToEntity(model, entity);

            // Assert
            Assert.AreEqual(model.StorageId, entity.FailureMechanismEntityId);
        }
    }
}
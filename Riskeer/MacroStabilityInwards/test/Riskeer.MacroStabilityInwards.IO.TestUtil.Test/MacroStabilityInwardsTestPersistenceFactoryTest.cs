// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Components.Persistence.Stability;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Shared.Components.Persistence;

namespace Riskeer.MacroStabilityInwards.IO.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsTestPersistenceFactoryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory();

            // Assert
            Assert.IsInstanceOf<IPersistenceFactory>(persistenceFactory);
            Assert.IsNull(persistenceFactory.PersistableDataModel);
            Assert.IsNull(persistenceFactory.FilePath);
            Assert.IsNull(persistenceFactory.CreatedPersister);
        }

        [Test]
        public void CreateArchivePersister_ThrowExceptionIsFalse_SetsPropertiesAndReturnsTestPersister()
        {
            // Setup
            var persistableDataModel = new PersistableDataModel();
            const string filePath = "FilePath";

            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory();

            // Call
            IPersister persister = persistenceFactory.CreateArchivePersister(filePath, persistableDataModel);

            // Assert
            Assert.AreSame(persistableDataModel, persistenceFactory.PersistableDataModel);
            Assert.AreEqual(filePath, persistenceFactory.FilePath);
            Assert.IsInstanceOf<MacroStabilityInwardsTestPersister>(persister);
            Assert.AreSame(persister, persistenceFactory.CreatedPersister);
        }

        [Test]
        public void CreateArchivePersister_ThrowExceptionIsTrue_ThrowsException()
        {
            // Setup
            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory
            {
                ThrowException = true
            };

            // Call
            void Call() => persistenceFactory.CreateArchivePersister("FilePath", new PersistableDataModel());

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreEqual("Exception in persistor.", exception.Message);
        }

        [Test]
        public void CreateArchiveReader_Always_ThrowsNotImplementedException()
        {
            // Setup
            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory();

            // Call
            void Call() => persistenceFactory.CreateArchiveReader(null);

            // Assert
            Assert.Throws<NotImplementedException>(Call);
        }
    }
}
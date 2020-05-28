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
using System.IO;
using Components.Persistence.Stability;
using Components.Persistence.Stability.Data;
using Shared.Components.Persistence;

namespace Riskeer.MacroStabilityInwards.IO.TestUtil
{
    /// <summary>
    /// Persistence factory that can be used in tests.
    /// </summary>
    public class MacroStabilityInwardsTestPersistenceFactory : IPersistenceFactory
    {
        /// <summary>
        /// Gets the <see cref="PersistableDataModel"/>.
        /// </summary>
        public PersistableDataModel PersistableDataModel { get; private set; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the created <see cref="IPersister"/>.
        /// </summary>
        public IPersister CreatedPersister { get; private set; }

        /// <summary>
        /// Gets or sets whether an exception should be thrown.
        /// </summary>
        public bool ThrowException { get; set; }

        /// <summary>
        /// Gets or sets whether a file should be written to the file system.
        /// </summary>
        public bool WriteFile { get; set; }

        public IPersister CreateArchivePersister(string path, PersistableDataModel dataModel)
        {
            if (ThrowException)
            {
                throw new Exception("Exception in persister.");
            }

            FilePath = path;
            PersistableDataModel = dataModel;

            if (WriteFile)
            {
                FileStream stream = File.OpenWrite(FilePath);
                stream.Close();
            }

            return CreatedPersister ?? (CreatedPersister = new MacroStabilityInwardsTestPersister());
        }

        public Reader<PersistableDataModel> CreateArchiveReader(string path)
        {
            throw new NotImplementedException();
        }
    }
}
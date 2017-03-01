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
using Migration.Core.Storage;

namespace Application.Ringtoets.Migration
{
    /// <summary>
    /// Class responsible for migrating a Ringtoets project in the application.
    /// </summary>
    public class RingtoetsMigrator
    {
        private VersionedFileMigrator migrator;

        /// <summary>
        /// Instantiates a <see cref="RingtoetsMigrator"/>.
        /// </summary>
        /// <param name="migrator"></param>
        public RingtoetsMigrator(VersionedFileMigrator migrator)
        {
            if (migrator == null)
            {
                throw new ArgumentNullException(nameof(migrator));
            }
            
            this.migrator = migrator;
        }

        public bool Migrate(string sourcePath, string targetPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public bool ShouldMigrate(string sourcePath)
        {
            throw new NotImplementedException();
        }
    }
}

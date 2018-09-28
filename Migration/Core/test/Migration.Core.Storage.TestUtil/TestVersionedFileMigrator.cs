// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Migration.Scripts.Data;

namespace Migration.Core.Storage.TestUtil
{
    /// <summary>
    /// Test class for migrating a <see cref="IVersionedFile"/>.
    /// </summary>
    public class TestVersionedFileMigrator : VersionedFileMigrator
    {
        private readonly IEnumerable<UpgradeScript> upgradeScripts;
        private readonly IEnumerable<CreateScript> createScripts;

        /// <summary>
        /// Creates a new instance of the <see cref="TestVersionedFileMigrator"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use to compare versions.</param>
        /// <param name="upgradeScripts">The upgrade scripts to use.</param>
        /// <param name="createScripts">The create scripts to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is 
        /// <c>null</c>.</exception>
        public TestVersionedFileMigrator(IComparer comparer, IEnumerable<UpgradeScript> upgradeScripts,
                                         IEnumerable<CreateScript> createScripts) : base(comparer)
        {
            if (upgradeScripts == null)
            {
                throw new ArgumentNullException(nameof(upgradeScripts));
            }

            if (createScripts == null)
            {
                throw new ArgumentNullException(nameof(createScripts));
            }

            this.upgradeScripts = upgradeScripts;
            this.createScripts = createScripts;
        }

        protected override IEnumerable<UpgradeScript> GetAvailableUpgradeScripts()
        {
            return upgradeScripts;
        }

        protected override IEnumerable<CreateScript> GetAvailableCreateScripts()
        {
            return createScripts;
        }
    }
}
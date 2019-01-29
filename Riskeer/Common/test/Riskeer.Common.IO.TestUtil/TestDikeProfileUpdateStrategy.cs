// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.FileImporters;

namespace Riskeer.Common.IO.TestUtil
{
    /// <summary>
    /// Implementation of a <see cref="IDikeProfileUpdateDataStrategy"/>
    /// which can be used for testing.
    /// </summary>
    public class TestDikeProfileUpdateStrategy : IDikeProfileUpdateDataStrategy
    {
        /// <summary>
        /// Gets a value to indicate if <see cref="UpdateDikeProfilesWithImportedData"/>
        /// was called.
        /// </summary>
        public bool Updated { get; private set; }

        /// <summary>
        /// Gets the file path that was passed in <see cref="UpdateDikeProfilesWithImportedData"/>.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the dike profiles which were passed in <see cref="UpdateDikeProfilesWithImportedData"/>.
        /// </summary>
        public DikeProfile[] ReadDikeProfiles { get; private set; }

        /// <summary>
        /// Gets or sets the updated instances that are returned by <see cref="UpdateDikeProfilesWithImportedData"/>.
        /// </summary>
        public IEnumerable<IObservable> UpdatedInstances { get; set; } = Enumerable.Empty<IObservable>();

        public IEnumerable<IObservable> UpdateDikeProfilesWithImportedData(IEnumerable<DikeProfile> importedDataCollection, string sourceFilePath)
        {
            Updated = true;
            FilePath = sourceFilePath;
            ReadDikeProfiles = importedDataCollection.ToArray();
            return UpdatedInstances;
        }
    }
}
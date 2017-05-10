// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.FileImporters;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    public class ForeshoreProfileUpdateDataStrategy : UpdateDataStrategyBase<ForeshoreProfile, IFailureMechanism>,
                                                      IForeshoreProfileUpdateDataStrategy
    {
        public ForeshoreProfileUpdateDataStrategy(IFailureMechanism failureMechanism) : base(failureMechanism, new ForeshoreProfileEqualityComparer()) {}


        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(ForeshoreProfile objectToUpdate, ForeshoreProfile objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);
            return new IObservable[]
            {
                objectToUpdate
            };
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(ForeshoreProfile removedObject)
        {
            // TODO
            return new IObservable[0];
        }

        public IEnumerable<IObservable> UpdateForeshoreProfilesWithImportedData(ForeshoreProfileCollection targetDataCollection, IEnumerable<ForeshoreProfile> importedDataCollection, string sourceFilePath)
        {
            return UpdateTargetCollectionData(targetDataCollection, importedDataCollection, sourceFilePath);
        }

        /// <summary>
        /// Class for comparing he <see cref="ForeshoreProfile"/> only by ID.
        /// </summary>
        private class ForeshoreProfileEqualityComparer : IEqualityComparer<ForeshoreProfile>
        {
            public bool Equals(ForeshoreProfile x, ForeshoreProfile y)
            {
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(ForeshoreProfile obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
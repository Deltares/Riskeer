﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Service;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for 
    /// updating surface lines based on imported data.
    /// </summary>
    public class ForeshoreProfileUpdateDataStrategy : UpdateDataStrategyBase<ForeshoreProfile, IFailureMechanism>,
                                                      IForeshoreProfileUpdateDataStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="ForeshoreProfileUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the <see cref="ForeshoreProfile"/>
        ///  are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public ForeshoreProfileUpdateDataStrategy(IFailureMechanism failureMechanism)
            : base(failureMechanism, new ForeshoreProfileEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateForeshoreProfilesWithImportedData(ForeshoreProfileCollection targetDataCollection,
                                                                                IEnumerable<ForeshoreProfile> importedDataCollection,
                                                                                string sourceFilePath)
        {
            return UpdateTargetCollectionData(targetDataCollection, importedDataCollection, sourceFilePath);
        }

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(ForeshoreProfile objectToUpdate, ForeshoreProfile objectToUpdateFrom)
        {
            var affectedObjects = new List<IObservable>();
            if (!objectToUpdate.Equals(objectToUpdateFrom))
            {
                objectToUpdate.CopyProperties(objectToUpdateFrom);

                IEnumerable<ICalculation<ICalculationInput>> affectedCalculations = GetAffectedCalculationWithSurfaceLines(objectToUpdate);

                foreach (ICalculation<ICalculationInput> calculation in affectedCalculations)
                {
                    affectedObjects.Add(calculation.InputParameters);
                    affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                }
            }

            return affectedObjects;
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(ForeshoreProfile removedObject)
        {
            IEnumerable<ICalculation<ICalculationInput>> affectedCalculations = GetAffectedCalculationWithSurfaceLines(removedObject);

            var affectedObjects = new List<IObservable>();
            foreach (ICalculation<ICalculationInput> calculation in affectedCalculations)
            {
                ((IHasForeshoreProfile) calculation.InputParameters).ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));
            }

            return affectedObjects;
        }

        private IEnumerable<ICalculation<ICalculationInput>> GetAffectedCalculationWithSurfaceLines(ForeshoreProfile foreshoreProfile)
        {
            IEnumerable<ICalculation<ICalculationInput>> calculations = FailureMechanism.Calculations.Cast<ICalculation<ICalculationInput>>();
            IEnumerable<ICalculation<ICalculationInput>> affectedCalculations =
                calculations.Where(calc => ReferenceEquals(
                                       ((IHasForeshoreProfile) calc.InputParameters).ForeshoreProfile, foreshoreProfile));
            return affectedCalculations;
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
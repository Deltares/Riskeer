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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.Service;

namespace Riskeer.Integration.Plugin.FileImporters
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
        /// are updated.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles which will be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public ForeshoreProfileUpdateDataStrategy(IFailureMechanism failureMechanism, ForeshoreProfileCollection foreshoreProfiles)
            : base(failureMechanism, foreshoreProfiles, new ForeshoreProfileEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateForeshoreProfilesWithImportedData(IEnumerable<ForeshoreProfile> importedDataCollection, string sourceFilePath)
        {
            return UpdateTargetCollectionData(importedDataCollection, sourceFilePath);
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(ForeshoreProfile removedObject)
        {
            IEnumerable<ICalculation<ICalculationInput>> affectedCalculations = GetAffectedCalculationWithForeshoreProfiles(removedObject);

            var affectedObjects = new List<IObservable>();
            foreach (ICalculation<ICalculationInput> calculation in affectedCalculations)
            {
                ((IHasForeshoreProfile) calculation.InputParameters).ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
            }

            return affectedObjects;
        }

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(ForeshoreProfile objectToUpdate, ForeshoreProfile objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);

            var affectedObjects = new List<IObservable>();

            IEnumerable<ICalculation<ICalculationInput>> affectedCalculations = GetAffectedCalculationWithForeshoreProfiles(objectToUpdate);

            foreach (ICalculation<ICalculationInput> calculation in affectedCalculations)
            {
                affectedObjects.Add(calculation.InputParameters);
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));

                if (!objectToUpdate.Geometry.Any())
                {
                    ((IUseForeshore) calculation.InputParameters).UseForeshore = false;
                }
            }

            return affectedObjects;
        }

        private IEnumerable<ICalculation<ICalculationInput>> GetAffectedCalculationWithForeshoreProfiles(ForeshoreProfile foreshoreProfile)
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
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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Service;
using Riskeer.GrassCoverErosionInwards.Util;

namespace Riskeer.GrassCoverErosionInwards.Plugin.FileImporters
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for updating 
    /// dike profiles based on imported data.
    /// </summary>
    public class GrassCoverErosionInwardsDikeProfileUpdateDataStrategy
        : UpdateDataStrategyBase<DikeProfile, GrassCoverErosionInwardsFailureMechanism>,
          IDikeProfileUpdateDataStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsDikeProfileUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the dike profiles are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(GrassCoverErosionInwardsFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.DikeProfiles, new DikeProfileIdEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateDikeProfilesWithImportedData(IEnumerable<DikeProfile> importedDataCollection, string sourceFilePath)
        {
            return UpdateTargetCollectionData(importedDataCollection, sourceFilePath);
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(DikeProfile removedObject)
        {
            return GrassCoverErosionInwardsDataSynchronizationService.RemoveDikeProfile(
                removedObject,
                FailureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(),
                FailureMechanism.DikeProfiles,
                FailureMechanism.SectionResults);
        }

        private IEnumerable<GrassCoverErosionInwardsCalculation> GetAffectedCalculationsWithDikeProfile(DikeProfile objectToUpdate)
        {
            IEnumerable<GrassCoverErosionInwardsCalculation> affectedCalculations =
                FailureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>()
                                .Where(calc => ReferenceEquals(objectToUpdate, calc.InputParameters.DikeProfile));
            return affectedCalculations;
        }

        /// <summary>
        /// Class for comparing the <see cref="DikeProfile"/> only by ID.
        /// </summary>
        private class DikeProfileIdEqualityComparer : IEqualityComparer<DikeProfile>
        {
            public bool Equals(DikeProfile x, DikeProfile y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(DikeProfile obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        #region Update logic

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(DikeProfile objectToUpdate, DikeProfile objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);

            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(UpdateDikeDependentData(objectToUpdate));

            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateDikeDependentData(DikeProfile objectToUpdate)
        {
            var affectedObjects = new List<IObservable>();
            foreach (GrassCoverErosionInwardsCalculation calculation in GetAffectedCalculationsWithDikeProfile(objectToUpdate))
            {
                affectedObjects.Add(calculation.InputParameters);
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));

                if (!objectToUpdate.ForeshoreGeometry.Any())
                {
                    calculation.InputParameters.UseForeshore = false;
                }
            }

            affectedObjects.AddRange(GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                                         FailureMechanism.SectionResults,
                                         FailureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>()));
            return affectedObjects;
        }

        #endregion
    }
}
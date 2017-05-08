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
using System.Linq;
using Core.Common.Base;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// A <see cref="ReplaceDataStrategyBase{TTargetData,TFailureMechanism}"/>
    /// to replace foreshore profiles with imported dike profiles.
    /// </summary>
    public class ForeshoreProfileReplaceDataStrategy : ReplaceDataStrategyBase<ForeshoreProfile, IFailureMechanism>,
                                                       IForeshoreProfileUpdateDataStrategy
    {
        /// <summary>
        /// Initializes a <see cref="ForeshoreProfileReplaceDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the foreshore 
        /// profiles are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="failureMechanism"/>
        /// is not supported.</exception>
        public ForeshoreProfileReplaceDataStrategy(IFailureMechanism failureMechanism) : base(failureMechanism)
        {
            if (!(failureMechanism is WaveImpactAsphaltCoverFailureMechanism)
                && !(failureMechanism is StabilityStoneCoverFailureMechanism)
                && !(failureMechanism is GrassCoverErosionOutwardsFailureMechanism)
                && !(failureMechanism is HeightStructuresFailureMechanism)
                && !(failureMechanism is StabilityPointStructuresFailureMechanism)
                && !(failureMechanism is ClosingStructuresFailureMechanism))
            {
                throw new NotSupportedException($"Can't apply this strategy for {failureMechanism.GetType()}.");
            }
        }

        public IEnumerable<IObservable> UpdateForeshoreProfilesWithImportedData(ForeshoreProfileCollection targetDataCollection,
                                                                                IEnumerable<ForeshoreProfile> importedDataCollection,
                                                                                string sourceFilePath)
        {
            try
            {
                return ReplaceTargetCollectionWithImportedData(targetDataCollection, importedDataCollection, sourceFilePath);
            }
            catch (UpdateDataException e)
            {
                string message = string.Format(
                    Resources.ForeshoreProfileReplaceDataStrategy_UpdateForeshoreProfilesWithImportedData_Importing_ForeshoreProfiles_failed_Reason_0,
                    e.Message);

                throw new ForeshoreProfileUpdateException(message, e);
            }
        }

        protected override IEnumerable<IObservable> ClearData(IFailureMechanism failureMechanism)
        {
            // TODO: Clearing the output will be implemented as part of WTI-1116
            var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
            if (waveImpactAsphaltCoverFailureMechanism != null)
            {
                return ClearForeshoreProfilesDependentData(waveImpactAsphaltCoverFailureMechanism);
            }

            var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
            if (stabilityStoneCoverFailureMechanism != null)
            {
                return ClearForeshoreProfilesDependentData(stabilityStoneCoverFailureMechanism);
            }

            var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
            if (grassCoverErosionOutwardsFailureMechanism != null)
            {
                return ClearForeshoreProfilesDependentData(grassCoverErosionOutwardsFailureMechanism);
            }

            var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
            if (heightStructuresFailureMechanism != null)
            {
                return ClearForeshoreProfilesDependentData(heightStructuresFailureMechanism);
            }

            var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
            if (stabilityPointStructuresFailureMechanism != null)
            {
                return ClearForeshoreProfilesDependentData(stabilityPointStructuresFailureMechanism);
            }

            var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
            if (closingStructuresFailureMechanism != null)
            {
                return ClearForeshoreProfileDependentData(closingStructuresFailureMechanism);
            }

            return Enumerable.Empty<IObservable>();
        }

        private static IEnumerable<IObservable> ClearForeshoreProfileDependentData(ClosingStructuresFailureMechanism closingStructuresFailureMechanism)
        {
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> affectedCalculations =
                closingStructuresFailureMechanism.Calculations
                                                 .OfType<StructuresCalculation<ClosingStructuresInput>>()
                                                 .Where(calc => calc.InputParameters.ForeshoreProfile != null);

            var affectedObjects = new List<IObservable>();
            foreach (var calculation in affectedCalculations)
            {
                calculation.InputParameters.ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
            }

            closingStructuresFailureMechanism.ForeshoreProfiles.Clear();
            affectedObjects.Add(closingStructuresFailureMechanism.ForeshoreProfiles);
            return affectedObjects;
        }

        private static IEnumerable<IObservable> ClearForeshoreProfilesDependentData(StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism)
        {
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> affectedCalculations =
                stabilityPointStructuresFailureMechanism.Calculations
                                                        .OfType<StructuresCalculation<StabilityPointStructuresInput>>()
                                                        .Where(calc => calc.InputParameters.ForeshoreProfile != null);

            var affectedObjects = new List<IObservable>();
            foreach (var calculation in affectedCalculations)
            {
                calculation.InputParameters.ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
            }

            stabilityPointStructuresFailureMechanism.ForeshoreProfiles.Clear();
            affectedObjects.Add(stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            return affectedObjects;
        }

        private static IEnumerable<IObservable> ClearForeshoreProfilesDependentData(HeightStructuresFailureMechanism heightStructuresFailureMechanism)
        {
            IEnumerable<StructuresCalculation<HeightStructuresInput>> affectedCalculations =
                heightStructuresFailureMechanism.Calculations
                                                .OfType<StructuresCalculation<HeightStructuresInput>>()
                                                .Where(calc => calc.InputParameters.ForeshoreProfile != null);

            var affectedObjects = new List<IObservable>();
            foreach (var calculation in affectedCalculations)
            {
                calculation.InputParameters.ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
            }

            heightStructuresFailureMechanism.ForeshoreProfiles.Clear();
            affectedObjects.Add(heightStructuresFailureMechanism.ForeshoreProfiles);
            return affectedObjects;
        }

        private static IEnumerable<IObservable> ClearForeshoreProfilesDependentData(GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism)
        {
            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> affectedCalculations =
                grassCoverErosionOutwardsFailureMechanism.Calculations
                                                         .OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                         .Where(calc => calc.InputParameters.ForeshoreProfile != null);

            var affectedObjects = new List<IObservable>();
            foreach (var calculation in affectedCalculations)
            {
                calculation.InputParameters.ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
            }

            grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles.Clear();
            affectedObjects.Add(grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);
            return affectedObjects;
        }

        private static IEnumerable<IObservable> ClearForeshoreProfilesDependentData(StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism)
        {
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedCalculations =
                stabilityStoneCoverFailureMechanism.Calculations
                                                   .OfType<StabilityStoneCoverWaveConditionsCalculation>()
                                                   .Where(calc => calc.InputParameters.ForeshoreProfile != null);

            var affectedObjects = new List<IObservable>();
            foreach (var calculation in affectedCalculations)
            {
                calculation.InputParameters.ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
            }

            stabilityStoneCoverFailureMechanism.ForeshoreProfiles.Clear();
            affectedObjects.Add(stabilityStoneCoverFailureMechanism.ForeshoreProfiles);
            return affectedObjects;
        }

        private static IEnumerable<IObservable> ClearForeshoreProfilesDependentData(WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism)
        {
            IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> affectedCalculations =
               waveImpactAsphaltCoverFailureMechanism.Calculations
                                                  .OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                  .Where(calc => calc.InputParameters.ForeshoreProfile != null);

            var affectedObjects = new List<IObservable>();
            foreach (var calculation in affectedCalculations)
            {
                calculation.InputParameters.ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
            }

            waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles.Clear();
            affectedObjects.Add(waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);
            return affectedObjects;
        }
    }
}
﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Integration.Service;

namespace Riskeer.Integration.Plugin.FileImporters
{
    /// <summary>
    /// A <see cref="ReplaceDataStrategyBase{TTargetData,TFailureMechanism}"/>
    /// to replace foreshore profiles with imported foreshore profiles.
    /// </summary>
    public class ForeshoreProfileReplaceDataStrategy : ReplaceDataStrategyBase<ForeshoreProfile, ICalculatableFailureMechanism>,
                                                       IForeshoreProfileUpdateDataStrategy
    {
        private readonly ForeshoreProfileCollection foreshoreProfileCollection;

        /// <summary>
        /// Initializes a <see cref="ForeshoreProfileReplaceDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the foreshore 
        /// profiles are updated.</param>
        /// <param name="foreshoreProfiles">The collection containing the foreshore profiles.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public ForeshoreProfileReplaceDataStrategy(ICalculatableFailureMechanism failureMechanism, ForeshoreProfileCollection foreshoreProfiles)
            : base(failureMechanism, foreshoreProfiles)
        {
            foreshoreProfileCollection = foreshoreProfiles;
        }

        public IEnumerable<IObservable> UpdateForeshoreProfilesWithImportedData(IEnumerable<ForeshoreProfile> importedDataCollection, string sourceFilePath)
        {
            return ReplaceTargetCollectionWithImportedData(importedDataCollection, sourceFilePath);
        }

        protected override IEnumerable<IObservable> ClearData()
        {
            IEnumerable<ICalculation<ICalculationInput>> calculations = FailureMechanism.Calculations
                                                                                        .Cast<ICalculation<ICalculationInput>>();
            return RiskeerDataSynchronizationService.RemoveAllForeshoreProfiles(calculations, foreshoreProfileCollection);
        }
    }
}
﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Service;

namespace Riskeer.DuneErosion.Forms
{
    /// <summary>
    /// Class that handles changes to the data model at failure mechanism level of
    /// <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public class DuneErosionFailureMechanismPropertyChangeHandler : FailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>
    {
        protected override bool RequiresConfirmation(DuneErosionFailureMechanism failureMechanism)
        {
            return base.RequiresConfirmation(failureMechanism)
                   || failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Any(HasOutput)
                   || failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Any(HasOutput)
                   || failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Any(HasOutput)
                   || failureMechanism.CalculationsForLowerLimitNorm.Any(HasOutput)
                   || failureMechanism.CalculationsForFactorizedLowerLimitNorm.Any(HasOutput);
        }

        protected override IEnumerable<IObservable> PropertyChanged(DuneErosionFailureMechanism failureMechanism)
        {
            return DuneErosionDataSynchronizationService.ClearDuneLocationCalculationsOutput(failureMechanism);
        }

        private static bool HasOutput(DuneLocationCalculation calculation)
        {
            return calculation.Output != null;
        }
    }
}
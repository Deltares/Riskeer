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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Service;

namespace Ringtoets.GrassCoverErosionOutwards.Forms
{
    /// <summary>
    /// Class which properly handles data model changes due to a change of a
    /// grass cover erosion outwards failure mechanism property.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler : FailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>
    {
        protected override bool RequiresConfirmation(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return base.RequiresConfirmation(failureMechanism)
                   || failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Any(c => c.HasOutput)
                   || failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Any(c => c.HasOutput)
                   || failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Any(c => c.HasOutput)
                   || failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Any(c => c.HasOutput)
                   || failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Any(c => c.HasOutput)
                   || failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Any(c => c.HasOutput);
        }

        protected override IEnumerable<IObservable> PropertyChanged(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            var affectedObjects = new List<IObservable>(base.PropertyChanged(failureMechanism));
            affectedObjects.AddRange(GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(failureMechanism));

            return affectedObjects;
        }
    }
}
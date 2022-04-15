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

using System;
using Core.Common.Base.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.StabilityStoneCover
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralStabilityStoneCoverWaveConditionsInput"/>
    /// based on the <see cref="StabilityStoneCoverFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class StabilityStoneCoverFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="StabilityStoneCoverFailureMechanismMetaEntity"/> and use the information to
        /// update the <see cref="GeneralStabilityStoneCoverWaveConditionsInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityStoneCoverFailureMechanismMetaEntity"/> to update
        /// <see cref="GeneralStabilityStoneCoverWaveConditionsInput"/> for.</param>
        /// <param name="generalInput">The <see cref="GeneralStabilityStoneCoverWaveConditionsInput"/> to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this StabilityStoneCoverFailureMechanismMetaEntity entity, GeneralStabilityStoneCoverWaveConditionsInput generalInput)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            generalInput.N = (RoundedDouble) entity.N;
            generalInput.ApplyLengthEffectInSection = Convert.ToBoolean(entity.ApplyLengthEffectInSection);
        }
    }
}
// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.DuneErosion.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.DuneErosion
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralDuneErosionInput"/>
    /// based on the <see cref="DuneErosionFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class DuneErosionFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="DuneErosionFailureMechanismMetaEntity"/> and use the information to update
        /// the <paramref name="input"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DuneErosionFailureMechanismMetaEntity"/> to use to update
        /// the <paramref name="input"/>.</param>
        /// <param name="input">The <see cref="GeneralDuneErosionInput"/> to be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void Read(this DuneErosionFailureMechanismMetaEntity entity, GeneralDuneErosionInput input)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            input.N = (RoundedDouble) entity.N;
        }
    }
}
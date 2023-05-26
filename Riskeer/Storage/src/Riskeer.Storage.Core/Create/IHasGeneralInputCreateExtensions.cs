// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="IHasGeneralInput"/> related
    /// to creating an implementation of <see cref="IStandAloneFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class IHasGeneralInputCreateExtensions
    {
        /// <summary>
        /// Creates an implementation of <see cref="IStandAloneFailureMechanismMetaEntity"/> based on the information
        /// of the <see cref="IHasGeneralInput"/>.
        /// </summary>
        /// <param name="mechanism">The implementation of <see cref="IHasGeneralInput"/>.</param>
        /// <typeparam name="TFailureMechanismMetaEntity">The implementation of <see cref="IStandAloneFailureMechanismMetaEntity"/>.</typeparam>
        /// <returns>A new <see cref="TFailureMechanismMetaEntity"/>.</returns>
        internal static TFailureMechanismMetaEntity Create<TFailureMechanismMetaEntity>(this IHasGeneralInput mechanism)
            where TFailureMechanismMetaEntity : IStandAloneFailureMechanismMetaEntity, new()
        {
            return new TFailureMechanismMetaEntity
            {
                N = mechanism.GeneralInput.N,
                ApplyLengthEffectInSection = Convert.ToByte(mechanism.GeneralInput.ApplyLengthEffectInSection)
            };
        }
    }
}
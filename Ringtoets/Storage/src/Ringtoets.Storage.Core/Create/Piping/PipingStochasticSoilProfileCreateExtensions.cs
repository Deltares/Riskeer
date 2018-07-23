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
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingStochasticSoilProfile"/> related to creating
    /// a <see cref="PipingStochasticSoilProfileEntity"/>.
    /// </summary>
    internal static class PipingStochasticSoilProfileCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingStochasticSoilProfileEntity"/> based on the information of the 
        /// <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="profile">The profile to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="PipingStochasticSoilProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static PipingStochasticSoilProfileEntity Create(this PipingStochasticSoilProfile profile,
                                                                 PersistenceRegistry registry,
                                                                 int order)
        {
            var entity = new PipingStochasticSoilProfileEntity
            {
                Probability = profile.Probability,
                PipingSoilProfileEntity = profile.SoilProfile.Create(registry),
                Order = order
            };
            if (registry.Contains(profile))
            {
                return registry.Get(profile);
            }

            registry.Register(entity, profile);
            return entity;
        }
    }
}
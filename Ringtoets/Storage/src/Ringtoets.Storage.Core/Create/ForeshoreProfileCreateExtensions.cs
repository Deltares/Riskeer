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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="ForeshoreProfile"/> related to creating a <see cref="ForeshoreProfileEntity"/>.
    /// </summary>
    public static class ForeshoreProfileCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ForeshoreProfileEntity"/> based on the information of the <see cref="ForeshoreProfile"/>.
        /// </summary>
        /// <param name="foreshoreProfile">The foreshore profile to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="foreshoreProfile"/> resides within its parent.</param>
        /// <returns>A new <see cref="ForeshoreProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static ForeshoreProfileEntity Create(this ForeshoreProfile foreshoreProfile, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(foreshoreProfile))
            {
                return registry.Get(foreshoreProfile);
            }

            var foreshoreProfileEntity = new ForeshoreProfileEntity
            {
                Id = foreshoreProfile.Id.DeepClone(),
                Name = foreshoreProfile.Name.DeepClone(),
                GeometryXml = new Point2DCollectionXmlSerializer().ToXml(foreshoreProfile.Geometry),
                X = foreshoreProfile.WorldReferencePoint.X,
                Y = foreshoreProfile.WorldReferencePoint.Y,
                X0 = foreshoreProfile.X0,
                Orientation = foreshoreProfile.Orientation,
                Order = order
            };

            if (foreshoreProfile.HasBreakWater)
            {
                foreshoreProfileEntity.BreakWaterHeight = foreshoreProfile.BreakWater.Height;
                foreshoreProfileEntity.BreakWaterType = Convert.ToByte(foreshoreProfile.BreakWater.Type);
            }

            registry.Register(foreshoreProfileEntity, foreshoreProfile);

            return foreshoreProfileEntity;
        }
    }
}
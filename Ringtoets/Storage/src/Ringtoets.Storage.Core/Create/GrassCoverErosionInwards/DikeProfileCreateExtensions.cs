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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="DikeProfile"/> related to creating a <see cref="DikeProfileEntity"/>.
    /// </summary>
    internal static class DikeProfileCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="DikeProfileEntity"/> based on the information of the <see cref="DikeProfile"/>.
        /// </summary>
        /// <param name="dikeProfile">The dike profile to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="DikeProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static DikeProfileEntity Create(this DikeProfile dikeProfile, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(dikeProfile))
            {
                return registry.Get(dikeProfile);
            }

            var sectionResultEntity = new DikeProfileEntity
            {
                X = dikeProfile.WorldReferencePoint.X,
                Y = dikeProfile.WorldReferencePoint.Y,
                X0 = dikeProfile.X0,
                DikeGeometryXml = new RoughnessPointCollectionXmlSerializer().ToXml(dikeProfile.DikeGeometry),
                ForeshoreXml = new Point2DXmlSerializer().ToXml(dikeProfile.ForeshoreGeometry),
                Orientation = dikeProfile.Orientation,
                DikeHeight = dikeProfile.DikeHeight,
                Id = dikeProfile.Id.DeepClone(),
                Name = dikeProfile.Name.DeepClone(),
                Order = order
            };
            if (dikeProfile.HasBreakWater)
            {
                sectionResultEntity.BreakWaterHeight = dikeProfile.BreakWater.Height;
                sectionResultEntity.BreakWaterType = Convert.ToByte(dikeProfile.BreakWater.Type);
            }

            registry.Register(sectionResultEntity, dikeProfile);
            return sectionResultEntity;
        }
    }
}
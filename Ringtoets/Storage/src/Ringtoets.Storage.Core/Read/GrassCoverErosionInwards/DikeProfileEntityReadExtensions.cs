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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Read.GrassCoverErosionInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="DikeProfile"/>
    /// based on the <see cref="DikeProfileEntity"/>.
    /// </summary>
    internal static class DikeProfileEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="DikeProfileEntity"/> and use the information to update a 
        /// <see cref="DikeProfile"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DikeProfileEntity"/> to create <see cref="DikeProfile"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="DikeProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="DikeProfileEntity.DikeGeometryXml"/> 
        /// or <see cref="DikeProfileEntity.ForeshoreXml"/> of <paramref name="entity"/> is <c>null</c> or empty.</exception>
        internal static DikeProfile Read(this DikeProfileEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var dikeProfile = new DikeProfile(new Point2D(entity.X.ToNullAsNaN(), entity.Y.ToNullAsNaN()),
                                              new RoughnessPointXmlSerializer().FromXml(entity.DikeGeometryXml),
                                              new Point2DXmlSerializer().FromXml(entity.ForeshoreXml),
                                              CreateBreakWater(entity),
                                              CreateProperties(entity));

            collector.Read(entity, dikeProfile);

            return dikeProfile;
        }

        private static DikeProfile.ConstructionProperties CreateProperties(DikeProfileEntity entity)
        {
            return new DikeProfile.ConstructionProperties
            {
                Id = entity.Id,
                Name = entity.Name,
                Orientation = entity.Orientation.ToNullAsNaN(),
                DikeHeight = entity.DikeHeight.ToNullAsNaN(),
                X0 = entity.X0.ToNullAsNaN()
            };
        }

        private static BreakWater CreateBreakWater(DikeProfileEntity entity)
        {
            if (entity.BreakWaterType == null || entity.BreakWaterHeight == null)
            {
                return null;
            }

            return new BreakWater((BreakWaterType) entity.BreakWaterType, entity.BreakWaterHeight.Value);
        }
    }
}
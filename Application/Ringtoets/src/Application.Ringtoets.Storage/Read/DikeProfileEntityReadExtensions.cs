// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Geometry;

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Read
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
        /// <returns>A new <see cref="DikeProfile"/>.</returns>
        internal static DikeProfile Read(this DikeProfileEntity entity)
        {
            return new DikeProfile(new Point2D(entity.X, entity.Y),
                                   new RoughnessPointBinaryConverter().ToData(entity.DikeGeometryData),
                                   new Point2DBinaryConverter().ToData(entity.ForeShoreData),
                                   CreateBreakWater(entity),
                                   CreateProperties(entity))
            {
                StorageId = entity.DikeProfileEntityId
            };
        }

        private static DikeProfile.ConstructionProperties CreateProperties(DikeProfileEntity entity)
        {
            return new DikeProfile.ConstructionProperties
            {
                Name = entity.Name,
                Orientation = entity.Orientation,
                DikeHeight = entity.DikeHeight,
                X0 = entity.X0
            };
        }

        private static BreakWater CreateBreakWater(DikeProfileEntity entity)
        {
            if (entity.BreakWaterType == null)
            {
                return null;
            }
            return new BreakWater((BreakWaterType)entity.BreakWaterType, entity.BreakWaterHeight.Value);
        }
    }
}
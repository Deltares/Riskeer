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
using Core.Common.Base.Geometry;
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.DikeProfiles;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="ForeshoreProfile"/> based on the
    /// <see cref="ForeshoreProfileEntity"/>.
    /// </summary>
    internal static class ForeshoreProfileEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="ForeshoreProfileEntity"/> and use the information to construct a <see cref="ForeshoreProfile"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ForeshoreProfileEntity"/> to create <see cref="ForeshoreProfile"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="ForeshoreProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="ForeshoreProfileEntity.GeometryXml"/> 
        /// of <paramref name="entity"/> is <c>null</c> or empty.</exception>
        internal static ForeshoreProfile Read(this ForeshoreProfileEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            Point2D[] points = new Point2DCollectionXmlSerializer().FromXml(entity.GeometryXml);

            var foreshoreProfile = new ForeshoreProfile(new Point2D(entity.X.ToNullAsNaN(), entity.Y.ToNullAsNaN()),
                                                        points,
                                                        CreateBreakWater(entity.BreakWaterType, entity.BreakWaterHeight),
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = entity.Id.DeepClone(),
                                                            Name = entity.Name.DeepClone(),
                                                            Orientation = entity.Orientation.ToNullAsNaN(),
                                                            X0 = entity.X0.ToNullAsNaN()
                                                        });

            collector.Read(entity, foreshoreProfile);

            return foreshoreProfile;
        }

        private static BreakWater CreateBreakWater(byte? breakWaterType, double? breakWaterHeight)
        {
            if (breakWaterType == null)
            {
                return null;
            }

            return new BreakWater((BreakWaterType) breakWaterType.Value, breakWaterHeight.ToNullAsNaN());
        }
    }
}
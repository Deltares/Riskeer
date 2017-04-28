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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="StochasticSoilModel"/> based on the
    /// <see cref="StochasticSoilModelEntity"/>.
    /// </summary>
    internal static class StochasticSoilModelEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="StochasticSoilModelEntity"/> and use the information to construct a <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to create <see cref="StochasticSoilModel"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="StochasticSoilModelEntity.StochasticSoilModelSegmentPointXml"/> 
        /// of <paramref name="entity"/> is <c>null</c> or empty.</exception>
        internal static StochasticSoilModel Read(this StochasticSoilModelEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }
            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var model = new StochasticSoilModel(-1, entity.Name, entity.SegmentName);
            entity.ReadStochasticSoilProfiles(model, collector);
            entity.ReadSegmentPoints(model);

            collector.Read(entity, model);

            return model;
        }

        private static void ReadStochasticSoilProfiles(this StochasticSoilModelEntity entity, StochasticSoilModel model, ReadConversionCollector collector)
        {
            foreach (StochasticSoilProfileEntity stochasticSoilProfileEntity in entity.StochasticSoilProfileEntities.OrderBy(ssp => ssp.Order))
            {
                model.StochasticSoilProfiles.Add(stochasticSoilProfileEntity.Read(collector));
            }
        }

        private static void ReadSegmentPoints(this StochasticSoilModelEntity entity, StochasticSoilModel model)
        {
            model.Geometry.AddRange(new Point2DXmlSerializer().FromXml(entity.StochasticSoilModelSegmentPointXml));
        }
    }
}
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.MacroStabilityInwards;
using Ringtoets.Storage.Core.Read.Piping;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a stochastic soil model
    /// based on the <see cref="StochasticSoilModelEntity"/>.
    /// </summary>
    internal static class StochasticSoilModelEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="StochasticSoilModelEntity"/> and use the information to construct 
        /// a <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to create <see cref="PipingStochasticSoilModel"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingStochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="StochasticSoilModelEntity.StochasticSoilModelSegmentPointXml"/> 
        /// of <paramref name="entity"/> is empty.</exception>
        public static PipingStochasticSoilModel ReadAsPipingStochasticSoilModel(this StochasticSoilModelEntity entity,
                                                                                ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.ContainsPipingStochasticSoilModel(entity))
            {
                return collector.GetPipingStochasticSoilModel(entity);
            }

            Point2D[] geometry = ReadSegmentPoints(entity.StochasticSoilModelSegmentPointXml).ToArray();
            PipingStochasticSoilProfile[] stochasticSoilProfiles = ReadPipingStochasticSoilProfiles(entity, collector).ToArray();
            var model = new PipingStochasticSoilModel(entity.Name, geometry, stochasticSoilProfiles);

            collector.Read(entity, model);

            return model;
        }

        /// <summary>
        /// Reads the <see cref="StochasticSoilModelEntity"/> and use the information to construct 
        /// a <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to create <see cref="MacroStabilityInwardsStochasticSoilModel"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsStochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="StochasticSoilModelEntity.StochasticSoilModelSegmentPointXml"/> 
        /// of <paramref name="entity"/> is empty.</exception>
        public static MacroStabilityInwardsStochasticSoilModel ReadAsMacroStabilityInwardsStochasticSoilModel(this StochasticSoilModelEntity entity,
                                                                                                              ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.ContainsMacroStabilityInwardsStochasticSoilModel(entity))
            {
                return collector.GetMacroStabilityInwardsStochasticSoilModel(entity);
            }

            Point2D[] geometry = ReadSegmentPoints(entity.StochasticSoilModelSegmentPointXml).ToArray();
            MacroStabilityInwardsStochasticSoilProfile[] stochasticSoilProfiles = entity.ReadMacroStabilityInwardsStochasticSoilProfiles(collector)
                                                                                        .ToArray();
            var model = new MacroStabilityInwardsStochasticSoilModel(entity.Name, geometry, stochasticSoilProfiles);

            collector.Read(entity, model);

            return model;
        }

        private static IEnumerable<PipingStochasticSoilProfile> ReadPipingStochasticSoilProfiles(this StochasticSoilModelEntity entity,
                                                                                                 ReadConversionCollector collector)
        {
            foreach (PipingStochasticSoilProfileEntity stochasticSoilProfileEntity in entity.PipingStochasticSoilProfileEntities
                                                                                            .OrderBy(ssp => ssp.Order))
            {
                yield return stochasticSoilProfileEntity.Read(collector);
            }
        }

        private static IEnumerable<MacroStabilityInwardsStochasticSoilProfile> ReadMacroStabilityInwardsStochasticSoilProfiles(this StochasticSoilModelEntity entity,
                                                                                                                               ReadConversionCollector collector)
        {
            foreach (MacroStabilityInwardsStochasticSoilProfileEntity stochasticSoilProfileEntity in entity.MacroStabilityInwardsStochasticSoilProfileEntities
                                                                                                           .OrderBy(ssp => ssp.Order))
            {
                yield return stochasticSoilProfileEntity.Read(collector);
            }
        }

        /// <summary>
        /// Reads the segment points.
        /// </summary>
        /// <param name="xml">The xml containing a collection of <see cref="Point2D"/>.</param>
        /// <returns>The read segment points.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xml"/> is empty.</exception>
        private static IEnumerable<Point2D> ReadSegmentPoints(string xml)
        {
            return new Point2DCollectionXmlSerializer().FromXml(xml);
        }
    }
}
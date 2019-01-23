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
using Core.Common.Base.Geometry;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.TestUtil.MacroStabilityInwards
{
    /// <summary>
    /// Factory for creating a <see cref="MacroStabilityInwardsSoilLayerTwoDEntity"/> that can 
    /// be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsSoilLayerTwoDEntityTestFactory
    {
        /// <summary>
        /// Creates a valid <see cref="MacroStabilityInwardsSoilLayerTwoDEntity"/>.
        /// </summary>
        /// <returns>The created <see cref="MacroStabilityInwardsSoilLayerTwoDEntity"/>.</returns>
        public static MacroStabilityInwardsSoilLayerTwoDEntity CreateMacroStabilityInwardsSoilLayerTwoDEntity()
        {
            var random = new Random(31);
            var points = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            return new MacroStabilityInwardsSoilLayerTwoDEntity
            {
                OuterRingXml = new Point2DCollectionXmlSerializer().ToXml(points)
            };
        }
    }
}
// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Base.IO;
using Core.Components.Gis.Data;
using DotSpatial.Data;
using log4net;
using ILog = log4net.ILog;

namespace Core.Components.Gis.IO.Importers
{
    /// <summary>
    /// Imports a <see cref="FeatureBasedMapData"/> from a <see cref="Shapefile"/> and stores
    /// it on a <see cref="MapDataCollection"/>.
    /// </summary>
    public class FeatureBasedMapDataImporter : FileImporterBase<MapDataCollection>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FeatureBasedMapDataImporter));

        /// <summary>
        /// Creates a new instance of <see cref="FeatureBasedMapDataImporter"/>.
        /// </summary>
        /// <param name="importTarget">The <see cref="MapDataCollection"/> to add the imported data to.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        public FeatureBasedMapDataImporter(MapDataCollection importTarget, string filePath)
            : base(filePath, importTarget) {}

        public override bool Import()
        {
            return true;
        }
    }
}
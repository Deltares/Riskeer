// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using Core.Common.Util;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Importers;
using Core.Gui.PresentationObjects.Map;
using Core.Gui.Properties;

namespace Core.Gui.Plugin.Map
{
    /// <summary>
    /// Factory for creating <see cref="ImportInfo"/> objects for <see cref="MapData"/>.
    /// </summary>
    public static class MapImportInfoFactory
    {
        /// <summary>
        /// Creates the <see cref="ImportInfo"/> objects.
        /// </summary>
        /// <returns>The created <see cref="ImportInfo"/> objects.</returns>
        public static IEnumerable<ImportInfo> Create()
        {
            yield return new ImportInfo<MapDataCollectionContext>
            {
                Name = Resources.Name_Layer,
                Category = Resources.Categories_Layer,
                Image = Resources.MapPlusIcon,
                FileFilterGenerator = new FileFilterGenerator(
                    Resources.MapImportInfoFactory_Create_MapDataCollection_filefilter_Extension,
                    Resources.MapImportInfoFactory_Create_MapDataCollection_filefilter_Description),
                IsEnabled = context => true,
                CreateFileImporter = (context, filePath) => new FeatureBasedMapDataImporter((MapDataCollection) context.WrappedData, filePath)
            };
        }
    }
}
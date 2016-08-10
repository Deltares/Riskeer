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

using System;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Components.Gis.Data;
using DotSpatial.Data;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Core.Components.Gis.IO.Writers
{
    /// <summary>
    /// The base class to write data to a shapefile.
    /// </summary>
    public abstract class ShapeFileWriterBase<T> : IDisposable where T : FeatureBasedMapData
    {
        protected Shapefile ShapeFile;

        /// <summary>
        /// Adds a feature to the in-memory shapefile.
        /// </summary>
        /// <param name="mapData">The <typeparamref name="T"/> to add to the in-memory shapefile as a feature.</param>
        /// <exception cref="ArgumentException">Thrown when a <paramref name="mapData"/> contains different metadata keys
        /// than the <paramref name="mapData"/> of the first call to <see cref="AddFeature"/>.</exception>
        public virtual void AddFeature(T mapData) {}

        /// <summary>
        /// Saves the in-memory shapefile to a file, overwriting when necessary.
        /// </summary>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public void SaveAs(string filePath)
        {
            FileUtils.ValidateFilePath(filePath);

            try
            {
                ShapeFile.SaveAs(filePath, true);
            }
            catch (Exception e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilsResources.Error_Writing_To_File_0_1, filePath, e.Message));
            }
        }

        public void Dispose()
        {
            if (ShapeFile != null)
            {
                ShapeFile.Close();
            }
        }
    }
}
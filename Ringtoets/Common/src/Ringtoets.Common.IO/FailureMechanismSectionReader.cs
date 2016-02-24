﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO;

using Ringtoets.Common.Data;

using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Reads <see cref="FailureMechanismSection"/> instances from a shapefile containing
    /// one or multiple line features.
    /// </summary>
    public class FailureMechanismSectionReader : IDisposable
    {
        private const string SectionNameAttributeKey = "Vaknaam";
        private readonly PolylineShapeFileReader polylineShapeFileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismSectionReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <exception cref="ArgumentException"><paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException"><paramref name="shapeFilePath"/> points to a file that does not exist.</exception>
        public FailureMechanismSectionReader(string shapeFilePath)
        {
            FileUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            polylineShapeFileReader = OpenPolyLineShapeFile(shapeFilePath);
        }

        /// <summary>
        /// Gets the number of failure mechanism sections in the shapefile.
        /// </summary>
        /// <exception cref="CriticalFileReadException">When the shapefile does not have
        /// a required attribute defined.</exception>
        public int GetFailureMechanismSectionCount()
        {
            ValidateExistenceOfRequiredAttributes();
            return polylineShapeFileReader.GetNumberOfLines();
        }

        /// <summary>
        /// Reads and consumes an entry in the shapefile, using the data to create a new
        /// instance of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The <see cref="FailureMechanismSection"/> read from the file, or <c>null</c>
        /// when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">When either:
        /// <list type="bullet">
        /// <item>the shapefile does not have a required attribute defined.</item>
        /// <item>the element read from the file is a multi-polyline.</item>
        /// </list></exception>
        public FailureMechanismSection ReadFailureMechanismSection()
        {
            ValidateExistenceOfRequiredAttributes();

            var lineData = ReadMapLineData();
            if (lineData == null)
            {
                return null;
            }

            return CreateFailureMechanismSection(lineData);
        }

        public void Dispose()
        {
            polylineShapeFileReader.Dispose();
        }

        /// <summary>
        /// Validates the existence of required attributes.
        /// </summary>
        /// <exception cref="CriticalFileReadException">When the shapefile does not have
        /// a required attribute defined.</exception>
        private void ValidateExistenceOfRequiredAttributes()
        {
            if (!polylineShapeFileReader.HasAttribute(SectionNameAttributeKey))
            {
                var message = string.Format(RingtoetsCommonIOResources.FailureMechanismSectionReader_File_lacks_required_Attribute_0_,
                                            SectionNameAttributeKey);
                throw new CriticalFileReadException(message);
            }
        }

        private static PolylineShapeFileReader OpenPolyLineShapeFile(string shapeFilePath)
        {
            try
            {
                return new PolylineShapeFileReader(shapeFilePath);
            }
            catch (CriticalFileReadException e)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.FailureMechanismSectionReader_OpenPolyLineShapeFile_File_can_only_have_polylines);
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Reads a new <see cref="MapLineData"/> from the file.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CriticalFileReadException">When the line being read is a multi-polyline.</exception>
        private MapLineData ReadMapLineData()
        {
            try
            {
                return polylineShapeFileReader.ReadLine();
            }
            catch (ElementReadException e)
            {
                throw new CriticalFileReadException(RingtoetsCommonIOResources.FailureMechanismSectionReader_File_has_unsupported_multiPolyline, e);
            }
        }

        private FailureMechanismSection CreateFailureMechanismSection(MapLineData lineData)
        {
            string name = GetSectionName(lineData);
            IEnumerable<Point2D> geometryPoints = GetSectionGeometry(lineData);
            return new FailureMechanismSection(name, geometryPoints);
        }

        private string GetSectionName(MapLineData lineData)
        {
            return (string)lineData.MetaData[SectionNameAttributeKey];
        }

        private IEnumerable<Point2D> GetSectionGeometry(MapLineData lineData)
        {
            return lineData.Points.Select(p => new Point2D(p.X, p.Y));
        }
    }
}
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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.Properties;

namespace Riskeer.Common.IO.ReferenceLines
{
    /// <summary>
    /// Exports a <see cref="ReferenceLine"/> associated to an <see cref="IAssessmentSection"/> and stores it as a shapefile.
    /// </summary>
    public class ReferenceLineExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReferenceLineExporter));

        private readonly ReferenceLine referenceLine;
        private readonly string filePath;
        private readonly string id;

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineExporter"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line to export.</param>
        /// <param name="id">The id of the assessment section to which this reference line is associated.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public ReferenceLineExporter(ReferenceLine referenceLine, string id, string filePath)
        {
            IOUtils.ValidateFilePath(filePath);

            this.referenceLine = referenceLine;
            this.filePath = filePath;
            this.id = id;
        }

        public bool Export()
        {
            var referenceLineWriter = new ReferenceLineWriter();

            try
            {
                referenceLineWriter.WriteReferenceLine(referenceLine, id, filePath);
            }
            catch (CriticalFileWriteException e)
            {
                log.Error(string.Format(Resources.ReferenceLineExporter_Error_0_no_ReferenceLine_exported, e.Message));
                return false;
            }

            return true;
        }
    }
}
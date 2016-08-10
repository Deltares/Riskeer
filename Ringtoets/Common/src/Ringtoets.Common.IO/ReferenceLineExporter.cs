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

using Core.Common.Base.IO;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Exports a <see cref="ReferenceLine"/> from a <see cref="IAssessmentSection"/> and stores it in a shapefile.
    /// </summary>
    public class ReferenceLineExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReferenceLineExporter));

        private readonly ReferenceLine referenceLine;
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineExporter"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line to export.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        public ReferenceLineExporter(ReferenceLine referenceLine, string filePath)
        {
            this.referenceLine = referenceLine;
            this.filePath = filePath;
        }

        public bool Export()
        {
            return true;
        }
    }
}
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
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Integration.IO.Importers
{
    /// <summary>
    /// Importer for hydraulic location configuration database files.
    /// </summary>
    public class HydraulicLocationConfigurationDatabaseImporter : FileImporterBase<HydraulicLocationConfigurationSettings>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationDatabaseImporter"/>.
        /// </summary>
        /// <param name="importTarget">The hydraulic location configuration settings to import to.</param>
        /// <param name="filePath">The path of the hydraulic boundary database file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicLocationConfigurationDatabaseImporter(HydraulicLocationConfigurationSettings importTarget, string filePath)
            : base(filePath, importTarget) {}

        protected override bool OnImport()
        {
            throw new NotImplementedException();
        }

        protected override void LogImportCanceledMessage()
        {
            throw new NotImplementedException();
        }
    }
}
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
using Core.Common.IO.Readers;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.Integration.IO.Handlers;

namespace Ringtoets.Integration.IO.Importers
{
    /// <summary>
    /// Importer for hydraulic boundary database files and corresponding configuration files.
    /// </summary>
    public class HydraulicBoundaryDatabaseImporter : FileImporterBase<HydraulicBoundaryDatabase>
    {
        private readonly IHydraulicBoundaryDatabaseUpdateHandler updateHandler;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="importTarget">The import target.</param>
        /// <param name="updateHandler">The object responsible for updating the <see cref="HydraulicBoundaryDatabase"/>.</param>
        /// <param name="filePath">The path of the hydraulic boundary database file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseImporter(HydraulicBoundaryDatabase importTarget, IHydraulicBoundaryDatabaseUpdateHandler updateHandler,
                                                 string filePath)
            : base(filePath, importTarget)
        {
            if (updateHandler == null)
            {
                throw new ArgumentNullException(nameof(updateHandler));
            }

            this.updateHandler = updateHandler;
        }

        protected override void LogImportCanceledMessage()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnImport()
        {
            ReadResult<ReadHydraulicBoundaryDatabase> readResult = ReadHydraulicBoundaryDatabase();

            return !readResult.CriticalErrorOccurred;
        }

        private ReadResult<ReadHydraulicBoundaryDatabase> ReadHydraulicBoundaryDatabase()
        {
            NotifyProgress("Inlezen van het hydraulische belastingen bestand.", 1, 1);
            try
            {
                using (HydraulicBoundaryDatabaseReader reader = new HydraulicBoundaryDatabaseReader(FilePath))
                {
                    ReadResult<ReadHydraulicBoundaryDatabase> readResult = ReadHydraulicBoundaryDatabase(reader);

                    return readResult;
                }
            }
            catch (CriticalFileReadException e)
            {
                return HandleCriticalFileReadError<ReadHydraulicBoundaryDatabase>(e);
            }
        }

        private ReadResult<ReadHydraulicBoundaryDatabase> ReadHydraulicBoundaryDatabase(HydraulicBoundaryDatabaseReader reader)
        {
            return new ReadResult<ReadHydraulicBoundaryDatabase>(false);
        }

        private ReadResult<T> HandleCriticalFileReadError<T>(Exception e)
        {
            string errorMessage = $"{e.Message} {Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            Log.Error(errorMessage);
            return new ReadResult<T>(true);
        }
    }
}
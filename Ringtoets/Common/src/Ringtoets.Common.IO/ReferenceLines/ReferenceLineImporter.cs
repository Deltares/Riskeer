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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Ringtoets.Common.Data.AssessmentSection;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Common.IO.ReferenceLines
{
    /// <summary>
    /// Imports a <see cref="ReferenceLine"/> taking data from a shapefile containing a single polyline.
    /// </summary>
    public class ReferenceLineImporter : FileImporterBase<ReferenceLine>
    {
        private readonly List<IObservable> changedObservables = new List<IObservable>();
        private readonly IReferenceLineReplaceHandler replacementHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLineImporter"/> class.
        /// </summary>
        /// <param name="importTarget">The reference line to update.</param>
        /// <param name="replacementHandler">The object responsible for replacing the
        /// <see cref="ReferenceLine"/>.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="importTarget"/>
        /// or <paramref name="filePath"/> is <c>null</c>.</exception>
        public ReferenceLineImporter(ReferenceLine importTarget,
                                     IReferenceLineReplaceHandler replacementHandler,
                                     string filePath)
            : base(filePath, importTarget)
        {
            this.replacementHandler = replacementHandler;
        }

        protected override bool OnImport()
        {
            changedObservables.Clear();

            bool clearReferenceLineDependentData = IsClearingOfReferenceLineDependentDataRequired();
            if (Canceled)
            {
                return false;
            }

            NotifyProgress(RingtoetsCommonIOResources.ReferenceLineImporter_ProgressText_Reading_referenceline,
                           1, clearReferenceLineDependentData ? 3 : 2);
            ReadResult<ReferenceLine> readResult = ReadReferenceLine();
            if (readResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            AddReferenceLineToDataModel(readResult.Items.First(), clearReferenceLineDependentData);
            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(RingtoetsCommonIOResources.ReferenceLineImporter_ProgressText_Import_canceled_No_data_changed);
        }

        protected override void DoPostImportUpdates()
        {
            replacementHandler.DoPostReplacementUpdates();

            base.DoPostImportUpdates();

            foreach (IObservable changedObservable in changedObservables)
            {
                changedObservable.NotifyObservers();
            }
        }

        private bool IsClearingOfReferenceLineDependentDataRequired()
        {
            var clearReferenceLineDependentData = false;

            if (ImportTarget.Points.Any())
            {
                if (!replacementHandler.ConfirmReplace())
                {
                    Cancel();
                }
                else
                {
                    clearReferenceLineDependentData = true;
                }
            }

            return clearReferenceLineDependentData;
        }

        private ReadResult<ReferenceLine> ReadReferenceLine()
        {
            try
            {
                return new ReadResult<ReferenceLine>(false)
                {
                    Items = new[]
                    {
                        new ReferenceLineReader().ReadReferenceLine(FilePath)
                    }
                };
            }
            catch (ArgumentException e)
            {
                return HandleCriticalFileReadError(e);
            }
            catch (CriticalFileReadException e)
            {
                return HandleCriticalFileReadError(e);
            }
        }

        private ReadResult<ReferenceLine> HandleCriticalFileReadError(Exception e)
        {
            string errorMessage = string.Format(RingtoetsCommonIOResources.ReferenceLineImporter_HandleCriticalFileReadError_Error_0_no_referenceline_imported,
                                                e.Message);
            Log.Error(errorMessage);
            return new ReadResult<ReferenceLine>(true);
        }

        private void AddReferenceLineToDataModel(ReferenceLine importedReferenceLine, bool clearReferenceLineDependentData)
        {
            NotifyProgress(RingtoetsCommonIOResources.Importer_ProgressText_Adding_imported_data_to_data_model,
                           2, clearReferenceLineDependentData ? 3 : 2);
            if (clearReferenceLineDependentData)
            {
                NotifyProgress(RingtoetsCommonIOResources.ReferenceLineImporter_ProgressText_Removing_calculation_output_and_failure_mechanism_sections,
                               3, 3);
            }

            changedObservables.AddRange(replacementHandler.Replace(ImportTarget, importedReferenceLine).Where(o => !ReferenceEquals(o, ImportTarget)));
        }
    }
}
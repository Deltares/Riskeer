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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Imports a <see cref="ReferenceLine"/> and stores in on a <see cref="IAssessmentSection"/>,
    /// taking data from a shapefile containing a single polyline.
    /// </summary>
    public class ReferenceLineImporter : FileImporterBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReferenceLineImporter));
        private readonly IAssessmentSection importTarget;

        private readonly IList<IObservable> changedObservables = new List<IObservable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLineImporter"/> class.
        /// </summary>
        /// <param name="importTarget">The assessment section to update.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="System.ArgumentNullException">When <paramref name="importTarget"/> is <c>null</c>.</exception>
        public ReferenceLineImporter(IAssessmentSection importTarget, string filePath) : base(filePath)
        {
            if (importTarget == null)
            {
                throw new ArgumentNullException("importTarget");
            }
            this.importTarget = importTarget;
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool Import()
        {
            Canceled = false;
            changedObservables.Clear();

            bool clearReferenceLineDependentData = false;

            if (importTarget.ReferenceLine != null)
            {
                clearReferenceLineDependentData = ConfirmImportOfReferenceLineToClearReferenceLineDependentData(importTarget);
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            NotifyProgress(RingtoetsCommonIOResources.ReferenceLineImporter_ProgressText_Reading_referenceline,
                           1, clearReferenceLineDependentData ? 4 : 2);
            ReadResult<ReferenceLine> readResult = ReadReferenceLine();
            if (readResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            AddReferenceLineToDataModel(importTarget, readResult.ImportedItems.First(), clearReferenceLineDependentData);
            return true;
        }

        protected override IEnumerable<IObservable> AffectedNonTargetObservableInstances
        {
            get
            {
                return changedObservables;
            }
        }

        private bool ConfirmImportOfReferenceLineToClearReferenceLineDependentData(IAssessmentSection assessmentSection)
        {
            var clearReferenceLineDependentData = false;

            DialogResult result = MessageBox.Show(RingtoetsCommonIOResources.ReferenceLineImporter_ConfirmImport_Confirm_referenceline_import_which_clears_data_when_performed,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel)
            {
                Canceled = true;
            }
            else
            {
                if (assessmentSection.GetFailureMechanisms() != null)
                {
                    clearReferenceLineDependentData = true;
                }
            }

            return clearReferenceLineDependentData;
        }

        private static void HandleUserCancellingImport()
        {
            log.Info(RingtoetsCommonIOResources.ReferenceLineImporter_ProgressText_Import_cancelled_no_data_read);
        }

        private ReadResult<ReferenceLine> ReadReferenceLine()
        {
            try
            {
                return new ReadResult<ReferenceLine>(false)
                {
                    ImportedItems = new[]
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

        private static ReadResult<ReferenceLine> HandleCriticalFileReadError(Exception e)
        {
            var errorMessage = string.Format(RingtoetsCommonIOResources.ReferenceLineImporter_HandleCriticalFileReadError_Error_0_no_referenceline_imported,
                                             e.Message);
            log.Error(errorMessage);
            return new ReadResult<ReferenceLine>(true);
        }

        private void AddReferenceLineToDataModel(IAssessmentSection assessmentSection, ReferenceLine importedReferenceLine, bool clearReferenceLineDependentData)
        {
            NotifyProgress(RingtoetsCommonIOResources.ReferenceLineImporter_ProgressText_Adding_imported_referenceline_to_assessmentsection,
                           2, clearReferenceLineDependentData ? 4 : 2);
            assessmentSection.ReferenceLine = importedReferenceLine;

            if (clearReferenceLineDependentData && assessmentSection.GetFailureMechanisms() != null)
            {
                ClearReferenceLineDependentData(assessmentSection);
            }
        }

        private void ClearReferenceLineDependentData(IAssessmentSection assessmentSection)
        {
            NotifyProgress(RingtoetsCommonIOResources.ReferenceLineImporter_ProgressText_Removing_calculation_output_and_failure_mechanism_sections,
                           3, 4);
            foreach (var failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                ClearCalculationOutput(failureMechanism);
                ClearFailureMechanismSections(failureMechanism);
            }
            NotifyProgress(RingtoetsCommonIOResources.ReferenceLineImporter_ProgressText_Removing_hydraulic_boundary_output,
                           4, 4);
            ClearHydraulicBoundaryOutput();
        }

        private void ClearCalculationOutput(IFailureMechanism failureMechanism)
        {
            foreach (var calculationItem in failureMechanism.Calculations)
            {
                calculationItem.ClearOutput();
                changedObservables.Add(calculationItem);
            }
        }

        private void ClearFailureMechanismSections(IFailureMechanism failureMechanism)
        {
            failureMechanism.ClearAllSections();

            changedObservables.Add(failureMechanism);
        }

        private void ClearHydraulicBoundaryOutput()
        {
            // TODO: WTI-440 - Clear all 'Toetspeil' calculation output
            //changedObservables.Add(clearedInstance);
        }
    }
}
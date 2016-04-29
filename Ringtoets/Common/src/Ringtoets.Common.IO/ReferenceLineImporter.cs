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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO.Properties;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Imports a <see cref="ReferenceLine"/> and stores in on a <see cref="IAssessmentSection"/>,
    /// taking data from a shapefile containing a single polyline.
    /// </summary>
    public class ReferenceLineImporter : FileImporterBase<ReferenceLineContext>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReferenceLineImporter));

        private readonly IList<IObservable> changedObservables = new List<IObservable>();

        public override string Name
        {
            get
            {
                return RingtoetsDataResources.ReferenceLine_DisplayName;
            }
        }

        public override string Category
        {
            get
            {
                return RingtoetsFormsResources.Ringtoets_Category;
            }
        }

        public override Bitmap Image
        {
            get
            {
                return RingtoetsFormsResources.ReferenceLineIcon;
            }
        }

        public override string FileFilter
        {
            get
            {
                return String.Format("{0} shapefile (*.shp)|*.shp",
                                     RingtoetsDataResources.ReferenceLine_DisplayName);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool Import(object targetItem, string filePath)
        {
            ImportIsCancelled = false;
            changedObservables.Clear();

            bool clearReferenceLineDependentData = false;

            var importTarget = (ReferenceLineContext)targetItem;
            if (importTarget.Parent.ReferenceLine != null)
            {
                clearReferenceLineDependentData = ConfirmImportOfReferenceLineToClearReferenceLineDependentData(importTarget.Parent);
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            NotifyProgress(Resources.ReferenceLineImporter_ProgressText_Reading_referenceline,
                           1, clearReferenceLineDependentData ? 4 : 2);
            ReadResult<ReferenceLine> readResult = ReadReferenceLine(filePath);
            if (readResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            AddReferenceLineToDataModel(importTarget.Parent, readResult.ImportedItems.First(), clearReferenceLineDependentData);
            return true;
        }

        protected override IEnumerable<IObservable> GetAffectedNonTargetObservableInstances()
        {
            return changedObservables;
        }

        private bool ConfirmImportOfReferenceLineToClearReferenceLineDependentData(IAssessmentSection assessmentSection)
        {
            var clearReferenceLineDependentData = false;

            DialogResult result = MessageBox.Show(Resources.ReferenceLineImporter_ConfirmImport_Confirm_referenceline_import_which_clears_data_when_performed,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel)
            {
                ImportIsCancelled = true;
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
            log.Info(Resources.ReferenceLineImporter_ProgressText_Import_cancelled_no_data_read);
        }

        private ReadResult<ReferenceLine> ReadReferenceLine(string filePath)
        {
            try
            {
                return new ReadResult<ReferenceLine>(false)
                {
                    ImportedItems = new[]
                    {
                        new ReferenceLineReader().ReadReferenceLine(filePath)
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
            var errorMessage = String.Format(Resources.ReferenceLineImporter_HandleCriticalFileReadError_Error_0_no_referenceline_imported,
                                             e.Message);
            log.Error(errorMessage);
            return new ReadResult<ReferenceLine>(true);
        }

        private void AddReferenceLineToDataModel(IAssessmentSection assessmentSection, ReferenceLine importedReferenceLine, bool clearReferenceLineDependentData)
        {
            NotifyProgress(Resources.ReferenceLineImporter_ProgressText_Adding_imported_referenceline_to_assessmentsection,
                           2, clearReferenceLineDependentData ? 4 : 2);
            assessmentSection.ReferenceLine = importedReferenceLine;

            changedObservables.Add(assessmentSection); // Note: Add assessmentSection to the list of changed observables. Otherwise only the reference line context item will only be notified.

            if (clearReferenceLineDependentData && assessmentSection.GetFailureMechanisms() != null)
            {
                ClearReferenceLineDependentData(assessmentSection);
            }
        }

        private void ClearReferenceLineDependentData(IAssessmentSection assessmentSection)
        {
            NotifyProgress(Resources.ReferenceLineImporter_ProgressText_Removing_calculation_output_and_failure_mechanism_sections,
                           3, 4);
            foreach (var failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                ClearCalculationOutput(failureMechanism);
                ClearFailureMechanismSections(failureMechanism);
            }
            NotifyProgress(Resources.ReferenceLineImporter_ProgressText_Removing_hydraulic_boundary_output,
                           4, 4);
            ClearHydraulicBoundaryOutput(assessmentSection);
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

            var observableFailureMechanism = failureMechanism as IObservable;
            if (observableFailureMechanism != null)
            {
                changedObservables.Add(observableFailureMechanism);
            }
        }

        private void ClearHydraulicBoundaryOutput(IAssessmentSection assessmentSection)
        {
            // TODO: WTI-360 - Clear all 'Toetspeil' calculation output
            //changedObservables.Add(clearedInstance);
        }
    }
}
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
using System.Drawing;
using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;

using log4net;

using Ringtoets.Common.Data;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.Properties;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// Imports a <see cref="ReferenceLine"/> and stores in on a <see cref="AssessmentSectionBase"/>,
    /// taking data from a shapefile containing a single polyline.
    /// </summary>
    public class ReferenceLineImporter : FileImporterBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReferenceLineImporter));

        private readonly IList<IObservable> changedObservables = new List<IObservable>();

        public override string Name
        {
            get
            {
                return RingtoetsIntegrationFormsResources.ReferenceLine_DisplayName;
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
                return RingtoetsIntegrationFormsResources.ReferenceLineIcon;
            }
        }

        public override Type SupportedItemType
        {
            get
            {
                return typeof(ReferenceLineContext);
            }
        }

        public override string FileFilter
        {
            get
            {
                return String.Format("{0} shapefile (*.shp)|*.shp",
                                     RingtoetsIntegrationFormsResources.ReferenceLine_DisplayName);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool Import(object targetItem, string filePath)
        {
            bool clearReferenceLineDependentData = false;
            var importTarget = (ReferenceLineContext)targetItem;
            if (importTarget.Parent.ReferenceLine != null)
            {
                var title = "Referentielijn vervangen?";
                var text = "Weet u zeker dat u de referentielijn wilt vervangen?" + Environment.NewLine +
                           "Als u door gaat zullen alle vakindelingen, berekende hydrolische randvoorwaarden en berekeningsresultaten worden verwijderd.";
                DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                {
                    return false;
                }
                else
                {
                    clearReferenceLineDependentData = true;
                }
            }

            ReferenceLine importedReferenceLine;
            try
            {
                importedReferenceLine = new ReferenceLineReader().ReadReferenceLine(filePath);
            }
            catch (ArgumentException e)
            {
                HandleCriticalFileReadError(e);
                return false;
            }
            catch (CriticalFileReadException e)
            {
                HandleCriticalFileReadError(e);
                return false;
            }

            AddReferenceLineToDataModel(importTarget.Parent, importedReferenceLine, clearReferenceLineDependentData);
            return true;
        }

        public override void Cancel() {}

        protected override IEnumerable<IObservable> GetAffectedNonTargetObservableInstances()
        {
            return changedObservables;
        }

        private void AddReferenceLineToDataModel(AssessmentSectionBase assessmentSection, ReferenceLine importedReferenceLine, bool clearReferenceLineDependentData)
        {
            assessmentSection.ReferenceLine = importedReferenceLine;

            if (clearReferenceLineDependentData)
            {
                ClearReferenceLineDependentData(assessmentSection);
            }
        }

        private void ClearReferenceLineDependentData(AssessmentSectionBase assessmentSection)
        {
            foreach (var failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                ClearCalculationOutput(failureMechanism);
                ClearFailureMechanismSections(failureMechanism);
            }
            ClearHydraulicBoundaryOutput(assessmentSection);
        }

        private void ClearCalculationOutput(IFailureMechanism failureMechanism)
        {
            foreach (var calculationItem in failureMechanism.CalculationItems)
            {
                calculationItem.ClearOutput();
                changedObservables.Add(calculationItem);
            }
        }

        private void ClearFailureMechanismSections(IFailureMechanism failureMechanisms)
        {
            // TODO: WTI-365 - Clear all 'vakindelingen'
            //changedObservables.Add(clearedInstance);
        }

        private void ClearHydraulicBoundaryOutput(AssessmentSectionBase assessmentSection)
        {
            // TODO: WTI-360 - Clear all 'Toetspeil' calculation output
            //changedObservables.Add(clearedInstance);
        }

        private static void HandleCriticalFileReadError(Exception e)
        {
            var errorMessage = String.Format(Resources.ReferenceLineImporter_HandleCriticalFileReadError_Error_0_no_referenceline_imported,
                                             e.Message);
            log.Error(errorMessage);
        }
    }
}
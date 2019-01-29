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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.ReferenceLines;
using Riskeer.Common.Service;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.Integration.Service;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for updating a <see cref="ReferenceLine"/>.
    /// </summary>
    public class ReferenceLineUpdateHandler : IReferenceLineUpdateHandler
    {
        private readonly IAssessmentSection assessmentSection;
        private readonly IViewCommands viewCommands;
        private readonly Queue<object> removedObjects = new Queue<object>();

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineUpdateHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to remove data for.</param>
        /// <param name="viewCommands">The view commands used to close views for removed data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ReferenceLineUpdateHandler(IAssessmentSection assessmentSection, IViewCommands viewCommands)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.assessmentSection = assessmentSection;
            this.viewCommands = viewCommands;
        }

        public bool ConfirmUpdate()
        {
            DialogResult result = MessageBox.Show(Resources.ReferenceLineUpdateHandler_Confirm_clear_referenceLine_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> Update(ReferenceLine originalReferenceLine, ReferenceLine newReferenceLine)
        {
            if (originalReferenceLine == null)
            {
                throw new ArgumentNullException(nameof(originalReferenceLine));
            }

            if (newReferenceLine == null)
            {
                throw new ArgumentNullException(nameof(newReferenceLine));
            }

            removedObjects.Clear();

            ClearResults results = RingtoetsDataSynchronizationService.ClearReferenceLineDependentData(assessmentSection);
            foreach (object removedObject in results.RemovedObjects)
            {
                removedObjects.Enqueue(removedObject);
            }

            originalReferenceLine.SetGeometry(newReferenceLine.Points);
            return results.ChangedObjects;
        }

        public void DoPostUpdateActions()
        {
            while (removedObjects.Count > 0)
            {
                viewCommands.RemoveAllViewsForItem(removedObjects.Dequeue());
            }
        }
    }
}
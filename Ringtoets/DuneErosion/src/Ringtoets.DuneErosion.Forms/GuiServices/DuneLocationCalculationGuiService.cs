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
using Core.Common.Gui.Forms.ProgressDialog;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service;

namespace Ringtoets.DuneErosion.Forms.GuiServices
{
    /// <summary>
    /// This class is responsible for calculating the <see cref="DuneLocation"/>.
    /// </summary>
    public class DuneLocationCalculationGuiService
    {
        private readonly IWin32Window viewParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DuneLocationCalculationGuiService"/> class.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when the input parameter is <c>null</c>.</exception>
        public DuneLocationCalculationGuiService(IWin32Window viewParent)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException(nameof(viewParent));
            }
            this.viewParent = viewParent;
        }

        /// <summary>
        /// Performs the calculation for all <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations">The <see cref="DuneLocation"/> objects to perform the calculation for.</param>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/>
        ///  that holds information about the contribution and the general inputs used in the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that hold information about 
        /// the norm used in the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public void Calculate(IEnumerable<DuneLocation> locations,
                              DuneErosionFailureMechanism failureMechanism,
                              IAssessmentSection assessmentSection)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            ActivityProgressDialogRunner.Run(viewParent, locations
                                                 .Select(l => new DuneErosionBoundaryCalculationActivity(l,
                                                                                                         failureMechanism,
                                                                                                         assessmentSection)).ToArray());
        }
    }
}
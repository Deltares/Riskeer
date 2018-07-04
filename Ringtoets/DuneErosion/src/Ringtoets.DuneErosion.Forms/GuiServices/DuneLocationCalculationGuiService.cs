// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.GuiServices
{
    /// <summary>
    /// This class is responsible for performing dune location calculations.
    /// </summary>
    public class DuneLocationCalculationGuiService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DuneLocationCalculationGuiService));
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
        /// Performs all <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="DuneLocationCalculation"/> to perform.</param>
        /// <param name="assessmentSection">The assessment section the calculations belong to.</param>
        /// <param name="norm">The norm to use during the calculations.</param>
        /// <param name="messageProvider">The provider of the messages to use during calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/>,
        /// <paramref name="assessmentSection"/> or <paramref name="messageProvider"/> is <c>null</c>.</exception>
        public void Calculate(IEnumerable<DuneLocationCalculation> calculations,
                              IAssessmentSection assessmentSection,
                              double norm,
                              ICalculationMessageProvider messageProvider)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            string hydraulicBoundaryDatabaseFilePath = assessmentSection.HydraulicBoundaryDatabase.FilePath;
            string preprocessorDirectory = assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory();

            string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(hydraulicBoundaryDatabaseFilePath,
                                                                                                   preprocessorDirectory);

            if (string.IsNullOrEmpty(validationProblem))
            {
                TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(norm, logMessage => validationProblem = logMessage);
            }

            if (!string.IsNullOrEmpty(validationProblem))
            {
                log.ErrorFormat(RingtoetsCommonFormsResources.CalculateHydraulicBoundaryLocation_Start_calculation_failed_0_,
                                validationProblem);
                return;
            }

            ActivityProgressDialogRunner.Run(
                viewParent,
                calculations.Select(calculation => new DuneLocationCalculationActivity(calculation,
                                                                                       hydraulicBoundaryDatabaseFilePath,
                                                                                       preprocessorDirectory,
                                                                                       norm,
                                                                                       messageProvider)).ToArray());
        }
    }
}
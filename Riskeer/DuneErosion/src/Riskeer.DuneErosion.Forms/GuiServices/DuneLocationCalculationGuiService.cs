// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Gui.Forms;
using Core.Gui.Forms.ProgressDialog;
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Service;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.GuiServices
{
    /// <summary>
    /// This class is responsible for performing dune location calculations.
    /// </summary>
    public class DuneLocationCalculationGuiService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DuneLocationCalculationGuiService));
        private readonly IViewParent viewParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DuneLocationCalculationGuiService"/> class.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when the input parameter is <c>null</c>.</exception>
        public DuneLocationCalculationGuiService(IViewParent viewParent)
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
        /// <param name="targetProbability">The target probability to use during the calculations.</param>
        /// <param name="calculationIdentifier">The calculation identifier to use in all messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or
        /// <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="calculationIdentifier"/> is <c>null</c> or empty.</exception>
        public void Calculate(IEnumerable<DuneLocationCalculation> calculations,
                              IAssessmentSection assessmentSection,
                              double targetProbability,
                              string calculationIdentifier)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            string hydraulicBoundaryDatabaseFilePath = assessmentSection.HydraulicBoundaryDatabase.FilePath;
            string preprocessorDirectory = assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory();
            HydraulicLocationConfigurationSettings hydraulicLocationConfigurationSettings = assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;

            string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(hydraulicBoundaryDatabaseFilePath,
                                                                                                   hydraulicLocationConfigurationSettings.FilePath,
                                                                                                   preprocessorDirectory,
                                                                                                   hydraulicLocationConfigurationSettings.UsePreprocessorClosure);

            if (string.IsNullOrEmpty(validationProblem))
            {
                TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(targetProbability, logMessage => validationProblem = logMessage);
            }

            if (!string.IsNullOrEmpty(validationProblem))
            {
                log.ErrorFormat(RiskeerCommonFormsResources.CalculateHydraulicBoundaryLocation_Start_calculation_failed_0_,
                                validationProblem);
                return;
            }

            ActivityProgressDialogRunner.Run(viewParent,
                                             DuneLocationCalculationActivityFactory.CreateCalculationActivities(calculations,
                                                                                                                assessmentSection,
                                                                                                                targetProbability,
                                                                                                                calculationIdentifier));
        }
    }
}
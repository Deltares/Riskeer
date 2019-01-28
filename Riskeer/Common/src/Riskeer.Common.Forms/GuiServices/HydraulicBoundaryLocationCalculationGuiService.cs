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
using Core.Common.Gui.Forms.ProgressDialog;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;

namespace Ringtoets.Common.Forms.GuiServices
{
    /// <summary>
    /// This class is responsible for calculating design water levels and wave heights.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationGuiService : IHydraulicBoundaryLocationCalculationGuiService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationCalculationGuiService));
        private readonly IWin32Window viewParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HydraulicBoundaryLocationCalculationGuiService"/> class.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when the input parameter is <c>null</c>.</exception>
        public HydraulicBoundaryLocationCalculationGuiService(IWin32Window viewParent)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException(nameof(viewParent));
            }

            this.viewParent = viewParent;
        }

        public void CalculateDesignWaterLevels(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                               IAssessmentSection assessmentSection,
                                               double norm,
                                               string categoryBoundaryName)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            RunActivities(assessmentSection.HydraulicBoundaryDatabase.FilePath,
                          assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                          norm,
                          HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(calculations,
                                                                                                                          assessmentSection,
                                                                                                                          norm,
                                                                                                                          categoryBoundaryName));
        }

        public void CalculateWaveHeights(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                         IAssessmentSection assessmentSection,
                                         double norm,
                                         string categoryBoundaryName)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            RunActivities(assessmentSection.HydraulicBoundaryDatabase.FilePath,
                          assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                          norm,
                          HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(calculations,
                                                                                                                    assessmentSection,
                                                                                                                    norm,
                                                                                                                    categoryBoundaryName));
        }

        private void RunActivities(string hydraulicBoundaryDatabaseFilePath,
                                   string preprocessorDirectory,
                                   double norm,
                                   IEnumerable<CalculatableActivity> activities)
        {
            string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(hydraulicBoundaryDatabaseFilePath,
                                                                                                   preprocessorDirectory);
            if (string.IsNullOrEmpty(validationProblem))
            {
                TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(norm, logMessage => validationProblem = logMessage);
            }

            if (string.IsNullOrEmpty(validationProblem))
            {
                ActivityProgressDialogRunner.Run(viewParent, activities);
            }
            else
            {
                log.ErrorFormat(Resources.CalculateHydraulicBoundaryLocation_Start_calculation_failed_0_,
                                validationProblem);
            }
        }
    }
}
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
using Core.Common.Base.Service;
using Core.Common.Gui.Forms.ProgressDialog;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;

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

        public void CalculateDesignWaterLevels(string hydraulicBoundaryDatabaseFilePath,
                                               string preprocessorDirectory,
                                               IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                               double norm,
                                               ICalculationMessageProvider messageProvider)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            RunActivities(hydraulicBoundaryDatabaseFilePath,
                          preprocessorDirectory,
                          calculations.Select(calculation => new DesignWaterLevelCalculationActivity(calculation,
                                                                                                     hydraulicBoundaryDatabaseFilePath,
                                                                                                     preprocessorDirectory,
                                                                                                     norm,
                                                                                                     messageProvider)).ToArray());
        }

        public void CalculateWaveHeights(string hydraulicBoundaryDatabaseFilePath,
                                         string preprocessorDirectory,
                                         IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                         double norm,
                                         ICalculationMessageProvider messageProvider)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            RunActivities(hydraulicBoundaryDatabaseFilePath,
                          preprocessorDirectory,
                          calculations.Select(calculation => new DesignWaterLevelCalculationActivity(calculation,
                                                                                                     hydraulicBoundaryDatabaseFilePath,
                                                                                                     preprocessorDirectory,
                                                                                                     norm,
                                                                                                     messageProvider)).ToArray());
        }

        private void RunActivities<TActivity>(string hydraulicBoundaryDatabasePath, string preprocessorDirectory,
                                              IEnumerable<TActivity> activities) where TActivity : Activity
        {
            string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(hydraulicBoundaryDatabasePath,
                                                                                                   preprocessorDirectory);
            if (string.IsNullOrEmpty(validationProblem))
            {
                ActivityProgressDialogRunner.Run(viewParent, activities);
                return;
            }

            log.ErrorFormat(Resources.CalculateHydraulicBoundaryLocation_Start_calculation_failed_0_,
                            validationProblem);
        }
    }
}
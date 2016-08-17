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
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.Integration.Service;

namespace Ringtoets.Integration.Forms.Commands
{
    /// <summary>
    /// This class is responsible for calculating the <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>.
    /// </summary>
    public class CalculateDesignWaterLevelCommandHandler : ICalculateDesignWaterLevelCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CalculateDesignWaterLevelCommandHandler));
        private readonly IWin32Window viewParent;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateDesignWaterLevelCommandHandler"/> class.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        public CalculateDesignWaterLevelCommandHandler(IWin32Window viewParent, IAssessmentSection assessmentSection)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException("viewParent");
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }
            this.viewParent = viewParent;
            this.assessmentSection = assessmentSection;
        }

        public void CalculateDesignWaterLevels(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            var hrdFile = assessmentSection.HydraulicBoundaryDatabase.FilePath;
            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hrdFile);
            if (string.IsNullOrEmpty(validationProblem))
            {
                var activities = locations.Select(hbl => new DesignWaterLevelCalculationActivity(assessmentSection, hbl)).ToArray();

                ActivityProgressDialogRunner.Run(viewParent, activities);

                assessmentSection.HydraulicBoundaryDatabase.NotifyObservers();
            }
            else
            {
                log.ErrorFormat(Resources.CalculateHydraulicBoundaryLocation_ContextMenuStrip_Start_calculation_failed_0_, validationProblem);
            }
        }
    }
}
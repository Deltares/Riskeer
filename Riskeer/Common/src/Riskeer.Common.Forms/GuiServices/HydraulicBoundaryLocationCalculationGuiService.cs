﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;

namespace Riskeer.Common.Forms.GuiServices
{
    /// <summary>
    /// This class is responsible for calculating water levels and wave heights.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationGuiService : IHydraulicBoundaryLocationCalculationGuiService
    {
        private readonly IViewParent viewParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HydraulicBoundaryLocationCalculationGuiService"/> class.
        /// </summary>
        /// <param name="viewParent">The parent of the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when the input parameter is <c>null</c>.</exception>
        public HydraulicBoundaryLocationCalculationGuiService(IViewParent viewParent)
        {
            if (viewParent == null)
            {
                throw new ArgumentNullException(nameof(viewParent));
            }

            this.viewParent = viewParent;
        }

        public void CalculateDesignWaterLevels(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                               IAssessmentSection assessmentSection,
                                               double targetProbability,
                                               string calculationIdentifier)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            ActivityProgressDialogRunner.Run(viewParent, HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                                                 calculations, assessmentSection, targetProbability, calculationIdentifier));
        }

        public void CalculateWaveHeights(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                         IAssessmentSection assessmentSection,
                                         double targetProbability,
                                         string calculationIdentifier)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            ActivityProgressDialogRunner.Run(viewParent, HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                                                 calculations, assessmentSection, targetProbability, calculationIdentifier));
        }
    }
}
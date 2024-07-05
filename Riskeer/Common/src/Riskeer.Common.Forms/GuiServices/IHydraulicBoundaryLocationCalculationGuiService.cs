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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Forms.GuiServices
{
    /// <summary>
    /// Interface for water level and wave height calculations.
    /// </summary>
    public interface IHydraulicBoundaryLocationCalculationGuiService
    {
        /// <summary>
        /// Performs the provided water level calculations.
        /// </summary>
        /// <param name="calculations">The calculations to perform.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculations"/> belong to.</param>
        /// <param name="targetProbability">The target probability to use during the calculations.</param>
        /// <param name="calculationIdentifier">The calculation identifier to use in all messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="calculationIdentifier"/> is <c>null</c> or empty.</exception>
        void CalculateDesignWaterLevels(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                        IAssessmentSection assessmentSection,
                                        double targetProbability,
                                        string calculationIdentifier);

        /// <summary>
        /// Performs the provided wave height calculations.
        /// </summary>
        /// <param name="calculations">The calculations to perform.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculations"/> belong to.</param>
        /// <param name="targetProbability">The target probability to use during the calculations.</param>
        /// <param name="calculationIdentifier">The calculation identifier to use in all messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="calculationIdentifier"/> is <c>null</c> or empty.</exception>
        void CalculateWaveHeights(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                  IAssessmentSection assessmentSection,
                                  double targetProbability,
                                  string calculationIdentifier);
    }
}
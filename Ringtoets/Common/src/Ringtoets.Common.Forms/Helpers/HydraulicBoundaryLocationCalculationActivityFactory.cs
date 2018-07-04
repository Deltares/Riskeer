﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Factory for creating hydraulic boundary location calculation activities.
    /// </summary>
    public static class HydraulicBoundaryLocationCalculationActivityFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="WaveHeightCalculationActivity"/> based on the
        /// given parameters.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path of the hydraulic boundary database file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="calculations">The collection of <see cref="HydraulicBoundaryLocationCalculation"/> to create
        /// the activities for.</param>
        /// <param name="norm">The norm to use during the calculations.</param>
        /// <param name="categoryBoundaryName">The category boundary name for the calculations.</param>
        /// <returns>A collection of <see cref="WaveHeightCalculationActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public static IEnumerable<WaveHeightCalculationActivity> CreateWaveHeightCalculationActivities(
            string hydraulicBoundaryDatabaseFilePath,
            string preprocessorDirectory,
            IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
            double norm,
            string categoryBoundaryName)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (categoryBoundaryName == null)
            {
                throw new ArgumentNullException(nameof(categoryBoundaryName));
            }

            return calculations.Select(calculation => new WaveHeightCalculationActivity(calculation,
                                                                                        hydraulicBoundaryDatabaseFilePath,
                                                                                        preprocessorDirectory,
                                                                                        norm,
                                                                                        categoryBoundaryName)).ToArray();
        }

        /// <summary>
        /// Creates a collection of <see cref="DesignWaterLevelCalculationActivity"/> based on the
        /// given parameters.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path of the hydraulic boundary database file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="calculations">The collection of <see cref="HydraulicBoundaryLocationCalculation"/> to create
        /// the activities for.</param>
        /// <param name="norm">The norm to use during the calculations.</param>
        /// <param name="messageProvider">The message provider for the activities.</param>
        /// <returns>A collection of <see cref="DesignWaterLevelCalculationActivity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/>
        /// or <paramref name="messageProvider"/> is <c>null</c>.</exception>
        public static IEnumerable<DesignWaterLevelCalculationActivity> CreateDesignWaterLevelCalculationActivities(
            string hydraulicBoundaryDatabaseFilePath,
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

            return calculations.Select(calculation => new DesignWaterLevelCalculationActivity(calculation,
                                                                                              hydraulicBoundaryDatabaseFilePath,
                                                                                              preprocessorDirectory,
                                                                                              norm,
                                                                                              messageProvider)).ToArray();
        }
    }
}
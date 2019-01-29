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

namespace Riskeer.Common.IO.TestUtil
{
    /// <summary>
    /// Wrapper class for progress notifications which can be used for testing.
    /// </summary>
    public class ProgressNotification
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProgressNotification"/>.
        /// </summary>
        /// <param name="description">The progress notification description.</param>
        /// <param name="currentStep">The current step count.</param>
        /// <param name="totalSteps">The total number of steps.</param>
        public ProgressNotification(string description, int currentStep, int totalSteps)
        {
            Description = description;
            CurrentStep = currentStep;
            TotalSteps = totalSteps;
        }

        /// <summary>
        /// Gets the progress notification description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the current step count.
        /// </summary>
        public int CurrentStep { get; }

        /// <summary>
        /// Gets the total number of steps.
        /// </summary>
        public int TotalSteps { get; }
    }
}
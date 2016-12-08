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

namespace Application.Ringtoets.Storage.DbContext
{
    public interface IHydraulicLocationOutputEntity
    {
        double? Result { get; set; }

        /// <summary>
        /// Gets or sets the required (maximum allowed) probability of failure.
        /// </summary>
        double? TargetProbability { get; set; }

        /// <summary>
        /// Gets or set the required (maximum allowed) reliability of the failure mechanism.
        /// </summary>
        double? TargetReliability { get; set; }

        /// <summary>
        /// Gets or sets the probability of failure.
        /// </summary>
        double? CalculatedProbability { get; set; }

        /// <summary>
        /// Gets or sets the reliability of the failure mechanism.
        /// </summary>
        double? CalculatedReliability { get; set; }

        byte CalculationConvergence { get; set; }
        byte HydraulicLocationOutputType { get; set; }
    }
}
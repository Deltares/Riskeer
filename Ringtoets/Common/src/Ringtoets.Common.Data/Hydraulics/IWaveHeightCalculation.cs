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

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Interface for a wave height calculation
    /// </summary>
    public interface IWaveHeightCalculation
    {
        /// <summary>
        /// Gets the database id of the hydraulic boundary location.
        /// </summary>
        /// <returns>The database id of the hydraulic boundary location.</returns>
        long GetId();

        /// <summary>
        /// Gets the name of the hydraulic boundary location.
        /// </summary>
        /// <returns>The name of the hydraulic boundary location.</returns>
        string GetName();

        /// <summary>
        /// Gets if the illustration points should be calculated.
        /// </summary>
        /// <returns>The illustration points should be calculated.</returns>
        bool GetCalculateIllustrationPoints();

        /// <summary>
        ///Sets the output of the design water level calculation.
        /// </summary>
        void SetOutput(HydraulicBoundaryLocationOutput output);
    }
}
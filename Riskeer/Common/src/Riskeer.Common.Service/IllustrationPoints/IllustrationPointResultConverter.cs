// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.IllustrationPoints;
using HydraRingIllustrationPointResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;

namespace Riskeer.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraRingIllustrationPointResult"/> data into 
    /// <see cref="IllustrationPointResult"/> data.
    /// </summary>
    public static class IllustrationPointResultConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointResult"/> based on the information 
        /// of <paramref name="hydraRingIllustrationPointResult"/>.
        /// </summary>
        /// <param name="hydraRingIllustrationPointResult">The <see cref="HydraRingIllustrationPointResult"/> 
        /// to base the <see cref="IllustrationPointResult"/> to create on.</param>
        /// <returns>The newly created <see cref="IllustrationPointResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingIllustrationPointResult"/> 
        /// is <c>null</c>.</exception>
        public static IllustrationPointResult Convert(HydraRingIllustrationPointResult hydraRingIllustrationPointResult)
        {
            if (hydraRingIllustrationPointResult == null)
            {
                throw new ArgumentNullException(nameof(hydraRingIllustrationPointResult));
            }

            return new IllustrationPointResult(hydraRingIllustrationPointResult.Description,
                                               hydraRingIllustrationPointResult.Value);
        }
    }
}
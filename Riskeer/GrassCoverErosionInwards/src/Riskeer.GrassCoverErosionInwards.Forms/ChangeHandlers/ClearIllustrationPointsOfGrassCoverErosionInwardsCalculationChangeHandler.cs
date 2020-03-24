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
using Core.Common.Gui.Helpers;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Util;

namespace Riskeer.GrassCoverErosionInwards.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing illustration point results from a grass cover erosion inwards calculation.
    /// </summary>
    public class ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler
        : ClearIllustrationPointsOfCalculationChangeHandlerBase<GrassCoverErosionInwardsCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="calculation">The calculation to clear the illustration points for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        public ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler(IInquiryHelper inquiryHelper,
                                                                                         GrassCoverErosionInwardsCalculation calculation)
            : base(inquiryHelper, calculation) {}

        public override bool ClearIllustrationPoints()
        {
            if (GrassCoverErosionInwardsIllustrationPointsHelper.HasIllustrationPoints(Calculation))
            {
                Calculation.ClearIllustrationPoints();
                return true;
            }

            return false;
        }
    }
}
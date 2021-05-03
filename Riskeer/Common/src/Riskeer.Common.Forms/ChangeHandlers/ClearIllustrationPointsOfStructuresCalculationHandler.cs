﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Gui.Helpers;
using Riskeer.Common.Data.Structures;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing illustration point results from a structures calculation.
    /// </summary>
    public class ClearIllustrationPointsOfStructuresCalculationHandler
        : ClearIllustrationPointsOfCalculationChangeHandlerBase<IStructuresCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfStructuresCalculationHandler"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="calculation">The calculation to clear the illustration points for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearIllustrationPointsOfStructuresCalculationHandler(IInquiryHelper inquiryHelper,
                                                                     IStructuresCalculation calculation)
            : base(inquiryHelper, calculation) {}

        public override bool ClearIllustrationPoints()
        {
            if (Calculation.HasOutput && Calculation.Output.HasGeneralResult)
            {
                Calculation.ClearIllustrationPoints();
                return true;
            }

            return false;
        }
    }
}
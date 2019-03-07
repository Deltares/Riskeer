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
using Core.Common.Gui;
using Riskeer.Common.Data.Structures;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing illustration point results of a single structures calculation.
    /// </summary>
    /// <typeparam name="TStructureInput">Object type of the structure calculation input.</typeparam>
    public class ClearIllustrationPointsOfStructuresCalculationHandler<TStructureInput>
        : ClearIllustrationPointsOfCalculationChangeHandlerBase<StructuresCalculation<TStructureInput>>
        where TStructureInput : IStructuresCalculationInput, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfStructuresCalculationHandler{TStructureInput}"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <param name="calculation">The calculation for which the illustration points should be cleared for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearIllustrationPointsOfStructuresCalculationHandler(IInquiryHelper inquiryHelper,
                                                                     StructuresCalculation<TStructureInput> calculation)
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
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
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Util;

namespace Riskeer.Piping.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing illustration point results from a probabilistic piping calculation.
    /// </summary>
    public class ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler
        : ClearIllustrationPointsOfCalculationChangeHandlerBase<ProbabilisticPipingCalculationScenario>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler"/>.
        /// </summary>
        /// <param name="calculation">The calculation to clear the illustration points for.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="viewCommands">The view commands used to close views for the illustration points.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        public ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(
            ProbabilisticPipingCalculationScenario calculation, IInquiryHelper inquiryHelper, IViewCommands viewCommands)
            : base(calculation, inquiryHelper, viewCommands) {}

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Thrown when a calculations has
        /// <see cref="IPartialProbabilisticPipingOutput"/> that is of an unsupported type.</exception>
        public override bool ClearIllustrationPoints()
        {
            if (ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(Calculation))
            {
                if (Calculation.Output.SectionSpecificOutput.HasGeneralResult)
                {
                    CloseView(Calculation.Output.SectionSpecificOutput);
                }

                if (Calculation.Output.ProfileSpecificOutput.HasGeneralResult)
                {
                    CloseView(Calculation.Output.ProfileSpecificOutput);
                }

                Calculation.ClearIllustrationPoints();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Closes the view for the illustration points on the given <paramref name="partialOutput"/>.
        /// </summary>
        /// <param name="partialOutput">The output that contains the illustration points to close the view for.</param>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="partialOutput"/> is of an unsupported type.</exception>
        private void CloseView(IPartialProbabilisticPipingOutput partialOutput)
        {
            switch (partialOutput)
            {
                case PartialProbabilisticFaultTreePipingOutput faultTreeOutput:
                    ViewCommands.RemoveAllViewsForItem(faultTreeOutput.GeneralResult);
                    break;
                case PartialProbabilisticSubMechanismPipingOutput subMechanismOutput:
                    ViewCommands.RemoveAllViewsForItem(subMechanismOutput.GeneralResult);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
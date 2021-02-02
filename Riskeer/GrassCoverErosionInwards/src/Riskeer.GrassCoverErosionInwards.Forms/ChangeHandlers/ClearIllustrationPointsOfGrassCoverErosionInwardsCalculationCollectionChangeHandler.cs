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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Service;
using Riskeer.GrassCoverErosionInwards.Util;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing the illustration points of a collection of grass cover erosion inwards calculations.
    /// </summary>
    public class ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler
        : ClearIllustrationPointsAndCloseViewOfCalculationCollectionChangeHandlerBase
    {
        private readonly IEnumerable<GrassCoverErosionInwardsCalculation> calculations;

        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler"/>.
        /// </summary>
        /// <param name="calculations">The calculations for which the illustration points should be cleared.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="viewCommands">The view commands used to close views for the illustration points.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationCollectionChangeHandler(
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations, IInquiryHelper inquiryHelper, IViewCommands viewCommands)
            : base(inquiryHelper, viewCommands)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            this.calculations = calculations;
        }

        protected override IEnumerable<IObservable> PerformClearIllustrationPoints()
        {
            return GrassCoverErosionInwardsDataSynchronizationService.ClearIllustrationPoints(calculations);
        }

        protected override void CloseView(IViewCommands viewCommands)
        {
            foreach (GrassCoverErosionInwardsCalculation calculation in calculations.Where(GrassCoverErosionInwardsIllustrationPointsHelper.HasIllustrationPoints))
            {
                GrassCoverErosionInwardsOutput output = calculation.Output;

                if (GrassCoverErosionInwardsIllustrationPointsHelper.HasOverToppingIllustrationPoints(output))
                {
                    viewCommands.RemoveAllViewsForItem(output.OvertoppingOutput.GeneralResult);
                }

                if (GrassCoverErosionInwardsIllustrationPointsHelper.HasDikeHeightOutputWithIllustrationPoints(output))
                {
                    viewCommands.RemoveAllViewsForItem(output.DikeHeightOutput.GeneralResult);
                }

                if (GrassCoverErosionInwardsIllustrationPointsHelper.HasOverToppingRateOutputWithIllustrationPoints(output))
                {
                    viewCommands.RemoveAllViewsForItem(output.OvertoppingRateOutput.GeneralResult);
                }
            }
        }

        protected override string GetConfirmationMessage()
        {
            return RiskeerCommonFormsResources.ClearIllustrationPointsCalculationCollection_ConfirmationMessage;
        }
    }
}
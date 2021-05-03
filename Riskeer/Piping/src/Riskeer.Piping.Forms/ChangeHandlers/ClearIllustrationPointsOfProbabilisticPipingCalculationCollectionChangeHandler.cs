// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Gui.Helpers;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Util;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing illustration points from a collection of probabilistic piping calculation scenarios.
    /// </summary>
    public class ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler : ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase
    {
        private readonly IEnumerable<ProbabilisticPipingCalculationScenario> calculations;

        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="calculations">The calculations for which the illustration points should be cleared.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(IInquiryHelper inquiryHelper,
                                                                                              IEnumerable<ProbabilisticPipingCalculationScenario> calculations)
            : base(inquiryHelper)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            this.calculations = calculations;
        }

        public override IEnumerable<IObservable> ClearIllustrationPoints()
        {
            var affectedObjects = new List<IObservable>();
            foreach (ProbabilisticPipingCalculationScenario calculation in calculations.Where(ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints))
            {
                affectedObjects.Add(calculation);
                calculation.ClearIllustrationPoints();
            }

            return affectedObjects;
        }

        protected override string GetConfirmationMessage()
        {
            return RiskeerCommonFormsResources.ClearIllustrationPointsCalculationCollection_ConfirmationMessage;
        }
    }
}
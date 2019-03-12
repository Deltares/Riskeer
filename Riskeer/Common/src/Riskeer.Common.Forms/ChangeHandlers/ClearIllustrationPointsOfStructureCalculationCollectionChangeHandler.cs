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
using Core.Common.Base;
using Core.Common.Gui;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Service;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing illustration points from a collection of structure calculations.
    /// </summary>
    public class ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler
        : ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase
    {
        private readonly IEnumerable<IStructuresCalculation> calculations;

        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="calculations">The calculations for which the illustration points should be cleared for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler(IInquiryHelper inquiryHelper,
                                                                                    IEnumerable<IStructuresCalculation> calculations)
            : base(inquiryHelper, Resources.ClearIllustrationPointsCalculationCollection_Inquiry)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            this.calculations = calculations;
        }

        public override IEnumerable<IObservable> ClearIllustrationPoints()
        {
            return RiskeerCommonDataSynchronizationService.ClearStructuresCalculationIllustrationPoints(calculations);
        }
    }
}
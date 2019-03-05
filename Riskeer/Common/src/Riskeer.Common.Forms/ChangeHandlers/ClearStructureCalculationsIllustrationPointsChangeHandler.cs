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
using Riskeer.Common.Data;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Service;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling structure calculations when its illustration points need to be cleared.
    /// </summary>
    /// <typeparam name="TStructureInput">Object type of the structure calculation input.</typeparam>
    /// <typeparam name="TStructure">Object type of the structure property of <typeparamref name="TStructureInput"/>.</typeparam>
    public class ClearStructureCalculationsIllustrationPointsChangeHandler<TStructureInput, TStructure> : ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase
        where TStructureInput : StructuresInputBase<TStructure>, new()
        where TStructure : StructureBase
    {
        private readonly IEnumerable<StructuresCalculation<TStructureInput>> calculations;

        /// <summary>
        /// Creates a new instance of <see cref="ClearStructureCalculationsIllustrationPointsChangeHandler{TStructuresInput,TStructure}"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <param name="inquiry">The description of the collection in which the illustration point results belong to.</param>
        /// <param name="calculations">The calculations for which the illustration points should be cleared for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearStructureCalculationsIllustrationPointsChangeHandler(IInquiryHelper inquiryHelper,
                                                                         string inquiry,
                                                                         IEnumerable<StructuresCalculation<TStructureInput>> calculations)
            : base(inquiryHelper, inquiry)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            this.calculations = calculations;
        }

        public override IEnumerable<IObservable> ClearIllustrationPoints()
        {
            return RiskeerCommonDataSynchronizationService.ClearStructuresCalculationIllustrationPoints<TStructureInput, TStructure>(calculations);
        }
    }
}
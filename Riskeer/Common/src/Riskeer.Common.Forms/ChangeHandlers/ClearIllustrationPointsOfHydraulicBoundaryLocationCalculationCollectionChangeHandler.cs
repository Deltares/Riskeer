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
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing the illustration points results from a collection of hydraulic boundary location calculations.
    /// </summary>
    public class ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler
        : ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase
    {
        private readonly string collectionDescription;
        private readonly Func<IEnumerable<IObservable>> clearIllustrationPointsFunc;

        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="collectionDescription">The description of the collection in which the illustration points results belong to.</param>
        /// <param name="clearIllustrationPointsFunc">The function to clear the illustration point results.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(IInquiryHelper inquiryHelper,
                                                                                                    string collectionDescription,
                                                                                                    Func<IEnumerable<IObservable>> clearIllustrationPointsFunc)
            : base(inquiryHelper)
        {
            if (collectionDescription == null)
            {
                throw new ArgumentNullException(nameof(collectionDescription));
            }

            if (clearIllustrationPointsFunc == null)
            {
                throw new ArgumentNullException(nameof(clearIllustrationPointsFunc));
            }

            this.collectionDescription = collectionDescription;
            this.clearIllustrationPointsFunc = clearIllustrationPointsFunc;
        }

        public override IEnumerable<IObservable> ClearIllustrationPoints()
        {
            return clearIllustrationPointsFunc();
        }

        protected override string GetConfirmationMessage()
        {
            return string.Format(Resources.ClearHydraulicBoundaryLocationCalculationsIllustrationPointsChangeHandler_ClearIllustrationPoints_Remove_calculated_IllustrationPoints_for_collection_0_, collectionDescription);
        }
    }
}
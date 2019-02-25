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
using Core.Common.Util.Extensions;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.ChangeHandlers {
    /// <summary>
    /// Class for handling objects when illustration point results need to be cleared.
    /// </summary>
    public class ClearIllustrationPointsChangeHandler
    {
        private readonly IInquiryHelper inquiryHelper;
        private readonly string itemDescription;
        private readonly Func<IEnumerable<IObservable>> clearIllustrationPointsFunc;

        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsChangeHandler"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <param name="itemDescription">The description of the item for which the illustration points results are cleared.</param>
        /// <param name="clearIllustrationPointsFunc">The function to clear the illustration point results.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearIllustrationPointsChangeHandler(IInquiryHelper inquiryHelper, 
                                                    string itemDescription,
                                                    Func<IEnumerable<IObservable>> clearIllustrationPointsFunc)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            if (itemDescription == null)
            {
                throw new ArgumentNullException(nameof(itemDescription));
            }

            if (clearIllustrationPointsFunc == null)
            {
                throw new ArgumentNullException(nameof(clearIllustrationPointsFunc));
            }

            this.inquiryHelper = inquiryHelper;
            this.itemDescription = itemDescription;
            this.clearIllustrationPointsFunc = clearIllustrationPointsFunc;
        }

        /// <summary>
        /// Clears all illustration points and updates the affected objects.
        /// </summary>
        public void ClearIllustrationPoints()
        {
            string message = string.Format(Resources.ClearIllustrationPointsChangeHandler_ClearIllustrationPoints_Remove_calculated_IllustrationPoints_for_collection_0_, 
                                           itemDescription);
            if (inquiryHelper.InquireContinuation(message))
            {
                IEnumerable<IObservable> affectedObjects = clearIllustrationPointsFunc();
                affectedObjects.ForEachElementDo(observable => observable.NotifyObservers());
            }
        }
    }
}
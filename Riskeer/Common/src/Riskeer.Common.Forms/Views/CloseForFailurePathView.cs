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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view which closes for an <see cref="IFailurePath"/> instance.
    /// </summary>
    public abstract class CloseForFailurePathView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="CloseForFailurePathView"/>.
        /// </summary>
        /// <param name="failurePath">The failure path belonging to the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failurePath"/>
        /// is <c>null</c>.</exception>
        protected CloseForFailurePathView(IFailurePath failurePath)
        {
            if (failurePath == null)
            {
                throw new ArgumentNullException(nameof(failurePath));
            }

            FailurePath = failurePath;
        }

        /// <summary>
        /// Gets the <see cref="IFailurePath"/> the view belongs to.
        /// </summary>
        public IFailurePath FailurePath { get; }

        public object Data { get; set; }
    }
}
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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view which closes for an <see cref="IFailureMechanism"/> instance.
    /// </summary>
    public abstract class CloseForFailureMechanismView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="CloseForFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism belonging to the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        protected CloseForFailureMechanismView(IFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the <see cref="IFailureMechanism"/> the view belongs to.
        /// </summary>
        public IFailureMechanism FailureMechanism { get; }

        public object Data { get; set; }
    }
}
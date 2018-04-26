// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// View for a collection of <see cref="FailureMechanismSection"/>, also showing information on
    /// the length effect.
    /// </summary>
    public class LengthEffectAwareFailureMechanismSectionsView : FailureMechanismSectionsView
    {
        private readonly Func<FailureMechanismSection, double> getLengthEffectFunc;

        /// <summary>
        /// Creates a new instance of <see cref="LengthEffectAwareFailureMechanismSectionsView"/>.
        /// </summary>
        /// <param name="sections">The sections to be displayed in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the view belongs to.</param>
        /// <param name="getLengthEffectFunc">The <see cref="Func{T,TResult}"/> for obtaining the
        /// length effect value for a <see cref="FailureMechanismSection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public LengthEffectAwareFailureMechanismSectionsView(IEnumerable<FailureMechanismSection> sections,
                                                             IFailureMechanism failureMechanism,
                                                             Func<FailureMechanismSection, double> getLengthEffectFunc)
            : base(sections, failureMechanism)
        {
            if (getLengthEffectFunc == null)
            {
                throw new ArgumentNullException(nameof(getLengthEffectFunc));
            }

            this.getLengthEffectFunc = getLengthEffectFunc;
        }

        protected override void UpdateTableData()
        {
            failureMechanismSectionsTable.SetDataSource(sections.Select(section => new FailureMechanismSectionRow(section)).ToArray());
        }
    }
}
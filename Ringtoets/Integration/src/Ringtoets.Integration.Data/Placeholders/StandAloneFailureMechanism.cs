// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Placeholder;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Integration.Data.Placeholders
{
    /// <summary>
    /// Defines a placeholder for stand alone failure mechanisms objects
    /// </summary>
    public class StandAloneFailureMechanism : FailureMechanismBase<FailureMechanismSectionResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandAloneFailureMechanism"/> class.
        /// </summary>
        /// <param name="name">The name for the <see cref="StandAloneFailureMechanism"/>.</param>
        /// <param name="code">The code for the <see cref="StandAloneFailureMechanism"/>.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="name"/> is <c>null</c> or empty.</item>
        /// <item><paramref name="code"/> is <c>null</c> or empty.</item>
        /// </list>
        /// </exception>
        public StandAloneFailureMechanism(string name, string code)
            : base(name, code)
        {
            AssessmentResult = new OutputPlaceholder(RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName);
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        protected override FailureMechanismSectionResult CreateFailureMechanismSectionResult(FailureMechanismSection section)
        {
            return new FailureMechanismSectionResult(section);
        }

        /// <summary>
        /// Gets the calculation results for this failure mechanism.
        /// </summary>
        public OutputPlaceholder AssessmentResult { get; private set; }
    }
}
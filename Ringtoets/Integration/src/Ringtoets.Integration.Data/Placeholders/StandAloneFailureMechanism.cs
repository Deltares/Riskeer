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
    /// Defines a placeholder for unimplemented failure mechanisms objects
    /// </summary>
    public class StandAloneFailureMechanism : FailureMechanismBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandAloneFailureMechanism"/> class.
        /// </summary>
        /// <param name="name">The name for the placeholder.</param>
        /// <param name="code">The code for the placeholder.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="name"/> is <c>null</c> or empty.</item>
        /// <item><paramref name="code"/> is <c>null</c> or empty.</item>
        /// </list>
        /// </exception>
        public StandAloneFailureMechanism(string name, string code)
            : base(name, code)
        {
            Locations = new InputPlaceholder(RingtoetsCommonDataResources.FailureMechanism_Locations_DisplayName);
            BoundaryConditions = new InputPlaceholder(RingtoetsCommonDataResources.FailureMechanism_BoundaryConditions_DisplayName);
            AssessmentResult = new OutputPlaceholder(RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName);
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the locations relevant for evaluating this failure mechanism.
        /// </summary>
        public InputPlaceholder Locations { get; private set; }

        /// <summary>
        /// Gets the boundary conditions applying to this failure mechanism.
        /// </summary>
        public InputPlaceholder BoundaryConditions { get; private set; }

        /// <summary>
        /// Gets the calculation results for this failure mechanism.
        /// </summary>
        public OutputPlaceholder AssessmentResult { get; private set; }
    }
}
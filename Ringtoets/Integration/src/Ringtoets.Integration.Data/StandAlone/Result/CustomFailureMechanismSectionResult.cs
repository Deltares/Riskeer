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

using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.Data.StandAlone.Result
{
    /// <summary>
    /// Class which represents a result for a failure mechanism section with a custom outcome
    /// of a layer 2a assessment.
    /// </summary>
    public class CustomFailureMechanismSectionResult: FailureMechanismSectionResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="CustomFailureMechanismSectionResult"/>
        /// </summary>
        /// <param name="section">The section for which to add the result.</param>
        public CustomFailureMechanismSectionResult(FailureMechanismSection section) : base(section) { }

        /// <summary>
        /// Gets or sets the state of the assessment layer one.
        /// </summary>
        public bool AssessmentLayerOne { get; set; }

        /// <summary>
        /// Gets the value of assessment layer two a.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoA { get; set; }

        /// <summary>
        /// Gets or sets the value of assessment layer two b.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoB { get; set; }

        /// <summary>
        /// Gets or sets the value of assessment layer three.
        /// </summary>
        public RoundedDouble AssessmentLayerThree { get; set; }
    }
}
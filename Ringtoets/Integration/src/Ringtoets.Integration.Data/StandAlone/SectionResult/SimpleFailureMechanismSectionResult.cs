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

namespace Ringtoets.Integration.Data.StandAlone.SectionResult
{
    /// <summary>
    /// Class which represents results of different layers (1, 2a, 2b, 3) of a <see cref="FailureMechanismSection"/>.
    /// The result for a layer 2a assessment is any of three possible outcomes: 'successful', 
    /// 'failed' or 'not calculated'.
    /// </summary>
    public class SimpleFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="SimpleFailureMechanismSectionResult"/>
        /// </summary>
        /// <param name="section">The section for which to add the result.</param>
        public SimpleFailureMechanismSectionResult(FailureMechanismSection section) : base(section) { }

        /// <summary>
        /// Gets the value of assessment layer two a.
        /// </summary>
        public AssessmentLayerTwoAResult AssessmentLayerTwoA { get; set; }

        /// <summary>
        /// Gets or sets the value of assessment layer three.
        /// </summary>
        public RoundedDouble AssessmentLayerThree { get; set; }

        /// <summary>
        /// Gets or sets the state of the assessment layer one.
        /// </summary>
        public bool AssessmentLayerOne { get; set; }
    }
}
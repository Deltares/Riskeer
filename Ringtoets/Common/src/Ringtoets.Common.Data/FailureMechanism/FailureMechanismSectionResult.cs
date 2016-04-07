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
using Core.Common.Base;
using Core.Common.Base.Data;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class holds the information of the result of the <see cref="FailureMechanismSection"/>.
    /// </summary>
    public class FailureMechanismSectionResult : Observable
    {
        private readonly FailureMechanismSection section;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the result from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public FailureMechanismSectionResult(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }
            this.section = section;
        }

        /// <summary>
        /// Gets the encapsulated <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section
        {
            get
            {
                return section;
            }
        }

        /// <summary>
        /// Gets and sets the state of the assessment layer one.
        /// </summary>
        public bool AssessmentLayerOne { get; set; }

        /// <summary>
        /// Gets and sets the value of assessment layer two a.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoA { get; set; }

        /// <summary>
        /// Gets and sets the value of assessment layer two b.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoB { get; set; }

        /// <summary>
        /// Gets and sets the value of assessment layer three.
        /// </summary>
        public RoundedDouble AssessmentLayerThree { get; set; }
    }
}
﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for the comment of an <see cref="IAssessmentSection"/>.
    /// </summary>
    public class AssessmentSectionCommentContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCommentContext"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public AssessmentSectionCommentContext(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the wrapped assessment section
        /// </summary>
        public IAssessmentSection AssessmentSection { get; private set; }
    }
}

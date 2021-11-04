﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.Data.FailurePath;

namespace Riskeer.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for <see cref="SpecificFailurePath.Sections"/>.
    /// </summary>
    public class SpecificFailurePathSectionsContext : FailureMechanismSectionsContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificFailurePathSectionsContext"/> class.
        /// </summary>
        /// <param name="wrappedData">The <see cref="SpecificFailurePath"/> to wrap.</param>
        /// <param name="assessmentSection">The owning assessment section of <paramref name="wrappedData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public SpecificFailurePathSectionsContext(SpecificFailurePath wrappedData, IAssessmentSection assessmentSection)
            : base(wrappedData, assessmentSection) {}
    }
}
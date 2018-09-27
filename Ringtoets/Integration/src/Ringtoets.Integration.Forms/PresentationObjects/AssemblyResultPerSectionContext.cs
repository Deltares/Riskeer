// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for presenting the assembly results of an <see cref="AssessmentSection"/> on a per section basis.
    /// </summary>
    public class AssemblyResultPerSectionContext : ObservableWrappedObjectContextBase<AssessmentSection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultPerSectionContext"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to present the assembly results for on a per section basis.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public AssemblyResultPerSectionContext(AssessmentSection assessmentSection) : base(assessmentSection) {}
    }
}
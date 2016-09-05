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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This is a presentation object for <see cref="ObservableList{T}"/> for elements
    /// of type <see cref="ForeshoreProfile"/>.
    /// </summary>
    public class ForeshoreProfilesContext : ObservableWrappedObjectContextBase<ObservableList<ForeshoreProfile>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeshoreProfilesContext"/> class.
        /// </summary>
        /// <param name="foreshoreProfiles">The observable list of <see cref="ForeshoreProfile"/> objects.</param>
        /// <param name="parentAssessmentSection">The parent assessment section.</param>
        /// <exception cref="ArgumentNullException">When either <paramref name="foreshoreProfiles"/>
        /// or <paramref name="parentAssessmentSection"/> is null.</exception>
        public ForeshoreProfilesContext(ObservableList<ForeshoreProfile> foreshoreProfiles, IAssessmentSection parentAssessmentSection)
            : base(foreshoreProfiles)
        {
            if (parentAssessmentSection == null)
            {
                throw new ArgumentNullException("parentAssessmentSection");
            }
            ParentAssessmentSection = parentAssessmentSection;
        }

        /// <summary>
        /// Gets the parent assessment section.
        /// </summary>
        public IAssessmentSection ParentAssessmentSection { get; private set; }
    }
}
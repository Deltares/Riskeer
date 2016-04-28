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
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required grass cover erosion inwards input knowledge to configure and create
    /// related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    public class GrassCoverErosionInwardsContext<T> : WrappedObjectContextBase<T> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionInwardsContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="System.ArgumentNullException">When any input parameter is null.</exception>
        protected GrassCoverErosionInwardsContext(
            T wrappedData, IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (assessmentSection == null)
            {
                var message = String.Format(Resources.GrassCoverErosionInwardsContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.GrassCoverErosionInwardsContext_DataDescription_AssessmentSection);
                throw new ArgumentNullException("assessmentSection", message);
            }

            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the assessment section which the failure mechanism context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; private set; }
    }
}
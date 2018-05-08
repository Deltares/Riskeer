﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;

namespace Ringtoets.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveConditionsInputContext{T}"/> for properties panel in case the
    /// wave conditions input is of type <see cref="AssessmentSectionCategoryWaveConditionsInput"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of the wave conditions input context.</typeparam>
    public abstract class AssessmentSectionCategoryWaveConditionsInputContextProperties<TContext>
        : WaveConditionsInputContextProperties<
            TContext,
            AssessmentSectionCategoryWaveConditionsInput,
            AssessmentSectionCategoryType>
        where TContext : WaveConditionsInputContext<AssessmentSectionCategoryWaveConditionsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCategoryWaveConditionsInputContextProperties{TContext}"/>.
        /// </summary>
        /// <param name="context">The <see cref="WaveConditionsInputContext{TInput}"/> for which the properties are shown.</param>
        /// <param name="getAssessmentLevelFunc"><see cref="Func{TResult}"/> for obtaining the assessment level.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected AssessmentSectionCategoryWaveConditionsInputContextProperties(TContext context,
                                                                                Func<RoundedDouble> getAssessmentLevelFunc,
                                                                                IObservablePropertyChangeHandler propertyChangeHandler)
            : base(context, getAssessmentLevelFunc, propertyChangeHandler) {}

        protected override AssessmentSectionCategoryType GetCategoryType()
        {
            return data.WrappedData.CategoryType;
        }

        protected override void SetCategoryType(AssessmentSectionCategoryType categoryType)
        {
            data.WrappedData.CategoryType = categoryType;
        }
    }
}
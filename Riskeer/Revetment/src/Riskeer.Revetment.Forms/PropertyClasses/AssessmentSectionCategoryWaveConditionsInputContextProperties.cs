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
using Core.Common.Base.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.PresentationObjects;

namespace Riskeer.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveConditionsInputContext{T}"/> for properties panel in case the
    /// wave conditions input is of type <see cref="AssessmentSectionCategoryWaveConditionsInput"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of the wave conditions input context.</typeparam>
    /// <typeparam name="TWaveConditionsInput">The wave conditions input.</typeparam>
    /// <typeparam name="TCalculationType">The type of the calculation.</typeparam>
    public abstract class AssessmentSectionCategoryWaveConditionsInputContextProperties<TContext, TWaveConditionsInput, TCalculationType>
        : WaveConditionsInputContextProperties<
            TContext,
            TWaveConditionsInput,
            AssessmentSectionCategoryType,
            TCalculationType>
        where TContext : WaveConditionsInputContext<TWaveConditionsInput>
        where TWaveConditionsInput : AssessmentSectionCategoryWaveConditionsInput
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCategoryWaveConditionsInputContextProperties{TContext, TWaveConditionsInput, TCalculationType}"/>.
        /// </summary>
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
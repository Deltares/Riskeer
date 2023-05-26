﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

namespace Riskeer.Integration.Data
{
    /// <summary>
    /// Factory to create instances of <see cref="RiskeerProject"/>.
    /// </summary>
    public class RiskeerProjectFactory : IProjectFactory
    {
        private readonly Func<AssessmentSection> createAssessmentSectionFunc;

        /// <summary>
        /// Creates a new instance of <see cref="RiskeerProjectFactory"/>.
        /// </summary>
        /// <param name="createAssessmentSectionFunc">The <see cref="Func{TResult}"/>
        /// to create an <see cref="AssessmentSection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="createAssessmentSectionFunc"/>
        /// is <c>null</c>.</exception>
        public RiskeerProjectFactory(Func<AssessmentSection> createAssessmentSectionFunc)
        {
            if (createAssessmentSectionFunc == null)
            {
                throw new ArgumentNullException(nameof(createAssessmentSectionFunc));
            }

            this.createAssessmentSectionFunc = createAssessmentSectionFunc;
        }

        /// <inheritdoc />
        /// <returns>A <see cref="RiskeerProject"/>; or <c>null</c> when there
        /// is no <see cref="AssessmentSection"/>.</returns>
        public IProject CreateNewProject()
        {
            AssessmentSection assessmentSection;

            try
            {
                assessmentSection = createAssessmentSectionFunc();
            }
            catch (Exception e)
            {
                throw new ProjectFactoryException(e.Message, e);
            }

            return assessmentSection == null ? null : new RiskeerProject(assessmentSection);
        }
    }
}
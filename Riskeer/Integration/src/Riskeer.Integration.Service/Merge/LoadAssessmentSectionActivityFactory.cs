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
using Core.Common.Base.Service;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;

namespace Riskeer.Integration.Service.Merge
{
    /// <summary>
    /// This class defines factory methods that can be used to create instance of <see cref="Activity"/>
    /// to load <see cref="AssessmentSection"/>.
    /// </summary>
    public static class LoadAssessmentSectionActivityFactory
    {
        /// <summary>
        /// Creates an activity to load an <see cref="AssessmentSection"/> from a file.
        /// </summary>
        /// <param name="owner">The owner to set the <see cref="AssessmentSection"/> on.</param>
        /// <param name="loadAssessmentSectionService">The service for retrieving the
        /// <see cref="AssessmentSection"/> from a file.</param>
        /// <param name="filePath">The file path to retrieve the <see cref="AssessmentSection"/> from.</param>
        /// <returns>The <see cref="Activity"/> to load <see cref="AssessmentSection"/> from a file.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the arguments is <c>null</c>.</exception>
        public static Activity CreateLoadAssessmentSectionActivity(AssessmentSectionOwner owner,
                                                                   ILoadAssessmentSectionService loadAssessmentSectionService,
                                                                   string filePath)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (loadAssessmentSectionService == null)
            {
                throw new ArgumentNullException(nameof(loadAssessmentSectionService));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            return new LoadAssessmentSectionActivity(owner, loadAssessmentSectionService, filePath);
        }
    }
}
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
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Interface for handling adding <see cref="AssessmentSection"/> to a <see cref="TProject"/>.
    /// </summary>
    /// <typeparam name="TProject">The type of project.</typeparam>
    public interface IAssessmentSectionFromFileHandler<in TProject>
        where TProject : IProject
    {
        /// <summary>
        /// Displays available <see cref="AssessmentSection"/> objects to the user and asks to select one. 
        /// The selected <see cref="AssessmentSection"/>, if any, will be returned.
        /// </summary>
        /// <param name="project">The project to add the <see cref="AssessmentSection"/> to.</param>
        /// <returns>The selected <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/>
        /// is <c>null</c>.</exception>
        AssessmentSection GetAssessmentSectionFromFile(TProject project);
    }
}
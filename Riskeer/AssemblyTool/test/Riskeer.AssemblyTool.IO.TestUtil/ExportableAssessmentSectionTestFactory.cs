﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.AssemblyTool.IO.Model;

namespace Riskeer.AssemblyTool.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ExportableAssessmentSection"/> instances
    /// which can be used for testing.
    /// </summary>
    public static class ExportableAssessmentSectionTestFactory
    {
        /// <summary>
        /// Creates a default <see cref="ExportableAssessmentSection"/>.
        /// </summary>
        /// <returns>A default instance of <see cref="ExportableAssessmentSection"/>.</returns>
        public static ExportableAssessmentSection Create()
        {
            return Create("id");
        }

        /// <summary>
        /// Creates a default <see cref="ExportableAssessmentSection"/>.
        /// </summary>
        /// <returns>A default instance of <see cref="ExportableAssessmentSection"/>.</returns>
        public static ExportableAssessmentSection Create(string id)
        {
            return new ExportableAssessmentSection(
                "id2", string.Empty, Enumerable.Empty<Point2D>(),
                ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                Enumerable.Empty<ExportableFailureMechanism>(), Enumerable.Empty<ExportableCombinedSectionAssembly>());
        }
    }
}
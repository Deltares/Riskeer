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

using Core.Common.Base.Geometry;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ExportableFailureMechanismSection"/> instances
    /// which can be used for testing.
    /// </summary>
    public static class ExportableFailureMechanismSectionTestFactory
    {
        /// <summary>
        /// Creates a <see cref="ExportableFailureMechanismSection"/> with a given start and end distance.
        /// </summary>
        /// <param name="startDistance">The start distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <param name="endDistance">The end distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <returns>A <see cref="ExportableFailureMechanismSection"/>.</returns>
        public static ExportableFailureMechanismSection CreateExportableFailureMechanismSection(double startDistance = 1, double endDistance = 2)
        {
            return new ExportableFailureMechanismSection("id", new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, startDistance, endDistance);
        }

        /// <summary>
        /// Creates a <see cref="ExportableCombinedFailureMechanismSection"/> with a given start and end distance.
        /// </summary>
        /// <param name="startDistance">The start distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <param name="endDistance">The end distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <returns>A <see cref="ExportableCombinedFailureMechanismSection"/>.</returns>
        public static ExportableCombinedFailureMechanismSection CreateExportableCombinedFailureMechanismSection(double startDistance = 1, double endDistance = 3)
        {
            return new ExportableCombinedFailureMechanismSection("id", new[]
            {
                new Point2D(1, 1),
                new Point2D(3, 3)
            }, startDistance, endDistance, ExportableAssemblyMethod.BOI3A1);
        }
    }
}
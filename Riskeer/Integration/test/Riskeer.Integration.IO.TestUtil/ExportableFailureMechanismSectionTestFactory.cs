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

using Core.Common.Base.Geometry;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ExportableFailureMechanismSection"/> instances
    /// which can be used for testing.
    /// </summary>
    public static class ExportableFailureMechanismSectionTestFactory
    {
        /// <summary>
        /// Creates a default <see cref="ExportableFailureMechanismSection"/>.
        /// </summary>
        /// <returns>A default instance of <see cref="ExportableFailureMechanismSection"/>.</returns>
        public static ExportableFailureMechanismSection CreateExportableFailureMechanismSection()
        {
            return new ExportableFailureMechanismSection(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, 1, 2);
        }

        /// <summary>
        /// Creates a default <see cref="ExportableCombinedFailureMechanismSection"/>.
        /// </summary>
        /// <returns>A default instance of <see cref="ExportableCombinedFailureMechanismSection"/>.</returns>
        public static ExportableCombinedFailureMechanismSection CreateExportableCombinedFailureMechanismSection()
        {
            return new ExportableCombinedFailureMechanismSection(new[]
            {
                new Point2D(1, 1),
                new Point2D(3, 3)
            }, 1, 3, ExportableAssemblyMethod.WBI3A1);
        }
    }
}
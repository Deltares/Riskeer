// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.Common.Data.Contribution;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="FailureMechanismContribution"/> instances that can be used
    /// for testing.
    /// </summary>
    public static class FailureMechanismContributionTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContribution"/> without failure mechanisms.
        /// </summary>
        /// <returns>The created <see cref="FailureMechanismContribution"/>.</returns>
        public static FailureMechanismContribution CreateFailureMechanismContribution()
        {
            return new FailureMechanismContribution(1.0 / 30000,
                                                    1.0 / 30000);
        }
    }
}
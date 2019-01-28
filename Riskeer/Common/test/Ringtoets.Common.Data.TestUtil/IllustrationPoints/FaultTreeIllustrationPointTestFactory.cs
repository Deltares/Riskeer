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

using System.Linq;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.IllustrationPoints
{
    /// <summary>
    /// Factory to create simple <see cref="FaultTreeIllustrationPoint"/> instances that can be used for testing.
    /// </summary>
    public static class FaultTreeIllustrationPointTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPoint"/> with arbitrary value
        /// for <see cref="CombinationType"/>.
        /// </summary>
        /// <param name="beta">The beta value of the illustration point.</param>
        /// <returns>The created <see cref="FaultTreeIllustrationPoint"/>.</returns>
        public static FaultTreeIllustrationPoint CreateTestFaultTreeIllustrationPoint(double beta = 1.23)
        {
            return CreateTestFaultTreeIllustrationPointCombinationTypeAnd(beta);
        }

        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPoint"/> with <see cref="CombinationType"/> 
        /// set to <see cref="CombinationType.And"/>.
        /// </summary>
        /// <param name="beta">The beta value of the illustration point.</param>
        /// <returns>The created <see cref="FaultTreeIllustrationPoint"/>.</returns>
        public static FaultTreeIllustrationPoint CreateTestFaultTreeIllustrationPointCombinationTypeAnd(double beta)
        {
            return new FaultTreeIllustrationPoint("Illustration point",
                                                  beta,
                                                  Enumerable.Empty<Stochast>(),
                                                  CombinationType.And);
        }

        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPoint"/> with <see cref="CombinationType"/> 
        /// set to <see cref="CombinationType.And"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point.</param>
        /// <returns>The created <see cref="FaultTreeIllustrationPoint"/>.</returns>
        public static FaultTreeIllustrationPoint CreateTestFaultTreeIllustrationPointCombinationTypeAnd(string name)
        {
            return new FaultTreeIllustrationPoint(name,
                                                  1.23,
                                                  Enumerable.Empty<Stochast>(),
                                                  CombinationType.And);
        }

        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPoint"/> with <see cref="CombinationType"/> 
        /// set to <see cref="CombinationType.Or"/>.
        /// </summary>
        /// <param name="beta">The beta value of the illustration point.</param>
        /// <returns>The created <see cref="FaultTreeIllustrationPoint"/>.</returns>
        public static FaultTreeIllustrationPoint CreateTestFaultTreeIllustrationPointCombinationTypeOr(double beta)
        {
            return new FaultTreeIllustrationPoint("Illustration point",
                                                  beta,
                                                  Enumerable.Empty<Stochast>(),
                                                  CombinationType.Or);
        }
    }
}
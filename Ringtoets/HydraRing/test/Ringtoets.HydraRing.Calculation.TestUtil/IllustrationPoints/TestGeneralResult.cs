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

using System.Collections.Generic;
using System.Linq;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints
{
    /// <summary>
    /// A simple general result which can be used for testing.
    /// </summary>
    public class TestGeneralResult : GeneralResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestGeneralResult"/>.
        /// </summary>
        public TestGeneralResult()
            : base(0,
                   new TestWindDirection(),
                   Enumerable.Empty<Stochast>(),
                   new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestGeneralResult"/> with only fault tree
        /// illustration points.
        /// </summary>
        /// <returns>A <see cref="TestGeneralResult"/> with fault tree illustration points.</returns>
        public static TestGeneralResult CreateGeneralResultWithFaultTreeIllustrationPoints()
        {
            var generalResult = new TestGeneralResult();

            generalResult.IllustrationPoints.Add(new WindDirectionClosingSituation(new TestWindDirection(), "closing situation"),
                                                 new IllustrationPointTreeNode(new TestFaultTreeIllustrationPoint()));

            return generalResult;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TestGeneralResult"/> with only sub mechanism
        /// illustration points.
        /// </summary>
        /// <returns>A <see cref="TestGeneralResult"/> with sub mechanism illustration points.</returns>
        public static TestGeneralResult CreateGeneralResultWithSubMechanismIllustrationPoints()
        {
            var generalResult = new TestGeneralResult();
            generalResult.IllustrationPoints.Add(new WindDirectionClosingSituation(new TestWindDirection(), "closing situation"),
                                                 new IllustrationPointTreeNode(new TestSubMechanismIllustrationPoint()));

            return generalResult;
        }
    }
}
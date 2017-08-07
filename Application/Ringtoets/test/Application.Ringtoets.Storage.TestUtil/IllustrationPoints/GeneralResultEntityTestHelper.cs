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

using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.TestUtil.IllustrationPoints
{
    /// <summary>
    /// Class to assert the properties of general result entities.
    /// </summary>
    public static class GeneralResultEntityTestHelper
    {
        /// <summary>
        /// Determines if the properties of the <paramref name="generalResultEntity"/> match 
        /// with the <paramref name="generalResult"/>.
        /// </summary>
        /// <param name="generalResult">The <see cref="GeneralResult{T}"/> to assert against.</param>
        /// <param name="generalResultEntity">The <see cref="GeneralResultFaultTreeIllustrationPointEntity"/>
        /// to assert against to.</param>
        /// <remarks>This only asserts the properties of the <see cref="GeneralResultFaultTreeIllustrationPointEntity"/>
        /// that are directly associated with it, but not the values of the items it is composed of.</remarks>
        public static void AssertGeneralResultEntity(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult,
                                                     GeneralResultFaultTreeIllustrationPointEntity generalResultEntity)
        {
            if (generalResult == null)
            {
                Assert.IsNull(generalResultEntity);
                return;
            }

            Assert.IsNotNull(generalResultEntity);

            WindDirection governingWindDirection = generalResult.GoverningWindDirection;
            Assert.AreEqual(governingWindDirection.Name, generalResultEntity.GoverningWindDirectionName);
            Assert.AreEqual(governingWindDirection.Angle, generalResultEntity.GoverningWindDirectionAngle,
                            governingWindDirection.Angle.GetAccuracy());

            Assert.AreEqual(generalResult.Stochasts.Count(), generalResultEntity.StochastEntities.Count);
            Assert.AreEqual(generalResult.TopLevelIllustrationPoints.Count(), generalResultEntity.TopLevelFaultTreeIllustrationPointEntities.Count);
        }
    }
}
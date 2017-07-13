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

using System;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils.Extensions;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Application.Ringtoets.Storage.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="FaultTreeIllustrationPoint"/> 
    /// related to creating an instance of <see cref="FaultTreeIllustrationPointEntity"/>.
    /// </summary>
    internal static class FaultTreeIllustrationPointCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FaultTreeIllustrationPointEntity"/> based on the information 
        /// of <paramref name="faultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="faultTreeIllustrationPoint">The fault tree illustration point to create 
        /// a database entity for.</param>
        /// <returns>A new <see cref="FaultTreeIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="faultTreeIllustrationPoint"/> 
        /// is <c>null</c>.</exception>
        public static FaultTreeIllustrationPointEntity Create(
            this FaultTreeIllustrationPoint faultTreeIllustrationPoint)
        {
            if (faultTreeIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(faultTreeIllustrationPoint));
            }

            var entity = new FaultTreeIllustrationPointEntity
            {
                Beta = faultTreeIllustrationPoint.Beta,
                CombinationType = Convert.ToByte(faultTreeIllustrationPoint.CombinationType),
                Name = faultTreeIllustrationPoint.Name.DeepClone()
            };

            return entity;
        }
    }
}
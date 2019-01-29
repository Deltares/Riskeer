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

using System;
using System.Linq;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingIIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IIllustrationPoint;
using HydraRingFaultTreeIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraRingIllustrationPointTreeNode = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;

namespace Riskeer.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraRingIllustrationPointTreeNode"/> data into 
    /// <see cref="IllustrationPointNode"/> data.
    /// </summary>
    public static class IllustrationPointNodeConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointNode"/> based on the 
        /// information of <paramref name="hydraRingIllustrationPointTreeNode"/>.
        /// </summary>
        /// <param name="hydraRingIllustrationPointTreeNode">The <see cref="HydraRingIllustrationPointTreeNode"/> 
        /// to base the <see cref="IllustrationPointNode"/> to create on.</param>
        /// <returns>The created <see cref="IllustrationPointNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="hydraRingIllustrationPointTreeNode"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hydraRingIllustrationPointTreeNode"/>
        /// does not contain 0 or 2 children.</exception>
        /// <exception cref="IllustrationPointConversionException">Thrown when <paramref name="hydraRingIllustrationPointTreeNode"/>
        /// cannot be converted to a <see cref="IllustrationPointNode"/></exception>
        public static IllustrationPointNode Convert(HydraRingIllustrationPointTreeNode hydraRingIllustrationPointTreeNode)
        {
            if (hydraRingIllustrationPointTreeNode == null)
            {
                throw new ArgumentNullException(nameof(hydraRingIllustrationPointTreeNode));
            }

            IllustrationPointBase data;

            try
            {
                data = ConvertIllustrationPointTreeNodeData(hydraRingIllustrationPointTreeNode.Data);
            }
            catch (NotSupportedException e)
            {
                string errorMessage = "An illustration point containing a Hydra ring data type of " +
                                      $"{hydraRingIllustrationPointTreeNode.Data.GetType()} is not supported.";
                throw new IllustrationPointConversionException(errorMessage, e);
            }

            var illustrationPointNode = new IllustrationPointNode(data);
            illustrationPointNode.SetChildren(hydraRingIllustrationPointTreeNode.Children.Select(Convert).ToArray());

            return illustrationPointNode;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="IllustrationPointBase"/> based on the
        /// <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="HydraRingIIllustrationPoint"/> to base the 
        /// <see cref="IllustrationPointBase"/> to create on.</param>
        /// <returns>A <see cref="IllustrationPointBase"/>.</returns>
        /// <exception cref="IllustrationPointConversionException">Thrown when <paramref name="data"/>
        /// could not be converted.</exception>
        /// <exception cref="NotSupportedException">Thrown when no suitable conversion for <paramref name="data"/>
        /// was found.</exception>
        private static IllustrationPointBase ConvertIllustrationPointTreeNodeData(HydraRingIIllustrationPoint data)
        {
            var faultTreeIllustrationPoint = data as HydraRingFaultTreeIllustrationPoint;
            if (faultTreeIllustrationPoint != null)
            {
                return FaultTreeIllustrationPointConverter.Convert(faultTreeIllustrationPoint);
            }

            var subMechanismIllustrationPoint = data as HydraRingSubMechanismIllustrationPoint;
            if (subMechanismIllustrationPoint != null)
            {
                return SubMechanismIllustrationPointConverter.Convert(subMechanismIllustrationPoint);
            }

            throw new NotSupportedException($"Cannot convert {data.GetType()}.");
        }
    }
}
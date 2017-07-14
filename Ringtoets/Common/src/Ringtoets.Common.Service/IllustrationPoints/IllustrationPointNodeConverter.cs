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
using System.Linq;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using HydraRingFaultTreeIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraRingIllustrationPointTreeNode = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;

namespace Ringtoets.Common.Service.IllustrationPoints
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
        public static IllustrationPointNode Create(HydraRingIllustrationPointTreeNode hydraRingIllustrationPointTreeNode)
        {
            if (hydraRingIllustrationPointTreeNode == null)
            {
                throw new ArgumentNullException(nameof(hydraRingIllustrationPointTreeNode));
            }

            IllustrationPointBase data = CreateIllustrationPointTreeNodeData(hydraRingIllustrationPointTreeNode.Data);

            var illustrationPointNode = new IllustrationPointNode(data);
            illustrationPointNode.SetChildren(hydraRingIllustrationPointTreeNode.Children.Select(Create).ToArray());

            return illustrationPointNode;
        }

        private static IllustrationPointBase CreateIllustrationPointTreeNodeData(IIllustrationPoint data)
        {
            var faultTreeIllustrationPoint = data as HydraRingFaultTreeIllustrationPoint;
            var subMechanismIllustrationPoint = data as HydraRingSubMechanismIllustrationPoint;
            return faultTreeIllustrationPoint != null
                       ? (IllustrationPointBase) FaultTreeIllustrationPointConverter.Create(faultTreeIllustrationPoint)
                       : (subMechanismIllustrationPoint != null
                              ? SubMechanismIllustrationPointConverter.Create(subMechanismIllustrationPoint)
                              : null);
        }
    }
}
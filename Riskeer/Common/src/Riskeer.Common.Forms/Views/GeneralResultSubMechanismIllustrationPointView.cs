// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class is a view for presenting <see cref="TopLevelSubMechanismIllustrationPoint"/> objects
    /// (as part of the <see cref="GeneralResult{T}"/> of a <see cref="ICalculation"/>).
    /// </summary>
    public class GeneralResultSubMechanismIllustrationPointView : GeneralResultIllustrationPointView<TopLevelSubMechanismIllustrationPoint>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralResultSubMechanismIllustrationPointView"/>.
        /// </summary>
        /// <param name="calculation">The calculation to show the illustration points for.</param>
        /// <param name="getGeneralResultFunc">A <see cref="Func{TResult}"/> for obtaining the illustration point
        /// data (<see cref="GeneralResult{T}"/> with <see cref="TopLevelSubMechanismIllustrationPoint"/> objects)
        /// that must be presented.</param>
        /// <exception cref="NullReferenceException">Thrown when any parameter is <c>null</c>.</exception>
        public GeneralResultSubMechanismIllustrationPointView(ICalculation calculation,
                                                              Func<GeneralResult<TopLevelSubMechanismIllustrationPoint>> getGeneralResultFunc)
            : base(calculation, getGeneralResultFunc) {}

        protected override IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
        {
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = GetGeneralResultFunc();

            if (generalResult == null)
            {
                return Enumerable.Empty<IllustrationPointControlItem>();
            }

            return generalResult.TopLevelIllustrationPoints.Select(topLevelSubMechanismIllustrationPoint =>
            {
                SubMechanismIllustrationPoint illustrationPoint = topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint;

                return new IllustrationPointControlItem(topLevelSubMechanismIllustrationPoint,
                                                        topLevelSubMechanismIllustrationPoint.WindDirection.Name,
                                                        topLevelSubMechanismIllustrationPoint.ClosingSituation,
                                                        illustrationPoint.Stochasts,
                                                        illustrationPoint.Beta);
            }).ToArray();
        }

        protected override void UpdateSpecificIllustrationPointsControl() {}

        protected override object GetSelectedTopLevelIllustrationPoint(IllustrationPointControlItem selection)
        {
            return new SelectedTopLevelSubMechanismIllustrationPoint(
                (TopLevelSubMechanismIllustrationPoint) selection.Source,
                GetIllustrationPointControlItems().Select(ipci => ipci.ClosingSituation));
        }
    }
}
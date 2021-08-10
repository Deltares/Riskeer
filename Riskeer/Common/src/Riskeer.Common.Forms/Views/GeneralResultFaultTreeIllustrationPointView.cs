// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class is a view for presenting <see cref="TopLevelFaultTreeIllustrationPoint"/> objects
    /// (as part of the <see cref="GeneralResult{T}"/> of a <see cref="ICalculation"/>).
    /// </summary>
    public class GeneralResultFaultTreeIllustrationPointView : GeneralResultIllustrationPointView<TopLevelFaultTreeIllustrationPoint>
    {
        private IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl;

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResultFaultTreeIllustrationPointView"/>.
        /// </summary>
        /// <param name="calculation">The calculation to show the illustration points for.</param>
        /// <param name="getGeneralResultFunc">A <see cref="Func{TResult}"/> for obtaining the illustration point
        /// data (<see cref="GeneralResult{T}"/> with <see cref="TopLevelFaultTreeIllustrationPoint"/> objects)
        /// that must be presented.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the top level fault tree illustration 
        /// contains an illustration point that is not of type <see cref="FaultTreeIllustrationPoint"/> 
        /// or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        public GeneralResultFaultTreeIllustrationPointView(ICalculation calculation,
                                                           Func<GeneralResult<TopLevelFaultTreeIllustrationPoint>> getGeneralResultFunc)
            : base(calculation, getGeneralResultFunc)
        {
            AddIllustrationPointsFaultTreeControl();
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Thrown when the top level fault tree illustration 
        /// contains an illustration point that is not of type <see cref="FaultTreeIllustrationPoint"/> 
        /// or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        protected override IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
        {
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultFunc();

            if (generalResult == null)
            {
                return Enumerable.Empty<IllustrationPointControlItem>();
            }

            return generalResult.TopLevelIllustrationPoints.Select(topLevelFaultTreeIllustrationPoint =>
            {
                IllustrationPointBase illustrationPoint = topLevelFaultTreeIllustrationPoint.FaultTreeNodeRoot.Data;

                return new IllustrationPointControlItem(topLevelFaultTreeIllustrationPoint,
                                                        topLevelFaultTreeIllustrationPoint.WindDirection.Name,
                                                        topLevelFaultTreeIllustrationPoint.ClosingSituation,
                                                        GetStochasts(illustrationPoint),
                                                        illustrationPoint.Beta);
            }).ToArray();
        }

        protected override void UpdateSpecificIllustrationPointsControl()
        {
            illustrationPointsFaultTreeControl.Data = (TopLevelFaultTreeIllustrationPoint) (IllustrationPointsControl.Selection as IllustrationPointControlItem)?.Source;
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Thrown when the top level fault tree illustration 
        /// contains an illustration point that is not of type <see cref="FaultTreeIllustrationPoint"/> 
        /// or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        protected override object GetSelectedTopLevelIllustrationPoint(IllustrationPointControlItem selection)
        {
            return new SelectedTopLevelFaultTreeIllustrationPoint(
                (TopLevelFaultTreeIllustrationPoint) selection.Source,
                GetIllustrationPointControlItems().Select(ipci => ipci.ClosingSituation));
        }

        private void AddIllustrationPointsFaultTreeControl()
        {
            illustrationPointsFaultTreeControl = new IllustrationPointsFaultTreeControl
            {
                Dock = DockStyle.Fill
            };
            illustrationPointsFaultTreeControl.SelectionChanged += IllustrationPointsFaultTreeControlOnSelectionChanged;

            SplitContainer.Panel2.Controls.Add(illustrationPointsFaultTreeControl);
        }

        /// <summary>
        /// Returns the stochasts of the <paramref name="illustrationPoint"/>.
        /// </summary>
        /// <param name="illustrationPoint">The illustration point to get the stochasts from.</param>
        /// <returns>The stochasts.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="illustrationPoint"/> 
        /// is not of type <see cref="FaultTreeIllustrationPoint"/> or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        private static IEnumerable<Stochast> GetStochasts(IllustrationPointBase illustrationPoint)
        {
            switch (illustrationPoint)
            {
                case FaultTreeIllustrationPoint faultTreeIllustrationPoint:
                    return faultTreeIllustrationPoint.Stochasts;
                case SubMechanismIllustrationPoint subMechanismIllustrationPoint:
                    return subMechanismIllustrationPoint.Stochasts;
                default:
                    throw new NotSupportedException(
                        $"IllustrationPointNode of type {illustrationPoint.GetType().Name} is not supported. " +
                        $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}");
            }
        }

        private void IllustrationPointsFaultTreeControlOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            var selection = illustrationPointsFaultTreeControl.Selection as IllustrationPointNode;
            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint = illustrationPointsFaultTreeControl.Data;
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultFunc();

            if (selection == null || topLevelFaultTreeIllustrationPoint == null || generalResult == null)
            {
                return;
            }

            string closingSituation = generalResult.TopLevelIllustrationPoints.HasMultipleUniqueValues(p => p.ClosingSituation)
                                          ? topLevelFaultTreeIllustrationPoint.ClosingSituation
                                          : string.Empty;

            switch (selection.Data)
            {
                case FaultTreeIllustrationPoint faultTreeIllustrationPoint:
                    Selection = new IllustrationPointContext<FaultTreeIllustrationPoint>(
                        faultTreeIllustrationPoint,
                        selection,
                        topLevelFaultTreeIllustrationPoint.WindDirection.Name,
                        closingSituation);
                    break;
                case SubMechanismIllustrationPoint subMechanismIllustrationPoint:
                    Selection = new IllustrationPointContext<SubMechanismIllustrationPoint>(
                        subMechanismIllustrationPoint,
                        selection,
                        topLevelFaultTreeIllustrationPoint.WindDirection.Name,
                        closingSituation);
                    break;
            }

            OnSelectionChanged();
        }
    }
}
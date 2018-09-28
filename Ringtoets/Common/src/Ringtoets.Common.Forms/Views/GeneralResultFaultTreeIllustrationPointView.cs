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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class is a view for presenting <see cref="TopLevelFaultTreeIllustrationPoint"/> objects
    /// (as part of the <see cref="GeneralResult{T}"/> of a <see cref="ICalculation"/>).
    /// </summary>
    public partial class GeneralResultFaultTreeIllustrationPointView : UserControl, IView, ISelectionProvider
    {
        private readonly Observer calculationObserver;
        private readonly Func<GeneralResult<TopLevelFaultTreeIllustrationPoint>> getGeneralResultFunc;

        private ICalculation data;
        private bool suspendIllustrationPointsControlEvents;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResultFaultTreeIllustrationPointView"/>.
        /// </summary>
        /// <param name="getGeneralResultFunc">A <see cref="Func{TResult}"/> for obtaining the illustration point
        /// data (<see cref="GeneralResult{T}"/> with <see cref="TopLevelFaultTreeIllustrationPoint"/> objects)
        /// that must be presented.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="getGeneralResultFunc"/> is <c>null</c>.</exception>
        public GeneralResultFaultTreeIllustrationPointView(Func<GeneralResult<TopLevelFaultTreeIllustrationPoint>> getGeneralResultFunc)
        {
            InitializeComponent();

            if (getGeneralResultFunc == null)
            {
                throw new ArgumentNullException(nameof(getGeneralResultFunc));
            }

            this.getGeneralResultFunc = getGeneralResultFunc;

            calculationObserver = new Observer(UpdateControls);

            illustrationPointsControl.SelectionChanged += IllustrationPointsControlOnSelectionChanged;
            illustrationPointsFaultTreeControl.SelectionChanged += IllustrationPointsFaultTreeControlOnSelectionChanged;
        }

        public object Selection { get; private set; }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as ICalculation;

                calculationObserver.Observable = data;

                UpdateControls();
            }
        }

        protected override void Dispose(bool disposing)
        {
            calculationObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the controls.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the top level fault tree illustration 
        /// contains an illustration point that is not of type <see cref="FaultTreeIllustrationPoint"/> 
        /// or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        private void UpdateControls()
        {
            suspendIllustrationPointsControlEvents = true;
            UpdateIllustrationPointsControl();
            suspendIllustrationPointsControlEvents = false;

            UpdateIllustrationPointsFaultTreeControl();
            ProvideIllustrationPointSelection();
        }

        private void IllustrationPointsFaultTreeControlOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            var selection = illustrationPointsFaultTreeControl.Selection as IllustrationPointNode;
            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint = illustrationPointsFaultTreeControl.Data;
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = getGeneralResultFunc();

            if (selection == null || topLevelFaultTreeIllustrationPoint == null || generalResult == null)
            {
                return;
            }

            string closingSituation = generalResult.TopLevelIllustrationPoints.HasMultipleUniqueValues(p => p.ClosingSituation)
                                          ? topLevelFaultTreeIllustrationPoint.ClosingSituation
                                          : string.Empty;

            var faultTreeIllustrationPoint = selection.Data as FaultTreeIllustrationPoint;
            if (faultTreeIllustrationPoint != null)
            {
                Selection = new IllustrationPointContext<FaultTreeIllustrationPoint>(faultTreeIllustrationPoint,
                                                                                     selection,
                                                                                     topLevelFaultTreeIllustrationPoint.WindDirection.Name,
                                                                                     closingSituation);
            }

            var subMechanismIllustrationPoint = selection.Data as SubMechanismIllustrationPoint;
            if (subMechanismIllustrationPoint != null)
            {
                Selection = new IllustrationPointContext<SubMechanismIllustrationPoint>(subMechanismIllustrationPoint,
                                                                                        selection,
                                                                                        topLevelFaultTreeIllustrationPoint.WindDirection.Name,
                                                                                        closingSituation);
            }

            OnSelectionChanged();
        }

        private void UpdateIllustrationPointsControl()
        {
            illustrationPointsControl.Data = GetIllustrationPointControlItems();
        }

        private IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
        {
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = getGeneralResultFunc();

            if (data == null || generalResult == null)
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

        /// <summary>
        /// Returns the stochasts of the <paramref name="illustrationPoint"/>.
        /// </summary>
        /// <param name="illustrationPoint">The illustration point to get the stochasts from.</param>
        /// <returns>The stochasts.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="illustrationPoint"/> 
        /// is not of type <see cref="FaultTreeIllustrationPoint"/> or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        private static IEnumerable<Stochast> GetStochasts(IllustrationPointBase illustrationPoint)
        {
            var faultTreeIllustrationPoint = illustrationPoint as FaultTreeIllustrationPoint;
            if (faultTreeIllustrationPoint != null)
            {
                return faultTreeIllustrationPoint.Stochasts;
            }

            var subMechanismIllustrationPoint = illustrationPoint as SubMechanismIllustrationPoint;
            if (subMechanismIllustrationPoint != null)
            {
                return subMechanismIllustrationPoint.Stochasts;
            }

            throw new NotSupportedException($"IllustrationPointNode of type {illustrationPoint.GetType().Name} is not supported. " +
                                            $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}");
        }

        private void UpdateIllustrationPointsFaultTreeControl()
        {
            illustrationPointsFaultTreeControl.Data = (TopLevelFaultTreeIllustrationPoint) (illustrationPointsControl.Selection as IllustrationPointControlItem)?.Source;
        }

        private void IllustrationPointsControlOnSelectionChanged(object sender, EventArgs e)
        {
            if (suspendIllustrationPointsControlEvents)
            {
                return;
            }

            UpdateIllustrationPointsFaultTreeControl();

            ProvideIllustrationPointSelection();
        }

        /// <summary>
        /// Sets the <see cref="Selection"/> based on the selection of the <see cref="IllustrationPointsControl"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the top level fault tree illustration 
        /// contains an illustration point that is not of type <see cref="FaultTreeIllustrationPoint"/> 
        /// or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        private void ProvideIllustrationPointSelection()
        {
            var selection = illustrationPointsControl.Selection as IllustrationPointControlItem;
            Selection = selection != null
                            ? new SelectedTopLevelFaultTreeIllustrationPoint((TopLevelFaultTreeIllustrationPoint) selection.Source,
                                                                             GetIllustrationPointControlItems().Select(ipci => ipci.ClosingSituation))
                            : null;
            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }
    }
}
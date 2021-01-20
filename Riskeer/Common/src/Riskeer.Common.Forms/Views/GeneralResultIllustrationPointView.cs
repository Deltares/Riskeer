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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.IllustrationPoints;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class is a view for presenting <typeparamref name="TTopLevelIllustrationPoint"/> objects
    /// (as part of the <see cref="GeneralResult{T}"/> of a <see cref="ICalculation"/>).
    /// </summary>
    /// <typeparam name="TTopLevelIllustrationPoint">The type of the top level illustration point.</typeparam>
    public abstract partial class GeneralResultIllustrationPointView<TTopLevelIllustrationPoint> : UserControl, IView, ISelectionProvider
        where TTopLevelIllustrationPoint : TopLevelIllustrationPointBase
    {
        private readonly Observer calculationObserver;
        private ICalculation calculation;

        private bool suspendIllustrationPointsControlEvents;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResultIllustrationPointView{T}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to show the illustration points for.</param>
        /// <param name="getGeneralResultFunc">A <see cref="Func{TResult}"/> for obtaining the
        /// illustration point that must be presented.</param>
        /// <exception cref="NullReferenceException">Thrown when any parameter is <c>null</c>.</exception>
        protected GeneralResultIllustrationPointView(ICalculation calculation, Func<GeneralResult<TTopLevelIllustrationPoint>> getGeneralResultFunc)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (getGeneralResultFunc == null)
            {
                throw new ArgumentNullException(nameof(getGeneralResultFunc));
            }

            InitializeComponent();

            this.calculation = calculation;
            GetGeneralResultFunc = getGeneralResultFunc;

            calculationObserver = new Observer(UpdateControls)
            {
                Observable = calculation
            };

            IllustrationPointsControl.SelectionChanged += IllustrationPointsControlOnSelectionChanged;
            Name = "GeneralResultIllustrationPointView";
        }

        public object Selection { get; protected set; }

        public object Data
        {
            get => calculation;
            set => calculation = value as ICalculation;
        }

        /// <summary>
        /// Gets the <see cref="Func{TResult}"/> for obtaining the <see cref="GeneralResult{T}"/>. 
        /// </summary>
        protected Func<GeneralResult<TTopLevelIllustrationPoint>> GetGeneralResultFunc { get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateControls();
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
        /// Gets a collection of <see cref="IllustrationPointControlItem"/>.
        /// </summary>
        /// <returns>A collection of <see cref="IllustrationPointControlItem"/>.</returns>
        protected abstract IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems();

        /// <summary>
        /// Updates specific illustration point controls.
        /// </summary>
        protected abstract void UpdateSpecificIllustrationPointsControl();

        /// <summary>
        /// Gets the selected top level illustration point.
        /// </summary>
        /// <param name="selection">The selection <see cref="IllustrationPointControlItem"/>
        /// to get the selected top level illustration point from.</param>
        /// <returns>The selected top level illustration point.</returns>
        protected abstract object GetSelectedTopLevelIllustrationPoint(IllustrationPointControlItem selection);

        protected void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void UpdateControls()
        {
            suspendIllustrationPointsControlEvents = true;
            UpdateIllustrationPointsControl();
            suspendIllustrationPointsControlEvents = false;

            UpdateSpecificIllustrationPointsControl();
            ProvideIllustrationPointSelection();
        }

        private void UpdateIllustrationPointsControl()
        {
            IllustrationPointsControl.Data = GetIllustrationPointControlItems();
        }

        private void IllustrationPointsControlOnSelectionChanged(object sender, EventArgs e)
        {
            if (suspendIllustrationPointsControlEvents)
            {
                return;
            }

            UpdateSpecificIllustrationPointsControl();

            ProvideIllustrationPointSelection();
        }

        private void ProvideIllustrationPointSelection()
        {
            Selection = IllustrationPointsControl.Selection is IllustrationPointControlItem selection
                            ? GetSelectedTopLevelIllustrationPoint(selection)
                            : null;
            OnSelectionChanged();
        }
    }
}
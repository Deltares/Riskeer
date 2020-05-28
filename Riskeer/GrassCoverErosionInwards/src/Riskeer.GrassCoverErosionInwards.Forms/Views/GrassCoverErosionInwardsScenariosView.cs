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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Riskeer.Common.Data.Calculation;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Util;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// View for configuring scenarios for the grass cover erosion inwards failure mechanism.
    /// Shows a grid view where for each failure mechanism section, a calculation within the section
    /// can be selected.
    /// </summary>
    public partial class GrassCoverErosionInwardsScenariosView : UserControl, IView
    {
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly Observer failureMechanismObserver;
        private readonly GrassCoverErosionInwardsFailureMechanism failureMechanism;
        private CalculationGroup data;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsScenariosView"/>.
        /// </summary>
        /// <param name="calculationGroup">The data to show in this view.</param>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/>
        /// the <paramref name="calculationGroup"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public GrassCoverErosionInwardsScenariosView(CalculationGroup calculationGroup, GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            data = calculationGroup;
            this.failureMechanism = failureMechanism;

            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateDataGridViewDataSource)
            {
                Observable = failureMechanism
            };

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateDataGridViewDataSource, cg => cg.Children.Concat<object>(cg.Children
                                                                                 .OfType<GrassCoverErosionInwardsCalculationScenario>()
                                                                                 .Select(c => c.InputParameters)))
            {
                Observable = calculationGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(UpdateDataGridViewDataSource, c => c.Children)
            {
                Observable = calculationGroup
            };

            UpdateDataGridViewDataSource();
        }

        public object Data
        {
            get => data;
            set => data = value as CalculationGroup;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Necessary to correctly load the content of the dropdown lists of the comboboxes.
        /// </remarks>
        protected override void OnLoad(EventArgs e)
        {
            UpdateDataGridViewDataSource();
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateDataGridViewDataSource()
        {
            scenarioSelectionControl.EndEdit();

            ICalculation[] calculations = data.GetCalculations().ToArray();

            IDictionary<string, List<ICalculation>> calculationsPerSegment =
                GrassCoverErosionInwardsHelper.CollectCalculationsPerSection(failureMechanism.Sections, calculations.Cast<GrassCoverErosionInwardsCalculationScenario>());

            List<GrassCoverErosionInwardsScenarioRow> scenarioRows =
                failureMechanism.SectionResults.Select(sectionResult => new GrassCoverErosionInwardsScenarioRow(sectionResult)).ToList();

            scenarioSelectionControl.UpdateDataGridViewDataSource(calculations, scenarioRows, calculationsPerSegment);
        }
    }
}
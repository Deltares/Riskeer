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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Util;
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.Forms.Views
{
    /// <summary>
    /// View for configuring scenarios for the height structures failure mechanism.
    /// Shows a grid view where for each failure mechanism section, a calculation within the section
    /// can be selected.
    /// </summary>
    public partial class HeightStructuresScenariosView : UserControl, IView
    {
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly Observer failureMechanismObserver;
        private HeightStructuresFailureMechanism failureMechanism;
        private CalculationGroup data;

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresScenariosView"/>.
        /// </summary>
        public HeightStructuresScenariosView()
        {
            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateDataGridViewDataSource);

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateDataGridViewDataSource, cg => cg.Children.Concat<object>(cg.Children
                                                                                 .OfType<StructuresCalculation<HeightStructuresInput>>()
                                                                                 .Select(c => c.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(UpdateDataGridViewDataSource, c => c.Children);
        }

        /// <summary>
        /// Gets or sets the failure mechanism.
        /// </summary>
        public HeightStructuresFailureMechanism FailureMechanism
        {
            get
            {
                return failureMechanism;
            }
            set
            {
                failureMechanism = value;
                failureMechanismObserver.Observable = failureMechanism;
                UpdateDataGridViewDataSource();
            }
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as CalculationGroup;

                calculationInputObserver.Observable = data;
                calculationGroupObserver.Observable = data;
                UpdateDataGridViewDataSource();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            // Necessary to correctly load the content of the dropdown lists of the comboboxes...
            UpdateDataGridViewDataSource();
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver?.Dispose();
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

            if (FailureMechanism?.SectionResults == null || data?.Children == null)
            {
                scenarioSelectionControl.ClearDataSource();
            }
            else
            {
                ICalculation[] calculations = data.GetCalculations().ToArray();

                IDictionary<string, List<ICalculation>> calculationsPerSegment =
                    StructuresHelper.CollectCalculationsPerSection(failureMechanism.Sections, calculations.Cast<StructuresCalculation<HeightStructuresInput>>());

                List<HeightStructuresScenarioRow> scenarioRows =
                    FailureMechanism.SectionResults.Select(sectionResult => new HeightStructuresScenarioRow(sectionResult)).ToList();

                scenarioSelectionControl.UpdateDataGridViewDataSource(calculations, scenarioRows, calculationsPerSegment);
            }
        }
    }
}
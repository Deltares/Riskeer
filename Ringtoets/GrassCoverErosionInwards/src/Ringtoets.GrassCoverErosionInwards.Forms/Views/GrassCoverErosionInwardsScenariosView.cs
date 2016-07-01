// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Creates a new instance of <see cref="GrassCoverErosionInwardsScenariosView"/>.
    /// </summary>
    public partial class GrassCoverErosionInwardsScenariosView : UserControl, IView
    {
        private const int calculationsColumnIndex = 1;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly Observer failureMechanismObserver;
        private GrassCoverErosionInwardsFailureMechanism failureMechanism;
        private object data;
        private DataGridViewControl dataGridViewControl;

        /// <summary>
        /// Creates a new instance of the <see cref="GrassCoverErosionInwardsScenariosView"/> class.
        /// </summary>
        public GrassCoverErosionInwardsScenariosView()
        {
            InitializeComponent();

            AddDataGridColumns();

            failureMechanismObserver = new Observer(UpdateDataGridViewDataSource);

            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(UpdateDataGridViewDataSource, cg => cg.Children.Concat<object>(cg.Children.OfType<ICalculation>().Select(c => c.GetObservableInput())));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(UpdateDataGridViewDataSource, c => c.Children);
        }

        /// <summary>
        /// Gets or sets the failure mechanism.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism FailureMechanism
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
                data = value;

                CalculationGroup observableGroup = data as CalculationGroup;
                if (observableGroup != null)
                {
                    calculationInputObserver.Observable = observableGroup;
                    calculationGroupObserver.Observable = observableGroup;
                }
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
            if (failureMechanismObserver != null)
            {
                failureMechanismObserver.Dispose();
            }
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateDataGridViewDataComboBoxesContent()
        {
            UpdateCalculationsColumn();
        }

        private void UpdateCalculationsColumn()
        {
            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> calculationsPerSegmentName = MapCalculationsToSegments();
            foreach (DataGridViewRow dataGridViewRow in dataGridViewControl.GetRows())
            {
                FillAvailableCalculationsList(dataGridViewRow, calculationsPerSegmentName);
            }
        }

        private Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> MapCalculationsToSegments()
        {
            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> map = new Dictionary<string, IList<GrassCoverErosionInwardsCalculation>>();

            Dictionary<string, IEnumerable<Segment2D>> sectionSegments = new Dictionary<string, IEnumerable<Segment2D>>();
            foreach (var sectionResult in FailureMechanism.SectionResults)
            {
                IEnumerable<Segment2D> lineSegments = Math2D.ConvertLinePointsToLineSegments(sectionResult.Section.Points);
                sectionSegments.Add(sectionResult.Section.Name, lineSegments);
            }

            CalculationGroup calculationGroup = data as CalculationGroup;
            if (calculationGroup != null)
            {
                IEnumerable<GrassCoverErosionInwardsCalculation> calculations = calculationGroup.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>();
                foreach (var calculation in calculations)
                {
                    var minimumDistance = double.PositiveInfinity;
                    string sectionName = null;
                    foreach (KeyValuePair<string, IEnumerable<Segment2D>> keyValuePair in sectionSegments)
                    {
                        if (calculation.InputParameters.DikeProfile != null)
                        {
                            var distance = keyValuePair.Value.Min(segment => segment.GetEuclideanDistanceToPoint(calculation.InputParameters.DikeProfile.WorldReferencePoint));
                            if (distance < minimumDistance)
                            {
                                minimumDistance = distance;
                                sectionName = keyValuePair.Key;
                            }
                        }
                    }

                    if (sectionName != null)
                    {
                        if (!map.ContainsKey(sectionName))
                        {
                            map.Add(sectionName, new List<GrassCoverErosionInwardsCalculation>());
                        }
                        map[sectionName].Add(calculation);
                    }
                }
            }
            return map;
        }

        private void FillAvailableCalculationsList(DataGridViewRow dataGridViewRow, Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> calculationsPerSegmentName)
        {
            var rowData = (GrassCoverErosionInwardsSectionResultRow) dataGridViewRow.DataBoundItem;
            string sectionName = rowData.Name;

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[calculationsColumnIndex];
            var items = new List<DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>>
            {
                new DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>(null)
            };

            if (calculationsPerSegmentName.ContainsKey(sectionName))
            {
                items.AddRange(calculationsPerSegmentName[sectionName].Select(c => new DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>(c)));
            }
            cell.DataSource = items;
            cell.Value = rowData.Calculation;
        }

        private void AddDataGridColumns()
        {
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<GrassCoverErosionInwardsSectionResultRow>(sr => sr.Name),
                                                 Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                                                 true);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>>(
                TypeUtils.GetMemberName<GrassCoverErosionInwardsSectionResultRow>(sr => sr.Calculation),
                Properties.Resources.GrassCoverErosionInwardsScenariosView_AddDataGridColumns_Calculation,
                null,
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>>(wrapper => wrapper.WrappedObject),
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>>(wrapper => wrapper.DisplayName)
                );
        }

        private void UpdateDataGridViewDataSource()
        {
            dataGridViewControl.EndEdit();

            if (FailureMechanism == null || FailureMechanism.SectionResults == null)
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            dataGridViewControl.SetDataSource(FailureMechanism.SectionResults.Select(CreateGrassCoverErosionInwardsSectionResultRow).ToList());

            UpdateDataGridViewDataComboBoxesContent();
        }

        private GrassCoverErosionInwardsSectionResultRow CreateGrassCoverErosionInwardsSectionResultRow(GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult)
        {
            return new GrassCoverErosionInwardsSectionResultRow()
            {
                Name = sectionResult.Section.Name,
                Calculation = null
            };
        }
    }
}
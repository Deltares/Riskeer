﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view for configuring calculations.
    /// </summary>
    /// <typeparam name="TCalculation">The type of calculation.</typeparam>
    /// /// <typeparam name="TCalculationInput">The type of the calculation input.</typeparam>
    /// <typeparam name="TCalculationRow">The type of the calculation row.</typeparam>
    public abstract partial class CalculationsView<TCalculation, TCalculationInput, TCalculationRow> : UserControl, ISelectionProvider, IView
        where TCalculation : class, ICalculation<TCalculationInput>
        where TCalculationRow : CalculationRow<TCalculation>
        where TCalculationInput : class, ICalculationInput
    {
        private const int selectableHydraulicBoundaryLocationColumnIndex = 1;

        private readonly IFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        private Observer failureMechanismObserver;
        private Observer hydraulicBoundaryLocationsObserver;
        private RecursiveObserver<CalculationGroup, TCalculationInput> inputObserver;
        private RecursiveObserver<CalculationGroup, TCalculation> calculationObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;

        private CalculationGroup calculationGroup;
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="CalculationsView{TCalculation, TCalculationInput, TCalculationRow}"/>.
        /// </summary>
        /// <param name="calculationGroup">All the calculations of the failure mechanism.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> the calculations belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the calculations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected CalculationsView(CalculationGroup calculationGroup, IFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.calculationGroup = calculationGroup;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            InitializeObservers();

            InitializeComponent();

            InitializeListBox();
            InitializeDataGridView();

            UpdateSectionsListBox();
            UpdateDataGridViewDataSource();
        }

        public object Selection => CreateSelectedItemFromCurrentRow();

        public object Data
        {
            get => calculationGroup;
            set => calculationGroup = value as CalculationGroup;
        }

        protected abstract object CreateSelectedItemFromCurrentRow();

        protected override void OnLoad(EventArgs e)
        {
            // Necessary to correctly load the content of the dropdown lists of the comboboxes...
            UpdateDataGridViewDataSource();

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                failureMechanismObserver.Dispose();
                inputObserver.Dispose();
                calculationObserver.Dispose();
                calculationGroupObserver.Dispose();
                hydraulicBoundaryLocationsObserver.Dispose();

                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected abstract IEnumerable<Point2D> GetReferenceLocations();

        protected abstract bool IsCalculationIntersectionWithReferenceLineInSection(TCalculation calculation, IEnumerable<Segment2D> lineSegments);

        protected abstract TCalculationRow CreateRow(TCalculation calculation);

        private void InitializeObservers()
        {
            failureMechanismObserver = new Observer(UpdateSectionsListBox)
            {
                Observable = failureMechanism
            };
            hydraulicBoundaryLocationsObserver = new Observer(() =>
            {
                PrefillComboBoxListItemsAtColumnLevel();
                UpdateSelectableHydraulicBoundaryLocationsColumn();
            })
            {
                Observable = assessmentSection.HydraulicBoundaryDatabase.Locations
            };
            
            // The concat is needed to observe the input of calculations in child groups.
            inputObserver = new RecursiveObserver<CalculationGroup, TCalculationInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<TCalculation>().Select(pc => pc.InputParameters)))
            {
                Observable = calculationGroup
            };
            calculationObserver = new RecursiveObserver<CalculationGroup, TCalculation>(() => DataGridViewControl.RefreshDataGridView(), pcg => pcg.Children)
            {
                Observable = calculationGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children)
            {
                Observable = calculationGroup
            };
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = nameof(FailureMechanismSection.Name);
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (failureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(failureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = failureMechanism.Sections.First();
            }
        }

        private void InitializeDataGridView()
        {
            DataGridViewControl.CurrentRowChanged += DataGridViewOnCurrentRowChangedHandler;

            DataGridViewControl.AddTextBoxColumn(
                nameof(CalculationRow<TCalculation>.Name),
                Resources.FailureMechanism_Name_DisplayName);

            DataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>>(
                nameof(CalculationRow<TCalculation>.SelectableHydraulicBoundaryLocation),
                Resources.HydraulicBoundaryLocation_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.This),
                nameof(DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>.DisplayName));
        }

        private void UpdateDataGridViewDataSource()
        {
            // Skip changes coming from the view itself
            if (DataGridViewControl.IsCurrentCellInEditMode)
            {
                DataGridViewControl.AutoResizeColumns();
            }

            if (!(listBox.SelectedItem is FailureMechanismSection failureMechanismSection))
            {
                DataGridViewControl.SetDataSource(null);
                return;
            }

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<TCalculation> calculations = calculationGroup
                                                     .GetCalculations()
                                                     .OfType<TCalculation>()
                                                     .Where(cs => IsCalculationIntersectionWithReferenceLineInSection(cs, lineSegments));

            PrefillComboBoxListItemsAtColumnLevel();

            List<TCalculationRow> dataSource = calculations.Select(CreateRow).ToList();
            DataGridViewControl.SetDataSource(dataSource);
            DataGridViewControl.ClearCurrentCell();

            UpdateSelectableHydraulicBoundaryLocationsColumn();
        }

        #region Prefill combo box list items

        private void PrefillComboBoxListItemsAtColumnLevel()
        {
            var selectableHydraulicBoundaryLocationColumn = (DataGridViewComboBoxColumn) DataGridViewControl.GetColumnFromIndex(selectableHydraulicBoundaryLocationColumnIndex);

            // Need to prefill for all possible data in order to guarantee 'combo box' columns
            // do not generate errors when their cell value is not present in the list of available
            // items.
            using (new SuspendDataGridViewColumnResizes(selectableHydraulicBoundaryLocationColumn))
            {
                SetItemsOnObjectCollection(selectableHydraulicBoundaryLocationColumn.Items,
                                           GetSelectableHydraulicBoundaryLocationsDataSource(GetSelectableHydraulicBoundaryLocationsFromFailureMechanism()));
            }
        }

        #endregion

        private static void SetItemsOnObjectCollection(DataGridViewComboBoxCell.ObjectCollection objectCollection, object[] comboBoxItems)
        {
            objectCollection.Clear();
            objectCollection.AddRange(comboBoxItems);
        }

        #region HydraulicBoundaryLocations

        private void UpdateSelectableHydraulicBoundaryLocationsColumn()
        {
            var column = (DataGridViewComboBoxColumn) DataGridViewControl.GetColumnFromIndex(selectableHydraulicBoundaryLocationColumnIndex);

            using (new SuspendDataGridViewColumnResizes(column))
            {
                foreach (DataGridViewRow dataGridViewRow in DataGridViewControl.Rows)
                {
                    FillAvailableSelectableHydraulicBoundaryLocationsList(dataGridViewRow);
                }
            }
        }

        private void FillAvailableSelectableHydraulicBoundaryLocationsList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (TCalculationRow) dataGridViewRow.DataBoundItem;
            IEnumerable<SelectableHydraulicBoundaryLocation> locations = GetSelectableHydraulicBoundaryLocations(assessmentSection?.HydraulicBoundaryDatabase.Locations,
                                                                                                                 rowData.GetCalculationLocation());

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[selectableHydraulicBoundaryLocationColumnIndex];
            DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>[] dataGridViewComboBoxItemWrappers = GetSelectableHydraulicBoundaryLocationsDataSource(locations);
            SetItemsOnObjectCollection(cell.Items, dataGridViewComboBoxItemWrappers);
        }

        private IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocationsFromFailureMechanism()
        {
            List<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;

            List<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations = hydraulicBoundaryLocations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null)).ToList();

            foreach (Point2D referenceLocation in GetReferenceLocations())
            {
                selectableHydraulicBoundaryLocations.AddRange(GetSelectableHydraulicBoundaryLocations(hydraulicBoundaryLocations, referenceLocation));
            }

            return selectableHydraulicBoundaryLocations;
        }

        private static DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>[] GetSelectableHydraulicBoundaryLocationsDataSource(
            IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations = null)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>>
            {
                new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(null)
            };

            if (selectableHydraulicBoundaryLocations != null)
            {
                dataGridViewComboBoxItemWrappers.AddRange(selectableHydraulicBoundaryLocations.Select(hbl => new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(hbl)));
            }

            return dataGridViewComboBoxItemWrappers.ToArray();
        }

        private static IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations(
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, Point2D referencePoint)
        {
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                hydraulicBoundaryLocations, referencePoint);
        }

        #endregion

        #region Event handling

        private void DataGridViewOnCurrentRowChangedHandler(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
        }

        #endregion
    }
}
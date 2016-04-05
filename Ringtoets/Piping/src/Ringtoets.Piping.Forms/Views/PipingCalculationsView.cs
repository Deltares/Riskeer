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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Gui.Selection;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring piping calculations.
    /// </summary>
    public partial class PipingCalculationsView : UserControl, IView
    {
        private readonly Observer assessmentSectionObserver;
        private readonly RecursiveObserver<PipingCalculationGroup, PipingCalculationGroup> pipingCalculationGroupObserver;
        private readonly RecursiveObserver<PipingCalculationGroup, PipingCalculation> pipingCalculationObserver;
        private readonly Observer pipingFailureMechanismObserver;
        private readonly RecursiveObserver<PipingCalculationGroup, PipingInput> pipingInputObserver;
        private readonly Observer pipingStochasticSoilProfilesObserver;
        private IAssessmentSection assessmentSection;
        private DataGridViewComboBoxColumn hydraulicBoundaryLocationColumn;
        private PipingCalculationGroup pipingCalculationGroup;
        private PipingFailureMechanism pipingFailureMechanism;
        private DataGridViewComboBoxColumn stochasticSoilProfileColumn;
        private bool updatingDataSource;

        /// <summary>
        /// Creates a new instance of the <see cref="PipingCalculationsView"/> class.
        /// </summary>
        public PipingCalculationsView()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();

            pipingStochasticSoilProfilesObserver = new Observer(OnStochasticSoilProfilesUpdate);
            pipingFailureMechanismObserver = new Observer(OnPipingFailureMechanismUpdate);
            assessmentSectionObserver = new Observer(UpdateHydraulicBoundaryLocationsColumn);
            pipingInputObserver = new RecursiveObserver<PipingCalculationGroup, PipingInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<PipingCalculation>().Select(pc => pc.InputParameters)));
            pipingCalculationObserver = new RecursiveObserver<PipingCalculationGroup, PipingCalculation>(RefreshDataGridView, pcg => pcg.Children);
            pipingCalculationGroupObserver = new RecursiveObserver<PipingCalculationGroup, PipingCalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children);
        }

        /// <summary>
        /// Gets or sets the piping failure mechanism.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism
        {
            get
            {
                return pipingFailureMechanism;
            }
            set
            {
                pipingFailureMechanism = value;

                pipingStochasticSoilProfilesObserver.Observable = pipingFailureMechanism != null ? pipingFailureMechanism.StochasticSoilModels : null;
                pipingFailureMechanismObserver.Observable = pipingFailureMechanism;

                UpdateStochasticSoilProfileColumn();
                UpdateSectionsListBox();
                UpdateGenerateScenariosButtonState();
            }
        }

        /// <summary>
        /// Gets or sets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection
        {
            get
            {
                return assessmentSection;
            }
            set
            {
                assessmentSection = value;

                assessmentSectionObserver.Observable = assessmentSection;

                UpdateHydraulicBoundaryLocationsColumn();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IApplicationSelection"/>.
        /// </summary>
        public IApplicationSelection ApplicationSelection { get; set; }

        public object Data
        {
            get
            {
                return pipingCalculationGroup;
            }
            set
            {
                pipingCalculationGroup = value as PipingCalculationGroup;

                if (pipingCalculationGroup != null)
                {
                    UpdateDataGridViewDataSource();
                    pipingInputObserver.Observable = pipingCalculationGroup;
                    pipingCalculationObserver.Observable = pipingCalculationGroup;
                    pipingCalculationGroupObserver.Observable = pipingCalculationGroup;
                }
                else
                {
                    dataGridView.DataSource = null;
                    pipingInputObserver.Observable = null;
                    pipingCalculationObserver.Observable = null;
                    pipingCalculationGroupObserver.Observable = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            AssessmentSection = null;
            PipingFailureMechanism = null;

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridView.CurrentCellDirtyStateChanged += DataGridViewCurrentCellDirtyStateChanged;
            dataGridView.CellClick += DataGridViewOnCellClick;

            var nameColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = Resources.PipingCalculation_Name_DisplayName,
                Name = "column_Name"
            };

            stochasticSoilProfileColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "StochasticSoilProfile",
                HeaderText = Resources.PipingInput_StochasticSoilProfile_DisplayName,
                Name = "column_SoilProfile",
                ValueMember = "This",
                DisplayMember = "DisplayName"
            };

            hydraulicBoundaryLocationColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "HydraulicBoundaryLocation",
                HeaderText = Resources.PipingInput_HydraulicBoundaryLocation_DisplayName,
                Name = "column_HydraulicBoundaryLocation",
                ValueMember = "This",
                DisplayMember = "DisplayName"
            };

            var dampingFactorExitHeader = Resources.PipingInput_DampingFactorExit_DisplayName;
            dampingFactorExitHeader = char.ToLowerInvariant(dampingFactorExitHeader[0]) + dampingFactorExitHeader.Substring(1);
            var dampingFactorExitMeanColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DampingFactorExitMean",
                HeaderText = string.Format("{0} {1}", Resources.Probabilistics_Mean_Symbol, dampingFactorExitHeader),
                Name = "column_DampingFactorExitMean"
            };

            var phreaticLevelExitHeader = Resources.PipingInput_PhreaticLevelExit_DisplayName;
            phreaticLevelExitHeader = char.ToLowerInvariant(phreaticLevelExitHeader[0]) + phreaticLevelExitHeader.Substring(1);
            var phreaticLevelExitMeanColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PhreaticLevelExitMean",
                HeaderText = string.Format("{0} {1}", Resources.Probabilistics_Mean_Symbol, phreaticLevelExitHeader),
                Name = "column_PhreaticLevelExitMean"
            };

            var entryPointLColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EntryPointL",
                HeaderText = Resources.PipingInput_EntryPointL_DisplayName,
                Name = "column_EntryPointL"
            };

            var exitPointLColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ExitPointL",
                HeaderText = Resources.PipingInput_ExitPointL_DisplayName,
                Name = "column_ExitPointL"
            };

            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.AddRange(
                nameColumn,
                stochasticSoilProfileColumn,
                hydraulicBoundaryLocationColumn,
                dampingFactorExitMeanColumn,
                phreaticLevelExitMeanColumn,
                entryPointLColumn,
                exitPointLColumn);

            foreach (var column in dataGridView.Columns.OfType<DataGridViewColumn>())
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            UpdateHydraulicBoundaryLocationsColumn();
            UpdateStochasticSoilProfileColumn();
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = "Name";

            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateHydraulicBoundaryLocationsColumn()
        {
            using (new SuspendDataGridViewColumnResizes(hydraulicBoundaryLocationColumn))
            {
                var hydraulicBoundaryLocations = assessmentSection != null && assessmentSection.HydraulicBoundaryDatabase != null
                                                     ? assessmentSection.HydraulicBoundaryDatabase.Locations
                                                     : null;
                SetItemsOnObjectCollection(hydraulicBoundaryLocationColumn.Items, GetHydraulicBoundaryLocationsDataSource(hydraulicBoundaryLocations).ToArray());
            }
        }

        private void OnStochasticSoilProfilesUpdate()
        {
            UpdateGenerateScenariosButtonState();
            UpdateStochasticSoilProfileColumn();
        }

        private void UpdateStochasticSoilProfileColumn()
        {
            using (new SuspendDataGridViewColumnResizes(stochasticSoilProfileColumn))
            {
                foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
                {
                    FillAvailableSoilProfilesList(dataGridViewRow);
                }
            }
        }

        private void UpdateGenerateScenariosButtonState()
        {
            buttonGenerateScenarios.Enabled = pipingFailureMechanism != null &&
                                              pipingFailureMechanism.SurfaceLines.Any() &&
                                              pipingFailureMechanism.StochasticSoilModels.Any();
        }

        private void RefreshDataGridView()
        {
            dataGridView.Refresh();
            dataGridView.AutoResizeColumns();
        }

        private void UpdateDataGridViewDataSource()
        {
            // Skip changes coming from the view itself
            if (dataGridView.IsCurrentCellInEditMode)
            {
                dataGridView.AutoResizeColumns();

                return;
            }

            var failureMechanismSection = listBox.SelectedItem as FailureMechanismSection;
            if (failureMechanismSection == null)
            {
                dataGridView.DataSource = null;
                return;
            }

            var lineSegments = Math2D.ConvertLinePointsToLineSegments(failureMechanismSection.Points);
            var pipingCalculations = pipingCalculationGroup
                .GetPipingCalculations()
                .Where(pc => IsSurfaceLineIntersectionWithReferenceLineInSection(pc.InputParameters.SurfaceLine, lineSegments));

            updatingDataSource = true;

            PrefillComboBoxListItemsAtColumnLevel();

            dataGridView.DataSource = pipingCalculations
                .Select(pc => new PipingCalculationRow(pc))
                .ToList();

            UpdateStochasticSoilProfileColumn();

            updatingDataSource = false;
        }

        private static bool IsSurfaceLineIntersectionWithReferenceLineInSection(RingtoetsPipingSurfaceLine surfaceLine, IEnumerable<Segment2D> lineSegments)
        {
            if (surfaceLine == null)
            {
                return false;
            }
            var minimalDistance = lineSegments.Min(segment => segment.GetEuclideanDistanceToPoint(surfaceLine.ReferenceLineIntersectionWorldPoint));
            return minimalDistance < 1.0e-6;
        }

        private void PrefillComboBoxListItemsAtColumnLevel()
        {
            // Need to prefill for all possible data in order to guarantee 'combo box' columns
            // do not generate errors when their cell value is not present in the list of available
            // items.
            using (new SuspendDataGridViewColumnResizes(stochasticSoilProfileColumn))
            {
                var pipingSoilProfiles = GetPipingStochasticSoilProfilesFromStochasticSoilModels();
                SetItemsOnObjectCollection(stochasticSoilProfileColumn.Items, GetSoilProfilesDataSource(pipingSoilProfiles).ToArray());
            }
            using (new SuspendDataGridViewColumnResizes(hydraulicBoundaryLocationColumn))
            {
                var hydraulicBoundaryLocations = assessmentSection != null && assessmentSection.HydraulicBoundaryDatabase != null
                                                     ? assessmentSection.HydraulicBoundaryDatabase.Locations
                                                     : null;
                SetItemsOnObjectCollection(
                    hydraulicBoundaryLocationColumn.Items,
                    GetHydraulicBoundaryLocationsDataSource(hydraulicBoundaryLocations).ToArray());
            }
        }

        private StochasticSoilProfile[] GetPipingStochasticSoilProfilesFromStochasticSoilModels()
        {
            if (pipingFailureMechanism != null)
            {
                return pipingFailureMechanism.StochasticSoilModels
                                             .SelectMany(ssm => ssm.StochasticSoilProfiles)
                                             .Distinct()
                                             .ToArray();
            }
            return null;
        }

        private void FillAvailableSoilProfilesList(DataGridViewRow dataGridViewRow)
        {
            var rowData = (PipingCalculationRow) dataGridViewRow.DataBoundItem;

            IEnumerable<StochasticSoilProfile> stochasticSoilProfiles = GetSoilProfilesForCalculation(rowData.PipingCalculation);

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[stochasticSoilProfileColumn.Index];
            SetItemsOnObjectCollection(cell.Items, GetSoilProfilesDataSource(stochasticSoilProfiles).ToArray());
        }

        private IEnumerable<StochasticSoilProfile> GetSoilProfilesForCalculation(PipingCalculation pipingCalculation)
        {
            if (pipingFailureMechanism == null)
            {
                return Enumerable.Empty<StochasticSoilProfile>();
            }
            return PipingCalculationConfigurationHelper.GetStochasticSoilProfilesForSurfaceLine(
                pipingCalculation.InputParameters.SurfaceLine,
                pipingFailureMechanism.StochasticSoilModels);
        }

        private static void SetItemsOnObjectCollection(DataGridViewComboBoxCell.ObjectCollection objectCollection, object[] comboBoxItems)
        {
            objectCollection.Clear();
            objectCollection.AddRange(comboBoxItems);
        }

        private void OnPipingFailureMechanismUpdate()
        {
            UpdateGenerateScenariosButtonState();
            UpdateSectionsListBox();
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (pipingFailureMechanism != null && pipingFailureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(pipingFailureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = pipingFailureMechanism.Sections.First();
            }
        }

        private static IEnumerable<DataGridViewComboBoxItemWrapper<StochasticSoilProfile>> GetSoilProfilesDataSource(IEnumerable<StochasticSoilProfile> stochasticSoilProfile = null)
        {
            yield return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(null);

            if (stochasticSoilProfile != null)
            {
                foreach (StochasticSoilProfile profile in stochasticSoilProfile)
                {
                    yield return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(profile);
                }
            }
        }

        private static List<DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>> GetHydraulicBoundaryLocationsDataSource(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = null)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>>
            {
                new DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>(null)
            };

            if (hydraulicBoundaryLocations != null)
            {
                dataGridViewComboBoxItemWrappers.AddRange(hydraulicBoundaryLocations.Select(hbl => new DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>(hbl)));
            }

            return dataGridViewComboBoxItemWrappers;
        }

        #region Nested types

        /// <summary>
        /// This class makes it easier to temporarily disable automatic resizing of a column,
        /// for example when it's data is being changed or you are replacing the list items
        /// available in a combo-box for that column.
        /// </summary>
        private class SuspendDataGridViewColumnResizes : IDisposable
        {
            private readonly DataGridViewColumn column;
            private readonly DataGridViewAutoSizeColumnMode originalValue;

            public SuspendDataGridViewColumnResizes(DataGridViewColumn columnToSuspend)
            {
                column = columnToSuspend;
                originalValue = columnToSuspend.AutoSizeMode;
                columnToSuspend.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            public void Dispose()
            {
                column.AutoSizeMode = originalValue;
            }
        }

        private class PipingCalculationRow
        {
            private readonly PipingCalculation pipingCalculation;

            public PipingCalculationRow(PipingCalculation pipingCalculation)
            {
                this.pipingCalculation = pipingCalculation;
            }

            public PipingCalculation PipingCalculation
            {
                get
                {
                    return pipingCalculation;
                }
            }

            public string Name
            {
                get
                {
                    return pipingCalculation.Name;
                }
                set
                {
                    pipingCalculation.Name = value;

                    pipingCalculation.NotifyObservers();
                }
            }

            public DataGridViewComboBoxItemWrapper<StochasticSoilProfile> StochasticSoilProfile
            {
                get
                {
                    return new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(pipingCalculation.InputParameters.StochasticSoilProfile);
                }
                set
                {
                    pipingCalculation.InputParameters.StochasticSoilProfile = value != null
                                                                                  ? value.WrappedObject
                                                                                  : null;

                    pipingCalculation.InputParameters.NotifyObservers();
                }
            }

            public DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation> HydraulicBoundaryLocation
            {
                get
                {
                    return new DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>(pipingCalculation.InputParameters.HydraulicBoundaryLocation);
                }
                set
                {
                    pipingCalculation.InputParameters.HydraulicBoundaryLocation = value != null
                                                                                      ? value.WrappedObject
                                                                                      : null;

                    pipingCalculation.InputParameters.NotifyObservers();
                }
            }

            public RoundedDouble DampingFactorExitMean
            {
                get
                {
                    return pipingCalculation.InputParameters.DampingFactorExit.Mean;
                }
                set
                {
                    pipingCalculation.InputParameters.DampingFactorExit.Mean = value;

                    pipingCalculation.InputParameters.NotifyObservers();
                }
            }

            public RoundedDouble PhreaticLevelExitMean
            {
                get
                {
                    return pipingCalculation.InputParameters.PhreaticLevelExit.Mean;
                }
                set
                {
                    pipingCalculation.InputParameters.PhreaticLevelExit.Mean = value;

                    pipingCalculation.InputParameters.NotifyObservers();
                }
            }

            public RoundedDouble EntryPointL
            {
                get
                {
                    return pipingCalculation.InputParameters.EntryPointL;
                }
                set
                {
                    pipingCalculation.InputParameters.EntryPointL = value;

                    pipingCalculation.InputParameters.NotifyObservers();
                }
            }

            public RoundedDouble ExitPointL
            {
                get
                {
                    return pipingCalculation.InputParameters.ExitPointL;
                }
                set
                {
                    pipingCalculation.InputParameters.ExitPointL = value;

                    pipingCalculation.InputParameters.NotifyObservers();
                }
            }
        }

        #endregion

        # region Event handling

        private void DataGridViewCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Ensure combobox values are directly committed
            DataGridViewColumn currentColumn = dataGridView.Columns[dataGridView.CurrentCell.ColumnIndex];
            if (currentColumn is DataGridViewComboBoxColumn)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dataGridView.EndEdit();
            }
        }

        private void DataGridViewOnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (updatingDataSource)
            {
                return;
            }

            UpdateApplicationSelection();
        }

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
            UpdateApplicationSelection();
        }

        private void OnGenerateScenariosButtonClick(object sender, EventArgs e)
        {
            var dialog = new PipingSurfaceLineSelectionDialog(Parent, pipingFailureMechanism.SurfaceLines);
            dialog.ShowDialog();
            var calculationsStructure = PipingCalculationConfigurationHelper.GenerateCalculationsStructure(
                dialog.SelectedSurfaceLines,
                pipingFailureMechanism.StochasticSoilModels,
                pipingFailureMechanism.GeneralInput,
                pipingFailureMechanism.SemiProbabilisticInput);
            foreach (var item in calculationsStructure)
            {
                pipingCalculationGroup.Children.Add(item);
            }
            pipingCalculationGroup.NotifyObservers();
        }

        private void UpdateApplicationSelection()
        {
            if (ApplicationSelection == null)
            {
                return;
            }

            var pipingCalculationRow = dataGridView.CurrentRow != null
                                           ? (PipingCalculationRow) dataGridView.CurrentRow.DataBoundItem
                                           : null;

            PipingInputContext selection = null;
            if (pipingCalculationRow != null)
            {
                selection = new PipingInputContext(
                    pipingCalculationRow.PipingCalculation.InputParameters,
                    pipingFailureMechanism.SurfaceLines,
                    pipingFailureMechanism.StochasticSoilModels,
                    assessmentSection);
            }

            if ((ApplicationSelection.Selection == null && selection != null)
                || (ApplicationSelection.Selection != null && !ApplicationSelection.Selection.Equals(selection)))
            {
                ApplicationSelection.Selection = selection;
            }
        }

        # endregion
    }
}
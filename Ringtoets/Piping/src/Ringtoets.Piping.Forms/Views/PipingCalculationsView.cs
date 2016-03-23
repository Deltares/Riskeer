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
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring piping calculations.
    /// </summary>
    public partial class PipingCalculationsView : UserControl, IView
    {
        private readonly Observer pipingSoilProfilesObserver;
        private readonly Observer assessmentSectionObserver;
        private readonly RecursiveObserver<PipingCalculationGroup, PipingInput> pipingInputObserver;
        private readonly RecursiveObserver<PipingCalculationGroup, PipingCalculation> pipingCalculationObserver;
        private readonly RecursiveObserver<PipingCalculationGroup, PipingCalculationGroup> pipingCalculationGroupObserver;
        private AssessmentSectionBase assessmentSection;
        private PipingFailureMechanism pipingFailureMechanism;
        private PipingCalculationGroup pipingCalculationGroup;
        private DataGridViewComboBoxColumn soilProfileColumn;
        private DataGridViewComboBoxColumn hydraulicBoundaryLocationColumn;

        /// <summary>
        /// Creates a new instance of the <see cref="PipingCalculationsView"/> class.
        /// </summary>
        public PipingCalculationsView()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();

            pipingSoilProfilesObserver = new Observer(UpdateSoilProfileColumn);
            assessmentSectionObserver = new Observer(UpdateHydraulicBoundaryLocationsColumn);
            pipingInputObserver = new RecursiveObserver<PipingCalculationGroup, PipingInput>(RefreshDataGridView, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<PipingCalculation>().Select(pc => pc.InputParameters)));
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

                pipingSoilProfilesObserver.Observable = pipingFailureMechanism != null ? pipingFailureMechanism.StochasticSoilModels : null;

                UpdateSoilProfileColumn();
                UpdateDikeSectionsListBox();
            }
        }

        /// <summary>
        /// Gets or sets the assessment section.
        /// </summary>
        public AssessmentSectionBase AssessmentSection
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

            var nameColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = Resources.PipingCalculation_Name_DisplayName,
                Name = "column_Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            soilProfileColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "SoilProfile",
                HeaderText = Resources.PipingInput_SoilProfile_DisplayName,
                Name = "column_SoilProfile",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ValueMember = "This",
                DisplayMember = "DisplayName",
                DataSource = GetSoilProfilesDataSource()
            };

            hydraulicBoundaryLocationColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "HydraulicBoundaryLocation",
                HeaderText = Resources.PipingInput_HydraulicBoundaryLocation_DisplayName,
                Name = "column_HydraulicBoundaryLocation",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ValueMember = "This",
                DisplayMember = "DisplayName",
                DataSource = GetHydraulicBoundaryLocationsDataSource()
            };

            var dampingFactorExitMeanColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DampingFactorExitMean",
                HeaderText = Resources.PipingInput_DampingFactorExit_DisplayName,
                Name = "column_DampingFactorExitMean",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            var phreaticLevelExitMeanColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PhreaticLevelExitMean",
                HeaderText = Resources.PipingInput_PhreaticLevelExit_DisplayName,
                Name = "column_PhreaticLevelExitMean",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            var entryPointLColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EntryPointL",
                HeaderText = Resources.PipingInput_EntryPointL_DisplayName,
                Name = "column_EntryPointL",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            var exitPointLColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ExitPointL",
                HeaderText = Resources.PipingInput_ExitPointL_DisplayName,
                Name = "column_ExitPointL",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.AddRange(nameColumn, soilProfileColumn, hydraulicBoundaryLocationColumn, dampingFactorExitMeanColumn, phreaticLevelExitMeanColumn, entryPointLColumn, exitPointLColumn);
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = "Name";

            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateHydraulicBoundaryLocationsColumn()
        {
            var hydraulicBoundaryLocations = assessmentSection != null && assessmentSection.HydraulicBoundaryDatabase != null
                                                 ? assessmentSection.HydraulicBoundaryDatabase.Locations
                                                 : null;

            hydraulicBoundaryLocationColumn.DataSource = GetHydraulicBoundaryLocationsDataSource(hydraulicBoundaryLocations);
        }

        private void UpdateSoilProfileColumn()
        {
            var pipingSoilProfiles = pipingFailureMechanism != null ? pipingFailureMechanism.SoilProfiles : null;

            soilProfileColumn.DataSource = GetSoilProfilesDataSource(pipingSoilProfiles);
        }

        private void RefreshDataGridView()
        {
            dataGridView.Refresh();
            dataGridView.AutoResizeColumns();
        }

        private void UpdateDataGridViewDataSource()
        {
            var failureMechanismSection = listBox.SelectedItem as FailureMechanismSection;
            if (failureMechanismSection == null)
            {
                dataGridView.DataSource = null;
                return;
            }

            var lineSegments = Math2D.ConvertLinePointsToLineSegments(failureMechanismSection.Points);
            var pipingCalculations = pipingCalculationGroup.GetPipingCalculations()
                                                           .Where(pc => pc.InputParameters.SurfaceLine != null
                                                                        && lineSegments.Min(segment => segment.GetEuclideanDistanceToPoint(pc.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint)) < 1.0e-6);

            dataGridView.DataSource = pipingCalculations
                .Select(pc => new PipingCalculationRow(pc))
                .ToList();
        }

        private void UpdateDikeSectionsListBox()
        {
            listBox.Items.Clear();

            if (pipingFailureMechanism != null && pipingFailureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(pipingFailureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = pipingFailureMechanism.Sections.First();
            }
        }

        private static List<DataGridViewComboBoxItemWrapper<PipingSoilProfile>> GetSoilProfilesDataSource(IEnumerable<PipingSoilProfile> soilProfiles = null)
        {
            var dataGridViewComboBoxItemWrappers = new List<DataGridViewComboBoxItemWrapper<PipingSoilProfile>>
            {
                new DataGridViewComboBoxItemWrapper<PipingSoilProfile>(null)
            };

            if (soilProfiles != null)
            {
                dataGridViewComboBoxItemWrappers.AddRange(soilProfiles.Select(sp => new DataGridViewComboBoxItemWrapper<PipingSoilProfile>(sp)));
            }

            return dataGridViewComboBoxItemWrappers;
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

        private class PipingCalculationRow
        {
            private readonly PipingCalculation pipingCalculation;

            public PipingCalculationRow(PipingCalculation pipingCalculation)
            {
                this.pipingCalculation = pipingCalculation;
            }

            public string Name
            {
                get
                {
                    return pipingCalculation.Name;
                }
            }

            public DataGridViewComboBoxItemWrapper<PipingSoilProfile> SoilProfile
            {
                get
                {
                    return new DataGridViewComboBoxItemWrapper<PipingSoilProfile>(pipingCalculation.InputParameters.SoilProfile);
                }
                set
                {
                    pipingCalculation.InputParameters.SoilProfile = value != null
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

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs eventArgs)
        {
            UpdateDataGridViewDataSource();
        }

        # endregion
    }
}
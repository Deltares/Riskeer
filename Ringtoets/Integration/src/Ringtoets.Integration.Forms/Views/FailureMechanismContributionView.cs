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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;
using CommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="FailureMechanismContribution"/>, from which the <see cref="FailureMechanismContribution.Norm"/>
    /// can be updated and the <see cref="FailureMechanismContributionItem.Contribution"/>
    /// and <see cref="FailureMechanismContributionItem.ProbabilitySpace"/> can be seen in a grid.
    /// </summary>
    public partial class FailureMechanismContributionView : UserControl, IView, IObserver
    {
        private readonly Observer isFailureMechanismRelevantObserver;
        private readonly Observer closeViewsForIrrelevantFailureMechanismObserver;
        private DataGridViewColumn probabilityPerYearColumn;
        private FailureMechanismContribution data;

        private bool revertingComboBoxSelectedValue;
        private IAssessmentSection assessmentSection;

        private DataGridViewCheckBoxColumn isRelevantColumn;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionView"/>.
        /// </summary>
        public FailureMechanismContributionView()
        {
            InitializeComponent();
            InitializeGridColumns();
            InitializeAssessmentSectionCompositionComboBox();
            BindNormChange();
            BindNormInputLeave();
            SubscribeEvents();

            isFailureMechanismRelevantObserver = new Observer(SetRowStyling);
            closeViewsForIrrelevantFailureMechanismObserver = new Observer(CloseViewsForIrrelevantFailureMechanism);
            Load += OnLoad;
        }

        /// <summary>
        /// Gets or sets the assessment section this view belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection
        {
            get
            {
                return assessmentSection;
            }
            set
            {
                HandleNewAssessmentSectionSet(value);
            }
        }

        public IViewCommands ViewCommands { private get; set; }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                HandleNewDataSet((FailureMechanismContribution) value);
            }
        }

        public void UpdateObserver()
        {
            SetNormText();
            probabilityDistributionGrid.Invalidate();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed; otherwise, <c>false</c>.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            UnsubscribeEvents();
            DetachFromFailureMechanisms();
            ViewCommands = null;
            base.Dispose(disposing);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (probabilityDistributionGrid.DataSource != null)
            {
                SetRowStyling();
            }
        }

        private void InitializeAssessmentSectionCompositionComboBox()
        {
            assessmentSectionCompositionComboBox.DataSource = new[]
            {
                Tuple.Create(AssessmentSectionComposition.Dike, "Dijk"),
                Tuple.Create(AssessmentSectionComposition.Dune, "Duin"),
                Tuple.Create(AssessmentSectionComposition.DikeAndDune, "Dijk / Duin")
            };
            assessmentSectionCompositionComboBox.ValueMember = TypeUtils.GetMemberName<Tuple<AssessmentSectionComposition, string>>(t => t.Item1);
            assessmentSectionCompositionComboBox.DisplayMember = TypeUtils.GetMemberName<Tuple<AssessmentSectionComposition, string>>(t => t.Item2);
        }

        private void SubscribeEvents()
        {
            probabilityDistributionGrid.CellFormatting += ProbabilityDistributionGridOnCellFormatting;
        }

        private void UnsubscribeEvents()
        {
            probabilityDistributionGrid.CellFormatting -= ProbabilityDistributionGridOnCellFormatting;
        }

        private void ProbabilityDistributionGridOnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == probabilityPerYearColumn.Index)
            {
                var contributionItem = data.Distribution.ElementAt(e.RowIndex);
                if (Math.Abs(contributionItem.Contribution) < 1e-6)
                {
                    e.Value = RingtoetsIntegrationFormsResources.FailureMechanismContributionView_ProbabilityPerYear_Not_applicable;
                    e.FormattingApplied = true;
                }
            }
        }

        private void HandleNewDataSet(FailureMechanismContribution value)
        {
            UnbindNormChange();
            DetachFromData();

            data = value;

            SetGridDataSource();
            SetNormText();

            AttachToData();
            BindNormChange();
        }

        private void HandleNewAssessmentSectionSet(IAssessmentSection value)
        {
            UnbindAssessmentSectionCompositionChange();
            DetachFromFailureMechanisms();

            assessmentSection = value;

            AttachToFailureMechanisms();
            SetAssessmentSectionComposition();
            BindAssessmentSectionCompositionChange();
        }

        private void DetachFromFailureMechanisms()
        {
            if (assessmentSection != null)
            {
                foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
                {
                    failureMechanism.Detach(isFailureMechanismRelevantObserver);
                    failureMechanism.Detach(closeViewsForIrrelevantFailureMechanismObserver);
                }
            }
        }

        private void AttachToFailureMechanisms()
        {
            if (assessmentSection != null)
            {
                foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
                {
                    failureMechanism.Attach(isFailureMechanismRelevantObserver);
                    failureMechanism.Attach(closeViewsForIrrelevantFailureMechanismObserver);
                }
            }
        }

        private void CloseViewsForIrrelevantFailureMechanism()
        {
            if (ViewCommands != null)
            {
                var irrelevantFailureMechanisms = assessmentSection.GetFailureMechanisms().Where(failureMechanism => !failureMechanism.IsRelevant);

                foreach (var failureMechanism in irrelevantFailureMechanisms)
                {
                    ViewCommands.RemoveAllViewsForItem(failureMechanism);
                }
            }
        }

        private void SetGridDataSource()
        {
            if (data != null)
            {
                probabilityDistributionGrid.DataSource = data.Distribution.Select(ci => new FailureMechanismContributionItemRow(ci)).ToArray();
                SetRowStyling();
                probabilityDistributionGrid.Invalidate();
            }
        }

        private void AttachToData()
        {
            if (data != null)
            {
                data.Attach(this);
            }
        }

        private void DetachFromData()
        {
            if (data != null)
            {
                data.Detach(this);
            }
        }

        private void BindAssessmentSectionCompositionChange()
        {
            assessmentSectionCompositionComboBox.SelectedIndexChanged += AssessmentSectionCompositionComboBoxSelectedIndexChanged;
        }

        private void UnbindAssessmentSectionCompositionChange()
        {
            assessmentSectionCompositionComboBox.SelectedIndexChanged -= AssessmentSectionCompositionComboBoxSelectedIndexChanged;
        }

        private void BindNormChange()
        {
            normInput.ValueChanged += NormValueChanged;
        }

        private void UnbindNormChange()
        {
            normInput.ValueChanged -= NormValueChanged;
        }

        private void BindNormInputLeave()
        {
            normInput.Leave += NormInputLeave;
        }

        private void NormInputLeave(object sender, EventArgs e)
        {
            ResetTextIfEmtpy();
        }

        private void NormValueChanged(object sender, EventArgs eventArgs)
        {
            data.Norm = Convert.ToInt32(normInput.Value);
            data.NotifyObservers();
        }

        private void ResetTextIfEmtpy()
        {
            if (string.IsNullOrEmpty(normInput.Text))
            {
                normInput.Text = string.Format("{0}", normInput.Value);
            }
        }

        private void SetNormText()
        {
            if (data != null)
            {
                normInput.Value = data.Norm;
            }
        }

        private void SetAssessmentSectionComposition()
        {
            if (AssessmentSection != null)
            {
                assessmentSectionCompositionComboBox.SelectedValue = AssessmentSection.Composition;
            }
        }

        private void InitializeGridColumns()
        {
            probabilityDistributionGrid.CurrentCellDirtyStateChanged += DataGridViewCurrentCellDirtyStateChanged;

            var columnNameFormat = "column_{0}";

            var isRelevantName = TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.IsRelevant);
            isRelevantColumn = new DataGridViewCheckBoxColumn
            {
                DataPropertyName = isRelevantName,
                HeaderText = CommonGuiResources.FailureMechanismContributionView_GridColumn_RelevancyFilter,
                Name = string.Format(columnNameFormat, isRelevantName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            };

            var assessmentName = TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.Assessment);
            var assessmentColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = assessmentName,
                HeaderText = CommonGuiResources.FailureMechanismContributionView_GridColumn_Assessment,
                Name = string.Format(columnNameFormat, assessmentName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader,
                ReadOnly = true
            };

            var contributionName = TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.Contribution);
            var probabilityColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = contributionName,
                HeaderText = CommonGuiResources.FailureMechanismContributionView_GridColumn_Contribution,
                Name = string.Format(columnNameFormat, contributionName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                ReadOnly = true
            };

            var probabilitySpaceName = TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.ProbabilitySpace);
            probabilityPerYearColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = probabilitySpaceName,
                HeaderText = CommonGuiResources.FailureMechanismContributionView_GridColumn_ProbabilitySpace,
                Name = string.Format(columnNameFormat, probabilitySpaceName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 100,
                DefaultCellStyle =
                {
                    Format = "1/#,#"
                },
                ReadOnly = true
            };
            probabilityDistributionGrid.AutoGenerateColumns = false;
            probabilityDistributionGrid.Columns.AddRange(isRelevantColumn, assessmentColumn, probabilityColumn, probabilityPerYearColumn);
        }

        private void DataGridViewCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Ensure checkbox values are directly committed
            DataGridViewColumn currentColumn = probabilityDistributionGrid.Columns[probabilityDistributionGrid.CurrentCell.ColumnIndex];
            if (currentColumn is DataGridViewCheckBoxColumn)
            {
                probabilityDistributionGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void AssessmentSectionCompositionComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (revertingComboBoxSelectedValue)
            {
                return;
            }

            double[] originalFailureMechanismContributions = assessmentSection.GetFailureMechanisms().Select(fm => fm.Contribution).ToArray();

            var dialogResult = MessageBox.Show(RingtoetsIntegrationFormsResources.FailureMechanismContributionView_ChangeComposition_Change_will_clear_calculation_output_accept_question,
                                               CoreCommonBaseResources.Confirm,
                                               MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                assessmentSection.ChangeComposition((AssessmentSectionComposition) assessmentSectionCompositionComboBox.SelectedValue);
                SetGridDataSource();

                ClearCalculationOutputForChangedContributions(originalFailureMechanismContributions);
                assessmentSection.NotifyObservers();
            }
            else
            {
                revertingComboBoxSelectedValue = true;
                assessmentSectionCompositionComboBox.SelectedValue = assessmentSection.Composition;
                revertingComboBoxSelectedValue = false;
            }
        }

        private void ClearCalculationOutputForChangedContributions(double[] originalFailureMechanismContributions)
        {
            var allFailureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();
            for (int i = 0; i < allFailureMechanisms.Length; i++)
            {
                IFailureMechanism failureMechanism = allFailureMechanisms[i];
                if (originalFailureMechanismContributions[i] != failureMechanism.Contribution)
                {
                    foreach (ICalculation calculation in failureMechanism.Calculations)
                    {
                        calculation.ClearOutput();
                        calculation.NotifyObservers();
                    }
                }
            }
        }

        private void SetRowStyling()
        {
            foreach (DataGridViewRow row in probabilityDistributionGrid.Rows)
            {
                var isRelevantCell = (DataGridViewCheckBoxCell) row.Cells[isRelevantColumn.Index];
                FailureMechanismContributionItem rowData = data.Distribution.ElementAt(row.Index);
                isRelevantCell.ReadOnly = rowData.IsAlwaysRelevant;

                var isFailureMechanismRelevant = (bool) isRelevantCell.Value;
                SetRowStyle(isFailureMechanismRelevant, row);
            }
        }

        private void SetRowStyle(bool checkboxSelected, DataGridViewRow row)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (i != isRelevantColumn.Index)
                {
                    if (checkboxSelected)
                    {
                        SetCellStyle(row.Cells[i], Color.FromKnownColor(KnownColor.White), Color.FromKnownColor(KnownColor.ControlText));
                    }
                    else
                    {
                        SetCellStyle(row.Cells[i], Color.FromKnownColor(KnownColor.DarkGray), Color.FromKnownColor(KnownColor.GrayText));
                    }
                }
            }
        }

        private void SetCellStyle(DataGridViewCell cell, Color backgroundColor, Color textColor)
        {
            cell.Style.BackColor = backgroundColor;
            cell.Style.ForeColor = textColor;
        }

        private class FailureMechanismContributionItemRow
        {
            private readonly FailureMechanismContributionItem item;

            public FailureMechanismContributionItemRow(FailureMechanismContributionItem item)
            {
                this.item = item;
            }

            public string Assessment
            {
                get
                {
                    return item.Assessment;
                }
            }

            public double Contribution
            {
                get
                {
                    return item.Contribution;
                }
            }

            public int Norm
            {
                get
                {
                    return item.Norm;
                }
            }

            public double ProbabilitySpace
            {
                get
                {
                    return item.ProbabilitySpace;
                }
            }

            public bool IsRelevant
            {
                get
                {
                    return item.IsRelevant;
                }
                set
                {
                    item.IsRelevant = value;
                    item.NotifyFailureMechanismObservers();
                }
            }
        }
    }
}
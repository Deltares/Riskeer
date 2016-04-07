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
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.PresentationObjects;

using CommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="FailureMechanismContribution"/>, from which the <see cref="FailureMechanismContribution.Norm"/>
    /// can be updated and the <see cref="FailureMechanismContributionItem.Contribution"/> and <see cref="FailureMechanismContributionItem.ProbabilitySpace"/>
    /// can be seen in a grid.
    /// </summary>
    public partial class FailureMechanismContributionView : UserControl, IView, IObserver
    {
        private DataGridViewColumn probabilityPerYearColumn;
        private FailureMechanismContributionContext data;

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
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                HandleNewDataSet((FailureMechanismContributionContext)value);
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
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            UnsubscribeEvents();
            base.Dispose(disposing);
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
                var contributionItem = data.WrappedData.Distribution.ElementAt(e.RowIndex);
                if (contributionItem.Contribution == 0.0)
                {
                    e.Value = RingtoetsIntegrationFormsResources.FailureMechanismContributionView_ProbabilityPerYear_Not_applicable;
                    e.FormattingApplied = true;
                }
            }
        }

        private void HandleNewDataSet(FailureMechanismContributionContext value)
        {
            UnbindAssessmentSectionCompositionChange();
            UnbindNormChange();
            DetachFromData();

            data = value;

            SetGridDataSource();
            SetNormText();
            SetAssessmentSectionComposition();

            AttachToData();
            BindNormChange();
            BindAssessmentSectionCompositionChange();
        }

        private void SetGridDataSource()
        {
            if (data != null)
            {
                probabilityDistributionGrid.DataSource = data.WrappedData.Distribution;
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
            FailureMechanismContribution contribution = data.WrappedData;

            contribution.Norm = Convert.ToInt32(normInput.Value);
            contribution.NotifyObservers();
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
                normInput.Value = data.WrappedData.Norm;
            }
        }

        private void SetAssessmentSectionComposition()
        {
            if (data != null)
            {
                assessmentSectionCompositionComboBox.SelectedValue = data.Parent.Composition;
            }
        }

        private void InitializeGridColumns()
        {
            var assessmentName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(fmci => fmci.Assessment);
            var columnNameFormat = "column_{0}";
            var assessmentColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = assessmentName,
                HeaderText = CommonGuiResources.FailureMechanismContributionView_GridColumn_Assessment,
                Name = string.Format(columnNameFormat, assessmentName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader
            };

            var contributionName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(fmci => fmci.Contribution);
            var probabilityColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = contributionName,
                HeaderText = CommonGuiResources.FailureMechanismContributionView_GridColumn_Contribution,
                Name = string.Format(columnNameFormat, contributionName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            };

            var probabilitySpaceName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(fmci => fmci.ProbabilitySpace);
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
                }
            };
            probabilityDistributionGrid.AutoGenerateColumns = false;
            probabilityDistributionGrid.Columns.AddRange(assessmentColumn, probabilityColumn, probabilityPerYearColumn);
        }

        private void AssessmentSectionCompositionComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            IAssessmentSection assessmentSection = data.Parent;

            assessmentSection.ChangeComposition((AssessmentSectionComposition)assessmentSectionCompositionComboBox.SelectedValue);
            SetGridDataSource();
            assessmentSection.NotifyObservers();
        }
    }
}
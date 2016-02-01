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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;
using Ringtoets.Integration.Data.Contribution;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="FailureMechanismContribution"/>, from which the <see cref="FailureMechanismContribution.Norm"/>
    /// can be updated and the <see cref="FailureMechanismContributionItem.Contribution"/> and <see cref="FailureMechanismContributionItem.ProbabilitySpace"/>
    /// can be seen in a grid.
    /// </summary>
    public partial class FailureMechanismContributionView : UserControl, IView, IObserver
    {
        private FailureMechanismContribution data;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionView"/>.
        /// </summary>
        public FailureMechanismContributionView()
        {
            InitializeComponent();
            InitializeGridColumns();
            BindNormChange();
            BindNormInputLeave();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                SetNormValue((FailureMechanismContribution)value);
            }
        }

        public void UpdateObserver()
        {
            SetNormText();
            probabilityDistributionGrid.Invalidate();
        }

        private void SetNormValue(FailureMechanismContribution value)
        {
            UnbindNormChange();
            DetachFromData();

            data = value;

            SetGridDataSource();
            SetNormText();

            AttachToData();
            BindNormChange();
        }

        private void SetGridDataSource()
        {
            if (data != null)
            {
                probabilityDistributionGrid.DataSource = data.Distribution;
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

        private void InitializeGridColumns()
        {
            var assessmentName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(fmci => fmci.Assessment);
            var columnNameFormat = "column_{0}";
            var assessmentColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = assessmentName,
                HeaderText = Resources.FailureMechanismContributionView_GridColumn_Assessment,
                Name = string.Format(columnNameFormat, assessmentName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader
            };

            var contributionName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(fmci => fmci.Contribution);
            var probabilityColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = contributionName,
                HeaderText = Resources.FailureMechanismContributionView_GridColumn_Contribution,
                Name = string.Format(columnNameFormat, contributionName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            };

            var probabilitySpaceName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(fmci => fmci.ProbabilitySpace);
            var probabilityPerYearColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = probabilitySpaceName,
                HeaderText = Resources.FailureMechanismContributionView_GridColumn_ProbabilitySpace,
                Name = string.Format(columnNameFormat, probabilitySpaceName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 100
            };

            probabilityDistributionGrid.AutoGenerateColumns = false;
            probabilityDistributionGrid.Columns.AddRange(assessmentColumn, probabilityColumn, probabilityPerYearColumn);
        }
    }
}
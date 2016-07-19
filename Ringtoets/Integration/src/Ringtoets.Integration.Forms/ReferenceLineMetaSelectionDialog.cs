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
using Core.Common.Controls.Dialogs;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Forms.Properties;
using CommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection form a given set of <see cref="ReferenceLineMeta"/>. Upon
    /// closing of the dialog, the selected <see cref="ReferenceLineMeta"/> can be obtained.
    /// </summary>
    public partial class ReferenceLineMetaSelectionDialog : DialogBase
    {
        private enum SignalingLowerLimit
        {
            SignalingValue,
            LowerLimitValue
        }

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineMetaSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="referenceLineMetas">A list of <see cref="ReferenceLineMeta"/> the user can select.</param>
        public ReferenceLineMetaSelectionDialog(IWin32Window dialogParent, IEnumerable<ReferenceLineMeta> referenceLineMetas)
            : base(dialogParent, CommonFormsResources.SelectionDialogIcon, 410, 350)
        {
            if (referenceLineMetas == null)
            {
                throw new ArgumentNullException("referenceLineMetas");
            }
            InitializeComponent();
            InitializeSignalingLowerLimitComboBox();
            InitializeReferenceLineMetaDataGridViewControl(referenceLineMetas);
        }

        public ReferenceLineMeta SelectedReferenceLineMeta { get; private set; }

        public int? SelectedLimitValue { get; private set; }

        protected override Button GetCancelButton()
        {
            return Cancel;
        }

        private int? GetSelectedLimitValue()
        {
            var selectedRow = GetSelectedReferenceLineMetaSelectionRow();
            if (selectedRow == null)
            {
                return null;
            }

            var selectedItemInComboBox = (SignalingLowerLimit) SignalingLowerLimitComboBox.SelectedValue;
            return selectedItemInComboBox == SignalingLowerLimit.SignalingValue ?
                       selectedRow.SignalingValue :
                       selectedRow.LowerLimitValue;
        }

        private void InitializeReferenceLineMetaDataGridViewControl(IEnumerable<ReferenceLineMeta> referenceLineMetas)
        {
            ReferenceLineMetaDataGridViewControl.AddTextBoxColumn("AssessmentSectionId", Resources.ReferenceLineMetaSelectionDialog_ColumnHeader_AssessmentSectionId);
            ReferenceLineMetaDataGridViewControl.AddTextBoxColumn("SignalingValue", Resources.ReferenceLineMetaSelectionDialog_ColumnHeader_SignalingValue);
            ReferenceLineMetaDataGridViewControl.AddTextBoxColumn("LowerLimitValue", Resources.ReferenceLineMetaSelectionDialog_ColumnHeader_LowerLimitValue);
            ReferenceLineMetaDataGridViewControl.SetDataSource(referenceLineMetas.Select(rlm => new ReferenceLineMetaSelectionRow(rlm)).ToArray());
        }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            SetSelectionProperties();
            Close();
        }

        private void SetSelectionProperties()
        {
            ReferenceLineMetaSelectionRow referenceLineMetaSelectionRow = GetSelectedReferenceLineMetaSelectionRow();
            if (referenceLineMetaSelectionRow != null)
            {
                SelectedReferenceLineMeta = referenceLineMetaSelectionRow.ReferenceLineMeta;
                SelectedLimitValue = GetSelectedLimitValue();
            }
        }

        private void InitializeSignalingLowerLimitComboBox()
        {
            SignalingLowerLimitComboBox.DataSource = new[]
            {
                Tuple.Create(SignalingLowerLimit.SignalingValue, Resources.ReferenceLineMetaSelectionView_SignalingValue_DisplayName),
                Tuple.Create(SignalingLowerLimit.LowerLimitValue, Resources.ReferenceLineMetaSelectionView_LowerLimitValue_DisplayName)
            };
            SignalingLowerLimitComboBox.ValueMember = TypeUtils.GetMemberName<Tuple<AssessmentSectionComposition, string>>(t => t.Item1);
            SignalingLowerLimitComboBox.DisplayMember = TypeUtils.GetMemberName<Tuple<AssessmentSectionComposition, string>>(t => t.Item2);
        }

        private ReferenceLineMetaSelectionRow GetSelectedReferenceLineMetaSelectionRow()
        {
            var selectedRow = ReferenceLineMetaDataGridViewControl.GetCurrentRow();
            return selectedRow == null ? null : (ReferenceLineMetaSelectionRow) selectedRow.DataBoundItem;
        }

        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private class ReferenceLineMetaSelectionRow
        {
            public ReferenceLineMetaSelectionRow(ReferenceLineMeta referenceLineMeta)
            {
                AssessmentSectionId = referenceLineMeta.AssessmentSectionId;
                SignalingValue = referenceLineMeta.SignalingValue ?? 0;
                LowerLimitValue = referenceLineMeta.LowerLimitValue ?? 0;
                ReferenceLineMeta = referenceLineMeta;
            }

            public string AssessmentSectionId { get; private set; }
            public int SignalingValue { get; private set; }
            public int LowerLimitValue { get; private set; }
            public ReferenceLineMeta ReferenceLineMeta { get; private set; }
        }
    }
}
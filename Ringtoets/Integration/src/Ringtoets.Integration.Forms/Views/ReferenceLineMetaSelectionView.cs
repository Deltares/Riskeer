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
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// A <see cref="UserControl"/> which can be used to display a list of <see cref="ReferenceLineMeta"/>
    /// from which a selection can be made.
    /// </summary>
    public partial class ReferenceLineMetaSelectionView
    {
        private enum SignalingLowerLimit
        {
            SignalingValue,
            LowerLimitValue
        }

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineMetaSelectionView"/>.
        /// </summary>
        /// <param name="referenceLineMetas">A list of <see cref="ReferenceLineMeta"/> the user can select.</param>
        public ReferenceLineMetaSelectionView(IEnumerable<ReferenceLineMeta> referenceLineMetas)
        {
            if (referenceLineMetas == null)
            {
                throw new ArgumentNullException("referenceLineMetas");
            }
            InitializeComponent();

            InitializeSignalingLowerLimitComboBox();

            ReferenceLineMetaDataGrid.AutoGenerateColumns = false;
            ReferenceLineMetaDataGrid.DataSource = referenceLineMetas.Select(rlm => new ReferenceLineMetaSelectionRow(rlm)).ToArray();
        }

        public ReferenceLineMeta GetSelectedReferenceLineMeta()
        {
            var selectedRow = GetSelectedReferenceLineMetaSelectionRow();
            return selectedRow == null ? null : selectedRow.ReferenceLineMeta;
        }

        public int? GetSelectedLimitValue()
        {
            var selectedRow = GetSelectedReferenceLineMetaSelectionRow();
            if (selectedRow == null)
            {
                return null;
            }

            var selectedItemInComboBox = (SignalingLowerLimit)SignalingLowerLimitComboBox.SelectedValue;
            return selectedItemInComboBox == SignalingLowerLimit.SignalingValue ?
                       selectedRow.SignalingValue :
                       selectedRow.LowerLimitValue;
        }

        private ReferenceLineMetaSelectionRow GetSelectedReferenceLineMetaSelectionRow()
        {
            var selectedRows = ReferenceLineMetaDataGrid.SelectedRows;
            if (selectedRows.Count == 0)
            {
                return null;
            }

            var selectedRow = selectedRows[0];
            return (ReferenceLineMetaSelectionRow) selectedRow.DataBoundItem;
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

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e) {}

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
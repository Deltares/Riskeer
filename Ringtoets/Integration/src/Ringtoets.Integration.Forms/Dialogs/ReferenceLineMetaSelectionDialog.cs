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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Dialogs
{
    /// <summary>
    /// A dialog which allows the user to make a selection from a given set of <see cref="ReferenceLineMeta"/>. Upon
    /// closing of the dialog, the selected <see cref="ReferenceLineMeta"/> can be obtained.
    /// </summary>
    public partial class ReferenceLineMetaSelectionDialog : DialogBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineMetaSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="referenceLineMetas">A list of <see cref="ReferenceLineMeta"/> the user can select.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> or 
        /// <paramref name="referenceLineMetas"/> is <c>null</c>.</exception>
        public ReferenceLineMetaSelectionDialog(IWin32Window dialogParent, IEnumerable<ReferenceLineMeta> referenceLineMetas)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 372, 350)
        {
            if (referenceLineMetas == null)
            {
                throw new ArgumentNullException(nameof(referenceLineMetas));
            }

            InitializeComponent();
            InitializeReferenceLineMetaDataGridViewControl(referenceLineMetas);
        }

        /// <summary>
        /// Gets the <see cref="ReferenceLineMeta"/> from the selected row in the <see cref="DataGridViewControl"/>.
        /// </summary>
        public ReferenceLineMeta SelectedReferenceLineMeta { get; private set; }

        /// <summary>
        /// Gets the lower limit norm value from the selected row in the <see cref="DataGridViewControl"/>.
        /// </summary>
        public double SelectedLowerLimitNorm { get; private set; }

        /// <summary>
        /// Gets the signaling norm value from the selected row in the <see cref="DataGridViewControl"/>.
        /// </summary>
        public double SelectedSignalingNorm { get; private set; }

        /// <summary>
        /// Gets the norm type from the selected <see cref="RadioButton"/> in the dialog.
        /// </summary>
        public NormType SelectedNormativeNorm { get; private set; }

        protected override Button GetCancelButton()
        {
            return Cancel;
        }

        private void InitializeReferenceLineMetaDataGridViewControl(IEnumerable<ReferenceLineMeta> referenceLineMetas)
        {
            ReferenceLineMetaDataGridViewControl.AddTextBoxColumn("AssessmentSectionId", Resources.ReferenceLineMetaSelectionDialog_ColumnHeader_AssessmentSectionId);
            ReferenceLineMetaDataGridViewControl.AddTextBoxColumn("SignalingValue", Resources.ReferenceLineMetaSelectionDialog_ColumnHeader_SignalingValue);
            ReferenceLineMetaDataGridViewControl.AddTextBoxColumn("LowerLimitValue", Resources.ReferenceLineMetaSelectionDialog_ColumnHeader_LowerLimitValue);

            IOrderedEnumerable<ReferenceLineMetaSelectionRow> dataSource = referenceLineMetas.Select(rlm => new ReferenceLineMetaSelectionRow(rlm)).OrderBy(row => row.AssessmentSectionId, new AssessmentSectionIdComparer());
            ReferenceLineMetaDataGridViewControl.SetDataSource(dataSource.ToArray());
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

                SelectedNormativeNorm = SignallingValueRadioButton.Checked
                                            ? NormType.Signaling
                                            : NormType.LowerLimit;

                double lowerLimitNormValue = GetNormValue(referenceLineMetaSelectionRow.LowerLimitValueReturnPeriod);

                SelectedLowerLimitNorm = lowerLimitNormValue;
                SelectedSignalingNorm = referenceLineMetaSelectionRow.SignalingReturnPeriod.HasValue
                                            ? GetNormValue(referenceLineMetaSelectionRow.SignalingReturnPeriod.Value)
                                            : lowerLimitNormValue;
            }
        }

        private static double GetNormValue(int returnPeriod)
        {
            return 1.0 / returnPeriod;
        }

        private ReferenceLineMetaSelectionRow GetSelectedReferenceLineMetaSelectionRow()
        {
            DataGridViewRow selectedRow = ReferenceLineMetaDataGridViewControl.CurrentRow;
            return (ReferenceLineMetaSelectionRow) selectedRow?.DataBoundItem;
        }

        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private class AssessmentSectionIdComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int idX;
                string suffixX;
                int subX;

                int idY;
                string suffixY;
                int subY;

                SplitAssessmentSectionId(x, out idX, out suffixX, out subX);
                SplitAssessmentSectionId(y, out idY, out suffixY, out subY);

                if (idX != idY)
                {
                    return idX - idY;
                }

                if (string.IsNullOrEmpty(suffixX) != string.IsNullOrEmpty(suffixY))
                {
                    return string.IsNullOrEmpty(suffixX) ? -1 : 1;
                }

                if (!string.IsNullOrEmpty(suffixX) && suffixX != suffixY)
                {
                    return string.Compare(suffixX, suffixY, StringComparison.Ordinal);
                }

                return subX - subY;
            }

            private static void SplitAssessmentSectionId(string str, out int id, out string suffix, out int sub)
            {
                if (string.IsNullOrEmpty(str))
                {
                    id = 0;
                    sub = 0;
                    suffix = string.Empty;
                    return;
                }

                string[] parts = str.Split('-');
                string[] firstPart = Regex.Split(parts.First(), "([A-Za-z])");
                if (firstPart.Length > 1)
                {
                    int.TryParse(firstPart[0], out id);
                    suffix = firstPart[1];
                }
                else
                {
                    int.TryParse(parts[0], out id);
                    suffix = string.Empty;
                }

                if (parts.Length == 2)
                {
                    int.TryParse(parts[1], out sub);
                }
                else
                {
                    sub = 0;
                }
            }
        }

        private class ReferenceLineMetaSelectionRow
        {
            public ReferenceLineMetaSelectionRow(ReferenceLineMeta referenceLineMeta)
            {
                AssessmentSectionId = referenceLineMeta.AssessmentSectionId;
                ReferenceLineMeta = referenceLineMeta;

                SignalingValue = GetNormValue(referenceLineMeta.SignalingValue);
                if (SignalingValue != string.Empty)
                {
                    SignalingReturnPeriod = referenceLineMeta.SignalingValue;
                }

                LowerLimitValue = GetNormValue(referenceLineMeta.LowerLimitValue);
                if (LowerLimitValue != string.Empty)
                {
                    LowerLimitValueReturnPeriod = referenceLineMeta.LowerLimitValue;
                }
            }

            public string AssessmentSectionId { get; }
            public string SignalingValue { get; }
            public int? SignalingReturnPeriod { get; }
            public string LowerLimitValue { get; }
            public int LowerLimitValueReturnPeriod { get; }
            public ReferenceLineMeta ReferenceLineMeta { get; }

            private static string GetNormValue(int? returnPeriod)
            {
                return returnPeriod.HasValue && returnPeriod > 0
                           ? ProbabilityFormattingHelper.FormatFromReturnPeriod(returnPeriod.Value)
                           : string.Empty;
            }
        }
    }
}
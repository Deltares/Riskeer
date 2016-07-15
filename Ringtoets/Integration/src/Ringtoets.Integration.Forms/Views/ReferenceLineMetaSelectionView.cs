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

            SignalingLowerLimitComboBox.DataSource = GetSignalingLowerLimitComboBoxDataSource();

            ReferenceLineMetaDataGrid.AutoGenerateColumns = false;
            ReferenceLineMetaDataGrid.DataSource = referenceLineMetas.Select(rlm => new ReferenceLineMetaSelectionRow(rlm)).ToArray();
        }

        private static IList<string> GetSignalingLowerLimitComboBoxDataSource()
        {
            var datasourceList = new List<string>
            {
                Resources.ReferenceLineMetaSelectionView_SignalingValue_DisplayName,
                Resources.ReferenceLineMetaSelectionView_LowerLimitValue_DisplayName
            };
            return datasourceList;
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
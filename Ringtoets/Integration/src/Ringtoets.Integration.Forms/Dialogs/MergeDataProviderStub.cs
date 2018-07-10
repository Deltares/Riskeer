// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Merge;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Dialogs
{
    /// <summary>
    /// Stub for providing merge data.
    /// </summary>
    public partial class MergeDataProviderStub : DialogBase, IMergeDataProvider
    {
        /// <summary>
        /// Creates a new instance of <see cref="MergeDataProviderStub"/>.
        /// </summary>
        /// <param name="dialogParent"></param>
        public MergeDataProviderStub(IWin32Window dialogParent)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 160, 160)
        {
            InitializeComponent();
        }

        public AssessmentSection SelectedAssessmentSection { get; private set; }

        public IEnumerable<IFailureMechanism> SelectedFailureMechanisms { get; private set; }

        public bool SelectData(IEnumerable<AssessmentSection> assessmentSections)
        {
            if (ShowDialog() == DialogResult.OK)
            {
                SelectedAssessmentSection = assessmentSections?.First();
                SelectedFailureMechanisms = assessmentSections != null
                                                ? new List<IFailureMechanism>
                                                {
                                                    SelectedAssessmentSection.Piping
                                                }
                                                : null;
                return true;
            }

            return false;
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }
    }
}
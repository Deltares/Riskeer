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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Service.Merge;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Dialogs
{
    /// <summary>
    /// Stub for providing assessment sections to merge.
    /// </summary>
    public partial class AssessmentSectionProviderStub : DialogBase, IAssessmentSectionProvider
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionProviderStub"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> is <c>null</c>.</exception>
        public AssessmentSectionProviderStub(IWin32Window dialogParent)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 1, 1)
        {
            InitializeComponent();
        }

        public IEnumerable<AssessmentSection> GetAssessmentSections(string filePath)
        {
            if (ShowDialog() == DialogResult.Cancel)
            {
                return null;
            }

            return new List<AssessmentSection>();
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }
    }
}
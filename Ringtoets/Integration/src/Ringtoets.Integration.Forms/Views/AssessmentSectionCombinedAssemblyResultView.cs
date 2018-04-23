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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// The view for the combined assembly result for all failure mechanisms of 
    /// the <see cref="AssessmentSection"/>. 
    /// </summary>
    public partial class AssessmentSectionCombinedAssemblyResultView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCombinedAssemblyResultView"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create the view for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionCombinedAssemblyResultView(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();
            InitializeDataGridView();
        }

        public object Data { get; set; }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(null, "Toetsspoor", true);
            dataGridViewControl.AddTextBoxColumn(null, "Label", true);
            dataGridViewControl.AddTextBoxColumn(null, "Groep", true);
        }
    }
}
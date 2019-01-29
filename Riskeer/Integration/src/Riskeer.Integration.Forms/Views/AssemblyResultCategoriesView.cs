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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Riskeer.Common.Forms.Helpers;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// View to show a collection of <see cref="FailureMechanismAssemblyCategory"/>.
    /// </summary>
    public partial class AssemblyResultCategoriesView : UserControl, IView
    {
        private readonly Func<IEnumerable<FailureMechanismAssemblyCategory>> getAssemblyCategoriesFunc;

        private readonly Observer failureMechanismContributionObserver;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultCategoriesView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <param name="getAssemblyCategoriesFunc">The func to get the assembly categories from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssemblyResultCategoriesView(AssessmentSection assessmentSection,
                                            Func<IEnumerable<FailureMechanismAssemblyCategory>> getAssemblyCategoriesFunc)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getAssemblyCategoriesFunc == null)
            {
                throw new ArgumentNullException(nameof(getAssemblyCategoriesFunc));
            }

            AssessmentSection = assessmentSection;
            this.getAssemblyCategoriesFunc = getAssemblyCategoriesFunc;

            InitializeComponent();

            failureMechanismContributionObserver = new Observer(UpdateTableData)
            {
                Observable = assessmentSection.FailureMechanismContribution
            };

            UpdateTableData();
        }

        public AssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                failureMechanismContributionObserver.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateTableData()
        {
            assemblyCategoriesTable.SetData(
                getAssemblyCategoriesFunc().Select(
                    category => new Tuple<AssemblyCategory, Color, FailureMechanismAssemblyCategoryGroup>(
                        category,
                        AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(category.Group),
                        category.Group)).ToArray());
        }
    }
}
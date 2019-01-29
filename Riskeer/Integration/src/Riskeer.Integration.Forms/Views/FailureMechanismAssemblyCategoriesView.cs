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
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// View for an <see cref="IFailureMechanism"/> to show 
    /// its assembly categories.
    /// </summary>
    public partial class FailureMechanismAssemblyCategoriesView : CloseForFailureMechanismView
    {
        private readonly Func<IEnumerable<FailureMechanismAssemblyCategory>> getFailureMechanismAssemblyCategoriesFunc;
        private readonly Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionAssemblyCategoriesFunc;
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoriesView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism belonging to the view.</param>
        /// <param name="assessmentSection">The assessment section belonging to the view.</param>
        /// <param name="getFailureMechanismAssemblyCategoriesFunc">The function to get a collection
        /// of <see cref="FailureMechanismAssemblyCategory"/>.</param>
        /// <param name="getFailureMechanismSectionAssemblyCategoriesFunc">The function to get a collection
        /// of <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismAssemblyCategoriesView(IFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection,
                                                      Func<IEnumerable<FailureMechanismAssemblyCategory>> getFailureMechanismAssemblyCategoriesFunc,
                                                      Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionAssemblyCategoriesFunc)
            : base(failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getFailureMechanismAssemblyCategoriesFunc == null)
            {
                throw new ArgumentNullException(nameof(getFailureMechanismAssemblyCategoriesFunc));
            }

            if (getFailureMechanismSectionAssemblyCategoriesFunc == null)
            {
                throw new ArgumentNullException(nameof(getFailureMechanismSectionAssemblyCategoriesFunc));
            }

            this.getFailureMechanismAssemblyCategoriesFunc = getFailureMechanismAssemblyCategoriesFunc;
            this.getFailureMechanismSectionAssemblyCategoriesFunc = getFailureMechanismSectionAssemblyCategoriesFunc;

            InitializeComponent();

            failureMechanismObserver = new Observer(UpdateTableData)
            {
                Observable = failureMechanism
            };

            assessmentSectionObserver = new Observer(UpdateTableData)
            {
                Observable = assessmentSection
            };

            UpdateTableData();
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            assessmentSectionObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateTableData()
        {
            failureMechanismAssemblyCategoriesTable.SetData(
                getFailureMechanismAssemblyCategoriesFunc().Select(
                    category => new Tuple<AssemblyCategory, Color, FailureMechanismAssemblyCategoryGroup>(
                        category,
                        AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(category.Group),
                        category.Group)).ToArray());

            failureMechanismSectionAssemblyCategoriesTable.SetData(
                getFailureMechanismSectionAssemblyCategoriesFunc().Select(
                    category => new Tuple<AssemblyCategory, Color, FailureMechanismSectionAssemblyCategoryGroup>(
                        category,
                        AssemblyCategoryGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(category.Group),
                        category.Group)).ToArray());
        }
    }
}
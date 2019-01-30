// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.Integration.Data.StandAlone;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// View for a <see cref="MacroStabilityOutwardsFailureMechanism"/> to show
    /// its assembly categories.
    /// </summary>
    public partial class MacroStabilityOutwardsAssemblyCategoriesView : CloseForFailureMechanismView
    {
        private readonly Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionAssemblyCategoriesFunc;
        private readonly Observer failureMechanismObserver;
        private readonly Observer assessmentSectionObserver;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityOutwardsAssemblyCategoriesView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism belonging to the view.</param>
        /// <param name="assessmentSection">The assessment section belonging to the view.</param>
        /// <param name="getFailureMechanismSectionAssemblyCategoriesFunc">The function to get a collection
        /// of <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityOutwardsAssemblyCategoriesView(MacroStabilityOutwardsFailureMechanism failureMechanism,
                                                            IAssessmentSection assessmentSection,
                                                            Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionAssemblyCategoriesFunc)
            : base(failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getFailureMechanismSectionAssemblyCategoriesFunc == null)
            {
                throw new ArgumentNullException(nameof(getFailureMechanismSectionAssemblyCategoriesFunc));
            }

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
            failureMechanismSectionAssemblyCategoriesTable.SetData(
                getFailureMechanismSectionAssemblyCategoriesFunc().Select(
                    category => new Tuple<AssemblyCategory, Color, FailureMechanismSectionAssemblyCategoryGroup>(
                        category,
                        AssemblyCategoryGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(category.Group),
                        category.Group)).ToArray());
        }
    }
}
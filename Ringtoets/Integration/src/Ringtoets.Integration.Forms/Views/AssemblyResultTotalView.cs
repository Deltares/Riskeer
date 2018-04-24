﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Controls.Views;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Properties;
using CommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// The view for the combined assembly result for all failure mechanisms of 
    /// the <see cref="AssessmentSection"/>. 
    /// </summary>
    public partial class AssemblyResultTotalView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultTotalView"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create the view for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssemblyResultTotalView(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;

            InitializeComponent();
            InitializeDataGridView();
            SetData();
        }

        /// <summary>
        /// Gets the <see cref="AssessmentSection"/> the view belongs to.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRow.Name),
                                                 CommonGuiResources.FailureMechanismContributionView_GridColumn_Assessment,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRow.Code),
                                                 RingtoetsCommonFormsResources.FailureMechanism_Code_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRow.Group),
                                                 RingtoetsCommonFormsResources.FailureMechanism_Group_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRow.CategoryGroup),
                                                 Resources.AssemblyCategory_Group_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRow.Probablity),
                                                 Resources.AssemblyResultTotalView_Probability_DisplayName,
                                                 true);
        }

        private void SetData()
        {
            var rows = new List<FailureMechanismAssemblyResultRow>
            {
                new FailureMechanismAssemblyResultRow(AssessmentSection.Piping, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.GrassCoverErosionInwards, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.MacroStabilityInwards, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.MacroStabilityOutwards, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.Microstability, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.StabilityStoneCover, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.WaveImpactAsphaltCover, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.WaterPressureAsphaltCover, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.GrassCoverErosionOutwards, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.GrassCoverSlipOffOutwards, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.GrassCoverSlipOffInwards, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.HeightStructures, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.ClosingStructures, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.PipingStructure, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.StabilityPointStructures, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.StrengthStabilityLengthwiseConstruction, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.DuneErosion, GetFailureMechanismAssembly),
                new FailureMechanismAssemblyResultRow(AssessmentSection.TechnicalInnovation, GetFailureMechanismAssembly)
            };

            dataGridViewControl.SetDataSource(rows);
        }

        private static FailureMechanismAssembly GetFailureMechanismAssembly()
        {
            var random = new Random();
            return new FailureMechanismAssembly(random.NextDouble(), FailureMechanismAssemblyCategoryGroup.IIIt);
        }
    }
}
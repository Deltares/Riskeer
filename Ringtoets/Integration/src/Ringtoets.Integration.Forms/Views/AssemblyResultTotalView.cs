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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
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
        private IEnumerable<FailureMechanismAssemblyResultRowBase> assemblyResultRows;

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
            LocalizeControl();
            InitializeDataGridView();
        }

        /// <summary>
        /// Gets the <see cref="AssessmentSection"/> the view belongs to.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        private void LocalizeControl()
        {
            RefreshAssemblyResultsButton.Text = Resources.AssemblyResultTotalView_RefreshAssemblyResultsButton_Text;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Name),
                                                 CommonGuiResources.FailureMechanismContributionView_GridColumn_Assessment,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Code),
                                                 RingtoetsCommonFormsResources.FailureMechanism_Code_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Group),
                                                 RingtoetsCommonFormsResources.FailureMechanism_Group_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.CategoryGroup),
                                                 Resources.AssemblyCategory_Group_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Probablity),
                                                 Resources.AssemblyResultTotalView_Probability_DisplayName,
                                                 true);

            InitializeRows();
        }

        private void InitializeRows()
        {
            assemblyResultRows = new List<FailureMechanismAssemblyResultRowBase>
            {
                CreatePipingFailureMechanismAssemblyResultRow(),
                CreateGrassCoverErosionInwardsFailureMechanismAssemblyResultRow(),
                CreateMacroStabilityInwardsFailureMechanismAssemblyResultRow(),
                CreateMacroStabilityOutwardsFailureMechanismAssemblyResultRow(),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.Microstability, GetFailureMechanismAssemblyCategoryGroup),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.StabilityStoneCover, GetFailureMechanismAssemblyCategoryGroup),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.WaveImpactAsphaltCover, GetFailureMechanismAssemblyCategoryGroup),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.WaterPressureAsphaltCover, GetFailureMechanismAssemblyCategoryGroup),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.GrassCoverErosionOutwards, GetFailureMechanismAssemblyCategoryGroup),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.GrassCoverSlipOffOutwards, GetFailureMechanismAssemblyCategoryGroup),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.GrassCoverSlipOffInwards, GetFailureMechanismAssemblyCategoryGroup),
                CreateHeightStructuresFailureMechanismAssemblyResultRow(),
                CreateClosingStructuresFailureMechanismAssemblyResultRow(),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.PipingStructure, GetFailureMechanismAssemblyCategoryGroup),
                CreateStabilityPointsStructuresFailureMechanismAssemblyResultRow(),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.StrengthStabilityLengthwiseConstruction, GetFailureMechanismAssemblyCategoryGroup),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.DuneErosion, GetFailureMechanismAssemblyCategoryGroup),
                new FailureMechanismAssemblyCategoryGroupResultRow(AssessmentSection.TechnicalInnovation, GetFailureMechanismAssemblyCategoryGroup)
            };

            dataGridViewControl.SetDataSource(assemblyResultRows);
        }

        private FailureMechanismAssemblyResultRowBase CreateMacroStabilityOutwardsFailureMechanismAssemblyResultRow()
        {
            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = AssessmentSection.MacroStabilityOutwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(macroStabilityOutwards, GetFailureMechanismAssemblyCategoryGroup);
        }

        private FailureMechanismAssemblyResultRowBase CreateMacroStabilityInwardsFailureMechanismAssemblyResultRow()
        {
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = AssessmentSection.MacroStabilityInwards;
            return new FailureMechanismAssemblyResultRow(macroStabilityInwards,
                                                         () => MacroStabilityInwardsFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(macroStabilityInwards.SectionResults,
                                                                                                                                                          macroStabilityInwards.Calculations.Cast<MacroStabilityInwardsCalculationScenario>(),
                                                                                                                                                          macroStabilityInwards,
                                                                                                                                                          AssessmentSection));
        }

        private FailureMechanismAssemblyResultRowBase CreatePipingFailureMechanismAssemblyResultRow()
        {
            PipingFailureMechanism piping = AssessmentSection.Piping;
            return new FailureMechanismAssemblyResultRow(piping,
                                                         () => PipingFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(piping.SectionResults,
                                                                                                                                           piping.Calculations.Cast<PipingCalculationScenario>(),
                                                                                                                                           piping,
                                                                                                                                           AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateStabilityPointsStructuresFailureMechanismAssemblyResultRow()
        {
            StabilityPointStructuresFailureMechanism stabilityPointStructures = AssessmentSection.StabilityPointStructures;
            return new FailureMechanismAssemblyResultRow(stabilityPointStructures,
                                                         () => StabilityPointStructuresFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(stabilityPointStructures.SectionResults,
                                                                                                                                                             stabilityPointStructures,
                                                                                                                                                             AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateClosingStructuresFailureMechanismAssemblyResultRow()
        {
            ClosingStructuresFailureMechanism closingStructures = AssessmentSection.ClosingStructures;
            return new FailureMechanismAssemblyResultRow(closingStructures,
                                                         () => ClosingStructuresFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(closingStructures.SectionResults,
                                                                                                                                                      closingStructures,
                                                                                                                                                      AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateHeightStructuresFailureMechanismAssemblyResultRow()
        {
            HeightStructuresFailureMechanism heightStructures = AssessmentSection.HeightStructures;
            return new FailureMechanismAssemblyResultRow(heightStructures,
                                                         () => HeightStructuresFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(heightStructures.SectionResults,
                                                                                                                                                     heightStructures,
                                                                                                                                                     AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateGrassCoverErosionInwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = AssessmentSection.GrassCoverErosionInwards;
            return new FailureMechanismAssemblyResultRow(grassCoverErosionInwards,
                                                         () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleFailureMechanism(grassCoverErosionInwards.SectionResults,
                                                                                                                                                             grassCoverErosionInwards,
                                                                                                                                                             AssessmentSection));
        }

        private static FailureMechanismAssemblyCategoryGroup GetFailureMechanismAssemblyCategoryGroup()
        {
            return FailureMechanismAssemblyCategoryGroup.IIIt;
        }

        private void RefreshAssemblyResults_Click(object sender, EventArgs e)
        {
            assemblyResultRows.ForEachElementDo(row => row.Update());
            dataGridViewControl.RefreshDataGridView();
        }
    }
}
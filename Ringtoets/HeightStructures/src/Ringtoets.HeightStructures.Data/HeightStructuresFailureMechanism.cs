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
using Core.Common.Base;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Failure mechanism for Height structures.
    /// </summary>
    public class HeightStructuresFailureMechanism : FailureMechanismBase, ICalculatableFailureMechanism, IHasSectionResults<HeightStructuresFailureMechanismSectionResult>
    {
        private readonly IList<HeightStructuresFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Creates a new instance of the <see cref="HeightStructuresFailureMechanism"/> class.
        /// </summary>
        public HeightStructuresFailureMechanism()
            : base(Resources.HeightStructuresFailureMechanism_DisplayName, Resources.HeightStructuresFailureMechanism_Code)
        {
            sectionResults = new List<HeightStructuresFailureMechanismSectionResult>();
            CalculationsGroup = new CalculationGroup(RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName, false);
            GeneralInput = new GeneralHeightStructuresInput();
            HeightStructures = new ObservableList<HeightStructure>();
            HeightStructuresCollection = new StructureCollection<HeightStructure>();
            ForeshoreProfiles = new ForeshoreProfileCollection();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return CalculationsGroup.GetCalculations().Cast<StructuresCalculation<HeightStructuresInput>>();
            }
        }

        /// <summary>
        /// Gets the height structures calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralHeightStructuresInput GeneralInput { get; private set; }

        /// <summary>
        /// Gets the available height structure of this instance.
        /// </summary>
        public ObservableList<HeightStructure> HeightStructures { get; private set; }

        /// <summary>
        /// Gets the available height structures of this instance.
        /// </summary>
        public StructureCollection<HeightStructure> HeightStructuresCollection { get; private set; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ForeshoreProfileCollection ForeshoreProfiles { get; private set; }

        /// <summary>
        /// Gets the container of all calculations.
        /// </summary>
        public CalculationGroup CalculationsGroup { get; }

        public IEnumerable<HeightStructuresFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            sectionResults.Add(new HeightStructuresFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }
    }
}
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
    public class HeightStructuresFailureMechanism : FailureMechanismBase,
                                                    ICalculatableFailureMechanism,
                                                    IHasSectionResults<HeightStructuresFailureMechanismSectionResult>
    {
        private readonly ObservableList<HeightStructuresFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Creates a new instance of the <see cref="HeightStructuresFailureMechanism"/> class.
        /// </summary>
        public HeightStructuresFailureMechanism()
            : base(Resources.HeightStructuresFailureMechanism_DisplayName, Resources.HeightStructuresFailureMechanism_Code, 1)
        {
            sectionResults = new ObservableList<HeightStructuresFailureMechanismSectionResult>();
            CalculationsGroup = new CalculationGroup
            {
                Name = RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName
            };
            GeneralInput = new GeneralHeightStructuresInput();
            HeightStructures = new StructureCollection<HeightStructure>();
            ForeshoreProfiles = new ForeshoreProfileCollection();
        }

        /// <summary>
        /// Gets the height structures calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralHeightStructuresInput GeneralInput { get; }

        /// <summary>
        /// Gets the available height structures of this instance.
        /// </summary>
        public StructureCollection<HeightStructure> HeightStructures { get; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ForeshoreProfileCollection ForeshoreProfiles { get; }

        /// <summary>
        /// Gets the container of all calculations.
        /// </summary>
        public CalculationGroup CalculationsGroup { get; }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return CalculationsGroup.GetCalculations().Cast<StructuresCalculation<HeightStructuresInput>>();
            }
        }

        public IObservableEnumerable<HeightStructuresFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);
            sectionResults.Add(new HeightStructuresFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionResults()
        {
            sectionResults.Clear();
        }
    }
}
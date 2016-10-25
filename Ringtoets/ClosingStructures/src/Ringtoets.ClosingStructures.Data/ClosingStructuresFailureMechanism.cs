﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.ClosingStructures.Data.Properties;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.ClosingStructures.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Closing Structures failure mechanism.
    /// </summary>
    public class ClosingStructuresFailureMechanism : FailureMechanismBase, ICalculatableFailureMechanism, IHasSectionResults<ClosingStructuresFailureMechanismSectionResult>
    {
        private readonly List<ClosingStructuresFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingStructuresFailureMechanism"/> class.
        /// </summary>
        public ClosingStructuresFailureMechanism()
            : base(Resources.ClosingStructuresFailureMechanism_DisplayName, Resources.ClosingStructuresFailureMechanism_Code)
        {
            CalculationsGroup = new CalculationGroup(RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName, false);
            GeneralInput = new GeneralClosingStructuresInput();
            ClosingStructures = new ObservableList<ClosingStructure>();
            sectionResults = new List<ClosingStructuresFailureMechanismSectionResult>();
            ForeshoreProfiles = new ObservableList<ForeshoreProfile>();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return CalculationsGroup.GetCalculations().OfType<StructuresCalculation<ClosingStructuresInput>>();
            }
        }

        /// <summary>
        /// Gets the general closing structures calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralClosingStructuresInput GeneralInput { get; private set; }

        /// <summary>
        /// Gets the available closing structures  for this instance.
        /// </summary>
        public ObservableList<ClosingStructure> ClosingStructures { get; private set; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ObservableList<ForeshoreProfile> ForeshoreProfiles { get; private set; }

        public CalculationGroup CalculationsGroup { get; private set; }

        public IEnumerable<ClosingStructuresFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            sectionResults.Add(new ClosingStructuresFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }
    }
}
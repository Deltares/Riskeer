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

using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.DuneErosion.Data.Properties;

namespace Ringtoets.DuneErosion.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Dune Erosion failure mechanism.
    /// </summary>
    public class DuneErosionFailureMechanism : FailureMechanismBase, IHasSectionResults<DuneErosionFailureMechanismSectionResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuneErosionFailureMechanism"/> class.
        /// </summary>
        public DuneErosionFailureMechanism()
            : base(Resources.DuneErosionFailureMechanism_DisplayName, Resources.DuneErosionFailureMechanism_Code)
        {
            SectionResults = new ObservableList<DuneErosionFailureMechanismSectionResult>();
            GeneralInput = new GeneralDuneErosionInput();
            DuneLocations = new ObservableList<DuneLocation>();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the general dune erosion calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralDuneErosionInput GeneralInput { get; }

        /// <summary>
        /// Gets the dune locations.
        /// </summary>
        public ObservableList<DuneLocation> DuneLocations { get; }

        public ObservableList<DuneErosionFailureMechanismSectionResult> SectionResults { get; }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            SectionResults.Add(new DuneErosionFailureMechanismSectionResult(section));
            SectionResults.NotifyObservers();
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            SectionResults.Clear();
            SectionResults.NotifyObservers();
        }
    }
}
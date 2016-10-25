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

using System;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Forms;

namespace Ringtoets.ClosingStructures.Forms.Views
{
    /// <summary>
    /// Container of a <see cref="ClosingStructuresFailureMechanismSectionResult"/>,
    /// which takes care of the representation of properties in a grid.
    /// </summary>
    public class ClosingStructuresScenarioRow : IScenarioRow<ClosingStructuresCalculation>
    {
        private readonly ClosingStructuresFailureMechanismSectionResult sectionResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingStructuresScenarioRow"/> class.
        /// </summary>
        /// <param name="sectionResult">The section result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public ClosingStructuresScenarioRow(ClosingStructuresFailureMechanismSectionResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }
            this.sectionResult = sectionResult;
        }

        public string Name
        {
            get
            {
                return sectionResult.Section.Name;
            }
        }

        public ClosingStructuresCalculation Calculation
        {
            get
            {
                return sectionResult.Calculation;
            }
            set
            {
                sectionResult.Calculation = value;
                sectionResult.NotifyObservers();
            }
        }
    }
}
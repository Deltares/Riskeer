﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Views;

namespace Riskeer.ClosingStructures.Forms.Views
{
    /// <summary>
    /// View for configuring closing structures scenarios.
    /// </summary>
    public class ClosingStructuresScenariosView : ScenariosView<StructuresCalculationScenario<ClosingStructuresInput>, ClosingStructuresInput, ClosingStructuresScenarioRow, ClosingStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresScenariosView"/>.
        /// </summary>
        /// <param name="calculationGroup">The data to show in this view.</param>
        /// <param name="failureMechanism">The <see cref="ClosingStructuresFailureMechanism"/>
        /// the <paramref name="calculationGroup"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public ClosingStructuresScenariosView(CalculationGroup calculationGroup, ClosingStructuresFailureMechanism failureMechanism)
            : base(calculationGroup, failureMechanism) {}

        protected override ClosingStructuresInput GetCalculationInput(StructuresCalculationScenario<ClosingStructuresInput> calculationScenario)
        {
            return calculationScenario.InputParameters;
        }

        protected override IEnumerable<ClosingStructuresScenarioRow> GetScenarioRows(FailureMechanismSection failureMechanismSection)
        {
            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<StructuresCalculationScenario<ClosingStructuresInput>> calculations = CalculationGroup.GetCalculations()
                                                                                                              .OfType<StructuresCalculationScenario<ClosingStructuresInput>>()
                                                                                                              .Where(cs => cs.IsStructureIntersectionWithReferenceLineInSection(lineSegments));

            return calculations.Select(c => new ClosingStructuresScenarioRow(c)).ToList();
        }
    }
}
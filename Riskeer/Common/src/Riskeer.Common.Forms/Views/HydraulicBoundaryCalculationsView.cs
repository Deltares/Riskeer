// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.GuiServices;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view for <see cref="HydraulicBoundaryLocationCalculation"/> views which should be derived in
    /// order to get a consistent look and feel.
    /// </summary>
    public abstract partial class HydraulicBoundaryCalculationsView : LocationCalculationsView<HydraulicBoundaryLocationCalculation>
    {
        private readonly Observer calculationsObserver;
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> calculationObserver;

        private readonly IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationsView"/>.
        /// </summary>
        /// <param name="calculations">The calculations to show in the view.</param>
        /// <param name="assessmentSection">The assessment section which the calculations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        protected HydraulicBoundaryCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                    IAssessmentSection assessmentSection) : base(calculations, assessmentSection)
        {
            UpdateDataGridViewDataSource();
        }
    }
}
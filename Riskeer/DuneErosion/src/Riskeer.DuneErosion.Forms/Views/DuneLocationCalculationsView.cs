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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.GuiServices;
using Riskeer.DuneErosion.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// View for presenting and performing dune location calculations.
    /// </summary>
    public partial class DuneLocationCalculationsView : DuneLocationCalculationsViewBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsView"/>.
        /// </summary>
        /// <param name="calculations">The calculations to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism which the calculations belong to.</param>
        /// <param name="assessmentSection">The assessment section which the calculations belong to.</param>
        /// <param name="getTargetProbabilityFunc"><see cref="Func{TResult}"/> for getting the target probability to use during calculations.</param>
        /// <param name="getCalculationIdentifierFunc"><see cref="Func{TResult}"/> for getting the calculation identifier to use in all messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneLocationCalculationsView(IObservableEnumerable<DuneLocationCalculation> calculations,
                                            DuneErosionFailureMechanism failureMechanism,
                                            IAssessmentSection assessmentSection,
                                            Func<double> getTargetProbabilityFunc,
                                            Func<string> getCalculationIdentifierFunc) : base(calculations, failureMechanism, assessmentSection, getTargetProbabilityFunc, getCalculationIdentifierFunc)
        {
            InitializeComponent();
        }
    }
}
﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.MapLayers;
using Riskeer.Piping.Data;

namespace Riskeer.Piping.Forms.Views.RegistrationState
{
    /// <summary>
    /// Registration state view showing map data for a piping failure mechanism.
    /// </summary>
    public class PipingFailureMechanismView : CalculationsState.PipingFailureMechanismView
    {
        private CalculatableFailureMechanismSectionResultsMapLayer<PipingFailureMechanism, AdoptableFailureMechanismSectionResult, PipingInput> assemblyResultMapLayer;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the data for.</param>
        /// <param name="assessmentSection">The assessment section to show the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismView(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(failureMechanism, assessmentSection) {}

        protected override void Dispose(bool disposing)
        {
            assemblyResultMapLayer.Dispose();

            base.Dispose(disposing);
        }

        protected override void CreateMapData()
        {
            base.CreateMapData();

            assemblyResultMapLayer = new CalculatableFailureMechanismSectionResultsMapLayer<PipingFailureMechanism, AdoptableFailureMechanismSectionResult, PipingInput>(
                FailureMechanism, sr => PipingFailureMechanismAssemblyFactory.AssembleSection(sr, FailureMechanism, AssessmentSection).AssemblyResult);
            MapDataCollection.Insert(4, assemblyResultMapLayer.MapData);
        }
    }
}
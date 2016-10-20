// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Service;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Calculation.Activities;

namespace Ringtoets.ClosingStructures.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a structures closure calculation.
    /// </summary>
    public class ClosingStructuresCalculationActivity : HydraRingActivityBase
    {
        private readonly ClosingStructuresCalculation calculation;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The height structures data used for the calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public ClosingStructuresCalculationActivity(ClosingStructuresCalculation calculation, string hlcdDirectory,
                                                    ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            if (hlcdDirectory == null)
            {
             throw new ArgumentNullException("hlcdDirectory");   
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            this.calculation = calculation;

            Name = calculation.Name;
        }

        protected override void PerformCalculation()
        {
            throw new NotImplementedException();
        }

        protected override void OnCancel()
        {
            throw new NotImplementedException();
        }

        protected override void OnFinish()
        {
            calculation.NotifyObservers();
        }
    }
}
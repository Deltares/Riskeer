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

namespace Ringtoets.HydraRing.Calculation
{
    /// <summary>
    /// Container for configuration settings on a per <see cref="FailureMechanismType"/> basis.
    /// </summary>
    public class HydraRingConfigurationSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public HydraRingFailureMechanismType FailureMechanismType { get; set; }

        /// <summary>
        /// Gets or sets the id of the sub mechanism.
        /// </summary>
        public int SubMechanismId { get; set; }

        /// <summary>
        /// Gets or sets the id of the calculation technique that should be used.
        /// </summary>
        public int CalculationTechniqueId { get; set; }

        /// <summary>
        /// Gets or set the FORM start method.
        /// </summary>
        public int FormStartMethod { get; set; }

        /// <summary>
        /// Gets or set the maximum number of FORM iterations to perform.
        /// </summary>
        public int FormNumberOfIterations { get; set; }

        /// <summary>
        /// Gets or set a relaxation towards the design point for FORM.
        /// </summary>
        public double FormRelaxationFactor { get; set; }

        /// <summary>
        /// Gets or set one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsHOH"/>
        /// <seealso cref="FormEpsZFunc"/>
        public double FormEpsBeta { get; set; }

        /// <summary>
        /// Gets or set one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsBeta"/>
        /// <seealso cref="FormEpsZFunc"/>
        public double FormEpsHOH { get; set; }

        /// <summary>
        /// Gets or set one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsBeta"/>
        /// <seealso cref="FormEpsHOH"/>
        public double FormEpsZFunc { get; set; }

        /// <summary>
        /// Gets or set the DIRS start method.
        /// </summary>
        public int DsStartMethod { get; set; }

        /// <summary>
        /// Gets or set the minimum number of DIRS iterations to perform.
        /// </summary>
        public int DsMinNumberOfIterations { get; set; }

        /// <summary>
        /// Gets or set the maximum number of DIRS iterations to perform.
        /// </summary>
        public int DsMaxNumberOfIterations { get; set; }

        /// <summary>
        /// Gets or set the variation coefficient to use within the DIRS iterations.
        /// </summary>
        public double DsVarCoefficient { get; set; }

        /// <summary>
        /// Gets or set the lower size limit of a uniform grid deployed by NINT.
        /// </summary>
        public double NiUMin { get; set; }

        /// <summary>
        /// Gets or set the upper size limit of a uniform grid deployed by NINT.
        /// </summary>
        public double NiUMax { get; set; }

        /// <summary>
        /// Gets or set the number of steps between <seealso cref="NiUMin"/> and <seealso cref="NiUMax"/> for NINT.
        /// </summary>
        public int NiNumberSteps { get; set; }
    }
}
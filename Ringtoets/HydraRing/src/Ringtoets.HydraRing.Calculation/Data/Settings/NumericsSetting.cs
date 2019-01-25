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

namespace Riskeer.HydraRing.Calculation.Data.Settings
{
    /// <summary>
    /// Container for numerics settings.
    /// </summary>
    public class NumericsSetting
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NumericsSetting"/> class.
        /// </summary>
        /// <param name="calculationTechniqueId">The id of the calculation technique that should be used.</param>
        /// <param name="formStartMethod">The FORM start method.</param>
        /// <param name="formNumberOfIterations">The maximum number of FORM iterations to perform.</param>
        /// <param name="formRelaxationFactor">A relaxation towards the design point for FORM.</param>
        /// <param name="formEpsBeta">One of the three convergence criteria for FORM.</param>
        /// <param name="formEpsHoh">One of the three convergence criteria for FORM.</param>
        /// <param name="formEpsZFunc">One of the three convergence criteria for FORM.</param>
        /// <param name="dsStartMethod">The DIRS start method.</param>
        /// <param name="dsMinNumberOfIterations">The minimum number of DIRS iterations to perform.</param>
        /// <param name="dsMaxNumberOfIterations">The maximum number of DIRS iterations to perform.</param>
        /// <param name="dsVarCoefficient">The variation coefficient to use within the DIRS iterations.</param>
        /// <param name="niUMin">The lower size limit of a uniform grid deployed by NINT.</param>
        /// <param name="niUMax">The upper size limit of a uniform grid deployed by NINT.</param>
        /// <param name="niNumberSteps">The number of steps between <paramref name="niUMin"/> and <paramref name="niUMax"/> for NINT</param>
        public NumericsSetting(int calculationTechniqueId, int formStartMethod, int formNumberOfIterations, double formRelaxationFactor, double formEpsBeta, double formEpsHoh, double formEpsZFunc, int dsStartMethod, int dsMinNumberOfIterations, int dsMaxNumberOfIterations, double dsVarCoefficient, double niUMin, double niUMax, int niNumberSteps)
        {
            CalculationTechniqueId = calculationTechniqueId;
            FormStartMethod = formStartMethod;
            FormNumberOfIterations = formNumberOfIterations;
            FormRelaxationFactor = formRelaxationFactor;
            FormEpsBeta = formEpsBeta;
            FormEpsHoh = formEpsHoh;
            FormEpsZFunc = formEpsZFunc;
            DsStartMethod = dsStartMethod;
            DsMinNumberOfIterations = dsMinNumberOfIterations;
            DsMaxNumberOfIterations = dsMaxNumberOfIterations;
            DsVarCoefficient = dsVarCoefficient;
            NiUMin = niUMin;
            NiUMax = niUMax;
            NiNumberSteps = niNumberSteps;
        }

        /// <summary>
        /// Gets the id of the calculation technique that should be used.
        /// </summary>
        public int CalculationTechniqueId { get; }

        /// <summary>
        /// Gets the FORM start method.
        /// </summary>
        public int FormStartMethod { get; }

        /// <summary>
        /// Gets the maximum number of FORM iterations to perform.
        /// </summary>
        public int FormNumberOfIterations { get; }

        /// <summary>
        /// Gets a relaxation towards the design point for FORM.
        /// </summary>
        public double FormRelaxationFactor { get; }

        /// <summary>
        /// Gets one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsHoh"/>
        /// <seealso cref="FormEpsZFunc"/>
        public double FormEpsBeta { get; }

        /// <summary>
        /// Gets one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsBeta"/>
        /// <seealso cref="FormEpsZFunc"/>
        public double FormEpsHoh { get; }

        /// <summary>
        /// Gets one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsBeta"/>
        /// <seealso cref="FormEpsHoh"/>
        public double FormEpsZFunc { get; }

        /// <summary>
        /// Gets the DIRS start method.
        /// </summary>
        public int DsStartMethod { get; }

        /// <summary>
        /// Gets the minimum number of DIRS iterations to perform.
        /// </summary>
        public int DsMinNumberOfIterations { get; }

        /// <summary>
        /// Gets the maximum number of DIRS iterations to perform.
        /// </summary>
        public int DsMaxNumberOfIterations { get; }

        /// <summary>
        /// Gets the variation coefficient to use within the DIRS iterations.
        /// </summary>
        public double DsVarCoefficient { get; }

        /// <summary>
        /// Gets the lower size limit of a uniform grid deployed by NINT.
        /// </summary>
        public double NiUMin { get; }

        /// <summary>
        /// Gets the upper size limit of a uniform grid deployed by NINT.
        /// </summary>
        public double NiUMax { get; }

        /// <summary>
        /// Gets the number of steps between <seealso cref="NiUMin"/> and <seealso cref="NiUMax"/> for NINT.
        /// </summary>
        public int NiNumberSteps { get; }
    }
}
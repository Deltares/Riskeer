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

namespace Ringtoets.HydraRing.Calculation.Data.Settings
{
    /// <summary>
    /// Container for numerics settings.
    /// </summary>
    public class NumericsSetting
    {
        private readonly int calculationTechniqueId;
        private readonly int formStartMethod;
        private readonly int formNumberOfIterations;
        private readonly double formRelaxationFactor;
        private readonly double formEpsBeta;
        private readonly double formEpsHoh;
        private readonly double formEpsZFunc;
        private readonly int dsStartMethod;
        private readonly int dsMinNumberOfIterations;
        private readonly int dsMaxNumberOfIterations;
        private readonly double dsVarCoefficient;
        private readonly double niUMin;
        private readonly double niUMax;
        private readonly int niNumberSteps;

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
            this.calculationTechniqueId = calculationTechniqueId;
            this.formStartMethod = formStartMethod;
            this.formNumberOfIterations = formNumberOfIterations;
            this.formRelaxationFactor = formRelaxationFactor;
            this.formEpsBeta = formEpsBeta;
            this.formEpsHoh = formEpsHoh;
            this.formEpsZFunc = formEpsZFunc;
            this.dsStartMethod = dsStartMethod;
            this.dsMinNumberOfIterations = dsMinNumberOfIterations;
            this.dsMaxNumberOfIterations = dsMaxNumberOfIterations;
            this.dsVarCoefficient = dsVarCoefficient;
            this.niUMin = niUMin;
            this.niUMax = niUMax;
            this.niNumberSteps = niNumberSteps;
        }

        /// <summary>
        /// Gets the id of the calculation technique that should be used.
        /// </summary>
        public int CalculationTechniqueId
        {
            get
            {
                return calculationTechniqueId;
            }
        }

        /// <summary>
        /// Gets the FORM start method.
        /// </summary>
        public int FormStartMethod
        {
            get
            {
                return formStartMethod;
            }
        }

        /// <summary>
        /// Gets the maximum number of FORM iterations to perform.
        /// </summary>
        public int FormNumberOfIterations
        {
            get
            {
                return formNumberOfIterations;
            }
        }

        /// <summary>
        /// Gets a relaxation towards the design point for FORM.
        /// </summary>
        public double FormRelaxationFactor
        {
            get
            {
                return formRelaxationFactor;
            }
        }

        /// <summary>
        /// Gets one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsHoh"/>
        /// <seealso cref="FormEpsZFunc"/>
        public double FormEpsBeta
        {
            get
            {
                return formEpsBeta;
            }
        }

        /// <summary>
        /// Gets one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsBeta"/>
        /// <seealso cref="FormEpsZFunc"/>
        public double FormEpsHoh
        {
            get
            {
                return formEpsHoh;
            }
        }

        /// <summary>
        /// Gets one of the three convergence criteria for FORM.
        /// </summary>
        /// <seealso cref="FormEpsBeta"/>
        /// <seealso cref="FormEpsHoh"/>
        public double FormEpsZFunc
        {
            get
            {
                return formEpsZFunc;
            }
        }

        /// <summary>
        /// Gets the DIRS start method.
        /// </summary>
        public int DsStartMethod
        {
            get
            {
                return dsStartMethod;
            }
        }

        /// <summary>
        /// Gets the minimum number of DIRS iterations to perform.
        /// </summary>
        public int DsMinNumberOfIterations
        {
            get
            {
                return dsMinNumberOfIterations;
            }
        }

        /// <summary>
        /// Gets the maximum number of DIRS iterations to perform.
        /// </summary>
        public int DsMaxNumberOfIterations
        {
            get
            {
                return dsMaxNumberOfIterations;
            }
        }

        /// <summary>
        /// Gets the variation coefficient to use within the DIRS iterations.
        /// </summary>
        public double DsVarCoefficient
        {
            get
            {
                return dsVarCoefficient;
            }
        }

        /// <summary>
        /// Gets the lower size limit of a uniform grid deployed by NINT.
        /// </summary>
        public double NiUMin
        {
            get
            {
                return niUMin;
            }
        }

        /// <summary>
        /// Gets the upper size limit of a uniform grid deployed by NINT.
        /// </summary>
        public double NiUMax
        {
            get
            {
                return niUMax;
            }
        }

        /// <summary>
        /// Gets the number of steps between <seealso cref="NiUMin"/> and <seealso cref="NiUMax"/> for NINT.
        /// </summary>
        public int NiNumberSteps
        {
            get
            {
                return niNumberSteps;
            }
        }
    }
}
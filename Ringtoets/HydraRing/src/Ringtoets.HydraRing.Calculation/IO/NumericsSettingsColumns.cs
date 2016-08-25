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

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// The columns of the numerics settings file.
    /// </summary>
    public enum NumericsSettingsColumns
    {
        /// <summary>
        /// The ring id column.
        /// </summary>
        RingId,

        /// <summary>
        /// The mechanism id column.
        /// </summary>
        MechanismId,

        /// <summary>
        /// The submechanism id column.
        /// </summary>
        SubMechanismId,

        /// <summary>
        /// The calculation method column.
        /// </summary>
        CalculationMethod,

        /// <summary>
        /// The form start method column.
        /// </summary>
        FormStartMethod,

        /// <summary>
        /// The form iterations column.
        /// </summary>
        FormIterations,

        /// <summary>
        /// The form relaxation factor column.
        /// </summary>
        FormRelaxationFactor,

        /// <summary>
        /// The form eps beta column.
        /// </summary>
        FormEpsBeta,

        /// <summary>
        /// The form eps hoh column.
        /// </summary>
        FormEpsHoh,

        /// <summary>
        /// The form eps z func column.
        /// </summary>
        FormEpsZFunc,

        /// <summary>
        /// The ds start method column.
        /// </summary>
        DsStartMethod,

        /// <summary>
        /// The ds minimum number of iteration column.
        /// </summary>
        DsMinNumberOfIterations,

        /// <summary>
        /// The ds maximum number of iterations column.
        /// </summary>
        DsMaxNumberOfIterations,

        /// <summary>
        /// The ds var coefficient column.
        /// </summary>
        DsVarCoefficient,

        /// <summary>
        /// The ni u minimum column.
        /// </summary>
        NiUMin,

        /// <summary>
        /// The ni u maximum column.
        /// </summary>
        NiUMax,

        /// <summary>
        /// The ni number of steps column.
        /// </summary>
        NiNumberSteps
    }
}
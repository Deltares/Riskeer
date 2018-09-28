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

namespace Ringtoets.HydraRing.Calculation.Data
{
    /// <summary>
    /// Enumeration that defines the failure mechanism types supported by Hydra-Ring.
    /// </summary>
    /// <remarks>
    /// The integer values DON'T correspond to failure mechanism ids defined within Hydra-Ring.
    /// They DO, however, correspond to ids in the Hydra-Ring settings database files.
    /// </remarks>
    public enum HydraRingFailureMechanismType
    {
        AssessmentLevel = 0,
        QVariant = 1,
        WaveHeight = 2,
        WavePeakPeriod = 3,
        WaveSpectralPeriod = 4,
        DikeHeight = 5,
        DikesOvertopping = 6,
        StructuresOvertopping = 7,
        StructuresClosure = 8,
        StructuresStructuralFailure = 9,
        DunesBoundaryConditions = 10,
        OvertoppingRate = 11
    }
}
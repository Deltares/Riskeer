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

namespace Ringtoets.Storage.Core.DbContext
{
    /// <summary>
    /// The type failure mechanism available in the application.
    /// </summary>
    public enum FailureMechanismType
    {
        /// <summary>
        /// Piping - STPH
        /// </summary>
        Piping = 1,

        /// <summary>
        /// Macrostabiliteit binnenwaarts - STBI
        /// </summary>
        MacroStabilityInwards = 2,

        /// <summary>
        /// Golfklappen op asfaltbekleding - AGK
        /// </summary>
        WaveImpactOnAsphaltRevetment = 3,

        /// <summary>
        /// Grasbekleding erosie buitentalud - GEBU
        /// </summary>
        GrassRevetmentErosionOutwards = 4,

        /// <summary>
        /// Grasbekleding afschuiven buitentalud - GABU
        /// </summary>
        GrassRevetmentSlidingOutwards = 5,

        /// <summary>
        /// Grasbekleding erosie kruin en binnentalud - GEKB
        /// </summary>
        GrassRevetmentTopErosionAndInwards = 6,

        /// <summary>
        /// Stabiliteit steenzetting - ZST
        /// </summary>
        StabilityStoneRevetment = 7,

        /// <summary>
        /// Duinafslag - DA
        /// </summary>
        DuneErosion = 8,

        /// <summary>
        /// Hoogte kunstwerk - HTKW
        /// </summary>
        StructureHeight = 9,

        /// <summary>
        /// Betrouwbaarheid sluiting kunstwerk - BSKW
        /// </summary>
        ReliabilityClosingOfStructure = 10,

        /// <summary>
        /// Piping bij kunstwerk - PKW
        /// </summary>
        PipingAtStructure = 11,

        /// <summary>
        /// Sterkte en stabiliteit puntconstructies - STKWp
        /// </summary>
        StabilityPointStructures = 12,

        /// <summary>
        /// Macrostabiliteit buitenwaarts - STBU
        /// </summary>
        MacroStabilityOutwards = 13,

        /// <summary>
        /// Microstabiliteit - STMI
        /// </summary>
        Microstability = 14,

        /// <summary>
        /// Wateroverdruk bij asfaltbekleding - AWO
        /// </summary>
        WaterOverpressureAsphaltRevetment = 15,

        /// <summary>
        /// Grasbekleding afschuiven binnentalud - GABI
        /// </summary>
        GrassRevetmentSlidingInwards = 16,

        /// <summary>
        /// Sterkte en stabiliteit langsconstructies - STKWl
        /// </summary>
        StrengthAndStabilityParallelConstruction = 17,

        /// <summary>
        /// Technische innovaties - INN
        /// </summary>
        TechnicalInnovations = 18
    }
}
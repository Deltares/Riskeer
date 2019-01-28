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

namespace Ringtoets.Common.IO.SoilProfile.Schema
{
    /// <summary>
    /// Defines the various failure mechanism types in the D-Soil Model database.
    /// </summary>
    public enum FailureMechanismType : long
    {
        None = 0,
        Stability = 1,
        Settlement = 2,
        PipingUpliftGradient = 3,
        Piping = 4,
        Koswat = 5,
        FlowSlide = 6,
        Overtopping = 7,
        AnchorLoading = 9,
        DAM = 10,
        Structures = 11,
        BlockRevetment = 12,
        AssessmentLevel = 13,
        Dunes = 14,
        AsphaltRevetment = 15
    }
}
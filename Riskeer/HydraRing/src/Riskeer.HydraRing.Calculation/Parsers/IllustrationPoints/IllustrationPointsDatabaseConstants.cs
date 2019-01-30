// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Riskeer.HydraRing.Calculation.Parsers.IllustrationPoints
{
    /// <summary>
    /// Constants which are used for assigning values in database columns to values of
    /// data objects.
    /// </summary>
    internal static class IllustrationPointsDatabaseConstants
    {
        public const string FaultTreeId = "FaultTreeId";
        public const string FaultTreeName = "FaultTreeName";
        public const string SubMechanismId = "SubMechanismId";
        public const string SubMechanismName = "SubMechanismName";

        public const string ClosingSituationId = "ClosingSituationId";
        public const string ClosingSituationName = "ClosingSituationName";

        public const string WindDirectionId = "WindDirectionId";
        public const string WindDirectionName = "WindDirectionName";
        public const string WindDirectionAngle = "WindDirectionAngle";
        public const string IsGoverning = "IsGoverning";

        public const string StochastName = "StochastName";
        public const string AlphaValue = "AlphaValue";
        public const string Duration = "Duration";
        public const string Realization = "X";

        public const string BetaValue = "BetaValue";

        public const string IllustrationPointResultValue = "Value";
        public const string IllustrationPointResultDescription = "OutputVarDescription";

        public const string RecursiveFaultTreeId = "id";
        public const string RecursiveFaultTreeChildId = "childId";
        public const string RecursiveFaultTreeType = "type";
        public const string RecursiveFaultTreeCombine = "combine";
        public const string RecursiveFaultTreeParentId = "parentId";
    }
}
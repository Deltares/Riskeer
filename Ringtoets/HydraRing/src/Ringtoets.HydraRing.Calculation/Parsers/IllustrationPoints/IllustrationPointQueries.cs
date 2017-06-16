// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints
{
    public static class IllustrationPointQueries
    {
        public static readonly string ClosingSituations = 
            $"SELECT {IllustrationPointsDatabaseConstants.ClosingSituationId}, " +
            "ClosingSituationName " +
            "FROM ClosingSituations;";

        public static readonly string WindDirections =
            $"SELECT WindDirections.{IllustrationPointsDatabaseConstants.WindDirectionId}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionName}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionAngle}, " +
            $"WindDirections.WindDirectionId = GoverningWind.WindDirectionId as {IllustrationPointsDatabaseConstants.IsGoverning} " +
            "FROM WindDirections " +
            "JOIN GoverningWind " +
            "WHERE OuterIterationId = (SELECT MAX(OuterIterationID) FROM GoverningWind);";

        public static readonly string GeneralAlphaValues =
            $"SELECT {IllustrationPointsDatabaseConstants.StochastName}, " +
            $"{IllustrationPointsDatabaseConstants.AlphaValue}, " +
            $"{IllustrationPointsDatabaseConstants.Duration} " +
            "FROM DesignAlpha " +
            "JOIN Stochasts USING(StochastId) " +
            "WHERE LevelTypeId = 3 " +
            "AND OuterIterationId = (SELECT MAX(OuterIterationId) FROM DesignAlpha) " +
            "AND PeriodId = (SELECT MIN(PeriodId) FROM DesignAlpha);";

        public static readonly string GeneralBetaValues =
            $"SELECT {IllustrationPointsDatabaseConstants.BetaValue} " +
            "FROM DesignBeta " +
            "WHERE LevelTypeId = 3 " +
            "AND OuterIterationId = (SELECT MAX(OuterIterationId) FROM DesignAlpha) " +
            "AND PeriodId = (SELECT MIN(PeriodId) FROM DesignAlpha);";

        public static readonly string RecursiveFaultTree =
            "WITH RECURSIVE " +
            "child_of(" +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeChildId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine}" +
            ") AS(" +
            "SELECT FaultTreeId, Id1, Type1, CombinFunction " +
            "FROM FaultTrees " +
            "UNION " +
            "SELECT FaultTreeId, Id2, Type2, CombinFunction " +
            "FROM FaultTrees)," +
            $"children(" +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeParentId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine}" +
            $") AS(" +
            $"SELECT {IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeChildId}, " +
            $"child_of.{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType}, " +
            $"child_of.{IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine} " +
            "FROM child_of " +
            "WHERE id NOT IN " +
            $"(SELECT {IllustrationPointsDatabaseConstants.RecursiveFaultTreeChildId} FROM child_of) " +
            "UNION ALL " +
            "SELECT id, child_id, child_of.type, child_of.combine " +
            "FROM child_of " +
            $"JOIN children USING({IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}) " +
            $"WHERE children.{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType} = \"faulttree\") " +
            "SELECT " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeParentId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine} " +
            $"FROM children;";
    }
}
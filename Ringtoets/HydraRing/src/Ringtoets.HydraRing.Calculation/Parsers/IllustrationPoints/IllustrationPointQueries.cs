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

namespace Riskeer.HydraRing.Calculation.Parsers.IllustrationPoints
{
    /// <summary>
    /// Collection of queries used for reading illustration points from the Hydra-Ring database.
    /// </summary>
    internal static class IllustrationPointQueries
    {
        /// <summary>
        /// Selects all the closing situations.
        /// </summary>
        public static readonly string ClosingSituations =
            $"SELECT {IllustrationPointsDatabaseConstants.ClosingSituationId}, " +
            $"{IllustrationPointsDatabaseConstants.ClosingSituationName} " +
            "FROM ClosingSituations;";

        /// <summary>
        /// Selects all wind direction with a flag whether the wind direction is governing.
        /// </summary>
        public static readonly string WindDirections =
            $"SELECT WindDirections.{IllustrationPointsDatabaseConstants.WindDirectionId}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionName}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionAngle}, " +
            $"WindDirections.WindDirectionId = GoverningWind.WindDirectionId AS {IllustrationPointsDatabaseConstants.IsGoverning} " +
            "FROM WindDirections " +
            "JOIN GoverningWind " +
            $"WHERE {lastIteration}" +
            $"AND {firstPeriod};";

        /// <summary>
        /// Selects all the sub mechanisms.
        /// </summary>
        public static readonly string SubMechanisms =
            "SELECT " +
            $"{IllustrationPointsDatabaseConstants.SubMechanismId}, " +
            $"{IllustrationPointsDatabaseConstants.SubMechanismName} " +
            "FROM SubMechanisms;";

        /// <summary>
        /// Selects all the fault trees.
        /// </summary>
        public static readonly string FaultTrees =
            "SELECT " +
            $"{IllustrationPointsDatabaseConstants.FaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.FaultTreeName} " +
            "FROM FaultTrees;";

        /// <summary>
        /// Selects the alpha values for a general result.
        /// </summary>
        public static readonly string GeneralAlphaValues =
            $"SELECT {IllustrationPointsDatabaseConstants.StochastName}, " +
            $"{IllustrationPointsDatabaseConstants.AlphaValue}, " +
            $"{IllustrationPointsDatabaseConstants.Duration} " +
            "FROM DesignAlpha " +
            "JOIN Stochasts USING(StochastId) " +
            "WHERE LevelTypeId = 4 " +
            $"AND {lastIteration};";

        /// <summary>
        /// Selects the beta values for a general result.
        /// </summary>
        public static readonly string GeneralBetaValues =
            $"SELECT {IllustrationPointsDatabaseConstants.BetaValue} " +
            "FROM DesignBeta " +
            "WHERE LevelTypeId = 4 " +
            $"AND {lastIteration};";

        /// <summary>
        /// Selects the alpha values for each fault tree illustration point.
        /// </summary>
        public static readonly string FaultTreeAlphaValues =
            "SELECT " +
            $"{IllustrationPointsDatabaseConstants.FaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionId}, " +
            $"{IllustrationPointsDatabaseConstants.ClosingSituationId}, " +
            $"{IllustrationPointsDatabaseConstants.StochastName}, " +
            $"{IllustrationPointsDatabaseConstants.AlphaValue}," +
            $"{IllustrationPointsDatabaseConstants.Duration} " +
            "FROM FaultTrees " +
            "JOIN DesignAlpha USING(FaultTreeId) " +
            "JOIN Stochasts USING(StochastId) " +
            "WHERE DesignAlpha.LevelTypeId = 5 " +
            $"AND {lastIteration} " +
            $"AND {firstPeriod};";

        /// <summary>
        /// Selects the beta values for each fault tree illustration point.
        /// </summary>
        public static readonly string FaultTreeBetaValues =
            "SELECT " +
            $"{IllustrationPointsDatabaseConstants.FaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionId}, " +
            $"{IllustrationPointsDatabaseConstants.ClosingSituationId}, " +
            $"{IllustrationPointsDatabaseConstants.BetaValue} " +
            "FROM FaultTrees " +
            "JOIN DesignBeta USING(FaultTreeId) " +
            "WHERE DesignBeta.LevelTypeId = 5 " +
            $"AND {lastIteration} " +
            $"AND {firstPeriod};";

        /// <summary>
        /// Selects the alpha values for each sub mechanism illustration point.
        /// </summary>
        public static readonly string SubMechanismAlphaValues =
            "SELECT " +
            $"{IllustrationPointsDatabaseConstants.SubMechanismId}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionId}, " +
            $"{IllustrationPointsDatabaseConstants.ClosingSituationId}, " +
            $"{IllustrationPointsDatabaseConstants.StochastName}, " +
            $"{IllustrationPointsDatabaseConstants.AlphaValue}," +
            $"{IllustrationPointsDatabaseConstants.Duration}, " +
            $"{IllustrationPointsDatabaseConstants.Realization} " +
            "FROM SubMechanisms " +
            "JOIN DesignAlpha USING(SubMechanismId) " +
            "JOIN Stochasts USING(StochastId) " +
            "WHERE DesignAlpha.LevelTypeId = 7 " +
            $"AND {lastIteration} " +
            $"AND {firstPeriod};";

        /// <summary>
        /// Selects the beta values for each sub mechanism illustration point.
        /// </summary>
        public static readonly string SubMechanismBetaValues =
            "SELECT " +
            $"{IllustrationPointsDatabaseConstants.SubMechanismId}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionId}, " +
            $"{IllustrationPointsDatabaseConstants.ClosingSituationId}, " +
            $"{IllustrationPointsDatabaseConstants.BetaValue} " +
            "FROM SubMechanisms " +
            "JOIN DesignBeta USING(SubMechanismId) " +
            "WHERE DesignBeta.LevelTypeId = 7 " +
            $"AND {lastIteration} " +
            $"AND {firstPeriod};";

        /// <summary>
        /// Selects the output variables for each sub mechanism illustration point.
        /// </summary>
        public static readonly string SubMechanismIllustrationPointResults =
            "SELECT " +
            $"{IllustrationPointsDatabaseConstants.SubMechanismId}, " +
            $"{IllustrationPointsDatabaseConstants.WindDirectionId}, " +
            $"{IllustrationPointsDatabaseConstants.ClosingSituationId}, " +
            $"DesignPointResults.{IllustrationPointsDatabaseConstants.IllustrationPointResultValue}, " +
            $"{IllustrationPointsDatabaseConstants.IllustrationPointResultDescription} " +
            "FROM SubMechanisms " +
            "JOIN DesignPointResults USING(SubMechanismId) " +
            "JOIN OutputVariables USING(OutputVariableId) " +
            $"WHERE {lastIteration} " +
            $"AND {firstPeriod};";

        /// <summary>
        /// Selects all the illustration points from the fault tree.
        /// </summary>
        public static readonly string RecursiveFaultTree =
            "WITH RECURSIVE " +
            "combineFunctions(id, combine) AS (" +
            "SELECT FaultTreeId, CombinFunction " +
            "FROM FaultTrees)," +
            "child_of(" +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeChildId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine}" +
            ") AS (" +
            "SELECT FaultTreeId, Id1, Type1, combine " +
            "FROM FaultTrees " +
            "LEFT OUTER JOIN combineFunctions ON combineFunctions.id = Id1 " +
            "UNION " +
            "SELECT FaultTreeId, Id2, Type2, combine " +
            "FROM FaultTrees " +
            "LEFT OUTER JOIN combineFunctions ON combineFunctions.id = Id2), " +
            "children(" +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeParentId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine}" +
            ") AS (" +
            "SELECT null, " +
            "FaultTreeId, " +
            "\"faulttree\", " +
            "CombinFunction " +
            "FROM FaultTrees " +
            "WHERE FaultTreeId NOT IN " +
            $"(SELECT {IllustrationPointsDatabaseConstants.RecursiveFaultTreeChildId} FROM child_of) " +
            "UNION ALL " +
            $"SELECT {IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeChildId}, " +
            $"child_of.{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType}, " +
            $"child_of.{IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine} " +
            "FROM child_of " +
            $"JOIN children USING({IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}) " +
            $"WHERE children.{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType} = \"faulttree\") " +
            "SELECT DISTINCT " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeParentId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeId}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeType}, " +
            $"{IllustrationPointsDatabaseConstants.RecursiveFaultTreeCombine} " +
            "FROM children;";

        private const string firstPeriod = "PeriodId = (SELECT MIN(PeriodId) FROM GoverningWind)";
        private const string lastIteration = "OuterIterationId = (SELECT MAX(OuterIterationId) FROM GoverningWind)";
    }
}
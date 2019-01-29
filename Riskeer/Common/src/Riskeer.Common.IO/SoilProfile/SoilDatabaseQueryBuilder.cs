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

using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// Defines queries to execute on the D-Soil Model database.
    /// </summary>
    internal static class SoilDatabaseQueryBuilder
    {
        private static readonly string getMaterialPropertiesOfLayerQuery =
            "SELECT " +
            "mat.MA_ID, " +
            $"mat.MA_Name AS {SoilProfileTableDefinitions.MaterialName}, " +
            $"max(case when pn.PN_Name = 'Color' then pv.PV_Value end) AS {SoilProfileTableDefinitions.Color}, " +
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelDistributionType}, " +
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelShift}, " +
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelMean}, " +
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Deviation end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelDeviation}, " +
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelCoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.PermeabilityDistributionType}, " +
            $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.PermeabilityShift}, " +
            $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.PermeabilityMean}, " +
            $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.DiameterD70DistributionType}, " +
            $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.DiameterD70Shift}, " +
            $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.DiameterD70Mean}, " +
            $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'UsePop' then pv.PV_Value end) AS {SoilProfileTableDefinitions.UsePop}, " +
            $"max(case when pn.PN_Name = 'ShearStrengthModel' then pv.PV_Value end) AS {SoilProfileTableDefinitions.ShearStrengthModel}, " +
            $"max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.AbovePhreaticLevelDistributionType}, " +
            $"max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.AbovePhreaticLevelShift}, " +
            $"max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.AbovePhreaticLevelMean}, " +
            $"max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.AbovePhreaticLevelCoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.CohesionDistributionType}, " +
            $"max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.CohesionShift}, " +
            $"max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.CohesionMean}, " +
            $"max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.CohesionCoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.FrictionAngleDistributionType}, " +
            $"max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.FrictionAngleShift}, " +
            $"max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.FrictionAngleMean}, " +
            $"max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.FrictionAngleCoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.ShearStrengthRatioDistributionType}, " +
            $"max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.ShearStrengthRatioShift}, " +
            $"max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.ShearStrengthRatioMean}, " +
            $"max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.ShearStrengthRatioCoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.StrengthIncreaseExponentDistributionType}, " +
            $"max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.StrengthIncreaseExponentShift}, " +
            $"max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.StrengthIncreaseExponentMean}, " +
            $"max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.StrengthIncreaseExponentCoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'POPStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.PopDistributionType}, " +
            $"max(case when pn.PN_Name = 'POPStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.PopShift}, " +
            $"max(case when pn.PN_Name = 'POPStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.PopMean}, " +
            $"max(case when pn.PN_Name = 'POPStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.PopCoefficientOfVariation} " +
            "FROM ParameterNames AS pn " +
            "LEFT JOIN ParameterValues AS pv USING(PN_ID) " +
            "LEFT JOIN Stochast AS s USING(PN_ID) " +
            "JOIN Materials AS mat " +
            "WHERE pv.MA_ID = mat.MA_ID OR s.MA_ID = mat.MA_ID " +
            "GROUP BY mat.MA_ID ";

        /// <summary>
        /// Returns the SQL query to execute to check if version of the D-Soil Model database is as expected.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        /// <remarks><see cref="System.Data.SQLite.SQLiteParameter"/> "@<see cref="MetaTableDefinitions.Value"/>"
        /// needs to be defined as the required database version.</remarks>
        public static string GetCheckVersionQuery()
        {
            return $"SELECT {MetaTableDefinitions.Value} " +
                   $"FROM {MetaTableDefinitions.TableName} " +
                   $"WHERE {MetaTableDefinitions.Key} = '{MetaTableDefinitions.ValueVersion}' " +
                   $"AND {MetaTableDefinitions.Value} = @{MetaTableDefinitions.Value};";
        }

        /// <summary>
        /// Returns the SQL query to execute to check if segment names in the D-Soil Model database 
        /// are unique.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetSoilModelNamesUniqueQuery()
        {
            return string.Format(
                "SELECT [All].nameCount == [Distinct].nameCount AS {0} " +
                "FROM (SELECT COUNT({1}) nameCount FROM {2}) AS [All] " +
                "JOIN (SELECT COUNT(DISTINCT {1}) nameCount FROM {2}) AS [Distinct];",
                StochasticSoilModelTableDefinitions.AreSegmentsUnique,
                StochasticSoilModelTableDefinitions.StochasticSoilModelName,
                StochasticSoilModelTableDefinitions.TableName);
        }

        /// <summary>
        /// Returns the query to get the amount of <see cref="StochasticSoilModel"/> 
        /// that can be read from the database.
        /// </summary>
        /// <returns>The query to get the amount of <see cref="StochasticSoilModel"/> 
        /// from the database.</returns>
        public static string GetStochasticSoilModelOfMechanismCountQuery()
        {
            return $"SELECT COUNT() AS {StochasticSoilModelTableDefinitions.Count} " +
                   $"FROM {StochasticSoilModelTableDefinitions.TableName} SSM " +
                   $"INNER JOIN {SegmentTableDefinitions.TableName} S USING({StochasticSoilModelTableDefinitions.StochasticSoilModelId}) " +
                   $"INNER JOIN {MechanismTableDefinitions.TableName} M USING({MechanismTableDefinitions.MechanismId});";
        }

        /// <summary>
        /// Returns the SQL query to execute to check if the probabilities of the stochastic
        /// soil profiles are valid.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetStochasticSoilProfileProbabilitiesValidQuery()
        {
            return string.Format(
                "SELECT COUNT({1}) == 0 AS {0} " +
                "FROM {2} " +
                "WHERE {1} NOT BETWEEN 0 AND 1 OR {1} ISNULL;",
                StochasticSoilProfileTableDefinitions.AllProbabilitiesValid,
                StochasticSoilProfileTableDefinitions.Probability,
                StochasticSoilProfileTableDefinitions.TableName);
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch stochastic soil models 
        /// per failure mechanism from the D-Soil Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetStochasticSoilModelPerMechanismQuery()
        {
            return $"SELECT SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelId}, " +
                   $"M.{MechanismTableDefinitions.MechanismId}, " +
                   $"M.{MechanismTableDefinitions.MechanismName}, " +
                   $"SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelName}, " +
                   $"SSP.{StochasticSoilProfileTableDefinitions.Probability}, " +
                   $"SSP.{StochasticSoilProfileTableDefinitions.SoilProfile1DId}, " +
                   $"SSP.{StochasticSoilProfileTableDefinitions.SoilProfile2DId} " +
                   $"FROM {MechanismTableDefinitions.TableName} M " +
                   $"INNER JOIN {SegmentTableDefinitions.TableName} S USING({MechanismTableDefinitions.MechanismId}) " +
                   $"INNER JOIN {StochasticSoilModelTableDefinitions.TableName} SSM USING({StochasticSoilModelTableDefinitions.StochasticSoilModelId}) " +
                   $"LEFT JOIN {StochasticSoilProfileTableDefinitions.TableName} SSP USING({StochasticSoilModelTableDefinitions.StochasticSoilModelId}) " +
                   $"ORDER BY SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelId};";
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch segments and segment points
        /// per stochastic soil model from the D-Soil Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetSegmentPointsQuery()
        {
            return $"SELECT SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelId}, " +
                   $"SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelName}, " +
                   $"SP.{SegmentPointsTableDefinitions.CoordinateX}, " +
                   $"SP.{SegmentPointsTableDefinitions.CoordinateY} " +
                   $"FROM {SegmentTableDefinitions.TableName} S " +
                   $"INNER JOIN {StochasticSoilModelTableDefinitions.TableName} SSM USING({StochasticSoilModelTableDefinitions.StochasticSoilModelId}) " +
                   $"INNER JOIN {SegmentPointsTableDefinitions.TableName} SP USING({SegmentTableDefinitions.SegmentId}) " +
                   $"ORDER BY SSM.{StochasticSoilModelTableDefinitions.StochasticSoilModelId};";
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch 1D soil profile from the D-Soil Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetSoilProfile1DQuery()
        {
            const string getNumberOfLayerProfile1DQuery =
                "SELECT COUNT(*) " +
                "FROM SoilLayer1D WHERE SoilLayer1D.SP1D_ID = sp1d.SP1D_ID";

            string getLayerPropertiesOfLayer1DQuery =
                $"SELECT SL1D_ID, PV_Value AS {SoilProfileTableDefinitions.IsAquifer} " +
                "FROM ParameterNames " +
                "JOIN LayerParameterValues USING(PN_ID) " +
                $"WHERE PN_NAME = '{SoilProfileTableDefinitions.IsAquifer}'";

            return
                "SELECT " +
                $"sp1d.SP1D_Name AS {SoilProfileTableDefinitions.ProfileName}, " +
                $"sp1d.BottomLevel AS {SoilProfileTableDefinitions.Bottom}, " +
                $"sl1d.TopLevel AS {SoilProfileTableDefinitions.Top}, " +
                $"{SoilProfileTableDefinitions.MaterialName}, " +
                $"{SoilProfileTableDefinitions.IsAquifer}, " +
                $"{SoilProfileTableDefinitions.Color}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDistributionType}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelShift}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelMean}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDeviation}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.DiameterD70DistributionType}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Shift}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Mean}, " +
                $"{SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.PermeabilityDistributionType}, " +
                $"{SoilProfileTableDefinitions.PermeabilityShift}, " +
                $"{SoilProfileTableDefinitions.PermeabilityMean}, " +
                $"{SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.UsePop}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthModel}, " +
                $"{SoilProfileTableDefinitions.AbovePhreaticLevelDistributionType}, " +
                $"{SoilProfileTableDefinitions.AbovePhreaticLevelMean}, " +
                $"{SoilProfileTableDefinitions.AbovePhreaticLevelCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.AbovePhreaticLevelShift}, " +
                $"{SoilProfileTableDefinitions.CohesionDistributionType}, " +
                $"{SoilProfileTableDefinitions.CohesionMean}, " +
                $"{SoilProfileTableDefinitions.CohesionCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.CohesionShift}, " +
                $"{SoilProfileTableDefinitions.FrictionAngleDistributionType}, " +
                $"{SoilProfileTableDefinitions.FrictionAngleMean}, " +
                $"{SoilProfileTableDefinitions.FrictionAngleCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.FrictionAngleShift}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthRatioDistributionType}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthRatioMean}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthRatioCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthRatioShift}, " +
                $"{SoilProfileTableDefinitions.StrengthIncreaseExponentDistributionType}, " +
                $"{SoilProfileTableDefinitions.StrengthIncreaseExponentMean}, " +
                $"{SoilProfileTableDefinitions.StrengthIncreaseExponentCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.StrengthIncreaseExponentShift}, " +
                $"{SoilProfileTableDefinitions.PopDistributionType}, " +
                $"{SoilProfileTableDefinitions.PopMean}, " +
                $"{SoilProfileTableDefinitions.PopCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.PopShift}, " +
                $"sp1d.SP1D_ID AS {SoilProfileTableDefinitions.SoilProfileId}," +
                $"({getNumberOfLayerProfile1DQuery}) AS {SoilProfileTableDefinitions.LayerCount} " +
                "FROM Segment AS segment " +
                "JOIN (SELECT SSM_ID, SP1D_ID FROM StochasticSoilProfile GROUP BY SSM_ID, SP1D_ID) ssp USING(SSM_ID) " +
                "JOIN SoilProfile1D sp1d USING(SP1D_ID) " +
                "LEFT JOIN SoilLayer1D sl1d USING(SP1D_ID) " +
                $"LEFT JOIN ({getMaterialPropertiesOfLayerQuery}) materialProperties USING(MA_ID) " +
                $"LEFT JOIN ({getLayerPropertiesOfLayer1DQuery}) USING(SL1D_ID) " +
                "GROUP BY sp1d.SP1D_ID, sl1d.SL1D_ID;";
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch 2D soil profile from the D-Soil Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetSoilProfile2DQuery()
        {
            const string getNumberOfLayerProfile2DQuery =
                "SELECT COUNT(*) " +
                "FROM SoilLayer2D WHERE SoilLayer2D.SP2D_ID = sp2d.SP2D_ID";

            string getLayerPropertiesOfLayer2DQuery =
                $"SELECT SL2D_ID, PV_Value AS {SoilProfileTableDefinitions.IsAquifer} " +
                "FROM ParameterNames " +
                "JOIN LayerParameterValues USING(PN_ID) " +
                $"WHERE PN_NAME = '{SoilProfileTableDefinitions.IsAquifer}'";

            return
                "SELECT " +
                $"sp2d.SP2D_Name AS {SoilProfileTableDefinitions.ProfileName}, " +
                $"sl2d.GeometrySurface AS {SoilProfileTableDefinitions.LayerGeometry}, " +
                $"mpl.X AS {SoilProfileTableDefinitions.IntersectionX}, " +
                $"{SoilProfileTableDefinitions.MaterialName}, " +
                $"{SoilProfileTableDefinitions.IsAquifer}, " +
                $"{SoilProfileTableDefinitions.Color}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDistributionType}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelShift}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelMean}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDeviation}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.DiameterD70DistributionType}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Shift}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Mean}, " +
                $"{SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.PermeabilityDistributionType}, " +
                $"{SoilProfileTableDefinitions.PermeabilityShift}, " +
                $"{SoilProfileTableDefinitions.PermeabilityMean}, " +
                $"{SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.UsePop}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthModel}, " +
                $"{SoilProfileTableDefinitions.AbovePhreaticLevelDistributionType}, " +
                $"{SoilProfileTableDefinitions.AbovePhreaticLevelMean}, " +
                $"{SoilProfileTableDefinitions.AbovePhreaticLevelCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.AbovePhreaticLevelShift}, " +
                $"{SoilProfileTableDefinitions.CohesionDistributionType}, " +
                $"{SoilProfileTableDefinitions.CohesionMean}, " +
                $"{SoilProfileTableDefinitions.CohesionCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.CohesionShift}, " +
                $"{SoilProfileTableDefinitions.FrictionAngleDistributionType}, " +
                $"{SoilProfileTableDefinitions.FrictionAngleMean}, " +
                $"{SoilProfileTableDefinitions.FrictionAngleCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.FrictionAngleShift}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthRatioDistributionType}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthRatioMean}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthRatioCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.ShearStrengthRatioShift}, " +
                $"{SoilProfileTableDefinitions.StrengthIncreaseExponentDistributionType}, " +
                $"{SoilProfileTableDefinitions.StrengthIncreaseExponentMean}, " +
                $"{SoilProfileTableDefinitions.StrengthIncreaseExponentCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.StrengthIncreaseExponentShift}, " +
                $"{SoilProfileTableDefinitions.PopDistributionType}, " +
                $"{SoilProfileTableDefinitions.PopMean}, " +
                $"{SoilProfileTableDefinitions.PopCoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.PopShift}, " +
                $"sp2d.SP2D_ID AS {SoilProfileTableDefinitions.SoilProfileId}, " +
                $"({getNumberOfLayerProfile2DQuery}) AS {SoilProfileTableDefinitions.LayerCount} " +
                $"FROM {MechanismTableDefinitions.TableName} AS m " +
                $"JOIN {SegmentTableDefinitions.TableName} AS segment USING({MechanismTableDefinitions.MechanismId}) " +
                "JOIN (SELECT SSM_ID, SP2D_ID FROM StochasticSoilProfile GROUP BY SSM_ID, SP2D_ID) ssp USING(SSM_ID) " +
                "JOIN SoilProfile2D sp2d USING(SP2D_ID) " +
                "LEFT JOIN SoilLayer2D sl2d USING(SP2D_ID) " +
                "LEFT JOIN MechanismPointLocation mpl USING(ME_ID, SP2D_ID) " +
                $"LEFT JOIN ({getMaterialPropertiesOfLayerQuery}) materialProperties USING(MA_ID) " +
                $"LEFT JOIN ({getLayerPropertiesOfLayer2DQuery}) USING(SL2D_ID) " +
                "GROUP BY sp2d.SP2D_ID, sl2d.SL2D_ID;";
        }

        /// <summary>
        /// Returns the SQL query to execute to fetch the preconsolidated stresses belonging to a 
        /// 2D soil profile from the D-Soil Model database.
        /// </summary>
        /// <returns>The SQL query to execute.</returns>
        public static string GetSoilProfile2DPreconsolidationStressesQuery()
        {
            return
                "SELECT " +
                "sp2d.SP2D_Name AS ProfileName, " +
                $"{StochasticSoilProfileTableDefinitions.SoilProfile2DId} AS {SoilProfileTableDefinitions.SoilProfileId}, " +
                $"ps.X AS {PreconsolidationStressTableDefinitions.PreconsolidationStressXCoordinate}, " +
                $"ps.Z AS {PreconsolidationStressTableDefinitions.PreconsolidationStressZCoordinate}, " +
                $"s.ST_Dist_Type AS {PreconsolidationStressTableDefinitions.PreconsolidationStressDistributionType}, " +
                $"s.ST_Mean AS {PreconsolidationStressTableDefinitions.PreconsolidationStressMean}, " +
                $"s.ST_Variation AS {PreconsolidationStressTableDefinitions.PreconsolidationStressCoefficientOfVariation}, " +
                $"s.ST_Shift AS {PreconsolidationStressTableDefinitions.PreconsolidationStressShift} " +
                $"FROM {PreconsolidationStressTableDefinitions.TableName} AS ps " +
                $"JOIN SoilProfile2D AS sp2d USING({StochasticSoilProfileTableDefinitions.SoilProfile2DId}) " +
                "LEFT JOIN Stochast AS s USING(ST_ID) " +
                $"ORDER BY {StochasticSoilProfileTableDefinitions.SoilProfile2DId};";
        }
    }
}
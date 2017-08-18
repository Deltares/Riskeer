﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.IO.SoilProfile
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
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelDistribution}, " +
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelShift}, " +
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelMean}, " +
            $"max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Deviation end) AS {SoilProfileTableDefinitions.BelowPhreaticLevelDeviation}, " +
            $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.PermeabilityDistribution}, " +
            $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.PermeabilityShift}, " +
            $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.PermeabilityMean}, " +
            $"max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation}, " +
            $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Dist_Type end) AS {SoilProfileTableDefinitions.DiameterD70Distribution}, " +
            $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Shift end) AS {SoilProfileTableDefinitions.DiameterD70Shift}, " +
            $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Mean end) AS {SoilProfileTableDefinitions.DiameterD70Mean}, " +
            $"max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Variation end) AS {SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation} " +
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
            string getNumberOfLayerProfile1DQuery =
                $"SELECT SP1D_ID, COUNT(*) AS {SoilProfileTableDefinitions.LayerCount} " +
                "FROM SoilLayer1D " +
                "GROUP BY SP1D_ID";

            string getLayerPropertiesOfLayer1DQuery =
                $"SELECT SL1D_ID, PV_Value AS {SoilProfileTableDefinitions.IsAquifer} " +
                "FROM ParameterNames " +
                "JOIN LayerParameterValues USING(PN_ID) " +
                $"WHERE PN_NAME = '{SoilProfileTableDefinitions.IsAquifer}'";

            return
                "SELECT " +
                $"sp1d.SP1D_Name AS {SoilProfileTableDefinitions.ProfileName}, " +
                $"layerCount.{SoilProfileTableDefinitions.LayerCount}, " +
                $"sp1d.BottomLevel AS {SoilProfileTableDefinitions.Bottom}, " +
                $"sl1d.TopLevel AS {SoilProfileTableDefinitions.Top}, " +
                $"{SoilProfileTableDefinitions.MaterialName}, " +
                $"{SoilProfileTableDefinitions.IsAquifer}, " +
                $"{SoilProfileTableDefinitions.Color}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDistribution}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelShift}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelMean}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDeviation}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Distribution}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Shift}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Mean}, " +
                $"{SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.PermeabilityDistribution}, " +
                $"{SoilProfileTableDefinitions.PermeabilityShift}, " +
                $"{SoilProfileTableDefinitions.PermeabilityMean}, " +
                $"{SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation}, " +
                $"sp1d.SP1D_ID AS {SoilProfileTableDefinitions.SoilProfileId} " +
                "FROM Segment AS segment " +
                "JOIN (SELECT SSM_ID, SP1D_ID FROM StochasticSoilProfile GROUP BY SSM_ID, SP1D_ID) ssp USING(SSM_ID) " +
                "JOIN SoilProfile1D sp1d USING(SP1D_ID) " +
                $"JOIN ({getNumberOfLayerProfile1DQuery}) {SoilProfileTableDefinitions.LayerCount} USING(SP1D_ID) " +
                "JOIN SoilLayer1D sl1d USING(SP1D_ID) " +
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
            string getNumberOfLayerProfile2DQuery =
                $"SELECT SP2D_ID, COUNT(*) AS {SoilProfileTableDefinitions.LayerCount} " +
                "FROM SoilLayer2D " +
                "GROUP BY SP2D_ID";

            string getLayerPropertiesOfLayer2DQuery =
                $"SELECT SL2D_ID, PV_Value AS {SoilProfileTableDefinitions.IsAquifer} " +
                "FROM ParameterNames " +
                "JOIN LayerParameterValues USING(PN_ID) " +
                $"WHERE PN_NAME = '{SoilProfileTableDefinitions.IsAquifer}'";

            return
                "SELECT " +
                $"sp2d.SP2D_Name AS {SoilProfileTableDefinitions.ProfileName}, " +
                $"layerCount.{SoilProfileTableDefinitions.LayerCount}, " +
                $"sl2d.GeometrySurface AS {SoilProfileTableDefinitions.LayerGeometry}, " +
                $"mpl.X AS {SoilProfileTableDefinitions.IntersectionX}, " +
                $"{SoilProfileTableDefinitions.MaterialName}, " +
                $"{SoilProfileTableDefinitions.IsAquifer}, " +
                $"{SoilProfileTableDefinitions.Color}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDistribution}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelShift}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelMean}, " +
                $"{SoilProfileTableDefinitions.BelowPhreaticLevelDeviation}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Distribution}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Shift}, " +
                $"{SoilProfileTableDefinitions.DiameterD70Mean}, " +
                $"{SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation}, " +
                $"{SoilProfileTableDefinitions.PermeabilityDistribution}, " +
                $"{SoilProfileTableDefinitions.PermeabilityShift}, " +
                $"{SoilProfileTableDefinitions.PermeabilityMean}, " +
                $"{SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation}, " +
                $"sp2d.SP2D_ID AS {SoilProfileTableDefinitions.SoilProfileId} " +
                $"FROM {MechanismTableDefinitions.TableName} AS m " +
                $"JOIN {SegmentTableDefinitions.TableName} AS segment USING({MechanismTableDefinitions.MechanismId}) " +
                "JOIN (SELECT SSM_ID, SP2D_ID FROM StochasticSoilProfile GROUP BY SSM_ID, SP2D_ID) ssp USING(SSM_ID) " +
                "JOIN SoilProfile2D sp2d USING(SP2D_ID) " +
                $"JOIN ({getNumberOfLayerProfile2DQuery}) {SoilProfileTableDefinitions.LayerCount} USING(SP2D_ID) " +
                "JOIN SoilLayer2D sl2d USING(SP2D_ID) " +
                "LEFT JOIN MechanismPointLocation mpl USING(ME_ID, SP2D_ID) " +
                $"LEFT JOIN ({getMaterialPropertiesOfLayerQuery}) materialProperties USING(MA_ID) " +
                $"LEFT JOIN ({getLayerPropertiesOfLayer2DQuery}) USING(SL2D_ID) " +
                "GROUP BY sp2d.SP2D_ID, sl2d.SL2D_ID;";
        }
    }
}
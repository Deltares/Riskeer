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

using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilDatabaseQueryBuilderTest
    {
        [Test]
        public void GetCheckVersionQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetCheckVersionQuery();

            // Assert
            const string expectedQuery =
                "SELECT Value " +
                "FROM _MetaData " +
                "WHERE Key = 'VERSION' " +
                "AND Value = @Value;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetSoilModelNamesUniqueQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetSoilModelNamesUniqueQuery();

            // Assert
            const string expectedQuery =
                "SELECT [All].nameCount == [Distinct].nameCount AS AreSegmentsUnique " +
                "FROM (SELECT COUNT(SSM_Name) nameCount FROM StochasticSoilModel) AS [All] " +
                "JOIN (SELECT COUNT(DISTINCT SSM_Name) nameCount FROM StochasticSoilModel) AS [Distinct];";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetStochasticSoilModelOfMechanismCountQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilModelOfMechanismCountQuery();

            // Assert
            const string expectedQuery =
                "SELECT COUNT() AS nrOfRows " +
                "FROM StochasticSoilModel SSM " +
                "INNER JOIN Segment S USING(SSM_ID) " +
                "INNER JOIN Mechanism M USING(ME_ID);";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetStochasticSoilProfileProbabilitiesDefinedQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilProfileProbabilitiesValidQuery();

            // Assert
            const string expectedQuery =
                "SELECT COUNT(Probability) == 0 AS AllProbabilitiesValid " +
                "FROM StochasticSoilProfile " +
                "WHERE Probability NOT BETWEEN 0 AND 1 OR Probability ISNULL;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetStochasticSoilModelPerMechanismQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetStochasticSoilModelPerMechanismQuery();

            // Assert
            const string expectedQuery =
                "SELECT SSM.SSM_ID, M.ME_ID, M.ME_Name, SSM.SSM_Name, SSP.Probability, SSP.SP1D_ID, SSP.SP2D_ID " +
                "FROM Mechanism M " +
                "INNER JOIN Segment S USING(ME_ID) " +
                "INNER JOIN StochasticSoilModel SSM USING(SSM_ID) " +
                "LEFT JOIN StochasticSoilProfile SSP USING(SSM_ID) " +
                "ORDER BY SSM.SSM_ID;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetSegmentPointsQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetSegmentPointsQuery();

            // Assert
            const string expectedQuery =
                "SELECT SSM.SSM_ID, SSM.SSM_Name, SP.XWorld, SP.YWorld " +
                "FROM Segment S " +
                "INNER JOIN StochasticSoilModel SSM USING(SSM_ID) " +
                "INNER JOIN SegmentPoints SP USING(SE_ID) " +
                "ORDER BY SSM.SSM_ID;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetSoilProfile1DQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetSoilProfile1DQuery();

            // Assert
            const string expectedQuery =
                "SELECT " +
                "sp1d.SP1D_Name AS ProfileName, " +
                "layerCount.LayerCount, " +
                "sp1d.BottomLevel AS Bottom, " +
                "sl1d.TopLevel AS Top, " +
                "MaterialName, " +
                "IsAquifer, " +
                "Color, " +
                "BelowPhreaticLevelDistribution, " +
                "BelowPhreaticLevelShift, " +
                "BelowPhreaticLevelMean, " +
                "BelowPhreaticLevelDeviation, " +
                "BelowPhreaticLevelCoefficientOfVariation, " +
                "DiameterD70Distribution, " +
                "DiameterD70Shift, " +
                "DiameterD70Mean, " +
                "DiameterD70CoefficientOfVariation, " +
                "PermeabKxDistribution, " +
                "PermeabKxShift, " +
                "PermeabKxMean, " +
                "PermeabKxCoefficientOfVariation, " +
                "UsePOP, " +
                "ShearStrengthModel, " +
                "AbovePhreaticLevelDistribution, " +
                "AbovePhreaticLevelMean, " +
                "AbovePhreaticLevelCoefficientOfVariation, " +
                "AbovePhreaticLevelShift, " +
                "CohesionDistribution, " +
                "CohesionMean, " +
                "CohesionCoefficientOfVariation, " +
                "CohesionShift, " +
                "FrictionAngleDistribution, " +
                "FrictionAngleMean, " +
                "FrictionAngleCoefficientOfVariation, " +
                "FrictionAngleShift, " +
                "ShearStrengthRatioDistribution, " +
                "ShearStrengthRatioMean, " +
                "ShearStrengthRatioCoefficientOfVariation, " +
                "ShearStrengthRatioShift, " +
                "StrengthIncreaseExponentDistribution, " +
                "StrengthIncreaseExponentMean, " +
                "StrengthIncreaseExponentCoefficientOfVariation, " +
                "StrengthIncreaseExponentShift, " +
                "PopDistribution, " +
                "PopMean, " +
                "PopCoefficientOfVariation, " +
                "PopShift, " +
                "sp1d.SP1D_ID AS SoilProfileId " +
                "FROM Segment AS segment " +
                "JOIN " +
                "(" +
                "SELECT SSM_ID, SP1D_ID " +
                "FROM StochasticSoilProfile " +
                "GROUP BY SSM_ID, SP1D_ID" +
                ") ssp USING(SSM_ID) " +
                "JOIN SoilProfile1D sp1d USING(SP1D_ID) " +
                "JOIN " +
                "(" +
                "SELECT SP1D_ID, COUNT(*) AS LayerCount " +
                "FROM SoilLayer1D " +
                "GROUP BY SP1D_ID" +
                ") LayerCount USING(SP1D_ID) " +
                "JOIN SoilLayer1D sl1d USING(SP1D_ID) " +
                "LEFT JOIN " +
                "(" +
                "SELECT " +
                "mat.MA_ID, " +
                "mat.MA_Name AS MaterialName, " +
                "max(case when pn.PN_Name = 'Color' then pv.PV_Value end) AS Color, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Dist_Type end) AS BelowPhreaticLevelDistribution, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Shift end) AS BelowPhreaticLevelShift, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Mean end) AS BelowPhreaticLevelMean, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Deviation end) AS BelowPhreaticLevelDeviation, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Variation end) AS BelowPhreaticLevelCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Dist_Type end) AS PermeabKxDistribution, " +
                "max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Shift end) AS PermeabKxShift, " +
                "max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Mean end) AS PermeabKxMean, " +
                "max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Variation end) AS PermeabKxCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Dist_Type end) AS DiameterD70Distribution, " +
                "max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Shift end) AS DiameterD70Shift, " +
                "max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Mean end) AS DiameterD70Mean, " +
                "max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Variation end) AS DiameterD70CoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'UsePop' then pv.PV_Value end) AS UsePOP, " +
                "max(case when pn.PN_Name = 'ShearStrengthModel' then pv.PV_Value end) AS ShearStrengthModel, " +
                "max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Dist_Type end) AS AbovePhreaticLevelDistribution, " +
                "max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Shift end) AS AbovePhreaticLevelShift, " +
                "max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Mean end) AS AbovePhreaticLevelMean, " +
                "max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Variation end) AS AbovePhreaticLevelCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Dist_Type end) AS CohesionDistribution, " +
                "max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Shift end) AS CohesionShift, " +
                "max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Mean end) AS CohesionMean, " +
                "max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Variation end) AS CohesionCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Dist_Type end) AS FrictionAngleDistribution, " +
                "max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Shift end) AS FrictionAngleShift, " +
                "max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Mean end) AS FrictionAngleMean, " +
                "max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Variation end) AS FrictionAngleCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Dist_Type end) AS ShearStrengthRatioDistribution, " +
                "max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Shift end) AS ShearStrengthRatioShift, " +
                "max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Mean end) AS ShearStrengthRatioMean, " +
                "max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Variation end) AS ShearStrengthRatioCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Dist_Type end) AS StrengthIncreaseExponentDistribution, " +
                "max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Shift end) AS StrengthIncreaseExponentShift, " +
                "max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Mean end) AS StrengthIncreaseExponentMean, " +
                "max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Variation end) AS StrengthIncreaseExponentCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'POPStochast' then s.ST_Dist_Type end) AS PopDistribution, " +
                "max(case when pn.PN_Name = 'POPStochast' then s.ST_Shift end) AS PopShift, " +
                "max(case when pn.PN_Name = 'POPStochast' then s.ST_Mean end) AS PopMean, " +
                "max(case when pn.PN_Name = 'POPStochast' then s.ST_Variation end) AS PopCoefficientOfVariation " +
                "FROM ParameterNames AS pn " +
                "LEFT JOIN ParameterValues AS pv USING(PN_ID) " +
                "LEFT JOIN Stochast AS s USING(PN_ID) " +
                "JOIN Materials AS mat " +
                "WHERE pv.MA_ID = mat.MA_ID OR s.MA_ID = mat.MA_ID GROUP BY mat.MA_ID " +
                ") materialProperties USING(MA_ID) " +
                "LEFT JOIN " +
                "(" +
                "SELECT SL1D_ID, PV_Value AS IsAquifer " +
                "FROM ParameterNames " +
                "JOIN LayerParameterValues USING(PN_ID) " +
                "WHERE PN_NAME = 'IsAquifer'" +
                ") USING(SL1D_ID) " +
                "GROUP BY sp1d.SP1D_ID, sl1d.SL1D_ID;";
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void GetSoilProfile2DQuery_ReturnsExpectedValues()
        {
            // Call
            string query = SoilDatabaseQueryBuilder.GetSoilProfile2DQuery();

            // Assert
            const string expectedQuery =
                "SELECT sp2d.SP2D_Name AS ProfileName, " +
                "layerCount.LayerCount, " +
                "sl2d.GeometrySurface AS LayerGeometry, " +
                "mpl.X AS IntersectionX, " +
                "MaterialName, " +
                "IsAquifer, " +
                "Color, " +
                "BelowPhreaticLevelDistribution, " +
                "BelowPhreaticLevelShift, " +
                "BelowPhreaticLevelMean, " +
                "BelowPhreaticLevelDeviation, " +
                "BelowPhreaticLevelCoefficientOfVariation, " +
                "DiameterD70Distribution, " +
                "DiameterD70Shift, " +
                "DiameterD70Mean, " +
                "DiameterD70CoefficientOfVariation, " +
                "PermeabKxDistribution, " +
                "PermeabKxShift, " +
                "PermeabKxMean, " +
                "PermeabKxCoefficientOfVariation, " +
                "UsePOP, " +
                "ShearStrengthModel, " +
                "AbovePhreaticLevelDistribution, " +
                "AbovePhreaticLevelMean, " +
                "AbovePhreaticLevelCoefficientOfVariation, " +
                "AbovePhreaticLevelShift, " +
                "CohesionDistribution, " +
                "CohesionMean, " +
                "CohesionCoefficientOfVariation, " +
                "CohesionShift, " +
                "FrictionAngleDistribution, " +
                "FrictionAngleMean, " +
                "FrictionAngleCoefficientOfVariation, " +
                "FrictionAngleShift, " +
                "ShearStrengthRatioDistribution, " +
                "ShearStrengthRatioMean, " +
                "ShearStrengthRatioCoefficientOfVariation, " +
                "ShearStrengthRatioShift, " +
                "StrengthIncreaseExponentDistribution, " +
                "StrengthIncreaseExponentMean, " +
                "StrengthIncreaseExponentCoefficientOfVariation, " +
                "StrengthIncreaseExponentShift, " +
                "PopDistribution, " +
                "PopMean, " +
                "PopCoefficientOfVariation, " +
                "PopShift, " +
                "sp2d.SP2D_ID AS SoilProfileId " +
                "FROM Mechanism AS m " +
                "JOIN Segment AS segment USING(ME_ID) " +
                "JOIN " +
                "(" +
                "SELECT SSM_ID, SP2D_ID " +
                "FROM StochasticSoilProfile " +
                "GROUP BY SSM_ID, SP2D_ID" +
                ") ssp USING(SSM_ID) " +
                "JOIN SoilProfile2D sp2d USING(SP2D_ID) " +
                "JOIN " +
                "(" +
                "SELECT SP2D_ID, COUNT(*) AS LayerCount " +
                "FROM SoilLayer2D " +
                "GROUP BY SP2D_ID" +
                ") LayerCount USING(SP2D_ID) " +
                "JOIN SoilLayer2D sl2d USING(SP2D_ID) " +
                "LEFT JOIN MechanismPointLocation mpl USING(ME_ID, SP2D_ID) " +
                "LEFT JOIN " +
                "(" +
                "SELECT " +
                "mat.MA_ID, mat.MA_Name AS MaterialName, " +
                "max(case when pn.PN_Name = 'Color' then pv.PV_Value end) AS Color, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Dist_Type end) AS BelowPhreaticLevelDistribution, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Shift end) AS BelowPhreaticLevelShift, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Mean end) AS BelowPhreaticLevelMean, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Deviation end) AS BelowPhreaticLevelDeviation, " +
                "max(case when pn.PN_Name = 'BelowPhreaticLevelStochast' then s.ST_Variation end) AS BelowPhreaticLevelCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Dist_Type end) AS PermeabKxDistribution, " +
                "max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Shift end) AS PermeabKxShift, " +
                "max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Mean end) AS PermeabKxMean, " +
                "max(case when pn.PN_Name = 'PermeabKxStochast' then s.ST_Variation end) AS PermeabKxCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Dist_Type end) AS DiameterD70Distribution, " +
                "max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Shift end) AS DiameterD70Shift, " +
                "max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Mean end) AS DiameterD70Mean, " +
                "max(case when pn.PN_Name = 'DiameterD70Stochast' then s.ST_Variation end) AS DiameterD70CoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'UsePop' then pv.PV_Value end) AS UsePOP, " +
                "max(case when pn.PN_Name = 'ShearStrengthModel' then pv.PV_Value end) AS ShearStrengthModel, " +
                "max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Dist_Type end) AS AbovePhreaticLevelDistribution, " +
                "max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Shift end) AS AbovePhreaticLevelShift, " +
                "max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Mean end) AS AbovePhreaticLevelMean, " +
                "max(case when pn.PN_Name = 'AbovePhreaticLevelStochast' then s.ST_Variation end) AS AbovePhreaticLevelCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Dist_Type end) AS CohesionDistribution, " +
                "max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Shift end) AS CohesionShift, " +
                "max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Mean end) AS CohesionMean, " +
                "max(case when pn.PN_Name = 'CohesionStochast' then s.ST_Variation end) AS CohesionCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Dist_Type end) AS FrictionAngleDistribution, " +
                "max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Shift end) AS FrictionAngleShift, " +
                "max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Mean end) AS FrictionAngleMean, " +
                "max(case when pn.PN_Name = 'FrictionAngleStochast' then s.ST_Variation end) AS FrictionAngleCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Dist_Type end) AS ShearStrengthRatioDistribution, " +
                "max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Shift end) AS ShearStrengthRatioShift, " +
                "max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Mean end) AS ShearStrengthRatioMean, " +
                "max(case when pn.PN_Name = 'RatioCuPcStochast' then s.ST_Variation end) AS ShearStrengthRatioCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Dist_Type end) AS StrengthIncreaseExponentDistribution, " +
                "max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Shift end) AS StrengthIncreaseExponentShift, " +
                "max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Mean end) AS StrengthIncreaseExponentMean, " +
                "max(case when pn.PN_Name = 'StrengthIncreaseExponentStochast' then s.ST_Variation end) AS StrengthIncreaseExponentCoefficientOfVariation, " +
                "max(case when pn.PN_Name = 'POPStochast' then s.ST_Dist_Type end) AS PopDistribution, " +
                "max(case when pn.PN_Name = 'POPStochast' then s.ST_Shift end) AS PopShift, " +
                "max(case when pn.PN_Name = 'POPStochast' then s.ST_Mean end) AS PopMean, " +
                "max(case when pn.PN_Name = 'POPStochast' then s.ST_Variation end) AS PopCoefficientOfVariation " +
                "FROM ParameterNames AS pn " +
                "LEFT JOIN ParameterValues AS pv USING(PN_ID) " +
                "LEFT JOIN Stochast AS s USING(PN_ID) " +
                "JOIN Materials AS mat " +
                "WHERE pv.MA_ID = mat.MA_ID OR s.MA_ID = mat.MA_ID " +
                "GROUP BY mat.MA_ID " +
                ") materialProperties USING(MA_ID) " +
                "LEFT JOIN " +
                "(" +
                "SELECT SL2D_ID, PV_Value AS IsAquifer " +
                "FROM ParameterNames " +
                "JOIN LayerParameterValues USING(PN_ID) " +
                "WHERE PN_NAME = 'IsAquifer'" +
                ") USING(SL2D_ID) " +
                "GROUP BY sp2d.SP2D_ID, sl2d.SL2D_ID;";
            Assert.AreEqual(expectedQuery, query);
        }
    }
}
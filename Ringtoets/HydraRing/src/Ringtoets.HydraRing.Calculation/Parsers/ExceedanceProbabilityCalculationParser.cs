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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for the output of Hydra-Ring calculations that compute a probability of failure.
    /// </summary>
    public class ExceedanceProbabilityCalculationParser : IHydraRingFileParser
    {
        private const string betaResultQuery = "SELECT BetaId, RingCombinMethod, PresentationSectionId, MainMechanismId, MainMechanismCombinMethod, MechanismId, LayerId, AlternativeId, Beta " +
                                               "FROM BetaResults " +
                                               "WHERE SectionId = @SectionId " +
                                               "ORDER BY BetaId DESC LIMIT 0,1;";

        private const string alphaResultsQuery = "SELECT RingCombinMethod, PresentationSectionId, MainMechanismId, MainMechanismCombinMethod, MechanismId, LayerId, AlternativeId, VariableId, LoadVariableId, Alpha " +
                                                 "FROM AlphaResults " +
                                                 "WHERE BetaId = @BetaId " +
                                                 "ORDER BY BetaId, VariableId, LoadVariableId;";

        /// <summary>
        /// Gets the output of a successful parse of the output file.
        /// </summary>
        /// <returns>A <see cref="ExceedanceProbabilityCalculationOutput"/> corresponding to the section id if <see cref="Parse"/> executed
        /// successfully; or <c>null</c> otherwise.</returns>
        public ExceedanceProbabilityCalculationOutput Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            try
            {
                Output = DoParse(Path.Combine(workingDirectory, HydraRingFileConstants.WorkingDatabaseFileName), sectionId);
            }
            catch
            {
                throw new HydraRingFileParserException();
            }
        }

        private static ExceedanceProbabilityCalculationOutput DoParse(string outputFilePath, int sectionId)
        {
            using (var sqLiteConnection = CreateConnection(outputFilePath))
            {
                sqLiteConnection.Open();

                int betaId;
                var exceedanceProbabilityCalculationOutput = ReadExceedanceProbabilityCalculationOutput(sectionId, sqLiteConnection, out betaId);

                foreach (var alpha in ReadExceedanceProbabilityCalculationAlphaOutput(sectionId, betaId, sqLiteConnection))
                {
                    exceedanceProbabilityCalculationOutput.Alphas.Add(alpha);
                }

                return exceedanceProbabilityCalculationOutput;
            }
        }

        private static SQLiteDataReader CreateDataReader(SQLiteConnection connection, string queryString, params SQLiteParameter[] parameters)
        {
            using (var query = new SQLiteCommand(connection)
            {
                CommandText = queryString
            })
            {
                query.Parameters.AddRange(parameters);
                return query.ExecuteReader();
            }
        }

        private static SQLiteConnection CreateConnection(string databaseFile)
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = databaseFile,
                ReadOnly = true
            }.ConnectionString;

            return new SQLiteConnection(connectionStringBuilder);
        }

        #region Read AlphaResults

        private static IList<ExceedanceProbabilityCalculationAlphaOutput> ReadExceedanceProbabilityCalculationAlphaOutput(int sectionId, int betaId, SQLiteConnection sqLiteConnection)
        {
            var sectionIdParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = "@sectionId",
                Value = sectionId
            };
            var betaIdParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = "@BetaId",
                Value = betaId
            };
            using (var sqLiteDataReader = CreateDataReader(sqLiteConnection, alphaResultsQuery, sectionIdParameter, betaIdParameter))
            {
                return ReadAlphaResults(sectionId, sqLiteDataReader);
            }
        }

        private static IList<ExceedanceProbabilityCalculationAlphaOutput> ReadAlphaResults(int sectionId, SQLiteDataReader sqLiteDataReader)
        {
            var alphaResults = new List<ExceedanceProbabilityCalculationAlphaOutput>();
            while (sqLiteDataReader.Read())
            {
                alphaResults.Add(ReadExceedanceProbabilityCalculationAlphaOutput(sectionId, sqLiteDataReader));
            }
            return alphaResults;
        }

        private static ExceedanceProbabilityCalculationAlphaOutput ReadExceedanceProbabilityCalculationAlphaOutput(int sectionId, SQLiteDataReader sqLiteDataReader)
        {
            var ringCombinMethod = Convert.ToInt32(sqLiteDataReader["RingCombinMethod"]);
            var presentationSectionId = Convert.ToInt32(sqLiteDataReader["PresentationSectionId"]);
            var mainMechanismId = Convert.ToInt32(sqLiteDataReader["MainMechanismId"]);
            var mainMechanismCombinMethod = Convert.ToInt32(sqLiteDataReader["MainMechanismCombinMethod"]);
            var mechanismId = Convert.ToInt32(sqLiteDataReader["MechanismId"]);
            var layerId = Convert.ToInt32(sqLiteDataReader["LayerId"]);
            var alternativeId = Convert.ToInt32(sqLiteDataReader["AlternativeId"]);
            var variableId = Convert.ToInt32(sqLiteDataReader["VariableId"]);
            var loadVariableId = Convert.ToInt32(sqLiteDataReader["LoadVariableId"]);
            var alpha = Convert.ToDouble(sqLiteDataReader["Alpha"]);
            return new ExceedanceProbabilityCalculationAlphaOutput(
                ringCombinMethod, presentationSectionId, mainMechanismId, mainMechanismCombinMethod,
                mechanismId, sectionId, layerId, alternativeId, variableId, loadVariableId, alpha);
        }

        #endregion

        #region Read BetaResults

        private static ExceedanceProbabilityCalculationOutput ReadExceedanceProbabilityCalculationOutput(int sectionId, SQLiteConnection sqLiteConnection, out int betaId)
        {
            var sectionIdParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = "@sectionId",
                Value = sectionId
            };
            using (var sqLiteDataReader = CreateDataReader(sqLiteConnection, betaResultQuery, sectionIdParameter))
            {
                if (!sqLiteDataReader.Read())
                {
                    throw new HydraRingFileParserException();
                }
                betaId = ReadBetaId(sqLiteDataReader);
                return ReadExceedanceProbabilityCalculationOutput(sectionId, sqLiteDataReader);
            }
        }

        private static int ReadBetaId(SQLiteDataReader sqLiteDataReader)
        {
            return Convert.ToInt32(sqLiteDataReader["BetaId"]);
        }

        private static ExceedanceProbabilityCalculationOutput ReadExceedanceProbabilityCalculationOutput(int sectionId, SQLiteDataReader sqLiteDataReader)
        {
            var ringCombinMethod = Convert.ToInt32(sqLiteDataReader["RingCombinMethod"]);
            var presentationSectionId = Convert.ToInt32(sqLiteDataReader["PresentationSectionId"]);
            var mainMechanismId = Convert.ToInt32(sqLiteDataReader["MainMechanismId"]);
            var mainMechanismCombinMethod = Convert.ToInt32(sqLiteDataReader["MainMechanismCombinMethod"]);
            var mechanismId = Convert.ToInt32(sqLiteDataReader["MechanismId"]);
            var layerId = Convert.ToInt32(sqLiteDataReader["LayerId"]);
            var alternativeId = Convert.ToInt32(sqLiteDataReader["AlternativeId"]);
            var beta = Convert.ToDouble(sqLiteDataReader["Beta"]);
            return new ExceedanceProbabilityCalculationOutput(
                ringCombinMethod, presentationSectionId, mainMechanismId, mainMechanismCombinMethod,
                mechanismId, sectionId, layerId, alternativeId, beta);
        }

        #endregion
    }
}
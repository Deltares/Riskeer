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
using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.Utils;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Properties;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.HydraRing.Calculation.Parsers
{
    /// <summary>
    /// Parser for the convergence of Hydra-Ring calculations.
    /// </summary>
    public class ConvergenceParser : IHydraRingFileParser
    {
        private const string sectionIdParameterName = "@sectionId";
        private const string convergedColumnName = "Converged";

        private readonly string getLastResultQuery =
            $"SELECT ConvOnBeta OR ConvOnValue AS {convergedColumnName} " +
            "FROM IterateToGivenBetaConvergence " +
            $"WHERE SectionId = {sectionIdParameterName} " +
            "AND OuterIterationId = (SELECT MAX(OuterIterationId) FROM IterateToGivenBetaConvergence);";

        /// <summary>
        /// Gets the value indicating whether the calculation for a section has converged.
        /// </summary>
        public bool? Output { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            if (workingDirectory == null)
            {
                throw new ArgumentNullException(nameof(workingDirectory));
            }
            IOUtils.ValidateFilePath(workingDirectory);

            ParseFile(workingDirectory, sectionId);
        }

        private void ParseFile(string workingDirectory, int sectionId)
        {
            string outputDatabasePath = Path.Combine(workingDirectory, HydraRingFileConstants.OutputDatabaseFileName);

            using (SQLiteConnection connection = CreateConnection(outputDatabasePath))
            {
                ReadIsConverged(connection, sectionId);
            }
        }

        private static SQLiteConnection CreateConnection(string databaseFile)
        {
            string connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                FailIfMissing = true,
                DataSource = databaseFile,
                ReadOnly = true
            }.ConnectionString;

            return new SQLiteConnection(connectionStringBuilder);
        }

        /// <summary>
        /// Reads the value indicating whether the calculation for a section has converged.
        /// </summary>
        /// <param name="sqLiteConnection">The connection to the database.</param>
        /// <param name="sectionId">The section id to get the output for.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when: 
        /// <list type="bullet">
        /// <item>the output file does not exist.</item>
        /// <item>the convergence result could not be read from the output file.</item>
        /// </list>
        /// </exception>
        private void ReadIsConverged(SQLiteConnection sqLiteConnection, int sectionId)
        {
            try
            {
                using (SQLiteDataReader reader = CreateReader(sqLiteConnection, sectionId))
                {
                    SetOutput(reader);
                }
            }
            catch (SQLiteException e)
            {
                throw new HydraRingFileParserException(Resources.Parse_Cannot_read_convergence_in_output_file, e);
            }
        }

        private SQLiteDataReader CreateReader(SQLiteConnection connection, int sectionId)
        {
            using (var command = CreateCommand(connection, sectionId))
            {
                OpenConnection(connection);
                return command.ExecuteReader();
            }
        }

        private SQLiteCommand CreateCommand(SQLiteConnection connection, int sectionId)
        {
            var command = new SQLiteCommand(getLastResultQuery, connection);
            command.Parameters.Add(new SQLiteParameter
            {
                DbType = DbType.Int64,
                ParameterName = sectionIdParameterName,
                Value = sectionId
            });
            return command;
        }

        /// <summary>
        /// Opens the connection using <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The connection to open.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when <paramref name="connection"/> could not be opened.</exception>
        private static void OpenConnection(SQLiteConnection connection)
        {
            try
            {
                connection.Open();
            }
            catch (SQLiteException e)
            {
                throw new HydraRingFileParserException(e.Message, e);
            }
        }

        /// <summary>
        /// Sets <see cref="Output"/> with the value read in the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <exception cref="HydraRingFileParserException">Thrown when no result could be read using the <paramref name="reader"/>.</exception>
        private void SetOutput(SQLiteDataReader reader)
        {
            if (reader.Read())
            {
                Output = Convert.ToBoolean(reader[convergedColumnName]);
            }
            else
            {
                throw new HydraRingFileParserException(Resources.Parse_No_convergence_found_in_output_file);
            }
        }
    }
}
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
            "AND IterationNr = (SELECT MAX(IterationNr) FROM IterateToGivenBetaConvergence);";

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
            FileUtils.ValidateFilePath(workingDirectory);

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

        private void ReadIsConverged(SQLiteConnection sqLiteConnection, int sectionId)
        {
            try
            {
                SQLiteDataReader reader = CreateReader(sqLiteConnection, sectionId);
                SetOutput(reader);
            }
            catch (SQLiteException e)
            {
                throw new HydraRingFileParserException("Er kon geen resultaat voor convergentie gelezen worden uit de Hydra-Ring uitvoerdatabase.", e);
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

        private void SetOutput(SQLiteDataReader reader)
        {
            if (reader.Read())
            {
                Output = Convert.ToBoolean(reader[convergedColumnName]);
            }
            else
            {
                throw new HydraRingFileParserException("Er is geen resultaat voor convergentie gevonden in de Hydra-Ring uitvoerdatabase.");
            }
        }
    }
}
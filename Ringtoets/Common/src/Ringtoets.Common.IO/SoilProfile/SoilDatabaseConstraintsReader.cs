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

using System;
using System.Data;
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a D-Soil Model file and validates whether it meets required constraints.
    /// </summary>
    public class SoilDatabaseConstraintsReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilDatabaseConstraintsReader"/> which will 
        /// use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters;</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public SoilDatabaseConstraintsReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Verifies that the database meets required constraints.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>Required information for constraint evaluation could not be read;</item>
        /// <item>The database segment names are not unique.</item>
        /// </list>
        /// </exception>
        public void VerifyConstraints()
        {
            try
            {
                ReadUniqueSegements();
            }
            catch (SQLiteException exception)
            {
                throw new CriticalFileReadException(
                    BuildMessageWithPath(string.Format(
                                             Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Can_not_read_StochasticSoilModel_Perhaps_table_missing,
                                             StochasticSoilModelTableDefinitions.TableName)),
                    exception);
            }

            try
            {
                ReadAllProbabilitiesValid();
            }
            catch (SQLiteException exception)
            {
                throw new CriticalFileReadException(
                    BuildMessageWithPath(string.Format(
                                             Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Can_not_read_StochasticSoilProfile_Perhaps_table_missing,
                                             StochasticSoilProfileTableDefinitions.TableName)),
                    exception);
            }
        }

        /// <summary>
        /// Verifies that the segments in the database are unique.
        /// </summary>
        /// <exception cref="SQLiteException">Thrown when the execution the query failed.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The database segments could not be read;</item>
        /// <item>The database segments are not unique.</item>
        /// </list>
        /// </exception>
        private void ReadUniqueSegements()
        {
            string query = SoilDatabaseQueryBuilder.GetSoilModelNamesUniqueQuery();
            using (IDataReader dataReader = CreateDataReader(query))
            {
                if (!dataReader.Read())
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath(Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Unexpected_error_while_verifying_unique_StochasticSoilModel_names));
                }

                if (!Convert.ToBoolean(dataReader[StochasticSoilModelTableDefinitions.AreSegmentsUnique]))
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath(Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Non_unique_StochasticSoilModel_names));
                }
            }
        }

        /// <summary>
        /// Verifies that the probabilities in the database are valid.
        /// </summary>
        /// <exception cref="SQLiteException">Thrown when the execution the query failed.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The probabilities could not be read;</item>
        /// <item>One or more probabilities are not valid.</item>
        /// </list>
        /// </exception>
        /// <remarks>A valid probability:
        /// <list type="bullet">
        /// <item>has a value in the interval [0.0, 1.0];</item>
        /// <item>is not <c>null</c>.</item>
        /// </list>
        /// </remarks>
        private void ReadAllProbabilitiesValid()
        {
            string checkSegmentNameUniqueness = SoilDatabaseQueryBuilder.GetStochasticSoilProfileProbabilitiesValidQuery();
            using (IDataReader dataReader = CreateDataReader(checkSegmentNameUniqueness))
            {
                if (!dataReader.Read())
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath(Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Unexpected_error_while_verifying_valid_StochasticSoilProfile_probability));
                }

                if (!Convert.ToBoolean(dataReader[StochasticSoilProfileTableDefinitions.AllProbabilitiesValid]))
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath(Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Invalid_StochasticSoilProfile_probability));
                }
            }
        }

        private string BuildMessageWithPath(string innerMessage)
        {
            return new FileReaderErrorMessageBuilder(Path).Build(innerMessage);
        }
    }
}
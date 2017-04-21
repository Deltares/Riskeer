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
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile.Schema;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class reads a soil database file and validates whether it meets required constraints.
    /// </summary>
    public class SoilDatabaseConstraintsReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilDatabaseConstraintsReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>The database version could not be read.</item>
        /// </list></exception>
        public SoilDatabaseConstraintsReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Verifies if the database has the required version.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item>Required information for constraint evaluation could not be read.</item>
        /// <item>The database segment names are not unique.</item>
        /// </list></exception>
        public void VerifyConstraints()
        {
            try
            {
                ReadUniqueSegements();
            }
            catch (SQLiteException exception)
            {
                string innerMessage = string.Format(
                    Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Can_not_read_StochasticSoilModel_Perhaps_table_missing,
                    StochasticSoilModelTableColumns.TableName);
                throw new CriticalFileReadException(
                    BuildMessageWithPath(innerMessage), exception);
            }

            try
            {
                ReadNoEmptyProbabilityValues();
            }
            catch (SQLiteException exception)
            {
                string innerMessage = string.Format(
                    Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Can_not_read_StochasticSoilProfile_Perhaps_table_missing,
                    StochasticSoilProfileTableColumns.TableName);
                throw new CriticalFileReadException(
                    BuildMessageWithPath(innerMessage), exception);
            }
        }

        private void ReadUniqueSegements()
        {
            string checkSegmentNameUniqueness = SoilDatabaseQueryBuilder.GetSoilModelNamesUniqueQuery();
            using (IDataReader dataReader = CreateDataReader(checkSegmentNameUniqueness))
            {
                if (!dataReader.Read())
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath(Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Unexpected_error_while_verifying_unique_StochasticSoilModel_names));
                }
                if (!Convert.ToBoolean(dataReader[StochasticSoilModelTableColumns.AreSegmentsUnique]))
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath(Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Non_unique_StochasticSoilModel_names));
                }
            }
        }

        private void ReadNoEmptyProbabilityValues()
        {
            string checkSegmentNameUniqueness = SoilDatabaseQueryBuilder.GetStochasticSoilProfileProbabilitiesValidQuery();
            using (IDataReader dataReader = CreateDataReader(checkSegmentNameUniqueness))
            {
                if (!dataReader.Read())
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath(Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Unexpected_error_while_verifying_valid_StochasticSoilProfile_probability));
                }
                if (!Convert.ToBoolean(dataReader[StochasticSoilProfileTableColumns.HasNoInvalidProbabilities]))
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath(Resources.SoilDatabaseConstraintsReader_VerifyConstraints_Invalid_StochasticSoilProfile_probability));
                }
            }
        }

        private string BuildMessageWithPath(string innerMessage)
        {
            string message = new FileReaderErrorMessageBuilder(Path)
                .Build(innerMessage);
            return message;
        }
    }
}
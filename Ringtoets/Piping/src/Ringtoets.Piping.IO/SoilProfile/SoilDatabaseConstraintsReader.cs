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
using Core.Common.IO.Exceptions;
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
                string innerMessage = $"Kan geen ondergrondmodellen lezen. Mogelijk bestaat de '{StochasticSoilModelTableColumns.TableName}' tabel niet.";
                throw new CriticalFileReadException(
                    BuildMessageWithPath(innerMessage), exception);
            }

            try
            {
                ReadNoEmptyProbabilityValues();
            }
            catch (SQLiteException exception)
            {
                string innerMessage = $"Kan geen ondergrondschematisaties lezen. Mogelijk bestaat de '{StochasticSoilProfileTableColumns.TableName}' tabel niet.";
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
                        BuildMessageWithPath("Onverwachte fout tijdens het verifiëren van unieke ondergrondmodelnamen."));
                }
                if (!Convert.ToBoolean(dataReader[StochasticSoilModelTableColumns.AreSegmentsUnique]))
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath("Namen van ondergrondmodellen zijn niet uniek."));
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
                        BuildMessageWithPath("Onverwachte fout tijdens het verifiëren van kansen van voorkomen voor profielen."));
                }
                if (!Convert.ToBoolean(dataReader[StochasticSoilProfileTableColumns.HasNoInvalidProbabilities]))
                {
                    throw new CriticalFileReadException(
                        BuildMessageWithPath("Er zijn stochastische ondergrondschematisaties zonder geldige kans van voorkomen."));
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
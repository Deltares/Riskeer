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
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile.Schema;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class reads a DSoil database file and reads <see cref="StochasticSoilProfile"/>
    /// from this database.
    /// </summary>
    public class StochasticSoilProfileReader : SqLiteDatabaseReaderBase
    {
        private IDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilProfileReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list></exception>
        public StochasticSoilProfileReader(string databaseFilePath) : base(databaseFilePath)
        {
            VerifyVersion(databaseFilePath);
            InitializeReader();
        }

        /// <summary>
        /// Gets a value indicating whether or not more stochastic soil profiles can be read 
        /// using the <see cref="StochasticSoilProfileReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Reads the information for the next stochastic soil profile from the database
        /// and creates a <see cref="StochasticSoilProfile"/> instance of the information.
        /// </summary>
        /// <param name="stochasticSoilModelId">Identifier of the next <see cref="StochasticSoilModel"/> 
        /// to look for.</param>
        /// <returns>The next <see cref="StochasticSoilProfile"/> from the database, or <c>null</c> 
        /// if no more stochastic soil profiles can be read.</returns>
        /// <exception cref="StochasticSoilProfileReadException">Thrown when the database returned 
        /// incorrect values for required properties.</exception>
        /// <remarks>Rows are being read in ascending order based on the database ID of the 
        /// stochastic soil model. Therefore once a stochastic soil model has been read already, 
        /// it will not be found with this method.</remarks>
        public StochasticSoilProfile ReadStochasticSoilProfile(long stochasticSoilModelId)
        {
            if (!HasNext)
            {
                return null;
            }

            if (!MoveToStochasticSoilModelId(stochasticSoilModelId))
            {
                return null;
            }
            try
            {
                StochasticSoilProfile stochasticSoilProfile = ReadStochasticSoilProfileProbability();
                MoveToNextStochasticSoilModelId(stochasticSoilModelId);
                return stochasticSoilProfile;
            }
            catch (SystemException exception)
            {
                if (exception is FormatException || exception is OverflowException || exception is InvalidCastException)
                {
                    string message = new FileReaderErrorMessageBuilder(Path)
                        .Build(Resources.StochasticSoilProfileDatabaseReader_StochasticSoilProfile_has_invalid_value);
                    throw new StochasticSoilProfileReadException(message, exception);
                }
                throw;
            }
        }

        public override void Dispose()
        {
            dataReader?.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Prepares a new data reader with queries for obtaining the profiles and updates
        /// the reader so that it points to the first row of the result set.
        /// </summary>
        /// <exception cref="CriticalFileReadException">A query could not be executed on 
        /// the database schema.</exception>
        private void InitializeReader()
        {
            CreateDataReader();
            MoveNext();
        }

        /// <summary>
        /// Moves the reader to the next record in the database.
        /// </summary>
        private void MoveNext()
        {
            HasNext = MoveNext(dataReader);
        }

        private void CreateDataReader()
        {
            string stochasticSoilProfileQuery = SoilDatabaseQueryBuilder.GetAllStochasticSoilProfileQuery();
            try
            {
                dataReader = CreateDataReader(stochasticSoilProfileQuery);
            }
            catch (SQLiteException exception)
            {
                CloseConnection();
                string message = new FileReaderErrorMessageBuilder(Path)
                    .Build(Resources.StochasticSoilModelDatabaseReader_Failed_to_read_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        private bool MoveToStochasticSoilModelId(long stochasticSoilModelId)
        {
            while (HasNext && ReadStochasticSoilModelId() < stochasticSoilModelId)
            {
                MoveNext();
            }
            if (HasNext && ReadStochasticSoilModelId() == stochasticSoilModelId)
            {
                return true;
            }
            MoveToNextStochasticSoilModelId(stochasticSoilModelId);
            return false;
        }

        private void MoveToNextStochasticSoilModelId(long stochasticSoilModelId)
        {
            MoveNext();
            if (HasNext)
            {
                HasNext = ReadStochasticSoilModelId() == stochasticSoilModelId;
            }
        }

        private void VerifyVersion(string databaseFilePath)
        {
            using (var versionReader = new SoilDatabaseVersionReader(databaseFilePath))
            {
                try
                {
                    versionReader.VerifyVersion();
                }
                catch (CriticalFileReadException)
                {
                    CloseConnection();
                    throw;
                }
            }
        }

        private long ReadStochasticSoilModelId()
        {
            return Convert.ToInt64(dataReader[StochasticSoilProfileTableColumns.StochasticSoilModelId]);
        }

        private StochasticSoilProfile ReadStochasticSoilProfileProbability()
        {
            var valueProbability = dataReader[StochasticSoilProfileTableColumns.Probability];
            var probability = valueProbability.Equals(DBNull.Value) ? 0 : Convert.ToDouble(valueProbability);

            var soilProfile1DId = ReadSoilProfile1DId(probability);
            if (soilProfile1DId != null)
            {
                return soilProfile1DId;
            }

            var soilProfile2DId = ReadSoilProfile2DId(probability);
            if (soilProfile2DId != null)
            {
                return soilProfile2DId;
            }

            var message = new FileReaderErrorMessageBuilder(Path)
                .Build(Resources.StochasticSoilProfileDatabaseReader_StochasticSoilProfile_has_invalid_value);
            throw new StochasticSoilProfileReadException(message);
        }

        private StochasticSoilProfile ReadSoilProfile2DId(double probability)
        {
            var valueSoilProfile2DId = dataReader[StochasticSoilProfileTableColumns.SoilProfile2DId];
            if (!valueSoilProfile2DId.Equals(DBNull.Value))
            {
                var soilProfileId = Convert.ToInt64(valueSoilProfile2DId);
                return new StochasticSoilProfile(probability, SoilProfileType.SoilProfile2D, soilProfileId);
            }
            return null;
        }

        private StochasticSoilProfile ReadSoilProfile1DId(double probability)
        {
            var valueSoilProfile1DId = dataReader[StochasticSoilProfileTableColumns.SoilProfile1DId];
            if (!valueSoilProfile1DId.Equals(DBNull.Value))
            {
                var soilProfileId = Convert.ToInt64(valueSoilProfile1DId);
                return new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, soilProfileId);
            }
            return null;
        }
    }
}
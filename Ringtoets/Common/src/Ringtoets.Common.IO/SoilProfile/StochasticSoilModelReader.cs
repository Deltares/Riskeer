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

using System.Data;
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a DSoil database file and reads stochastic soil model from this database.
    /// </summary>
    public class StochasticSoilModelReader : SqLiteDatabaseReaderBase
    {
        private IDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public StochasticSoilModelReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Validates the database.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The database version could not be read;</item>
        /// <item>The database version is incorrect;</item>
        /// <item>Required information for constraint evaluation could not be read;</item>
        /// <item>The database segment names are not unique;</item>
        /// <item>Failed to fetch stochastic soil models from the database.</item>
        /// </list>
        /// </exception>
        public void Validate()
        {
            VerifyVersion(Path);
            VerifyConstraints(Path);
            InitializeReader();
        }

        protected override void Dispose(bool disposing)
        {
            if (dataReader != null)
            {
                dataReader.Close();
                dataReader.Dispose();
                dataReader = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets a value indicating whether or not more stochastic soil models can be read using 
        /// the <see cref="StochasticSoilModelReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="SQLiteDataReader"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when failed to fetch stochastic soil models from the database.</exception>
        private void InitializeReader()
        {
            CreateDataReader();
            MoveNext();
        }

        private void MoveNext()
        {
            HasNext = MoveNext(dataReader);
        }

        /// <summary>
        /// Creates a new <see cref="SQLiteDataReader"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when failed to fetch stochastic soil models from the database.</exception>
        private void CreateDataReader()
        {
            string stochasticSoilModelSegmentsQuery = SoilDatabaseQueryBuilder.GetStochasticSoilModelOfMechanismQuery();
            try
            {
                dataReader = CreateDataReader(stochasticSoilModelSegmentsQuery);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.StochasticSoilModelDatabaseReader_Failed_to_read_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Verifies that the database at <paramref name="databaseFilePath"/> has the required version.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The database version could not be read;</item>
        /// <item>The database version is incorrect.</item>
        /// </list>
        /// </exception>
        private static void VerifyVersion(string databaseFilePath)
        {
            using (var reader = new SoilDatabaseVersionReader(databaseFilePath))
            {
                reader.VerifyVersion();
            }
        }

        /// <summary>
        /// Verifies that the database at <paramref name="databaseFilePath"/> meets required constraints.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>Required information for constraint evaluation could not be read;</item>
        /// <item>The database segment names are not unique.</item>
        /// </list>
        /// </exception>
        private static void VerifyConstraints(string databaseFilePath)
        {
            using (var reader = new SoilDatabaseConstraintsReader(databaseFilePath))
            {
                reader.VerifyConstraints();
            }
        }
    }
}
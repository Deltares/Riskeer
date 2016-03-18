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
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class reads a DSoil database file and reads <see cref="StochasticSoilModel"/> from this database.
    /// </summary>
    public class StochasticSoilModelReader : SqLiteDatabaseReaderBase
    {
        private const string pipingMechanismName = "Piping";

        private SQLiteDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list></exception>
        public StochasticSoilModelReader(string databaseFilePath) : base(databaseFilePath)
        {
            VerifyVersion(databaseFilePath);
            InitializeReader();
        }

        /// <summary>
        /// Gets a value indicating whether or not more stochastic soil profiles can be read using 
        /// the <see cref="StochasticSoilModelReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        public StochasticSoilModel ReadStochasticSoilModel()
        {
            try
            {
                return ReadPipingStochasticSoilModel();
            }
            catch (SystemException exception)
            {
                if (exception is FormatException || exception is OverflowException || exception is InvalidCastException)
                {
                    var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.StochasticSoilProfileDatabaseReader_StochasticSoilProfile_has_invalid_value);
                    throw new StochasticSoilProfileReadException(message, exception);
                }
                throw;
            }
        }

        public override void Dispose()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
            }
            base.Dispose();
        }

        private StochasticSoilModel ReadPipingStochasticSoilModel()
        {
            var stochasticSoilModelSegment = ReadStochasticSoilModelSegment();
            var currentSegmentSoilModelId = stochasticSoilModelSegment.SegmentSoilModelId;
            do
            {
                // Read Points
                var point2D = ReadSegmentPoint(dataReader);
                if (point2D != null)
                {
                    stochasticSoilModelSegment.Geometry.Add(point2D);
                }
                MoveNext();
            } while (HasNext && ReadStochasticSoilModelSegment().SegmentSoilModelId == currentSegmentSoilModelId);

            return stochasticSoilModelSegment;
        }

        /// <summary>
        /// Prepares a new data reader with queries for obtaining the models and updates the reader
        /// so that it points to the first row of the result set.
        /// </summary>
        /// <exception cref="CriticalFileReadException">A query could not be executed on the database schema.</exception>
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
            var stochasticSoilModelSegmentsQuery = SoilDatabaseQueryBuilder.GetStochasticSoilModelOfMechanismQuery();
            var sqliteParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = String.Format("@{0}", MechanismDatabaseColumns.MechanismName),
                Value = pipingMechanismName
            };
            try
            {
                dataReader = CreateDataReader(stochasticSoilModelSegmentsQuery, sqliteParameter);
            }
            catch (SQLiteException exception)
            {
                CloseConnection();
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.StochasticSoilModelDatabaseReader_Failed_to_read_database);
                throw new CriticalFileReadException(message, exception);
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

        private StochasticSoilModel ReadStochasticSoilModelSegment()
        {
            var stochasticSoilModelId = Convert.ToInt64(dataReader[StochasticSoilModelDatabaseColumns.StochasticSoilModelId]);
            var stochasticSoilModelName = Convert.ToString(dataReader[StochasticSoilModelDatabaseColumns.StochasticSoilModelName]);
            var segmentName = Convert.ToString(dataReader[SegmentDatabaseColumns.SegmentName]);
            return new StochasticSoilModel(stochasticSoilModelId, stochasticSoilModelName, segmentName);
        }

        private static Point2D ReadSegmentPoint(SQLiteDataReader dataReader)
        {
            double coordinateX = Convert.ToDouble(dataReader[SegmentPointsDatabaseColumns.CoordinateX]);
            double coordinateY = Convert.ToDouble(dataReader[SegmentPointsDatabaseColumns.CoordinateY]);
            return new Point2D(coordinateX, coordinateY);
        }
    }
}
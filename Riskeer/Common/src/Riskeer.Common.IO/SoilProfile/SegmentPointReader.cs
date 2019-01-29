// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.Properties;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a D-Soil Model file and reads segment points from this database.
    /// </summary>
    public class SegmentPointReader : SqLiteDatabaseReaderBase
    {
        private IDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="SegmentPointReader"/> which will use the 
        /// <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters;</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public SegmentPointReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets a value indicating whether or not more stochastic soil model geometry can be 
        /// read using the <see cref="SegmentPointReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the query to fetch stochastic 
        /// soil model segment points from the database failed.</exception>
        public void Initialize()
        {
            CreateDataReader();
            MoveNext();
        }

        /// <summary>
        /// Reads the segment points corresponding to the stochastic soil model.
        /// </summary>
        /// <returns>The segment points corresponding to the stochastic soil model.</returns>
        /// <exception cref="StochasticSoilModelException">Thrown when the geometry could not be read.</exception>
        public IEnumerable<Point2D> ReadSegmentPoints()
        {
            return !HasNext
                       ? Enumerable.Empty<Point2D>()
                       : TryReadSegmentPoints();
        }

        /// <summary>
        /// Reads the stochastic soil model id from the data reader.
        /// </summary>
        /// <returns>The stochastic soil model id.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the data reader does not point to a row.</exception>
        public long ReadStochasticSoilModelId()
        {
            if (!HasNext)
            {
                throw new InvalidOperationException("The reader does not have a row to read.");
            }

            return Convert.ToInt64(dataReader[StochasticSoilModelTableDefinitions.StochasticSoilModelId]);
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
        /// Tries to read the segment points corresponding to the stochastic soil model.
        /// </summary>
        /// <returns>The segment points corresponding to the stochastic soil model.</returns>
        /// <exception cref="StochasticSoilModelException">Thrown when the geometry could not be read.</exception>
        private IEnumerable<Point2D> TryReadSegmentPoints()
        {
            var segmentPoints = new List<Point2D>();
            long currentStochasticSoilModelId = ReadStochasticSoilModelId();
            try
            {
                while (HasNext && ReadStochasticSoilModelId() == currentStochasticSoilModelId)
                {
                    segmentPoints.Add(ReadSegmentPoint());
                    MoveNext();
                }
            }
            catch (StochasticSoilModelException)
            {
                MoveToNextStochasticSoilModel();
                throw;
            }

            return segmentPoints;
        }

        /// <summary>
        /// Moves the reader to the next stochastic soil model.
        /// </summary>
        private void MoveToNextStochasticSoilModel()
        {
            long currentStochasticSoilModelId = ReadStochasticSoilModelId();
            while (HasNext && currentStochasticSoilModelId.Equals(ReadStochasticSoilModelId()))
            {
                MoveNext();
            }
        }

        private string ReadStochasticSoilModelSegmentName()
        {
            return Convert.ToString(dataReader[StochasticSoilModelTableDefinitions.StochasticSoilModelName]);
        }

        /// <summary>
        /// Reads the segment point from the database.
        /// </summary>
        /// <returns>The segment point.</returns>
        /// <exception cref="StochasticSoilModelException">Thrown when the geometry could not be read.</exception>
        private Point2D ReadSegmentPoint()
        {
            object coordinateX = dataReader[SegmentPointsTableDefinitions.CoordinateX];
            object coordinateY = dataReader[SegmentPointsTableDefinitions.CoordinateY];
            if (coordinateX == Convert.DBNull || coordinateY == Convert.DBNull)
            {
                throw new StochasticSoilModelException(
                    string.Format(Resources.SegmentPointReader_ReadSegmentPoint_StochasticSoilModel_0_must_contain_geometry,
                                  ReadStochasticSoilModelSegmentName()));
            }

            double coordinateXValue = Convert.ToDouble(coordinateX);
            double coordinateYValue = Convert.ToDouble(coordinateY);
            return new Point2D(coordinateXValue, coordinateYValue);
        }

        /// <summary>
        /// Creates a new <see cref="SQLiteDataReader"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the query to fetch stochastic soil 
        /// model segment points from the database failed.</exception>
        private void CreateDataReader()
        {
            string stochasticSoilModelSegmentsQuery = SoilDatabaseQueryBuilder.GetSegmentPointsQuery();
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

        private void MoveNext()
        {
            HasNext = MoveNext(dataReader);
        }
    }
}
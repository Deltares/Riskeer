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
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a D-Soil Model file and reads segment points from this database.
    /// </summary>
    public class SegmentPointReader : SqLiteDatabaseReaderBase
    {
        private IDataReader dataReader;
        private long currentStochasticSoilModelId = -1;

        /// <summary>
        /// Creates a new instance of <see cref="SegmentPointReader"/>, 
        /// that will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public SegmentPointReader(string databaseFilePath) : base(databaseFilePath) {}

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
            while (HasNext && ReadStochasticSoilModelId() == currentStochasticSoilModelId)
            {
                yield return ReadSegmentPoint();
                MoveNext();
            }
        }

        /// <summary>
        /// Moves the reader to the stochastic soil model with id <paramref name="stochasticSoilModelId"/>.
        /// </summary>
        /// <param name="stochasticSoilModelId">The id of the stochastic soil model.</param>
        /// <returns><c>true</c> if the reader was moved to the stochastic soil model with id 
        /// <paramref name="stochasticSoilModelId"/> successfully, <c>false</c> otherwise.</returns>
        public bool MoveToStochasticSoilModel(long stochasticSoilModelId)
        {
            while (HasNext && ReadStochasticSoilModelId() <= stochasticSoilModelId)
            {
                if (ReadStochasticSoilModelId() == stochasticSoilModelId)
                {
                    currentStochasticSoilModelId = stochasticSoilModelId;
                    return true;
                }
                MoveNext();
            }
            return false;
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

        private bool HasNext { get; set; }

        private long ReadStochasticSoilModelId()
        {
            return Convert.ToInt64(dataReader[StochasticSoilModelTableDefinitions.StochasticSoilModelId]);
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
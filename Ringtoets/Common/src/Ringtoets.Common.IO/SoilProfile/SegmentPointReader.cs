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
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a DSoil database file and reads segment points from this database.
    /// </summary>
    public class SegmentPointReader : SqLiteDatabaseReaderBase
    {
        private IDataReader dataReader;

        public SegmentPointReader(string databaseFilePath) : base(databaseFilePath) {}

        public void Initialize()
        {
            InitializeReader();
        }

        public IEnumerable<Point2D> ReadSegmentPoints(long stochasticSoilModelId)
        {
            while (HasNext && ReadStochasticSoilModelSegmentId() == stochasticSoilModelId)
            {
                yield return ReadSegmentPoint();
                MoveNext();
            }
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

        private long ReadStochasticSoilModelSegmentId()
        {
            return Convert.ToInt64(dataReader[StochasticSoilModelTableDefinitions.StochasticSoilModelId]);
        }

        private Point2D ReadSegmentPoint()
        {
            double coordinateX = Convert.ToDouble(dataReader[SegmentPointsTableDefinitions.CoordinateX]);
            double coordinateY = Convert.ToDouble(dataReader[SegmentPointsTableDefinitions.CoordinateY]);
            return new Point2D(coordinateX, coordinateY);
        }

        private void InitializeReader()
        {
            CreateDataReader();
            MoveNext();
        }

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
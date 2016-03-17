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
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class reads a soil database file and reads <see cref="StochasticSoilModelSegment"/> from this database.
    /// </summary>
    public class StochasticSoilModelDatabaseReader : SqLiteDatabaseReaderBase
    {
        private const string databaseRequiredVersion = "15.0.5.0";
        private const string pipingMechanismName = "Piping";

        private static readonly ILog log = LogManager.GetLogger(typeof(StochasticSoilModelDatabaseReader));

        private SQLiteDataReader stochasticSoilProfilesDataReader;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelDatabaseReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>The database version could not be read.</item>
        /// <item>The database version is incorrect.</item>
        /// </list></exception>
        public StochasticSoilModelDatabaseReader(string databaseFilePath) : base(databaseFilePath)
        {
            VerifyVersion();
        }

        /// <summary>
        /// Reads the <see cref="StochasticSoilModelSegment"/> of failure mechanism "Piping" from the databae.
        /// </summary>
        /// <returns>List of <see cref="StochasticSoilModelSegment"/>, read from the database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when failed to read the database.</exception>
        public IEnumerable<StochasticSoilModelSegment> GetStochasticSoilModelSegmentOfPiping()
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
                if (!PrepareStochasticSoilProfilesDataReader())
                {
                    return null;
                }
                using (SQLiteDataReader dataReader = CreateDataReader(stochasticSoilModelSegmentsQuery, sqliteParameter))
                {
                    return ReadStochasticSoilModels(dataReader);
                }
            }
            catch (SQLiteException exception)
            {
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.StochasticSoilModelDatabaseReader_failed_to_read_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        public override void Dispose()
        {
            if (stochasticSoilProfilesDataReader != null)
            {
                stochasticSoilProfilesDataReader.Dispose();
            }
            base.Dispose();
        }

        private IEnumerable<StochasticSoilModelSegment> ReadStochasticSoilModels(SQLiteDataReader dataReader)
        {
            var segmentSoilModels = new List<StochasticSoilModelSegment>();
            StochasticSoilModelSegment currentStochasticSoilModelSegment = null;
            while (MoveNext(dataReader))
            {
                // Read Points
                var point2D = ReadSegmentPoint(dataReader);
                if (point2D == null)
                {
                    continue;
                }

                // Read SoilModels
                var segmentSoilModel = ReadSoilModels(dataReader);
                if (currentStochasticSoilModelSegment == null ||
                    segmentSoilModel.SegmentSoilModelId != currentStochasticSoilModelSegment.SegmentSoilModelId)
                {
                    currentStochasticSoilModelSegment = segmentSoilModel;

                    var probabilityList = ReadProbability(currentStochasticSoilModelSegment.SegmentSoilModelId);
                    if (probabilityList == null)
                    {
                        // Probability could not be read, ignore StochasticSoilModelSegment
                        continue;
                    }
                    foreach (var probability in probabilityList)
                    {
                        currentStochasticSoilModelSegment.StochasticSoilProfileProbabilities.Add(probability);
                    }

                    // Add
                    segmentSoilModels.Add(segmentSoilModel);
                }
                currentStochasticSoilModelSegment.SegmentPoints.Add(point2D);
            }
            return segmentSoilModels;
        }

        private bool PrepareStochasticSoilProfilesDataReader()
        {
            var stochasticSoilModelSegmentsQuery = SoilDatabaseQueryBuilder.GetAllStochasticSoilProfileQuery();
            stochasticSoilProfilesDataReader = CreateDataReader(stochasticSoilModelSegmentsQuery);

            if (!stochasticSoilProfilesDataReader.HasRows)
            {
                return false;
            }
            MoveNext(stochasticSoilProfilesDataReader);
            return true;
        }

        private IEnumerable<StochasticSoilProfileProbability> ReadProbability(long stochasticSoilModelId)
        {
            var probabilityList = new List<StochasticSoilProfileProbability>();
            try
            {
                long currentStochasticSoilModelId;
                do
                {
                    currentStochasticSoilModelId = ReadStochasticSoilModelId(stochasticSoilProfilesDataReader);
                    if (currentStochasticSoilModelId == stochasticSoilModelId)
                    {
                        probabilityList.Add(ReadStochasticSoilProfileProbability(stochasticSoilProfilesDataReader));
                    }
                    if (currentStochasticSoilModelId <= stochasticSoilModelId)
                    {
                        MoveNext(stochasticSoilProfilesDataReader);
                    }
                } while (currentStochasticSoilModelId <= stochasticSoilModelId && stochasticSoilProfilesDataReader.HasRows);
            }
            catch (Exception exception)
            {
                if (exception is FormatException || exception is OverflowException || exception is InvalidCastException)
                {
                    log.Warn(Resources.StochasticSoilModelDatabaseReader_failed_to_read_soil_model);
                    return null;
                }
                throw;
            }
            return probabilityList;
        }

        private static long ReadStochasticSoilModelId(SQLiteDataReader dataReader)
        {
            return Convert.ToInt64(dataReader[StochasticSoilProfileDatabaseColumns.StochasticSoilModelId]);
        }

        private static StochasticSoilProfileProbability ReadStochasticSoilProfileProbability(SQLiteDataReader dataReader)
        {
            var valueSoilProfile1DId = dataReader[StochasticSoilProfileDatabaseColumns.SoilProfile1DId];
            var valueSoilProfile2DId = dataReader[StochasticSoilProfileDatabaseColumns.SoilProfile2DId];
            var valueProbability = dataReader[StochasticSoilProfileDatabaseColumns.Probability];

            var probability = (valueProbability.Equals(DBNull.Value)) ? 0 : Convert.ToDouble(valueProbability);

            if (!valueSoilProfile1DId.Equals(DBNull.Value))
            {
                var soilProfileId = Convert.ToInt64(valueSoilProfile1DId);
                return new StochasticSoilProfileProbability(probability, SoilProfileType.SoilProfile1D, soilProfileId);
            }
            if (!valueSoilProfile2DId.Equals(DBNull.Value))
            {
                var soilProfileId = Convert.ToInt64(valueSoilProfile2DId);
                return new StochasticSoilProfileProbability(probability, SoilProfileType.SoilProfile2D, soilProfileId);
            }
            return null;
        }

        private StochasticSoilModelSegment ReadSoilModels(SQLiteDataReader dataReader)
        {
            var stochasticSoilModelId = Convert.ToInt64(dataReader[StochasticSoilModelDatabaseColumns.StochasticSoilModelId]);
            var stochasticSoilModelName = Convert.ToString(dataReader[StochasticSoilModelDatabaseColumns.StochasticSoilModelName]);
            var segmentName = Convert.ToString(dataReader[SegmentDatabaseColumns.SegmentName]);
            return new StochasticSoilModelSegment(stochasticSoilModelId, stochasticSoilModelName, segmentName);
        }

        private void VerifyVersion()
        {
            var checkVersionQuery = SoilDatabaseQueryBuilder.GetCheckVersionQuery();
            var sqliteParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = String.Format("@{0}", MetaDataDatabaseColumns.Value),
                Value = databaseRequiredVersion
            };

            try
            {
                using (SQLiteDataReader dataReader = CreateDataReader(checkVersionQuery, sqliteParameter))
                {
                    if (!dataReader.HasRows)
                    {
                        Dispose();
                        throw new CriticalFileReadException(String.Format(
                            Resources.PipingSoilProfileReader_Database_incorrect_version_requires_Version_0_,
                            databaseRequiredVersion));
                    }
                }
            }
            catch (SQLiteException exception)
            {
                Dispose();
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.StochasticSoilModelDatabaseReader_failed_to_read_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        private Point2D ReadSegmentPoint(SQLiteDataReader dataReader)
        {
            try
            {
                double coordinateX = Convert.ToDouble(dataReader[SegmentPointsDatabaseColumns.CoordinateX]);
                double coordinateY = Convert.ToDouble(dataReader[SegmentPointsDatabaseColumns.CoordinateY]);
                return new Point2D(coordinateX, coordinateY);
            }
            catch (Exception exception)
            {
                if (exception is FormatException || exception is OverflowException || exception is InvalidCastException)
                {
                    log.Warn(Resources.StochasticSoilModelDatabaseReader_SegmentPoint_has_invalid_value);
                    return null;
                }
                throw;
            }
        }
    }
}
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
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a DSoil database file and reads stochastic soil model from this database.
    /// </summary>
    public class StochasticSoilModelReader : SqLiteDatabaseReaderBase
    {
        private readonly Dictionary<long, SoilProfile1D> soilProfile1Ds = new Dictionary<long, SoilProfile1D>();
        private readonly Dictionary<long, SoilProfile2D> soilProfile2Ds = new Dictionary<long, SoilProfile2D>();
        private IDataReader dataReader;
        private SegmentPointReader segmentPointReader;

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
        /// Gets a value indicating whether or not more stochastic soil models can be read using 
        /// the <see cref="StochasticSoilModelReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Validates the database.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The database version could not be read;</item>
        /// <item>The database version is incorrect;</item>
        /// <item>Required information for constraint evaluation could not be read;</item>
        /// <item>The database segment names are not unique;</item>
        /// <item>The query to fetch stochastic soil models from the database failed.</item>
        /// </list>
        /// </exception>
        public void Validate()
        {
            VerifyVersion(Path);
            VerifyConstraints(Path);
            InitializeReaders();
            InitializeLookups();
        }

        /// <summary>
        /// Reads the information for the next stochastic soil model from the database and creates a 
        /// <see cref="StochasticSoilModel"/> instance of the information.
        /// </summary>
        /// <returns>The next <see cref="StochasticSoilModel"/> from the database, or <c>null</c> 
        /// if no more soil models can be read.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database returned incorrect 
        /// values for required properties.</exception>
        public StochasticSoilModel ReadStochasticSoilModel()
        {
            try
            {
                return TryReadStochasticSoilModel();
            }
            catch (SystemException exception) when (exception is FormatException ||
                                                    exception is OverflowException ||
                                                    exception is InvalidCastException)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.StochasticSoilProfileDatabaseReader_StochasticSoilProfile_has_invalid_value);
                throw new CriticalFileReadException(message, exception);
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

            if (segmentPointReader != null)
            {
                segmentPointReader.Dispose();
                segmentPointReader = null;
            }

            base.Dispose(disposing);
        }

        private void InitializeLookups()
        {
            using (var soilProfile1DReader = new SoilProfile1DReader(Path))
            {
                soilProfile1DReader.Initialize();

                while (soilProfile1DReader.HasNext)
                {
                    try
                    {
                        SoilProfile1D soilProfile1D = soilProfile1DReader.ReadSoilProfile();

                        long soilProfileId = soilProfile1D.Id;
                        if (!soilProfile1Ds.ContainsKey(soilProfileId))
                        {
                            soilProfile1Ds.Add(soilProfileId, soilProfile1D);
                        }
                    }
                    catch (SoilProfileReadException)
                    {
                        soilProfile1DReader.MoveNext();
                    }
                }
            }

            using (var soilProfile2DReader = new SoilProfile2DReader(Path))
            {
                soilProfile2DReader.Initialize();

                while (soilProfile2DReader.HasNext)
                {
                    try
                    {
                        SoilProfile2D soilProfile2D = soilProfile2DReader.ReadSoilProfile();

                        long soilProfileId = soilProfile2D.Id;
                        if (!soilProfile2Ds.ContainsKey(soilProfileId))
                        {
                            soilProfile2Ds.Add(soilProfileId, soilProfile2D);
                        }
                    }
                    catch (SoilProfileReadException)
                    {
                        soilProfile2DReader.MoveNext();
                    }
                }
            }
        }

        private StochasticSoilModel TryReadStochasticSoilModel()
        {
            if (!HasNext)
            {
                return null;
            }

            StochasticSoilModel stochasticSoilModel = CreateStochasticSoilModel();
            long currentSoilModelId = ReadStochasticSoilModelSegmentId();

            stochasticSoilModel.Geometry.AddRange(segmentPointReader.ReadSegmentPoints(currentSoilModelId));
            stochasticSoilModel.StochasticSoilProfiles.AddRange(ReadStochasticSoilProfiles(currentSoilModelId));

            return stochasticSoilModel;
        }

        private IEnumerable<StochasticSoilProfile> ReadStochasticSoilProfiles(long stochasticSoilModelId)
        {
            while (HasNext && ReadStochasticSoilModelSegmentId() == stochasticSoilModelId)
            {
                double probability = ReadStochasticSoilProfileProbability();

                long? soilProfile1D = ReadSoilProfile1DId();
                long? soilProfile2D = ReadSoilProfile2DId();
                if (soilProfile1D.HasValue && soilProfile1Ds.ContainsKey(soilProfile1D.Value))
                {
                    yield return new StochasticSoilProfile(probability, soilProfile1Ds[soilProfile1D.Value]);
                }
                else if (soilProfile2D.HasValue && soilProfile2Ds.ContainsKey(soilProfile2D.Value))
                {
                    yield return new StochasticSoilProfile(probability, soilProfile2Ds[soilProfile2D.Value]);
                }

                MoveNext();
            }
        }

        private double ReadStochasticSoilProfileProbability()
        {
            return Convert.ToDouble(dataReader[StochasticSoilProfileTableDefinitions.Probability]);
        }

        private long? ReadSoilProfile1DId()
        {
            object soilProfileId = dataReader[StochasticSoilProfileTableDefinitions.SoilProfile1DId];
            return soilProfileId == Convert.DBNull
                       ? (long?) null
                       : Convert.ToInt64(soilProfileId);
        }

        private long? ReadSoilProfile2DId()
        {
            object soilProfileId = dataReader[StochasticSoilProfileTableDefinitions.SoilProfile2DId];
            return soilProfileId == Convert.DBNull
                       ? (long?) null
                       : Convert.ToInt64(soilProfileId);
        }

        private long ReadStochasticSoilModelSegmentId()
        {
            return Convert.ToInt64(dataReader[StochasticSoilModelTableDefinitions.StochasticSoilModelId]);
        }

        private StochasticSoilModel CreateStochasticSoilModel()
        {
            string stochasticSoilModelName = Convert.ToString(dataReader[StochasticSoilModelTableDefinitions.StochasticSoilModelName]);
            return new StochasticSoilModel
            {
                Name = stochasticSoilModelName
            };
        }

        /// <summary>
        /// Initializes a new <see cref="SQLiteDataReader"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when failed to fetch stochastic soil models from the database.</exception>
        private void InitializeReaders()
        {
            CreateDataReader();
            MoveNext();

            segmentPointReader = new SegmentPointReader(Path);
            segmentPointReader.Initialize();
        }

        private void MoveNext()
        {
            HasNext = MoveNext(dataReader);
        }

        /// <summary>
        /// Creates a new <see cref="SQLiteDataReader"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when query to fetch stochastic soil 
        /// models from the database failed.</exception>
        private void CreateDataReader()
        {
            string stochasticSoilModelSegmentsQuery = SoilDatabaseQueryBuilder.GetStochasticSoilModelPerMechanismQuery();
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
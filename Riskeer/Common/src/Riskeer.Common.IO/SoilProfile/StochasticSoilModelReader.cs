// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Builders;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.Properties;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a D-Soil Model file and reads stochastic soil model from this database.
    /// </summary>
    public class StochasticSoilModelReader : SqLiteDatabaseReaderBase
    {
        private readonly Dictionary<long, SoilProfile1D> soilProfile1Ds = new Dictionary<long, SoilProfile1D>();
        private readonly Dictionary<long, SoilProfile2D> soilProfile2Ds = new Dictionary<long, SoilProfile2D>();

        private IDataReader dataReader;
        private SegmentPointReader segmentPointReader;
        private long currentStochasticSoilModelId = -1;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelReader"/> 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters;</item>
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
        /// Gets the amount of <see cref="StochasticSoilModel"/> that can be read from the database.
        /// </summary>
        public int StochasticSoilModelCount { get; private set; }

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
        /// <exception cref="StochasticSoilModelException">Thrown when:
        /// <list type="bullet">
        /// <item>No stochastic soil profiles could be read;</item>
        /// <item>The read failure mechanism type is not supported.</item>
        /// </list>
        /// </exception>
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

        private void MoveNext()
        {
            HasNext = MoveNext(dataReader);
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
                    catch (SoilProfileReadException e)
                    {
                        throw new StochasticSoilModelException(e.Message, e);
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
                    catch (SoilProfileReadException e)
                    {
                        throw new StochasticSoilModelException(e.Message, e);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="StochasticSoilModel"/> from the data reader.
        /// </summary>
        /// <returns>The next <see cref="StochasticSoilModel"/> from the database, or <c>null</c> 
        /// if no more soil models can be read.</returns>
        /// <exception cref="StochasticSoilModelException">Thrown when:
        /// <list type="bullet">
        /// <item>No stochastic soil profiles could be read;</item>
        /// <item>The geometry could not be read;</item>
        /// <item>The read failure mechanism type is not supported.</item>
        /// </list>
        /// </exception>
        private StochasticSoilModel TryReadStochasticSoilModel()
        {
            if (!HasNext)
            {
                return null;
            }

            StochasticSoilModel stochasticSoilModel = CreateStochasticSoilModel();
            currentStochasticSoilModelId = ReadStochasticSoilModelId();

            SetGeometry(stochasticSoilModel);
            SetStochasticSoilProfiles(stochasticSoilModel);

            return stochasticSoilModel;
        }

        /// <summary>
        /// Sets <see cref="StochasticSoilProfile"/> objects that belong to soil model.
        /// </summary>
        /// <param name="stochasticSoilModel">The stochastic soil model of which the profiles to set.</param>
        /// <exception cref="StochasticSoilModelException">Thrown when:
        /// <list type="bullet">
        /// <item>No stochastic soil profiles could be read;</item>
        /// <item>The read failure mechanism type is not supported.</item>
        /// </list>
        /// </exception>
        /// <exception cref="InvalidCastException">Thrown when the conversion is not supported.</exception>
        private void SetStochasticSoilProfiles(StochasticSoilModel stochasticSoilModel)
        {
            stochasticSoilModel.StochasticSoilProfiles.AddRange(ReadStochasticSoilProfiles());
        }

        /// <summary>
        /// Sets the geometry points of <paramref name="stochasticSoilModel"/> from the database.
        /// </summary>
        /// <param name="stochasticSoilModel">The stochastic soil model of which the geometry to set.</param>
        /// <exception cref="InvalidCastException">Thrown when the stochastic soil model id from the D-Soil Model database
        /// could not be convert.</exception>
        private void SetGeometry(StochasticSoilModel stochasticSoilModel)
        {
            if (!segmentPointReader.HasNext || segmentPointReader.ReadStochasticSoilModelId() != currentStochasticSoilModelId)
            {
                return;
            }

            stochasticSoilModel.Geometry.AddRange(segmentPointReader.ReadSegmentPoints());
        }

        /// <summary>
        /// Reads and returns <see cref="StochasticSoilProfile"/> objects that belong to soil model.
        /// </summary>
        /// <returns>The read stochastic soil profiles.</returns>
        /// <exception cref="StochasticSoilModelException">Thrown when:
        /// <list type="bullet">
        /// <item>No stochastic soil profiles could be read;</item>
        /// <item>The read failure mechanism type is not supported.</item>
        /// </list>
        /// </exception>
        /// <exception cref="InvalidCastException">Thrown when the conversion is not supported.</exception>
        private IEnumerable<StochasticSoilProfile> ReadStochasticSoilProfiles()
        {
            while (HasNext && ReadStochasticSoilModelId() == currentStochasticSoilModelId)
            {
                double? probability = ReadStochasticSoilProfileProbability();

                if (!probability.HasValue)
                {
                    MoveNext();
                    yield break;
                }

                long? soilProfile1D = ReadSoilProfile1DId();
                long? soilProfile2D = ReadSoilProfile2DId();
                if (soilProfile1D.HasValue && soilProfile1Ds.ContainsKey(soilProfile1D.Value))
                {
                    yield return new StochasticSoilProfile(probability.Value, soilProfile1Ds[soilProfile1D.Value]);
                }
                else if (soilProfile2D.HasValue && soilProfile2Ds.ContainsKey(soilProfile2D.Value))
                {
                    yield return new StochasticSoilProfile(probability.Value, soilProfile2Ds[soilProfile2D.Value]);
                }

                MoveNext();
            }
        }

        /// <summary>
        /// Creates a new basic <see cref="StochasticSoilModel"/>, based on the data read from the data reader.
        /// </summary>
        /// <returns>The newly created <see cref="StochasticSoilModel"/>.</returns>
        /// <exception cref="StochasticSoilModelException">Thrown when the read failure mechanism 
        /// type is not supported.</exception>
        /// <exception cref="InvalidCastException">Thrown when the conversion is not supported.</exception>
        private StochasticSoilModel CreateStochasticSoilModel()
        {
            return new StochasticSoilModel(ReadStochasticSoilModelName(), ReadFailureMechanismType());
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

        /// <summary>
        /// Creates a new <see cref="SQLiteDataReader"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when query to fetch stochastic soil 
        /// models from the database failed.</exception>
        private void CreateDataReader()
        {
            string stochasticSoilModelCount = SoilDatabaseQueryBuilder.GetStochasticSoilModelOfMechanismCountQuery();
            string stochasticSoilModelSegmentsQuery = SoilDatabaseQueryBuilder.GetStochasticSoilModelPerMechanismQuery();
            try
            {
                dataReader = CreateDataReader(stochasticSoilModelCount + stochasticSoilModelSegmentsQuery);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.StochasticSoilModelDatabaseReader_Failed_to_read_database);
                throw new CriticalFileReadException(message, exception);
            }

            StochasticSoilModelCount = ReadStochasticSoilModelCount();
            dataReader.NextResult();
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

        #region Read columns

        /// <summary>
        /// Reads the stochastic soil profile probability from the data reader.
        /// </summary>
        /// <returns>The 1D soil profile id.</returns>
        /// <exception cref="FormatException">The read value is not in an appropriate format.</exception>
        /// <exception cref="OverflowException">The read value represents a number that is less 
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        /// <exception cref="InvalidCastException">Thrown when the conversion to <see cref="double"/> 
        /// is not supported.</exception>
        private double? ReadStochasticSoilProfileProbability()
        {
            object probability = dataReader[StochasticSoilProfileTableDefinitions.Probability];
            return probability == Convert.DBNull
                       ? (double?) null
                       : Convert.ToDouble(probability);
        }

        /// <summary>
        /// Reads the 1D soil profile id from the data reader.
        /// </summary>
        /// <returns>The 1D soil profile id.</returns>
        /// <exception cref="FormatException">The read value is not in an appropriate format.</exception>
        /// <exception cref="OverflowException">The read value represents a number that is less 
        /// than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>.</exception>
        /// <exception cref="InvalidCastException">Thrown when the conversion to <see cref="long"/> 
        /// is not supported.</exception>
        private long? ReadSoilProfile1DId()
        {
            object soilProfileId = dataReader[StochasticSoilProfileTableDefinitions.SoilProfile1DId];
            return soilProfileId == Convert.DBNull
                       ? (long?) null
                       : Convert.ToInt64(soilProfileId);
        }

        /// <summary>
        /// Reads the 2D soil profile id from the data reader.
        /// </summary>
        /// <returns>The 2D soil profile id.</returns>
        /// <exception cref="FormatException">The read value is not in an appropriate format.</exception>
        /// <exception cref="OverflowException">The read value represents a number that is less 
        /// than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>.</exception>
        /// <exception cref="InvalidCastException">Thrown when the conversion to <see cref="long"/> 
        /// is not supported.</exception>
        private long? ReadSoilProfile2DId()
        {
            object soilProfileId = dataReader[StochasticSoilProfileTableDefinitions.SoilProfile2DId];
            return soilProfileId == Convert.DBNull
                       ? (long?) null
                       : Convert.ToInt64(soilProfileId);
        }

        private int ReadStochasticSoilModelCount()
        {
            return !dataReader.Read() ? 0 : Convert.ToInt32(dataReader[StochasticSoilModelTableDefinitions.Count]);
        }

        /// <summary>
        /// Reads the stochastic soil model id from the data reader.
        /// </summary>
        /// <returns>The stochastic soil model id.</returns>
        /// <exception cref="FormatException">The read value is not in an appropriate format.</exception>
        /// <exception cref="OverflowException">The read value represents a number that is less 
        /// than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>.</exception>
        /// <exception cref="InvalidCastException">Thrown when the conversion to <see cref="long"/> is not supported.</exception>
        private long ReadStochasticSoilModelId()
        {
            return Convert.ToInt64(dataReader[StochasticSoilModelTableDefinitions.StochasticSoilModelId]);
        }

        private string ReadStochasticSoilModelName()
        {
            return Convert.ToString(dataReader[StochasticSoilModelTableDefinitions.StochasticSoilModelName]);
        }

        private string ReadMechanismName()
        {
            return Convert.ToString(dataReader[MechanismTableDefinitions.MechanismName]);
        }

        /// <summary>
        /// Reads the failure mechanism type from the data reader.
        /// </summary>
        /// <returns>The failure mechanism type.</returns>
        /// <exception cref="StochasticSoilModelException">Thrown when the read failure mechanism type is not supported.</exception>
        /// <exception cref="InvalidCastException">Thrown when the conversion to <see cref="long"/> is not supported.</exception>
        private FailureMechanismType ReadFailureMechanismType()
        {
            long mechanismId = Convert.ToInt64(dataReader[MechanismTableDefinitions.MechanismId]);
            if (Enum.IsDefined(typeof(FailureMechanismType), mechanismId))
            {
                return (FailureMechanismType) mechanismId;
            }

            string message = string.Format(Resources.StochasticSoilModelReader_ReadFailureMechanismType_Failure_mechanism_0_not_supported, ReadMechanismName());
            throw new StochasticSoilModelException(message);
        }

        #endregion
    }
}
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
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class reads a D-Soil Model file and reads the preconsolidation stresses from this database.
    /// </summary>
    public class PreconsolidationStressReader : SqLiteDatabaseReaderBase, IRowBasedDatabaseReader
    {
        private IDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="PreconsolidationStressReader"/>
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters;</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public PreconsolidationStressReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets a value indicating whether or not more preconsolidation stresses can be read
        /// using the <see cref="PreconsolidationStressReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the query to 
        /// fetch preconsolidation stresses from the database failed.</exception>
        public void Initialize()
        {
            CreateReader();
            MoveNext();
        }

        /// <summary>
        /// Reads the preconsolidation stresses defined for the soil profile.
        /// </summary>
        /// <returns>A collection of preconsolidation stresses defined for the profile.</returns>
        /// <exception cref="SoilProfileReadException">Thrown when the preconsolidation stress
        /// could not be read.</exception>
        public IEnumerable<PreconsolidationStress> ReadPreconsolidationStresses()
        {
            if (!HasNext)
            {
                return Enumerable.Empty<PreconsolidationStress>();
            }

            var stresses = new List<PreconsolidationStress>();
            long currentSoilProfileId = ReadSoilProfileId();
            try
            {
                while (HasNext && currentSoilProfileId == ReadSoilProfileId())
                {
                    stresses.Add(ReadPreconsolidationStress());
                    MoveNext();
                }
            }
            catch (SoilProfileReadException)
            {
                MoveToNextSoilProfile();
                throw;
            }

            return stresses;
        }

        /// <summary>
        /// Reads the soil profile id from the data reader.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the data reader does not point to a row.</exception>
        public long ReadSoilProfileId()
        {
            if (!HasNext)
            {
                throw new InvalidOperationException("The reader does not have a row to read.");
            }

            return Convert.ToInt64(dataReader[SoilProfileTableDefinitions.SoilProfileId]);
        }

        public void MoveNext()
        {
            HasNext = MoveNext(dataReader);
        }

        public T Read<T>(string columnName)
        {
            return (T) dataReader[columnName];
        }

        public T ReadOrDefault<T>(string columnName)
        {
            object valueObject = dataReader[columnName];
            if (valueObject.Equals(DBNull.Value))
            {
                return default(T);
            }

            return (T) valueObject;
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
        /// Moves the reader to the next soil profile.
        /// </summary>
        private void MoveToNextSoilProfile()
        {
            long currentSoilProfileId = ReadSoilProfileId();
            while (HasNext && currentSoilProfileId == ReadSoilProfileId())
            {
                MoveNext();
            }
        }

        /// <summary>
        /// Reads the preconsolidation stress. 
        /// </summary>
        /// <returns>A <see cref="PreconsolidationStress"/> based on the read values.</returns>
        /// <exception cref="SoilProfileReadException">Thrown when the values in the database
        /// cannot be casted to the expected column types.</exception>
        private PreconsolidationStress ReadPreconsolidationStress()
        {
            string profileName = Convert.ToString(dataReader[SoilProfileTableDefinitions.ProfileName]);
            var stressReadValues = new PreconsolidationStressReadValues(this, profileName);

            var preconsolidationStress = new PreconsolidationStress();

            if (stressReadValues.XCoordinate.HasValue)
            {
                preconsolidationStress.XCoordinate = stressReadValues.XCoordinate.Value;
            }

            if (stressReadValues.ZCoordinate.HasValue)
            {
                preconsolidationStress.ZCoordinate = stressReadValues.ZCoordinate.Value;
            }

            if (stressReadValues.StressDistributionType.HasValue)
            {
                preconsolidationStress.StressDistributionType =
                    stressReadValues.StressDistributionType.Value;
            }

            if (stressReadValues.StressMean.HasValue)
            {
                preconsolidationStress.StressMean =
                    stressReadValues.StressMean.Value;
            }

            if (stressReadValues.StressCoefficientOfVariation.HasValue)
            {
                preconsolidationStress.StressCoefficientOfVariation =
                    stressReadValues.StressCoefficientOfVariation.Value;
            }

            if (stressReadValues.StressShift.HasValue)
            {
                preconsolidationStress.StressShift =
                    stressReadValues.StressShift.Value;
            }

            return preconsolidationStress;
        }

        /// <summary>
        /// Creates a new <see cref="IDataReader"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the query to fetch
        /// preconsolidation stresses from the database failed.</exception>
        private void CreateReader()
        {
            string soilProfile2DPreconsolidationStressesQuery =
                SoilDatabaseQueryBuilder.GetSoilProfile2DPreconsolidationStressesQuery();

            try
            {
                dataReader = CreateDataReader(soilProfile2DPreconsolidationStressesQuery);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.PreconsolidationStressReader_Failed_to_read_database);
                throw new CriticalFileReadException(message, exception);
            }
        }
    }
}
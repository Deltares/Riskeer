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
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// This class describes properties which are critical when reading soil profiles from a data source.
    /// If obtaining properties could not be obtained, then it is impossible to guarantee a correct import.
    /// </summary>
    internal class CriticalProfileProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="CriticalProfileProperties"/> which contains properties
        /// that are critical for reading profiles. If these properties cannot be read, then something
        /// went wrong while querying the database.
        /// </summary>
        /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> from which to obtain the critical properties.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the values in the database could not be 
        /// casted to the expected column types.</exception>
        public CriticalProfileProperties(IRowBasedDatabaseReader reader)
        {
            try
            {
                ProfileName = reader.Read<string>(SoilProfileTableDefinitions.ProfileName);
                LayerCount = reader.Read<long>(SoilProfileTableDefinitions.LayerCount);
                ProfileId = reader.Read<long>(SoilProfileTableDefinitions.SoilProfileId);
            }
            catch (InvalidCastException e)
            {
                var messageBuilder = new FileReaderErrorMessageBuilder(reader.Path);
                if (!string.IsNullOrEmpty(ProfileName))
                {
                    messageBuilder.WithSubject(string.Format(Resources.SoilProfileReader_SoilProfileName_0_, ProfileName));
                }

                string message = messageBuilder.Build(Resources.SoilProfileReader_Critical_Unexpected_value_on_column);
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// The name of the profile to read.
        /// </summary>
        public string ProfileName { get; }

        /// <summary>
        /// The number of layers that the profile to read has.
        /// </summary>
        public long LayerCount { get; }

        /// <summary>
        /// Gets the database identifier of the profile.
        /// </summary>
        public long ProfileId { get; }
    }
}
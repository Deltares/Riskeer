﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class describes properties which are critical when reading soil profiles from a data source.
    /// If obtaining properties could not be obtained, then it is impossible to guarantee a correct import.
    /// </summary>
    internal class CriticalProfileProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="CriticalProfileProperties"/>, which contains properties
        /// that are critical for reading profiles. If these properties cannot be read, then something
        /// went wrong while querying the database.
        /// </summary>
        /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> from which to obtain the critical properties.</param>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">Thrown when the values in the database could not be 
        /// casted to the expected column types.</exception>
        internal CriticalProfileProperties(IRowBasedDatabaseReader reader)
        {
            try
            {
                ProfileName = reader.Read<string>(SoilProfileDatabaseColumns.ProfileName);
                LayerCount = reader.Read<long>(SoilProfileDatabaseColumns.LayerCount);
            }
            catch (InvalidCastException e)
            {
                var messageBuilder = new FileReaderErrorMessageBuilder(reader.Path);
                if (!string.IsNullOrEmpty(ProfileName))
                {
                    messageBuilder.WithSubject(string.Format(Resources.PipingSoilProfileReader_SoilProfileName_0_, ProfileName));
                }
                var message = messageBuilder.Build(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column);
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// The name of the profile to read
        /// </summary>
        internal string ProfileName { get; private set; }

        /// <summary>
        /// The number of layers that the profile to read has
        /// </summary>
        internal long LayerCount { get; private set; }
    }
}
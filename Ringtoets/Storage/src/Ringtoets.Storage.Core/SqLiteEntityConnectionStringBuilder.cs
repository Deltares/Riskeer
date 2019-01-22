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
using System.Data.Entity.Core.EntityClient;
using Core.Common.IO;

namespace Riskeer.Storage.Core
{
    /// <summary>
    /// This class builds a connection string to a SQLite database file.
    /// </summary>
    public static class SqLiteEntityConnectionStringBuilder
    {
        /// <summary>
        /// Constructs a connection string to connect the Entity Framework to <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Location of the storage file.</param>
        /// <returns>A new connection string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is <c>null</c> or empty (only whitespaces).</exception>
        public static string BuildSqLiteEntityConnectionString(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), @"Cannot create a connection string without the path to the file to connect to.");
            }

            return new EntityConnectionStringBuilder
            {
                Metadata = string.Format(@"res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", "DbContext.RiskeerEntities"),
                Provider = @"System.Data.SQLite.EF6",
                ProviderConnectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(GetDataSourceLocation(filePath), false)
            }.ConnectionString;
        }

        private static string GetDataSourceLocation(string filePath)
        {
            return new Uri(filePath).IsUnc ? "\\\\" + filePath : filePath;
        }
    }
}
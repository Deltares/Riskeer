// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;

namespace Riskeer.Storage.Core.DbContext
{
    public class SqLiteConfiguration : DbConfiguration
    {
        public SqLiteConfiguration()
        {
            DbProviderServices providerServices = (DbProviderServices) System.Data.SQLite.EF6.SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices));

            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);

            SetProviderServices("System.Data.SQLite", providerServices);

            SetProviderFactory("System.Data.SQLite.EF6", System.Data.SQLite.EF6.SQLiteProviderFactory.Instance);

            SetProviderServices("System.Data.SQLite.EF6", providerServices);
        }
    }
}
﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="FailureMechanismSection"/> related to creating a <see cref="FailureMechanismSectionEntity"/>.
    /// </summary>
    internal static class FailureMechanismSectionCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismSectionEntity"/> based on the information of the <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="section">The section to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismSectionEntity Create(this FailureMechanismSection section, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                Name = section.Name.DeepClone(),
                FailureMechanismSectionPointXml = new Point2DXmlSerializer().ToXml(section.Points)
            };

            registry.Register(failureMechanismSectionEntity, section);

            return failureMechanismSectionEntity;
        }
    }
}
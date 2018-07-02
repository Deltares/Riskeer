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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="FailureMechanismSection"/> based on the
    /// <see cref="FailureMechanismSectionEntity"/>.
    /// </summary>
    internal static class FailureMechanismSectionEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="FailureMechanismSectionEntity"/> and use the information to construct a <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> to create <see cref="FailureMechanismSection"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="FailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="FailureMechanismSectionEntity.FailureMechanismSectionPointXml"/> 
        /// of <paramref name="entity"/> is <c>null</c> or empty.</exception>
        internal static FailureMechanismSection Read(this FailureMechanismSectionEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            Point2D[] points = new Point2DCollectionXmlSerializer().FromXml(entity.FailureMechanismSectionPointXml);
            var mechanismSection = new FailureMechanismSection(entity.Name, points);

            collector.Read(entity, mechanismSection);

            return mechanismSection;
        }
    }
}
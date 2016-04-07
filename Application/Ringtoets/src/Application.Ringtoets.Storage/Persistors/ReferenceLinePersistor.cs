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
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="ReferenceLine"/>.
    /// </summary>
    public class ReferenceLinePersistor
    {
        private readonly ReferenceLineConverter converter;
        private readonly DbSet<ReferenceLinePointEntity> referenceLineEntities;

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLinePersistor"/>.
        /// </summary>
        /// <param name="ringtoetsEntities">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsEntities"/> is <c>null</c>.</exception>
        public ReferenceLinePersistor(IRingtoetsEntities ringtoetsEntities)
        {
            if (ringtoetsEntities == null)
            {
                throw new ArgumentNullException("ringtoetsEntities");
            }

            referenceLineEntities = ringtoetsEntities.ReferenceLinePointEntities;
            converter = new ReferenceLineConverter();
        }

        /// <summary>
        /// Inserts the <paramref name="referenceLine"/> as points into the <paramref name="entityCollection"/>.
        /// </summary>
        /// <param name="entityCollection">The collection where the entities are added.</param>
        /// <param name="referenceLine">The reference line which will be added tot the <paramref name="entityCollection"/>
        ///  as entities.</param>
        /// <exception cref="ArgumentNullException"><paramref name="entityCollection"/> is <c>null</c>.</exception>
        public void InsertModel(ICollection<ReferenceLinePointEntity> entityCollection, ReferenceLine referenceLine)
        {
            if (entityCollection == null)
            {
                throw new ArgumentNullException("entityCollection");
            }

            if (HasChanges(entityCollection, referenceLine))
            {
                if (entityCollection.Any())
                {
                    referenceLineEntities.RemoveRange(entityCollection);
                    entityCollection.Clear();
                }

                if (referenceLine != null)
                {
                    converter.ConvertModelToEntity(referenceLine, entityCollection);
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="ReferenceLine"/> based on the information in <paramref name="entityCollection"/>.
        /// </summary>
        /// <param name="entityCollection">The database entity containing the information to set on the new model.</param>
        /// <returns>A new <see cref="ReferenceLine"/> with properties set from the database.</returns>
        public ReferenceLine LoadModel(ICollection<ReferenceLinePointEntity> entityCollection)
        {
            if (entityCollection == null)
            {
                throw new ArgumentNullException("entityCollection");
            }
            return converter.ConvertEntityToModel(entityCollection);
        }

        private bool HasChanges(ICollection<ReferenceLinePointEntity> entityCollection, ReferenceLine otherLine)
        {
            var existingLine = converter.ConvertEntityToModel(entityCollection);

            if (existingLine == null)
            {
                return otherLine != null;
            }
            if (otherLine == null)
            {
                return true;
            }

            var pointsArray = existingLine.Points.ToArray();
            var otherPointsArray = otherLine.Points.ToArray();
            if (pointsArray.Length != otherPointsArray.Length)
            {
                return true;
            }
            for (int i = 0; i < pointsArray.Length; i++)
            {
                var isXAlmostEqual = Math.Abs(pointsArray[i].X - otherPointsArray[i].X) < 1e-6;
                var isYAlmostEqual = Math.Abs(pointsArray[i].Y - otherPointsArray[i].Y) < 1e-6;
                if (!isXAlmostEqual || !isYAlmostEqual)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
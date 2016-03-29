using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    public class ReferenceLinePersistor
    {
        private readonly ReferenceLineConverter converter;
        private readonly DbSet<ReferenceLinePointEntity> referenceLineEntities;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationEntityPersistor"/>.
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
                var isXAlmostEqual = Math.Abs(pointsArray[i].X - otherPointsArray[i].X) < 1e-8;
                var isYAlmostEqual = Math.Abs(pointsArray[i].Y - otherPointsArray[i].Y) < 1e-8;
                if (!isXAlmostEqual || !isYAlmostEqual)
                {
                    return true;
                }
            }
            return false;
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
    }
}
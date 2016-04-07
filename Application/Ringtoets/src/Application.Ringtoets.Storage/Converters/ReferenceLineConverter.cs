using System;
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// This class is able to convert a <see cref="ReferenceLine"/> model to a <see cref="ReferenceLinePointEntity"/>
    /// entity and back.
    /// </summary>
    public class ReferenceLineConverter : IEntityConverter<ReferenceLine, ICollection<ReferenceLinePointEntity>>
    {
        /// <summary>
        /// Creates a new <see cref="ReferenceLine"/> from the <paramref name="entityCollection"/>.
        /// </summary>
        /// <param name="entityCollection">Collection of <see cref="ReferenceLinePointEntity"/> containing
        /// the geometry of the <see cref="ReferenceLine"/>.</param>
        /// <returns>A new <see cref="ReferenceLine"/> instance with its geometry taken from the 
        /// <paramref name="entityCollection"/>.</returns>
        public ReferenceLine ConvertEntityToModel(ICollection<ReferenceLinePointEntity> entityCollection)
        {
            if (entityCollection == null)
            {
                throw new ArgumentNullException("entityCollection");
            }
            if (!entityCollection.Any())
            {
                return null;
            }
            var line = new ReferenceLine();
            var geometry = new Point2D[entityCollection.Count];
            foreach (var entity in entityCollection)
            {
                var point = new Point2D(decimal.ToDouble(entity.X), decimal.ToDouble(entity.Y));
                geometry[entity.Order] = point;
            }
            line.SetGeometry(geometry);

            return line;
        }

        /// <summary>
        /// Updates the <paramref name="entityCollection"/> by adding the geometry from <see cref="ReferenceLine"/> to it.
        /// </summary>
        /// <param name="modelObject">The <see cref="ReferenceLine"/> used as source.</param>
        /// <param name="entityCollection">The target collection.</param>
        public void ConvertModelToEntity(ReferenceLine modelObject, ICollection<ReferenceLinePointEntity> entityCollection)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entityCollection == null)
            {
                throw new ArgumentNullException("entityCollection");
            }

            foreach (Point2D point in modelObject.Points)
            {
                entityCollection.Add(new ReferenceLinePointEntity
                {
                    X = Convert.ToDecimal(point.X),
                    Y = Convert.ToDecimal(point.Y),
                    Order = entityCollection.Count
                });
            }
        }
    }
}
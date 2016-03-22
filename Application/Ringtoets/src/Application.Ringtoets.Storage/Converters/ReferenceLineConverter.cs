using System;
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;

namespace Application.Ringtoets.Storage.Converters
{
    public class ReferenceLineConverter : IEntityConverter<ReferenceLine, ICollection<ReferenceLinePointEntity>>
    {
        public ReferenceLine ConvertEntityToModel(ICollection<ReferenceLinePointEntity> entityCollection)
        {
            var line = new ReferenceLine();

            var geometry = entityCollection.Select(entity => new Point2D(decimal.ToDouble(entity.X), decimal.ToDouble(entity.Y)));
            line.SetGeometry(geometry);

            return line;
        }

        public ReferenceLine ConvertEntityToModel(ICollection<ReferenceLinePointEntity> entity, Func<ReferenceLine> model)
        {
            throw new NotImplementedException();
        }

        public void ConvertModelToEntity(ReferenceLine modelObject, ICollection<ReferenceLinePointEntity> entity)
        {
            entity.Clear();
            foreach (Point2D point in modelObject.Points)
            {
                entity.Add(new ReferenceLinePointEntity
                {
                    X = Convert.ToDecimal(point.X),
                    Y = Convert.ToDecimal(point.Y),
                    Order = entity.Count
                });
            }
        }
    }
}
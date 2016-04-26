using System;
using Core.Common.Base.Geometry;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class ReferenceLinePointCreateExtensions
    {
        public static ReferenceLinePointEntity CreateReferenceLinePoint(this Point2D point, int order)
        {
            var entity = new ReferenceLinePointEntity
            {
                X = Convert.ToDecimal(point.X),
                Y = Convert.ToDecimal(point.Y),
                Order = order
            };

            return entity;
        }
    }
}
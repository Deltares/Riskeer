using System;
using Core.Common.Base.Geometry;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class ReferenceLinePointEntity
    {
        public Point2D Read()
        {
            return new Point2D(Convert.ToDouble(X), Convert.ToDouble(Y));
        }
    }
}
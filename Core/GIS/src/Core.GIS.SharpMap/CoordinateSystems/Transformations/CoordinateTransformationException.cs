using System;
using Core.Gis.GeoApi.CoordinateSystems;

namespace Core.GIS.SharpMap.CoordinateSystems.Transformations
{
    public class CoordinateTransformException : Exception
    {
        public CoordinateTransformException(string itemName, ICoordinateSystem sourceCs,
                                            ICoordinateSystem targetCs)
        {
            ItemName = itemName;
            SourceCS = sourceCs;
            TargetCS = targetCs;
        }

        public override string Message
        {
            get
            {
                return string.Format("could not convert '{0}' from coordinate system '{1}' to '{2}'", ItemName,
                                     SourceCS, TargetCS);
            }
        }

        private ICoordinateSystem SourceCS { get; set; }

        private ICoordinateSystem TargetCS { get; set; }

        private string ItemName { get; set; }
    }
}
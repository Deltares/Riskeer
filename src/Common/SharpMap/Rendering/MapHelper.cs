using System;
using System.Drawing;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using SharpMap.Api;
using GeometryFactory = SharpMap.Converters.Geometries.GeometryFactory;

namespace SharpMap.Rendering
{
    public static class MapHelper
    {
        public static double ImageToWorld(IMap map, float imageSize)
        {
            ICoordinate c1 = map.ImageToWorld(new PointF(0, 0));
            ICoordinate c2 = map.ImageToWorld(new PointF(imageSize, imageSize));
            return Math.Abs(c1.X - c2.X);
        }

        public static ICoordinate ImageToWorld(IMap map, double width, double height)
        {
            ICoordinate c1 = map.ImageToWorld(new PointF(0, 0));
            ICoordinate c2 = map.ImageToWorld(new PointF((float)width, (float)height));
            return GeometryFactory.CreateCoordinate(Math.Abs(c1.X - c2.X), Math.Abs(c1.Y - c2.Y));
        }

        public static IEnvelope GetEnvelope(ICoordinate worldPos, double width, double height)
        {
            // maak een rectangle in wereldcoordinaten ter grootte van 20 pixels rondom de click
            IPoint p = GeometryFactory.CreatePoint(worldPos.X, worldPos.Y);
            IEnvelope Envelope = (IEnvelope)p.EnvelopeInternal.Clone();
            Envelope.SetCentre(p, width, height);
            return Envelope;
        }

        public static IEnvelope GetEnvelope(ICoordinate worldPos, float radius)
        {
            // maak een rectangle in wereldcoordinaten ter grootte van 20 pixels rondom de click
            IPoint p = GeometryFactory.CreatePoint(worldPos);
            IEnvelope Envelope = (IEnvelope)p.EnvelopeInternal.Clone();
            Envelope.SetCentre(p, radius, radius);
            return Envelope;
        }
        public static IEnvelope GetEnvelopeForImage(IMap map, ICoordinate centre, double pixelWidth, double pixelHeight)
        {
            var envelope = new Envelope();
            ICoordinate size = ImageToWorld(map, pixelWidth, pixelHeight);
            envelope.SetCentre(centre, size.X, size.Y);
            return envelope;
        }
    }
}

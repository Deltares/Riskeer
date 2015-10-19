using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using SharpMap.Extensions.Properties;

namespace SharpMap.Extensions.CoordinateSystems
{
    public class OgrCoordinateSystemFactory : ICoordinateSystemFactory
    {
        /// <summary>
        /// supported coordinate system ids (geographic and projected)
        /// </summary>
        private static IList<ICoordinateSystem> supportedCoordinateSystems;

        public static IList<ICoordinateSystem> SupportedCoordinateSystems
        {
            get
            {
                if (supportedCoordinateSystems == null)
                {
                    supportedCoordinateSystems = Resources.cs_ids.Split(new[]
                    {
                        "\r\n"
                    }, StringSplitOptions.RemoveEmptyEntries)
                                                          .Where(s => !s.StartsWith("#"))
                                                          .Select(GetOgrCoordinateSystem).ToArray();
                }

                // Creates the cs_ids file data
                //var st = supportedCoordinateSystems.Select(c => c.AuthorityCode + ";\"" + c.Name + "\";" + c.IsGeographic + "\n").Aggregate("#EPSG;Name;IsGeographic", (current, s) => current + s);

                return supportedCoordinateSystems;
            }
        }

        public IList<ICoordinateSystem> CustomCoordinateSystems { get; set; }

        IList<ICoordinateSystem> ICoordinateSystemFactory.SupportedCoordinateSystems
        {
            get
            {
                return SupportedCoordinateSystems;
            }
        }

        public static string ExportToPrettyWkt(ICoordinateSystem coordinateSystem)
        {
            var ogrCoordinateSystem = coordinateSystem as OgrCoordinateSystem;

            if (ogrCoordinateSystem == null)
            {
                return null;
            }

            string wkt;
            ogrCoordinateSystem.ExportToPrettyWkt(out wkt, 1);
            return wkt;
        }

        public ICoordinateSystem CreateFromEPSG(int code)
        {
            return SupportedCoordinateSystems.First(crs => crs.Authority == "EPSG" && crs.AuthorityCode == code);
        }

        public ICoordinateTransformation CreateTransformation(ICoordinateSystem src, ICoordinateSystem dst)
        {
            var srcCoordinateSystem = (OgrCoordinateSystem) src;
            var dstCoordinateSystem = (OgrCoordinateSystem) dst;

            if (!srcCoordinateSystem.IsLoaded)
            {
                srcCoordinateSystem.Load();
            }

            if (!dstCoordinateSystem.IsLoaded)
            {
                dstCoordinateSystem.Load();
            }

            return new OgrCoordinateTransformation(srcCoordinateSystem, dstCoordinateSystem);
        }

        public ICompoundCoordinateSystem CreateCompoundCoordinateSystem(string name, ICoordinateSystem head, ICoordinateSystem tail)
        {
            throw new NotImplementedException();
        }

        public IEllipsoid CreateEllipsoid(string name, double semiMajorAxis, double semiMinorAxis, ILinearUnit linearUnit)
        {
            throw new NotImplementedException();
        }

        public IFittedCoordinateSystem CreateFittedCoordinateSystem(string name, ICoordinateSystem baseCoordinateSystem,
                                                                    string toBaseWkt, List<AxisInfo> arAxes)
        {
            throw new NotImplementedException();
        }

        public IEllipsoid CreateFlattenedSphere(string name, double semiMajorAxis, double inverseFlattening, ILinearUnit linearUnit)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSystem CreateFromXml(string xml)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSystem CreateFromWkt(string wkt)
        {
            // bug(s) in OSR? remove after upgrade to a new version
            if (wkt.Contains("Lambert_Conformal_Conic"))
            {
                wkt = wkt.Replace("Lambert_Conformal_Conic", "Lambert_Conformal_Conic_2sp");
            }

            if (wkt.Contains("Double_Stereographic"))
            {
                wkt = wkt.Replace("Double_Stereographic", "Oblique_Stereographic");
            }

            return new OgrCoordinateSystem(wkt);
        }

        public IGeographicCoordinateSystem CreateGeographicCoordinateSystem(string name, IAngularUnit angularUnit,
                                                                            IHorizontalDatum datum, IPrimeMeridian primeMeridian, AxisInfo axis0, AxisInfo axis1)
        {
            throw new NotImplementedException();
        }

        public IHorizontalDatum CreateHorizontalDatum(string name, DatumType datumType, IEllipsoid ellipsoid,
                                                      Wgs84ConversionInfo toWgs84)
        {
            throw new NotImplementedException();
        }

        public ILocalCoordinateSystem CreateLocalCoordinateSystem(string name, ILocalDatum datum, IUnit unit, List<AxisInfo> axes)
        {
            throw new NotImplementedException();
        }

        public ILocalDatum CreateLocalDatum(string name, DatumType datumType)
        {
            throw new NotImplementedException();
        }

        public IPrimeMeridian CreatePrimeMeridian(string name, IAngularUnit angularUnit, double longitude)
        {
            throw new NotImplementedException();
        }

        public IProjectedCoordinateSystem CreateProjectedCoordinateSystem(string name, IGeographicCoordinateSystem gcs,
                                                                          IProjection projection, ILinearUnit linearUnit, AxisInfo axis0, AxisInfo axis1)
        {
            throw new NotImplementedException();
        }

        public IProjection CreateProjection(string name, string wktProjectionClass, List<ProjectionParameter> Parameters)
        {
            throw new NotImplementedException();
        }

        public IVerticalCoordinateSystem CreateVerticalCoordinateSystem(string name, IVerticalDatum datum, ILinearUnit verticalUnit,
                                                                        AxisInfo axis)
        {
            throw new NotImplementedException();
        }

        public IVerticalDatum CreateVerticalDatum(string name, DatumType datumType)
        {
            throw new NotImplementedException();
        }

        private static OgrCoordinateSystem GetOgrCoordinateSystem(string idName)
        {
            var str = idName.Split(';');
            var id = int.Parse(str[0]);
            var name = str[1].Trim('\"');
            var isGeographic = bool.Parse(str[2]);

            var ogrCoordinateSystem = new OgrCoordinateSystem(id, name, isGeographic);

            return ogrCoordinateSystem;
        }
    }
}
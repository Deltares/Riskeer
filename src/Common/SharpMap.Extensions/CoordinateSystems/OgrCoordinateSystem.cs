using System;
using GeoAPI.CoordinateSystems;
using OSGeo.OSR;

namespace SharpMap.Extensions.CoordinateSystems
{
    /// <summary>
    /// See http://www.gdal.org/ogr/classOGRSpatialReference.html for the documentation of all SpatialReference methods.
    /// </summary>
    public class OgrCoordinateSystem : SpatialReference, ICoordinateSystem
    {
        static OgrCoordinateSystem()
        {
            GdalConfiguration.ConfigureGdal();
        }

        public OgrCoordinateSystem(IntPtr cPtr, bool cMemoryOwn, object parent) : base(cPtr, cMemoryOwn, parent)
        {
            IsLoaded = false;
        }

        public OgrCoordinateSystem() : this("") {}

        public OgrCoordinateSystem(long authorityCode, string name, bool isGeographic) : this("")
        {
            AuthorityCode = authorityCode;
            Authority = "EPSG";
            Name = name;
            IsGeographic = isGeographic;
        }

        public OgrCoordinateSystem(string wkt) : base(wkt)
        {
            IsLoaded = false;
            if (!string.IsNullOrEmpty(wkt))
            {
                AutoIdentifyEPSG();
                Authority = IsProjected() == 1 ? GetAuthorityName("PROJCS") : GetAuthorityName("GEOGCS");
                IsGeographic = base.IsGeographic() == 1;
                Name = GetAttrValue(IsGeographic ? "GEOGCS" : "PROJCS", 0);

                if (Authority != null)
                {
                    AuthorityCode = IsProjected() == 1
                                        ? int.Parse(GetAuthorityCode("PROJCS"))
                                        : int.Parse(GetAuthorityCode("GEOGCS"));
                }

                IsLoaded = true;
            }
        }

        public bool IsLoaded { get; private set; }

        public string Name { get; private set; }

        public string Authority { get; private set; }

        public long AuthorityCode { get; private set; }

        public string Alias { get; private set; }

        public string Abbreviation { get; private set; }

        public string Remarks { get; private set; }

        public string WKT
        {
            get
            {
                Load();

                string s;
                ExportToPrettyWkt(out s, 0);
                return s;
            }
        }

        public string XML
        {
            get
            {
                Load();

                string xml;
                ExportToXML(out xml, string.Empty);
                return xml;
            }
        }

        public string PROJ4
        {
            get
            {
                Load();

                string proj4;
                ExportToProj4(out proj4);
                return proj4;
            }
        }

        public int Dimension { get; private set; }

        public double[] DefaultEnvelope { get; private set; }

        public bool IsGeographic { get; private set; }

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

        public void Load()
        {
            if (IsLoaded)
            {
                return;
            }

            if (AuthorityCode != 0)
            {
                ImportFromEPSG((int) AuthorityCode);
            }

            Name = GetAttrValue(IsGeographic() == 1 ? "GEOGCS" : "PROJCS", 0);
            IsLoaded = true;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool EqualParams(object obj)
        {
            throw new NotImplementedException();
        }

        public AxisInfo GetAxis(int dimension)
        {
            throw new NotImplementedException();
        }

        public IUnit GetUnits(int dimension)
        {
            throw new NotImplementedException();
        }

        public double GetInverseFlattening()
        {
            Load();

            return GetInvFlattening();
        }

        public object Clone()
        {
            return new OgrCoordinateSystem(WKT);
        }
    }
}
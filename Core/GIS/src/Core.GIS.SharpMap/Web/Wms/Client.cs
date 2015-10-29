using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Converters.Geometries;

namespace Core.GIS.SharpMap.Web.Wms
{
    /// <summary>
    /// Class for requesting and parsing a WMS servers capabilities
    /// </summary>
    [Serializable]
    public class Client
    {
        private XmlNamespaceManager nsmgr;

        /// <summary>
        /// Initalizes WMS server and parses the Capabilities request
        /// </summary>
        /// <param name="url">URL of wms server</param>
        public Client(string url)
            : this(url, null) {}

        /// <summary>
        /// Initalizes WMS server and parses the Capabilities request
        /// </summary>
        /// <param name="url">URL of wms server</param>
        /// <param name="proxy">Proxy to use</param>
        public Client(string url, WebProxy proxy)
        {
            StringBuilder strReq = new StringBuilder(url);
            if (!url.Contains("?"))
            {
                strReq.Append("?");
            }
            if (!strReq.ToString().EndsWith("&") && !strReq.ToString().EndsWith("?"))
            {
                strReq.Append("&");
            }
            if (!url.ToLower().Contains("service=wms"))
            {
                strReq.AppendFormat("SERVICE=WMS&");
            }
            if (!url.ToLower().Contains("request=getcapabilities"))
            {
                strReq.AppendFormat("REQUEST=GetCapabilities&");
            }

            XmlDocument xml = GetRemoteXml(strReq.ToString(), proxy);
            ParseCapabilities(xml);
        }

        /// <summary>
        /// Downloads servicedescription from WMS service
        /// </summary>
        /// <returns>XmlDocument from Url. Null if Url is empty or inproper XmlDocument</returns>
        private XmlDocument GetRemoteXml(string Url, WebProxy proxy)
        {
            try
            {
                WebRequest myRequest = WebRequest.Create(Url);
                if (proxy != null)
                {
                    myRequest.Proxy = proxy;
                }
                WebResponse myResponse = myRequest.GetResponse();
                Stream stream = myResponse.GetResponseStream();
                XmlTextReader r = new XmlTextReader(Url, stream);
                XmlDocument doc = new XmlDocument();
                doc.Load(r);
                stream.Close();
                nsmgr = new XmlNamespaceManager(doc.NameTable);
                return doc;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could now download capabilities", ex);
            }
        }

        /// <summary>
        /// Parses a servicedescription and stores the data in the ServiceDescription property
        /// </summary>
        /// <param name="doc">XmlDocument containing a valid Service Description</param>
        private void ParseCapabilities(XmlDocument doc)
        {
            if (doc.DocumentElement.Attributes["version"] != null)
            {
                WmsVersion = doc.DocumentElement.Attributes["version"].Value;
                if (WmsVersion != "1.0.0" && WmsVersion != "1.1.0" && WmsVersion != "1.1.1" && WmsVersion != "1.3.0")
                {
                    throw new ApplicationException("WMS Version " + WmsVersion + " not supported");
                }

                nsmgr.AddNamespace(String.Empty, "http://www.opengis.net/wms");
                nsmgr.AddNamespace("sm", WmsVersion == "1.3.0" ? "http://www.opengis.net/wms" : "");
                nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
                nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            }
            else
            {
                throw (new ApplicationException("No service version number found!"));
            }

            XmlNode xnService = doc.DocumentElement.SelectSingleNode("sm:Service", nsmgr);
            XmlNode xnCapability = doc.DocumentElement.SelectSingleNode("sm:Capability", nsmgr);
            if (xnService != null)
            {
                ParseServiceDescription(xnService);
            }
            else
            {
                throw (new ApplicationException("No service tag found!"));
            }

            if (xnCapability != null)
            {
                ParseCapability(xnCapability);
            }
            else
            {
                throw (new ApplicationException("No capability tag found!"));
            }
        }

        /// <summary>
        /// Parses service description node
        /// </summary>
        /// <param name="xnlServiceDescription"></param>
        private void ParseServiceDescription(XmlNode xnlServiceDescription)
        {
            XmlNode node = xnlServiceDescription.SelectSingleNode("sm:Title", nsmgr);
            _ServiceDescription.Title = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:OnlineResource/@xlink:href", nsmgr);
            _ServiceDescription.OnlineResource = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:Abstract", nsmgr);
            _ServiceDescription.Abstract = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:Fees", nsmgr);
            _ServiceDescription.Fees = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:AccessConstraints", nsmgr);
            _ServiceDescription.AccessConstraints = (node != null ? node.InnerText : null);

            XmlNodeList xnlKeywords = xnlServiceDescription.SelectNodes("sm:KeywordList/sm:Keyword", nsmgr);
            if (xnlKeywords != null)
            {
                _ServiceDescription.Keywords = new string[xnlKeywords.Count];
                for (int i = 0; i < xnlKeywords.Count; i++)
                {
                    ServiceDescription.Keywords[i] = xnlKeywords[i].InnerText;
                }
            }
            //Contact information
            _ServiceDescription.ContactInformation = new Capabilities.WmsContactInformation();
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactAddress/sm:Address", nsmgr);
            _ServiceDescription.ContactInformation.Address.Address = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactAddress/sm:AddressType", nsmgr);
            _ServiceDescription.ContactInformation.Address.AddressType = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactAddress/sm:City", nsmgr);
            _ServiceDescription.ContactInformation.Address.City = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactAddress/sm:Country", nsmgr);
            _ServiceDescription.ContactInformation.Address.Country = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactAddress/sm:PostCode", nsmgr);
            _ServiceDescription.ContactInformation.Address.PostCode = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactElectronicMailAddress", nsmgr);
            _ServiceDescription.ContactInformation.Address.StateOrProvince = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactElectronicMailAddress", nsmgr);
            _ServiceDescription.ContactInformation.ElectronicMailAddress = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactFacsimileTelephone", nsmgr);
            _ServiceDescription.ContactInformation.FacsimileTelephone = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactPersonPrimary/sm:ContactOrganisation", nsmgr);
            _ServiceDescription.ContactInformation.PersonPrimary.Organisation = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactPersonPrimary/sm:ContactPerson", nsmgr);
            _ServiceDescription.ContactInformation.PersonPrimary.Person = (node != null ? node.InnerText : null);
            node = xnlServiceDescription.SelectSingleNode("sm:ContactInformation/sm:ContactVoiceTelephone", nsmgr);
            _ServiceDescription.ContactInformation.VoiceTelephone = (node != null ? node.InnerText : null);
        }

        /// <summary>
        /// Parses capability node
        /// </summary>
        /// <param name="xnCapability"></param>
        private void ParseCapability(XmlNode xnCapability)
        {
            XmlNode xnRequest = xnCapability.SelectSingleNode("sm:Request", nsmgr);
            if (xnRequest == null)
            {
                throw (new Exception("Request parameter not specified in Service Description"));
            }
            ParseRequest(xnRequest);
            XmlNode xnLayer = xnCapability.SelectSingleNode("sm:Layer", nsmgr);
            if (xnLayer == null)
            {
                throw (new Exception("No layer tag found in Service Description"));
            }
            Layer = ParseLayer(xnLayer);

            XmlNode xnException = xnCapability.SelectSingleNode("sm:Exception", nsmgr);
            if (xnException != null)
            {
                ParseExceptions(xnException);
            }
        }

        /// <summary>
        /// Parses valid exceptions
        /// </summary>
        /// <param name="xnlExceptionNode"></param>
        private void ParseExceptions(XmlNode xnlExceptionNode)
        {
            XmlNodeList xnlFormats = xnlExceptionNode.SelectNodes("sm:Format", nsmgr);
            if (xnlFormats != null)
            {
                ExceptionFormats = new string[xnlFormats.Count];
                for (int i = 0; i < xnlFormats.Count; i++)
                {
                    ExceptionFormats[i] = xnlFormats[i].InnerText;
                }
            }
        }

        /// <summary>
        /// Parses request node
        /// </summary>
        /// <param name="xmlRequestNode"></param>
        private void ParseRequest(XmlNode xmlRequestNode)
        {
            XmlNode xnGetMap = xmlRequestNode.SelectSingleNode("sm:GetMap", nsmgr);
            ParseGetMapRequest(xnGetMap);
            //TODO:
            //XmlNode xnGetFeatureInfo = xmlRequestNodes.SelectSingleNode("sm:GetFeatureInfo", nsmgr);
            //XmlNode xnCapa = xmlRequestNodes.SelectSingleNode("sm:GetCapabilities", nsmgr); <-- We don't really need this do we?			
        }

        /// <summary>
        /// Parses GetMap request nodes
        /// </summary>
        /// <param name="GetMapRequestNodes"></param>
        private void ParseGetMapRequest(XmlNode GetMapRequestNodes)
        {
            XmlNode xnlHttp = GetMapRequestNodes.SelectSingleNode("sm:DCPType/sm:HTTP", nsmgr);
            if (xnlHttp != null && xnlHttp.HasChildNodes)
            {
                GetMapRequests = new WmsOnlineResource[xnlHttp.ChildNodes.Count];
                for (int i = 0; i < xnlHttp.ChildNodes.Count; i++)
                {
                    WmsOnlineResource wor = new WmsOnlineResource();
                    wor.Type = xnlHttp.ChildNodes[i].Name;
                    wor.OnlineResource = xnlHttp.ChildNodes[i].SelectSingleNode("sm:OnlineResource", nsmgr).Attributes["xlink:href"].InnerText;
                    GetMapRequests[i] = wor;
                }
            }
            XmlNodeList xnlFormats = GetMapRequestNodes.SelectNodes("sm:Format", nsmgr);
            //_GetMapOutputFormats = new Collection<string>(xnlFormats.Count);
            GetMapOutputFormats = new Collection<string>();
            for (int i = 0; i < xnlFormats.Count; i++)
            {
                GetMapOutputFormats.Add(xnlFormats[i].InnerText);
            }
        }

        /// <summary>
        /// Iterates through the layer nodes recursively
        /// </summary>
        /// <param name="xmlLayer"></param>
        /// <returns></returns>
        private WmsServerLayer ParseLayer(XmlNode xmlLayer)
        {
            WmsServerLayer layer = new WmsServerLayer();
            XmlNode node = xmlLayer.SelectSingleNode("sm:Name", nsmgr);
            layer.Name = (node != null ? node.InnerText : null);
            node = xmlLayer.SelectSingleNode("sm:Title", nsmgr);
            layer.Title = (node != null ? node.InnerText : null);
            if (string.IsNullOrEmpty(layer.Name))
            {
                layer.Name = layer.Title;
            }
            node = xmlLayer.SelectSingleNode("sm:Abstract", nsmgr);
            layer.Abstract = (node != null ? node.InnerText : null);
            XmlAttribute attr = xmlLayer.Attributes["queryable"];
            layer.Queryable = (attr != null && attr.InnerText == "1");

            XmlNodeList xnlKeywords = xmlLayer.SelectNodes("sm:KeywordList/sm:Keyword", nsmgr);
            if (xnlKeywords != null)
            {
                layer.Keywords = new string[xnlKeywords.Count];
                for (int i = 0; i < xnlKeywords.Count; i++)
                {
                    layer.Keywords[i] = xnlKeywords[i].InnerText;
                }
            }
            XmlNodeList xnlCrs = xmlLayer.SelectNodes("sm:CRS", nsmgr);
            if (xnlCrs != null)
            {
                layer.CRS = new string[xnlCrs.Count];
                for (int i = 0; i < xnlCrs.Count; i++)
                {
                    layer.CRS[i] = xnlCrs[i].InnerText;
                }
            }
            XmlNodeList xnlStyle = xmlLayer.SelectNodes("sm:Style", nsmgr);
            if (xnlStyle != null)
            {
                layer.Style = new WmsLayerStyle[xnlStyle.Count];
                for (int i = 0; i < xnlStyle.Count; i++)
                {
                    node = xnlStyle[i].SelectSingleNode("sm:Name", nsmgr);
                    layer.Style[i].Name = (node != null ? node.InnerText : null);
                    node = xnlStyle[i].SelectSingleNode("sm:Title", nsmgr);
                    layer.Style[i].Title = (node != null ? node.InnerText : null);
                    node = xnlStyle[i].SelectSingleNode("sm:Abstract", nsmgr);
                    layer.Style[i].Abstract = (node != null ? node.InnerText : null);
                    node = xnlStyle[i].SelectSingleNode("sm:LegendUrl", nsmgr);
                    if (node != null)
                    {
                        layer.Style[i].LegendUrl = new WmsStyleLegend();
                        layer.Style[i].LegendUrl.Size = new Size(
                            int.Parse(node.Attributes["width"].InnerText), int.Parse(node.Attributes["height"].InnerText));
                        layer.Style[i].LegendUrl.OnlineResource.OnlineResource = node.SelectSingleNode("sm:OnlineResource", nsmgr).Attributes["xlink:href"].InnerText;
                        layer.Style[i].LegendUrl.OnlineResource.Type = node.SelectSingleNode("sm:Format", nsmgr).InnerText;
                    }
                    node = xnlStyle[i].SelectSingleNode("sm:StyleSheetURL", nsmgr);
                    if (node != null)
                    {
                        layer.Style[i].StyleSheetUrl = new WmsOnlineResource();
                        layer.Style[i].StyleSheetUrl.OnlineResource = node.SelectSingleNode("sm:OnlineResource", nsmgr).Attributes["xlink:href"].InnerText;
                        //layer.Style[i].StyleSheetUrl.OnlineResource = node.SelectSingleNode("sm:Format", nsmgr).InnerText;
                    }
                }
            }
            XmlNodeList xnlLayers = xmlLayer.SelectNodes("sm:Layer", nsmgr);
            if (xnlLayers != null)
            {
                layer.ChildLayers = new WmsServerLayer[xnlLayers.Count];
                for (int i = 0; i < xnlLayers.Count; i++)
                {
                    layer.ChildLayers[i] = ParseLayer(xnlLayers[i]);
                }
            }
            node = xmlLayer.SelectSingleNode("sm:LatLonBoundingBox", nsmgr);
            if (node == null)
            {
                node = xmlLayer.SelectSingleNode("sm:BoundingBox", nsmgr);
            }
            if (node != null)
            {
                double minx = 0;
                double miny = 0;
                double maxx = 0;
                double maxy = 0;
                if (!double.TryParse(node.Attributes["minx"].Value, NumberStyles.Any, Map.Map.numberFormat_EnUS, out minx) &
                    !double.TryParse(node.Attributes["miny"].Value, NumberStyles.Any, Map.Map.numberFormat_EnUS, out miny) &
                    !double.TryParse(node.Attributes["maxx"].Value, NumberStyles.Any, Map.Map.numberFormat_EnUS, out maxx) &
                    !double.TryParse(node.Attributes["maxy"].Value, NumberStyles.Any, Map.Map.numberFormat_EnUS, out maxy))
                {
                    throw new ArgumentException("Invalid LatLonBoundingBox on layer '" + layer.Name + "'");
                }
                layer.LatLonBoundingBox = GeometryFactory.CreateEnvelope(minx, maxx, miny, maxy);
            }
            return layer;
        }

        #region WMS Data structures

        /// <summary>
        /// Structure for holding information about a WMS Layer 
        /// </summary>
        public struct WmsServerLayer
        {
            /// <summary>
            /// Layer title
            /// </summary>
            public string Title;

            /// <summary>
            /// Unique name of this layer used for requesting layer
            /// </summary>
            public string Name;

            /// <summary>
            /// Abstract
            /// </summary>
            public string Abstract;

            /// <summary>
            /// Specifies whether this layer is queryable using GetFeatureInfo requests
            /// </summary>
            public bool Queryable;

            /// <summary>
            /// Keywords
            /// </summary>
            public string[] Keywords;

            /// <summary>
            /// List of styles supported by layer
            /// </summary>
            public WmsLayerStyle[] Style;

            /// <summary>
            /// Coordinate Reference Systems supported by layer
            /// </summary>
            public string[] CRS;

            /// <summary>
            /// Collection of child layers
            /// </summary>
            public WmsServerLayer[] ChildLayers;

            /// <summary>
            /// Latitudal/longitudal extent of this layer
            /// </summary>
            public IEnvelope LatLonBoundingBox;
        }

        /// <summary>
        /// Structure for storing information about a WMS Layer Style
        /// </summary>
        public struct WmsLayerStyle
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name;

            /// <summary>
            /// Title
            /// </summary>
            public string Title;

            /// <summary>
            /// Abstract
            /// </summary>
            public string Abstract;

            /// <summary>
            /// Legend
            /// </summary>
            public WmsStyleLegend LegendUrl;

            /// <summary>
            /// Style Sheet Url
            /// </summary>
            public WmsOnlineResource StyleSheetUrl;
        }

        /// <summary>
        /// Structure for storing WMS Legend information
        /// </summary>
        public struct WmsStyleLegend
        {
            /// <summary>
            /// Online resource for legend style 
            /// </summary>
            public WmsOnlineResource OnlineResource;

            /// <summary>
            /// Size of legend
            /// </summary>
            public Size Size;
        }

        /// <summary>
        /// Structure for storing info on an Online Resource
        /// </summary>
        public struct WmsOnlineResource
        {
            /// <summary>
            /// Type of online resource (Ex. request method 'Get' or 'Post')
            /// </summary>
            public string Type;

            /// <summary>
            /// URI of online resource
            /// </summary>
            public string OnlineResource;
        }

        #endregion

        #region Properties

        private Capabilities.WmsServiceDescription _ServiceDescription;

        /// <summary>
        /// Gets the service description
        /// </summary>
        public Capabilities.WmsServiceDescription ServiceDescription
        {
            get
            {
                return _ServiceDescription;
            }
        }

        /// <summary>
        /// Gets the version of the WMS server (ex. "1.3.0")
        /// </summary>
        public string WmsVersion { get; private set; }

        /// <summary>
        /// Gets a list of available image mime type formats
        /// </summary>
        public Collection<string> GetMapOutputFormats { get; private set; }

        /// <summary>
        /// Gets a list of available exception mime type formats
        /// </summary>
        public string[] ExceptionFormats { get; private set; }

        /// <summary>
        /// Gets the available GetMap request methods and Online Resource URI
        /// </summary>
        public WmsOnlineResource[] GetMapRequests { get; private set; }

        /// <summary>
        /// Gets the hiarchial layer structure
        /// </summary>
        public WmsServerLayer Layer { get; private set; }

        #endregion
    }
}
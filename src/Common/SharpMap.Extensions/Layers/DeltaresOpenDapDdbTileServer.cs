using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using BruTile;

namespace SharpMap.Extensions.Layers
{
    public static class DeltaresOpenDapDdbTileServer
    {
        private static readonly string catalogUrl = "http://opendap.deltares.nl/thredds/opendap/deltares/delftdashboard/bathymetry/";
        private static readonly string dodscUrl = "http://opendap.deltares.nl/thredds/dodsC/opendap/deltares/delftdashboard/bathymetry/";
        private static readonly string tileUrl = "http://opendap.deltares.nl/thredds/fileServer/opendap/deltares/delftdashboard/bathymetry/";

        public static IEnumerable<string> GetAvailableDataSets()
        {
            // find the available datasets:
            var datasetsPage = RetrieveWebPage(catalogUrl + "catalog.html");
            return Regex.Matches(datasetsPage, "<a href=.*?<tt>(?<file>.*?)/</tt>")
                        .OfType<Match>().Select(m => m.Groups["file"].Value).ToList();
        }

        public static DdbTiledDataSetInfo GetInformationFromCatalog(string datasetName)
        {
            var info = new DdbTiledDataSetInfo();

            // find schema file:
            var catalogPage = RetrieveWebPage(catalogUrl + string.Format("{0}/catalog.html", datasetName));
            var files = Regex.Matches(catalogPage, "<a href=.*?<tt>(?<file>.*?)</tt>")
                             .OfType<Match>().Select(m => m.Groups["file"].Value).ToList();
            var schemaFile = files.First(f => f.EndsWith(".nc"));
            var fileprefix = Path.GetFileNameWithoutExtension(schemaFile);

            // resolutions, tile & coordinate system info:
            var dataSetInfoPage = RetrieveWebPageLines(dodscUrl + string.Format(
                "{0}/{1}.ascii?crs,grid_size_x,x0[0:0],y0[0:0],ntilesx[0:0],ntilesy[0:0],nx[0:0],ny[0:0]",
                datasetName, schemaFile))
                .SkipWhile(l => !l.Contains("-------")).ToList();

            // resolutions:
            var resolutionsLineIndex = dataSetInfoPage.FindIndex(l => l.Contains("grid_size_x[")) + 1;
            var resolutionStrings = dataSetInfoPage[resolutionsLineIndex].Split(',');
            info.Resolutions.AddRange(resolutionStrings.Select(rs => double.Parse(rs, CultureInfo.InvariantCulture)).Reverse());

            // epsg code:
            var crsLine = dataSetInfoPage.First(l => l.StartsWith("crs"));
            info.Epsg = Int32.Parse(crsLine.Split(',')[1]);

            // tiles origin (assumed cs origin)
            var originXLineIndex = dataSetInfoPage.FindIndex(l => l.Contains("x0[1]")) + 1;
            var originYLineIndex = dataSetInfoPage.FindIndex(l => l.Contains("y0[1]")) + 1;
            var originX = Double.Parse(dataSetInfoPage[originXLineIndex], CultureInfo.InvariantCulture);
            var originY = Double.Parse(dataSetInfoPage[originYLineIndex], CultureInfo.InvariantCulture);

            // num tiles:
            var numTilesXLineIndex = dataSetInfoPage.FindIndex(l => l.Contains("ntilesx[")) + 1;
            var numTilesYLineIndex = dataSetInfoPage.FindIndex(l => l.Contains("ntilesy[")) + 1;
            var numTilesX = Int32.Parse(dataSetInfoPage[numTilesXLineIndex]);
            var numTilesY = Int32.Parse(dataSetInfoPage[numTilesYLineIndex]);

            // tile dimensions:
            var tileWidthLineIndex = dataSetInfoPage.FindIndex(l => l.Contains("nx[")) + 1;
            var tileHeightLineIndex = dataSetInfoPage.FindIndex(l => l.Contains("ny[")) + 1;
            info.TileWidth = Int32.Parse(dataSetInfoPage[tileWidthLineIndex]);
            info.TileHeight = Int32.Parse(dataSetInfoPage[tileHeightLineIndex]);

            // cs width / height:
            var width = numTilesX*info.Resolutions.Last()*info.TileWidth;
            var height = numTilesY*info.Resolutions.Last()*info.TileHeight;
            info.Extent = new Extent(originX, originY, originX + width, originY + height);

            //http://opendap.deltares.nl/thredds/fileServer/opendap/deltares/delftdashboard/bathymetry/gebco_08/zl06/gebco08.zl06.00005.00003.nc
            info.UrlFormat = tileUrl + datasetName + "/zl{0:00}/" + fileprefix + ".zl{0:00}.{1:00000}.{2:00000}.nc";

            return info;
        }

        public static IEnumerable<string> GetHighestResolutionTileUrlsInExtent(DdbTiledDataSetInfo datasetInfo, double minX, double minY, double maxX, double maxY)
        {
            var highResLevel = datasetInfo.NumZoomLevels - 1;
            var datasetExtend = datasetInfo.Extent;
            var resolution = datasetInfo.Resolutions[highResLevel];

            var unitsPerTileX = resolution*datasetInfo.TileWidth;
            var unitsPerTileY = resolution*datasetInfo.TileHeight;

            // make sure we don't ask outside our dataset extend:
            var extent = new Extent(Math.Max(minX, datasetExtend.MinX),
                                    Math.Max(minY, datasetExtend.MinY),
                                    Math.Min(maxX, datasetExtend.MaxX),
                                    Math.Min(maxY, datasetExtend.MaxY));

            var minCol = (int) Math.Floor((extent.MinX - datasetExtend.MinX)/unitsPerTileX);
            var maxCol = (int) Math.Ceiling((extent.MaxX - datasetExtend.MinX)/unitsPerTileX);
            var minRow = (int) Math.Floor((extent.MinY - datasetExtend.MinY)/unitsPerTileY);
            var maxRow = (int) Math.Ceiling((extent.MaxY - datasetExtend.MinY)/unitsPerTileY);

            for (int c = minCol; c < maxCol; c++)
            {
                for (int r = minRow; r < maxRow; r++)
                {
                    yield return string.Format(datasetInfo.UrlFormat, datasetInfo.NumZoomLevels - highResLevel, c + 1, r + 1);
                }
            }
        }

        private static string RetrieveWebPage(string url)
        {
            return string.Join("\n\r", RetrieveWebPageLines(url));
        }

        private static List<string> RetrieveWebPageLines(string url)
        {
            var lines = new List<string>();
            var response = (WebRequest.Create(url)).GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line.Trim()))
                        {
                            lines.Add(line);
                        }
                    }
                }
            }
            return lines;
        }
    }

    public class DdbTiledDataSetInfo
    {
        public DdbTiledDataSetInfo()
        {
            Resolutions = new List<double>();
        }

        public int Epsg { get; set; }
        public List<double> Resolutions { get; private set; }
        public Extent Extent { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public string UrlFormat { get; set; }

        public int NumZoomLevels
        {
            get
            {
                return Resolutions.Count;
            }
        }
    }
}
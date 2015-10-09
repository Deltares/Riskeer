// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EGIS.ShapeFileLib;
using GeoAPI.CoordinateSystems;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using log4net;
using NetTopologySuite.Extensions.Features;
using SharpMap.Api;
using SharpMap.Data.Providers.EGIS.ShapeFileLib;
using GeometryFactory = SharpMap.Converters.Geometries.GeometryFactory;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Shapefile dataprovider
    /// </summary>
    /// <remarks>
    /// <para>The ShapeFile provider is used for accessing ESRI ShapeFiles. The ShapeFile should at least contain the
    /// [filename].shp, [filename].idx, and if feature-data is to be used, also [filename].dbf file.</para>
    /// <para>The first time the ShapeFile is accessed, SharpMap will automatically create a spatial index
    /// of the shp-file, and save it as [filename].shp.sidx. If you change or update the contents of the .shp file,
    /// delete the .sidx file to force SharpMap to rebuilt it. In web applications, the index will automatically
    /// be cached to memory for faster access, so to reload the index, you will need to restart the web application
    /// as well.</para>
    /// <para>
    /// M and Z values in a shapefile is ignored by SharpMap.
    /// </para>
    /// </remarks>
    /// <example>
    /// Adding a datasource to a layer:
    /// <code lang="C#">
    /// SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("My layer");
    /// myLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(@"C:\data\MyShapeData.shp");
    /// </code>
    /// </example>
    public class ShapeFile : IFeatureProvider
    {
        #region Delegates

        /// <summary>
        /// Filter Delegate Method
        /// </summary>
        /// <remarks>
        /// The FilterMethod delegate is used for applying a method that filters data from the dataset.
        /// The method should return 'true' if the feature should be included and false if not.
        /// <para>See the <see cref="FilterDelegate"/> property for more info</para>
        /// </remarks>
        /// <seealso cref="FilterDelegate"/>
        /// <param name="dr"><see cref="SharpMap.Data.FeatureDataRow"/> to test on</param>
        /// <returns>true if this feature should be included, false if it should be filtered</returns>
        public delegate bool FilterMethod(IFeature dr);

        #endregion

        private static readonly ILog log = LogManager.GetLogger(typeof(ShapeFile));
        private bool _CoordsysReadFromFile = false;
        private IEnvelope _Envelope;
        private int _FeatureCount;
        private readonly bool _FileBasedIndex;
        private string path;
        private bool _IsOpen;
        private ShapeType _ShapeType;
        private string srsWkt;
        private BinaryReader brShapeFile;
        private BinaryReader brShapeIndex;
        private DbaseReader dbaseFile;
        private FileStream fsShapeFile;
        private FileStream fsShapeIndex;
        private bool logNoShapeFile = true;

        private IntPtr shpFileMemoryMap;
        private IntPtr shpFileMemoryMapView;

        private RecordHeader[] recordHeaders;
        private ShapeFileMainHeader mainHeader;

        private IntPtr idxFileMemoryMap;
        private IntPtr idxFileMemoryMapView;

        private unsafe byte* zeroPtr;

        public ShapeFile()
        {
            _FileBasedIndex = false;
        }

        /// <summary>
        /// Initializes a ShapeFile DataProvider without a file-based spatial index.
        /// </summary>
        /// <param name="filename">Path to shape file</param>
        public ShapeFile(string filename)
            : this(filename, false) {}

        /// <summary>
        /// Initializes a ShapeFile DataProvider.
        /// </summary>
        /// <remarks>
        /// <para>If FileBasedIndex is true, the spatial index will be read from a local copy. If it doesn't exist,
        /// it will be generated and saved to [filename] + '.sidx'.</para>
        /// <para>Using a file-based index is especially recommended for ASP.NET applications which will speed up
        /// start-up time when the cache has been emptied.
        /// </para>
        /// </remarks>
        /// <param name="filename">Path to shape file</param>
        /// <param name="fileBasedIndex">Use file-based spatial index</param>
        public ShapeFile(string filename, bool fileBasedIndex)
        {
            Open(filename);
            _FileBasedIndex = fileBasedIndex;
        }

        /// <summary>
        /// Gets the <see cref="SharpMap.Data.Providers.ShapeType">shape geometry type</see> in this shapefile.
        /// </summary>
        /// <remarks>
        /// The property isn't set until the first time the datasource has been opened,
        /// and will throw an exception if this property has been called since initialization. 
        /// <para>All the non-Null shapes in a shapefile are required to be of the same shape
        /// type.</para>
        /// </remarks>
        public virtual ShapeType ShapeType
        {
            get
            {
                return _ShapeType;
            }
        }

        /// <summary>
        /// Gets or sets the filename of the shapefile
        /// </summary>
        /// <remarks>If the filename changes, indexes will be rebuilt</remarks>
        public virtual string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                Open(path);
            }
        }

        /// <summary>
        /// Gets or sets the encoding used for parsing strings from the DBase DBF file.
        /// </summary>
        /// <remarks>
        /// The DBase default encoding is <see cref="System.Text.Encoding.UTF7"/>.
        /// </remarks>
        public virtual Encoding Encoding
        {
            get
            {
                return dbaseFile.Encoding;
            }
            set
            {
                dbaseFile.Encoding = value;
            }
        }

        /// <summary>
        /// Filter Delegate Method for limiting the datasource
        /// </summary>
        /// <remarks>
        /// <example>
        /// Using an anonymous method for filtering all features where the NAME column starts with S:
        /// <code lang="C#">
        /// myShapeDataSource.FilterDelegate = new SharpMap.Data.Providers.ShapeFile.FilterMethod(delegate(SharpMap.Data.FeatureDataRow row) { return (!row["NAME"].ToString().StartsWith("S")); });
        /// </code>
        /// </example>
        /// <example>
        /// Declaring a delegate method for filtering (multi)polygon-features whose area is larger than 5.
        /// <code>
        /// myShapeDataSource.FilterDelegate = CountryFilter;
        /// [...]
        /// public static bool CountryFilter(SharpMap.Data.FeatureDataRow row)
        /// {
        ///		if(row.Geometry.GetType()==typeof(SharpMap.Geometries.Polygon))
        ///			return ((row.Geometry as SharpMap.Geometries.Polygon).Area>5);
        ///		if (row.Geometry.GetType() == typeof(SharpMap.Geometries.MultiPolygon))
        ///			return ((row.Geometry as SharpMap.Geometries.MultiPolygon).Area > 5);
        ///		else return true;
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <seealso cref="FilterMethod"/>
        public virtual FilterMethod FilterDelegate { get; set; }

        public virtual IEnumerable<string> Paths
        {
            get
            {
                if (Path != null)
                {
                    yield return Path;
                }
                var dbaseFilePath = GetDbaseFilePath();
                if (dbaseFilePath != null)
                {
                    yield return dbaseFilePath;
                }
                var indexFilePath = GetIndexFilePath();
                if (indexFilePath != null)
                {
                    yield return indexFilePath;
                }
            }
        }

        public virtual string FileFilter
        {
            get
            {
                return "Shape file (*.shp)|*.shp";
            }
        }

        public virtual bool IsRelationalDataBase
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the coordinate system of the ShapeFile. If a shapefile has 
        /// a corresponding [filename].prj file containing a Well-Known Text 
        /// description of the coordinate system this will automatically be read.
        /// If this is not the case, the coordinate system will default to null.
        /// </summary>
        public virtual ICoordinateSystem CoordinateSystem { get; set; }

        public virtual void ReConnect() {}

        public virtual void Delete()
        {
            File.Delete(Path);
            var dbaseFilePath = GetDbaseFilePath();
            if (dbaseFilePath != null)
            {
                File.Delete(dbaseFilePath);
            }
            var indexFilePath = GetIndexFilePath();
            if (indexFilePath != null)
            {
                File.Delete(indexFilePath);
            }
        }

        /// <summary>
        /// Gets a datarow from the datasource at the specified index belonging to the specified datatable
        /// </summary>
        /// <param name="RowID"></param>
        /// <returns></returns>
        public virtual IFeature GetFeature(int RowID)
        {
            if (dbaseFile != null)
            {
                Open(Path);

/*
                if (structureTable == null)
                {
                    structureTable = dbaseFile.NewTable;
                }
*/

                var dr = dbaseFile.GetFeature(RowID);
                dr.Geometry = ReadGeometry(RowID);

                if (FilterDelegate == null || FilterDelegate(dr))
                {
                    return dr;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw (new FileNotFoundException(
                    "An attempt was made to read DBase data from a shapefile without a valid .DBF file"));
            }
        }

        public virtual unsafe IEnvelope GetBounds(int recordIndex)
        {
            var dataPtr = zeroPtr + recordHeaders[recordIndex].Offset + 8 + 4;

            if (IsPoint)
            {
                var pt = *(PointD*) dataPtr;
                return new Envelope(pt.X, pt.X, pt.Y, pt.Y);
            }
            else
            {
                var minX = *((double*) dataPtr);
                dataPtr += 8;
                var minY = *((double*) dataPtr);
                dataPtr += 8;
                var maxX = *((double*) dataPtr);
                dataPtr += 8;
                var maxY = *((double*) dataPtr);

                return new Envelope(minX, maxX, minY, maxY);
            }
        }

        private bool IsPoint
        {
            get
            {
                return _ShapeType == ShapeType.Point || ShapeType == ShapeType.PointM || ShapeType == ShapeType.PointZ;
            }
        }

        private void InitializeShape(string filename, bool FileBasedIndex)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(String.Format("Could not find file \"{0}\"", filename));
            }

            if (!filename.ToLower().EndsWith(".shp"))
            {
                throw (new Exception("Invalid shapefile filename: " + filename));
            }

            recordHeaders = LoadIndexfile(GetIndexFilePath());

            //Read the spatial bounding box of the contents
            brShapeIndex.BaseStream.Seek(36, 0); //seek to box

            double x1, x2, y1, y2;
            x1 = brShapeIndex.ReadDouble();
            y1 = brShapeIndex.ReadDouble();
            x2 = brShapeIndex.ReadDouble();
            y2 = brShapeIndex.ReadDouble();

            _Envelope = GeometryFactory.CreateEnvelope(x1, x2, y1, y2);
        }

        /// <summary>
        /// Reads and parses the header of the .shx index file
        /// </summary>
        private void ParseHeader()
        {
            fsShapeIndex = new FileStream(System.IO.Path.ChangeExtension(path, ".shx"), FileMode.Open, FileAccess.Read);
            brShapeIndex = new BinaryReader(fsShapeIndex, Encoding.Unicode);

            brShapeIndex.BaseStream.Seek(0, 0);
            //Check file header
            if (brShapeIndex.ReadInt32() != 170328064)
            {
                //File Code is actually 9994, but in Little Endian Byte Order this is '170328064'
                throw (new ApplicationException("Invalid Shapefile Index (.shx)"));
            }

            brShapeIndex.BaseStream.Seek(24, 0); //seek to File Length
            int IndexFileSize = SwapByteOrder(brShapeIndex.ReadInt32());
            //Read filelength as big-endian. The length is based on 16bit words
            _FeatureCount = (2*IndexFileSize - 100)/8;
            //Calculate FeatureCount. Each feature takes up 8 bytes. The header is 100 bytes

            brShapeIndex.BaseStream.Seek(32, 0); //seek to ShapeType
            _ShapeType = (ShapeType) brShapeIndex.ReadInt32();

            //Read the spatial bounding box of the contents
            brShapeIndex.BaseStream.Seek(36, 0); //seek to box

            double x1, x2, y1, y2;
            x1 = brShapeIndex.ReadDouble();
            y1 = brShapeIndex.ReadDouble();
            x2 = brShapeIndex.ReadDouble();
            y2 = brShapeIndex.ReadDouble();

            _Envelope = GeometryFactory.CreateEnvelope(x1, x2, y1, y2);

            brShapeIndex.Close();
            fsShapeIndex.Close();
        }

        /// <summary>
        /// Reads and parses the projection if a projection file exists
        /// </summary>
        private void ParseProjection()
        {
            string projfile = System.IO.Path.GetDirectoryName(Path) + "\\" + System.IO.Path.GetFileNameWithoutExtension(Path) + ".prj";
            if (File.Exists(projfile))
            {
                try
                {
                    SrsWkt = File.ReadAllText(projfile);
                    _CoordsysReadFromFile = true;
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("Coordinate system file '" + projfile +
                                       "' found, but could not be parsed. WKT parser returned:" + ex.Message);
                    throw (ex);
                }
            }
        }

        private RecordHeader[] LoadIndexfile(string path)
        {
            //read record headers from the index file
            RecordHeader[] recordHeaders = null;
            BinaryReader bReader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
            try
            {
                mainHeader = new ShapeFileMainHeader(bReader.ReadBytes(100));
                int totalRecords = (mainHeader.FileLength - 100) >> 3;
                recordHeaders = new RecordHeader[totalRecords];
                int numRecs = 0;
                //now read the record headers
                byte[] data = new byte[mainHeader.FileLength - 100];
                bReader.Read(data, 0, data.Length);
                while (numRecs < totalRecords)
                {
                    RecordHeader recHead = new RecordHeader(numRecs + 1);
                    recHead.readFromIndexFile(data, numRecs << 3);
                    recordHeaders[numRecs++] = recHead;
                }
                data = null;
            }
            finally
            {
                bReader.Close();
                bReader = null;
            }

            return recordHeaders;
        }

        ///<summary>
        ///Swaps the byte order of an int32
        ///</summary>
        /// <param name="i">Integer to swap</param>
        /// <returns>Byte Order swapped int32</returns>
        private int SwapByteOrder(int i)
        {
            byte[] buffer = BitConverter.GetBytes(i);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Reads and parses the geometry with ID 'oid' from the ShapeFile
        /// </summary>
        /// <remarks><see cref="FilterDelegate">Filtering</see> is not applied to this method</remarks>
        /// <param name="oid">Object ID</param>
        /// <returns>geometry</returns>
        private unsafe IGeometry ReadGeometry(int oid)
        {
            if (_ShapeType == ShapeType.Polygon || _ShapeType == ShapeType.PolygonM || _ShapeType == ShapeType.PolygonZ)
            {
                return ReadPolygon(oid);
            }

            var dataPtr = zeroPtr + recordHeaders[oid].Offset + 8;
            var type = *((ShapeType*) dataPtr);
            if (type == ShapeType.Null)
            {
                return null;
            }

            dataPtr += 4;

            if (IsPoint)
            {
                var x = *((double*) dataPtr);
                dataPtr += 8;
                var y = *((double*) dataPtr);

                return GeometryFactory.CreatePoint(x, y);
            }

            if (_ShapeType == ShapeType.Multipoint || _ShapeType == ShapeType.MultiPointM || _ShapeType == ShapeType.MultiPointZ)
            {
                dataPtr += 32; // min/max box

                var points = new List<IPoint>();
                var nPoints = *(int*) dataPtr; // get the number of points
                dataPtr += 4;

                if (nPoints == 0)
                {
                    return null;
                }

                for (var i = 0; i < nPoints; i++)
                {
                    var x = *((double*) dataPtr);
                    dataPtr += 8;
                    var y = *((double*) dataPtr);
                    dataPtr += 8;

                    points.Add(GeometryFactory.CreatePoint(x, y));
                }

                return GeometryFactory.CreateMultiPoint(points.ToArray());
            }

            if (_ShapeType == ShapeType.PolyLine || _ShapeType == ShapeType.Polygon ||
                _ShapeType == ShapeType.PolyLineM || _ShapeType == ShapeType.PolygonM ||
                _ShapeType == ShapeType.PolyLineZ || _ShapeType == ShapeType.PolygonZ)
            {
                dataPtr += 32; // min/max

                int nParts = *(int*) dataPtr; // get number of parts (segments)
                dataPtr += 4;

                if (nParts == 0)
                {
                    return null;
                }

                int nPoints = *((int*) dataPtr); // get number of points
                dataPtr += 4;

                var segments = new int[nParts + 1];

                //Read in the segment indexes
                for (int b = 0; b < nParts; b++)
                {
                    segments[b] = *((int*) dataPtr);
                    dataPtr += 4;
                }

                //add end point
                segments[nParts] = nPoints;

                if ((int) _ShapeType%10 == 3)
                {
                    var mline = new List<ILineString>();
                    for (var lineId = 0; lineId < nParts; lineId++)
                    {
                        var line = new List<ICoordinate>();
                        for (var i = segments[lineId]; i < segments[lineId + 1]; i++)
                        {
                            var x = *((double*) dataPtr);
                            dataPtr += 8;
                            var y = *((double*) dataPtr);
                            dataPtr += 8;
                            line.Add(GeometryFactory.CreateCoordinate(x, y));
                        }

                        //line.Vertices.Add(new SharpMap.Geometries.Point(
                        mline.Add(GeometryFactory.CreateLineString(line.ToArray()));
                    }

                    if (mline.Count == 1)
                    {
                        return mline[0];
                    }

                    return GeometryFactory.CreateMultiLineString(mline.ToArray());
                }

                // TODO: check it!
                //(_ShapeType == ShapeType.Polygon etc...)
                {
                    //First read all the rings
                    //List<SharpMap.Geometries.LinearRing> rings = new List<SharpMap.Geometries.LinearRing>();
                    var rings = new List<ILinearRing>();
                    for (int RingID = 0; RingID < nParts; RingID++)
                    {
                        //SharpMap.Geometries.LinearRing ring = new SharpMap.Geometries.LinearRing();
                        var ring = new List<ICoordinate>();
                        for (int i = segments[RingID]; i < segments[RingID + 1]; i++)
                        {
                            ring.Add(GeometryFactory.CreateCoordinate(brShapeFile.ReadDouble(), brShapeFile.ReadDouble()));
                        }

                        // polygon should be closed, try to fix
                        if (!ring[ring.Count - 1].Equals2D(ring[0]))
                        {
                            ring.Add(GeometryFactory.CreateCoordinate(ring[0].X, ring[0].Y));
                        }

                        //ring.Vertices.Add(new SharpMap.Geometries.Point
                        rings.Add(GeometryFactory.CreateLinearRing(ring.ToArray()));
                    }
                    var IsCounterClockWise = new bool[rings.Count];
                    int PolygonCount = 0;
                    for (int i = 0; i < rings.Count; i++)
                    {
                        IsCounterClockWise[i] = GeometryFactory.IsCCW(rings[i].Coordinates);
                        if (!IsCounterClockWise[i])
                        {
                            PolygonCount++;
                        }
                    }
                    if (PolygonCount == 1) //We only have one polygon
                    {
                        ILinearRing shell = rings[0];
                        var holes = new List<ILinearRing>();
                        if (rings.Count > 1)
                        {
                            for (int i = 1; i < rings.Count; i++)
                            {
                                holes.Add(rings[i]);
                            }
                        }
                        return GeometryFactory.CreatePolygon(shell, holes.ToArray());
                    }
                    else
                    {
                        var polys = new List<IPolygon>();
                        ILinearRing shell = rings[0];
                        var holes = new List<ILinearRing>();
                        for (int i = 1; i < rings.Count; i++)
                        {
                            if (!IsCounterClockWise[i])
                            {
                                polys.Add(GeometryFactory.CreatePolygon(shell, null));
                                shell = rings[i];
                            }
                            else
                            {
                                holes.Add(rings[i]);
                            }
                        }
                        polys.Add(GeometryFactory.CreatePolygon(shell, holes.ToArray()));
                        return GeometryFactory.CreateMultiPolygon(polys.ToArray());
                    }
                }
            }
            else
            {
                throw (new ApplicationException("Shapefile type " + _ShapeType.ToString() + " not supported"));
            }
        }

        private unsafe IGeometry ReadPolygon(int oid)
        {
            var dataPtr = zeroPtr + recordHeaders[oid].Offset;
            var polygonRecord = (PolygonRecordP*) (dataPtr + 8);

            //First read all the rings
            int offset = polygonRecord->DataOffset;
            int parts = polygonRecord->NumParts;

            var rings = new ILinearRing[parts];

            for (int part = 0; part < parts; ++part)
            {
                int points;

                if ((parts - part) > 1)
                {
                    points = polygonRecord->PartOffsets[part + 1] - polygonRecord->PartOffsets[part];
                }
                else
                {
                    points = polygonRecord->NumPoints - polygonRecord->PartOffsets[part];
                }
                if (points <= 1)
                {
                    continue;
                }

                var ring = new ICoordinate[points];

                int index = 0;
                PointD* pointPtr = (PointD*) (dataPtr + 8 + offset + (polygonRecord->PartOffsets[part] << 4));
                PointD point = *(pointPtr++);

                ring[index] = GeometryFactory.CreateCoordinate(point.X, point.Y);
                ++index;

                while (index < points)
                {
                    point = *(pointPtr++);
                    ring[index] = GeometryFactory.CreateCoordinate(point.X, point.Y);
                    ++index;
                }

                // polygon should be closed, try to fix
                if (!ring[ring.Length - 1].Equals2D(ring[0]))
                {
                    ring[ring.Length - 1] = GeometryFactory.CreateCoordinate(ring[0].X, ring[0].Y);
                }

                rings[part] = GeometryFactory.CreateLinearRing(ring);
            }

            if (rings.Length == 1) //We only have one polygon
            {
                ILinearRing shell = rings[0];
                if (rings.Length > 1)
                {
                    var holes = new ILinearRing[rings.Length];
                    for (int i = 1; i < rings.Length; i++)
                    {
                        holes[i] = rings[i];
                    }
                    return GeometryFactory.CreatePolygon(shell, holes);
                }

                return GeometryFactory.CreatePolygon(shell, null);
            }
            else
            {
                var polys = new List<IPolygon>();
                ILinearRing shell = rings[0];
                var holes = new List<ILinearRing>();
                for (int i = 1; i < rings.Length; i++)
                {
                    if (!GeometryFactory.IsCCW(rings[i].Coordinates))
                    {
                        polys.Add(GeometryFactory.CreatePolygon(shell, null));
                        shell = rings[i];
                    }
                    else
                    {
                        holes.Add(rings[i]);
                    }
                }

                polys.Add(GeometryFactory.CreatePolygon(shell, holes.ToArray()));
                return GeometryFactory.CreateMultiPolygon(polys.ToArray());
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private unsafe struct RecordHeader
        {
            public readonly int RecordNumber;
            public int Offset;
            public int ContentLength;

            public RecordHeader(int recNum)
            {
                RecordNumber = recNum;
                ContentLength = 0;
                Offset = 0;
            }

            public void readFromIndexFile(byte[] data, int dataOffset)
            {
                Offset = EndianUtils.ReadIntBE(data, dataOffset) << 1; //offset in bytes
                ContentLength = EndianUtils.ReadIntBE(data, dataOffset + 4) << 1; //*2 because length is in words not bytes
            }
        }

        #region Disposers and finalizers

        private bool disposed = false;

        /// <summary>
        /// Disposes the object
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Close();
                    _Envelope = null;
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Finalizes the object
        /// </summary>
        ~ShapeFile()
        {
            Dispose();
        }

        #endregion

        #region IFeatureProvider Members

        public virtual Type FeatureType
        {
            get
            {
                return typeof(Feature);
            }
        }

        public virtual IList Features
        {
            get
            {
                //problem with reuse 
                return GetFeatures(GetExtents()).ToList();
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public virtual IFeature Add(IGeometry geometry)
        {
            throw new NotImplementedException();
        }

        public virtual Func<IFeatureProvider, IGeometry, IFeature> AddNewFeatureFromGeometryDelegate { get; set; }
        public virtual event EventHandler FeaturesChanged;
        public virtual event EventHandler CoordinateSystemChanged;

        /// <summary>
        /// Returns all objects whose boundingbox intersects bbox.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Please note that this method doesn't guarantee that the geometries returned actually intersect 'bbox', but only
        /// that their boundingbox intersects 'bbox'.
        /// </para>
        /// </remarks>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public virtual IEnumerable<IFeature> GetFeatures(IEnvelope bbox)
        {
            Open(path);

            if (!_IsOpen)
            {
                yield break; //return empty list in case there is no connection
            }

            if (dbaseFile == null)
            {
                yield break;
            }

            //Use the spatial index to get a list of features whose boundingbox intersects bbox
            for (int i = 0; i < _FeatureCount; i++)
            {
                var rect = GetBounds(i);
                if (rect.Intersects(bbox))
                {
                    var fdr = dbaseFile.GetFeature(i);
                    fdr.Geometry = ReadGeometry(i);

                    if (FilterDelegate == null || FilterDelegate(fdr))
                    {
                        yield return fdr;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns>geometry</returns>
        public virtual IGeometry GetGeometryByID(int oid)
        {
            if (FilterDelegate != null) //Apply filtering
            {
                // TODO: this should work as IFeature
                var fdr = (IFeature) GetFeature(oid);
                if (fdr != null)
                {
                    return fdr.Geometry;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return ReadGeometry(oid);
            }
        }

        /// <summary>
        /// Returns the total number of features in the datasource (without any filter applied)
        /// </summary>
        /// <returns></returns>
        public virtual int GetFeatureCount()
        {
            return _FeatureCount;
        }

        public virtual bool Contains(IFeature feature)
        {
            var shapefileFeature = feature as ShapeFileFeature;
            if (shapefileFeature == null)
            {
                return false;
            }

            return GetGeometryByID(shapefileFeature.Oid).EqualsExact(feature.Geometry);
        }

        public virtual int IndexOf(IFeature feature)
        {
            if (feature is ShapeFileFeature)
            {
                return Features.IndexOf(feature); // TODO: improve performance, use search by id in index and then compare geometry!
            }

            return -1;
        }

        /// <summary>
        /// Returns the extents of the datasource
        /// </summary>
        /// <returns></returns>
        public virtual IEnvelope GetExtents()
        {
            if (!IsOpen)
            {
                Open(path);
                if (!IsOpen)
                {
                    return null;
                }
            }

            return _Envelope;
        }

        /// <summary>
        /// Gets or sets the spatial reference system in WKT format.
        /// </summary>
        public virtual string SrsWkt
        {
            get
            {
                return srsWkt;
            }
            set
            {
                srsWkt = value;

                if (Map.CoordinateSystemFactory == null || string.IsNullOrEmpty(srsWkt))
                {
                    CoordinateSystem = null;
                    return;
                }

                CoordinateSystem = Map.CoordinateSystemFactory.CreateFromWkt(value);
            }
        }

        #endregion

        #region IFileBased Members

        /// <summary>
        /// Opens the datasource
        /// </summary>
        public virtual void Open(string path)
        {
            // Get a Connector.  The connector returned is guaranteed to be connected and ready to go.
            // Pooling.Connector connector = Pooling.ConnectorPool.ConnectorPoolManager.RequestConnector(this,true);

            logNoShapeFile = true;

            if (!_IsOpen || this.path != path)
            {
                if (!File.Exists(path))
                {
                    if (logNoShapeFile)
                    {
                        log.Error("Could not find " + path);
                        logNoShapeFile = false;
                    }
                    return;
                }

                try
                {
                    unsafe
                    {
                        this.path = path;
                        //tree = null;

                        //Initialize DBF
                        string dbffile = GetDbaseFilePath();
                        if (File.Exists(dbffile))
                        {
                            dbaseFile = new DbaseReader(dbffile);
                        }
                        //Parse shape header
                        ParseHeader();
                        //Read projection file
                        ParseProjection();

                        fsShapeIndex = new FileStream(GetIndexFilePath(), FileMode.Open, FileAccess.Read);
                        brShapeIndex = new BinaryReader(fsShapeIndex, Encoding.Unicode);
                        fsShapeFile = new FileStream(this.path, FileMode.Open, FileAccess.Read);
                        brShapeFile = new BinaryReader(fsShapeFile);

                        shpFileMemoryMap = NativeMethods.MapFile(fsShapeFile);
                        shpFileMemoryMapView = NativeMethods.MapViewOfFile(shpFileMemoryMap,
                                                                           NativeMethods.FileMapAccess.FILE_MAP_READ, 0,
                                                                           0, 0);

                        zeroPtr = (byte*) shpFileMemoryMapView.ToPointer();

                        InitializeShape(this.path, _FileBasedIndex);

                        if (dbaseFile != null)
                        {
                            dbaseFile.Open();
                        }
                        _IsOpen = true;
                    }
                }
                catch (IOException e)
                {
                    log.Error(e.Message);
                    _IsOpen = false;
                }
            }
        }

        private string GetIndexFilePath()
        {
            if (path == null)
            {
                return null;
            }

            return path.Remove(path.Length - 4, 4) + ".shx";
        }

        private string GetDbaseFilePath()
        {
            if (path == null)
            {
                return null;
            }

            return path.Substring(0, path.LastIndexOf(".")) + ".dbf";
        }

        public virtual void CreateNew(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the datasource
        /// </summary>
        public virtual void Close()
        {
            if (!disposed)
            {
                //TODO: (ConnectionPooling)
                /*	if (connector != null)
					{ Pooling.ConnectorPool.ConnectorPoolManager.Release...()
				}*/
                if (_IsOpen)
                {
                    brShapeFile.Close();
                    fsShapeFile.Close();
                    brShapeIndex.Close();
                    fsShapeIndex.Close();
                    if (dbaseFile != null)
                    {
                        dbaseFile.Close();
                    }

                    NativeMethods.UnmapViewOfFile(idxFileMemoryMapView);
                    NativeMethods.CloseHandle(idxFileMemoryMap);
                    idxFileMemoryMap = IntPtr.Zero;
                    idxFileMemoryMapView = IntPtr.Zero;

                    NativeMethods.UnmapViewOfFile(shpFileMemoryMapView);
                    NativeMethods.CloseHandle(shpFileMemoryMap);
                    shpFileMemoryMapView = IntPtr.Zero;

                    _IsOpen = false;
                }
            }
        }

        /// <summary>
        /// Returns true if the datasource is currently open
        /// </summary>		
        public virtual bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
        }

        public virtual void SwitchTo(string newPath)
        {
            Close();
            Open(newPath);
        }

        public virtual void CopyTo(string destinationPath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
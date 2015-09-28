using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace DelftTools.Utils.NetCdf
{
    /// <summary>
    /// This object represents a netcdf file 
    /// </summary>
    public class NetCdfFile
    {
        private readonly int id;
        private readonly string path;
        private const NetCdfWrapper.CreateMode FileCreateMode = NetCdfWrapper.CreateMode.NC_CLASSIC;

        private IDictionary<string, NetCdfVariable> ncVariableLookupByName;

        public string Path
        {
            get { return path; }
        }

        private NetCdfFile(string filePath, int ncId)
        {
            path = filePath;
            id = ncId;

            ncVariableLookupByName = new Dictionary<string, NetCdfVariable>();
        }
        
        public static NetCdfFile CreateNew(string path, bool fill = false)
        {
            int id;
            CheckResult(NetCdfWrapper.nc_create(path, FileCreateMode, out id));
            var ncFile = new NetCdfFile(path, id);
            ncFile.SetFill(fill);
            return ncFile;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="writeAccess">opens readonly when true</param>
        /// <returns></returns>
        public static NetCdfFile OpenExisting(string path, bool writeAccess = false)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            if (!IsValidNetCdfFile(path))
                CheckResult(-51); // netcdf: file does not appear to have a valid format

            int id;
            var mode = writeAccess ? NetCdfWrapper.CreateMode.NC_WRITE : NetCdfWrapper.CreateMode.NC_NOWRITE;
            CheckResult(NetCdfWrapper.nc_open(path, mode, out id));

            var netcdf = new NetCdfFile(path, id);
            netcdf.BuildVariableLookup();
            return netcdf;
        }

        /// <summary>
        /// Checks if the file is a valid netcdf file by checking the magic number (first 4 bytes of the stream).
        /// Note: nc_open also performs this check, but due to a .b.u.g. it doesn't unlock the file when that check
        /// fails.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsValidNetCdfFile(string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // source: netcdf-4.3.0/libdispatch/dfile.c::NC_check_file_type
                    var magic = new byte[4];
                    var read = stream.Read(magic, 0, 4);

                    if (read == 4)
                    {
                        return (magic[0] == 'C' && magic[1] == 'D' && magic[2] == 'F') || //netcdf classic (3)
                               (magic[1] == 'H' && magic[2] == 'D' && magic[3] == 'F') || //hdf 5
                               (magic[0] == 16 && magic[1] == 3 && magic[2] == 23 && magic[3] == 1); //hdf 4

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return false;
        }

        private void BuildVariableLookup()
        {
            UpdateVariableLookup(ref ncVariableLookupByName);
        }

        public void Create()
        {
            CheckResult(NetCdfWrapper.nc_enddef(id));
        }
        
        public void ReDefine()
        {
            CheckResult(NetCdfWrapper.nc_redef(id));
        }

        public void EndDefine()
        {
            CheckResult(NetCdfWrapper.nc_enddef(id));
        }

        public void Close()
        {
            ncVariableLookupByName.Clear();
            CheckResult(NetCdfWrapper.nc_close(id));
        }

        public void Flush()
        {
            CheckResult(NetCdfWrapper.nc_sync(id));
        }

        public void SetFill(bool shouldFill)
        {
            int oldMode;
            CheckResult(NetCdfWrapper.nc_set_fill(id, shouldFill
                                              ? (int) NetCdfWrapper.CreateMode.NC_FILL
                                              : (int) NetCdfWrapper.CreateMode.NC_NOFILL, out oldMode));
        }

        /// <summary>
        /// write array to file, origin is assumed at 0, and shape is taken from array
        /// </summary>
        /// <param name="ncVariable"></param>
        /// <param name="array"></param>
        public void Write(NetCdfVariable ncVariable, Array array)
        {
            var shape = array.GetShape();
            var origin = new int[array.Rank];

            Write(ncVariable, origin, shape, array);
        }

        public void Write(NetCdfVariable ncVariable, int[] origin, int[] shape, Array array)
        {
            var originPtr = NetCdfFileHelper.ConvertToIntPtr(origin);
            var shapePtr = NetCdfFileHelper.ConvertToIntPtr(shape);
            var ncDataType = GetNetCdfDataType(array);

            switch (ncDataType)
            {
                case NetCdfDataType.NC_BYTE:
                    if (shape.Length > 1)
                        array = NetCdfFileHelper.FlattenArray<byte>(array);
                    CheckResult(NetCdfWrapper.nc_put_vara(id, ncVariable, originPtr, shapePtr, (byte[])array));
                    break;
                case NetCdfDataType.NC_CHAR:
                    var bytes = NetCdfFileHelper.FlattenCharArray(array, shape);
                    CheckResult(NetCdfWrapper.nc_put_vara_text(id, ncVariable, originPtr, shapePtr, bytes));
                    break;
                case NetCdfDataType.NC_INT:
                    if (shape.Length > 1)
                        array = NetCdfFileHelper.FlattenArray<int>(array);
                    CheckResult(NetCdfWrapper.nc_put_vara_int(id, ncVariable, originPtr, shapePtr, (int[]) array));
                    break;
                case NetCdfDataType.NC_SHORT:
                    if (shape.Length > 1)
                        array = NetCdfFileHelper.FlattenArray<short>(array);
                    CheckResult(NetCdfWrapper.nc_put_vara_short(id, ncVariable, originPtr, shapePtr, (short[])array));
                    break;
                case NetCdfDataType.NC_FLOAT:
                    if (shape.Length > 1)
                        array = NetCdfFileHelper.FlattenArray<float>(array);
                    CheckResult(NetCdfWrapper.nc_put_vara_float(id, ncVariable, originPtr, shapePtr, (float[])array));
                    break;
                case NetCdfDataType.NC_DOUBLE:
                    if (shape.Length > 1)
                        array = NetCdfFileHelper.FlattenArray<double>(array);
                    CheckResult(NetCdfWrapper.nc_put_vara_double(id, ncVariable, originPtr, shapePtr, (double[]) array));
                    break;
                default:
                    throw new Exception(
                        String.Format("Unknown type for writing NetCDF variable to file: type {0} to file {1}",
                                      ncDataType, path));
            }
        }

        /// <summary>
        /// Get mapped netcdf data type for array elements, special case for char[]
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private NetCdfDataType GetNetCdfDataType(Array array)
        {
            var type = array.GetType().GetElementType();
            if (type == typeof (char[]))
            {
                return NetCdfDataType.NC_CHAR;
            }
            return NetCdfWrapper.GetNetCdf3DataType(type);
        }

        /// <summary>
        /// Read out the entire variable from the file
        /// </summary>
        /// <param name="ncVariable"></param>
        /// <returns></returns>
        public Array Read(NetCdfVariable ncVariable)
        {
            var type = GetDataType(ncVariable);
            var size = GetSize(ncVariable);
            var shape = GetShape(ncVariable);

            switch (type)
            {
                case NetCdfDataType.NC_BYTE:
                    var byteArray = new byte[size];
                    CheckResult(NetCdfWrapper.nc_get_var(id, ncVariable, byteArray));
                    return NetCdfFileHelper.CreateArrayFromShape(byteArray, shape);
                case NetCdfDataType.NC_CHAR:
                    var charArray = new byte[size];
                    CheckResult(NetCdfWrapper.nc_get_var_text(id, ncVariable, charArray));
                    return NetCdfFileHelper.CreateCharArrayFromShape(charArray, shape);
                case NetCdfDataType.NC_INT:
                    var intArray = new int[size];
                    CheckResult(NetCdfWrapper.nc_get_var_int(id, ncVariable, intArray));
                    return NetCdfFileHelper.CreateArrayFromShape(intArray, shape);
                case NetCdfDataType.NC_SHORT:
                    var shortArray = new short[size];
                    CheckResult(NetCdfWrapper.nc_get_var_short(id, ncVariable, shortArray));
                    return NetCdfFileHelper.CreateArrayFromShape(shortArray, shape);
                case NetCdfDataType.NC_FLOAT:
                    var floatArray = new float[size];
                    CheckResult(NetCdfWrapper.nc_get_var_float(id, ncVariable, floatArray));
                    return NetCdfFileHelper.CreateArrayFromShape(floatArray, shape);
                case NetCdfDataType.NC_DOUBLE:
                    var doubleArray = new double[size];
                    CheckResult(NetCdfWrapper.nc_get_var_double(id, ncVariable, doubleArray));
                    return NetCdfFileHelper.CreateArrayFromShape(doubleArray, shape);
                default:
                    throw new Exception(
                        String.Format("Unknown type for reading NetCDF variable from file: type {0} from file {1}",
                                      type, path));
            }
        }
        
        /// <summary>
        /// Read a part of the variable values from file
        /// </summary>
        /// <param name="ncVariable"></param>
        /// <param name="origin"></param>
        /// <param name="shape">When shape is -1, this corresponds to reading that entire dimension</param>
        /// <returns></returns>
        public Array Read(NetCdfVariable ncVariable, int[] origin, int[] shape)
        {
            CreateShapeForFullRange(ncVariable, ref shape);
            var size = NetCdfFileHelper.GetSize(shape);
            var originPtr = NetCdfFileHelper.ConvertToIntPtr(origin);
            var shapePtr = NetCdfFileHelper.ConvertToIntPtr(shape);
            var type = GetDataType(ncVariable);
           
            switch (type)
            {
                case NetCdfDataType.NC_BYTE:
                    var byteArray = new byte[size];
                    CheckResult(NetCdfWrapper.nc_get_vara(id, ncVariable, originPtr, shapePtr, byteArray));
                    return NetCdfFileHelper.CreateArrayFromShape(byteArray, shape);
                case NetCdfDataType.NC_CHAR:
                    if (shape.Length != 2)
                    {
                        throw new NotSupportedException(
                            "NetCdf: only char arrays for independent string variables supported");
                    }
                    var charArray = new byte[size];
                    CheckResult(NetCdfWrapper.nc_get_vara_text(id, ncVariable, originPtr, shapePtr, charArray));
                    return NetCdfFileHelper.CreateCharArrayFromShape(charArray, shape);
                case NetCdfDataType.NC_INT:
                    var intArray = new int[size];
                    CheckResult(NetCdfWrapper.nc_get_vara_int(id, ncVariable, originPtr, shapePtr, intArray));
                    return NetCdfFileHelper.CreateArrayFromShape(intArray, shape);
                case NetCdfDataType.NC_FLOAT:
                    var floatArray = new float[size];
                    CheckResult(NetCdfWrapper.nc_get_vara_float(id, ncVariable, originPtr, shapePtr, floatArray));
                    return NetCdfFileHelper.CreateArrayFromShape(floatArray, shape);
                case NetCdfDataType.NC_DOUBLE:
                    var doubleArray = new double[size];
                    CheckResult(NetCdfWrapper.nc_get_vara_double(id, ncVariable, originPtr, shapePtr, doubleArray));
                    return NetCdfFileHelper.CreateArrayFromShape(doubleArray, shape);
                default:
                    throw new Exception(
                        String.Format("Unknown type for reading NetCDF variable from file: type {0} from file {1}",
                                      type, path));
            }
        }

        /// <summary>
        /// Read a part of the variable from file, given a certain stride
        /// </summary>
        /// <param name="ncVariable"></param>
        /// <param name="origin"></param>
        /// <param name="shape">When shape is -1, this corresponds to reading that entire dimension</param>
        /// <param name="stride"></param>
        /// <returns></returns>
        public Array Read(NetCdfVariable ncVariable, int[] origin, int[] shape, int[] stride)
        {
            CreateShapeForFullRange(ncVariable, ref shape);
            var count = Enumerable.Range(0, origin.Length).Select(i => (shape[i])/stride[i]).ToArray();
            var size = NetCdfFileHelper.GetSize(count);

            var originPtr = NetCdfFileHelper.ConvertToIntPtr(origin);
            var stridePtr = NetCdfFileHelper.ConvertToIntPtr(stride);
            var countPtr = NetCdfFileHelper.ConvertToIntPtr(count);

            var type = GetDataType(ncVariable);
            switch (type)
            {
                case NetCdfDataType.NC_BYTE:
                    var byteArray = new byte[size];
                    CheckResult(NetCdfWrapper.nc_get_vars(id, ncVariable, originPtr, countPtr, stridePtr, byteArray));
                    return NetCdfFileHelper.CreateArrayFromShape(byteArray, count);
                case NetCdfDataType.NC_CHAR:
                    if (shape.Length != 2)
                    {
                        throw new NotSupportedException(
                            "NetCdf: only char arrays for independent string variables supported");
                    }
                    var charArray = new byte[size];
                    CheckResult(NetCdfWrapper.nc_get_vars_text(id, ncVariable, originPtr, countPtr, stridePtr, charArray));
                    return NetCdfFileHelper.CreateCharArrayFromShape(charArray, count);
                case NetCdfDataType.NC_INT:
                    var intArray = new int[size];
                    CheckResult(NetCdfWrapper.nc_get_vars_int(id, ncVariable, originPtr, countPtr, stridePtr, intArray));
                    return NetCdfFileHelper.CreateArrayFromShape(intArray, count);
                case NetCdfDataType.NC_FLOAT:
                    var floatArray = new float[size];
                    CheckResult(NetCdfWrapper.nc_get_vars_float(id, ncVariable, originPtr, countPtr, stridePtr, floatArray));
                    return NetCdfFileHelper.CreateArrayFromShape(floatArray, count);
                case NetCdfDataType.NC_DOUBLE:
                    var doubleArray = new double[size];
                    CheckResult(NetCdfWrapper.nc_get_vars_double(id, ncVariable, originPtr, countPtr, stridePtr, doubleArray));
                    return NetCdfFileHelper.CreateArrayFromShape(doubleArray, count);
                default:
                    throw new Exception(
                        String.Format("Unknown type for reading NetCDF variable from file: type {0} from file {1}",
                                      type, path));
            }
        }

        /// <summary>
        /// When shape = -1, this is replaced by the size of the dimension
        /// as read from the netcdf file
        /// </summary>
        /// <param name="ncVariable"></param>
        /// <param name="shape"></param>
        private void CreateShapeForFullRange(NetCdfVariable ncVariable, ref int[] shape)
        {
            for (int i = 0; i < shape.Length; ++i)
            {
                if (shape[i] != -1) continue;
                shape[i] = GetShape(ncVariable)[i];
            }
        }

        
        #region methods on variables

        private NetCdfDataType GetDataType(NetCdfVariable ncVariable)
        {
            NetCdfDataType type;
            CheckResult(NetCdfWrapper.nc_inq_vartype(id, ncVariable, out type));
            return type;
        }

        public NetCdfVariable GetVariableByName(string name)
        {
            return ncVariableLookupByName.ContainsKey(name) ? ncVariableLookupByName[name] : null;
        }

        public void SetCaching(NetCdfVariable ncVariable, bool caching)
        {
            // todo: what should we do here?
        }

        public int[] GetShape(NetCdfVariable ncVariable)
        {
            var dimIds = GetDimensionIds(ncVariable);
            var nDims = dimIds.Length;
            var shape = new int[nDims];

            for (int i = 0; i < nDims; ++i)
            {
                IntPtr len;
                CheckResult(NetCdfWrapper.nc_inq_dimlen(id, dimIds[i], out len));
                shape[i] = len.ToInt32();
            }

            return shape;
        }

        public int GetSize(NetCdfVariable ncVariable)
        {
            var shape = GetShape(ncVariable);
            return NetCdfFileHelper.GetSize(shape);
        }

        public bool IsCharArray(NetCdfVariable ncVariable)
        {
            int nDims;
            NetCdfWrapper.nc_inq_varndims(id, ncVariable, out nDims);
            if (nDims < 2) return false;

            NetCdfDataType ncType;
            CheckResult(NetCdfWrapper.nc_inq_vartype(id, ncVariable, out ncType));
            return (ncType == NetCdfDataType.NC_CHAR);
        }
       
        public NetCdfVariable AddVariable(string varName, Type type, NetCdfDimension[] ncDimensions)
        {
            var netCdf3DataType = NetCdfWrapper.GetNetCdf3DataType(type);

            return AddVariable(varName, netCdf3DataType, ncDimensions);
        }

        public NetCdfVariable AddVariable(string varName, NetCdfDataType netCdfType, NetCdfDimension[] ncDimensions)
        {
            int varId;
            CheckResult(NetCdfWrapper.nc_def_var(id, varName, netCdfType, ncDimensions.Length,
                                                 ncDimensions.Select(d => (int) d).ToArray(), out varId));
            var ncVar = new NetCdfVariable(varId);

            // variable name is unique here, guaranteed by nc_def_var call above
            ncVariableLookupByName.Add(varName, ncVar);
            return ncVar;
        }

        /// <summary>
        /// Reads the variables from the cached lookup
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NetCdfVariable> GetVariables()
        {
            return ncVariableLookupByName.Values;
        }

        /// <summary>
        /// Reads the variables from the netcdf file, needed when opening a new file
        /// </summary>
        /// <returns></returns>
        private void UpdateVariableLookup(ref IDictionary<string, NetCdfVariable> lookup)
        {
            lookup.Clear();

            int nVars;
            CheckResult(NetCdfWrapper.nc_inq_nvars(id, out nVars));

            var variables = Enumerable.Range(0, nVars).Select(vi => new NetCdfVariable(vi)).ToList();
            foreach (var ncVar in variables)
            {
                var variableName = GetVariableName(ncVar);
                lookup.Add(variableName, ncVar);
            }
        }

        public string GetVariableName(NetCdfVariable ncVariable)
        {
            var nameBuilder = new StringBuilder((int)NetCdfWrapper.Limits.NC_MAX_NAME);
            CheckResult(NetCdfWrapper.nc_inq_varname(id, ncVariable, nameBuilder));
            return nameBuilder.ToString();
        }

        public NetCdfDataType GetVariableDataType(NetCdfVariable ncVariable)
        {
            NetCdfDataType type;
            CheckResult(NetCdfWrapper.nc_inq_vartype(id, ncVariable, out type));
            return type;
        }

        public bool IsVariableUnlimited(NetCdfVariable ncVariable)
        {
            int nDims;
            CheckResult(NetCdfWrapper.nc_inq_varndims(id, ncVariable, out nDims));

            var dimIds = new int[nDims];
            CheckResult(NetCdfWrapper.nc_inq_vardimid(id, ncVariable, dimIds));

            int unlimitedDimId;
            CheckResult(NetCdfWrapper.nc_inq_unlimdim(id, out unlimitedDimId));

            return dimIds.Contains(unlimitedDimId);
        }

        #endregion
        
        #region methods on dimensions
 

        public NetCdfDimension GetDimension(string dimName)
        {
            int dimId;
            var status = NetCdfWrapper.nc_inq_dimid(id, dimName, out dimId);
            return (status == (int)NetCdfWrapper.ResultCode.NC_NOERR) ? new NetCdfDimension(dimId) : null;
        }

        public IEnumerable<NetCdfDimension> GetAllDimensions()
        {
            int nDims;
            CheckResult(NetCdfWrapper.nc_inq_ndims(id, out nDims));

            // NETCDF-3: dimension has id between 0 and nDims-1:
            return Enumerable.Range(0, nDims).Select(d => new NetCdfDimension(d));
        }

        private int[] GetDimensionIds(NetCdfVariable ncVariable)
        {
            int nDims;
            CheckResult(NetCdfWrapper.nc_inq_varndims(id, ncVariable, out nDims));

            var dimIds = new int[nDims];
            NetCdfWrapper.nc_inq_vardimid(id, ncVariable, dimIds);
            return dimIds;
        }

        public IEnumerable<NetCdfDimension> GetDimensions(NetCdfVariable ncVariable)
        {
            return GetVariableDimensionNames(ncVariable).Select(GetDimension);
        }

        public IEnumerable<string> GetVariableDimensionNames(NetCdfVariable ncVariable)
        {
            var dimensions = GetDimensionIds(ncVariable);
            foreach (var dimId in dimensions)
            {
                var nameBuilder = new StringBuilder((int) NetCdfWrapper.Limits.NC_MAX_NAME);
                CheckResult(NetCdfWrapper.nc_inq_dimname(id, dimId, nameBuilder));

                yield return nameBuilder.ToString();
            }
        }

        public string GetDimensionName(NetCdfDimension ncDimension)
        {
            var nameBuilder = new StringBuilder((int)NetCdfWrapper.Limits.NC_MAX_NAME);
            CheckResult(NetCdfWrapper.nc_inq_dimname(id, ncDimension, nameBuilder));
            return nameBuilder.ToString();
        }

        public int GetDimensionLength(NetCdfDimension ncDimension)
        {
            IntPtr length;
            CheckResult(NetCdfWrapper.nc_inq_dimlen(id, ncDimension, out length));
            return length.ToInt32();
        }


        public int GetDimensionLength(string dimensionName)
        {
            var dim = GetDimension(dimensionName);
            return GetDimensionLength(dim);
        }

        public NetCdfDimension AddDimension(string dimName, int dimSize)
        {
            int dimId;
            CheckResult(NetCdfWrapper.nc_def_dim(id, dimName, new IntPtr(dimSize), out dimId));
            return new NetCdfDimension(dimId);
        }

        public NetCdfDimension AddUnlimitedDimension(string dimName)
        {
            int dimId;
            CheckResult(NetCdfWrapper.nc_def_dim(id, dimName, new IntPtr(NetCdfWrapper.NC_UNLIMITED), out dimId));
            return new NetCdfDimension(dimId);
        }

        #endregion

        #region methods on attributes
        
        public void AddGlobalAttribute(NetCdfAttribute ncAttribute)
        {
            WriteAttribute(NetCdfWrapper.NC_GLOBAL, ncAttribute);
        }

        public void AddAttribute(NetCdfVariable ncVariable, NetCdfAttribute ncAttribute)
        {
           WriteAttribute(ncVariable, ncAttribute);
        }

        private void WriteAttribute(int varId, NetCdfAttribute ncAttribute)
        {
            var value = ncAttribute.Value;
            if (value is string)
            {
                var str = (string) value;
                CheckResult(NetCdfWrapper.nc_put_att_text(id, varId, ncAttribute.Name,
                                                          new IntPtr(str.Length), UTF8Marshal.StringUTF8ToPtr(str)));
            }
            else if (value is double)
            {
                var d = (double) value;
                CheckResult(NetCdfWrapper.nc_put_att_double(id, varId, ncAttribute.Name,
                                                            NetCdfDataType.NC_DOUBLE, new IntPtr(1),
                                                            new[] {d} ));
            }
            else if (value is float)
            {
                var i = (float)value;
                CheckResult(NetCdfWrapper.nc_put_att_float(id, varId, ncAttribute.Name,
                                                            NetCdfDataType.NC_FLOAT, new IntPtr(1),
                                                            new[] { i }));
            }
            else if (value is int)
            {
                var i = (int)value;
                CheckResult(NetCdfWrapper.nc_put_att_int(id, varId, ncAttribute.Name,
                                                            NetCdfDataType.NC_INT, new IntPtr(1),
                                                            new[] { i }));
            }
            else
            {
                throw new NotImplementedException(string.Format("NetCdf Attribute type '{0}' not implemented",
                                                                value != null ? value.GetType().ToString() : "<null>"));
            }
        }
        
        public NetCdfAttribute GetGlobalAttribute(string attributeName)
        {
            return GetAttribute(NetCdfWrapper.NC_GLOBAL, attributeName);
        }

        public Dictionary<string, object> GetGlobalAttributes()
        {
            return GetAttributesCore(NetCdfWrapper.NC_GLOBAL);
        }

        public Dictionary<string, object> GetAttributes(NetCdfVariable ncVariable)
        {
            return GetAttributesCore(ncVariable);
        }

        private Dictionary<string, object> GetAttributesCore(int varId)
        {
            var nameValueDictionary = new Dictionary<string, object>();

            int nAtts;
            CheckResult(NetCdfWrapper.nc_inq_varnatts(id, varId, out nAtts));

            for (int i = 0; i < nAtts; ++i)
            {
                var nameBuilder = new StringBuilder((int)NetCdfWrapper.Limits.NC_MAX_NAME);
                NetCdfWrapper.nc_inq_attname(id, varId, i, nameBuilder);
                var name = nameBuilder.ToString();

                var attribute = GetAttribute(varId, name);
                nameValueDictionary.Add(attribute.Name, attribute.Value);
            }

            return nameValueDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varId"></param>
        /// <param name="attributeName"></param>
        /// <returns>null when attribute does not exist</returns>
        private NetCdfAttribute GetAttribute(int varId, string attributeName)
        {
            NetCdfDataType type;
            IntPtr lengthPtr;
            var status = NetCdfWrapper.nc_inq_att(id, varId, attributeName, out type, out lengthPtr);
            int length = lengthPtr.ToInt32();

            if (status == (int)NetCdfWrapper.ResultCode.NC_ENOTATT) return null;
            CheckResult(status);
            
            switch (type)
            {
                
                case NetCdfDataType.NC_CHAR:
                    var charArray = new byte[length+1];
                    IntPtr ptr = UTF8Marshal.BytesUTF8ToPtr(charArray);
                    CheckResult(NetCdfWrapper.nc_get_att_text(id, varId, attributeName, ptr));
                    charArray[length] = 0;
                    return new NetCdfAttribute(attributeName, UTF8Marshal.PtrToStringUTF8(ptr));
                case NetCdfDataType.NC_INT:
                    var intArray = new int[length];
                    CheckResult(NetCdfWrapper.nc_get_att_int(id, varId, attributeName, intArray));
                    return new NetCdfAttribute(attributeName, intArray[0]);
                case NetCdfDataType.NC_FLOAT:
                    var floatArray = new float[length];
                    CheckResult(NetCdfWrapper.nc_get_att_float(id, varId, attributeName, floatArray));
                    return new NetCdfAttribute(attributeName, floatArray[0]);
                case NetCdfDataType.NC_DOUBLE:
                    var doubleArray = new double[length];
                    CheckResult(NetCdfWrapper.nc_get_att_double(id, varId, attributeName, doubleArray));
                    return new NetCdfAttribute(attributeName, doubleArray[0]);
                default:
                    throw new Exception(
                        String.Format("Unknown type {0} for reading NetCDF attribute {1} from file {2}",
                                      type, attributeName, path));
            }

        }

        /// <summary>
        /// Use this when the attribute might not exist
        /// </summary>
        /// <param name="ncVariable"></param>
        /// <param name="attributeName"></param>
        /// <returns>The attribute value as a string, or null</returns>
        public string GetAttributeValue(NetCdfVariable ncVariable, string attributeName)
        {
            var attribute = GetAttribute(ncVariable, attributeName);
            if (attribute != null) return attribute.Value.ToString();
            return null;
        }

        #endregion
        
        /// <summary>
        /// Check native netcdf status code, throws when error, and displays error code and message
        /// </summary>
        /// <param name="result"></param>
        private static void CheckResult(int result)
        {
            if (result != (int)NetCdfWrapper.ResultCode.NC_NOERR)
            {
                if (result > 0) //system error code
                    throw new Win32Exception(result);
                else //netcdf error code
                    throw new Exception(String.Format("NetCDF error code {0}: " + NetCdfWrapper.nc_strerror(result),
                                                      result));
            }
        }
    }
}
// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DelftTools.Utils.Interop;

namespace DelftTools.Utils.NetCdf
{
    public static partial class NetCdfWrapper
    {
        // Locking has been commented out due to TOOLS-9969 for performance reasons. An initial run on the test server did not reveal any problems.
        static readonly object LockObject = new object();

        public static int nc_open(string path, CreateMode mode, out int ncidp) { lock (LockObject) { var r = NetCDFInterop.nc_open(path, mode, out ncidp); return r; } }

        public static int nc_create(string path, CreateMode mode, out int ncidp) { lock (LockObject) { var r = NetCDFInterop.nc_create(path, mode, out ncidp); return r; } }

        public static int nc_close(int ncidp) { lock (LockObject) { var r = NetCDFInterop.nc_close(ncidp); return r; } }

        public static int nc_sync(int ncid) { lock (LockObject){ var r = NetCDFInterop.nc_sync(ncid); return r; } }

        public static int nc_redef(int ncid) { lock (LockObject) { var r = NetCDFInterop.nc_redef(ncid); return r; } }

        public static int nc_enddef(int ncid) { lock (LockObject){ var r = NetCDFInterop.nc_enddef(ncid); return r; } }

        public static int nc_set_fill(int ncid, int fillmode, out int oldmodep) { lock (LockObject){ var r = NetCDFInterop.nc_set_fill(ncid, fillmode, out oldmodep); return r; } }

        public static int nc_def_var(int ncid, string name, NetCdfDataType xtype, int ndims, int[] dimids, out int varidp) { lock (LockObject){ var r = NetCDFInterop.nc_def_var(ncid, name, xtype, ndims, dimids, out varidp); return r; } }
        
        public static int nc_inq_ndims(int ncid, out int ndims) { lock (LockObject) { var r = NetCDFInterop.nc_inq_ndims(ncid, out ndims); return r; } }
        
        public static int nc_inq_nvars(int ncid, out int nvars) { lock (LockObject) { var r = NetCDFInterop.nc_inq_nvars(ncid, out nvars); return r; } }

        public static int nc_inq_vartype(int ncid, int varid, out NetCdfDataType xtypep) { lock (LockObject){ var r = NetCDFInterop.nc_inq_vartype(ncid, varid, out xtypep); return r; } }

        public static int nc_inq_varnatts(int ncid, int varid, out int nattsp) { lock (LockObject){ var r = NetCDFInterop.nc_inq_varnatts(ncid, varid, out nattsp); return r; } }

        public static int nc_inq_varname(int ncid, int varid, StringBuilder name) { lock (LockObject){ var r = NetCDFInterop.nc_inq_varname(ncid, varid, name); return r; } }

        public static int nc_inq_varndims(int ncid, int varid, out int ndims) { lock (LockObject){ var r = NetCDFInterop.nc_inq_varndims(ncid, varid, out ndims); return r; } }

        public static int nc_inq_vardimid(int ncid, int varid, int[] dimids) { lock (LockObject){ var r = NetCDFInterop.nc_inq_vardimid(ncid, varid, dimids); return r; } }

        public static int nc_inq_unlimdim(int ncid, out int unlimdimid) { lock (LockObject){ var r = NetCDFInterop.nc_inq_unlimdim(ncid, out unlimdimid); return r; } }

        public static int nc_inq_attname(int ncid, int varid, int attnum, StringBuilder name) { lock (LockObject){ var r = NetCDFInterop.nc_inq_attname(ncid, varid, attnum, name); return r; } }

        public static int nc_inq_att(int ncid, int varid, string name, out NetCdfDataType type, out IntPtr length) { lock (LockObject){ var r = NetCDFInterop.nc_inq_att(ncid, varid, name, out type, out length); return r; } }

        public static int nc_get_att_text(int ncid, int varid, string name, IntPtr value) { lock (LockObject){ var r = NetCDFInterop.nc_get_att_text(ncid, varid, name, value); return r; } }

        public static int nc_get_att_int(int ncid, int varid, string name, int[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_att_int(ncid, varid, name, data); return r; } }

        public static int nc_get_att_float(int ncid, int varid, string name, float[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_att_float(ncid, varid, name, data); return r; } }

        public static int nc_get_att_double(int ncid, int varid, string name, double[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_att_double(ncid, varid, name, data); return r; } }

        public static int nc_put_att_text(int ncid, int varid, string name, IntPtr len, IntPtr tp) { lock (LockObject){ var r = NetCDFInterop.nc_put_att_text(ncid, varid, name, len, tp); return r; } }

        public static int nc_put_att_double(int ncid, int varid, string name, NetCdfDataType type, IntPtr len, double[] tp) { lock (LockObject){ var r = NetCDFInterop.nc_put_att_double(ncid, varid, name, type, len, tp); return r; } }

        public static int nc_put_att_float(int ncid, int varid, string name, NetCdfDataType type, IntPtr len, float[] tp) { lock (LockObject) { var r = NetCDFInterop.nc_put_att_float(ncid, varid, name, type, len, tp); return r; } }

        public static int nc_put_att_int(int ncid, int varid, string name, NetCdfDataType type, IntPtr len, int[] tp) { lock (LockObject) { var r = NetCDFInterop.nc_put_att_int(ncid, varid, name, type, len, tp); return r; } }

        public static int nc_def_dim(int ncid, string name, IntPtr len, out int dimidp) { lock (LockObject){ var r = NetCDFInterop.nc_def_dim(ncid, name, len, out dimidp); return r; } }

        public static int nc_inq_dimname(int ncid, int dimid, StringBuilder name) { lock (LockObject){ var r = NetCDFInterop.nc_inq_dimname(ncid, dimid, name); return r; } }

        public static int nc_inq_dimid(int ncid, string name, out int dimid) { lock (LockObject){ var r = NetCDFInterop.nc_inq_dimid(ncid, name, out dimid); return r; } }

        public static int nc_inq_dimlen(int ncid, int dimid, out IntPtr length) { lock (LockObject){ var r = NetCDFInterop.nc_inq_dimlen(ncid, dimid, out length); return r; } }

        public static int nc_get_var(int ncid, int varid, byte[] data) { lock (LockObject) { var r = NetCDFInterop.nc_get_var(ncid, varid, data); return r; } }

        public static int nc_get_var_text(int ncid, int varid, byte[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_var_text(ncid, varid, data); return r; } }

        public static int nc_get_var_int(int ncid, int varid, int[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_var_int(ncid, varid, data); return r; } }

        public static int nc_get_var_short(int ncid, int varid, short[] data) { lock (LockObject) { var r = NetCDFInterop.nc_get_var_short(ncid, varid, data); return r; } }

        public static int nc_get_var_float(int ncid, int varid, float[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_var_float(ncid, varid, data); return r; } }

        public static int nc_get_var_double(int ncid, int varid, double[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_var_double(ncid, varid, data); return r; } }
 
        public static int nc_put_vara(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] vp) { lock (LockObject) { var r = NetCDFInterop.nc_put_vara(ncid, varid, start, count, vp); return r; } }
        
        public static int nc_put_vara_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] tp) { lock (LockObject) { var r = NetCDFInterop.nc_put_vara_text(ncid, varid, start, count, tp); return r; } }

        public static int nc_put_vara_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, double[] dp) { lock (LockObject){ var r = NetCDFInterop.nc_put_vara_double(ncid, varid, start, count, dp); return r; } }

        public static int nc_put_vara_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, float[] fp) { lock (LockObject){ var r = NetCDFInterop.nc_put_vara_float(ncid, varid, start, count, fp); return r; } }

        public static int nc_put_vara_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, int[] ip) { lock (LockObject){ var r = NetCDFInterop.nc_put_vara_int(ncid, varid, start, count, ip); return r; } }
        
        public static int nc_put_vara_short(int ncid, int varid, IntPtr[] start, IntPtr[] count, short[] ip) { lock (LockObject) { var r = NetCDFInterop.nc_put_vara_short(ncid, varid, start, count, ip); return r; } }

        public static int nc_get_vara(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_vara(ncid, varid, start, count, data); return r; } }
        
        public static int nc_get_vara_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_vara_text(ncid, varid, start, count, data); return r; } }

        public static int nc_get_vara_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, int[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_vara_int(ncid, varid, start, count, data); return r; } }

        public static int nc_get_vara_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, float[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_vara_float(ncid, varid, start, count, data); return r; } }
        
        public static int nc_get_vara_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, double[] data) { lock (LockObject){ var r = NetCDFInterop.nc_get_vara_double(ncid, varid, start, count, data); return r; } }

        public static int nc_get_vars(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, byte[] data) { lock (LockObject) { var r = NetCDFInterop.nc_get_vars(ncid, varid, start, count, stride, data); return r; } }

        public static int nc_get_vars_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, byte[] data) { lock (LockObject) { var r = NetCDFInterop.nc_get_vars_text(ncid, varid, start, count, stride, data); return r; } }

        public static int nc_get_vars_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, int[] data) { lock (LockObject) { var r = NetCDFInterop.nc_get_vars_int(ncid, varid, start, count, stride, data); return r; } }

        public static int nc_get_vars_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, float[] data) { lock (LockObject) { var r = NetCDFInterop.nc_get_vars_float(ncid, varid, start, count, stride, data); return r; } }

        public static int nc_get_vars_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, double[] data) { lock (LockObject) { var r = NetCDFInterop.nc_get_vars_double(ncid, varid, start, count, stride, data); return r; } }
 
        public static string nc_strerror(int ncerror)
        {
            switch (ncerror)
            {
                case (0): return "No error";
                case (-1): return "Returned for all errors in the v2 API";
                case (-33): return "Not a netcdf id";
                case (-34): return "Too many netcdfs open";
                case (-35): return "netcdf file exists && NC_NOCLOBBER";
                case (-36): return "Invalid Argument";
                case (-37): return "Write to read only";
                case (-38): return "Operation not allowed in data mode";
                case (-39): return "Operation not allowed in define mode";
                case (-40): return "Index exceeds dimension bound. Consider cloning the file into NetCDF 4 format to enable data extdending";
                case (-41): return "NC_MAX_DIMS exceeded";
                case (-42): return "String match to name in use";
                case (-43): return "Attribute not found";
                case (-44): return "NC_MAX_ATTRS exceeded";
                case (-45): return "Not a netcdf data type. Some types are not supported by the classic NetCDF format. Consider cloning the file into NetCDF 4 format to enable use of all supported types";
                case (-46): return "Invalid dimension id or name";
                case (-47): return "NC_UNLIMITED in the wrong index";
                case (-48): return "NC_MAX_VARS exceeded";
                case (-49): return "Variable not found";
                case (-50): return "Action prohibited on NC_GLOBAL varid";
                case (-51): return "Not a netcdf file";
                case (-52): return "In Fortran, string too short";
                case (-53): return "NC_MAX_NAME exceeded";
                case (-54): return "NC_UNLIMITED size already in use";
                case (-55): return "nc_rec op when there are no record vars";
                case (-56): return "Attempt to convert between text & numbers";
                case (-57): return "Start+count exceeds dimension bound";
                case (-58): return "Illegal stride";
                case (-59): return "Attribute or variable name contains illegal characters";
                case (-60): return "Math result not representable";
                case (-61): return "Memory allocation (malloc) failure";
                case (-62): return "One or more variable sizes violate format constraints";
                case (-63): return "Invalid dimension size";
                case (-64): return "File likely truncated or possibly corrupted";
                case (-65): return "Unknown axis type.";
                // DAP errors
                case (-66): return "Generic DAP error";
                case (-67): return "Generic libcurl error";
                case (-68): return "Generic IO error";
                // netcdf-4 errors
                case (-100): return "NetCDF4 error";
                case (-101): return "Error at HDF5 layer.";
                case (-102): return "Can't read.";
                case (-103): return "Can't write.";
                case (-104): return "Can't create.";
                case (-105): return "Problem with file metadata.";
                case (-106): return "Problem with dimension metadata.";
                case (-107): return "Problem with attribute metadata.";
                case (-108): return "Problem with variable metadata.";
                case (-109): return "Not a compound type.";
                case (-110): return "Attribute already exists.";
                case (-111): return "Attempting netcdf-4 operation on netcdf-3 file.";
                case (-112): return "Attempting netcdf-4 operation on strict nc3 netcdf-4 file.";
                case (-113): return "Attempting netcdf-3 operation on netcdf-4 file.";
                case (-114): return "Parallel operation on file opened for non-parallel access.";
                case (-115): return "Error initializing for parallel access.";
                case (-116): return "Bad group ID.";
                case (-117): return "Bad type ID.";
                case (-118): return "Type has already been defined and may not be edited.";
                case (-119): return "Bad field ID.";
                case (-120): return "Bad class.";
                case (-121): return "Mapped access for atomic types only.";
                case (-122): return "Attempt to define fill value when data already exists.";
                case (-123): return "Attempt to define var properties, like deflate, after enddef.";
                case (-124): return "Probem with HDF5 dimscales.";
                case (-125): return "No group found.";
                case (-126): return "Can't specify both contiguous and chunking.";
                case (-127): return "Bad chunksize.";
                case (-128): return "NetCDF4 error";
                default: return "NetCDF error " + ncerror;
            }
        }
    }

    internal static class NetCDFInterop
    {
        static NetCDFInterop()
        {
            var dir = Path.GetDirectoryName(typeof(NetCDFInterop).Assembly.Location);
            NativeLibrary.LoadNativeDllForCurrentPlatform("netcdf-4.3.dll", dir);
        }

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_open(string path, NetCdfWrapper.CreateMode mode, out int ncidp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_create(string path, NetCdfWrapper.CreateMode mode, out int ncidp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_close(int ncidp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_sync(int ncid);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_redef(int ncid);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_enddef(int ncid);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_def_var(int ncid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, NetCdfDataType xtype, int ndims, int[] dimids, out int varidp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_set_fill(int ncid, int fillmode, out int oldmodep);
        
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_nvars(int ncid, out int nvars);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_ndims(int ncid, out int ndims);

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_vartype(int ncid, int varid, out NetCdfDataType xtypep);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_varnatts(int ncid, int varid, out int nattsp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_varname(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] StringBuilder name);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_varndims(int ncid, int varid, out int ndims);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_vardimid(int ncid, int varid, int[] dimids);

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_unlimdim(int ncid, out int unlimdimid);

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_attname(int ncid, int varid, int attnum, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] StringBuilder name);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_att(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, out NetCdfDataType type, out IntPtr length);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_att_text(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, IntPtr value);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_att_int(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, int[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_att_float(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, float[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_att_double(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, double[] data);

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_att_text(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, IntPtr len, IntPtr tp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_att_double(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, NetCdfDataType type, IntPtr len, double[] tp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_att_int(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, NetCdfDataType type, IntPtr len, int[] tp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_att_float(int ncid, int varid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, NetCdfDataType type, IntPtr len, float[] tp); 
        
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_def_dim(int ncid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, IntPtr len, out int dimidp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_dimname(int ncid, int dimid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] StringBuilder name); 
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_dimid(int ncid, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string name, out int dimid);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_inq_dimlen(int ncid, int dimid, out IntPtr length);

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_var(int ncid, int varid, byte[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_var_text(int ncid, int varid, byte[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_var_int(int ncid, int varid, int[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_var_short(int ncid, int varid, short[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_var_float(int ncid, int varid, float[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_var_double(int ncid, int varid, double[] data);

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_vara(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] tp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_vara_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] tp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_vara_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, double[] dp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_vara_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, float[] fp);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_vara_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, int[] ip);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_put_vara_short(int ncid, int varid, IntPtr[] start, IntPtr[] count, short[] ip);

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vara(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vara_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vara_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, int[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vara_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, float[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vara_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, double[] data);

        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vars(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, byte[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vars_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, byte[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vars_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, int[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vars_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, float[] data);
        [DllImport("netcdf-4.3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nc_get_vars_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, double[] data);
    }

}
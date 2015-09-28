// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)

using System;

namespace DelftTools.Utils.NetCdf
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NetCdfWrapper
    {
        /// <summary>
        /// 'size' argument to ncdimdef for an unlimited dimension
        /// </summary>
        public const int NC_UNLIMITED = 0;

        /// <summary>
        /// Variable id to put/get a global attribute
        /// </summary>
        public const int NC_GLOBAL = -1;

        // In HDF5 files you can set storage for each variable to be either
        // contiguous or chunked, with nc_def_var_chunking()
        public const int NC_CHUNKED = 0;
        public const int NC_CONTIGUOUS = 1;

        public static Type GetClrDataType(NetCdfDataType type)
        {
            if (type == NetCdfDataType.NC_BYTE)
            {
                return typeof (byte);
            }
            if (type == NetCdfDataType.NC_CHAR)
            {
                return typeof(char);
            }
            if (type == NetCdfDataType.NC_DOUBLE)
            {
                return typeof(double);
            }
            if (type == NetCdfDataType.NC_INT)
            {
                return typeof(int);
            }
            if (type == NetCdfDataType.NC_INT64)
            {
                return typeof(long);
            }
            if (type == NetCdfDataType.NC_FLOAT)
            {
                return typeof(float);
            }
            if (type == NetCdfDataType.NC_SHORT)
            {
                return typeof(Int16);
            }
            throw new NotSupportedException(string.Format("NetCDF type {0} cannot be converted to CLR type", type));
        }

        public static NetCdfDataType GetNetCdf3DataType(Type type)
        {
            if (type == typeof(double))
            {
                return NetCdfDataType.NC_DOUBLE;
            }
            if (type == typeof(float))
            {
                return NetCdfDataType.NC_FLOAT;
            }
            if (type == typeof(int))
            {
                return NetCdfDataType.NC_INT;;
            }
            if (type == typeof(DateTime))
            {
                return NetCdfDataType.NC_DOUBLE;
            }
            if (type == typeof(Int16))
            {
                return NetCdfDataType.NC_SHORT;
            }
            if (type == typeof(byte))
            {
                return NetCdfDataType.NC_BYTE;
            }
            if (type == typeof(char))
            {
                return NetCdfDataType.NC_CHAR;
            }
            throw new NotSupportedException(string.Format("Type is not supported by NetCdf: {0}", type));
        }

        /// <summary>
        /// 
        /// </summary>
        public enum CreateMode : int
        {
            NC_NOWRITE = 0,
            /// <summary>read &amp; write</summary>
            NC_WRITE = 0x0001,
            NC_CLOBBER = 0,
            /// <summary>Don't destroy existing file on create</summary>
            NC_NOCLOBBER = 0x0004,
            /// <summary>argument to ncsetfill to clear NC_NOFILL</summary>
            NC_FILL = 0,
            /// <summary>Don't fill data section an records</summary>
            NC_NOFILL = 0x0100,
            /// <summary>Use locking if available</summary>
            NC_LOCK = 0x0400,
            /// <summary>Share updates, limit cacheing</summary>
            NC_SHARE = 0x0800,
            NC_64BIT_OFFSET = 0x0200,
            /// <summary>Enforce strict netcdf-3 rules</summary>
            NC_CLASSIC = 0x0100,
            /// <summary>causes netCDF to create a HDF5/NetCDF-4 file</summary>
            NC_NETCDF4 = 0x1000
        }

        /// <summary>
        /// Error codes taken from 'Appendix D: NetCDF-3 Error Codes' (NetCDF Version 4.1.3)
        /// </summary>
        public enum ResultCode : int
        {
            NC_NOERR = 0, /* No Error */
            NC_EBADID = -33, /* Not a netcdf id */
            NC_ENFILE = -34, /* Too many netcdfs open */
            NC_EEXIST = -35, /* netcdf file exists && NC_NOCLOBBER */
            NC_EINVAL = -36, /* Invalid Argument */
            NC_EPERM = -37, /* Write to read only */
            NC_ENOTINDEFINE = -38, /* Operation not allowed in data mode */
            NC_EINDEFINE = -39, /* Operation not allowed in define mode */
            NC_EINVALCOORDS = -40, /* Index exceeds dimension bound */
            NC_EMAXDIMS = -41, /* NC_MAX_DIMS exceeded */
            NC_ENAMEINUSE = -42, /* String match to name in use */
            NC_ENOTATT = -43, /* Attribute not found */
            NC_EMAXATTS = -44, /* NC_MAX_ATTRS exceeded */
            NC_EBADTYPE = -45, /* Not a netcdf data type */
            NC_EBADDIM = -46, /* Invalid dimension id or name */
            NC_EUNLIMPOS = -47, /* NC_UNLIMITED in the wrong index */
            NC_EMAXVARS = -48, /* NC_MAX_VARS exceeded */
            NC_ENOTVAR = -49, /* Variable not found */
            NC_EGLOBAL = -50, /* Action prohibited on NC_GLOBAL varid */
            NC_ENOTNC = -51, /* Not a netcdf file */
            NC_ESTS = -52, /* In Fortran, string too short */
            NC_EMAXNAME = -53, /* NC_MAX_NAME exceeded */
            NC_EUNLIMIT = -54, /* NC_UNLIMITED size already in use */
            NC_ENORECVARS = -55, /* nc_rec op when there are no record vars */
            NC_ECHAR = -56, /* Attempt to convert between text & numbers */
            NC_EEDGE = -57, /* Edge+start exceeds dimension bound */
            NC_ESTRIDE = -58, /* Illegal stride */
            NC_EBADNAME = -59, /* Attribute or variable name contains illegal characters */
            NC_ERANGE = -60, /* Math result not representable */
            NC_ENOMEM = -61, /* Memory allocation = malloc, failure */
            NC_EVARSIZE = -62, /* One or more variable sizes violate format constraints */
            NC_EDIMSIZE = -63, /* Invalid dimension size */
            NC_ETRUNC = -64 /* File likely truncated or possibly corrupted */
        }


        /// <summary>
        ///	Default fill values, used unless _FillValue attribute is set.
        /// These values are stuffed into newly allocated space as appropriate.
        /// The hope is that one might use these to notice that a particular datum
        /// has not been set.
        /// </summary>
        public static class FillValues
        {
            public const byte NC_FILL_BYTE = 255;
            public const char NC_FILL_CHAR = (char)0;
            public const short NC_FILL_SHORT = -32767;
            public const int NC_FILL_INT = -2147483647;
            public const float NC_FILL_FLOAT = 9.96921E+36f;    /* near 15 * 2^119 */
            public const double NC_FILL_DOUBLE = 9.969209968386869E+36;
        }


        ///<summary>These maximums are enforced by the interface, to facilitate writing
        ///applications and utilities.  However, nothing is statically allocated to
        ///these sizes internally.</summary>
        public enum Limits
        {
            /// <summary>max dimensions per file </summary>
            NC_MAX_DIMS = 10,
            /// <summary>max global or per variable attributes </summary>
            NC_MAX_ATTRS = 2000,
            /// <summary>max variables per file</summary>
            NC_MAX_VARS = 2000,
            /// <summary>max length of a name </summary>
            NC_MAX_NAME = 128,
            /// <summary>max per variable dimensions </summary>
            NC_MAX_VAR_DIMS = 10
        }
    }
}


// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
namespace DelftTools.Utils.NetCdf
{
    /// <summary>
    /// The netcdf external data types
    /// </summary>
    public enum NetCdfDataType : int
    {
        /// <summary>signed 1 byte integer</summary>
        NC_BYTE = 1,
        /// <summary>unsigned 1 byte int</summary>
        NC_UBYTE = 7,
        /// <summary>ISO/ASCII character</summary>
        NC_CHAR = 2,
        /// <summary>signed 2 byte integer</summary>
        NC_SHORT = 3,
        /// <summary>unsigned 2 byte integer</summary>
        NC_USHORT = 8,
        /// <summary>signed 4 byte integer</summary>
        NC_INT = 4,
        /// <summary>unsigned 4-byte int </summary>
        NC_UINT = 9,
        /// <summary>single precision floating point number</summary>
        NC_FLOAT = 5,
        /// <summary>double precision floating point number</summary>
        NC_DOUBLE = 6,
        /// <summary>signed 8-byte int</summary>
        NC_INT64 = 10,
        /// <summary>unsigned 8-byte int</summary>
        NC_UINT64 = 11,
        /// <summary>string</summary>
        NC_STRING = 12
    }
}
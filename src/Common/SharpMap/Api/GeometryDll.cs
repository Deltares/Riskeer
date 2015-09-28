using System;
using System.IO;
using System.Runtime.InteropServices;
using DelftTools.Utils.Interop;

namespace SharpMap.Api
{
    public static class GeometryDll
    {
        static GeometryDll()
        {
            var dir = Path.GetDirectoryName(typeof(GeometryDll).Assembly.Location);
            NativeLibrary.LoadNativeDllForCurrentPlatform("geometry.dll", dir);
        }

        [DllImport("geometry", EntryPoint = "triang", CallingConvention = CallingConvention.Cdecl)]
        public static extern void triangulate([In] ref IntPtr sx, [In] ref IntPtr sy, [In] ref IntPtr sv, [In] ref int numSamples, // samples
                                              [In] ref IntPtr dx, [In] ref IntPtr dy, [In] ref int numDestination, //destination points
                                              [In, Out] ref IntPtr values); //triangulated result values

        [DllImport("geometry", EntryPoint = "averaging", CallingConvention = CallingConvention.Cdecl)]
        public static extern void averaging([In] ref IntPtr sx, [In] ref IntPtr sy, [In] ref IntPtr sv, [In] ref int numSamples, // samples
                                            [In] ref IntPtr cx, [In] ref IntPtr cy, [In] ref IntPtr cxx, [In] ref IntPtr cyy, [In] ref IntPtr ccn, [In] ref int ncells, [In] ref int ncor, // cell center/corner information
                                            [In, Out] ref IntPtr values,  //averaged result values
                                            [In] ref int method, [In] ref int nsmin, [In] ref double rcel); //settings

        [DllImport("geometry", EntryPoint = "find_cells", CallingConvention = CallingConvention.Cdecl)]
        public static extern void find_cells([In] string netFilePath, [Out] out int numCells, [Out] out int maxPerCell, [In, Out] ref IntPtr netElemNode);
    }
}

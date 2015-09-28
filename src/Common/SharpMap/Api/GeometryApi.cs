using System;
using System.Linq;
using System.Runtime.InteropServices;
using DelftTools.Utils.Interop;

namespace SharpMap.Api
{
    /// <summary>
    /// TODO: currently, the dflowfm core is not prepared for non-remote usage... please check with them before using this!!
    /// </summary>
    public class LocalGeometryApi : IGeometryApi
    {
        public double[] Triangulate(double[] sourceX,
                                    double[] sourceY,
                                    double[] sourceZ,
                                    double[] targetX,
                                    double[] targetY)
        {
            if (sourceZ.Length != sourceX.Length || sourceZ.Length != sourceY.Length)
            {
                throw new ArgumentException("sample component arrays have different lengths");
            }
            var numSamples = sourceZ.Length;
            if (targetX.Length != targetY.Length)
            {
                throw new ArgumentException("target component arrays have different lengths");
            }

            var numNodes = targetX.Length;
            var valuesDouble = new double[numNodes];

            using (var pinnedSx = new Pinned(sourceX))
            using (var pinnedSy = new Pinned(sourceY))
            using (var pinnedSValues = new Pinned(sourceZ))
            using (var pinnedDx = new Pinned(targetX))
            using (var pinnedDy = new Pinned(targetY))
            using (var pinnedDres = new Pinned(valuesDouble))
            {
                var ptrSx = pinnedSx.IntPtr;
                var ptrSy = pinnedSy.IntPtr;
                var ptrSValues = pinnedSValues.IntPtr;
                var ptrDx = pinnedDx.IntPtr;
                var ptrDy = pinnedDy.IntPtr;
                var ptrDres = pinnedDres.IntPtr;

                GeometryDll.triangulate(ref ptrSx, ref ptrSy, ref ptrSValues, ref numSamples, ref ptrDx, ref ptrDy, ref numNodes, ref ptrDres);
            }

            return valuesDouble;
        }

        public double[] Averaging(double[] sourceX, 
                                  double[] sourceY, 
                                  double[] sourceZ, 
                                  double[] targetX,
                                  double[] targetY, 
                                  double[,] cellcornersX, 
                                  double[,] cellcornersY, 
                                  int[] numCellCorners,
                                  int method = 1, 
                                  int nsmin = 1, 
                                  double rcel = 1.0)
        {
            if (sourceZ.Length != sourceX.Length || sourceZ.Length != sourceY.Length)
            {
                throw new ArgumentException("sample component arrays have different lengths");
            }

            var numSamples = sourceZ.Length;
            
            if (targetX.Length != targetY.Length)
            {
                throw new ArgumentException("target component arrays have different lengths");
            }

            if (cellcornersX.GetLength(0) != cellcornersY.GetLength(0) ||
                cellcornersX.GetLength(1) != cellcornersY.GetLength(1) ||
                cellcornersX.GetLength(0) != numCellCorners.Length)
            {
                throw new ArgumentException("cell corner arrays have non-matching lengths");
            }

            var numCells = numCellCorners.Length;
            var maxCorners = cellcornersX.GetLength(1);

            var flattenedCellCornersX = cellcornersX.Cast<double>().ToArray();
            var flattenedCellCornersY = cellcornersY.Cast<double>().ToArray();

            var targetZ = new double[targetX.Length];

            using (var pinnedSx = new Pinned(sourceX))
            using (var pinnedSy = new Pinned(sourceY))
            using (var pinnedSz = new Pinned(sourceZ))
            using (var pinnedTx = new Pinned(targetX))
            using (var pinnedTy = new Pinned(targetY))
            using (var pinnedTz = new Pinned(targetZ))
            using (var pinnedCcx = new Pinned(flattenedCellCornersX))
            using (var pinnedCcy = new Pinned(flattenedCellCornersY))
            using (var pinnedNcc = new Pinned(numCellCorners))
            {
                var ptrSx = pinnedSx.IntPtr;
                var ptrSy = pinnedSy.IntPtr;
                var ptrSz = pinnedSz.IntPtr;
                var ptrTx = pinnedTx.IntPtr;
                var ptrTy = pinnedTy.IntPtr;
                var ptrTz = pinnedTz.IntPtr;
                var ptrCcx = pinnedCcx.IntPtr;
                var ptrCcy = pinnedCcy.IntPtr;
                var ptrNcc = pinnedNcc.IntPtr;
                var meth = method;
                var nmin = nsmin;
                var csize = rcel;

                GeometryDll.averaging(ref ptrSx, ref ptrSy, ref ptrSz, ref numSamples, ref ptrTx, ref ptrTy, ref ptrCcx, ref ptrCcy,
                    ref ptrNcc, ref numCells, ref maxCorners, ref ptrTz, ref meth, ref nmin, ref csize);
            }
            return targetZ;
        }


        public int[] FindCells(string netFilePath, out int numCells)
        {
            var ptr = new IntPtr();
            int maxPerCell;

            GeometryDll.find_cells(netFilePath, out numCells, out maxPerCell, ref ptr);

            var totalSize = numCells * maxPerCell;
            var tmp = new int[numCells * maxPerCell];
            Marshal.Copy(ptr, tmp, 0, totalSize);

            return tmp;
        }
    }

}
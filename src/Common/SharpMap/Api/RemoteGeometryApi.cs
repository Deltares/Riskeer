using System;
using DelftTools.Utils.Remoting;

namespace SharpMap.Api
{
    public class RemoteGeometryApi : IGeometryApi, IDisposable
    {
        private IGeometryApi remoteApi;

        public RemoteGeometryApi()
        {
            remoteApi = RemoteInstanceContainer.CreateInstance<IGeometryApi, LocalGeometryApi>();

            // local (debug?) usage:
            //remoteApi = new LocalGeometryApi();
        }

        public double[] Triangulate(double[] sourceX, double[] sourceY, double[] sourceZ, double[] targetX, double[] targetY)
        {
            return remoteApi.Triangulate(sourceX, sourceY, sourceZ, targetX, targetY);
        }

        public double[] Averaging(double[] sourceX, double[] sourceY, double[] sourceZ, double[] targetX, double[] targetY,
            double[,] cellcornersX, double[,] cellcornersY, int[] numCellCorners, int method = 1, int nsmin = 1, double rcel = 1)
        {
            return remoteApi.Averaging(sourceX, sourceY, sourceZ, targetX, targetY, cellcornersX, cellcornersY,
                numCellCorners, method, nsmin, rcel);
        }

        public int[] FindCells(string netFilePath, out int numCells)
        {
            return remoteApi.FindCells(netFilePath, out numCells);
        }

        public void Dispose()
        {
            if (remoteApi != null)
                RemoteInstanceContainer.RemoveInstance(remoteApi);
            remoteApi = null;
        }
    }
}
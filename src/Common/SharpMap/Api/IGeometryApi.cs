namespace SharpMap.Api
{
    public interface IGeometryApi
    {
        double[] Triangulate(double[] sourceX,
            double[] sourceY,
            double[] sourceZ,
            double[] targetX,
            double[] targetY);

        double[] Averaging(double[] sourceX,
            double[] sourceY,
            double[] sourceZ,
            double[] targetX,
            double[] targetY,
            double[,] cellcornersX,
            double[,] cellcornersY,
            int[] numCellCorners,
            int method = 1,
            int nsmin = 1,
            double rcel = 1.0);

        int[] FindCells(string netFilePath, out int numCells);
    }
}